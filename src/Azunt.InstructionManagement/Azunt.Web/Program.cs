using Azunt.InstructionManagement;
using Azunt.Web.Components;
using Azunt.Web.Components.Account;
using Azunt.Web.Components.Pages.Instructions;
using Azunt.Web.Data;
using Azunt.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentUIComponents();

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Azunt.Web 테스트 프로젝트 자체도 SQL Server 없이 실행되도록 EF Core In-Memory를 사용합니다.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AzuntWebInMemoryDb"));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Azunt.Web 테스트 프로젝트에서 현재 사용자와 테넌트 연결 문자열을 제공하는 In-Memory UserService입니다.
builder.Services.AddScoped<IUserService, InMemoryUserService>();

// Instruction CRUD 테스트는 SQL Server 게시 없이 EF Core In-Memory 저장소로 바로 실행합니다.
builder.Services.AddDependencyInjectionContainerForInstructionApp(
    mode: InstructionServicesRegistrationExtensions.RepositoryMode.EfCoreInMemory);

var app = builder.Build();

await InstructionSeedData.InitializeAsync(app.Services);
await InstructionTenantSeedData.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();
app.MapControllers();

app.Run();
