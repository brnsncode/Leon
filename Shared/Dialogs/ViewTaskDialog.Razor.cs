using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Leon.Services;
using Microsoft.JSInterop;
using Leon.Models;
using System.Net.NetworkInformation;
using static MudBlazor.CategoryTypes;

namespace Leon.Shared.Dialogs
{
    public partial class ViewTaskDialog
    {
        /*  
       *IAM boilerplate code
       *Code here should be minimal
       *If code requires complexity, please add code to the applicable services being used (IAMservice, ADservice, HelperService, etc.)
       *Each boilerplate should include:
       *1. OPTIAM Service(s) being used (IAMservice, ADservice, HelperService, etc.)
       *2. Any injections needed to the page
       *3  Any parameters needed to be set on the page
       *4. A List used to display data to a table (must be a List type, and a table model passed)
       *5. Calling your functions from the services
       *6. Any other small functions
       */

        //1. Services
        [Inject] LeonWorkerService _leonWorkerService { get; set; }
        [Inject] AssignedTaskService _assignedTaskService { get; set; }
        [Inject] ProjectService _projectService { get; set; }
        [Inject] ResourceService _resourceService { get; set; }
        [Inject] GraphApiService _graphApiService { get; set; }
        private List<AssignedTaskNote> _assignedTaskNotes { get; set; }
        private List<AssignedTaskNote> assignedTaskNotes { get; set; }
        [Inject] private IDialogService DialogService { get; set; }

        //2. Other Injections
        [Inject] IJSRuntime jsruntime { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        //3. Parameters
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        //Initialize searchString to "" to prevent nulls
        public string searchString { get; set; } = "";

        //Default starting Date will be 365 days ago
        public DateTime startDate { get; set; } = DateTime.Today.AddDays(-365);
        //Default end Date will be today's date
        //Date starts off end date with 23:59pm, but when filter is changed in front end, it changes to 
        public DateTime endDate { get; set; } = DateTime.Today.AddHours(23).AddMinutes(59);

        private DateTime effectiveDate { get; set; }
        private bool resetValueOnEmptyText;
        private bool isFirst = true;
        private string projectValue { get; set; } = "";
        private string resourceValue { get; set; } = "";
        private string newNote { get; set; } = "";
        [Parameter] public int filteredTaskId { get; set; }
        [Parameter] public int filteredProjectId { get; set; }
        [Parameter] public int filteredResourceId { get; set; }
        [Parameter] public bool linkedToPlanner { get; set; }
        [Parameter] public bool linkedToOneList { get; set; }
        [Parameter] public string filteredTaskName { get; set; }
        [Parameter] public DateTime? filteredReminderDate { get; set; }
        [Parameter] public string taskId { get; set; } = "";
        [Parameter] public string taskName { get; set; } = "";
        [Parameter] public string projectId { get; set; } = "";
        [Parameter] public string operationId { get; set; } = "";
        [Parameter] public string resourceId { get; set; } = "";
        [Parameter] public string capacityPercentage { get; set; }
        [Parameter] public string status { get; set; } = "";
        string state = "Message box hasn't been opened yet";

        void Cancel() => MudDialog.Cancel();



        //void Cancel() => MudDialog.Cancel();

        IEnumerable<Project> projectProperties = null;
        IEnumerable<Resource> resourceProperties = null;

        //4. Lists
        //Setting temp adusers list to load the list upon initialization
        private List<Project> _projects;
        private string _currentProjectName;
        public List<Project> projects => _projects.Where(p => p.ProjectName != null && p.ProjectName.ToLower().Contains(searchString.ToLower())).ToList();
        private List<Resource> _resources;
        private string _currentResourceName;
        public List<Resource> resources => _resources.Where(r => r.EmployeeName != null && r.EmployeeName.ToLower().Contains(searchString.ToLower())).ToList();
        //Then setting new adusers list to make previous list filterable/searchable (u.Name?.ToLower().Contains(searchString.ToLower()) ?? false))
        //public List<Project> projects => _projects.Where(p => (p.TechnologyLeads?.ToLower().Contains(searchString.ToLower()) ?? false)
        //    && (p.DateEntered >= startDate && p.DateEntered <= endDate)).ToList();


        protected override async Task OnInitializedAsync()
        {
            //Grab method from the service listed above
            //Function:

            _projects = await _projectService.GetProjects();
            _currentProjectName = await _projectService.GetProjectNameByProjectId(filteredProjectId);
            //_currentTaskName = await _operationService.GetTaskNameByTaskId(filteredOperationId);
            _resources = await _leonWorkerService.GetCurrentResources();
            _currentResourceName = await _resourceService.GetResourceNameByResourceId(filteredResourceId);
            _assignedTaskNotes = await _assignedTaskService.GetAllActiveTaskNotesByTaskIdDesc(filteredTaskId); ;

        }

        //Input event handler to handle real time email body updates
        public void OnInputHandler(ChangeEventArgs e)
        {
            //filteredTaskId = e.Value
            taskName = e.Value.ToString();
            projectId = e.Value.ToString();
            operationId = e.Value.ToString();
            resourceId = e.Value.ToString();
            _currentResourceName = e.Value.ToString();
            _currentProjectName = e.Value.ToString();
            capacityPercentage = e.Value.ToString();
            status = e.Value.ToString();
        }

        private async Task<IEnumerable<string>> SearchProjects(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return projects.Select(u => u.ProjectName).Where(u => u != null);
            projectProperties = projects.Where(u => u.ProjectName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return projectProperties.Select(u => u.ProjectName);
        }
        private async Task<IEnumerable<string>> SearchResources(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return resources.Select(u => u.EmployeeName).Where(u => u != null);
            resourceProperties = resources.Where(u => u.EmployeeName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return resourceProperties.Select(u => u.EmployeeName);
        }

        //private async Task<IEnumerable<string>> SearchAdUsers(string value)
        //{
        //    // if text is null or empty, show complete list
        //    if (string.IsNullOrEmpty(value))
        //        return adusers.Select(p => p.DisplayName).Where(p => p != null);
        //    properties = adusers.Where(p => p.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

        //    return properties.Select(p => p.DisplayName);
        //}

        public async void UpdateTask(MouseEventArgs e)
        {
            bool? result = await DialogService.ShowMessageBox(
          "Confirm",
          "Do you want to update this task?",
          yesText: "Yes", cancelText: "Cancel");
            state = result == null ? "Canceled" : "Task Updated";
            StateHasChanged();

            taskName = taskName;
            projectId = projectId;
            resourceId = resourceId;
            capacityPercentage = capacityPercentage;
            newNote = newNote;
            //status = status;
            Snackbar.Add($"Creating new task, please do not refresh the page", Severity.Warning);
            try
            {
                //Create new entry
                //add condition if item is linked to planner 
                if (newNote != null) {
                    string dateEntered = DateTime.Now.ToString("MMM dd");

                    _assignedTaskService.CreateNewTaskNote(filteredTaskId, filteredResourceId, newNote);
                    //Close Dialog
                    MudDialog.Close(DialogResult.Ok(true));
                    Snackbar.Add($"Entry sucessfully created.", Severity.Success);
                    await Task.Delay(100);
                    //NavManager.NavigateTo(NavManager.Uri, true);

                    if (linkedToPlanner)
                    {
                        _graphApiService.UpdatePlannerTaskNotes(filteredTaskName, newNote, dateEntered);
                        _graphApiService.UpdatePlannerTaskPreview(filteredTaskName);
                    }
                    else if (linkedToOneList)
                    {
                        _graphApiService.UpdateOneListTaskNotes(filteredTaskName, newNote, dateEntered);
                    }
                    else
                    {
                        // not linked, no further updates needed.
                    }

                }
                if (filteredReminderDate != null)
                {
                    _assignedTaskService.SetTaskReminder(filteredTaskId, filteredReminderDate);
                }
                else
                {
                    //Close Dialog
                    MudDialog.Close(DialogResult.Ok(true));
                    Snackbar.Add($"No changes found.", Severity.Normal);
                    await Task.Delay(100);
                    //NavManager.NavigateTo(NavManager.Uri, true);
                }
                _assignedTaskService.UpdateTaskLastUpdate(filteredTaskId);
            }
            catch
            {
                Snackbar.Add($"Error creating entry. Please contact ITSystems", Severity.Error);
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        private int GetLineCount(string noteText)
        {
            int lineCount = noteText.Count(c => c == '\n');

            if(lineCount < 5) {
                lineCount = 5;
            }
            else
            {
                //no re-assignment needed, return the count.
            }
            return lineCount + 1;
        }

        private async void DeleteTask(int filteredTaskId)
        {
            bool? result = await DialogService.ShowMessageBox(
                "Confirm",
                "Do you want to delete this task?",
                yesText: "Yes", cancelText: "Cancel");
            state = result == null ? "Canceled" : "Task Updated";
            StateHasChanged();
            _assignedTaskService.DeleteTask(filteredTaskId);
            //await Task.Delay(100);
            NavManager.NavigateTo(NavManager.Uri, true);
        }
    }
}
