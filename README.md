# SRTPluginManager
![TestDevBranch](https://img.shields.io/github/workflow/status/SpeedrunTooling/SRTPluginManager/Publish?label=latest%20build&style=for-the-badge)
![Release](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=current%20release&style=for-the-badge)
![Date](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?style=for-the-badge)
![Downloads](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?color=%23007EC6&style=for-the-badge)

# Join Me Live On Twitch
![Twitch](https://img.shields.io/twitch/status/videogameroulette?style=for-the-badge)

# Table of Contents
- [How to Install](https://github.com/SpeedrunTooling/SRTPluginManager#how-to-install)
- [How to Use](https://github.com/SpeedrunTooling/SRTPluginManager#how-to-use)
  - [Start SRT](https://github.com/SpeedrunTooling/SRTPluginManager#start-srt)
  - [Stop SRT](https://github.com/SpeedrunTooling/SRTPluginManager#stop-srt)
- [SRTHost](https://github.com/SpeedrunTooling/SRTPluginManager/blob/main/README.md#srt-host)
- [Extensions](https://github.com/SpeedrunTooling/SRTPluginManager#extensions)
  - [UI JSON](https://github.com/SpeedrunTooling/SRTPluginManager#ui-json)
  - [UI Websocket](https://github.com/SpeedrunTooling/SRTPluginManager#ui-websocket)
- [User Interfaces](https://github.com/SpeedrunTooling/SRTPluginManager#user-interfaces)
  - [Direct X](https://github.com/SpeedrunTooling/SRTPluginManager#direct-x)
  - [WPF](https://github.com/SpeedrunTooling/SRTPluginManager#wpf-windows-presentation-foundation)
- [Widgets](https://github.com/SpeedrunTooling/SRTPluginManager#widgets)
  - [Stats HUD](https://github.com/SpeedrunTooling/SRTPluginManager#stats-hud)
  - [Inventory HUD](https://github.com/SpeedrunTooling/SRTPluginManager#inventory-hud)

# How to Install
[Install Guide Video on YouTube](https://www.youtube.com/watch?v=sU_ibNIQnQ8)

or

- Download latest release at [Latest Release](https://github.com/SpeedrunTooling/SRTPluginManager/releases/latest)
- Extract contents of zip to any desired folder ex. "C:/SRTHost"
- Run SRTPluginManager.exe
- Navigate to SRTHost tab
- In Host Version Info Panel click Install to install latest SRT Host or alternatively Update to update it when available (Be careful this is a big download so its delayed click only once and let it do its thing you'll know when its done and install button disappears if you click twice it will crash the program)
- Select Game you want to install from Supported Games List
- In Current Plugin Panel click Install to install latest PluginProvider or alternatively Update to update it when available
- Navigate to Extension tab
- Install UI JSON
- Install UI Websocket (Optional; only required if you have 2 pc stream setup, are outputting UI to tourney, or want to view the SRT HUD from a phone, tablet, or another device's browser)
- Navigate to User Interfaces tab
- Install any desired User Interfaces for each game supported more info below.
- Navigate to Widgets tab
- Select options for Web UI (Note some options are game specific not all work for all games at this time)
- IF WEBSOCKET INSTALLED. Make sure you uncheck "Enable Local Host Server"
- IF WEBSOCKET INSTALLED. Make sure to generate User Token by clicking the refresh button beside the User Token box or manually typing one in from tourney provider and clicking save button.
- Copy links for stats or inventory url and paste into web browser or OBS.

# How to Use

## Start SRT
- Navigate to SRTHost tab
- Choose Game from Supported Game List
- Click Start SRT

## Stop SRT
- Navigate to SRTHost tab
- Choose Game from Supported Game List
- Click Stop SRT

## SRT Host
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865877964464586772/unknown.png)

## Extensions
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878441153134622/unknown.png)

### UI JSON
Initiallizes local JSON Server for communications between game and local web servers for web HUD's.\
[Local Host API](http://localhost:7190)
<details>
  <summary>Example</summary>

```
{
    GameName: "Example API RE2R",
    VersionInfo: "9.9.9.9",
    Timer: {
        IGTRunningTimer: 6028998549,
        IGTCutsceneTimer: 189283718,
        IGTMenuTimer: 1272932590,
        IGTPausedTimer: 1967921896
    },
    PlayerCharacter: 1,
    Player: {
        CurrentHP: 1200,
        MaxHP: 1200,
        Percentage: 1,
        IsAlive: true,
        HealthState: 1
    },
    PlayerName: "Claire: ",
    IsPoisoned: false,
    RankManager: {
        Rank: 6,
        RankScore: 6690.906
    },
    PlayerInventoryCount: 12,
    PlayerInventory: [
        {
            _DebuggerDisplay: "[#2] Item WoodenBoard Quantity 5",
            SlotPosition: 2,
            ItemID: 33,
            WeaponID: -1,
            Attachments: 0,
            Quantity: 5,
            IsItem: true,
            IsWeapon: false,
            IsEmptySlot: false
        },
        {
            _DebuggerDisplay: "[#5] Empty Slot",
            SlotPosition: 5,
            ItemID: 0,
            WeaponID: -1,
            Attachments: 0,
            Quantity: -1,
            IsItem: false,
            IsWeapon: false,
            IsEmptySlot: true
        }
    ],
    EnemyHealth: [
        {
            _DebuggerDisplay: "1500 / 1500 (100.0%)",
            MaximumHP: 1500,
            CurrentHP: 1500,
            IsTrigger: false,
            IsAlive: true,
            IsDamaged: false,
            Percentage: 1
        },
        {
            _DebuggerDisplay: "44 / 890 (4.9%)",
            MaximumHP: 890,
            CurrentHP: 44,
            IsTrigger: false,
            IsAlive: true,
            IsDamaged: true,
            Percentage: 0.0494382
        }
    ],
    IGTCalculated: 3871792935,
    IGTCalculatedTicks: 38717929350,
    IGTTimeSpan: {
        Ticks: 38717929350,
        Days: 0,
        Hours: 1,
        Milliseconds: 792,
        Minutes: 4,
        Seconds: 31,
        TotalDays: 0.044812418229166665,
        TotalHours: 1.0754980375,
        TotalMilliseconds: 3871792.935,
        TotalMinutes: 64.52988225,
        TotalSeconds: 3871.792935
    },
    IGTFormattedString: "01:04:31"
}
```
</details>

### UI Websocket
Initiallizes Websocket Server for communications between game and online web servers for web HUD's.

## User Interfaces
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878463856640010/unknown.png)

### Direct X 
Initiallizes in-game overlay using SharpDX.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865883096922849320/unknown.png)

### WPF (Windows Presentation Foundation)
Initiallizes External HUD Application using WPF.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865884741211652116/unknown.png)

## Widgets
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878482801262622/unknown.png)

### Stats HUD
[Stats HUD](https://speedruntooling.github.io/StatsHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890495401164801/unknown.png)

### Inventory HUD
[Inventory HUD](https://speedruntooling.github.io/InventoryHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890549995536414/unknown.png)
