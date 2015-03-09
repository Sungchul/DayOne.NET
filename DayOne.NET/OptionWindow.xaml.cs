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
        public OptionWindow()
        {
            InitializeComponent();

            dropBoxPathTextBox.Text = ConfigManager.DropBoxPath;
        }

        private void OpenDropBoxPathClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                // TO Do!! Localize!!!
                
                if (!System.IO.Directory.Exists(dialog.SelectedPath + ConfigManager.BaseEntryPath)) {
                    MessageBox.Show("선택된 DropBox 경로에서 Entry 폴더를 찾을 수 없습니다.");
                    saveButton.IsEnabled = false;
                    return;
                }

                if (!System.IO.Directory.Exists(dialog.SelectedPath + ConfigManager.BasePhotoPath)) {
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
            ConfigManager.DropBoxPath = dropBoxPathTextBox.Text;
            ConfigManager.EntryPath = dropBoxPathTextBox.Text + ConfigManager.BaseEntryPath;
            ConfigManager.PhotoPath = dropBoxPathTextBox.Text + ConfigManager.BasePhotoPath;

            ConfigManager.SaveConfig();
            
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
