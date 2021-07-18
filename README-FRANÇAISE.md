# SRTPluginManager
![Dernier Build](https://img.shields.io/github/workflow/status/SpeedrunTooling/SRTPluginManager/Publish?label=dernier%20build&style=for-the-badge)
![Dernière Version](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=derniere%20version&style=for-the-badge)
![Date](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?label=date%20de%20sortie&style=for-the-badge)
![Téléchargements](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?label=telechargements&color=%23007EC6&style=for-the-badge)

**Traduit par:** [GVirus](https://www.twitch.tv/itgvirus)

# Joignez-vous à moi en direct sur Twitch!
![Twitch](https://img.shields.io/twitch/status/videogameroulette?style=for-the-badge)

# Table de contenu
- [Installation](#installation)
- [Utilisation](#utilisation)
  - [Démarrer SRT](#démarrer-srt)
  - [Arrêter SRT](#arrêter-srt)
- [SRTHost](#srt-host)
- [Extensions](#extensions)
  - [Interface JSON](#interface-utilisateur-json)
  - [Interface Websocket](#interface-utilisateur-websocket)
- [Interfaces Utilisateurs](#interfaces-utilisateurs)
  - [Direct X](#direct-x)
  - [WPF](#wpf-windows-presentation-foundation)
- [Widgets](#widgets)
  - [ATH de Stats](#ath-de-stats)
  - [ATH de l'Inventaire](#ath-de-linventaire)

# Installation
- Téléchargez la dernière version [Dernière Version](https://github.com/SpeedrunTooling/SRTPluginManager/releases/latest)
- Extraire le contenu du fichier zip dans le dossier de votre choix (e.g. "C:/SRTHost")
- Exécutez SRTPluginManager.exe
- Allez sur la languette (Tab) "SRTHost"
- Dans le panneau "Host Version Info" cliquez sur "Install" pour installer la dernière version de l'hôte SRT ou sur "Update" pour mettre le logiciel à jour lorsque disponible. (Veuillez noter que c'est un gros téléchargement et un délai se fera sentir. Ne cliquez qu'une seule fois et attendez que la m-à-j soit terminée. Si vous cliquez plus d'une fois le logiciel va planter.)
- Sélectionnez le jeu pour lequel vous désirez installer à partir de la liste de jeux supportés
- Dans le panneau "Current Plugin" cliquez sur "Install" pour installer le dernier "PluginProvider" ou sur "Update" ^pour le mettre à jour lorsque disponible
- Allez sur la languette (Tab) "Extension"
- Installez "U JSON"
- Installez "U Websocket" (Optionnel. Requis seulement si vou utilisez 2 PCs pour votre diffusion en direct, envoyez un interface utilisateur vers un tournoi ou pour visualiser l'ATH SRT à partir de votre téléphone/tablette.)
- Allez sur la languette (Tab) "User Interfaces"
- Installez toutes les interfaces utilisateurs que vous désirez. Plus d'infos plus bas
- Allez sur la languette (Tab) "Widgets"
- Sélectionnez "options for Web UI" (À noter que certaines options sont spécifiques à certains jeux. Elles ne fonctionneront donc pas sur tous les jeux pour le moment.)
- SI WEBSOCKET EST INSTALLÉ: Assurez-vous de décocher "Enable Local Host Server"
- SI WEBSOCKET EST INSTALLÉ: Assurez-vous de générer un jeton utilisateur en cliquant sur le bouton "refresh" à côté de la boite du jeton utilisateur ou en en tapant un manuellement du founisseur de tournoi et ensuite en cliquant sur le bouton de sauvegarde
- Copiez les liens pour les stats ou l'inventaire et collez-les dans votre navigateur web ou bien OBS.

# Utilisation

## Démarrer SRT
- Allez sur la languette (Tab) "SRTHost"
- Sélectionnez le jeu à partir de la liste de jeux supportés
- Cliquez sur "Start SRT"

## Arrêter SRT
- Allez sur la languette (Tab) "SRTHost"
- Sélectionnez le jeu à partir de la liste de jeux supportés
- Cliquez sur "Stop SRT"

## SRT Host
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865877964464586772/unknown.png)

## Extensions
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878441153134622/unknown.png)

### Interface Utilisateur JSON
Initialise un serveur local JSON Server pour les communications entre les jeux et les serveurs web locaux pour les interfaces web.\
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

### Interface Utilisateur Websocket
Initiallise le serveur Websocket Server pour les communications entre les jeux et les serveurs web en ligne pour les interfaces web.

## Interfaces Utilisateurs
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878463856640010/unknown.png)

### Direct X 
Initiallise l'interface dans le jeu en utilisant SharpDX.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865883096922849320/unknown.png)

### WPF (Windows Presentation Foundation)
Initiallise l'applcation d'ATH externe en utilisant WPF.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865884741211652116/unknown.png)

## Widgets
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878482801262622/unknown.png)

### ATH de stats
[Stats HUD](https://speedruntooling.github.io/StatsHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890495401164801/unknown.png)

### ATH de l'Inventaire
[Inventory HUD](https://speedruntooling.github.io/InventoryHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890549995536414/unknown.png)
