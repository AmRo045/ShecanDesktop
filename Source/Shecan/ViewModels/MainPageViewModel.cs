using System;
using System.Windows.Input;
using Shecan.Core;
using Shecan.Core.Network;
using Shecan.Models;
using Shecan.ViewModels.Base;

namespace Shecan.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructors

        public MainPageViewModel()
        {
            var preferredServer = Properties.Resources.PreferredServer;
            var alternateServer = Properties.Resources.AlternateServer;
            _shecanDns = new Dns(preferredServer, alternateServer);

            _dnsService = new DnsService(@"D:\Workspace\Programming\VS2015\WPF\DnsChanger\DnsChanger\bin\Debug\DnsChanger.ps1");
            _dnsService.DnsChanged += OnDnsChanged;

            RegisterCommands();
            SetStatus();
        }

        #endregion

        #region Fields

        private readonly IDnsService _dnsService;
        private readonly Dns _shecanDns;

        #endregion

        #region Properties

        public bool ShecanStatus { get; set; }
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
            => !ShecanStatus;

        private bool CanExecuteDisableCommand(object parameter)
            => ShecanStatus;

        private bool CanExecuteCloseSnackbarCommand(object parameter)
            => InternetSnackbarVisibility;

        #endregion

        #region Callbacks

        private void EnableShecan(object parameter)
        {
            try
            {
                _dnsService.Set(_shecanDns);
            }
            catch (Exception exception)
            {
                Launcher.Logger.LogError(exception.Message);
                InternetSnackbarVisibility = true;
            }
        }

        private void DisableShecan(object parameter)
        {
            try
            {
                _dnsService.Unset();
            }
            catch (Exception exception)
            {
                Launcher.Logger.LogError(exception.Message);
                InternetSnackbarVisibility = true;
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
            EnableCommand = new RelayCommand(EnableShecan, CanExecuteEnableCommand);
            DisableCommand = new RelayCommand(DisableShecan, CanExecuteDisableCommand);
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
                ShecanStatus = false;
                return;
            }

            ShecanStatus = currentDns.Equals(_shecanDns);
        }

        private void OnDnsChanged(object sender, EventArgs e)
        {
            SetStatus();
        }

        #endregion
    }
}