using Microsoft.EntityFrameworkCore;

namespace Azunt.InstructionManagement;

/// <summary>
/// InstructionApp에서 사용하는 데이터베이스 컨텍스트 클래스입니다.
/// EF Core In-Memory 테스트와 SQL Server 운영 환경에서 모두 사용할 수 있습니다.
/// </summary>
public class InstructionAppDbContext : DbContext
{
    /// <summary>
    /// DbContextOptions을 인자로 받는 생성자입니다.
    /// 주로 Program.cs 또는 Startup.cs에서 서비스로 등록할 때 사용됩니다.
    /// </summary>
    public InstructionAppDbContext(DbContextOptions<InstructionAppDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    /// <summary>
    /// 데이터베이스 모델을 설정하는 메서드입니다.
    /// In-Memory 테스트와 SQL Server 운영 환경 모두에서 사용할 수 있도록
    /// 공급자 전용 SQL 설정은 최소화합니다.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instruction>(entity =>
        {
            entity.ToTable("Instructions");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            entity.Property(m => m.Active)
                .HasDefaultValue(true);

            entity.Property(m => m.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            entity.Property(m => m.CreatedBy)
                .HasMaxLength(255);

            entity.Property(m => m.Name);
            entity.Property(m => m.Content);
        });
    }

    /// <summary>
    /// InstructionApp 관련 테이블을 정의합니다.
    /// </summary>
    public DbSet<Instruction> Instructions { get; set; } = null!;
}
