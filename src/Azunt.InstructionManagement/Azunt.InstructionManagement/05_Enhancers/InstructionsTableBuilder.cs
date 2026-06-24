using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azunt.InstructionManagement;

/// <summary>
/// SQL Server 환경에서 Instructions 테이블을 생성하거나 누락된 컬럼을 보정하는 도우미 클래스입니다.
/// EF Core In-Memory 테스트 모드에서는 이 클래스를 실행하지 않아도 됩니다.
/// </summary>
public class InstructionsTableBuilder
{
    private readonly string _masterConnectionString;
    private readonly ILogger<InstructionsTableBuilder> _logger;

    public InstructionsTableBuilder(string masterConnectionString, ILogger<InstructionsTableBuilder> logger)
    {
        _masterConnectionString = masterConnectionString;
        _logger = logger;
    }

    public void BuildTenantDatabases()
    {
        var tenantConnectionStrings = GetTenantConnectionStrings();

        foreach (var connStr in tenantConnectionStrings)
        {
            try
            {
                EnsureInstructionsTable(connStr);
                _logger.LogInformation("Instructions table processed (tenant DB): {ConnectionString}", connStr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{ConnectionString}] Error processing tenant DB", connStr);
            }
        }
    }

    public void BuildMasterDatabase()
    {
        try
        {
            EnsureInstructionsTable(_masterConnectionString);
            _logger.LogInformation("Instructions table processed (master DB)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing master DB");
        }
    }

    private List<string> GetTenantConnectionStrings()
    {
        var result = new List<string>();

        using var connection = new SqlConnection(_masterConnectionString);
        connection.Open();

        using var cmd = new SqlCommand("SELECT ConnectionString FROM dbo.Tenants", connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var connectionString = reader["ConnectionString"]?.ToString();

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                result.Add(connectionString);
            }
        }

        return result;
    }

    private void EnsureInstructionsTable(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var cmdCheck = new SqlCommand(@"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Instructions'", connection);

        var tableCount = (int)cmdCheck.ExecuteScalar();

        if (tableCount == 0)
        {
            using var cmdCreate = new SqlCommand(@"
                CREATE TABLE [dbo].[Instructions] (
                    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Active] BIT DEFAULT ((1)) NULL,
                    [CreatedAt] DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),
                    [CreatedBy] NVARCHAR(255) NULL,
                    [Name] NVARCHAR(MAX) NULL,
                    [Content] NVARCHAR(MAX) NULL
                )", connection);

            cmdCreate.ExecuteNonQuery();
            _logger.LogInformation("Instructions table created.");
        }
        else
        {
            var expectedColumns = new Dictionary<string, string>
            {
                ["Active"] = "BIT",
                ["CreatedAt"] = "DATETIMEOFFSET",
                ["CreatedBy"] = "NVARCHAR(255)",
                ["Name"] = "NVARCHAR(MAX)",
                ["Content"] = "NVARCHAR(MAX)"
            };

            foreach (var kvp in expectedColumns)
            {
                EnsureColumn(connection, kvp.Key, kvp.Value);
            }
        }

        using var cmdCountRows = new SqlCommand("SELECT COUNT(*) FROM [dbo].[Instructions]", connection);
        var rowCount = (int)cmdCountRows.ExecuteScalar();

        if (rowCount == 0)
        {
            using var cmdInsertDefaults = new SqlCommand(@"
                INSERT INTO [dbo].[Instructions] (Active, CreatedAt, CreatedBy, Name, Content)
                VALUES
                    (1, SYSDATETIMEOFFSET(), 'System', 'Initial Instruction 1', 'Initial instruction content 1'),
                    (1, SYSDATETIMEOFFSET(), 'System', 'Initial Instruction 2', 'Initial instruction content 2')", connection);

            var inserted = cmdInsertDefaults.ExecuteNonQuery();
            _logger.LogInformation("Instructions 기본 데이터 {Count}건 삽입 완료", inserted);
        }
    }

    private void EnsureColumn(SqlConnection connection, string columnName, string columnType)
    {
        using var cmdColumnCheck = new SqlCommand(@"
            SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo'
              AND TABLE_NAME = 'Instructions'
              AND COLUMN_NAME = @ColumnName", connection);

        cmdColumnCheck.Parameters.AddWithValue("@ColumnName", columnName);
        var colExists = (int)cmdColumnCheck.ExecuteScalar();

        if (colExists > 0)
        {
            return;
        }

        using var alterCmd = new SqlCommand(
            $"ALTER TABLE [dbo].[Instructions] ADD [{columnName}] {columnType} NULL", connection);

        alterCmd.ExecuteNonQuery();
        _logger.LogInformation("Column added: {ColumnName} ({ColumnType})", columnName, columnType);
    }

    public static void Run(IServiceProvider services, bool forMaster)
    {
        try
        {
            var logger = services.GetRequiredService<ILogger<InstructionsTableBuilder>>();
            var config = services.GetRequiredService<IConfiguration>();
            var masterConnectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(masterConnectionString))
            {
                throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json.");
            }

            var builder = new InstructionsTableBuilder(masterConnectionString, logger);

            if (forMaster)
            {
                builder.BuildMasterDatabase();
            }
            else
            {
                builder.BuildTenantDatabases();
            }
        }
        catch (Exception ex)
        {
            var fallbackLogger = services.GetService<ILogger<InstructionsTableBuilder>>();
            fallbackLogger?.LogError(ex, "Error while processing Instructions table.");
        }
    }
}
