if( $config -eq $null )
{
	$config = "Release"
}

if( $env:VERSION_ID -eq $null )
{
	$version = "0.1.0.0"
}
else 
{
    $version = $env:VERSION_ID
}

$dir_root = ".."
$dir_output = "..\artifacts"
$dir_bld = "$dir_root\bld"
$dir_module = ".."
$dir_source = "..\src"
$dir_root_output = "$dir_root\artifacts"
$dir_common = "$dir_source"
$product = "Mixpanel.NET"

$solution = "$dir_source\Mixpanel.NET\Mixpanel.NET.sln"

Include msbuild-util.ps1
Include EPS.ps1

### MAIN VERBS ###

Task Default -Depends Compile, Test

Task Equip `
	-Description "Installs tools required to build and test the project. Assumes chocolatey is installed" `
{
	Invoke-psake equip.ps1
}

Task Clean `
	-Description "Removes generated files" `
	-Depends Clean-Artifacts, Clean-Source `
{
	
}

Task Mutate `
	-Description "Performs code generation tasks" `
	-Depends Update-Version `
{
}

Task Compile `
	-Description "Turns code into executables" `
	-Depends Equip, Clean, Mutate, Compile-Source
{
}

Task Test `
	-Description "Runs all tests" `
	-Depends Clean-TestResults
{
}


Task Package `
	-Depends Compile-Installers `
	-Description "Turns executables into artifacts" `
{    
}


Task Deploy `
	-Description "Release artifacts into production" `
{
}

### MAIN VERB DEPENDENCIES ####

function Get-ResolvedPath($path) 
{
    $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($path)
}


Task Clean-Source {
	Clean-Solution -SolutionPath $solution -Configuration $config
}

function Clean-Directory($directory)
{
    $absolute_dir = Get-ResolvedPath $directory
    if ((Test-Path -Path $absolute_dir) -eq $true)
    {
        Remove-Item $absolute_dir -Recurse -Force | Out-Null
    }

    New-Item -ItemType Directory $absolute_dir | Out-Null
}

Task Clean-Artifacts {
	if( (Test-Path $dir_output) -eq $true) 
	{
		remove-item $dir_output -force -recurse -confirm:$false
	}

	mkdir $dir_output -erroraction SilentlyContinue | Out-Null
}

Task Clean-TestResults {
	Clean-Directory $dir_module\TestResults
}

function unix2dos {
	param([Parameter(Mandatory=$true, ValueFromPipeline=$True)][String]$line)
	begin {}
	process {
		$result = [string]$line.replace("`n","`r`n")
		$result
	}
	end {}
}

function Get-ProductVersion($ver)
{
    $productVersion = [System.Version]($ver)
    $productVersion = new-object System.Version($productVersion.Major, $productVersion.Minor, $productVersion.Build, 0)
    $productVersion = $productVersion.ToString()
    return $productVersion
}

Task Update-Version {
	$productVersion = Get-ProductVersion $version
	$text = EPS-Render -file "$dir_common\GlobalAssemblyInfo.cs.eps"
	$text = unix2dos $text
	sc $dir_common\GlobalAssemblyInfo.cs $text
}

Task Compile-Source -Depends Get-Dependencies {
	Build-Project $solution -Platform "Any CPU" -Configuration $config
}


Task Compile-Installers `
	-Depends Equip, Clean-Artifacts, Mutate, Run-Agreement-Tool `
{	
	mkdir $dir_output -erroraction SilentlyContinue | Out-Null
	$dir_output_absolute = resolve-path $dir_output
	pushd .
	cd "$dir_source\Workshare.Mixpanel.NET"
	Exec {
		&nuget pack Workshare.Mixpanel.NET.csproj -includereferencedprojects -OutputDirectory $dir_output_absolute -Properties Configuration=$config -symbols
	}
	popd
}

Task Run-Agreement-Tool {
	$script:dotnetExe = (get-command dotnet).Source
	Write-Host "Found dotnet here: $dotnetExe"
	pushd .
	cd "$dir_source\Workshare.Mixpanel.NET"
	Exec {
		AddMsBuildPathIfNecessary
		&nuget install Litera.Agreements.Generator -o . -excludeversion
		&$dotnetExe Litera.Agreements.Generator\tools\Litera.Agreements.Generator.dll -p "Mixpanel.NET" -d "..\Mixpanel.NET" -o .
	}
	
	popd
}

Task Get-Dependencies {
	AddMsBuildPathIfNecessary
	& nuget restore $solution
}

