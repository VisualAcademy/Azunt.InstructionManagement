using Microsoft.Extensions.Configuration;

namespace Azunt.Web.Services;

/// <summary>
/// Azunt.Web 테스트 프로젝트에서 멀티테넌트 연결 문자열을 흉내 내는 In-Memory UserService입니다.
/// appsettings.json의 InstructionTenant 섹션을 읽고, 값이 없으면 InMemory:AzuntInstructionTenantDb를 사용합니다.
/// </summary>
public sealed class InMemoryUserService : IUserService
{
    private readonly IConfiguration _configuration;

    public InMemoryUserService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CurrentUserInfo GetUserNotCached()
    {
        return new CurrentUserInfo
        {
            UserName = _configuration["InstructionTenant:UserName"] ?? "TenantUser",
            Tenant = new TenantInfo
            {
                Name = _configuration["InstructionTenant:TenantName"] ?? "In-Memory Tenant",
                ConnectionString = _configuration["InstructionTenant:ConnectionString"] ?? "InMemory:AzuntInstructionTenantDb"
            }
        };
    }
}
