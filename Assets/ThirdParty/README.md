# ThirdParty Folder

Vendor assets imported from the Unity Asset Store or external sources are organized here. This keeps project code and third‑party code separate for easier updates and troubleshooting.

- Place vendor packages under `Assets/ThirdParty/<PackageName>`
- Keep demos/samples optional; consider moving them to `Assets/_Unused` after evaluation
- Assembly definition `ThirdParty.asmdef` isolates compile scope

Use Tools → Project Cleanup Tools → "Organize Third-Party Assets" to auto-move obvious vendor folders.
