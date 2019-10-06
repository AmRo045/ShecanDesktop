using System;
using Shecan.Models;

namespace Shecan.Core.Network
{
    public interface IDnsService
    {
        event EventHandler DnsChanged;

        void Set(Dns dns);
        void Unset();
        Dns GetCurrentDns();
        string GetCurrentInterfaceAlias();
    }
}