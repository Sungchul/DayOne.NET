﻿using System;
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
    /// ContentsEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContentsEditor : UserControl
    {
        public event EventHandler<EventArgs> EditDone;

        public DayOneContent currentEntry;

        private string photoPath;

        public ContentsEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty YearNumProperty =
            DependencyProperty.Register("Year", typeof(string), typeof(ContentsEditor));

        public string Year
        {
            get { return (string)GetValue(YearNumProperty); }
            set { SetValue(YearNumProperty, value); }
        }

        public static readonly DependencyProperty MonthNameProperty =
            DependencyProperty.Register("MonthName", typeof(string), typeof(ContentsEditor));

        public string MonthName
        {
            get { return (string)GetValue(MonthNameProperty); }
            set { SetValue(MonthNameProperty, value); }
        }

        public static readonly DependencyProperty DayNumProperty =
            DependencyProperty.Register("Day", typeof(string), typeof(ContentsEditor));

        public string Day
        {
            get { return (string)GetValue(DayNumProperty); }
            set { SetValue(DayNumProperty, value); }
        }

        public static readonly DependencyProperty DayNameProperty =
            DependencyProperty.Register("DayName", typeof(string), typeof(ContentsEditor));

        public string DayName
        {
            get { return (string)GetValue(DayNameProperty); }
            set { SetValue(DayNameProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(ContentsEditor));

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

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

        public void InitilaizeContents()
        {
            currentEntry = null;
            Year = DateTime.Now.Year.ToString();
            MonthName = AbbreviatedMonthName(DateTime.Now);
            Day = DateTime.Now.Day.ToString();
            DayName = AbbreviatedDayName(DateTime.Now);
            Time = DateTime.Now.ToString("hh:mm tt", new System.Globalization.CultureInfo("en-US"));
            editor.Text = string.Empty;
        }

        public void LoadContents(DayOneContent entry)
        {
            currentEntry = entry;
            editor.Text = entry.EntryText;
        }

        public void SaveContents()
        {
            if (currentEntry == null)
                currentEntry = DayOneContent.Create();

            var fullPath = Properties.Settings.Default.EntryPath + 
                System.IO.Path.DirectorySeparatorChar + currentEntry.UUID + ".doentry";
            
            //if (System.IO.File.Exists(fullPath)) {
            //    MessageBox.Show("Error???");
            //    return;
            //}

            currentEntry.EntryText = editor.Text;
            DayOneContent.SaveDayOneContent(fullPath, currentEntry);
        }

        public void SavePhoto()
        {
            if (photoPath != null || System.IO.File.Exists(photoPath)) {
                var fileExt = System.IO.Path.GetExtension(photoPath);
                var targetPath = Properties.Settings.Default.PhotoPath + 
                    System.IO.Path.DirectorySeparatorChar + currentEntry.UUID + fileExt;

                System.IO.File.Copy(photoPath, targetPath);
            }
        }

        private void DoneButtonClick(object sender, RoutedEventArgs e)
        {
            SaveContents();
            SavePhoto();

            if (EditDone != null)
                EditDone(this, EventArgs.Empty);
        }

        private void AddPhotoButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog() {
                Filter = "Image File|*.jpg"
            };
            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                photoPath = dialog.FileName;
            }
        }
    }
}
