namespace Azunt.InstructionManagement;

/// <summary>
/// 검색, 정렬, 페이징 조건을 Repository에 전달하기 위한 경량 옵션 형식입니다.
/// Dul.dll 또는 외부 FilterOptions 타입에 의존하지 않도록 InstructionManagement 패키지 내부에 포함합니다.
/// </summary>
/// <typeparam name="TParentIdentifier">부모 식별자 형식</typeparam>
public sealed class FilterOptions<TParentIdentifier>
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string SearchField { get; set; } = string.Empty;
    public string SearchQuery { get; set; } = string.Empty;
    public string SortOrder { get; set; } = string.Empty;
    public TParentIdentifier? ParentIdentifier { get; set; }
}
