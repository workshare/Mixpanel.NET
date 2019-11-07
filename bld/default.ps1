
$global:CreateInstaller = $true

Task Default -Depends Build

Task DefaultWithOutInstaller -Depends NotInstaller, Default

Task Build `
	-Description "Build project and run unit test cases and create installer based on condition" `
{
	Invoke-psake build.ps1 -parameters @{config='Release'}
	Invoke-psake build.ps1 -parameters @{config='Release-Net452'}
	Invoke-psake build.ps1 -parameters @{config='Release-Net40'}
	
	if( $CreateInstaller -eq $true )
	{
		Invoke-psake build.ps1 Compile-Installers -parameters @{config='Release'}
	}
	
}

Task NotInstaller `
{
	$global:CreateInstaller = $false
}