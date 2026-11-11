# 화면 기능 회귀 검증

Windows와 .NET Framework 4.8 개발 환경에서 실행합니다. 외부 테스트 프레임워크 없이 WPF 바인딩, 상세 화면 전환, 두 언어 검색, 카테고리 유지, 경로 유지·번역, 테마 복원과 최소 창 크기의 조합을 검사합니다. 별도 임시 DB를 사용하며 운영 DB는 수정하지 않습니다.

```powershell
dotnet run --project .\MALL-WAY.Tests\MALL-WAY.Tests.csproj
```

화면의 실제 가독성과 마우스·터치 스크롤은 앱에서 추가로 확인합니다.
