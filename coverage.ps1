nuget install coveralls.io -Version 1.3.4 -OutputDirectory .\tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory .\tools
nuget install ReportGenerator -Version 2.5.2 -OutputDirectory .\tools

$openCoverPath = Join-Path $PSScriptRoot "tools\OpenCover.4.6.519\tools\OpenCover.Console.exe"
$coverallsPath = Join-Path $PSScriptRoot "tools\coveralls.io.1.3.4\tools\coveralls.net.exe"
$reportGeneratorPath = Join-Path $PSScriptRoot "tools\ReportGenerator.2.5.2\tools\ReportGenerator.exe"
$testDllPath = Join-Path $PSScriptRoot "src\stashbox.tests\bin\release\Stashbox.Tests.dll"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

dir

$arguments = "-oldStyle", "-returntargetcode", "-register:user", "`"-filter:+[*]* -[Stashbox.Tests]* -[Stashbox]*.Utils*`"", "-target:vstest.console.exe", "`"-targetargs:$testDllPath`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. $openCoverPath $arguments
. $coverallsPath --serviceName appveyor --opencover -i .\coverage.xml
. $reportGeneratorPath -verbosity:Info -reports:coverage.xml -targetdir:$coverageReportDir "-assemblyfilters:-Stashbox.Tests*"