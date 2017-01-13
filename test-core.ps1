$testDllPath = Join-Path $PSScriptRoot "src\netcore\tests\bin\release\Stashbox.Tests.dll"
$vsTestPath = "c:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"

. $vsTestPath $testDllPath