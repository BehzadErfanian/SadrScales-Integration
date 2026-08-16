param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

$forbiddenExtensions = @(
    '.pcap', '.pcapng', '.cap',
    '.pfx', '.p12', '.key', '.snk',
    '.mdf', '.ldf', '.bak', '.bacpac'
)

$forbiddenNamePatterns = @(
    'license-private',
    'private-key',
    'diagnostic-client-key',
    'customer-database',
    'production-secret'
)

$violations = New-Object System.Collections.Generic.List[string]

Get-ChildItem -LiteralPath $RepositoryRoot -Recurse -File -Force |
    Where-Object { $_.FullName -notmatch '[\\/]\.git[\\/]' } |
    ForEach-Object {
        $relative = ($_.FullName.Substring($RepositoryRoot.Length) -replace '^[\\/]+', '')
        $ext = $_.Extension.ToLowerInvariant()

        if ($forbiddenExtensions -contains $ext) {
            $violations.Add("Forbidden public file type: $relative")
        }

        $lower = $relative.ToLowerInvariant()
        foreach ($pattern in $forbiddenNamePatterns) {
            if ($lower.Contains($pattern)) {
                $violations.Add("Forbidden/sensitive filename pattern '$pattern': $relative")
            }
        }
    }

if ($violations.Count -gt 0) {
    Write-Host 'PUBLIC REPOSITORY VALIDATION FAILED' -ForegroundColor Red
    $violations | Sort-Object -Unique | ForEach-Object { Write-Host " - $_" -ForegroundColor Red }
    exit 1
}

$requiredFiles = @(
    'README.md',
    'README.fa.md',
    'SECURITY.md',
    'AGENTS.md',
    'docs/PROJECT_STATUS.md',
    'docs/DECISIONS.md',
    'docs/ROADMAP.md',
    'docs/BACKLOG.md',
    'docs/SECURITY_BOUNDARY.md'
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $file))) {
        throw "Required governance file missing: $file"
    }
}

Write-Host 'Public repository validation passed.' -ForegroundColor Green
