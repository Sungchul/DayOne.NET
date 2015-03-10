using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DayOne.NET
{
    /// <summary>
    /// ContentsViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContentsViewer : UserControl
    {
        private Dictionary<DateTime, List<string>> contentsList;
        private IEnumerable<DateTime> dateTimeList;

        private MarkdownDeep.Markdown markdownProcessor;

        private string ENTRY_TEMPLATE;

        public ContentsViewer()
        {
            InitializeComponent();
        }

        public void InitializeViewer(Dictionary<DateTime, List<string>> contentsList)
        {
            markdownProcessor = new MarkdownDeep.Markdown {
                SafeMode = false,
                ExtraMode = true,
                AutoHeadingIDs = true,
                MarkdownInHtml = true,
                NewWindowForExternalLinks = true
            };

            var assembly = Assembly.GetExecutingAssembly();
            ENTRY_TEMPLATE =
                new StreamReader(assembly.GetManifestResourceStream("DayOne.NET.ContetnsViewer.html")).ReadToEnd();

            this.contentsList = contentsList;
            this.dateTimeList = contentsList.Keys.OrderBy(x => x);
        }

        #region DependencyProperty

        public static readonly DependencyProperty YearNumProperty =
            DependencyProperty.Register("Year", typeof(string), typeof(ContentsViewer));

        public string Year
        {
            get { return (string)GetValue(YearNumProperty); }
            set { SetValue(YearNumProperty, value); }
        }

        public static readonly DependencyProperty MonthNameProperty =
            DependencyProperty.Register("MonthName", typeof(string), typeof(ContentsViewer));

        public string MonthName
        {
            get { return (string)GetValue(MonthNameProperty); }
            set { SetValue(MonthNameProperty, value); }
        }

        public static readonly DependencyProperty DayNumProperty =
            DependencyProperty.Register("Day", typeof(string), typeof(ContentsViewer));

        public string Day
        {
            get { return (string)GetValue(DayNumProperty); }
            set { SetValue(DayNumProperty, value); }
        }

        public static readonly DependencyProperty DayNameProperty =
            DependencyProperty.Register("DayName", typeof(string), typeof(ContentsViewer));

        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(ContentsViewer));

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty IsHavePreviousItemProperty =
            DependencyProperty.Register("IsHavePreviousItem", typeof(bool), typeof(ContentsViewer));

        public bool IsHavePreviousItem
        {
            get { return (bool)GetValue(IsHavePreviousItemProperty); }
            set { SetValue(IsHavePreviousItemProperty, value); }
        }

        public static readonly DependencyProperty IsHaveNextItemProperty =
            DependencyProperty.Register("IsHaveNextItem", typeof(bool), typeof(ContentsViewer));

        public bool IsHaveNextItem
        {
            get { return (bool)GetValue(IsHaveNextItemProperty); }
            set { SetValue(IsHaveNextItemProperty, value); }
        }

        #endregion

        private static string AbbreviatedDayName(DateTime datetime)
        {
            // 영어로 고정
            var english = new System.Globalization.CultureInfo("en-US");
            string[] names = english.DateTimeFormat.AbbreviatedDayNames;

            return names[(int)datetime.DayOfWeek].ToUpper();
        }

        private static string AbbreviatedMonthName(DateTime datetime)
        {
            // 영어로 고정
            var english = new System.Globalization.CultureInfo("en-US");
            string[] names = english.DateTimeFormat.AbbreviatedMonthNames;

            return names[(int)datetime.Month].ToUpper();
        }

        public void InitilaizeSession(string contentsPath)
        {
            var prefs = new WebPreferences();
            var session = WebCore.CreateWebSession(prefs);
            session.AddDataSource("content", new Awesomium.Core.Data.DirectoryDataSource(contentsPath));
            htmlRenderer.WebSession = session;
        }

        public void LoadContents(string html)
        { 
            htmlRenderer.LoadHTML(html);
        }

        private void UpdateDateTime(DateTime datetime)
        {
            currentDateTime = datetime;

            Year = datetime.Year.ToString();
            MonthName = AbbreviatedMonthName(datetime);
            Day = datetime.Day.ToString("D2");
            DayName = AbbreviatedDayName(datetime);
            Time = DateTime.Now.ToString("hh:mm tt", new System.Globalization.CultureInfo("en-US"));
        }

        public void LoadContentsFromUri(DateTime dateTime, IEnumerable<string> selectedUUIDs)
        {
            UpdateDateTime(dateTime);

            var entries = selectedUUIDs.Select(id => ConfigManager.EntryPath + System.IO.Path.DirectorySeparatorChar + id + ".doentry");
            var html = GetHtmlContents(entries);
            File.WriteAllText("contents.html", html);
            var uri = @"file:///" + Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "contents.html";

            htmlRenderer.Source = new Uri(uri);
            htmlRenderer.Reload(true);

            IsHaveNextItem = HasNextItem(dateTime);
            IsHavePreviousItem = HasPreviousItem(dateTime);
        }     

        private string GetHtmlContents(IEnumerable<string> entries)
        {
            var images = Directory.GetFiles(ConfigManager.PhotoPath, "*.jpg", SearchOption.TopDirectoryOnly);

            var HasImage = new Func<string, bool>(uuid => {
                return images.FirstOrDefault(image => System.IO.Path.GetFileNameWithoutExtension(image) == uuid) != null;
            });

            var imgTagFormat = @"<img src=""{0}"" width=""100%"">";
            var contetsHtml = entries.Select(
                    path => {
                        var entry = DayOneContent.ReadContents(path);
                        var html = markdownProcessor.Transform(entry.EntryText);
                        if (HasImage(entry.UUID)) {
                            //var imagePath = @"asset://content/" + entry.UUID + ".jpg";
                            var imagePath = ConfigManager.PhotoPath + System.IO.Path.DirectorySeparatorChar + entry.UUID + ".jpg";
                            var imageTag = string.Format(imgTagFormat, imagePath);
                            html = imageTag + html;
                        }

                        return html;
                    }).
                Aggregate((e1, e2) => e1 + @"<hr />" + e2);

            return ENTRY_TEMPLATE.Replace("##HTML##", contetsHtml);
        }

        private bool HasPreviousItem(DateTime current)
        {
            if (dateTimeList != null)
                return dateTimeList.Count(k => k < current) > 0;

            return false;
        }

        private bool HasNextItem(DateTime current)
        {
            if (dateTimeList != null)
                return dateTimeList.Count(k => k > current) > 0;

            return false;
        }

        private DateTime currentDateTime;

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedKey = dateTimeList.Reverse().FirstOrDefault(k => k < currentDateTime);
            if (selectedKey != DateTime.MinValue) {
                var items = contentsList[selectedKey];
                LoadContentsFromUri(selectedKey, items);
            }
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedKey = dateTimeList.FirstOrDefault(k => k > currentDateTime);
            if (selectedKey != DateTime.MinValue) {
                var items = contentsList[selectedKey];
                LoadContentsFromUri(selectedKey, items);
            }
        }
    }
}
