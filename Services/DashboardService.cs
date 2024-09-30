using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;
using static MudBlazor.Colors;

namespace Leon.Services
{
	public class DashboardService
	{
		//This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

		//Initialize IAMContext and IAMService to be injected
		Leon_Context leon_context;

		//ActiveDirectoryService Constructor
		public DashboardService(Leon_Context leonctx)
		{
			//Inject anotherService into LeonWorkerService contructor

			//Inject IAMContext into LeonWorkerService contructor
			leon_context = leonctx;
			leon_context.Database.SetCommandTimeout(120);
		}

		public Task<List<DashboardStatus>> GetDashboardStatuses()
		{
			//Get Latest syncID
			//int latestSyncId = leon.GetLatestSyncId();

			//Filter users by latest syncID 
			var dashboardStatuses = leon_context.DashboardStatuses.ToListAsync();

			return dashboardStatuses;
		}
        public Task<decimal> GetInProgressCapacityByResourceId(int resourceId)
        {
            //Get Latest syncID
            //int latestSyncId = leon.GetLatestSyncId();

            //Dashboard id 2 = In Progress
            var dashboardStatuses = leon_context.AssignedTasks
                                 .Join(leon_context.DashboardStatuses,
                                       t => t.Status,
                                       d => d.Name,
                                       (t, d) => new { Task = t, Dashboard = d })
                                 .Where(td => td.Task.ResourceId == resourceId && td.Dashboard.DashboardId == 2)
                                 .SumAsync(td => (decimal)td.Task.CapacityPercentage);

            return dashboardStatuses;
        }

        public void UpdateDashboardTaskStatus(string newStatus, int taskId)
		{
			var dashboardTask = leon_context.AssignedTasks.SingleOrDefault(t => t.TaskId == taskId);
			dashboardTask.Status = newStatus;
			leon_context.SaveChanges();
		}


	}
}
