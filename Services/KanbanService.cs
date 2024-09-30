using Leon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;
using static MudBlazor.Colors;

namespace Leon.Services
{
	public class KanbanService
	{
		//This service is used for general Active Directory stuff or getting ADuser related content from the IAM database

		//Initialize IAMContext and IAMService to be injected
		Leon_Context leon_context;

		//ActiveDirectoryService Constructor
		public KanbanService(Leon_Context leonctx)
		{
			//Inject anotherService into LeonWorkerService contructor

			//Inject IAMContext into LeonWorkerService contructor
			leon_context = leonctx;
			leon_context.Database.SetCommandTimeout(120);
		}

		//public Task<List<KanbanColumn>> GetKanbanColumns()
		//{
		//	//Get Latest syncID
		//	//int latestSyncId = leon.GetLatestSyncId();

		//	//Filter users by latest syncID 
		//	var kanbanColumns = leon_context.KanbanColumns.ToListAsync();

		//	return kanbanColumns;
		//}

	}
}
