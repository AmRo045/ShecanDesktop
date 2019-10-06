using System.Windows;
using System.Windows.Input;
using ShecanDesktop.ViewModels.Base;
using ShecanDesktop.Views.Boxes;

namespace ShecanDesktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constrcutors

        public MainWindowViewModel()
        {
            OpenIssuesPageCommand = new RelayCommand(OpenIssuesPage);
            OpenAboutPageCommand = new RelayCommand(OpenAboutPage);
        }

        #endregion

        #region Commands

        public ICommand OpenIssuesPageCommand { get; set; }
        public ICommand OpenAboutPageCommand { get; set; }

        #endregion

        #region Callbacks

        private static void OpenIssuesPage(object parameter)
        {
            var issuesPage = (string)parameter;
            System.Diagnostics.Process.Start(issuesPage);
        }

        private static void OpenAboutPage(object parameter)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.DisplayBox(new AboutPage());
        }

        #endregion
    }
}