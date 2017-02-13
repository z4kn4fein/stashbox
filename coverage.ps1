nuget install coveralls.net -Version 0.412.0 -OutputDirectory .\tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory .\tools
nuget install ReportGenerator -Version 2.5.2 -OutputDirectory .\tools

$openCoverPath = Join-Path $PSScriptRoot "tools\OpenCover.4.6.519\tools\OpenCover.Console.exe"
$coverallsPath = Join-Path $PSScriptRoot "tools\coveralls.net.0.412\tools\csmacnz.Coveralls.exe"
$reportGeneratorPath = Join-Path $PSScriptRoot "tools\ReportGenerator.2.5.2\tools\ReportGenerator.exe"
$testPath = Join-Path $PSScriptRoot "src\stashbox.tests"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$arguments = "-oldStyle", "-returntargetcode", "-register:user", "`"-filter:+[*]* -[Stashbox.Tests]* -[Stashbox]*.Utils*`"", "-target:dotnet.exe", "`"-targetargs:$testPath`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. $openCoverPath $arguments

dir

. $coverallsPath --serviceName appveyor --opencover -i .\coverage.xml
. $reportGeneratorPath -verbosity:Info -reports:coverage.xml -targetdir:$coverageReportDir "-assemblyfilters:-Stashbox.Tests*"