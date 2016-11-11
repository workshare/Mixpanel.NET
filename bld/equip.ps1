Task Default -Depends Equip


Task Equip `
	-Description "Installs tools required to build and test the project. Assumes chocolatey is installed" `
{
	$registryKey = "Registry::HKEY_CURRENT_USER\Software\Workshare\Development\Mixpanel.NET"
	$equipHash = (Get-FileHash .\equip.ps1).Hash
	
	if((Test-Path $registryKey) -eq $false) 
	{
		New-Item -Path $registryKey -force
	}

	$expectedEquipHash = Get-ItemProperty -Path $registryKey -Name "equipHash" -ErrorAction SilentlyContinue| Select-Object -ExpandProperty equipHash
	
	$requiresUpdate = $false
	
	if($equipHash -ne $expectedEquipHash)
	{
		$requiresUpdate = $true		
	}

	if( $requiresUpdate ) 
	{
		New-Item -Path  $registryKey -ErrorAction SilentlyContinue | Out-Null
		New-ItemProperty -Path $registryKey -Name "equipHash" -Value $equipHash -ErrorAction SilentlyContinue | Out-Null
		Set-ItemProperty -Path $registryKey -Name "equipHash" -Value $equipHash -ErrorAction SilentlyContinue | Out-Null
	}
}

function Add-EnvPath {
    param(
        [Parameter(Mandatory=$true)]
        [string] $Path,

        [ValidateSet('Machine', 'User', 'Session')]
        [string] $Container = 'Machine'
    )

    if ($Container -ne 'Session') {
        $containerMapping = @{
            Machine = [EnvironmentVariableTarget]::Machine
            User = [EnvironmentVariableTarget]::User
        }
        $containerType = $containerMapping[$Container]

        $persistedPaths = [Environment]::GetEnvironmentVariable('Path', $containerType) -split ';'
        if ($persistedPaths -notcontains $Path) {
            $persistedPaths = $persistedPaths + $Path | where { $_ }
            [Environment]::SetEnvironmentVariable('Path', $persistedPaths -join ';', $containerType)
        }
    }

    $envPaths = $env:Path -split ';'
    if ($envPaths -notcontains $Path) {
        $envPaths = $envPaths + $Path | where { $_ }
        $env:Path = $envPaths -join ';'
    }
}
