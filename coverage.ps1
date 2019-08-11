﻿if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

choco install opencover.portable --no-progress
choco install codecov --no-progress

$testPath = Join-Path $PSScriptRoot "test\stashbox.tests.csproj"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Stashbox.* -[Stashbox.Tests]* -[Stashbox]*.Utils* -[Stashbox]*.Expressions.Compile*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f net45 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. OpenCover.Console.exe $arguments
. type coverage.xml
. codecov -f coverage.xml 