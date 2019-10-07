using System;
using ShecanDesktop.Models;

namespace ShecanDesktop.Core.Network
{
    public interface IDnsService
    {
        event EventHandler DnsChanged;

        void Set(Dns dns);
        void Unset();
        Dns GetCurrentDns();
        string GetCurrentInterfaceAlias();
        Dns GetDnsFromUrl(string url);
    }
}