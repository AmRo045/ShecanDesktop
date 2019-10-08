using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace ShecanDesktop.Core.Network
{
    public abstract class BaseDnsService
    {
        protected readonly string NetworkConfigurationPath;

        protected BaseDnsService()
        {
            NetworkConfigurationPath = "Win32_NetworkAdapterConfiguration";
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