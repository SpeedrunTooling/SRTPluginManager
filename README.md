# SRTPluginManager

![Release](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=current%20release&style=for-the-badge) ![Date](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?style=for-the-badge) ![Downloads](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?color=%23007EC6&style=for-the-badge)

## Table of Contents

- [How to Install](#how-to-install)
- [How to Use](#how-to-use)
  - [Start SRT](#start-srt)
  - [Stop SRT](#stop-srt)
- [User Interfaces](#user-interfaces)
  - [JSON](#json)
  - [Direct X](#direct-x)
  - [WPF](#wpf-windows-presentation-foundation)
  - [WinForms](#winforms-windows-forms)
  - [Web-based HUD UIs](#web-based-hud-uis)

## How to Install

- Install prerequisites:
  - [.NET 10 Desktop Runtime](https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe).
- Download the latest release at <https://github.com/SpeedrunTooling/SRTPluginManager/releases>.
- Extract contents of zip to any desired folder. For example: `C:/SRTHost`
- Run `SRTPluginManager.exe`.
  - Navigate to the `SRT Host` tab.
    - In the `Host Version Info` panel, click `Install` or `Update` to install the latest version of the `SRT Host plugin system`. Bear in mind that this is a large download and there is no progress indicator after clicking this button so give it a few moments.
    - Select the game you want to install from the `Supported Games` list.
      - In the `Current Plugin` panel, click `Install` or `Update` to install the latest `PluginProvider` for that game.
  - Navigate to the `Extensions` tab.
    - Install the `UI JSON` plugin. This allows web-based UIs to display game information locally.
  - Navigate to the `User Interfaces` tab.
    - Install any desired `User Interfaces` plugins for each game supported.
  - Navigate to the `Widgets` tab.
    - Select options for the `Web UI`. Note that some options are game-specific and may not work for all games.
    - Copy the links for the `StatsHUD` or `InventoryHUD` URLs and paste into a web browser or an OBS browser source.

## How to Use

### Start SRT

- Navigate to SRTHost tab
- Choose Game from Supported Game List
- Click Start SRT

### Stop SRT

- Navigate to SRTHost tab
- Choose Game from Supported Game List
- Click Stop SRT

### User Interfaces

#### JSON

Initializes local JSON Server for communications between game and local clients such as the Stats and Inventory HUDs referenced later on.
[Local Host API](http://localhost:7190)

<details>
  <summary>Example</summary>

```json
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

#### Direct X

Initializes in-game overlay using SharpDX.

#### WPF (Windows Presentation Foundation)

Initializes External HUD Application using WPF.

#### WinForms (Windows Forms)

Initializes External HUD Application using WinForms.

#### Web-based HUD UIs

[Stats HUD](https://speedruntooling.github.io/StatsHUD)

[Inventory HUD](https://speedruntooling.github.io/InventoryHUD)
