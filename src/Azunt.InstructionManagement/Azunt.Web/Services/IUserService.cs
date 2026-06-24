namespace Azunt.Web.Services;

/// <summary>
/// Azunt.Web.Services.IUserService와 같은 사용 패턴을 갖는 테스트용 인터페이스입니다.
/// 실제 애플리케이션으로 컴포넌트를 옮길 때는 해당 프로젝트의 기존 IUserService를 사용하면 됩니다.
/// </summary>
public interface IUserService
{
    CurrentUserInfo GetUserNotCached();
}
