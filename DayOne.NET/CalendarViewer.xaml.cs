using DayOne.NET.Controls;
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
    /// CalendarViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CalendarViewer : UserControl
    {
        private double verticalOffsetForwardItemAddTriger = 100;
        private double verticalOffsetBackwardItemAddTriger = 100;

        private DateTime lastForwardDateTime;
        private DateTime lastBackwardDateTime;
        private CalendarControl todayCalendar;

        public CalendarViewer()
        {
            InitializeComponent();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (contentsList == null)
                return;

            if (e.VerticalOffset < verticalOffsetForwardItemAddTriger) {
                for (var i = 0; i < 5; i++) {
                    AddForwardCalendar();
                }

                var newOffset = scrollViewer.VerticalOffset + 260 * 5;
                scrollViewer.ScrollToVerticalOffset(newOffset);
            }

            else if (e.ExtentHeight - (e.ViewportHeight + e.VerticalOffset) < verticalOffsetBackwardItemAddTriger) {
                for (var i = 0; i < 5; i++) {
                    AddBackwardCalendar();
                }
            }
        }


        private void GoToTodayCalendar()
        {
        }

        public void InitializeCalendar(Dictionary<DateTime, List<string>> contentsList)
        {
            this.contentsList = contentsList;
            
            lastForwardDateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            lastBackwardDateTime = lastForwardDateTime.AddMonths(-1);

            todayCalendar = AddForwardCalendar();
            AddForwardCalendar();
            AddForwardCalendar();

            AddBackwardCalendar();
            AddBackwardCalendar();
            AddBackwardCalendar();
        }

        private Dictionary<DateTime, List<string>> contentsList;

        private IEnumerable<int> GetHasContentsDays(int year, int month)
        {
            if (contentsList == null || contentsList.Count == 0)
                return null;

            return contentsList.
                Where(content => content.Key.Year == year && content.Key.Month == month).
                Select(content => content.Key.Day);
        }


        public CalendarControl AddForwardCalendar()
        {
            var calendar = new CalendarControl() { Height = 260 };
            var hasContetnsdays = GetHasContentsDays(lastForwardDateTime.Year, lastForwardDateTime.Month);
            calendar.Initialize(lastForwardDateTime.Year, lastForwardDateTime.Month, hasContetnsdays);
            container.Children.Insert(0, calendar);

            lastForwardDateTime = lastForwardDateTime.AddMonths(1);

            return calendar;
        }

        public CalendarControl AddBackwardCalendar()
        {
            var calendar = new CalendarControl() { Height = 260 };
            var hasContetnsdays = GetHasContentsDays(lastBackwardDateTime.Year, lastBackwardDateTime.Month);
            calendar.Initialize(lastBackwardDateTime.Year, lastBackwardDateTime.Month, hasContetnsdays);
            container.Children.Add(calendar);

            lastBackwardDateTime = lastBackwardDateTime.AddMonths(-1);

            return calendar;
        }

        private void EventHandlerName(object sender, RoutedEventArgs e)
        {   
            var day = (e.OriginalSource as Button).DataContext as Day;

            MessageBox.Show(string.Format("Year: {0}, Month: {1}, Day: {2}", day.ThisYear, day.ThisMonth, day.ThisDay));
        }
    }
}
