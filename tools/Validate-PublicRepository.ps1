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
    'LICENSE',
    'NOTICE.md',
    'SECURITY.md',
    'SUPPORT.md',
    'CONTRIBUTING.md',
    'CODE_OF_CONDUCT.md',
    'AGENTS.md',
    '.github/CODEOWNERS',
    '.github/dependabot.yml',
    '.github/ISSUE_TEMPLATE/config.yml',
    '.github/ISSUE_TEMPLATE/bug_report.yml',
    '.github/ISSUE_TEMPLATE/feature_request.yml',
    '.github/pull_request_template.md',
    '.github/workflows/public-repo-guard.yml',
    '.github/workflows/sdk-ci.yml',
    '.github/workflows/release.yml',
    'docs/PROJECT_STATUS.md',
    'docs/DECISIONS.md',
    'docs/ROADMAP.md',
    'docs/BACKLOG.md',
    'docs/API_COMPATIBILITY.md',
    'docs/COMPATIBILITY.md',
    'docs/PRODUCTION_READINESS_CHECKLIST.md',
    'docs/SECURITY_BOUNDARY.md'
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $file))) {
        throw "Required governance/release file missing: $file"
    }
}

$licensePath = Join-Path $RepositoryRoot 'LICENSE'
$licenseText = Get-Content -LiteralPath $licensePath -Raw
if ($licenseText -notmatch 'MIT License') {
    throw 'LICENSE is not the expected MIT License.'
}
if ($licenseText -notmatch 'Tozin Sadr and Behzad Erfanian') {
    throw 'LICENSE must identify both Tozin Sadr and Behzad Erfanian.'
}

$codeOwners = Get-Content -LiteralPath (Join-Path $RepositoryRoot '.github/CODEOWNERS') -Raw
if ($codeOwners -notmatch '@BehzadErfanian') {
    throw 'CODEOWNERS must identify @BehzadErfanian as repository owner/reviewer.'
}

Write-Host 'Public repository validation passed.' -ForegroundColor Green
