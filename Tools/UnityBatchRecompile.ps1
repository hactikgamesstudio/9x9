Param(
    [string]$ProjectPath = (Get-Location).Path,
    [string]$LogFile = "unity_recompile.log",
    [switch]$NoExit
)

function Write-Info($msg){ Write-Host "[INFO] $msg" -ForegroundColor Cyan }
function Write-Warn($msg){ Write-Host "[WARN] $msg" -ForegroundColor Yellow }
function Write-Err($msg){ Write-Host "[ERROR] $msg" -ForegroundColor Red }

$editorCandidates = @()
if($Env:UNITY_EDITOR_PATH){ $editorCandidates += $Env:UNITY_EDITOR_PATH }
$editorCandidates += @(
    "C:\\Program Files\\Unity\\Hub\\Editor\\6000.2.10f1\\Editor\\Unity.exe",
    "C:\\Program Files\\Unity\\Hub\\Editor\\6000.2.10f1f1\\Editor\\Unity.exe",
    "C:\\Program Files (x86)\\Unity\\Editor\\Unity.exe"
)
$editorCandidates = $editorCandidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique

if(-not $editorCandidates -or $editorCandidates.Count -eq 0){
    Write-Err "No Unity Editor executable found. Set UNITY_EDITOR_PATH env var to your Unity.exe."
    Write-Warn "Example: setx UNITY_EDITOR_PATH 'C:\\Program Files\\Unity\\Hub\\Editor\\6000.2.10f1\\Editor\\Unity.exe'"
    exit 1
}

$unityExe = $editorCandidates[0]
Write-Info "Using Unity Editor: $unityExe"

$fullLog = Join-Path $ProjectPath $LogFile
if(Test-Path $fullLog){ Remove-Item $fullLog -Force }

Write-Info "Starting batchmode recompile..."
& $unityExe -batchmode -quit -projectPath $ProjectPath -logFile $fullLog
$exitCode = $LASTEXITCODE

if(Test-Path $fullLog){
    Write-Info "Log (first 80 lines):"
    Get-Content $fullLog -First 80
} else {
    Write-Warn "Log file not created: $fullLog"
}

if($exitCode -ne 0){
    Write-Err "Unity exited with code $exitCode"
} else {
    Write-Info "Unity batch recompile completed successfully (exit 0)."
}

if(-not $NoExit){ exit $exitCode }
