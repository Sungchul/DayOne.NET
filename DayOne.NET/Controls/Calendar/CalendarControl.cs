/* 
 * Orignal Code : Jarloo, http://www.jarloo.com
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace DayOne.NET.Controls
{
    public class CalendarControl : Control
    {
        public ObservableCollection<Day> Days { get; private set; }
        

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(CalendarControl));

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(string), typeof(CalendarControl));

        public string Year
        {
            get { return (string)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(string), typeof(CalendarControl));

        public string Month
        {
            get { return (string)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }

        static CalendarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarControl), new FrameworkPropertyMetadata(typeof(CalendarControl)));
        }

        public CalendarControl()
        {
            DataContext = this;
            
            Days = new ObservableCollection<Day>();
            
            Initialize(DateTime.Today.Year, DateTime.Today.Month);
        }

        public void Initialize(int year, int month)
        {
            Days.Clear();

            var beginOfMonth = new DateTime(year, month, 1);
            var offset = (int)beginOfMonth.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(year, month);

            Year = year.ToString();
            Month = GetMonthFullName(month);

            var sum = offset + daysInMonth;
            RowCount = (int)Math.Ceiling(sum / 7.0);

            var nums =
                Enumerable.Repeat<int>(0, offset)
                .Concat(Enumerable.Range(1, daysInMonth))
                .Concat(Enumerable.Repeat<int>(0, (RowCount * 7) - sum));

            nums.ToList().ForEach(num => Days.Add(new Day(year, month, num)));

            Days[10].HasContents = true;
            Days[11].HasContents = true;
            Days[17].HasContents = true;
        }

        private string GetMonthFullName(int month)
        {
            return new DateTime(2014, month, 1).ToString("MMMM", CultureInfo.InvariantCulture);
        }

        private string GetMonthShortName(int month)
        {
            return new DateTime(2014, month, 1).ToString("MMM", CultureInfo.InvariantCulture);
        }
    }

    public class Day
    {
        public Day(int year, int month, int day, bool hasContents = false)
        {
            if (day > 0) {
                Visibility = System.Windows.Visibility.Visible;
                NumberOfDay = day.ToString();
                HasContents = hasContents;
                
                if (DateTime.Today.Day == day &&
                    DateTime.Today.Month == month &&
                    DateTime.Today.Year == year) {
                    IsToday = true;
                }
            }
            else {
                Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public bool IsToday {get; set;}

        public Visibility Visibility { get; set; }

        public bool HasContents { get; set; }

        public string NumberOfDay {get; set;}
    }
}
