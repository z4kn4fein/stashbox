if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

choco install opencover.portable --no-progress
choco install codecov --no-progress

$testPath = Join-Path $PSScriptRoot "test\stashbox.tests.csproj"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Stashbox.* -[Stashbox.Tests]* -[Stashbox]*.Utils* -[Stashbox]*.Expressions.Compile*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f netcoreapp3.1 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All", "-oldStyle"
. OpenCover.Console.exe $arguments
. codecov -f coverage.xml 