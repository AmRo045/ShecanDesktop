# The setup script that used to copy release directory content to Setup\Files\ directory 
# and change app version in setup creator config (.iss) file

# App name
$appName = "ShecanDesktop"
# Script base directory
$baseDirectory = $PSScriptRoot
# App release directoy
$releaseDirectory = Join-Path $baseDirectory "..\Source\ShecanDesktop\bin\Release\*"
# Setup files container directory
$setupFilesDirectory = Join-Path $baseDirectory "Files\"
# Output setup file directory
$setupOutputDirectory = Join-Path $baseDirectory "Output\"
# Main exe file full path
$mainFileFullPath = Join-Path $releaseDirectory.Replace("*", "") "$appName.exe"
# Get the main exe file version info
$mainFileVersionInfo = (Get-Item -Path $mainFileFullPath).VersionInfo;
# Set main file version
$mainFileVersion = "$($mainFileVersionInfo.FileMajorPart).$($mainFileVersionInfo.FileMinorPart).$($mainFileVersionInfo.FileBuildPart)"
# My app version
$myAppVersion = 'MyAppVersion "' + "$mainFileVersion" + '"'
# vshost file full path in setup container directory
$vshostFileName = Join-Path $setupFilesDirectory "$appName.vshost.exe"
# Setup config file path
$setupConfigFile = Join-Path $baseDirectory "SetupConfig.iss"

# Set script base directory as current directory
Set-Location $baseDirectory

# Delete old items
Remove-Item -Path $setupFilesDirectory -Recurse

# Create new directories
New-Item -Path $setupFilesDirectory -ItemType Directory

if (![System.IO.Directory]::Exists($setupOutputDirectory)) {
    New-Item -Path $setupOutputDirectory -ItemType Directory
}

# Copy dll files
copy-item -path $releaseDirectory -Filter *.dll -destination $setupFilesDirectory -recurse -Force -Verbose

# Copy exe files
copy-item -path $releaseDirectory -Filter *.exe -destination $setupFilesDirectory -recurse -Force -Verbose

# Delete .vshost file
if ([System.IO.File]::Exists($vshostFileName)) {
    Remove-Item -Path $vshostFileName
}

# Set main file version in setup creator config file
(Get-Content -Path $setupConfigFile) -Replace('MyAppVersion "(\d+\.)?(\d+\.)?(\d+\.)?(\*|\d+)"$', $myAppVersion) | Set-Content -Path $setupConfigFile 
