[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$PackagePath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resolvedPackage = (Resolve-Path -LiteralPath $PackagePath).Path
if (-not $resolvedPackage.EndsWith('.nupkg', [System.StringComparison]::OrdinalIgnoreCase) -or
    $resolvedPackage.EndsWith('.snupkg', [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Expected a NuGet .nupkg file: $resolvedPackage"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($resolvedPackage)

try {
    $entries = @($archive.Entries)
    $entryNames = @($entries | ForEach-Object { $_.FullName })

    $requiredEntries = @(
        'lib/netstandard2.0/SadrScales.Integration.dll',
        'lib/netstandard2.0/SadrScales.Integration.xml',
        'PACKAGE_README.md'
    )

    foreach ($required in $requiredEntries) {
        if ($entryNames -notcontains $required) {
            throw "Required package entry is missing: $required"
        }
    }

    $nuspecEntries = @($entries | Where-Object { $_.FullName -match '^[^/]+\.nuspec$' })
    if ($nuspecEntries.Count -ne 1) {
        throw "Expected exactly one root .nuspec entry, found $($nuspecEntries.Count)."
    }

    $reader = New-Object System.IO.StreamReader($nuspecEntries[0].Open())
    try {
        [xml]$nuspec = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    function Get-NuspecText([string]$ElementName) {
        $node = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']/*[local-name()='$ElementName']")
        if ($null -eq $node) {
            return $null
        }

        return [string]$node.InnerText
    }

    $id = Get-NuspecText 'id'
    $version = Get-NuspecText 'version'
    $readme = Get-NuspecText 'readme'
    $authors = Get-NuspecText 'authors'
    $copyright = Get-NuspecText 'copyright'

    if ($id -ne 'SadrScales.Integration') {
        throw "Unexpected package id: $id"
    }

    if ([string]::IsNullOrWhiteSpace($version)) {
        throw 'Package version is missing.'
    }

    if ($readme -ne 'PACKAGE_README.md') {
        throw "Unexpected package readme metadata: $readme"
    }

    if ($authors -notmatch 'Tozin Sadr' -or $authors -notmatch 'Behzad Erfanian') {
        throw "Package authors must identify both Tozin Sadr and Behzad Erfanian. Actual: $authors"
    }

    if ($copyright -notmatch 'Tozin Sadr' -or $copyright -notmatch 'Behzad Erfanian') {
        throw "Package copyright must identify both Tozin Sadr and Behzad Erfanian. Actual: $copyright"
    }

    $license = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']/*[local-name()='license']")
    if ($null -eq $license) {
        throw 'NuGet license metadata is missing.'
    }

    $licenseType = [string]$license.GetAttribute('type')
    $licenseValue = [string]$license.InnerText
    if ($licenseType -ne 'expression' -or $licenseValue -ne 'MIT') {
        throw "Expected MIT license expression metadata. Actual type='$licenseType', value='$licenseValue'."
    }

    $repository = $nuspec.SelectSingleNode("/*[local-name()='package']/*[local-name()='metadata']/*[local-name()='repository']")
    if ($null -eq $repository) {
        throw 'NuGet repository metadata is missing. Enable PublishRepositoryUrl/Source Link.'
    }

    $repositoryType = [string]$repository.GetAttribute('type')
    $repositoryUrl = [string]$repository.GetAttribute('url')
    $repositoryCommit = [string]$repository.GetAttribute('commit')

    if ($repositoryType -ne 'git') {
        throw "Unexpected repository type: $repositoryType"
    }

    if ($repositoryUrl -notmatch '^https://github\.com/BehzadErfanian/SadrScales-Integration(?:\.git)?$') {
        throw "Unexpected repository URL: $repositoryUrl"
    }

    if ($repositoryCommit -notmatch '^[0-9a-fA-F]{40}$') {
        throw "Repository commit metadata is missing or invalid: $repositoryCommit"
    }

    Write-Host 'NuGet package validation passed.'
    Write-Host "Package   : $id $version"
    Write-Host "Authors   : $authors"
    Write-Host "License   : $licenseValue"
    Write-Host "Copyright : $copyright"
    Write-Host "Commit    : $repositoryCommit"
}
finally {
    $archive.Dispose()
}
