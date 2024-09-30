using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;
using static MudBlazor.Colors;

namespace Leon.Services
{
    public class PlannerSyncService
    {
        //This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

        //Initialize IAMContext and IAMService to be injected
        Leon_Context leon_context;

        //ActiveDirectoryService Constructor
        public PlannerSyncService(Leon_Context leonctx)
        {
            //Inject anotherService into LeonWorkerService contructor

            //Inject IAMContext into LeonWorkerService contructor
            leon_context = leonctx;
            leon_context.Database.SetCommandTimeout(120);
        }

        //Gets the latest SyncID
        public DateTime GetLatestSyncDate()
        {
            var latestSyncID = (DateTime)leon_context.PlannerSyncHistories.Select(s => s.SyncDate).ToList().LastOrDefault();

            return latestSyncID;
        }
        //Gets the latest AdUsers
        public Task<List<Resource>> GetResourceByName(string employeeName)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var resources = leon_context.Resources.Where(r => r.EmployeeName == employeeName).ToListAsync();

            return resources;
        }
        public Task<string> GetResourceNameByResourceId(int resourceId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Filter users by latest syncID 
            var resourceName = leon_context.Resources.Where(p => p.ResourceId == resourceId)
                .Select(p => p.EmployeeName)
                .SingleOrDefaultAsync();

            return resourceName;
        }

        public void UpdateDashboardTaskStatus(string newStatus, int taskId)
        {
            var dashboardTask = leon_context.AssignedTasks.SingleOrDefault(t => t.TaskId == taskId);
            dashboardTask.Status = newStatus;
            leon_context.SaveChanges();
        }


    }
}
