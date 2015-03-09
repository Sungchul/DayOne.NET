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


        
        public MainWindow()
        {
            InitializeComponent();

            // DropBox 파일 경로가 있는지...
            ConfigManager.LoadConfig();
            if (!Directory.Exists(ConfigManager.EntryPath) || !Directory.Exists(ConfigManager.PhotoPath)) {
                MessageBox.Show("올바른 DropBox 경로를 찾을 수 없습니다.");
                
                var option = new OptionWindow();
                if (option.ShowDialog() != true) {
                    MessageBox.Show("DayOne과 연동된 DropBox 경로를 설정해야만 프로그램을 시작 할 수 있습니다.");
                    this.Close();
                }
            }

            canlendarViewer.DayItemSelected += CanlendarViewerDayItemSelected;

            // Temp            
            LoadDateContentsDateTime();
            canlendarViewer.InitializeCalendar(contentsList);
            contentsItemViewer.InitializeItemViewer(contentsList);

            contentsEditor.EditDone += (o, e) => {
                LoadDateContentsDateTime(); // Referesh
                contentsItemViewer.InitializeItemViewer(contentsList);
                ShowListView();
            };
        }
        

        //private string GetHtmlContents(IEnumerable<string> entries)
        //{
        //    var images = Directory.GetFiles(PHOTO_PATH, "*.jpg", SearchOption.TopDirectoryOnly);

        //    var HasImage = new Func<string, bool>(uuid => {                
        //        return images.FirstOrDefault(image => System.IO.Path.GetFileNameWithoutExtension(image) == uuid) != null;
        //    });

        //    var imgTagFormat = @"<img src=""{0}"" width=""100%"">";
        //    var contetsHtml = entries.Select(
        //            path => {
        //                var entry = DayOneContent.ReadContents(path);
        //                var html = markdownProcessor.Transform(entry.EntryText);
        //                if (HasImage(entry.UUID)) {
        //                    //var imagePath = @"asset://content/" + entry.UUID + ".jpg";
        //                    var imagePath = PHOTO_PATH + System.IO.Path.DirectorySeparatorChar + entry.UUID + ".jpg";
        //                    var imageTag = string.Format(imgTagFormat, imagePath);
        //                    html = imageTag + html;
        //                }

        //                return html;
        //            }).
        //        Aggregate((e1, e2) => e1 + @"<hr />" + e2);
            

        //    return ENTRY_TEMPLATE.Replace("##HTML##", contetsHtml);
        //}

        private void CanlendarViewerDayItemSelected(object sender, DayItemSelectedArgs e)
        {
            if (e.SelectedUUIDs != null) {
                //var entries = GetEntryList(e.SelectedUUIDs);
                //var html = GetHtmlContents(entries);
                //File.WriteAllText("contents.html", html);
                //var uri = @"file:///" + Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "contents.html";
                contentsViewer.LoadContentsFromUri(e.SelectedDay, e.SelectedUUIDs);

                contentsViewer.Visibility = System.Windows.Visibility.Visible;
                canlendarViewer.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //private IEnumerable<string> GetEntryList(IEnumerable<string> uuids)
        //{
        //    if (uuids != null) {
        //        return uuids.Select(id => ENTRY_PATH + System.IO.Path.DirectorySeparatorChar + id + ".doentry");
        //    }

        //    return null;
        //}
        
        private void LoadDateContentsDateTime()
        {
            if (!Directory.Exists(ConfigManager.EntryPath))
                throw new DirectoryNotFoundException();

            contentsList.Clear();
            var pathList = Directory.GetFiles(ConfigManager.EntryPath, "*.doentry");
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
            ShowListView();
        }

        private void ShowListView()
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
            
            // trick
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
