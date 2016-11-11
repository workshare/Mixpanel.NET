
### MSBUILD FUNCTIONS ####

$msbuildExe = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"

function Clean-Solution
{
    param(        
        [string] $solutionPath,
        [string] $configuration
    )   
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

    tskill nunit-agent

    $extraArgs = @()

    foreach($value in $buildProperties) {
        $param = "/p:$value"
        $extraArgs += $param
    }
    
    Exec { 
        &$msbuildExe $path /t:Build /clp:ErrorsOnly /nr:false /p:Platform="$platform" /m /p:Configuration="$configuration" $extraArgs /nologo
    }
}