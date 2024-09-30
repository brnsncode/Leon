using Azure;
using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;
using static MudBlazor.Colors;
using Operation = Leon.Models.Operation;

namespace Leon.Services
{
	public class OperationService
	{
		//This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

		//Initialize IAMContext and IAMService to be injected
		Leon_Context leon_context;

		//ActiveDirectoryService Constructor
		public OperationService(Leon_Context leonctx)
		{
			//Inject anotherService into LeonWorkerService contructor

			//Inject IAMContext into LeonWorkerService contructor
			leon_context = leonctx;
			leon_context.Database.SetCommandTimeout(120);
		}
		//Gets the latest AdUsers
		public Task<List<Project>> GetProjects()
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var projects = leon_context.Projects.ToListAsync();

			return projects;
		}
        public Task<string> GetProjectNameByProjectId(int projectId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var projectName = leon_context.Projects.Where(p => p.ProjectId == projectId)
				.Select(p => p.ProjectName)
				.SingleOrDefaultAsync();

            return projectName;
        }

        public Task<string> GetTaskNameByTaskId(int operationId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var operationName = leon_context.Operations.Where(p => p.OperationId == operationId)
                .Select(p => p.OperationName)
                .SingleOrDefaultAsync();

            return operationName;
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
		//public Task<List<ResourceCapacityView>> GetCurrentResourcesWithCapacity()
		//{
		//	//Get Latest syncID
		//	//int latestSyncId = leon.GetLatestSyncId();

		//	//Filter users by latest syncID 
		//	var currentResourcesWithCapacity = leon_context.ResourceCapacityViews.ToListAsync();

		//	return currentResourcesWithCapacity;
		//}



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
        //Gets the latest AdUsers
        public Task<List<AssignedTaskNote>> GetAllActiveTaskNotesByTaskId(int filteredTaskId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTaskNotesById = leon_context.AssignedTaskNotes.Where(n => n.TaskId == filteredTaskId).ToListAsync();

            return currentAssignedTaskNotesById;
        }

        public Task<List<AssignedTask>> GetAllActiveAssignedTasksByResource(int resourceId)
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var currentAssignedTasks = leon_context.AssignedTasks.Where(r => r.ResourceId == resourceId).ToListAsync();

			return currentAssignedTasks;
		}

        public int GetOperationIdByTaskName(string operationName, int resourceId)
        {
			int operationId;
            try
            {
                //Filter users by latest syncID 
                operationId = leon_context.Operations.Where(o => o.OperationName == operationName && o.ResourceId == resourceId).Select(o => o.OperationId).FirstOrDefault();
                return operationId;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                return 0;
            }


        }
        public string CreateNewOperationTask(string taskName, int resourceId)
		{
			try
			{
                //create new owner entry
                Operation newTask = new Operation
                {
					OperationName = taskName,
					ResourceId = resourceId
                };
				leon_context.Operations.Add(newTask);
				leon_context.SaveChanges();
			}
			catch (InvalidOperationException ex)
			{
				Debug.WriteLine(ex);
			}

			return "Saved Sucessfully.";
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
