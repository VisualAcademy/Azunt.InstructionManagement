using Microsoft.EntityFrameworkCore.Storage;

namespace Azunt.InstructionManagement;

/// <summary>
/// Azunt.Web에서 SQL Server 데이터베이스 프로젝트를 게시하지 않고도
/// Instruction 모듈을 바로 테스트하기 위한 EF Core In-Memory 데이터베이스 설정입니다.
/// </summary>
public static class InstructionInMemoryDatabase
{
    /// <summary>
    /// Instruction 모듈의 기본 In-Memory 데이터베이스 이름입니다.
    /// </summary>
    public const string DefaultName = "AzuntInstructionManagement";

    /// <summary>
    /// 여러 DbContext 인스턴스가 같은 In-Memory 저장소를 공유하도록 하는 Root입니다.
    /// </summary>
    public static readonly InMemoryDatabaseRoot Root = new();
}
