using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace ShecanDesktop.Core.Network
{
    public abstract class BaseDnsService
    {
        protected readonly string NetworkConfigurationRegistryPath;

        protected BaseDnsService()
        {
            NetworkConfigurationRegistryPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{0}";
        }

        protected virtual NetworkInterface GetCurrentInterface()
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                     (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                      a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && a.GetIPProperties()
                         .GatewayAddresses
                         .Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return networkInterface;
        }

        protected virtual void ChangeNameServerValue(string ipAddresses)
        {
            var currentAdapterId = GetCurrentInterface().Id;
            var registryPath = string.Format(NetworkConfigurationRegistryPath, currentAdapterId);

            using (var registryKey = Registry.LocalMachine.OpenSubKey(registryPath, true))
                registryKey?.SetValue("NameServer", ipAddresses);
        }

        protected virtual void FlushDns()
        {
            var processInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\cmd.exe",
                Arguments = "/C ipconfig /flushdns"
            };

            Process.Start(processInfo);
        }
    }
}