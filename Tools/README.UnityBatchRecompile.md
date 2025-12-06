# Unity Batch Recompile Utility

The `Tools/UnityBatchRecompile.ps1` script forces a clean Unity project script recompile without launching the editor UI.

## When To Use
- After large refactors (moving/removing many scripts)
- After fixing circular / ambiguous assembly references
- To validate CI environment can compile scripts headlessly
- To quickly surface compile errors before opening the Editor

## Requirements
- Installed Unity editor (matching project version: 6000.2.10f1)
- PowerShell 5.1+ (default on Windows)

## Setup
Set the `UNITY_EDITOR_PATH` environment variable if auto-detection fails.
```powershell
setx UNITY_EDITOR_PATH "C:\Program Files\Unity\Hub\Editor\6000.2.10f1\Editor\Unity.exe"
# Restart shell or set for current session:
$Env:UNITY_EDITOR_PATH = "C:\Program Files\Unity\Hub\Editor\6000.2.10f1\Editor\Unity.exe"
```

## Usage
```powershell
powershell -ExecutionPolicy Bypass -File Tools/UnityBatchRecompile.ps1
```
Default log location: `unity_recompile.log` in project root.

Custom parameters:
```powershell
powershell -File Tools/UnityBatchRecompile.ps1 -ProjectPath C:\Path\To\9x9 -LogFile build_log.txt
```

## Successful Run
- Exit code `0`
- Log shows standard Unity startup messages then script compilation summary

## Failure Indicators
- Non-zero exit code
- Log contains compiler errors or missing packages

## Tips
- Delete `Library/ScriptAssemblies` before running to guarantee fresh compile
- Keep Unity Hub closed to avoid file locks on packages
- Use in CI by setting the environment variable and invoking the script

## CI Example (GitHub Actions)
```yaml
- name: Batch Recompile
  run: |
    setx UNITY_EDITOR_PATH "C:\\Unity\\6000.2.10f1\\Editor\\Unity.exe"
    powershell -ExecutionPolicy Bypass -File Tools/UnityBatchRecompile.ps1 -LogFile ci_compile.log
```

## Related Documents
- `README.md` (Developer Utilities section)
- `Documentation/Development/BUILD_PLAN.md`

---
Maintained: November 2025
