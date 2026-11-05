<div align="center">

# 🏬 MALL-WAY

**원하는 매장 검색부터 층간 이동까지 안내하는 백화점 길찾기 키오스크**

3개 층의 매장과 편의시설을 탐색하고, 현재 위치에서 목적지 입구까지 경로를 보여주는 Windows WPF 프로젝트

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=csharp&logoColor=white)
![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/UI-WPF-0078D4?style=flat-square)
![MVVM Light](https://img.shields.io/badge/MVVM-Light-235274?style=flat-square)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white)
![Newtonsoft.Json](https://img.shields.io/badge/JSON-Newtonsoft.Json-4E7A3D?style=flat-square)

[주요 기능](#-주요-기능) · [설계 구조](#-설계-구조) · [실행 방법](#-실행-방법) · [검증](#-검증)

</div>

---

## 📌 프로젝트 소개

MALL-WAY는 C#과 WPF로 구현한 백화점 길안내 키오스크입니다. 매장명을 검색하거나 카테고리와 층별 지도에서 목적지를 선택하면 위치와 영업시간을 확인할 수 있습니다. 길찾기를 실행하면 1층 키오스크에서 목적지 입구까지 다익스트라로 경로를 계산하고, 층별 지도와 이동 안내를 제공합니다.

MVVM Light와 CommonServiceLocator로 화면 상태와 객체 생성을 연결하며, WPF 앱이 로컬 SQLite 파일에 직접 접근합니다. 키오스크의 시작 위치와 기본 표시 층은 JSON 설정으로 관리합니다.

가상 백화점인 **아르온 백화점**을 대상으로 하며, 매장명과 지도 배치는 시연용으로 구성했습니다.

## ✨ 주요 기능

| 영역 | 구현 내용 |
| --- | --- |
| 🔎 매장 검색 | 전체 층의 이름 부분 검색, 카테고리와 검색어 동시 적용, 검색 초기화 |
| 🗺️ 층별 지도 | 1F·2F·3F 전환, WPF 벡터 지도, 매장과 이동 시설 표시 |
| 📍 현재 위치 | 설정에 지정된 1층 키오스크를 빨간 표식으로 표시 |
| 🏪 매장 상세 | 이름, 카테고리, 층과 구역, 영업시간 표시 |
| 🎯 목적지 선택 | 검색 결과와 지도에서 선택, 해당 층으로 이동, 금색 강조 |
| 🚻 편의시설 | 화장실·수유실·고객센터·주차장 연결 통로 바로 찾기 |
| 🧭 최단 경로 | 다익스트라로 이동 비용이 가장 낮은 경로 계산 |
| 🛗 층간 안내 | 엘리베이터·에스컬레이터 탑승 위치와 하차층 안내 |
| 🔵 경로 표시 | 층별 파란 이동 경로, 도착 지점, 경로 지우기 |
| ⚠️ 상태 처리 | 검색 결과 없음, 연결된 경로 없음, 지도 조회 오류 안내 |

### 백화점 구성

| 층 | 구성 | 가상 매장과 편의시설 |
| --- | --- | --- |
| 1F | 푸드코트 | 한끼마루, 소담면가, 구름다방, 고객센터, 화장실, 주차장 연결 통로 |
| 2F | 패션 | 모노핏, 온결웨어, 리프스튜디오, 스텝로그, 수유실, 화장실 |
| 3F | 영화관 | 루미시네마, 별빛스낵, 화장실 |

각 층에는 중앙 엘리베이터와 동쪽 에스컬레이터가 있습니다. 초기 데이터는 매장·편의시설 15곳, 이동 노드 31개, 양방향 연결 32개로 구성합니다.

## 🔄 이용 흐름

```mermaid
flowchart LR
    A[이름 검색 · 카테고리 · 편의시설] --> B[목적지 선택]
    M[층별 지도에서 매장 선택] --> B
    B --> C[해당 층 지도 · 매장 상세]
    C --> D[길찾기 실행]
    D --> E[1층 출발 구간 표시]
    E --> F[층 버튼으로 다음 구간 확인]
    F --> G[목적지 입구 도착]
```

길찾기 중 층을 바꾸면 목적지를 유지하며 해당 층의 경로를 표시합니다. 다른 목적지를 선택하거나 검색 조건에서 기존 목적지가 제외되면 이전 경로를 해제합니다. 경로 지우기는 목적지를 유지한 채 선과 이동 안내를 지웁니다.

## 🧰 기술 구성

| 구분 | 기술 |
| --- | --- |
| 언어 / 실행 환경 | C#, .NET Framework 4.8, Windows x64 |
| UI | WPF, XAML, Canvas, Viewbox, 데이터 바인딩 |
| MVVM | MVVM Light 5.4.1.1, ViewModelBase, ObservableObject, RelayCommand |
| 객체 연결 | SimpleIoc, CommonServiceLocator 2.0.2 |
| 설정 | Newtonsoft.Json 13.0.4 |
| 데이터 저장 | SQLite, Microsoft.Data.Sqlite 8.0.30 |
| 경로 계산 | 다익스트라, 양방향 가중 그래프 |
| 버전 관리 | Git, main과 feature/development |

패키지 참조와 빌드 설정은 [프로젝트 파일](MALL-WAY/MALL-WAY.csproj)에서 확인할 수 있습니다.

## 🏗️ 설계 구조

```mermaid
flowchart TB
    APP[App · 시작과 의존성 등록] --> SETTINGS[KioskSettings · JSON 역직렬화]
    SETTINGS --> JSON[kiosk-settings.json]
    APP --> IOC[SimpleIoc · CommonServiceLocator]
    IOC --> VM[MainViewModel · 화면 상태와 명령]
    VIEW[MainWindow · XAML] <-->|DataContext · 바인딩| VM
    VM --> REPO[MapRepository · 지도와 매장 조회]
    VM --> ROUTE[RouteService · 다익스트라와 안내 생성]
    ROUTE --> REPO
    REPO --> DB[(로컬 SQLite)]
    SQL[initialize.sql · 테이블과 초기 데이터] -->|최초 초기화| DB
```

### MVVM과 화면 갱신

`App.xaml.cs`에서 설정과 저장소를 준비하고, SimpleIoc에 객체와 서비스를 등록합니다. CommonServiceLocator로 가져온 `MainViewModel`을 창의 `DataContext`에 지정합니다.

`MainViewModel`은 표시할 지도, 검색 조건, 선택한 매장, 경로를 관리합니다. 버튼은 `RelayCommand`에 연결하고, 상태를 변경하면 `RaisePropertyChanged`로 XAML에 알립니다. 매장 강조는 `MapPlace.IsSelected`와 XAML의 `DataTrigger`로 처리합니다.

### 지도와 경로 데이터

| 테이블 | 역할 |
| --- | --- |
| `Floors` | 층 번호와 층 이름 |
| `Nodes` | 복도·매장 입구·키오스크·층간 이동 지점의 좌표 |
| `Places` | 매장 정보, 지도에 그릴 영역, 입구 노드 ID |
| `Edges` | 두 노드의 양방향 연결, 이동 비용, 이동 방식 |

매장 사각형은 `Places`의 영역 좌표로 그립니다. 길찾기는 `Nodes`에 저장한 입구 좌표까지 계산합니다. 층간 간선은 이동 안내로 설명하고, 지도에는 현재 층 안에서 이동하는 선분을 표시합니다.

## 💡 구현 포인트

- **설정과 지도 데이터 연결** — JSON에서 출발 노드 ID를 읽고 SQLite에서 실제 좌표를 조회합니다. 기본 표시 층과 키오스크의 실제 층을 따로 관리합니다.
- **전체 층 검색** — 시작 시 읽은 매장 목록을 메모리에서 필터링합니다. 결과에 층과 구역을 표시해 이름이 같은 층별 화장실을 구분합니다.
- **층간 경로 계산** — 복도와 층간 연결에 지정한 비용을 함께 비교합니다. 연속된 엘리베이터 이동은 안내를 묶고, 에스컬레이터는 층별 탑승·하차를 안내합니다.
- **선택 상태 관리** — 경로 안내 중 층 전환에도 목적지를 유지하고, 목적지 변경 시 이전 경로를 해제합니다. 지도 조회가 실패하면 기존 화면을 유지합니다.
- **DB 초기화** — 트랜잭션에서 테이블과 초기 데이터를 만들고 SQLite의 `user_version`을 1로 기록합니다. 다음 실행부터는 기존 DB를 사용합니다.
- **동기 처리 선택** — 현재 데이터 규모에 맞춰 SQLite 조회, 검색 필터링, 경로 계산을 동기로 처리합니다. 이동 비용은 경로 비교용 가중치입니다.

## 🚀 실행 방법

### 준비 환경

- Windows x64와 .NET Framework 4.8 실행 환경
- .NET Framework 4.8 개발자 팩과 WPF 빌드를 지원하는 .NET SDK
- Visual Studio 사용 시 .NET 데스크톱 개발 워크로드
- 첫 NuGet 복원을 위한 네트워크 연결

### 1. 소스 가져오기

```powershell
git clone https://github.com/baeseojin/MALL-WAY.git
cd MALL-WAY
```

### 2. 빌드

Visual Studio에서 `MALL-WAY.sln`을 열고 F5로 실행하거나, 저장소 루트에서 다음 명령을 실행합니다.

```powershell
dotnet build .\MALL-WAY.sln
```

### 3. 앱 실행

```powershell
& .\MALL-WAY\bin\Debug\net48\win-x64\MALL-WAY.exe
```

Release 구성은 다음 명령으로 빌드하고 실행합니다.

```powershell
dotnet build .\MALL-WAY.sln -c Release
& .\MALL-WAY\bin\Release\net48\win-x64\MALL-WAY.exe
```

첫 실행에는 출력 폴더에 DB를 생성할 쓰기 권한이 필요합니다. 다른 PC나 폴더에서 실행할 때는 DLL, 설정, 초기 SQL을 포함한 **출력 폴더 전체**를 복사합니다.

### 4. 가상 매장으로 시연

1. 시작 화면에서 1층 지도와 빨간 현재 위치를 확인합니다.
2. `모노`를 검색하고 모노핏을 선택해 2층 지도, 금색 강조, 영업시간을 확인합니다.
3. 검색 초기화 후 루미시네마를 검색해 선택하고 길찾기를 누릅니다.
4. 1층에서 중앙 엘리베이터까지의 파란 경로를 확인하고, 3F를 눌러 영화관 입구까지의 경로를 봅니다.
5. 화장실 바로가기에서 3층 화장실을 선택하고 길찾기를 누릅니다. 각 층 지도를 바꾸며 에스컬레이터 구간을 확인합니다.
6. 경로 지우기와 다른 목적지 선택으로 이전 경로가 해제되는지 확인합니다.

### JSON 설정과 DB 초기화

실행 파일 옆의 `kiosk-settings.json`을 Newtonsoft.Json으로 읽습니다.

```json
{
  "mallName": "아르온 백화점",
  "databasePath": "Data/department.db",
  "startNodeId": "1-kiosk",
  "defaultFloor": 1
}
```

| 설정 | 의미 |
| --- | --- |
| `mallName` | 창과 화면에 표시할 백화점 이름 |
| `databasePath` | SQLite 파일 경로, 상대 경로의 기준은 실행 파일 폴더 |
| `startNodeId` | 길찾기의 출발점인 키오스크 노드 |
| `defaultFloor` | 앱을 열 때 처음 표시할 층 |

새 DB는 `Data/initialize.sql`로 초기화합니다. 이미 `user_version = 1`인 DB에는 초기 SQL을 다시 적용하지 않습니다. 초기 SQL을 수정한 경우 기존 DB의 데이터는 그대로 유지되며, 변경을 반영하려면 별도 데이터 갱신이 필요합니다.

## 📁 프로젝트 구조

```text
MALL-WAY/
├── MALL-WAY.sln
├── README.md
├── .gitignore
└── MALL-WAY/
    ├── MALL-WAY.csproj
    ├── App.xaml / App.xaml.cs       # 시작 · 설정 · 의존성 등록
    ├── MainWindow.xaml / .xaml.cs   # 화면 · 지도 레이어 · 바인딩
    ├── MainViewModel.cs            # 검색 · 층 전환 · 선택 · 길찾기 명령
    ├── KioskSettings.cs            # JSON 읽기와 설정 검사
    ├── MapRepository.cs            # SQLite 조회 · 지도와 매장 모델
    ├── RouteService.cs             # 그래프 모델 · 다익스트라 · 안내 생성
    ├── kiosk-settings.json         # 키오스크 시작 설정
    └── Data/
        └── initialize.sql          # 테이블 · 가상 매장 · 경로 데이터
```

## 🧪 검증

### 검증 기록 — 2026-11-05

| 항목 | 결과 |
| --- | --- |
| Debug / Release 빌드 | 경고 0 · 오류 0 |
| Release 시작 경로 | 창 열기와 SQLite 생성 확인 |
| 최단 비용 비교 | 31개 노드의 모든 출발·도착 조합 961개를 별도 계산과 비교해 일치 |
| 길찾기 통합 검증 | 경로 연속성, 비용 합계, 15개 목적지 흐름 등을 포함한 6,752개 확인 통과 |
| 기존 기능 회귀 검증 | 검색·카테고리·시설 선택·변경 알림 등 73개 확인 통과 |
| DB 호환 | 수요일에 생성한 DB와 새 DB에서 조회·길찾기 확인 |
| WPF 지도 렌더링 | 1층 출발, 3층 도착, 에스컬레이터 안내 화면 확인 |

검증 수에는 경로별 선분 확인과 반복 조합 검사가 포함됩니다. 검증 도구와 렌더링 산출물은 로컬에서 실행한 기록이며 저장소에는 포함하지 않습니다. 실제 단말에서 직접 확인할 때는 위 시연 순서를 사용할 수 있습니다.

## 📍 시연 범위와 다음 작업

현재 지도는 3개 층의 고정 배치로 구성되어 있습니다. 경로는 DB에 지정한 연결과 비용을 기준으로 계산하며, 실제 거리나 소요 시간으로 환산하지 않습니다.

11월 5일 목요일에 MVVM Light 버전의 검색·지도·상세·편의시설·길찾기를 완성했습니다. 11월 6일 금요일에는 완성본을 별도로 복사해 **CommunityToolkit.Mvvm으로 마이그레이션**할 예정입니다. ViewModel의 변경 알림, 명령, 객체 생성 방식을 옮기고 같은 JSON과 SQLite 데이터로 두 버전의 동작을 비교합니다.

## 📦 저장소 관리

개발은 `feature/development` 하나에서 계속 진행하고, 검증한 작업을 `main`에 `--no-ff`로 병합합니다. 작업 단계는 커밋으로 구분합니다.

솔루션, 소스, JSON 설정, 초기 SQL과 README를 관리합니다. `bin/`, `obj/`, `.dotnet/`, 실행 중 생성되는 `*.db`와 관련 파일, 사용자별 IDE 설정은 Git에서 제외합니다.

---

<div align="center">

**MALL-WAY · C# WPF 기반 백화점 검색 & 층별 경로 안내 프로젝트**

</div>
