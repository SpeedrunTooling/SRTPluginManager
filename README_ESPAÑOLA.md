# SRTPluginManager
![LatestBuild](https://img.shields.io/github/workflow/status/SpeedrunTooling/SRTPluginManager/Publish?label=ultima%20compilación&style=for-the-badge)
![Release](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=lanzamiento%20actual&style=for-the-badge)
![ReleaseDate](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?label=fecha%20de%20lanzamiento&style=for-the-badge)
![Downloads](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?label=descargas&color=%23007EC6&style=for-the-badge)

**Traducido por: Ares**

# Únete a mis directos en Twitch
![Twitch](https://img.shields.io/twitch/status/videogameroulette?style=for-the-badge)

# Tabla de Contenidos
- [Cómo instalar](#c%C3%B3mo-instalar)
- [Cómo utilizar](#c%C3%B3mo-utilizar)
  - [Iniciar SRT](#iniciar-srt)
  - [Detener SRT](#detener-srt)
- [SRTHost](#srt-host)
- [Extensiones](#extensiones)
  - [UI JSON](#ui-json)
  - [UI Websocket](#ui-websocket)
- [Interfaz de usuario](#interfaz-de-usuario)
  - [Direct X](#direct-x)
  - [WPF](#wpf-windows-presentation-foundation)
- [Widgets](#widgets)
  - [HUD de Stats](#hud-de-stats)
  - [HUD del Inventario](#hud-del-inventario)

# Cómo instalar
- Descarga la última versión en [Último Lanzamiento](https://github.com/SpeedrunTooling/SRTPluginManager/releases/latest)
- Extraiga el contenido del archivo zip a cualquier carpeta deseada, ej. "C:/SRTHost"
- Ejecuta SRTPluginManager.exe
- Vaya a la pestaña SRTHost
- En el Panel de información de la versión del host, haga clic en Instalar para instalar el último SRT Host o, alternativamente, Actualizar para actualizarlo cuando esté disponible (tenga cuidado, esta es una gran descarga, por lo que normalmente llega a tardar, haga clic una sola vez y espere pacientemente hasta que desaparezca, cuando desaparezca significa que termino de descargar. Si hace clic dos veces, el programa se bloqueará)
- Seleccione el juego que desea instalar de la lista de juegos compatibles
- En el Panel "Current Plugin Panel" haga clic en Instalar para instalar el último PluginProvider o, alternativamente, Actualizar para actualizarlo cuando esté disponible
- Vaya a la pestaña Extensión
- Instala U JSON
- Instala U Websocket (Opcional solo se requiere si tiene una configuración de stream con 2pcs, enviar una IU a un torneo o si desea ver srt HUD desde el teléfono / tableta)
- Vaya a la pestaña "User Interfaces"
- Instale las "User Interfaces" que desee para cada juego compatible. Más información a continuación.
- Vaya a la pestaña Widgets
- Seleccionar opciones para Web UI (Tenga en cuenta que algunas opciones son específicas del juego, no todas funcionan para todos los juegos en este momento)
- SI ESTÁ INSTALADO WEBSOCKET. Asegúrate de desmarcar "Enable Local Host Server"
- SI ESTÁ INSTALADO WEBSOCKET. Asegurate de genearar un "User Token" haciendo clic en el botón de actualización junto al "User Token" o escribiendo manualmente uno del proveedor del torneo y haciendo clic en el botón Guardar.
- Copie enlaces para los Stats o URL de inventario y péguelos en el navegador web o en OBS.

# Cómo utilizar

## Iniciar SRT
- Vaya a la pestaña SRTHost
- Elija un juego de la lista de juegos admitidos
- Click en Start SRT

## Detener SRT
- Vaya a la pestaña SRTHost
- Elija un juego de la lista de juegos admitidos
- Click en Stop SRT

## SRT Host
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865877964464586772/unknown.png)

## Extensiones
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878441153134622/unknown.png)

### UI JSON
Inicia el servidor JSON local para las comunicaciones entre el juego y los servidores web locales para los HUD web.\
[Anfitriona Local API](http://localhost:7190)
<details>
  <summary>Ejemplo</summary>

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
Inicia Websocket Server para las comunicaciones entre el juego y los servidores web en línea para los HUD web.

## Interfaz de Usuario
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878463856640010/unknown.png)

### Direct X 
Inicia una superposición en el juego con SharpDX.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865883096922849320/unknown.png)

### WPF (Windows Presentation Foundation)
Inicia la aplicación de HUD externa mediante WPF.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865884741211652116/unknown.png)

## Widgets
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878482801262622/unknown.png)

### HUD de Stats
[HUD de Stats](https://speedruntooling.github.io/StatsHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890495401164801/unknown.png)

### HUD del Inventario
[HUD del Inventario](https://speedruntooling.github.io/InventoryHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890549995536414/unknown.png)
