using System;
using System.Windows;
using System.Windows.Controls;
using ShecanDesktop.Contracts;

namespace ShecanDesktop.Views.Boxes
{
    public partial class AboutPage : IBox
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        public Action CloseCallback { get; set; }
        public Action CompleteCallback { get; set; }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            CloseCallback.Invoke();
        }

        private void LinkButtonOnClick(object sender, RoutedEventArgs e)
        {
            var hyperlink = (Button)sender;
            System.Diagnostics.Process.Start(hyperlink.Tag.ToString());
        }
    }
}
