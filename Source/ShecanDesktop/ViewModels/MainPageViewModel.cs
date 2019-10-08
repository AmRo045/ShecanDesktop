using System;
using System.Net.Sockets;
using System.Windows.Input;
using ShecanDesktop.Core;
using ShecanDesktop.Core.Network;
using ShecanDesktop.Models;
using ShecanDesktop.ViewModels.Base;

namespace ShecanDesktop.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructors

        public MainPageViewModel()
        {
            _dnsService = new DnsService();
            _dnsService.DnsChanged += OnDnsChanged;

            var shecanDnsProvider = Properties.Resources.ShecanDnsProvider;

            try
            {
                _shecanDns = _dnsService.GetDnsFromUrl(shecanDnsProvider);
            }
            catch (SocketException exception)
            {
                InternetSnackbarVisibility = true;
                Launcher.Logger.LogError(exception.Message);
            }

            RegisterCommands();
            SetStatus();
        }

        #endregion

        #region Fields

        private readonly IDnsService _dnsService;
        private readonly Dns _shecanDns;

        #endregion

        #region Properties

        public bool ShecanDesktopStatus { get; set; }
        public string CurrentInterface { get; set; }
        public string CurrentPreferredDns { get; set; }
        public string CurrentAlternateDns { get; set; }
        public bool InternetSnackbarVisibility { get; set; }

        #endregion

        #region Commands

        public ICommand EnableCommand { get; set; }
        public ICommand DisableCommand { get; set; }
        public ICommand CloseSnackbarCommand { get; set; }

        #endregion

        #region Predicates

        private bool CanExecuteEnableCommand(object parameter)
            => !ShecanDesktopStatus;

        private bool CanExecuteDisableCommand(object parameter)
            => ShecanDesktopStatus;

        private bool CanExecuteCloseSnackbarCommand(object parameter)
            => InternetSnackbarVisibility;

        #endregion

        #region Callbacks

        private void EnableShecanDesktop(object parameter)
        {
            try
            {
                _dnsService.Set(_shecanDns);
            }
            catch (SocketException exception)
            {
                InternetSnackbarVisibility = true;
                Launcher.Logger.LogError(exception.Message);
            }
        }

        private void DisableShecanDesktop(object parameter)
        {
            try
            {
                _dnsService.Unset();
            }
            catch (SocketException exception)
            {
                InternetSnackbarVisibility = true;
                Launcher.Logger.LogError(exception.Message);
            }
        }

        private void CloseSnackbar(object parameter)
        {
            InternetSnackbarVisibility = false;
        }

        #endregion

        #region Helpers

        private void RegisterCommands()
        {
            EnableCommand = new RelayCommand(EnableShecanDesktop, CanExecuteEnableCommand);
            DisableCommand = new RelayCommand(DisableShecanDesktop, CanExecuteDisableCommand);
            CloseSnackbarCommand = new RelayCommand(CloseSnackbar, CanExecuteCloseSnackbarCommand);
        }

        private void SetStatus()
        {
            CurrentInterface = _dnsService.GetCurrentInterfaceAlias();

            if (string.IsNullOrEmpty(CurrentInterface))
            {
                InternetSnackbarVisibility = true;
                return;
            }

            var currentDns = _dnsService.GetCurrentDns();
            var hasDns = currentDns != null;

            CurrentPreferredDns = hasDns ? currentDns.PreferredServer : "NOT SET";
            CurrentAlternateDns = hasDns ? currentDns.AlternateServer : "NOT SET";

            if (!hasDns)
            {
                ShecanDesktopStatus = false;
                return;
            }

            ShecanDesktopStatus = currentDns.Equals(_shecanDns);
        }

        private void OnDnsChanged(object sender, EventArgs e)
        {
            SetStatus();
        }

        #endregion
    }
}