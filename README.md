# MouseClickCounter

A cross-platform mouse click counter application built with Avalonia UI using MVVM pattern.

## Overview

This project is an Avalonia implementation based on the functionality of [mrhuo/MouseClickCounter](https://github.com/mrhuo/MouseClickCounter). It duplicates all non-UI functions identically while converting the UI from WinForms to Avalonia with MVVM pattern.

## Features

### Core Functionality (Identical to Original)
- **Real-time Click Statistics**: Uses enhanced mouse hooks to accurately record left and right button clicks
- **Local Data Storage**: Persists click data using encrypted local text files
- **Data Encryption**: Sensitive data is protected using AES encryption
- **Auto Sync**: Periodically syncs data to server for national rankings (if enabled)

### Ranking Features
- **Personal Ranking**: View your ranking among all national users
- **Provincial Ranking**: View click statistics for each province
- **Real-time Updates**: Periodically fetches latest ranking data from server
- **Data Visualization**: Clear table display with ranking indicators

### Configuration Options
- **Server Settings**: Customize API server address
- **Sync Frequency**: Set data sync interval (1/3/5/10 minutes)
- **Auto-start**: Option to run on system startup
- **Privacy Control**: Choose whether to participate in national rankings

## Technical Stack

- **.NET 10.0**
- **Avalonia 11.3.12** - Cross-platform UI framework
- **CommunityToolkit.MVVM 8.2.1** - MVVM implementation
- **Semi.Avalonia** - UI theme library
- **Irihi.Ursa** - Additional UI components

## Project Structure

```
MouseClickCounter/
├── Models/
│   └── ClickData.cs              # Click data model
├── Services/                      # Service layer (identical to original)
│   ├── DataStorageService.cs    # Data storage service
│   ├── EncryptionService.cs     # Encryption service
│   ├── LogService.cs            # Logging service
│   ├── DeviceInfoService.cs     # Device information service
│   ├── RankingApiService.cs     # Ranking API service
│   ├── ConfigManager.cs         # Configuration management
│   └── MouseHookService.cs      # Mouse hook service (cross-platform)
├── ViewModels/                   # MVVM ViewModels
│   ├── MainWindowViewModel.cs   # Main window ViewModel
│   ├── ConfigViewModel.cs       # Config window ViewModel
│   └── AllRankViewModel.cs      # Ranking window ViewModel
└── Views/                        # Avalonia Views
    ├── MainWindow.axaml         # Main window UI
    ├── ConfigWindow.axaml       # Config window UI
    └── AllRankWindow.axaml      # Ranking window UI
```

## Implementation Notes

### Non-UI Functions (Identical to Original)
All service classes and models are duplicated identically from the original WinForms application:
- `LogService` - Logging functionality
- `EncryptionService` - AES encryption/decryption
- `DeviceInfoService` - Device information retrieval
- `DataStorageService` - File-based data persistence
- `RankingApiService` - API communication
- `ConfigManager` - Configuration management (adapted for JSON instead of INI)

### UI Conversion to Avalonia MVVM
The UI has been completely rewritten using Avalonia and MVVM pattern:
- **MainWindow**: Displays real-time click counts, rankings, and provides navigation
- **ConfigWindow**: Settings management with binding to ConfigViewModel
- **AllRankWindow**: Provincial ranking display with data grid

### Cross-Platform Considerations
- **Mouse Hook**: Uses Windows-specific P/Invoke hooks when running on Windows. On other platforms, the hook functionality is not available (logged as info).
- **Configuration**: Uses JSON files instead of INI files for better cross-platform compatibility
- **Startup Registration**: Startup on boot functionality is Windows-specific and may not work on other platforms

## Building and Running

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio 2022 / VS Code / Rider

### Build
```bash
dotnet restore
dotnet build
```

### Run
```bash
dotnet run --project MouseClickCounter
```

## Differences from Original

1. **UI Framework**: WinForms → Avalonia
2. **Architecture**: Code-behind → MVVM pattern
3. **Configuration Storage**: INI files → JSON files
4. **Tray Icon**: Not yet implemented (optional feature)
5. **Cross-platform**: Now runs on Windows, Linux, and macOS (with limitations)

## Known Limitations

- Mouse hook functionality only works on Windows
- System tray icon support not yet implemented
- Startup on boot only works on Windows

## License

This project maintains the same license as the original [mrhuo/MouseClickCounter](https://github.com/mrhuo/MouseClickCounter).

## Acknowledgments

- Original project: [mrhuo/MouseClickCounter](https://github.com/mrhuo/MouseClickCounter)
- UI Framework: [Avalonia](https://avaloniaui.net/)
- MVVM Toolkit: [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
