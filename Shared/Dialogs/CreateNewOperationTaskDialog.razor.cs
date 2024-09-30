using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Graph;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Leon.Services;
using Microsoft.JSInterop;
using Leon.Models;
using System.Net.NetworkInformation;
using System.Resources;
using Microsoft.Graph;
using System;

namespace Leon.Shared.Dialogs
{
    public partial class CreateNewOperationTaskDialog
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
        [Inject] OperationService _operationService { get; set; }
        [Inject] DashboardService _dashboardService { get; set; }
        [Inject] ResourceService _resourceService { get; set; }
        [Inject] GraphApiService _graphApiService { get; set; }


        //2. Other Injections
        [Inject] IJSRuntime jsruntime { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager NavManager { get; set; }

        //3. Parameters
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        private MudDropContainer<CreateNewOperationTaskDialog> _dropContainer;
        //Initialize searchString to "" to prevent nulls
        public string searchString { get; set; } = "";

        //Default starting Date will be 365 days ago
        public DateTime startDate { get; set; } = DateTime.Today.AddDays(-365);
        //Default end Date will be today's date
        //Date starts off end date with 23:59pm, but when filter is changed in front end, it changes to 
        public DateTime endDate { get; set; } = DateTime.Today.AddHours(23).AddMinutes(59);

        private DateTime effectiveDate { get; set; }
        private bool resetValueOnEmptyText;
        private bool isReminder { get; set; } = true;
        private bool createPlannerTask { get; set; } = true;
        private bool linkToPlanner { get; set; }
        private string projectValue { get; set; } = "";
        private string resourceValue { get; set; } = "";
        private string dashboardValue { get; set; } = "";
        private string newNote { get; set; } = "";
        private string filteredResourceName { get; set; } = "";
        public int filteredTaskId { get; set; }
        public int filteredProjectId { get; set; }
        public int filteredOperationId { get; set; }
        //public int filteredResourceId { get; set; }
        public int filteredDashboardStatus { get; set; }

        [Parameter] public string passedStatus { get; set; } = "";
        [Parameter] public string passedResource { get; set; } = "";
        [Parameter] public string taskId { get; set; } = "";
        [Parameter] public string taskName { get; set; } = "";
        [Parameter] public string projectId { get; set; } = "";
        [Parameter] public string operationId { get; set; } = "";
        [Parameter] public string resourceId { get; set; } = "";
        public string capacityPercentage { get; set; } = "10";
        [Parameter] public string status { get; set; } = "";
        [Parameter] public int filteredResourceId { get; set; }

        void Cancel() => MudDialog.Cancel();



        //void Cancel() => MudDialog.Cancel();

        IEnumerable<Project> projectProperties = null;
        IEnumerable<Resource> resourceProperties = null;
        IEnumerable<DashboardStatus> dashboardProperties = null;

        //4. Lists
        //Setting temp adusers list to load the list upon initialization
        private List<Project> _projects;
        public List<Project> projects => _projects.Where(p => p.ProjectName != null && p.ProjectName.ToLower().Contains(searchString.ToLower())).ToList();
        private List<Resource> _resources;
        public List<Resource> resources => _resources.Where(r => r.EmployeeName != null && r.EmployeeName.ToLower().Contains(searchString.ToLower())).ToList();
        private List<DashboardStatus> _dashboardStatuses;
        public List<DashboardStatus> dashboardStatuses => _dashboardStatuses.Where(r => r.Name != null).ToList();
        //Then setting new adusers list to make previous list filterable/searchable (u.Name?.ToLower().Contains(searchString.ToLower()) ?? false))
        //public List<Project> projects => _projects.Where(p => (p.TechnologyLeads?.ToLower().Contains(searchString.ToLower()) ?? false)
        //    && (p.DateEntered >= startDate && p.DateEntered <= endDate)).ToList();


        protected override async Task OnInitializedAsync()
        {
            //Grab method from the service listed above
            //Function:
            _projects = await _leonWorkerService.GetAllActiveProjects();
            _resources = await _leonWorkerService.GetCurrentResources();
            filteredResourceName = await _resourceService.GetResourceNameByResourceId(filteredResourceId);
            _dashboardStatuses = await _dashboardService.GetDashboardStatuses();
        }

        //Input event handler to handle real time email body updates
        public void OnInputHandler(ChangeEventArgs e)
        {
            taskId = e.Value.ToString();
            newNote = e.Value.ToString();
            taskName = e.Value.ToString();
            filteredProjectId = Int32.Parse(e.Value.ToString());
            operationId = e.Value.ToString();
            filteredResourceId = Int32.Parse(e.Value.ToString());
            capacityPercentage = e.Value.ToString();
            status = e.Value.ToString();
        }

        private async Task<IEnumerable<string>> SearchProjects(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return projects.Select(u => u.ProjectName).Where(u => u != null);
            projectProperties = projects.Where(u => u.ProjectName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            filteredProjectId = projectProperties.Select(p => p.ProjectId).FirstOrDefault();

           return projectProperties.Select(u => u.ProjectName);
        }
        private async Task<IEnumerable<string>> SearchResources(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return resources.Select(u => u.EmployeeName).Where(u => u != null);
            resourceProperties = resources.Where(u => u.EmployeeName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            filteredResourceId = resourceProperties.Select(p => p.ResourceId).FirstOrDefault();

            return resourceProperties.Select(u => u.EmployeeName);
        }
        private async Task<IEnumerable<string>> SearchDashboardStatuses(string value)
        {
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return dashboardStatuses.Select(u => u.Name).Where(u => u != null);
            dashboardProperties = dashboardStatuses.Where(u => u.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            filteredDashboardStatus = dashboardProperties.Select(p => p.DashboardId).FirstOrDefault();

            return dashboardProperties.Select(u => u.Name);
        }


        //private async Task<IEnumerable<string>> SearchAdUsers(string value)
        //{
        //    // if text is null or empty, show complete list
        //    if (string.IsNullOrEmpty(value))
        //        return adusers.Select(p => p.DisplayName).Where(p => p != null);
        //    properties = adusers.Where(p => p.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

        //    return properties.Select(p => p.DisplayName);
        //}
        public string CalculateTime(string percentage)
        {
            if (double.TryParse(percentage, out double percent))
            {
                double totalHours = 8.0;
                double timeInHours = (percent / 100) * totalHours;

                // Calculate hours and minutes
                int hours = (int)timeInHours;
                int minutes = (int)((timeInHours - hours) * 60);

                return hours + " hour(s) and " + minutes + " minute(s)";
            }
            else
            {
                return "Invalid percentage";
            }
        }

        public async void CreateNewOperationsTask(MouseEventArgs e)
        {
            taskName = taskName;
            filteredOperationId = filteredProjectId;
            filteredOperationId = filteredOperationId;
            filteredResourceId = filteredResourceId;
            filteredDashboardStatus = filteredDashboardStatus;
            newNote = newNote;
            capacityPercentage = capacityPercentage;
            //status = status;
            Snackbar.Add($"Creating new directory owner, please do not refresh the page", Severity.Warning);
            try
            {
                if (createPlannerTask)
                {
                    linkToPlanner = true;
                    _graphApiService.CreatePlannerTask(taskName, passedStatus);
                    //await Task.Delay(5000);
                    //_graphApiService.UpdateTaskStatusNotes(taskName, "Task Created", DateTime.Now.ToString());
                    //await Task.Delay(2000);
                    //_graphApiService.UpdateTaskPreview(taskName);

                }
                else
                {
                    linkToPlanner = false;
                    // skip
                }
                //Create new entry
                if (filteredProjectId == 0)
                {
                    //add operational item.
                    _operationService.CreateNewOperationTask(taskName, filteredResourceId);
                    int operationId = _operationService.GetOperationIdByTaskName(taskName, filteredResourceId);
                    _assignedTaskService.CreateNewOperationsTask(taskName, filteredProjectId, operationId, filteredResourceId, capacityPercentage, passedStatus, isReminder, linkToPlanner, false);

                }
                else
                {
                    _assignedTaskService.CreateNewOperationsTask(taskName, filteredProjectId, filteredOperationId, filteredResourceId, capacityPercentage, passedStatus, isReminder, linkToPlanner, false);
                }
                //Create new note entry
                //var response = _assignedTaskService.CreateNewTaskNote(filteredTaskId, filteredResourceId, newNote);
                //if (response == "empty")
                //{
                //    //Close Dialog
                //    MudDialog.Close(DialogResult.Ok(true));
                //    Snackbar.Add($"No changes found.", Severity.Normal);
                //    await Task.Delay(1000);
                //    NavManager.NavigateTo(NavManager.Uri, true);
                //}
                //else
                //{
                //    //Close Dialog
                //    MudDialog.Close(DialogResult.Ok(true));
                //    Snackbar.Add($"Entry sucessfully created.", Severity.Success);
                //    await Task.Delay(1000);
                //    NavManager.NavigateTo(NavManager.Uri, true);
                //}
                //Close Dialog
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add($"Entry sucessfully created.", Severity.Success);
                await Task.Delay(100);
                NavManager.NavigateTo(NavManager.Uri, true);
            }
            catch
            {
                Snackbar.Add($"Error creating entry. Please contact ITSystems", Severity.Error);
                MudDialog.Close(DialogResult.Ok(true));
            }

            try
            {

                //var requestBody = new PlannerTask
                //{
                //    PlanId = "TIhaF5ZOnU2_2cfJJzzuVmQAFbYg",
                //    BucketId = "6e3H3uJqxECLLRvKCtZ_zmQACt0V",
                //    Title = "Test Task 2",
                //    Assignments = new PlannerAssignments
                //    {
                //        AdditionalData = new Dictionary<string, object>
                //        {
                //            {
                //                "abd81ab0-322b-4b56-9f8e-ab3deae30ab8" , new PlannerAssignment
                //                {
                //                    ODataType = "#microsoft.graph.plannerAssignment",
                //                    OrderHint = " !",
                //                }
                //            },
                //        },
                //    },
                //};



            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
