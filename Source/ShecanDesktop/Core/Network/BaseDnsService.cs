using System.Management.Automation;
using System.Management.Automation.Runspaces;
using ShecanDesktop.Models;

namespace ShecanDesktop.Core.Network
{
    public abstract class BaseDnsService
    {
        protected readonly string PowerShellScriptFile;

        protected BaseDnsService(string powerShellScriptFile)
        {
            PowerShellScriptFile = powerShellScriptFile;
        }

        protected virtual Command CreateSetCommand(string interfaceAlias, Dns dns)
        {
            var command = new Command(PowerShellScriptFile);
            var interfaceAliasParameter = new CommandParameter("InterfaceAlias", interfaceAlias);
            var preferredDnsParameter = new CommandParameter("PreferredDns", dns.PreferredServer);
            var alternateDnsParameter = new CommandParameter("AlternateDns", dns.AlternateServer);

            command.Parameters.Add(interfaceAliasParameter);
            command.Parameters.Add(preferredDnsParameter);
            command.Parameters.Add(alternateDnsParameter);

            return command;
        }

        protected virtual Command CreateUnsetCommand(string interfaceAlias)
        {
            var command = new Command(PowerShellScriptFile);
            var interfaceAliasParameter = new CommandParameter("InterfaceAlias", interfaceAlias);
            var unsetParameter = new CommandParameter("Unset", true);

            command.Parameters.Add(interfaceAliasParameter);
            command.Parameters.Add(unsetParameter);

            return command;
        }

        protected virtual void RunPowerShellCommand(Command command)
        {
            var runSpaceConfig = RunspaceConfiguration.Create();

            using (var runSpace = RunspaceFactory.CreateRunspace(runSpaceConfig))
            {
                runSpace.Open();

                var runSpaceInvoker = new RunspaceInvoke(runSpace);
                runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted");

                using (var pipeline = runSpace.CreatePipeline())
                {
                    pipeline.Commands.Add(command);
                    pipeline.Invoke();
                }
            }
        }
    }
}