using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// ContentsViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContentsViewer : UserControl
    {
        public ContentsViewer()
        {
            InitializeComponent();
            
         
        }


        public void InitilaizeSession(string contentsPath)
        {
            var prefs = new WebPreferences();
            var session = WebCore.CreateWebSession(prefs);
            session.AddDataSource("content", new Awesomium.Core.Data.DirectoryDataSource(contentsPath));
            htmlRenderer.WebSession = session;

        }
        public void LoadContents(string html)
        {   
            //htmlRenderer.na
            //htmlRenderer.Document = html;
            //htmlRenderer.Navigate("http://www.daum.net");
            //htmlRenderer.NavigateToStream(new System.IO.MemoryStream(Encoding.Default.GetBytes(html)));
            htmlRenderer.LoadHTML(html);
        }
        

        public void LoadContentsFromUri(string uri)
        { 
            //htmlRenderer.na
            //htmlRenderer.Document = html;
            //htmlRenderer.Navigate(uri);
            
            htmlRenderer.Source = new Uri(uri);
            htmlRenderer.Reload(true);
            //htmlRenderer.WebSession = WebCore.CreateWebSession(new WebPreferences() { CustomCSS = yourCSS });
        }
    }

    //public class MyHtmlPanel :HtmlPanel{
    //    public MyHtmlPanel()
    //    {
    //        Loaded += MyHtmlPanel_Loaded;
    //    }

    //    void MyHtmlPanel_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        var style = Application.Current.Resources["iOsScrollBarStyle"] as Style;

    //        _verticalScrollBar.Style = style;
    //        _verticalScrollBar.ClearValue(ScrollBar.WidthProperty);
    //    }
    //}
}
