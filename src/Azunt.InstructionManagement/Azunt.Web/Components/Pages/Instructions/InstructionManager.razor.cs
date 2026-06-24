using Azunt.Components.Paging;
using Azunt.InstructionManagement;
using Azunt.Web.Components.Pages.Instructions.Components;
using Azunt.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;

namespace Azunt.Web.Pages.Instructions;

public partial class InstructionManager : ComponentBase
{
    public ModalForm EditorFormReference { get; set; } = null!;
    public List<Instruction> models = new();
    public Instruction model = new();
    public string EditorFormTitle { get; set; } = "CREATE";

    private string sortOrder = "";
    private string searchQuery = "";

    protected PagerBase pager = new()
    {
        PageIndex = 0,
        PageNumber = 1,
        PageSize = 10,
        PagerButtonCount = 5
    };

    [Inject] public IInstructionRepository RepositoryReference { get; set; } = null!;
    [Inject] public NavigationManager Nav { get; set; } = null!;
    [Inject] public IJSRuntime JSRuntimeInjector { get; set; } = null!;
    [Inject] public UserManager<ApplicationUser> UserManagerRef { get; set; } = null!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProviderRef { get; set; } = null!;

    [Parameter] public string UserId { get; set; } = "";
    [Parameter] public string UserName { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(UserName))
        {
            await GetUserIdAndUserName();
        }

        await DisplayData();
    }

    private async Task DisplayData()
    {
        var result = await RepositoryReference.GetArticlesAsync<int>(
            pager.PageIndex,
            pager.PageSize,
            searchField: "",
            searchQuery,
            sortOrder,
            parentIdentifier: 0);

        pager.RecordCount = result.TotalCount;
        models = result.Items.ToList();

        StateHasChanged();
    }

    protected void ShowEditorForm()
    {
        EditorFormTitle = "CREATE";
        model = new Instruction();
        EditorFormReference.Show();
    }

    protected void EditBy(Instruction m)
    {
        EditorFormTitle = "EDIT";
        model = m;
        EditorFormReference.Show();
    }

    protected void DeleteBy(Instruction m)
    {
        model = m;
        IsDeleteDialogShow = true;
    }

    protected async void CreateOrEdit()
    {
        EditorFormReference.Hide();
        await Task.Delay(50);
        model = new Instruction();
        await DisplayData();
    }

    public bool IsDeleteDialogShow { get; set; } = false;

    protected void DeleteClose()
    {
        IsDeleteDialogShow = false;
        model = new Instruction();
    }

    protected async Task DeleteClick()
    {
        if (model.Id != 0)
        {
            await RepositoryReference.DeleteAsync(model.Id);
        }

        IsDeleteDialogShow = false;
        model = new Instruction();
        await DisplayData();
    }

    protected async void Search(string query)
    {
        pager.PageIndex = 0;
        pager.PageNumber = 1;
        searchQuery = query;
        await DisplayData();
    }

    protected async void PageIndexChanged(int pageIndex)
    {
        pager.PageIndex = pageIndex;
        pager.PageNumber = pageIndex + 1;
        await DisplayData();
    }

    protected async void SortBy(string column)
    {
        if (!sortOrder.StartsWith(column, StringComparison.OrdinalIgnoreCase))
        {
            sortOrder = "";
        }

        sortOrder = sortOrder switch
        {
            "" => column,
            var value when value == column => $"{column}Desc",
            _ => ""
        };

        await DisplayData();
    }

    public bool IsInlineDialogShow { get; set; } = false;

    protected void ToggleBy(Instruction m)
    {
        model = m;
        IsInlineDialogShow = true;
    }

    protected void ToggleClose()
    {
        IsInlineDialogShow = false;
        model = new Instruction();
    }

    protected async void ToggleClick()
    {
        model.Active = !(model.Active ?? false);
        await RepositoryReference.UpdateAsync(model);
        IsInlineDialogShow = false;
        model = new Instruction();
        await DisplayData();
    }

    private void ExportExcel()
    {
        Nav.NavigateTo("/api/InstructionExport/Excel", forceLoad: true);
    }

    private static string ShowTimeOrDate(DateTimeOffset value)
    {
        if (value == default)
        {
            return "";
        }

        var local = value.ToLocalTime();
        return local.Date == DateTimeOffset.Now.Date
            ? local.ToString("HH:mm")
            : local.ToString("yyyy-MM-dd HH:mm");
    }

    private async Task GetUserIdAndUserName()
    {
        var authState = await AuthenticationStateProviderRef.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var currentUser = await UserManagerRef.GetUserAsync(user);
            UserId = currentUser?.Id ?? "";
            UserName = user.Identity?.Name ?? "Anonymous";
        }
        else
        {
            UserId = "";
            UserName = "Anonymous";
        }
    }
}
