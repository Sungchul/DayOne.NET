using CommonMark;
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

        public ContentsViewer()
        {
            InitializeComponent();
        }

        public void InitializeViewer(Dictionary<DateTime, List<string>> contentsList)
        {   
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
            
            var images = Directory.GetFiles(Properties.Settings.Default.PhotoPath, "*.jpg", SearchOption.TopDirectoryOnly);

            var HasImage = new Func<string, bool>(uuid => {
                return images.FirstOrDefault(image => System.IO.Path.GetFileNameWithoutExtension(image) == uuid) != null;
            });

            var entries = selectedUUIDs.Select(id => Properties.Settings.Default.EntryPath + System.IO.Path.DirectorySeparatorChar + id + ".doentry");
            
            var flowDocument = new FlowDocument();
            foreach (var uuid in selectedUUIDs) {
                if (flowDocument.Blocks.Count > 0) {
                    var separator = new Rectangle();
                    separator.Stroke = new SolidColorBrush(Colors.LightGray);
                    separator.StrokeThickness = 3;
                    separator.Height = 2;
                    separator.Width = double.NaN;

                    var lineBlock = new BlockUIContainer(separator);
                    flowDocument.Blocks.Add(lineBlock);
                }

                var image = images.FirstOrDefault(x => System.IO.Path.GetFileNameWithoutExtension(x) == uuid);
                if (image != null) {
                    var picture = new Image();
                    picture.Source = new BitmapImage(new Uri(image, UriKind.Absolute));
                    var uiContainer = new BlockUIContainer(picture);

                    flowDocument.Blocks.Add(uiContainer);
                }

                var entry = Properties.Settings.Default.EntryPath + System.IO.Path.DirectorySeparatorChar + uuid + ".doentry";
                var content = DayOneContent.ReadContents(entry);
                var ast = CommonMarkConverter.Parse(content.EntryText);
                FlowDocumentFormatter.BlocksToFlowDocument(ast, flowDocument);
            }

            _viewer.Document = flowDocument;
            
            IsHaveNextItem = HasNextItem(dateTime);
            IsHavePreviousItem = HasPreviousItem(dateTime);
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

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {   
        }
    }
}
