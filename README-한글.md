# SRT플러그인매니저
![테스트 개발 분기](https://img.shields.io/github/workflow/status/SpeedrunTooling/SRTPluginManager/Publish?label=테스트%20개발%20분기&style=for-the-badge)
![릴리스](https://img.shields.io/github/v/release/SpeedrunTooling/SRTPluginManager?label=릴리스&style=for-the-badge)
![날짜](https://img.shields.io/github/release-date/SpeedrunTooling/SRTPluginManager?label=날짜&style=for-the-badge)
![다운로드](https://img.shields.io/github/downloads/SpeedrunTooling/SRTPluginManager/total?label=다운로드&color=%23007EC6&style=for-the-badge)

## 번역자: 다주경

# 트위치 생방송 접속
![트위치](https://img.shields.io/twitch/status/videogameroulette?label=트위치&style=for-the-badge)

# 목차
- [설치하는 방법](#설치하는-방법)
- [사용하는 방법](#사용하는-방법)
  - [SRT 시작](#srt-시작)
  - [SRT 중지](#srt-중지)
- [SRT 호스트](#srt-호스트)
- [확장](#확장)
  - [UI JSON](#ui-json)
  - [UI 웹 소켓](#ui-웹-소켓)
- [사용자 인터페이스](#사용자-인터페이스)
  - [다이렉트 X](#다이렉트-x)
  - [WPF](#wpf-(윈도우%20프레젠테이션%20파운데이션))
- [위젯](#위젯)
  - [통계 HUD](#통계-hud)
  - [물품 목록 HUD](#물품-목록-hud)

# 설치하는 방법
- 최신 릴리스에서 다운로드 합니다 [최신 릴리스](https://github.com/SpeedrunTooling/SRTPluginManager/releases/latest)
- 원하는 폴더에 Zip의 내용을 압축 해제 합니다 ex. "C:/SRTHost"
- SRTPluginManager.exe 을 실행합니다
- SRTHost 탭으로 이동을 합니다
- 호스트 버전 정보 패널에서 설치를 클릭하여 최신 SRT 호스트를 설치하거나 사용 가능한 경우 업데이트하여 업데이트합니다(이것은 큰 다운로드이므로 지연된 클릭은 한 번만 클릭하고 완료 및 설치 버튼이 완료되면 알 수 있도록 하십시오. 두 번 클릭하면 사라집니다 프로그램이 충돌합니다)
- 지원되는 게임 목록에서 설치하려는 게임을 선택합니다
- 현재 플러그인 패널에서 설치를 클릭하여 최신 PluginProvider를 설치하거나 업데이트를 클릭하여 사용 가능한 경우 업데이트합니다.
- 확장 탭으로 이동
- U JSON 설치
- U Websocket 설치(선택 사항은 2개의 PC Steam 설정이 있고 ui를 토너먼트로 출력하거나 전화/태블릿에서 srt HUD를 보고 싶은 경우에만 필요)
- 사용자 인터페이스 탭으로 이동
- 아래에서 지원되는 각 게임에 대해 원하는 사용자 인터페이스를 설치하십시오
- 위젯 탭으로 이동
- 웹 UI에 대한 옵션 선택(일부 옵션은 현재 모든 게임에서 작동하지 않는 게임별 옵션입니다)
- 웹소켓이 설치된 경우. "로컬 호스트 서버 사용"을 선택 취소 했는지 확인하십시오
- 웹소켓이 설치된 경우. 사용자 토큰 상자 옆에 있는 새로 고침 버튼을 클릭하거나 토너먼트 공급자로부터 수동으로 하나를 입력하고 저장 버튼을 클릭하여 사용자 토큰을 생성해야 합니다.
- 통계 또는 물품 목록 URL에 대한 링크를 복사하여 웹 브라우저 또는 OBS에 붙여넣습니다.

# 사용하는 방법

## SRT 시작
- SRT Host 탭으로 이동
- 지원되는 게임 목록에서 게임 선택
- SRT 시작 클릭

## SRT 중지
- SRT Host 탭으로 이동
- 지원되는 게임 목록에서 게임 선택
- SRT 중지 클릭

## SRT 호스트
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865877964464586772/unknown.png)

## 확장
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878441153134622/unknown.png)

### UI JSON
게임과 웹 HUD용 로컬 웹 서버 간의 통신을 위해 로컬 JSON 서버를 초기화합니다.\
[로컬 호스트 API](http://localhost:7190)
<details>
  <summary>예시</summary>

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

### UI 웹 소켓
게임과 웹 HUD용 온라인 웹 서버 간의 통신을 위해 Websocket Server를 초기화합니다.

## 사용자 인터페이스
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878463856640010/unknown.png)

### 다이렉트 X 
SharpDX를 사용하여 게임 내 오버레이를 초기화합니다.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865883096922849320/unknown.png)

### WPF (윈도우 프레젠테이션 파운데이션)
WPF를 사용하여 외부 HUD 응용 프로그램을 초기화합니다.\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865884741211652116/unknown.png)

## 위젯
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865878482801262622/unknown.png)

### 통계 HUD
[통계 HUD](https://speedruntooling.github.io/StatsHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890495401164801/unknown.png)

### 물품 목록 HUD
[물품 목록 HUD](https://speedruntooling.github.io/InventoryHUD)\
![screenshot](https://cdn.discordapp.com/attachments/551840398016774193/865890549995536414/unknown.png)
