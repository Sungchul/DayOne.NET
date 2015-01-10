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
    /// ContentsEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContentsEditor : UserControl
    {
        public DayOneContent currentEntry;

        public ContentsEditor()
        {
            InitializeComponent();
        }

        public void InitilaizeContents()
        {
            currentEntry = null;
        }

        public void LoadContents(DayOneContent entry)
        {
            currentEntry = entry;
            editor.Text = entry.EntryText;
        }

        public void SaveContents(string path)
        {
            if (currentEntry == null)
                currentEntry = DayOneContent.Create();

            var fullPath = path + System.IO.Path.DirectorySeparatorChar + currentEntry.UUID + ".doentry";
            if (System.IO.File.Exists(fullPath)) {
                MessageBox.Show("Error");
                return;
            }

            currentEntry.EntryText = editor.Text;
            DayOneContent.SaveDayOneContent(fullPath, currentEntry);
        }
    }
}
