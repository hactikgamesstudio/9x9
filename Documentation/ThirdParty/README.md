# Third-Party Asset Organization Guide

This guide explains how third-party assets from the Unity Asset Store are organized in this project.

## Folder Structure

### Source Location
All third-party vendor packages are initially placed in:
```
Assets/ThirdParty/<VendorName>/<AssetName>/
```

### Organized by Type
Use the **Project Cleanup Tools** to automatically organize assets by type:

- **Models/Meshes** → `Assets/Models/ThirdParty/<VendorName>/`
- **Textures** → `Assets/Textures/ThirdParty/<VendorName>/`
- **Prefabs** → `Assets/Prefabs/ThirdParty/<VendorName>/`
- **Documentation** → `Documentation/ThirdParty/<VendorName>/`

## Automation Tool

**Menu:** Tools → Project Cleanup Tools → "Organize Third-Party by Type"

This tool:
1. Scans `Assets/ThirdParty/` recursively
2. Detects folders with keywords: "Mesh", "Model", "Texture", "Prefab", "Doc"
3. Moves them to centralized type-specific folders
4. Preserves vendor attribution in subfolder names
5. Copies documentation outside `Assets/` to `Documentation/ThirdParty/`

## Credits

All third-party assets are credited in:
```
Assets/ThirdParty/CREDITS.md
```

Update this file whenever you add new assets.

## Best Practices

1. **Import to ThirdParty first**
   - Always import Asset Store packages to `Assets/ThirdParty/<VendorName>/`
   - This keeps vendor code isolated

2. **Run organization tool**
   - After import, run "Organize Third-Party by Type"
   - Review the dry-run preview before applying

3. **Update credits**
   - Add vendor/author info to `CREDITS.md`
   - Include license and source information

4. **Keep demos separate**
   - Move demo scenes/assets to `Assets/_Unused/` after evaluation
   - Don't include in builds

5. **Version control**
   - Commit organized structure
   - Document which assets are used where

## Current Third-Party Assets

See `Assets/ThirdParty/CREDITS.md` for the complete list.

---

**Last Updated:** November 30, 2025
