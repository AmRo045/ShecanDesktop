using System;
using System.Linq;
using System.Net.NetworkInformation;
using ShecanDesktop.Models;

namespace ShecanDesktop.Core.Network
{
    public class DnsService : BaseDnsService, IDnsService
    {
        public event EventHandler DnsChanged;

        protected virtual void OnDnsChanged()
        {
            FlushDns();
            DnsChanged?.Invoke(this, EventArgs.Empty);
        }


        public virtual void Set(Dns dns)
        {
            var ipAddresses = $"{dns.PreferredServer},{dns.AlternateServer}";
            ChangeNameServerValue(ipAddresses);
            OnDnsChanged();
        }

        public virtual void Unset()
        {
            var ipAddresses = string.Empty;
            ChangeNameServerValue(ipAddresses);
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
            try
            {
                // For some reason System.Net.Dns.GetHostAddresses method
                // does not return IP addresses in correct order.
                // So i have to reorder returned IP addresses to correct order.
                var addresses = System.Net.Dns.GetHostAddresses(url);

                var preferredIp = addresses[0].ToString().StartsWith("178") ?
                    addresses[0].ToString() : addresses[1].ToString();

                var alternateIp = addresses[0].ToString().StartsWith("185") ?
                    addresses[0].ToString() : addresses[1].ToString();

                return new Dns(preferredIp, alternateIp);
            }
            catch
            {
                return null;
            }
        }
    }
}