using System.Collections.Generic;

namespace Azunt.InstructionManagement;

/// <summary>
/// 페이징된 데이터 목록과 전체 레코드 수를 함께 전달하기 위한 경량 결과 형식입니다.
/// Dul.dll 또는 외부 ArticleSet 타입에 의존하지 않도록 InstructionManagement 패키지 내부에 포함합니다.
/// </summary>
/// <typeparam name="T">목록 항목 형식</typeparam>
/// <typeparam name="TCount">전체 개수 형식</typeparam>
public readonly struct ArticleSet<T, TCount>
{
    public ArticleSet(IEnumerable<T> items, TCount totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }

    /// <summary>
    /// 현재 페이지에 표시할 항목 목록입니다.
    /// </summary>
    public IEnumerable<T> Items { get; }

    /// <summary>
    /// 검색/필터링 조건을 반영한 전체 레코드 수입니다.
    /// </summary>
    public TCount TotalCount { get; }
}
