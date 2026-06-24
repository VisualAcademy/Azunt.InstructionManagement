using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azunt.InstructionManagement;

/// <summary>
/// Instructions 테이블과 일대일로 매핑되는 모델 클래스입니다.
/// </summary>
[Table("Instructions")]
public class Instruction
{
    /// <summary>
    /// 안내 문구 고유 아이디, 자동 증가
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// 활성 상태 표시, 기본값 true (활성)
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// 레코드 생성 시간
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 레코드 생성자 이름
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 안내 문구 이름 또는 제목
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 안내 문구 상세 내용
    /// </summary>
    public string? Content { get; set; }
}
