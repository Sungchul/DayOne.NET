using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var uuid = (string)entry["UUID"];

                if (!contentsList.Keys.Contains(created)) {
                    contentsList.Add(created, new List<string>() { uuid });
                }
                else {
                    contentsList[created].Add(uuid);
                }
            }
        }

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {
            var selected = @"C:\Users\sungchul\Dropbox\Apps\Day One\Journal.dayone\entries";
            LoadDateContentsDateTime(selected);
            canlendarViewer.InitializeCalendar(contentsList);

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            // 
        }
    }
}
