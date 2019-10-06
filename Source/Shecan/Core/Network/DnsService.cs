using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Shecan.Models;

namespace Shecan.Core.Network
{
    public class DnsService : BaseDnsService, IDnsService
    {
        public DnsService(string powerShellScriptFile)
            : base(powerShellScriptFile)
        {

        }

        public event EventHandler DnsChanged;

        public virtual void Set(Dns dns)
        {
            if (!File.Exists(PowerShellScriptFile))
                throw new InvalidOperationException("Script file not found!");

            var currentInterfaceAlias = GetCurrentInterfaceAlias();
            var setCommand = CreateSetCommand(currentInterfaceAlias, dns);

            RunPowerShellCommand(setCommand);
            OnDnsChanged();
        }

        public virtual void Unset()
        {
            if (!File.Exists(PowerShellScriptFile))
                throw new InvalidOperationException("Script file not found!");

            var currentInterfaceAlias = GetCurrentInterfaceAlias();
            var unsetCommand = CreateUnsetCommand(currentInterfaceAlias);

            RunPowerShellCommand(unsetCommand);
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


        protected virtual void OnDnsChanged()
        {
            DnsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}