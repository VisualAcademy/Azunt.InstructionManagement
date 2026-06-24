using Azunt.InstructionManagement;

namespace Azunt.Web.Components.Pages.Instructions;

public static class InstructionSeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IInstructionRepository>();

        var existing = await repository.GetAllAsync();
        if (existing.Count > 0)
        {
            return;
        }

        await repository.AddAsync(new Instruction
        {
            Active = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "System",
            Name = "Initial Instruction 1",
            Content = "This is the first sample instruction for EF Core In-Memory testing."
        });

        await repository.AddAsync(new Instruction
        {
            Active = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "System",
            Name = "Initial Instruction 2",
            Content = "This is the second sample instruction for quick Blazor Server CRUD testing."
        });
    }
}
