using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<DateTime, List<string>> contentsList = new Dictionary<DateTime, List<string>>();


        private readonly string PHOTO_PATH = @"C:\Users\sungchul\Dropbox\Apps\Day One\Journal.dayone\photos";

        private readonly string ENTRY_PATH = @"C:\Users\sungchul\Dropbox\Apps\Day One\Journal.dayone\entries";

        private readonly string ENTRY_TEMPLATE;

        private MarkdownDeep.Markdown markdownProcessor;

        public MainWindow()
        {
            InitializeComponent();

            canlendarViewer.DayItemSelected += CanlendarViewerDayItemSelected;
            var assembly = Assembly.GetExecutingAssembly();
            ENTRY_TEMPLATE = new StreamReader(assembly.GetManifestResourceStream("DayOne.NET.ContetnsViewer.html")).ReadToEnd();

            markdownProcessor = new MarkdownDeep.Markdown {
                SafeMode = false,
                ExtraMode = true,
                AutoHeadingIDs = true,
                MarkdownInHtml = true,
                NewWindowForExternalLinks = true
            };

            // Temp            
            LoadDateContentsDateTime(ENTRY_PATH);
            canlendarViewer.InitializeCalendar(contentsList);
            contentsItemViewer.InitializeItemViewer(contentsList);
        }
        

        private string GetHtmlContents(IEnumerable<string> entries)
        {
            var images = Directory.GetFiles(PHOTO_PATH, "*.jpg", SearchOption.TopDirectoryOnly);

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
                            var imagePath = PHOTO_PATH + System.IO.Path.DirectorySeparatorChar + entry.UUID + ".jpg";
                            var imageTag = string.Format(imgTagFormat, imagePath);
                            html = imageTag + html;
                        }

                        return html;
                    }).
                Aggregate((e1, e2) => e1 + @"<hr />" + e2);
            

            return ENTRY_TEMPLATE.Replace("##HTML##", contetsHtml);
        }

        private void CanlendarViewerDayItemSelected(object sender, DayItemSelectedArgs e)
        {
            if (e.SelectedUUIDs != null) {
                var entries = GetEntryList(e.SelectedUUIDs);                
                //contentsViewer.LoadContents(GetHtmlContents(entries));

                var html = GetHtmlContents(entries);
                File.WriteAllText("contents.html", html);
                var uri = @"file:///" + Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "contents.html";
                contentsViewer.LoadContentsFromUri(uri);
                //contentsViewer.LoadContents(html);

                contentsViewer.Visibility = System.Windows.Visibility.Visible;
                canlendarViewer.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private IEnumerable<string> GetEntryList(IEnumerable<string> uuids)
        {
            if (uuids != null) {
                return uuids.Select(id => ENTRY_PATH + System.IO.Path.DirectorySeparatorChar + id + ".doentry");
            }

            return null;
        }
        
        private void LoadDateContentsDateTime(string entryDir)
        {
            if (!Directory.Exists(entryDir))
                throw new DirectoryNotFoundException();

            contentsList.Clear();
            var pathList = Directory.GetFiles(entryDir, "*.doentry");
            foreach (var path in pathList) {
                Dictionary<string, object> entry = Plist.readPlist(path) as Dictionary<string, object>;
                var created = (DateTime)entry["Creation Date"];
                var createdKey = new DateTime(created.Year, created.Month, created.Day);
                var uuid = (string)entry["UUID"];

                if (!contentsList.Keys.Contains(createdKey)) {
                    contentsList.Add(createdKey, new List<string>() { uuid });
                }
                else {
                    contentsList[createdKey].Add(uuid);
                }
            }
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            contentsViewer.Visibility = System.Windows.Visibility.Collapsed;
            canlendarViewer.Visibility = System.Windows.Visibility.Collapsed;
            contentsItemViewer.Visibility = System.Windows.Visibility.Collapsed;
            
            contentsEditor.Visibility = System.Windows.Visibility.Visible;
            contentsEditor.InitilaizeContents();
        }

        private void ListButtonClick(object sender, RoutedEventArgs e)
        {
            contentsItemViewer.Visibility = System.Windows.Visibility.Visible;

            contentsEditor.Visibility = System.Windows.Visibility.Collapsed;
            contentsViewer.Visibility = System.Windows.Visibility.Collapsed;
            canlendarViewer.Visibility = System.Windows.Visibility.Collapsed; 
        }

        private void CalendarButtonClick(object sender, RoutedEventArgs e)
        {
            contentsEditor.Visibility = System.Windows.Visibility.Collapsed;
            contentsViewer.Visibility = System.Windows.Visibility.Collapsed;
            contentsItemViewer.Visibility = System.Windows.Visibility.Collapsed;

            canlendarViewer.Visibility = System.Windows.Visibility.Visible;
            canlendarViewer.GoToTodayCalendar();
        }

        private void MapButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("준비중");
        }

        private void AlarmButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("준비중");
        }

        private void OpenOptionButtonClick(object sender, RoutedEventArgs e)
        {
            var option = new OptionWindow();
            option.ShowDialog();
        }
    }
}
