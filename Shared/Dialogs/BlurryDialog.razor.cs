using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Leon.Shared.Dialogs
{
    public partial class BlurryDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    }
}
