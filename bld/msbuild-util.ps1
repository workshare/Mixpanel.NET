
### MSBUILD FUNCTIONS ####

function Get-MsBuildExe {

    $msbuild = vswhere -latest -property installationPath
    if($msbuild.Contains("2019")){
        $msbuild = Join-Path $msbuild "MSBuild\Current\Bin\msbuild.exe"
    }
    else {
        $msbuild = Join-Path $msbuild "MSBuild\15.0\Bin\msbuild.exe"
    }
    return $msbuild
}

function Clean-Solution
{
    param(        
        [string] $solutionPath,
        [string] $configuration
    )   
    $msbuildExe = Get-MsBuildExe
    Exec { &$msbuildExe $solutionPath /t:Clean /nr:false /clp:ErrorsOnly /m /p:Configuration="$configuration" /nologo }
}


function Build-Project 
{
    param(        
        [string] $path,
        [string] $configuration,
        [string] $platform,
        [array] $buildProperties
    )   

    Stop-Process -name nunit-agent -ErrorAction Continue

    $extraArgs = @()

    foreach($value in $buildProperties) {
        $param = "/p:$value"
        $extraArgs += $param
    }
    
    $msbuildExe = Get-MsBuildExe
    Exec { &$msbuildExe $path /t:Build /clp:ErrorsOnly /nr:false /p:Platform="$platform" /m /p:Configuration="$configuration" $extraArgs /nologo }
}