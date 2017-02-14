param($projectPath)

$version = $ENV:APPVEYOR_BUILD_VERSION
$csprojPath = Join-Path $PSScriptRoot $projectPath
[xml]$project = Get-Content -Path $csprojPath
$project.Project.PropertyGroup[0].DebugType = "full"
$project.Project.PropertyGroup[0].DebugSymbols = "true"
$project.Save($csprojPath)