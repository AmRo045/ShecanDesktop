using ShecanDesktop.Models.Base;

namespace ShecanDesktop.Models
{
    public class Dns : ObservableObject
    {
        public Dns(string preferredServer, string alternateServer)
        {
            PreferredServer = preferredServer;
            AlternateServer = alternateServer;
        }

        public string PreferredServer { get; }
        public string AlternateServer { get; }

        public override bool Equals(object parameter)
        {
            if (parameter == null) return false;
            var dns = (Dns)parameter;

            return PreferredServer.Equals(dns.PreferredServer) &&
                   AlternateServer.Equals(dns.AlternateServer);
        }

        protected bool Equals(Dns other)
        {
            return PreferredServer == other.PreferredServer && AlternateServer == other.AlternateServer;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PreferredServer != null ? PreferredServer.GetHashCode() : 0) * 397) ^ (AlternateServer != null ? AlternateServer.GetHashCode() : 0);
            }
        }
    }
}