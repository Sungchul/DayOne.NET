using System;
using System.Collections.Generic;
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
    /// SmallItemListViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmallItemListViewer : UserControl
    {
        public event EventHandler<ItemEditRequestEventArgs> ItemEditRequest;

        public SmallItemListViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DayNumProperty =
            DependencyProperty.Register("Day", typeof(string), typeof(SmallItemListViewer));

        public string Day
        {
            get { return (string)GetValue(DayNumProperty); }
            set { SetValue(DayNumProperty, value); }
        }

        public static readonly DependencyProperty DayNameProperty =
            DependencyProperty.Register("DayName", typeof(string), typeof(SmallItemListViewer));

        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }

        public static readonly DependencyProperty SplitLineVisibilityProperty =
            DependencyProperty.Register("SplitLineVisibility", typeof(Visibility), typeof(SmallItemListViewer));

        public Visibility SplitLineVisibility
        {
            get { return (Visibility)GetValue(SplitLineVisibilityProperty); }
            set { SetValue(SplitLineVisibilityProperty, value); }
        }

        private static string AbbreviatedDayName(DateTime datetime)
        {
            // 영어로 고정
            var english = new System.Globalization.CultureInfo("en-US");
            string[] names = english.DateTimeFormat.AbbreviatedDayNames;

            return names[(int)datetime.DayOfWeek].ToUpper();
        }

        

        public void InitializeItems(IEnumerable<DayOneContent> contents, string photoDir)
        {
            foreach (var content in contents) {
                var itemViewer = new SmallItemViewer() { UUID = content.UUID };
                itemViewer.ItemEditRequest += (o, e) => {
                    if (ItemEditRequest != null)
                        ItemEditRequest(this, e);
                };

                if (!string.IsNullOrEmpty(content.EntryText)) {
                    var splitedEntry = content.EntryText.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    // Has Tile
                    if (splitedEntry.Count() > 1) {
                        itemViewer.TitleVisibility = System.Windows.Visibility.Visible;
                        itemViewer.Title = splitedEntry.First();
                        itemViewer.Entry = splitedEntry.Skip(1).Aggregate((x1, x2) => x1 + " " + x2);
                    }
                    else {
                        itemViewer.TitleVisibility = System.Windows.Visibility.Collapsed;
                        itemViewer.Entry = content.EntryText;
                    }
                }

                itemViewer.Infotmation = content.GetInformation();

                var expectedPath = photoDir + System.IO.Path.DirectorySeparatorChar + content.UUID + ".jpg";
                if (System.IO.File.Exists(expectedPath)) {
                    itemViewer.ImageVisibility = System.Windows.Visibility.Visible;
                    itemViewer.SetPhoto(expectedPath);
                }
                else {
                    itemViewer.ImageVisibility = System.Windows.Visibility.Collapsed;
                }

                container.Children.Add(itemViewer);
            }

            container.Children.Cast<SmallItemViewer>().Last().SplitLineVisibility = System.Windows.Visibility.Collapsed;

            var today = contents.First().CreationDate;
            DayName = AbbreviatedDayName(today);
            Day = today.Day.ToString("D2");
        }
    }
}
