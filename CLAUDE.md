# Project Instructions

## Stardew Valley Game Path
The game is installed at a custom location. Use this for builds:
```
GamePath=/Volumes/SSD_Work/SteamLibrary/steamapps/common/Stardew Valley/Contents/MacOS
```

## Building
```bash
dotnet build <ProjectName>/<ProjectName>.csproj -p:GamePath="/Volumes/SSD_Work/SteamLibrary/steamapps/common/Stardew Valley/Contents/MacOS"
```

### ElementalForce
The PostBuild target in the .csproj automatically deploys:
- The C# mod DLL + manifest + i18n + assets to `Mods/ElementalForce/`
- The Content Patcher pack `[CP] Elemental Force` to `Mods/[CP] Elemental Force/` (root Mods folder, not nested)

## Language
The user prefers to communicate in Portuguese (pt-BR).
