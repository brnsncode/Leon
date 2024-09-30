using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;
using static MudBlazor.Colors;

namespace Leon.Services
{
    public class LeonWorkerService
    {
        //This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

        //Initialize IAMContext and IAMService to be injected
        Leon_Context leon_context;

        //ActiveDirectoryService Constructor
        public LeonWorkerService(Leon_Context leonctx)
        {
            //Inject anotherService into LeonWorkerService contructor

            //Inject IAMContext into LeonWorkerService contructor
            leon_context = leonctx;
            leon_context.Database.SetCommandTimeout(120);
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

		//public Task<List<KanbanColumn>> GetKanbanColumns()
		//{
		//	//Get Latest syncID
		//	//int latestSyncId = leon.GetLatestSyncId();

		//	//Filter users by latest syncID 
		//	var kanbanColumns = leon_context.KanbanColumns.ToListAsync();

		//	return kanbanColumns;
		//}
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
  //      {
  //          //Get Latest syncID
  //          //int latestSyncId = leon.GetLatestSyncId();

  //          //Filter users by latest syncID 
  //          //var currentProjects = leon_context.Projects.Where(p => p.Status == "Active").ToListAsync();
  //          var currentProjects = leon_context.Proj.ToListAsync();

  //          return currentProjects;
  //      }
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
        public Task<List<Project>> GetAllActiveProjects()
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentProjects = leon_context.Projects.ToListAsync();

            return currentProjects;
        }



        //Gets the latest AdUsers
        public Task<List<AssignedTask>> GetAllActiveAssignedTasks()
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTasks = leon_context.AssignedTasks.ToListAsync();

            return currentAssignedTasks;
        }
        public Task<List<AssignedTask>> GetAllActiveAssignedTasksByResource(int resourceId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var currentAssignedTasks = leon_context.AssignedTasks.Where(r => r.ResourceId == resourceId).ToListAsync();

            return currentAssignedTasks;
        }


        public string CreateNewProject(int projectId, string projectName, string businessOwner, string initiativeId, int resourceId)
        {
            try
            {
                //create new owner entry
                Project newProject = new Project
                {
                    ProjectId = projectId,
                    ProjectName = projectName,
                    BusinessOwner = businessOwner,
                    InitiativeId = initiativeId,
                    ResourceId = resourceId
                };
                //leon_context.Projects.Add(newProject);
                leon_context.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
            }

            return "Saved Sucessfully.";
        }


    }
}
