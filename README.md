# SRTPluginManager
![TestDevBranch](https://img.shields.io/github/workflow/status/SpeedrunTooling/SRTPluginManager/Publish?label=latest%20build&style=for-the-badge)
![Release](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=current%20release&style=for-the-badge)
![Date](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?style=for-the-badge)
![Downloads](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?color=%23007EC6&style=for-the-badge)

# Join Me Live On Twitch
![Twitch](https://img.shields.io/twitch/status/videogameroulette?style=for-the-badge)

# How To Install
Under Constructions

# Sections
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
[Stats HUD](https://speedruntooling.github.io/StatsHUD)
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890495401164801/unknown.png)

### Inventory HUD
[Inventory HUD](https://speedruntooling.github.io/InventoryHUD)
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890549995536414/unknown.png)
