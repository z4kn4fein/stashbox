$version = $ENV:APPVEYOR_BUILD_VERSION
$csprojPath = Join-Path $PSScriptRoot "src\stashbox\stashbox.csproj"
[xml]$project = Get-Content -Path $csprojPath
$project.Project.PropertyGroup[0].Version = $version
$project.Save($csprojPath)