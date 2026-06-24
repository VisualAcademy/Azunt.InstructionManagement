namespace Azunt.Web.Services;

/// <summary>
/// Azunt 기반 테스트 프로젝트에서 테넌트 정보를 표현하기 위한 경량 타입입니다.
/// 실제 멀티테넌트 애플리케이션에 적용할 때는 기존 Tenant 모델과 UserService를 사용하면 됩니다.
/// </summary>
public sealed class TenantInfo
{
    public string Name { get; set; } = "In-Memory Tenant";

    /// <summary>
    /// 실제 멀티테넌트 환경에서는 SQL Server 테넌트 DB 연결 문자열을 넣습니다.
    /// Azunt.Web 테스트 프로젝트에서는 "InMemory:DatabaseName" 형식으로 테넌트별 In-Memory DB를 분리합니다.
    /// </summary>
    public string ConnectionString { get; set; } = "InMemory:AzuntInstructionTenantDb";
}
