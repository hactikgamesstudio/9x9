# 9x9 Unity Multiplayer Build Plan

## Project Information
**Project Name**: 9x9  
**Unity Version**: 6000.2.10f1  
**Build Date**: November 2025  
**Build Type**: Multiplayer Game (Client + Dedicated Server)

---

## Build Targets

### 1. Client Builds
- **Windows Standalone** (Primary)
- **Linux Standalone** (Optional)
- **WebGL** (Future consideration)

### 2. Dedicated Server Build
- **Linux Server** (Primary)
- **Windows Server** (Development/Testing)

---

## Pre-Build Checklist

### Environment Setup
- [ ] Unity Editor 6000.2.10f1 installed
- [ ] Visual Studio Code configured with C# Dev Kit
- [ ] Unity extension installed in VS Code
- [ ] Git repository initialized and up to date
- [ ] Unity Gaming Services (UGS) project configured

### Dependencies Verification
- [ ] All packages restored from `Packages/manifest.json`
- [ ] Netcode for GameObjects (2.3.2) functioning
- [ ] Unity Services Multiplayer (1.1.3) configured
- [ ] URP (17.2.0) rendering correctly
- [ ] Input System (1.14.2) bindings set

### Code Quality
- [ ] No compilation errors
- [ ] All scenes load without errors
- [ ] Unit tests passing (if implemented)
- [ ] Code reviewed and commented
- [ ] Namespace consistency verified

### Assets & Resources
- [ ] All assets properly imported
- [ ] Prefabs assigned in DefaultNetworkPrefabs.asset
- [ ] UI Toolkit assets compiled
- [ ] Scenes added to Build Settings
- [ ] Audio/visual assets optimized

---

## Build Configuration

### Client Build Settings
```
Platform: Windows/Linux/Mac
Architecture: x86_64
Scripting Backend: IL2CPP (recommended) or Mono
API Compatibility: .NET Standard 2.1
Compression Method: LZ4 (faster) or LZ4HC (smaller)
Development Build: Toggle based on purpose
```

### Server Build Settings
```
Platform: Linux/Windows Dedicated Server
Headless Mode: Enabled
Server Build: Enabled (via Dedicated Server package)
Scripting Backend: IL2CPP
Strip Engine Code: Enabled
Managed Stripping Level: High
```

### Build Symbols
Define custom scripting symbols as needed:
- `DEVELOPMENT_BUILD` - For development features
- `DEDICATED_SERVER` - For server-specific code
- `ENABLE_PROFILER` - For profiling

---

## Build Process

### Step 1: Prepare Unity Project
```powershell
# Open Unity project
# Verify scene setup
# File > Build Settings
# Verify scenes in build list
```

### Step 2: Configure Build Settings
1. Open **File > Build Settings**
2. Select target platform
3. Click **Player Settings**
4. Configure:
   - Product Name: "9x9"
   - Company Name: Update as needed
   - Version: Use semantic versioning (e.g., 0.1.0)
   - Bundle Identifier: com.yourcompany.9x9

### Step 3: Build Client
```powershell
# Via Unity Editor:
# File > Build Settings > Build
# Choose output directory: Builds/Client/Windows

# Via Command Line (Windows):
"C:\Program Files\Unity\Hub\Editor\6000.2.10f1\Editor\Unity.exe" `
  -quit -batchmode -nographics `
  -projectPath "C:\Users\kiidh\9x9" `
  -buildWindows64Player "Builds/Client/Windows/9x9.exe" `
  -logFile "Builds/Logs/client-build.log"
```

### Step 4: Build Dedicated Server
```powershell
# Via Unity Editor:
# File > Build Settings > Dedicated Server Build > Build
# Choose output directory: Builds/Server/Linux

# Via Command Line:
"C:\Program Files\Unity\Hub\Editor\6000.2.10f1\Editor\Unity.exe" `
  -quit -batchmode -nographics `
  -projectPath "C:\Users\kiidh\9x9" `
  -buildLinuxServer "Builds/Server/Linux/9x9Server.x86_64" `
  -logFile "Builds/Logs/server-build.log"
```

### Step 5: Post-Build Verification
- [ ] Build completed without errors
- [ ] Executable runs without crashes
- [ ] Network connection works
- [ ] UGS authentication successful
- [ ] Matchmaking functional
- [ ] Gameplay features working

---

## Build Optimization

### Performance Optimization
- Enable **Static Batching** and **Dynamic Batching**
- Use **GPU Instancing** for repeated objects
- Optimize **Texture Compression** settings
- Enable **Occlusion Culling** in scenes
- Profile with **Unity Profiler** before build

### Size Optimization
- Set **Managed Stripping Level** to High (server builds)
- Enable **Strip Engine Code** for dedicated servers
- Compress textures appropriately
- Use **Asset Bundles** for large content
- Remove unused assets and packages

### Network Optimization
- Minimize **NetworkVariable** updates
- Use **NetworkVariable** write permissions wisely
- Batch RPC calls when possible
- Optimize spawn/despawn patterns
- Use object pooling for networked objects

---

## Automated Build Pipeline (Future)

### Using Unity Cloud Build
1. Connect project to Unity Cloud Build
2. Configure build targets
3. Set up automated triggers (git commits)
4. Configure post-build notifications

### Using GitHub Actions (Alternative)
```yaml
# Example .github/workflows/build.yml
name: Unity Build
on: [push]
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - uses: game-ci/unity-builder@v2
        with:
          targetPlatform: StandaloneWindows64
```

---

## Distribution Plan

### Client Distribution
- **Steam** (Primary platform)
- **Epic Games Store** (Secondary)
- **Itch.io** (Testing/Early Access)
- **Direct Download** (Website)

### Server Deployment
- **Cloud Provider**: AWS/Azure/Google Cloud
- **Container**: Docker (optional)
- **Orchestration**: Kubernetes (for scaling)
- **Monitoring**: CloudWatch/Application Insights

### Version Control
- Use **semantic versioning**: MAJOR.MINOR.PATCH
- Tag releases in Git: `v0.1.0`, `v0.2.0`, etc.
- Maintain changelog: `CHANGELOG.md`
- Keep build notes for each version

---

## Testing Strategy

### Pre-Release Testing
- [ ] **Unit Tests**: All critical systems
- [ ] **Integration Tests**: Network functionality
- [ ] **Multiplayer Tests**: Using Multiplayer Play Mode
- [ ] **Performance Tests**: Frame rate, latency
- [ ] **Stress Tests**: Maximum player count
- [ ] **Cross-Platform**: Windows/Linux compatibility

### Beta Testing
- Select small group of testers
- Monitor crash reports
- Collect feedback via forms
- Iterate based on findings
- Prepare for wider release

---

## Release Checklist

### Final Pre-Release
- [ ] All critical bugs fixed
- [ ] Performance targets met
- [ ] Multiplayer stress tested
- [ ] UGS services configured for production
- [ ] Legal/credits/EULA reviewed
- [ ] Marketing materials prepared

### Launch Day
- [ ] Builds uploaded to distribution platforms
- [ ] Servers deployed and monitored
- [ ] Support channels ready
- [ ] Social media announcement
- [ ] Monitor for critical issues
- [ ] Prepare hotfix pipeline

---

## Post-Release Support

### Monitoring
- Server health and uptime
- Player count and retention
- Crash analytics
- Performance metrics
- Network latency

### Update Cycle
- **Hotfixes**: Critical bugs within 24-48 hours
- **Patches**: Bug fixes every 1-2 weeks
- **Content Updates**: Monthly new features
- **Major Versions**: Quarterly significant updates

---

## Build Command Reference

### Quick Build Commands

**Windows Client:**
```powershell
cd "C:\Users\kiidh\9x9"
# Build via Unity Editor GUI or command line
```

**Linux Server:**
```bash
# On Linux build machine
./build-server.sh --platform linux --config release
```

**Test Server Locally:**
```powershell
# Windows
.\Builds\Server\Windows\9x9Server.exe -batchmode -nographics

# Linux
./Builds/Server/Linux/9x9Server.x86_64 -batchmode -nographics
```

---

## Additional Resources

- [Unity Build Settings Documentation](https://docs.unity3d.com/Manual/BuildSettings.html)
- [Dedicated Server Package](https://docs.unity3d.com/Packages/com.unity.dedicated-server@latest)
- [Netcode Build Considerations](https://docs-multiplayer.unity3d.com/netcode/current/learn/bossroom/bossroom-optimization)
- [UGS Deployment Guide](https://docs.unity.com/ugs/manual/overview/manual/getting-started)

---

**Document Version**: 1.0  
**Last Updated**: November 13, 2025
