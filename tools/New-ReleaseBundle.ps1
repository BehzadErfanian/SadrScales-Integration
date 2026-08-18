[CmdletBinding()]
param(
    [string]$Version = '1.0.0',
    [string]$PackageDirectory = '',
    [string]$OutputDirectory = '',
    [string]$GuidePath = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))

if ([string]::IsNullOrWhiteSpace($PackageDirectory)) {
    $PackageDirectory = Join-Path $repoRoot 'artifacts/package'
}
else {
    $PackageDirectory = [System.IO.Path]::GetFullPath($PackageDirectory)
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot 'artifacts/release'
}
else {
    $OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
}

$projectPath = Join-Path $repoRoot 'src/SadrScales.Integration/SadrScales.Integration.csproj'
[xml]$project = Get-Content -LiteralPath $projectPath -Raw
$projectVersionNode = $project.SelectSingleNode('/Project/PropertyGroup/Version')
if ($null -eq $projectVersionNode) {
    throw 'SDK project Version is missing.'
}

$projectVersion = [string]$projectVersionNode.InnerText
if ($projectVersion -ne $Version) {
    throw "Requested release version '$Version' does not match SDK project version '$projectVersion'."
}

$nupkg = Join-Path $PackageDirectory ("SadrScales.Integration.{0}.nupkg" -f $Version)
$snupkg = Join-Path $PackageDirectory ("SadrScales.Integration.{0}.snupkg" -f $Version)

foreach ($required in @($nupkg, $snupkg)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) {
        throw "Required package was not found: $required"
    }
}

& (Join-Path $PSScriptRoot 'Validate-NuGetPackage.ps1') -PackagePath $nupkg

if (Test-Path -LiteralPath $OutputDirectory) {
    Remove-Item -LiteralPath $OutputDirectory -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

Copy-Item -LiteralPath $nupkg -Destination $OutputDirectory
Copy-Item -LiteralPath $snupkg -Destination $OutputDirectory

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("SadrScalesIntegrationRelease-" + [Guid]::NewGuid().ToString('N'))
$extractRoot = Join-Path $tempRoot 'package'
$binaryStage = Join-Path $tempRoot 'binaries'
$developerStage = Join-Path $tempRoot 'developer-kit'

New-Item -ItemType Directory -Path $extractRoot, $binaryStage, $developerStage -Force | Out-Null

try {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($nupkg, $extractRoot)

    $dllPath = Join-Path $extractRoot 'lib/netstandard2.0/SadrScales.Integration.dll'
    $xmlPath = Join-Path $extractRoot 'lib/netstandard2.0/SadrScales.Integration.xml'
    foreach ($required in @($dllPath, $xmlPath)) {
        if (-not (Test-Path -LiteralPath $required -PathType Leaf)) {
            throw "Expected SDK binary/documentation was not found in package: $required"
        }
    }

    Copy-Item -LiteralPath $dllPath -Destination $binaryStage
    Copy-Item -LiteralPath $xmlPath -Destination $binaryStage
    Copy-Item -LiteralPath (Join-Path $repoRoot 'src/SadrScales.Integration/PACKAGE_README.md') -Destination (Join-Path $binaryStage 'README.md')
    Copy-Item -LiteralPath (Join-Path $repoRoot 'NOTICE.md') -Destination $binaryStage
    Copy-Item -LiteralPath (Join-Path $repoRoot 'SUPPORT.md') -Destination $binaryStage

    $licensePath = Join-Path $repoRoot 'LICENSE'
    if (Test-Path -LiteralPath $licensePath -PathType Leaf) {
        Copy-Item -LiteralPath $licensePath -Destination $binaryStage
    }

    $binaryZip = Join-Path $OutputDirectory ("SadrScales.Integration-{0}-Binaries.zip" -f $Version)
    Compress-Archive -Path (Join-Path $binaryStage '*') -DestinationPath $binaryZip -CompressionLevel Optimal

    $developerCopyPlan = @(
        @{ Source = 'samples/csharp'; Destination = 'samples/csharp' },
        @{ Source = 'samples/SQL'; Destination = 'samples/SQL' },
        @{ Source = 'docs/en'; Destination = 'docs/en' },
        @{ Source = 'docs/fa'; Destination = 'docs/fa' }
    )

    foreach ($entry in $developerCopyPlan) {
        $source = Join-Path $repoRoot $entry.Source
        $destination = Join-Path $developerStage $entry.Destination
        New-Item -ItemType Directory -Path ([System.IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
        Copy-Item -LiteralPath $source -Destination $destination -Recurse
    }

    foreach ($file in @(
        'README.md',
        'README.fa.md',
        'NOTICE.md',
        'SUPPORT.md',
        'CONTRIBUTING.md',
        'CHANGELOG.md',
        'docs/COMPATIBILITY.md',
        'docs/API_COMPATIBILITY.md',
        'docs/SDK_DESIGN_V1.md',
        'docs/SECURITY_BOUNDARY.md',
        'docs/CONTRACT_V1_FREEZE.md',
        'docs/PRODUCTION_READINESS_CHECKLIST.md'
    )) {
        $source = Join-Path $repoRoot $file
        $destination = Join-Path $developerStage $file
        New-Item -ItemType Directory -Path ([System.IO.Path]::GetDirectoryName($destination)) -Force | Out-Null
        Copy-Item -LiteralPath $source -Destination $destination
    }

    if (Test-Path -LiteralPath $licensePath -PathType Leaf) {
        Copy-Item -LiteralPath $licensePath -Destination $developerStage
    }

    $developerZip = Join-Path $OutputDirectory ("SadrScales.Integration-{0}-DeveloperKit.zip" -f $Version)
    Compress-Archive -Path (Join-Path $developerStage '*') -DestinationPath $developerZip -CompressionLevel Optimal

    $releaseNotesSource = Join-Path $repoRoot ("docs/releases/v{0}.md" -f $Version)
    if (-not (Test-Path -LiteralPath $releaseNotesSource -PathType Leaf)) {
        throw "Release notes were not found: $releaseNotesSource"
    }
    Copy-Item -LiteralPath $releaseNotesSource -Destination (Join-Path $OutputDirectory 'RELEASE_NOTES.md')

    if (-not [string]::IsNullOrWhiteSpace($GuidePath)) {
        $resolvedGuide = [System.IO.Path]::GetFullPath($GuidePath)
        if (-not (Test-Path -LiteralPath $resolvedGuide -PathType Leaf)) {
            throw "Guide file was not found: $resolvedGuide"
        }

        $guideDefinitionPath = Join-Path $repoRoot 'docs/reference/integration-guide-5.2.1.json'
        $guideDefinition = Get-Content -LiteralPath $guideDefinitionPath -Raw | ConvertFrom-Json
        $guideName = [string]$guideDefinition.fileName
        $expectedGuideHash = ([string]$guideDefinition.sha256).ToLowerInvariant()
        $actualGuideHash = (Get-FileHash -LiteralPath $resolvedGuide -Algorithm SHA256).Hash.ToLowerInvariant()

        if ($actualGuideHash -ne $expectedGuideHash) {
            throw "Guide SHA-256 mismatch. Expected $expectedGuideHash but got $actualGuideHash."
        }

        Copy-Item -LiteralPath $resolvedGuide -Destination (Join-Path $OutputDirectory $guideName)
    }

    $gitCommit = 'unknown'
    try {
        $candidate = (& git -C $repoRoot rev-parse HEAD 2>$null).Trim()
        if (-not [string]::IsNullOrWhiteSpace($candidate)) {
            $gitCommit = $candidate
        }
    }
    catch {
        $gitCommit = 'unknown'
    }

    $records = @()
    foreach ($file in Get-ChildItem -LiteralPath $OutputDirectory -File | Sort-Object Name) {
        if ($file.Name -in @('SHA256SUMS.txt', 'release-manifest.json')) {
            continue
        }

        $records += [ordered]@{
            file = $file.Name
            sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
            sizeBytes = [long]$file.Length
        }
    }

    $manifest = [ordered]@{
        schemaVersion = 1
        product = 'SadrScales.Integration'
        version = $Version
        sqlContract = 'v1'
        sadrScalesBaseline = '5.2.1'
        providers = @('Tozin Sadr', 'Behzad Erfanian')
        license = 'MIT'
        gitCommit = $gitCommit
        generatedUtc = [DateTime]::UtcNow.ToString('o')
        files = $records
    }

    $manifestPath = Join-Path $OutputDirectory 'release-manifest.json'
    $manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

    $hashLines = @()
    foreach ($file in Get-ChildItem -LiteralPath $OutputDirectory -File | Where-Object { $_.Name -ne 'SHA256SUMS.txt' } | Sort-Object Name) {
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        $hashLines += ("{0} *{1}" -f $hash, $file.Name)
    }
    $hashLines | Set-Content -LiteralPath (Join-Path $OutputDirectory 'SHA256SUMS.txt') -Encoding ASCII

    Write-Host ''
    Write-Host 'SadrScales.Integration release bundle created.' -ForegroundColor Green
    Write-Host ("Version : {0}" -f $Version)
    Write-Host ("Output  : {0}" -f $OutputDirectory)
    Get-ChildItem -LiteralPath $OutputDirectory -File | Sort-Object Name | ForEach-Object {
        Write-Host (" - {0}" -f $_.Name)
    }
}
finally {
    if (Test-Path -LiteralPath $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
