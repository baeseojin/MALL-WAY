# 아르온 백화점 키오스크

Windows x64에서 실행하는 WPF · .NET Framework 4.8 프로젝트입니다.

## 실행

Visual Studio에서 `DepartmentKiosk.sln`을 열고 .NET 데스크톱 개발 워크로드와 .NET Framework 4.8 개발자 팩을 설치한 환경에서 F5를 누릅니다. 명령줄에서는 `dotnet build DepartmentKiosk.sln`으로 빌드할 수 있습니다.

Debug 빌드의 실행 파일은 `DepartmentKiosk/bin/Debug/net48/win-x64/DepartmentKiosk.exe`입니다. 다른 폴더에서 시연할 때는 설정과 라이브러리를 포함한 출력 폴더 전체를 복사합니다. 첫 실행에는 출력 폴더에 쓸 수 있어야 합니다.

## 수요일 구현

- WPF 화면은 MVVM Light의 ViewModel에 바인딩합니다. 앱 시작 시 SimpleIoc에 설정, DB 접근 클래스, ViewModel을 등록하고 CommonServiceLocator로 ViewModel을 가져옵니다.
- Newtonsoft.Json으로 실행 파일 옆의 `kiosk-settings.json`을 읽습니다. 백화점 이름, SQLite 파일 경로, 기본 표시 층, 출발 노드 ID를 설정합니다. 상대 DB 경로의 기준은 실행 파일 폴더입니다.
- 첫 실행 때 `Data/initialize.sql`로 SQLite 파일을 생성합니다. 이후 실행은 기존 데이터를 사용합니다. 지도와 매장 정보는 WPF에서 SQLite 파일에 직접 연결해 읽습니다.
- DB에는 1층 푸드코트, 2층 패션, 3층 영화관과 편의시설, 매장 입구, 복도 및 층간 연결을 저장합니다. 각 간선은 양방향 이동을 의미하고, 양수 비용을 가집니다.
- 기본 1층 지도에서 매장 및 편의시설 배치, 중앙 엘리베이터, 동쪽 에스컬레이터, 1층 키오스크의 현재 위치를 볼 수 있습니다.

목요일에는 층 전환, 검색, 매장 상세, 편의시설 바로가기를 추가합니다. 금요일에는 다익스트라 경로 계산과 층별 경로 표시를 추가합니다.
