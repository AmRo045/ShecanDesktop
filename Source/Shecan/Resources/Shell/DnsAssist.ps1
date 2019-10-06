param(
    [string]$InterfaceAlias="",
    [string]$PreferredDns="",
    [string]$AlternateDns="",
    [switch]$Unset=$false
)


if ([string]::IsNullOrEmpty($InterfaceAlias)) {
    Write-Host "No interface alias specified."
    exit
}

if ($Unset) {
    Set-DnsClientServerAddress -InterfaceAlias $InterfaceAlias -ResetServerAddresses
    Write-Host "DNS Removed."
    exit
}

if ([string]::IsNullOrEmpty($PreferredDns) -or [string]::IsNullOrEmpty($AlternateDns)) {
    Write-Host "You must be pass your DNS servers."
    exit
}

Set-DnsClientServerAddress -InterfaceAlias $InterfaceAlias -ServerAddresses($PreferredDns, $AlternateDns)
Write-Host "DNS Enabled."
