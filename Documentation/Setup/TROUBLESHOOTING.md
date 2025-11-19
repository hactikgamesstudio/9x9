# 9x9 Unity Multiplayer - Troubleshooting Guide

## Project Information
**Unity Version**: 6000.2.10f1  
**Netcode Version**: 2.3.2  
**Last Updated**: November 13, 2025

---

## Table of Contents
1. [Editor Issues](#editor-issues)
2. [Build Issues](#build-issues)
3. [Network Issues](#network-issues)
4. [Unity Gaming Services Issues](#unity-gaming-services-issues)
5. [Performance Issues](#performance-issues)
6. [Common Errors](#common-errors)

---

## Editor Issues

### Problem: Unity Editor Won't Open Project
**Symptoms**: Unity crashes or hangs when opening the project

**Solutions**:
1. Delete the `Library` folder and let Unity regenerate it
   ```powershell
   Remove-Item -Recurse -Force "C:\Users\kiidh\9x9\Library"
   ```
2. Delete the `Temp` folder
   ```powershell
   Remove-Item -Recurse -Force "C:\Users\kiidh\9x9\Temp"
   ```
3. Check Unity Hub for correct editor version (6000.2.10f1)
4. Verify disk space is available (at least 10GB free)
5. Check Editor log: `%LOCALAPPDATA%\Unity\Editor\Editor.log`

---

### Problem: Missing Package Errors
**Symptoms**: Red errors about missing packages in Console

**Solutions**:
1. Open **Window > Package Manager**
2. Click **Refresh** button (circular arrow)
3. If packages still missing, restore from manifest:
   ```powershell
   # Close Unity first
   cd "C:\Users\kiidh\9x9\Packages"
   # Verify manifest.json is intact
   # Reopen Unity - it will download packages
   ```
4. Check internet connection
5. Clear Package Cache:
   ```powershell
   Remove-Item -Recurse -Force "$env:LOCALAPPDATA\Unity\cache"
   ```

---

### Problem: VS Code Not Showing IntelliSense
**Symptoms**: No autocomplete, red squiggles everywhere

**Solutions**:
1. In Unity: **Edit > Preferences > External Tools**
   - Set External Script Editor to "Visual Studio Code"
   - Check "Generate .csproj files for" all options
2. In Unity: **Assets > Open C# Project**
3. In VS Code: Install required extensions
   - C# Dev Kit (ms-dotnettools.csdevkit)
   - Unity (visualstudiotoolsforunity.vstuc)
4. Restart VS Code
5. Check if .csproj and .sln files exist in project root
6. In VS Code, press `Ctrl+Shift+P` > "OmniSharp: Restart OmniSharp"

---

### Problem: Compilation Errors After Git Pull
**Symptoms**: Code that worked before now has errors

**Solutions**:
1. Close Unity Editor
2. Delete Library folder
3. Reopen Unity and let it reimport everything
4. Check if you need to merge package changes in `Packages/manifest.json`
5. Verify Unity version matches project (6000.2.10f1)

---

## Build Issues

### Problem: Build Fails with Compilation Errors
**Symptoms**: Build process stops with script errors

**Solutions**:
1. Fix all errors in Console window first
2. Check for platform-specific code issues:
   ```csharp
   #if UNITY_STANDALONE_LINUX
   // Server-specific code
   #endif
   ```
3. Verify all scenes are added to Build Settings
4. Check Build Settings > Player Settings for configuration errors
5. Try **File > Build Settings > Build** instead of Build and Run

---

### Problem: Dedicated Server Build Fails
**Symptoms**: Server build won't complete

**Solutions**:
1. Verify Dedicated Server package is installed:
   - **Window > Package Manager**
   - Search for "Dedicated Server" (should be v1.6.1)
2. In Build Settings, check "Server Build" option is enabled
3. Ensure target platform is Linux (for production) or Windows (for testing)
4. Check for client-only code that needs preprocessor directives:
   ```csharp
   #if !UNITY_SERVER
   // Client-only rendering code
   #endif
   ```
5. Review build log at `Builds/Logs/server-build.log`

---

### Problem: Build Size Too Large
**Symptoms**: Build exceeds expected size

**Solutions**:
1. Enable **Managed Stripping Level**: High
   - **Edit > Project Settings > Player > Other Settings**
2. For server builds, enable **Strip Engine Code**
3. Remove unused assets via **Assets > Unused Assets** (use carefully)
4. Check texture compression settings
5. Use Asset Bundles for large content
6. Disable Development Build for release

---

## Network Issues

### Problem: NetworkManager Not Starting
**Symptoms**: Multiplayer features don't work

**Solutions**:
1. Verify NetworkManager is in scene
2. Check NetworkManager configuration:
   - Network Prefabs list populated
   - Network Transport set up correctly
3. Verify scene is in Build Settings
4. Check console for Netcode errors
5. Ensure you're calling `NetworkManager.Singleton.StartHost()` or `StartClient()`

---

### Problem: Players Can't Connect
**Symptoms**: Connection timeout or failure

**Solutions**:
1. **For Local Testing**:
   - Verify same network subnet
   - Check Windows Firewall rules
   - Use IP address instead of hostname
   - Verify port is open (default 7777)

2. **For Relay/Matchmaking**:
   - Verify Unity Gaming Services configured
   - Check authentication is successful
   - Review Matchmaker ticket status
   - Check UGS dashboard for service status

3. **Firewall Configuration**:
   ```powershell
   # Allow Unity through Windows Firewall
   New-NetFirewallRule -DisplayName "Unity 9x9" -Direction Inbound -Program "C:\Users\kiidh\9x9\Builds\Client\Windows\9x9.exe" -Action Allow
   ```

---

### Problem: High Network Latency
**Symptoms**: Laggy gameplay, delayed actions

**Solutions**:
1. Reduce NetworkVariable update frequency
2. Use NetworkVariable write permissions (ServerOnly, OwnerOnly)
3. Optimize RPC calls - batch when possible
4. Enable lag compensation features
5. Test on different networks
6. Profile with **Window > Multiplayer > Multiplayer Tools**

---

### Problem: Synchronization Issues
**Symptoms**: Players see different game states

**Solutions**:
1. Verify server authority is enforced:
   ```csharp
   [ServerRpc]
   void DoActionServerRpc()
   {
       if (!IsServer) return;
       // Server-authoritative logic
   }
   ```
2. Use NetworkVariable for state that must sync
3. Check network object spawn/despawn order
4. Verify ownership assignments are correct
5. Test with Multiplayer Play Mode in editor

---

## Unity Gaming Services Issues

### Problem: Authentication Fails
**Symptoms**: "Authentication failed" error in console

**Solutions**:
1. Check internet connection
2. Verify UGS project is linked:
   - **Edit > Project Settings > Services**
   - Link to correct organization and project
3. Check Project ID in ProjectSettings.asset
4. Verify UGS credentials are configured
5. Check UGS dashboard for service outages
6. Review authentication code:
   ```csharp
   await UnityServices.InitializeAsync();
   await AuthenticationService.Instance.SignInAnonymouslyAsync();
   ```

---

### Problem: Matchmaking Not Working
**Symptoms**: Ticket creation fails or no matches found

**Solutions**:
1. Verify Multiplayer Services package installed (1.1.3)
2. Check matchmaking queue configuration in UGS dashboard
3. Ensure minimum player count is reasonable
4. Verify matchmaking code:
   ```csharp
   await MatchmakerTicketer.CreateTicket();
   ```
5. Check ticket status in console logs
6. Review UGS Matchmaker dashboard for errors
7. Test with Development mode enabled

---

### Problem: Relay Connection Failed
**Symptoms**: Can't connect through Unity Relay

**Solutions**:
1. Verify Relay service is enabled in UGS dashboard
2. Check concurrent connection limits for your plan
3. Ensure relay allocation is created before joining
4. Review relay connection code
5. Test without relay first (direct connection)
6. Check Unity Services status page

---

## Performance Issues

### Problem: Low Frame Rate in Editor
**Symptoms**: Editor runs slowly

**Solutions**:
1. Disable unnecessary editor features:
   - Reduce Scene view quality
   - Disable Game view during editing
2. Optimize assets:
   - Reduce texture sizes
   - Use simpler materials during development
3. Close Multiplayer Play Mode when not testing
4. Profile with **Window > Analysis > Profiler**
5. Restart Unity Editor periodically

---

### Problem: Low Frame Rate in Build
**Symptoms**: Game runs slowly after building

**Solutions**:
1. Profile with Unity Profiler
2. Check for excessive draw calls
3. Enable GPU Instancing on materials
4. Use Static Batching for static objects
5. Optimize network traffic
6. Review URP quality settings
7. Check for memory leaks
8. Optimize physics calculations

---

### Problem: High Memory Usage
**Symptoms**: Game uses too much RAM

**Solutions**:
1. Use Object Pooling for frequently spawned objects
2. Unload unused assets:
   ```csharp
   Resources.UnloadUnusedAssets();
   ```
3. Use Asset Bundles for large content
4. Optimize texture sizes and formats
5. Profile with Memory Profiler package
6. Check for event subscription leaks

---

## Common Errors

### Error: "NetworkManager is null"
**Cause**: Accessing NetworkManager.Singleton before it's initialized

**Solution**:
```csharp
if (NetworkManager.Singleton != null)
{
    // Safe to use NetworkManager
}
```

---

### Error: "No NetworkManager found"
**Cause**: Scene doesn't have a NetworkManager object

**Solution**:
1. Add NetworkManager to scene
2. Or load a scene that has NetworkManager first
3. Ensure NetworkManager persists across scenes (DontDestroyOnLoad)

---

### Error: "Prefab is not in NetworkPrefabs list"
**Cause**: Trying to spawn a prefab not registered with NetworkManager

**Solution**:
1. Add prefab to DefaultNetworkPrefabs.asset
2. Or add to NetworkManager's Network Prefabs list
3. Ensure prefab has NetworkObject component

---

### Error: "RPC called before NetworkObject spawned"
**Cause**: Trying to call RPC on unspawned object

**Solution**:
```csharp
if (IsSpawned)
{
    MyRpc();
}
```

---

### Error: "Only server can spawn NetworkObjects"
**Cause**: Client trying to spawn network objects

**Solution**:
```csharp
[ServerRpc]
void SpawnObjectServerRpc()
{
    if (!IsServer) return;
    
    var instance = Instantiate(prefab);
    instance.GetComponent<NetworkObject>().Spawn();
}
```

---

### Error: "Transport already initialized"
**Cause**: Trying to start NetworkManager twice

**Solution**:
1. Check if NetworkManager is already running:
   ```csharp
   if (!NetworkManager.Singleton.IsListening)
   {
       NetworkManager.Singleton.StartHost();
   }
   ```
2. Avoid multiple NetworkManager instances

---

### Error: "UIDocument not found"
**Cause**: UI Toolkit document missing or not assigned

**Solution**:
1. Check UIDocument component has UXML asset assigned
2. Ensure UI assets are in Resources folder or properly referenced
3. Verify UI assets built correctly

---

### Error: "Assembly reference missing"
**Cause**: Missing assembly definition references

**Solution**:
1. Check .asmdef files have correct references
2. Add required assemblies:
   - Unity.Netcode.Runtime
   - Unity.Services.Core
   - Unity.Multiplayer.Tools

---

## Debug Tools & Commands

### Enable Netcode Logging
```csharp
NetworkManager.Singleton.LogLevel = LogLevel.Developer;
```

### View Network Stats
Open **Window > Multiplayer > Multiplayer Tools > Runtime Net Stats Monitor**

### Test Locally
```powershell
# Start server
.\Builds\Server\Windows\9x9Server.exe -batchmode -nographics -logFile server.log

# Start client
.\Builds\Client\Windows\9x9.exe
```

### Clear PlayerPrefs
```csharp
PlayerPrefs.DeleteAll();
```

### Force GC Collection
```csharp
System.GC.Collect();
Resources.UnloadUnusedAssets();
```

---

## Getting Help

### Unity Resources
- [Unity Forum - Netcode](https://forum.unity.com/forums/netcode-for-gameobjects.661/)
- [Unity Documentation](https://docs.unity3d.com/)
- [Multiplayer Networking Documentation](https://docs-multiplayer.unity3d.com/)

### Community Support
- Unity Discord
- r/Unity3D on Reddit
- Stack Overflow (tag: unity3d, netcode-for-gameobjects)

### Project-Specific
- Check project's GitHub Issues
- Review commit history for recent changes
- Contact development team

---

## Log File Locations

**Editor Log**:
- Windows: `%LOCALAPPDATA%\Unity\Editor\Editor.log`
- Linux: `~/.config/unity3d/Editor.log`

**Player Log**:
- Windows: `%USERPROFILE%\AppData\LocalLow\CompanyName\9x9\Player.log`
- Linux: `~/.config/unity3d/CompanyName/9x9/Player.log`

**Build Log**:
- Custom location: `Builds/Logs/`

---

**Document Version**: 1.0  
**Maintained By**: Development Team  
**Last Updated**: November 13, 2025
