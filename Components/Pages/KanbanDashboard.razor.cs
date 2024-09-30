using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Leon.Models;
using Leon.Services;
using MudBlazor;
using MudBlazor.Dialog;
using static MudBlazor.Colors;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Azure.Core;
using Azure;


namespace Leon.Components.Pages
{
    public partial class KanbanDashboard
    {
        [Inject] AssignedTaskService _assignedTaskService { get; set; }
        [Inject] DashboardService _dashboardService { get; set; }
        [Inject] ResourceService _resourceService { get; set; }
        [Inject] GraphApiService _graphApiService { get; set; }

        private List<AssignedTask> _assignedTasks { get; set; }
        private List<AssignedTask> assignedTasks { get; set; }

        private List<AssignedTaskNote> _assignedTaskNotes { get; set; }
        private List<AssignedTaskNote> assignedTaskNotes { get; set; }

        private List<DashboardStatus> _dashboardStatuses;
        private List<Resource> _resources;

        [Inject] IDialogService DialogService { get; set; }
                [Inject] NavigationManager NavManager { get; set; }

        //used for MudDialogs
        DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        private MudDropContainer<AssignedTask> _dropContainer;

        public bool showNewTaskButton { get; set; } = false;
        private bool _addSectionOpen;
        private bool _reminders { get; set; } = true;
        bool _disposed;

        public int resourceId { get; set; }
        public int dashboardId { get; set; }
        public List<AssignedTask> _dashboardColumnCapacities { get; set; }
        public int? _dashboardColumnCapacityForResource { get; set; }
        public string latestAssignedTaskNote { get; set; } = "";
        public string currentLoggedInUser { get; set; } = "";
        public string latestAssignedTaskNoteDate { get; set; } = "";
        private static InteractiveBrowserCredential? interactiveCredential;
        // Client configured with user authentication
        private static GraphServiceClient? _userClient;
        private static string[] scopes = new[] { "User.Read" };
        public string taskId{ get; set; }

        public int Value { get; set; }


        public void Dispose() => _disposed = true;
        //public class KanBanSections
        //{
        //    public string Name { get; init; }
        //    public bool NewTaskOpen { get; set; }
        //    public string NewTaskName { get; set; }

        //    public KanBanSections(string name, bool newTaskOpen, string newTaskName)
        //    {
        //        Name = name;
        //        NewTaskOpen = newTaskOpen;
        //        NewTaskName = newTaskName;
        //    }
        //}
        //public class KanbanTaskItem
        //{
        //    public string Name { get; init; }
        //    public string Status { get; set; }

        //    public KanbanTaskItem(string name, string status)
        //    {
        //        Name = name;
        //        Status = status;
        //    }
        //}

        //public class KanBanNewForm
        //{
        //    [Required]
        //    [StringLength(10, ErrorMessage = "Name length can't be more than 10.")]
        //    public string Name { get; set; }
        //}


        /* Setup for board  */
        //private List<KanBanSections> _sections = new()
        //{
        //    new KanBanSections("To Do", false, String.Empty),
        //    new KanBanSections("In Process", false, String.Empty),
        //    new KanBanSections("Done", false, String.Empty),
        //};

        //private List<KanbanTaskItem> _tasks = new()
        //{
        //    new KanbanTaskItem("Write unit test", "To Do"),
        //    new KanbanTaskItem("Some docu stuff", "To Do"),
        //    new KanbanTaskItem("Walking the dog", "To Do"),
        //};

        //KanBanNewForm newSectionModel = new KanBanNewForm();

        protected override async Task OnInitializedAsync()
        {

            await _graphApiService.PopulateLeonTasksFromPlanner();
            await _graphApiService.PopulateLeonTasksFromOneList();

            //Start GraphLogin 

            //End Graph Login
            //Grab method from the service listed above
            //Function:
            currentLoggedInUser = await _graphApiService.GetCurrentLoggedInUser();
            _dashboardStatuses = await _dashboardService.GetDashboardStatuses();
            _dashboardColumnCapacities = await _assignedTaskService.GetTaskCapacitiesByInProgressStatus();
            _resources = await _resourceService.GetResourceByName(currentLoggedInUser);
            _assignedTasks = await _assignedTaskService.GetAllActiveAssignedTasks();
            assignedTasks = _assignedTasks.Where(t => t.ResourceId != null).ToList();
            _assignedTaskNotes = await _assignedTaskService.GetAllActiveTaskNotes();

        }

        /* handling board events */
        private async Task PopulateTasks(int currentResourceId)
        {
            assignedTasks = _assignedTasks.Where(t => t.ResourceId == currentResourceId).ToList();
            _dashboardColumnCapacityForResource = assignedTasks.Sum(t => t.CapacityPercentage);

        }
        ///* handling board events */
        private async Task PopulateCapacity(int currentResourceId)
        {
            assignedTasks = _assignedTasks.Where(t => t.ResourceId == currentResourceId).ToList();
            _dashboardColumnCapacityForResource = assignedTasks.Where(t => t.Status == "In Progress").Sum(t => t.CapacityPercentage);
            //_dashboardColumnCapacityForResource = assignedTasks.Where(t => t.ResourceId == currentResourceId).Sum(t => t.CapacityPercentage);

        }
        ///* handling board events */
        private async Task GetLatestTaskNote(int taskId)
        {
            latestAssignedTaskNote = _assignedTaskNotes.Where(n => n.TaskId == taskId).OrderByDescending(n => n.CreationDate).Select(n => n.NoteText).SingleOrDefault();
            latestAssignedTaskNoteDate = _assignedTaskNotes.Where(n => n.TaskId == taskId).OrderByDescending(n => n.CreationDate).Select(n => n.CreationDate).SingleOrDefault().ToShortDateString();
            //_dashboardColumnCapacityForResource = assignedTasks.Where(t => t.ResourceId == currentResourceId).Sum(t => t.CapacityPercentage);

        }

        public void ToggleReminders(int currentResourceId)
        {
            _reminders = !_reminders;
            try
            {
                //searchString = e.Value.ToString();
                if (_reminders == false)
                {
                    assignedTasks = _assignedTasks.Where(t => t.ResourceId == currentResourceId).Where(t => t.IsReminder == null).ToList();

                }
                else
                {
                    assignedTasks = _assignedTasks.Where(t => t.ResourceId == currentResourceId).ToList();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        /* handling board events */
        private async void TaskUpdated(MudItemDropInfo<AssignedTask> info)
        {
            _dashboardService.UpdateDashboardTaskStatus(info.DropzoneIdentifier, info.Item.TaskId);
            PopulateCapacity((int)(info.Item.ResourceId));

            // To initialize your graphClient, see https://learn.microsoft.com/en-us/graph/sdks/create-client?from=snippets&tabs=csharp

            if(info.Item.LinkedToPlanner == true)
            {
                _graphApiService.UpdateTaskStatus(info.Item.TaskName, info.DropzoneIdentifier);
            }
            else if (info.Item.LinkedToOneList == true)
            {
                _graphApiService.UpdateTaskStatus(info.Item.TaskName, info.DropzoneIdentifier);
            }
            else
            {
            }

            ////Get Task in Planner list
            //var tasks = await GraphServiceClient.Me.Planner.Tasks.Request().GetAsync();
            //var taskNameToSearch = info.Item.TaskName; // Replace with the actual task name
            //var matchingTask = tasks.FirstOrDefault(t => t.Title.Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));

            //if (matchingTask != null)
            //{
            //    taskId = matchingTask.Id;
            //    // Now you have the task ID!
            //}
            ////Update in Planner
            ////
            //var taskToUpdate = await GraphServiceClient.Planner.Tasks[taskId].Request().GetAsync();
            //if (taskToUpdate != null)
            //{
            //    var requestBody = new PlannerTask
            //    {
            //        PercentComplete = 50
            //    };
            //    //taskToUpdate.Title = "[*] - Add Michael Pengelly access in Eagle TEST";
            //    await GraphServiceClient.Planner.Tasks[taskId].Request().Header("If-Match", taskToUpdate.GetEtag()).UpdateAsync(requestBody);

            //}
            ////
            _dropContainer.Refresh();
        }


        //ADDING NEW SECITON ( NOT NEEDED)
        //private void OnValidSectionSubmit(EditContext context)
        //{
        //	_sections.Add(new KanBanSections(newSectionModel.Name, false, String.Empty));
        //	newSectionModel.Name = string.Empty;
        //	_addSectionOpen = false;
        //}

        //private void OpenAddNewSection()
        //{
        //	_addSectionOpen = true;
        //}

        //ADDING NEW TASK (NEEDED)
        //private void AddTask(AssignedTask item)
        //{
        //    _assignedTaskService.CreateNewTask(item.TaskId, item.TaskName, item.ProjectId.Value, item.OperationId.Value, item.ResourceId.Value);
        //    _dropContainer.Refresh();
        //}

        //used for the dialog popup
        //used for the dialog popup
        private void OpenNewProjectTaskDialog(DialogOptions options, string passedStatus, int filteredResourceId)
        {
            //var userEmail = helperservice.GetCurrentLoggedInUserEmail();
            var parameters = new DialogParameters();
            parameters.Add("passedStatus", passedStatus);
            parameters.Add("filteredResourceId", filteredResourceId);
            DialogService.Show<Shared.Dialogs.CreateNewProjectTaskDialog>("Create New Project Task", parameters, options);
        }
        private void OpenNewOperationTaskDialog(DialogOptions options, string passedStatus, int filteredResourceId)
        {
            //var userEmail = helperservice.GetCurrentLoggedInUserEmail();
            var parameters = new DialogParameters();
            parameters.Add("passedStatus", passedStatus);
            parameters.Add("filteredResourceId", filteredResourceId);
            DialogService.Show<Shared.Dialogs.CreateNewOperationTaskDialog>("Create New Operation Task", parameters, options);
        }
        private void OpenViewTaskDialog(DialogOptions options, int filteredTaskId, int? filteredProjectId, int? filteredResourceId, string filteredTaskName, string capacityPercentage, DateTime? filteredReminderDate, bool? linkedToPlanner, bool? LinkedToOneList)
        {
            assignedTaskNotes = _assignedTaskNotes.Where(n => n.TaskId == filteredTaskId).ToList();
            //var userEmail = helperservice.GetCurrentLoggedInUserEmail();
            var parameters = new DialogParameters();
            parameters.Add("filteredProjectId", filteredProjectId);
            parameters.Add("filteredResourceId", filteredResourceId);
            parameters.Add("filteredTaskId", filteredTaskId);
            parameters.Add("filteredTaskName", filteredTaskName);
            parameters.Add("capacityPercentage", capacityPercentage);
            parameters.Add("filteredReminderDate", filteredReminderDate);
            parameters.Add("linkedToPlanner", linkedToPlanner);
            parameters.Add("linkedToOneList", LinkedToOneList);

            DialogService.Show<Shared.Dialogs.ViewTaskDialog>(filteredTaskName, parameters, options);
        }

        private void ClearCacheWithLogin()
        {
            new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddHours(-1),
                Path = "/",
                HttpOnly = true,
                IsEssential = true,
                Domain = ""
            };
            NavManager.NavigateTo(NavManager.Uri, true);
        }
            //private void DeleteSection(KanBanSections section)
            //{
            //	if (_sections.Count == 1)
            //	{
            //		_tasks.Clear();
            //		_sections.Clear();
            //	}
            //	else
            //	{
            //		int newIndex = _sections.IndexOf(section) - 1;
            //		if (newIndex < 0)
            //		{
            //			newIndex = 0;
            //		}

        //		_sections.Remove(section);

        //		var tasks = _tasks.Where(x => x.Status == section.Name);
        //		foreach (var item in tasks)
        //		{
        //			item.Status = _sections[newIndex].Name;
        //		}
        //	}
        //}
        }
}
