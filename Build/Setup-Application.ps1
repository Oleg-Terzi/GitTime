# Import modules

Import-Module WebAdministration

# Define variables

$CurrentPath = $pwd.Path

$WebSiteName = "Local GitTime"
$WebSitePath = "D:\Apps\GitTime\Source\GitTime.Web"

# Remove the existing web site

Set-Location IIS:\Sites

if ( Test-Path $WebSiteName ) { Remove-Item $WebSiteName -Recurse }

# Create web site bindings (note the SSL binding must exist before the web site binding is created)

New-WebSite -Name $WebSiteName -Port 80 -HostHeader local-gittime.millerdatabases.com -PhysicalPath $WebSitePath -ApplicationPool IntegratedAppPool

# Start the web site

Start-WebSite $WebSiteName

# Restore the current location

Set-Location $CurrentPath
