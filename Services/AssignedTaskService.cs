using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Graph;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MudBlazor.Colors;

namespace Leon.Services
{
	public class AssignedTaskService
	{
        //This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

        //Initialize IAMContext and IAMService to be injected
        private readonly GraphServiceClient _graphClient;
        Leon_Context leon_context;

		//ActiveDirectoryService Constructor
		public AssignedTaskService(Leon_Context leonctx, GraphServiceClient graphClient)
		{
			//Inject anotherService into LeonWorkerService contructor

			//Inject IAMContext into LeonWorkerService contructor
			leon_context = leonctx;
			leon_context.Database.SetCommandTimeout(120);

            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        }
		//Gets the latest AdUsers
		public Task<List<AssignedTask>> GetCurrentTasks()
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentTasks = leon_context.AssignedTasks.ToListAsync();

			return currentTasks;
		}
		//Gets the latest AdUsers
		//public Task<List<Allocation>> GetCurrentAllocations()
		//{
		//    //Get Latest syncID
		//    //int latestSyncId = leon.GetLatestSyncId();

		//    //Filter users by latest syncID 
		//    var currentAllocations = leon_context.Allocations.ToListAsync();

		//    return currentAllocations;
		//}
		//Gets the latest AdUsers
		//public Task<List<Project>> GetCurrentProjects()
		//{
		//	//Get Latest syncID
		//	//int latestSyncId = leon.GetLatestSyncId();

		//	//Filter users by latest syncID 
		//	//var currentProjects = leon_context.Projects.Where(p => p.Status == "Active").ToListAsync();
		//	var currentProjects = leon_context.Projects.ToListAsync();

		//	return currentProjects;
		//}
		//Gets the latest AdUsers
		public Task<List<Resource>> GetCurrentResources()
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentResource = leon_context.Resources.ToListAsync();

			return currentResource;
		}
        //Gets the latest AdUsers



        //Gets the latest AdUsers
        public Task<List<AssignedTask>> GetAllActiveAssignedTasks()
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentAssignedTasks = leon_context.AssignedTasks.ToListAsync();

			return currentAssignedTasks;
		}

        //Gets the latest AdUsers
        public Task<List<AssignedTaskNote>> GetAllActiveTaskNotes()
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTaskNotes = leon_context.AssignedTaskNotes.ToListAsync();

            return currentAssignedTaskNotes;
        }
        public Task<List<AssignedTask>> GetAllTaskResourceAllocations()
        {
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentCapacitities = leon_context.AssignedTasks.Join(leon_context.DashboardStatuses,
                                   t => t.Status,
                                   d => d.Name,
                                   (t, d) => t).ToListAsync();

            return currentCapacitities;
        }

        //Gets the latest AdUsers
        public Task<List<AssignedTaskNote>> GetAllActiveTaskNotesByTaskId(int filteredTaskId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTaskNotesById = leon_context.AssignedTaskNotes.Where(n => n.TaskId == filteredTaskId).ToListAsync();

            return currentAssignedTaskNotesById;
        }
        //Gets the latest AdUsers
        public Task<List<AssignedTaskNote>> GetAllActiveTaskNotesByTaskIdDesc(int filteredTaskId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTaskNotesById = leon_context.AssignedTaskNotes.Where(n => n.TaskId == filteredTaskId).OrderByDescending(t => t.CreationDate).ToListAsync();

            return currentAssignedTaskNotesById;
        }

        //Gets the latest AdUsers
        public async Task<string> GetLatestTaskNoteByTaskId(int filteredTaskId)
        {
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentAssignedTaskNotesById = await leon_context.AssignedTaskNotes.Where(n => n.TaskId == filteredTaskId).OrderByDescending(n => n.CreationDate).Select(n => n.NoteText).SingleOrDefaultAsync();

            return currentAssignedTaskNotesById;
        }

        public Task<List<AssignedTask>> GetTaskCapacitiesByInProgressStatus()
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var capacityPercentage = leon_context.AssignedTasks.Where(t => t.Status == "In Progress").ToListAsync();

            return capacityPercentage;
        }

        public Task<decimal> GetStatusCapacityByResourceId(int resourceId, int dashboardId)
        {
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var capacityPercentage = leon_context.AssignedTasks.Join(leon_context.DashboardStatuses,
                                       t => t.Status,
                                       d => d.Name,
                                       (t, d) => new { Task = t, Dashboard = d })
                                 .Where(td => td.Task.ResourceId == 2 && td.Dashboard.DashboardId == dashboardId)
                                 .SumAsync(td => (decimal)td.Task.CapacityPercentage);

            return capacityPercentage;
        }

        public Task<List<AssignedTask>> GetAllActiveAssignedTasksByResource(int resourceId)
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentAssignedTasks = leon_context.AssignedTasks.Where(r => r.ResourceId == resourceId).ToListAsync();

			return currentAssignedTasks;
		}


		public string CreateNewProjectTask(string taskName, int projectId, int operationId, int resourceId, string capacityPecentage, string status, bool isReminder, bool linkToPlanner)
		{
			try
			{
				//create new owner entry
				AssignedTask newTask = new AssignedTask
				{
					TaskName = taskName,
					ProjectId = projectId,
					OperationId = operationId,
                    ResourceId = resourceId,
					CapacityPercentage = Int32.Parse(capacityPecentage),
					Status = status,
					IsReminder = isReminder,
                    LinkedToPlanner = linkToPlanner
                };
				leon_context.AssignedTasks.Add(newTask);
				leon_context.SaveChanges();
			}
			catch (InvalidOperationException ex)
			{
				Debug.WriteLine(ex);
			}

			return "Saved Sucessfully.";
		}
        public string CreateNewOperationsTask(string taskName, int projectId, int operationId, int resourceId, string capacityPecentage, string status, bool isReminder, bool linkToPlanner, bool linkToOneList)
        {
            try
            {
                //create new owner entry
                AssignedTask newTask = new AssignedTask
                {
                    TaskName = taskName,
                    ProjectId = projectId,
                    OperationId = operationId,
                    ResourceId = resourceId,
                    CapacityPercentage = Int32.Parse(capacityPecentage),
                    Status = status,
                    IsReminder = isReminder,
                    LinkedToPlanner = linkToPlanner,
                    LinkedToOneList = linkToOneList
                };
                leon_context.AssignedTasks.Add(newTask);
                leon_context.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
            }

            return "Saved Sucessfully.";
        }

        public string CreateNewTaskNote(int taskId, int resourceId, string newNote)
        {
            try
            {
                //create new owner entry
				if(newNote == "")
				{
                    return "empty";
                }
				else
				{
                    AssignedTaskNote newTaskNote = new AssignedTaskNote
                    {
						TaskId = taskId,
						ResourceId = resourceId,
                        NoteText = newNote,
                    };
                    leon_context.AssignedTaskNotes.Add(newTaskNote);
                    leon_context.SaveChanges();
                }

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
            }

            return "Saved Sucessfully.";
        }

        public string UpdateTaskLastUpdate(int taskId)
        {
            try
            {
                // First, retrieve the row you want to update
                var task = leon_context.AssignedTasks.FirstOrDefault(t => t.TaskId == taskId);

                if (task != null)
                {
                    // Update the properties of the employee
                    task.LastUpdated = DateTime.Now;

                    // Submit the changes to the database
                    leon_context.SaveChanges();
                }

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
            }

            return "Saved Sucessfully.";
        }
        public string GetTaskReminder(int taskId)
        {
            try
            {
                // First, retrieve the row you want to update
                var task = leon_context.AssignedTasks.Where(t => t.TaskId == taskId).Select(t => t.SetReminderDate).FirstOrDefault().ToString();
                return task;

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                return null;
            }


        }

        public Task<List<DateTime?>> GetTodaysTaskReminders()
        {
            try
            {
                DateTime today = DateTime.Today;
                // First, retrieve the row you want to update
                var task = leon_context.AssignedTasks.Where(t => t.SetReminderDate != null && t.SetReminderDate == today).Select(t => t.SetReminderDate).ToListAsync();
                return task;

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                return null;
            }


        }
        public string SetTaskReminder(int taskId, DateTime? reminderDate)
        {
            try
            {
                // First, retrieve the row you want to update
                var task = leon_context.AssignedTasks.FirstOrDefault(t => t.TaskId == taskId);

                if (task != null)
                {
                    // Update the properties of the employee
                    task.SetReminderDate = reminderDate;

                    // Submit the changes to the database
                    leon_context.SaveChanges();
                }

            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
            }

            return "Saved Sucessfully.";
        }

        public void DeleteTask(int taskId)
		{
			// Query the database for the row to be deleted

			//first delete the notes since they are table constraints
			var taskNotesToDelete = leon_context.AssignedTaskNotes
				.Where(row => row.TaskId == taskId);

			leon_context.AssignedTaskNotes.RemoveRange(taskNotesToDelete);
			leon_context.SaveChanges();

            var taskToDelete = leon_context.AssignedTasks
				.Where(row => row.TaskId == taskId).SingleOrDefault();

            leon_context.AssignedTasks.Remove(taskToDelete);
            leon_context.SaveChanges();
            //         foreach (var row in rowsToDelete)
            //{
            //             if (row != null)
            //             {
            //                 // Call the DeleteOnSubmit method
            //                 leon_context.AssignedTaskNotes.ExecuteDeleteAsync(row);

            //                 // Submit the change to the database
            //                 dataContext.SubmitChanges();
            //             }
            //         }// Check if the row exists

        }


        //public string CreateNewProject(int projectId, string projectName, string businessOwner, int initiativeId, int resourceId)
        //{
        //	try
        //	{
        //		//create new owner entry
        //		Project newProject = new Project
        //		{
        //			ProjectId = projectId,
        //			ProjectName = projectName,
        //			BusinessOwner = businessOwner,
        //			InitiativeId = initiativeId,
        //			ResourceId = resourceId
        //		};
        //		//leon_context.Projects.Add(newProject);
        //		leon_context.SaveChanges();
        //	}
        //	catch (InvalidOperationException ex)
        //	{
        //		Debug.WriteLine(ex);
        //	}

        //	return "Saved Sucessfully.";
        //}


    }
}
