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
    /// ContentItemViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContentItemViewer : UserControl
    {
        private readonly string ENTRY_PATH = @"C:\Users\sungchul\Dropbox\Apps\Day One\Journal.dayone\entries";
        private readonly string PHOTO_PATH = @"C:\Users\sungchul\Dropbox\Apps\Day One\Journal.dayone\photos";

        private Dictionary<DateTime, List<string>> contentsList;
        private DateTime[] dateTimes;
        
        private double verticalOffsetBackwardItemAddTriger = 100;
        
        private int index = 0;

        public ContentItemViewer()
        {
            InitializeComponent();
        }

        public void InitializeItemViewer(Dictionary<DateTime, List<string>> contentsList)
        {
            container.Children.Clear();
            this.contentsList = contentsList;
            var keys = contentsList.Keys.ToList();
            keys.Sort();
            keys.Reverse();
            dateTimes = keys.ToArray();

            index = 0;
        }

        private void AddBackwardEntries()
        {
            if (index < dateTimes.Count()) {
                var selectedDate = dateTimes[index];
                var uuids = contentsList[selectedDate];

                var pathes = uuids.Select(id => ENTRY_PATH + System.IO.Path.DirectorySeparatorChar + id + ".doentry");
                var entries = pathes.Select(path => DayOneContent.ReadContents(path));
                var viewer = new SmallItemListViewer();
                viewer.InitializeItems(entries, PHOTO_PATH);

                container.Children.Add(viewer);
                index++;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (contentsList == null)
                return;

            if (e.ExtentHeight - (e.ViewportHeight + e.VerticalOffset) < verticalOffsetBackwardItemAddTriger) {
                for (var i = 0; i < 5; i++) {
                    AddBackwardEntries();
                }
            }
        }
    }
}
