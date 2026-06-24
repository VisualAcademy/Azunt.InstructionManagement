using Azunt.InstructionManagement;
using Microsoft.AspNetCore.Components;

namespace Azunt.Web.Components.Pages.Instructions.Components;

public partial class ModalForm : ComponentBase
{
    public bool IsShow { get; set; } = false;

    public void Show() => IsShow = true;

    public void Hide()
    {
        IsShow = false;
        StateHasChanged();
    }

    [Parameter] public string UserName { get; set; } = "";
    [Parameter] public string? ConnectionString { get; set; }
    [Parameter] public RenderFragment EditorFormTitle { get; set; } = null!;
    [Parameter] public Instruction ModelSender { get; set; } = null!;
    [Parameter] public Action CreateCallback { get; set; } = null!;
    [Parameter] public EventCallback<bool> EditCallback { get; set; }

    [Inject] public IInstructionRepository RepositoryReference { get; set; } = null!;

    public Instruction ModelEdit { get; set; } = new();
    public bool ActiveValue { get; set; } = true;

    protected override void OnParametersSet()
    {
        ModelEdit = ModelSender != null
            ? new Instruction
            {
                Id = ModelSender.Id,
                Name = ModelSender.Name,
                Content = ModelSender.Content,
                Active = ModelSender.Active,
                CreatedBy = ModelSender.CreatedBy,
                CreatedAt = ModelSender.CreatedAt
            }
            : new Instruction { Active = true };

        ActiveValue = ModelEdit.Active ?? true;
    }

    protected async Task HandleValidSubmit()
    {
        ModelSender.Name = ModelEdit.Name;
        ModelSender.Content = ModelEdit.Content;
        ModelSender.Active = ActiveValue;
        ModelSender.CreatedBy = string.IsNullOrWhiteSpace(UserName) ? "Anonymous" : UserName;

        if (ModelSender.Id == 0)
        {
            ModelSender.CreatedAt = DateTimeOffset.UtcNow;
            await RepositoryReference.AddAsync(ModelSender, ConnectionString);
            CreateCallback?.Invoke();
        }
        else
        {
            await RepositoryReference.UpdateAsync(ModelSender, ConnectionString);
            await EditCallback.InvokeAsync(true);
        }

        Hide();
    }
}
