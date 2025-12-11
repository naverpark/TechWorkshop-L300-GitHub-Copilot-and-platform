# infra/ (Issue #2) — 간단한 배포 안내

이 폴더는 Issue #2의 Azure 인프라 배포를 위한 Bicep 템플릿 및 모듈을 포함합니다.

파일 안내
- `main.bicep` - 루트 오케스트레이션 (ACR, Log Analytics, App Service 모듈 호출)
- `modules/acr.bicep` - Azure Container Registry 생성
- `modules/logAnalytics.bicep` - Log Analytics 워크스페이스 생성
- `modules/appService.bicep` - App Service Plan + Web App(컨테이너) 생성
- `main.parameters.json` - 배포 시 사용할 파라미터(이미지 자리표시자 포함)

간단 배포 예시

```bash
# 리소스 그룹이 이미 있다면 아래 명령으로 배포
az deployment group create \
  --resource-group <RG_NAME> \
  --template-file infra/main.bicep \
  --parameters @infra/main.parameters.json
```

이미지를 ACR에 푸시하고 App Service가 ACR의 이미지를 당겨오도록 구성하려면 추가 설정(Managed Identity로 ACR에 `AcrPull` 역할 부여)이 필요합니다. 현재 템플릿은 공개 또는 인증 설정된 이미지를 가정합니다.

다음 단계 제안
- ACR 관리자 사용자 활성화 또는 Managed Identity를 통한 `AcrPull` 역할 할당 추가
- AKS 모듈로 확장(컨테이너 오케스트레이션 필요 시)
- Application Gateway / Front Door 모듈 추가 (WAF 및 트래픽 관리)
