using System;
using System.Windows;
using Shecan.Contracts;
using Shecan.Views;

namespace Shecan
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            BoxHost.ParentElement = MainFrame;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new MainPage());
        }

        public void DisplayBox(IBox box, bool closeOnClickAway = true)
        {
            box.CloseCallback = BoxCloseCallback;

            BoxHost.CloseOnClickAway = closeOnClickAway;
            BoxHost.Show(box);
        }

        public void DisplayBox(IBox box, Action completeCallback, bool closeOnClickAway = true)
        {
            box.CloseCallback = BoxCloseCallback;
            box.CompleteCallback = completeCallback;

            BoxHost.CloseOnClickAway = closeOnClickAway;
            BoxHost.Show(box);

        }

        private void BoxCloseCallback()
        {
            BoxHost.Close();
        }

        public void CloseBox()
        {
            BoxCloseCallback();
        }
    }
}
