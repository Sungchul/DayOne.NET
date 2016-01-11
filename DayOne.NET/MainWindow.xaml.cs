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

            if (!Directory.Exists(Properties.Settings.Default.EntryPath) || !Directory.Exists(Properties.Settings.Default.PhotoPath)) {
                MessageBox.Show("올바른 DropBox 경로를 찾을 수 없습니다.");
                
                var option = new OptionWindow();
                if (option.ShowDialog() != true) {
                    MessageBox.Show("DayOne과 연동된 DropBox 경로를 설정해야만 프로그램을 시작 할 수 있습니다.");
                    this.Close();
                }
            }

            if (Properties.Settings.Default.UsePassword)
                _passwordTab.IsSelected = true;

            canlendarViewer.DayItemSelected += CanlendarViewerDayItemSelected;

            // Temp            
            LoadDateContentsDateTime();
            canlendarViewer.InitializeCalendar(contentsList);
            contentsItemViewer.InitializeItemViewer(contentsList);
            contentsItemViewer.ItemEditRequest += (o, e) => ShowEditWindowWithContents(e.UUID);
            contentsViewer.InitializeViewer(contentsList);

            contentsEditor.EditDone += (o, e) => {
                LoadDateContentsDateTime(); // Referesh
                contentsItemViewer.InitializeItemViewer(contentsList);
                contentsViewer.InitializeViewer(contentsList);
                ShowListPage();
            };
        }

        private void CanlendarViewerDayItemSelected(object sender, DayItemSelectedArgs e)
        {
            if (e.SelectedUUIDs != null) {
                contentsViewer.LoadContentsFromUri(e.SelectedDay, e.SelectedUUIDs);
                _contentsViewerTab.IsSelected = true;
            }
        }
        
        private void LoadDateContentsDateTime()
        {
            if (!Directory.Exists(Properties.Settings.Default.EntryPath))
                throw new DirectoryNotFoundException();

            contentsList.Clear();
            var pathList = Directory.GetFiles(Properties.Settings.Default.EntryPath, "*.doentry");
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

        private void ShowEditWindowWithContents(string uuid)
        {
            _editorViewTab.IsSelected = true;
            var path = Properties.Settings.Default.EntryPath + System.IO.Path.DirectorySeparatorChar + uuid + ".doentry";
            contentsEditor.LoadContents(DayOneContent.ReadContents(path));
        }

        private void OpenOptionButtonClick(object sender, RoutedEventArgs e)
        {
            var option = new OptionWindow();
            option.ShowDialog();
        }

        private void ShowNewPage()
        {
            _editorViewTab.IsSelected = true;
            contentsEditor.InitilaizeContents();
        }

        private void ShowListPage()
        {
            _contentsItemViewerTab.IsSelected = true;
        }

        private void ShowCalendarPage()
        {
            _canlendarViewerTab.IsSelected = true;
            canlendarViewer.GoToTodayCalendar();

            // trick
            canlendarViewer.GoToTodayCalendar();
        }

        private void ShowAlarmPage()
        {
            MessageBox.Show("준비중");
        }

        private void MainMenuButtonClick(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button).Tag as string;

            switch (tag) {
                case "New":
                    ShowNewPage();
                    break;

                case "List":
                    ShowListPage();
                    break;

                case "Calendar":
                    ShowCalendarPage();
                    break;

                case "Alarm":
                    ShowAlarmPage();
                    break;

                default:
                    break;
            }
        }

        private void CheckPasswordButtonClick(object sender, RoutedEventArgs e)
        {
            if (_password.Password != Properties.Settings.Default.Password) {
                MessageBox.Show("Invalid Password");
                return;
            }

            ShowListPage();
        }
    }
}
