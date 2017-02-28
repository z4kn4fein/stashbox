if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

nuget install coveralls.net -Version 0.7.0 -OutputDirectory .\tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory .\tools
nuget install ReportGenerator -Version 2.5.2 -OutputDirectory .\tools

$openCoverPath = Join-Path $PSScriptRoot "tools\OpenCover.4.6.519\tools\OpenCover.Console.exe"
$coverallsPath = Join-Path $PSScriptRoot "tools\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe"
$reportGeneratorPath = Join-Path $PSScriptRoot "tools\ReportGenerator.2.5.2\tools\ReportGenerator.exe"
$testPath = Join-Path $PSScriptRoot "src\stashbox.tests\stashbox.tests.csproj"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Stashbox.* -[Stashbox.Tests]* -[Stashbox]*.Utils* -[Stashbox]*.Expressions.Compile*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f net45 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. $openCoverPath $arguments
. $coverallsPath --opencover -i coverage.xml
. $reportGeneratorPath -verbosity:Info -reports:coverage.xml -targetdir:$coverageReportDir "-assemblyfilters:-Stashbox.Tests*"