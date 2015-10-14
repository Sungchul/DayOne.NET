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
    /// SmallItemViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmallItemViewer : UserControl
    {
        public event EventHandler<ItemEditRequestEventArgs> ItemEditRequest;

        public SmallItemViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SmallItemViewer));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(string), typeof(SmallItemViewer));

        public string Entry
        {
            get { return (string)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }

        public static readonly DependencyProperty InfotmationProperty =
            DependencyProperty.Register("Infotmation", typeof(string), typeof(SmallItemViewer));

        public string Infotmation
        {
            get { return (string)GetValue(InfotmationProperty); }
            set { SetValue(InfotmationProperty, value); }
        }

        public void SetPhoto(string path)
        {
            brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Relative));
        }

        public static readonly DependencyProperty ImageVisibilityProperty =
            DependencyProperty.Register("ImageVisibility", typeof(Visibility), typeof(SmallItemViewer));

        public Visibility ImageVisibility
        {
            get { return (Visibility)GetValue(ImageVisibilityProperty); }
            set { SetValue(ImageVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(SmallItemViewer));

        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SplitLineVisibilityProperty =
            DependencyProperty.Register("SplitLineVisibility", typeof(Visibility), typeof(SmallItemViewer));

        public Visibility SplitLineVisibility
        {
            get { return (Visibility)GetValue(SplitLineVisibilityProperty); }
            set { SetValue(SplitLineVisibilityProperty, value); }
        }

        public string UUID { get; set; }

        private void TextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ItemEditRequest != null)
                ItemEditRequest(this, new ItemEditRequestEventArgs() { UUID = this.UUID });
        }
    }

    public class ItemEditRequestEventArgs : EventArgs
    {
        public string UUID { get; set; }
    }
}
