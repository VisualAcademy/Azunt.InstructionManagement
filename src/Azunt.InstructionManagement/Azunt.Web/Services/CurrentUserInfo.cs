namespace Azunt.Web.Services;

/// <summary>
/// Azunt의 현재 사용자 정보를 Azunt.Web 테스트 프로젝트에서 흉내 내기 위한 경량 타입입니다.
/// </summary>
public sealed class CurrentUserInfo
{
    public string UserName { get; set; } = "TenantUser";

    public TenantInfo Tenant { get; set; } = new();
}
