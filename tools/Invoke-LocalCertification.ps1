[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$SqlMasterConnectionString
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$sdkProject = Join-Path $repoRoot "src/SadrScales.Integration/SadrScales.Integration.csproj"
$unitTests = Join-Path $repoRoot "tests/SadrScales.Integration.Tests/SadrScales.Integration.Tests.csproj"
$sqlTests = Join-Path $repoRoot "tests/SadrScales.Integration.SqlTests/SadrScales.Integration.SqlTests.csproj"
$net48 = Join-Path $repoRoot "tests/SadrScales.Integration.Net48Consumer/SadrScales.Integration.Net48Consumer.csproj"
$vendorAcceptance = Join-Path $repoRoot "tests/SadrScales.Integration.VendorAcceptance/SadrScales.Integration.VendorAcceptance.csproj"
$quickStart = Join-Path $repoRoot "samples/csharp/SadrScales.Integration.QuickStart/SadrScales.Integration.QuickStart.csproj"
$sampleApp = Join-Path $repoRoot "samples/csharp/SadrScales.Integration.SampleApp/SadrScales.Integration.SampleApp.csproj"
$packageDir = Join-Path $repoRoot "artifacts/package"
$vendorPackageDir = Join-Path $repoRoot "artifacts/vendor-package"

[xml]$project = Get-Content $sdkProject -Raw
$version = [string]$project.SelectSingleNode("/Project/PropertyGroup/Version").InnerText
if ([string]::IsNullOrWhiteSpace($version)) { throw "SDK Version is missing." }

Write-Host ""
Write-Host "SadrScales.Integration local certification" -ForegroundColor Cyan
Write-Host "SDK candidate : $version"
Write-Host "Target runtime: SadrScales 5.5 RC"
Write-Host ""

Remove-Item $packageDir -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $vendorPackageDir -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $packageDir, $vendorPackageDir | Out-Null

Write-Host "1/9 Public repository boundary..."
& (Join-Path $repoRoot "tools/Validate-PublicRepository.ps1")
if ($LASTEXITCODE -ne 0) { throw "Public repository validation failed." }

Write-Host "2/9 SDK unit tests..."
& dotnet restore $unitTests
if ($LASTEXITCODE -ne 0) { throw "Unit-test restore failed." }
& dotnet build $unitTests --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "Unit-test build failed." }
& dotnet test $unitTests --configuration Release --no-build --verbosity normal
if ($LASTEXITCODE -ne 0) { throw "Unit tests failed." }

Write-Host "3/9 Modern .NET QuickStart..."
& dotnet restore $quickStart
if ($LASTEXITCODE -ne 0) { throw "QuickStart restore failed." }
& dotnet build $quickStart --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "QuickStart build failed." }

Write-Host "4/9 SDK pack + NuGet validation..."
& dotnet pack $sdkProject --configuration Release --output $packageDir
if ($LASTEXITCODE -ne 0) { throw "SDK pack failed." }
$nupkg = Get-ChildItem $packageDir -Filter "SadrScales.Integration.*.nupkg" | Where-Object { $_.Name -notlike "*.snupkg" } | Select-Object -First 1
if (-not $nupkg) { throw "SDK NuGet package was not produced." }
& (Join-Path $repoRoot "tools/Validate-NuGetPackage.ps1") -PackagePath $nupkg.FullName
if ($LASTEXITCODE -ne 0) { throw "NuGet package validation failed." }
Copy-Item (Join-Path $packageDir "*") $vendorPackageDir -Force

Write-Host "5/9 SQL integration tests..."
$previousSql = $env:SADR_INTEGRATION_TEST_SQL
$env:SADR_INTEGRATION_TEST_SQL = $SqlMasterConnectionString
try {
    & dotnet restore $sqlTests
    if ($LASTEXITCODE -ne 0) { throw "SQL-test restore failed." }
    & dotnet build $sqlTests --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "SQL-test build failed." }
    & dotnet test $sqlTests --configuration Release --no-build --verbosity normal
    if ($LASTEXITCODE -ne 0) { throw "SQL integration tests failed." }
}
finally {
    $env:SADR_INTEGRATION_TEST_SQL = $previousSql
}

Write-Host "6/9 .NET Framework 4.8 package consumer..."
$net48Config = Join-Path $repoRoot "tests/SadrScales.Integration.Net48Consumer/NuGet.CI.config"
& dotnet restore $net48 --configfile $net48Config --no-cache
if ($LASTEXITCODE -ne 0) { throw "net48 package restore failed." }
& dotnet build $net48 --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "net48 package consumer build failed." }
$net48Exe = Join-Path $repoRoot "tests/SadrScales.Integration.Net48Consumer/bin/Release/net48/SadrScales.Integration.Net48Consumer.exe"
& $net48Exe
if ($LASTEXITCODE -ne 0) { throw "net48 package compatibility smoke failed." }

Write-Host "7/9 Package-only Vendor Acceptance..."
$vendorConfig = Join-Path $repoRoot "tests/SadrScales.Integration.VendorAcceptance/NuGet.CI.config"
& dotnet restore $vendorAcceptance --configfile $vendorConfig --no-cache
if ($LASTEXITCODE -ne 0) { throw "Vendor Acceptance package restore failed." }
& dotnet build $vendorAcceptance --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "Vendor Acceptance build failed." }
$previousVendorSql = $env:SADR_VENDOR_ACCEPTANCE_SQL
$env:SADR_VENDOR_ACCEPTANCE_SQL = $SqlMasterConnectionString
try {
    & dotnet run --project $vendorAcceptance --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) { throw "Package-only Vendor Acceptance failed." }
}
finally {
    $env:SADR_VENDOR_ACCEPTANCE_SQL = $previousVendorSql
}

Write-Host "8/9 WinForms Developer Sample build..."
& dotnet restore $sampleApp
if ($LASTEXITCODE -ne 0) { throw "Sample App restore failed." }
& dotnet build $sampleApp --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "Sample App build failed." }

Write-Host "9/9 Candidate package hashes..."
$hashFile = Join-Path $packageDir "SHA256SUMS.txt"
$hashLines = Get-ChildItem $packageDir -File | Where-Object { $_.Name -ne "SHA256SUMS.txt" } | Sort-Object Name | ForEach-Object {
    $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $($_.Name)"
}
$hashLines | Set-Content $hashFile -Encoding ascii

Write-Host ""
Write-Host "LOCAL SDK PRE-CERTIFICATION PASS" -ForegroundColor Green
Write-Host "Candidate package: $($nupkg.FullName)"
Write-Host "This does not yet publish or claim SadrScales 5.5 certification." -ForegroundColor Yellow
