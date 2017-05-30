if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

nuget install OpenCover -Version 4.6.519 -OutputDirectory .\tools
nuget install ReportGenerator -Version 2.5.2 -OutputDirectory .\tools

$openCoverPath = Join-Path $PSScriptRoot "tools\OpenCover.4.6.519\tools\OpenCover.Console.exe"
$reportGeneratorPath = Join-Path $PSScriptRoot "tools\ReportGenerator.2.5.2\tools\ReportGenerator.exe"
$testPath = Join-Path $PSScriptRoot "src\stashbox.tests\stashbox.tests.csproj"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$env:Path += ";c:\Python34;C:\Python34\Scripts"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Stashbox.* -[Stashbox.Tests]* -[Stashbox]*.Utils* -[Stashbox]*.Expressions.Compile*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f net45 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. $openCoverPath $arguments
. pip install codecov
. codecov -f coverage.xml -X gcov
. $reportGeneratorPath -verbosity:Info -reports:coverage.xml -targetdir:$coverageReportDir "-assemblyfilters:-Stashbox.Tests*"