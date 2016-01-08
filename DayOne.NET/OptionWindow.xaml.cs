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
using System.Windows.Shapes;

namespace DayOne.NET
{
    /// <summary>
    /// OptionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OptionWindow : Window
    {
        public static readonly string BaseEntryPath = @"\Apps\Day One\Journal.dayone\entries";

        public static readonly string BasePhotoPath = @"\Apps\Day One\Journal.dayone\photos"; 

        public OptionWindow()
        {
            InitializeComponent();

            dropBoxPathTextBox.Text = Properties.Settings.Default.DropBoxPath;
            UsePassword = Properties.Settings.Default.UsePassword;
            _password.Password = Properties.Settings.Default.Password;
            _confirmPassword.Password = Properties.Settings.Default.Password;
        }



        public string DropBoxPath
        {
            get { return (string)GetValue(DropBoxPathProperty); }
            set { SetValue(DropBoxPathProperty, value); }
        }

        public static readonly DependencyProperty DropBoxPathProperty =
            DependencyProperty.Register("DropBoxPath", typeof(string), typeof(OptionWindow), new PropertyMetadata(string.Empty));



        public bool UsePassword
        {
            get { return (bool)GetValue(UsePasswordProperty); }
            set { SetValue(UsePasswordProperty, value); }
        }
       
        public static readonly DependencyProperty UsePasswordProperty =
            DependencyProperty.Register("UsePassword", typeof(bool), typeof(OptionWindow), new PropertyMetadata(false));



        

        


        private void OpenDropBoxPathClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                // TO Do!! Localize!!!
                
                if (!System.IO.Directory.Exists(dialog.SelectedPath + BaseEntryPath)) {
                    MessageBox.Show("선택된 DropBox 경로에서 Entry 폴더를 찾을 수 없습니다.");
                    saveButton.IsEnabled = false;
                    return;
                }

                if (!System.IO.Directory.Exists(dialog.SelectedPath + BasePhotoPath)) {
                    MessageBox.Show("선택된 DropBox 경로에서 Photo 폴더를 찾을 수 없습니다.");
                    saveButton.IsEnabled = false;
                    return;
                }

                dropBoxPathTextBox.Text = dialog.SelectedPath;
                saveButton.IsEnabled = true;
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Validation
            if (UsePassword && _password.Password != _confirmPassword.Password) {
                MessageBox.Show("Password and confirm password are different.");
                return;
            }

            Properties.Settings.Default.DropBoxPath = dropBoxPathTextBox.Text;
            Properties.Settings.Default.EntryPath = dropBoxPathTextBox.Text + BaseEntryPath;
            Properties.Settings.Default.PhotoPath = dropBoxPathTextBox.Text + BasePhotoPath;

            Properties.Settings.Default.UsePassword = UsePassword;
            Properties.Settings.Default.Password = _password.Password;
            Properties.Settings.Default.Save();
            
            DialogResult = true;
            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
