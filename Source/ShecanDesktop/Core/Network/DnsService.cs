using System;
using System.Linq;
using System.Net.NetworkInformation;
using ShecanDesktop.Models;
using System.Management;

namespace ShecanDesktop.Core.Network
{
    public class DnsService : BaseDnsService, IDnsService
    {
        public event EventHandler DnsChanged;

        public virtual void Set(Dns dns)
        {
            var ipAddresses = new[] { dns.PreferredServer, dns.AlternateServer };
            var currentInterface = GetCurrentInterface();
            if (currentInterface == null) return;

            var managementClass = new ManagementClass(NetworkConfigurationPath);
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;

                if (!(bool)managementObject["IPEnabled"])
                    continue;

                if (!managementObject["Caption"].ToString().Contains(currentInterface.Description))
                    continue;

                var managementBaseObject = managementObject.GetMethodParameters("SetDNSServerSearchOrder");

                if (managementBaseObject == null) continue;

                managementBaseObject["DNSServerSearchOrder"] = ipAddresses;
                managementObject.InvokeMethod("SetDNSServerSearchOrder", managementBaseObject, null);
                break;
            }

            OnDnsChanged();
        }

        public virtual void Unset()
        {
            var currentInterface = GetCurrentInterface();
            if (currentInterface == null) return;

            var managementClass = new ManagementClass(NetworkConfigurationPath);
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;

                if (!(bool)managementObject["IPEnabled"])
                    continue;

                if (!managementObject["Caption"].ToString().Contains(currentInterface.Description))
                    continue;

                var managementBaseObject = managementObject.GetMethodParameters("SetDNSServerSearchOrder");

                if (managementBaseObject == null) continue;

                managementBaseObject["DNSServerSearchOrder"] = null;
                managementObject.InvokeMethod("SetDNSServerSearchOrder", managementBaseObject, null);
                break;
            }

            OnDnsChanged();
        }

        public virtual Dns GetCurrentDns()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up) continue;
                var ipProperties = networkInterface.GetIPProperties();
                var dnsAddresses = ipProperties.DnsAddresses;

                if (dnsAddresses.Count < 2) break;

                var preferredDns = dnsAddresses[0].ToString();
                var alternateDns = dnsAddresses[1].ToString();

                return new Dns(preferredDns, alternateDns);
            }

            return null;
        }

        public virtual string GetCurrentInterfaceAlias()
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                a => a.OperationalStatus == OperationalStatus.Up &&
                     (a.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                      a.NetworkInterfaceType == NetworkInterfaceType.Ethernet) && a.GetIPProperties()
                         .GatewayAddresses
                         .Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));

            return networkInterface?.Name;
        }

        public Dns GetDnsFromUrl(string url)
        {
            var addresses = System.Net.Dns.GetHostAddresses(url);

            return new Dns(addresses[1].ToString(),
                addresses[0].ToString());
        }

        protected virtual void OnDnsChanged()
        {
            FlushDns();
            DnsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}