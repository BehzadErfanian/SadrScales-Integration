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

    if ($id -ne 'SadrScales.Integration') {
        throw "Unexpected package id: $id"
    }

    if ([string]::IsNullOrWhiteSpace($version)) {
        throw 'Package version is missing.'
    }

    if ($readme -ne 'PACKAGE_README.md') {
        throw "Unexpected package readme metadata: $readme"
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

    Write-Host "NuGet package validation passed."
    Write-Host "Package : $id $version"
    Write-Host "Commit  : $repositoryCommit"
}
finally {
    $archive.Dispose()
}
