
namespace Azunt.InstructionManagement;

public interface IInstructionRepository
{
    Task<Instruction> AddAsync(Instruction model, string? connectionString = null);
    Task<List<Instruction>> GetAllAsync(string? connectionString = null);
    Task<Instruction> GetByIdAsync(long id, string? connectionString = null);
    Task<bool> UpdateAsync(Instruction model, string? connectionString = null);
    Task<bool> DeleteAsync(long id, string? connectionString = null);
    Task<ArticleSet<Instruction, int>> GetArticlesAsync<TParentIdentifier>(int pageIndex, int pageSize, string searchField, string searchQuery, string sortOrder, TParentIdentifier parentIdentifier, string? connectionString = null);
    Task<ArticleSet<Instruction, long>> GetByAsync<TParentIdentifier>(FilterOptions<TParentIdentifier> options, string? connectionString = null);
}
