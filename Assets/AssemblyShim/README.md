# Default Assembly Shims

This project can use two tiny shim classes to ensure Unity always generates the default assemblies expected by some tooling (e.g., Burst IL resolver). With Burst 1.8.27+, these shims may not be necessary and are currently disabled by default:

- `Assembly-CSharp.dll`: optional shim at `Assets/AssemblyShim/AssemblyCSharpShim.cs.disabled` (rename to `.cs` to enable)
- `Assembly-CSharp-Editor.dll`: optional shim at `Assets/Scripts/Editor/EditorAssemblyShim.cs.disabled` (rename to `.cs` to enable)

## Policy
- Do not add any production code to folders that compile into the default assemblies.
- If you enable shims, keep them empty; they exist only to stabilize tooling that assumes the default assemblies exist.
- If new types appear in the default assemblies, the editor validator will warn you.

## Why
Some Unity packages (notably certain Burst versions) attempt to resolve `Assembly-CSharp` and `Assembly-CSharp-Editor` by name during entry-point scanning. Projects that are fully asmdef-based may not produce these by default, causing resolver failures.

## Guardrails
- An editor validator at `Assets/Scripts/Editor/ValidateDefaultAssemblies.cs` warns if non-shim types are found in either default assembly and supports a no-shim setup.
- If necessary, upgrade `com.unity.burst` via Package Manager to a version aligned with your Unity editor version to reduce resolver sensitivity.
