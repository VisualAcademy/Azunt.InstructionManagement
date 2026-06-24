using Azunt.InstructionManagement;
using Azunt.Web.Services;

namespace Azunt.Web.Components.Pages.Instructions;

public static class InstructionTenantSeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IInstructionRepository>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var user = userService.GetUserNotCached();
        var connectionString = user.Tenant.ConnectionString;

        var existing = await repository.GetAllAsync(connectionString);
        if (existing.Count > 0)
        {
            return;
        }

        await repository.AddAsync(new Instruction
        {
            Active = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = user.UserName,
            Name = "Tenant Instruction 1",
            Content = $"This sample instruction is stored in the tenant database for {user.Tenant.Name}."
        }, connectionString);

        await repository.AddAsync(new Instruction
        {
            Active = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = user.UserName,
            Name = "Tenant Instruction 2",
            Content = "This data is loaded through IUserService.GetUserNotCached().Tenant.ConnectionString."
        }, connectionString);
    }
}
