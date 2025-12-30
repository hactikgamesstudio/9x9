# Unity CI (Batchmode Compile)

This repository uses [Game-CI Unity Builder](https://github.com/game-ci/unity-builder) to run a headless Unity compile on pull requests.

## Requirements
- Add a repository secret named `UNITY_LICENSE` containing your Unity license file (ULF) content.
  - In Unity Hub, go to Licenses → Activate/Manage, export the `.ulf` content, and paste it into the GitHub secret.
  - Both Personal and Pro licenses work with Game-CI. Ensure you are permitted to use CI under your license terms.

## Workflow
- Trigger: Pull requests to `Ground` and any `feature/**` branches, and pushes to `feature/**`.
- Job: Uses Unity 6000.2.10f1 to compile for `StandaloneLinux64`.
- Artifacts: Uploads Unity Editor logs for debugging.

## Verifying CI
1. Open the PR → Checks → Unity CI (Compile).
2. Inspect `unity-logs` artifact if a failure occurs.
3. Common causes:
   - Missing `UNITY_LICENSE` secret
   - Incorrect `unityVersion` vs `ProjectSettings/ProjectVersion.txt`
   - Script compilation errors in the project

## Notes
- This workflow is a compile check (no packaging/deployment). You can add platforms or builds later if needed.
- Keep default assemblies empty (see `Assets/Scripts/Editor/ValidateDefaultAssemblies.cs`). Our no‑shim setup is supported by Burst 1.8.27.
