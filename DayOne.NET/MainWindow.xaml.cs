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
        }


        private string GetHtmlContents(IEnumerable<string> entries)
        {
            var contetsHtml = string.Empty;

            foreach (var entry in entries) {
                var item = DayOneContent.ReadContents(entry);
                contetsHtml += markdownProcessor.Transform(item.EntryText);
            }
            
            return ENTRY_TEMPLATE.Replace("##STYLE##", "").Replace("##PICTURE##", "").Replace("##ENTRY##", contetsHtml);
        }

        private void CanlendarViewerDayItemSelected(object sender, DayItemSelectedArgs e)
        {
            if (e.SelectedUUIDs != null) {
                var entries = GetEntryList(e.SelectedUUIDs);
                contentsViewer.LoadContents(GetHtmlContents(entries));

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

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {
            

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            contentsEditor.Visibility = System.Windows.Visibility.Hidden;
            contentsViewer.Visibility = System.Windows.Visibility.Hidden;
            canlendarViewer.Visibility = System.Windows.Visibility.Visible; 
        }

        private void NewButtonClick(object sender, RoutedEventArgs e)
        {
            contentsEditor.Visibility = System.Windows.Visibility.Visible;
            contentsViewer.Visibility = System.Windows.Visibility.Hidden;
            canlendarViewer.Visibility = System.Windows.Visibility.Hidden;

            contentsEditor.InitilaizeContents();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            contentsEditor.SaveContents(ENTRY_PATH);

            contentsEditor.Visibility = System.Windows.Visibility.Hidden;
            contentsViewer.Visibility = System.Windows.Visibility.Visible;
            canlendarViewer.Visibility = System.Windows.Visibility.Hidden; 
        }
    }
}
