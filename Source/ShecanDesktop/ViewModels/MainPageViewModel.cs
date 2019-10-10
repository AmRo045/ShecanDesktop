using System;
using System.Windows.Input;
using ShecanDesktop.Core;
using ShecanDesktop.Core.Network;
using ShecanDesktop.ViewModels.Base;

namespace ShecanDesktop.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Constructors

        public MainPageViewModel()
        {
            _dnsProviderUrl = Properties.Resources.ShecanDnsProvider;
            _dnsResolvingError = "Unable to fetch IP addresses!";
            _offlineStateError = "You are offline.";
            _dnsService = new DnsService();
            _dnsService.DnsChanged += OnDnsChanged;

            RegisterCommands();
            SetStatus();
        }

        #endregion

        #region Fields

        private readonly string _dnsProviderUrl;
        private readonly string _dnsResolvingError;
        private readonly string _offlineStateError;
        private readonly IDnsService _dnsService;

        #endregion

        #region Properties

        public bool ShecanStatus { get; set; }
        public string CurrentInterface { get; set; }
        public string CurrentPreferredDns { get; set; }
        public string CurrentAlternateDns { get; set; }
        public string MessageSnackbarContent { get; set; }
        public bool MessageSnackbarVisibility { get; set; }

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
            => MessageSnackbarVisibility;

        #endregion

        #region Callbacks

        private void EnableShecan(object parameter)
        {
            if (_dnsService.GetCurrentInterfaceAlias() == null)
            {
                SetSnackbarStatus(true, _offlineStateError, true);
                return;
            }

            var shecanDns = _dnsService.GetDnsFromUrl(_dnsProviderUrl);

            if (shecanDns == null)
            {
                SetSnackbarStatus(true, _dnsResolvingError, true);
                return;
            }

            try
            {
                _dnsService.Set(shecanDns);
            }
            catch (Exception exception)
            {
                SetSnackbarStatus(true, exception.Message, true);
            }
        }

        private void DisableShecan(object parameter)
        {
            if (_dnsService.GetCurrentInterfaceAlias() == null)
            {
                SetSnackbarStatus(true, _offlineStateError, true);
                return;
            }

            try
            {
                _dnsService.Unset();
            }
            catch (Exception exception)
            {
                SetSnackbarStatus(true, exception.Message, true);
            }
        }

        private void CloseSnackbar(object parameter)
        {
            MessageSnackbarVisibility = false;
        }

        #endregion

        #region Helpers

        private void RegisterCommands()
        {
            EnableCommand = new RelayCommand(EnableShecan, CanExecuteEnableCommand);
            DisableCommand = new RelayCommand(DisableShecan, CanExecuteDisableCommand);
            CloseSnackbarCommand = new RelayCommand(CloseSnackbar, CanExecuteCloseSnackbarCommand);
        }

        private void SetSnackbarStatus(bool show, 
            string message = null, bool logMessage = false)
        {
            MessageSnackbarVisibility = show;
            MessageSnackbarContent = message;

            if (logMessage)
                Global.Logger.LogError(message);
        }

        private void SetStatus()
        {
            CurrentInterface = _dnsService.GetCurrentInterfaceAlias();

            if (string.IsNullOrEmpty(CurrentInterface))
            {
                SetSnackbarStatus(true, _offlineStateError);
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

            var shecanDns = _dnsService.GetDnsFromUrl(_dnsProviderUrl);
            ShecanStatus = currentDns.Equals(shecanDns);
        }

        private void OnDnsChanged(object sender, EventArgs e)
        {
            SetStatus();
        }

        #endregion
    }
}