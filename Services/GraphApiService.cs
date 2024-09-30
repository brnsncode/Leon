using Microsoft.Graph;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using Azure;
using Leon.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Leon.Services
{
    public class GraphApiService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly AssignedTaskService _assignedTaskService;
        private readonly OperationService _operationService;

        public GraphApiService(GraphServiceClient graphClient, AssignedTaskService assignedTaskService, OperationService operationService)
        {
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
            _assignedTaskService = assignedTaskService;
            _operationService = operationService;
        }


        public async Task<string> GetCurrentLoggedInUser()
        {
            User user;
            try
            {
                user = await _graphClient.Me.Request().GetAsync();
                return user.DisplayName;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "No User Found";
            }
        }

        public async Task<string> SendEmailAsSelf()
        {
            User user;

            var message = new Message
            {
                Subject = "Leon Reminder",
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = "Test From Leon"
                },
                From = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = "blee@optrust.com" // The email address of the user you want to send from
                    }
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "blee@optrust.com"
                        }
                    }
                }
            };
            try
            {
                // Send the email
                await _graphClient.Me.SendMail(message, true).Request().PostAsync();
                return "complete";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        public async Task<string> GetCurrentLoggedInUserAssignmentId()
        {
            User user;
            try
            {
                user = await _graphClient.Me.Request().GetAsync();
                return user.Id;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "No User Found";
            }
        }

        public async Task<IListItemsCollectionPage> PopulateLeonTasksFromOneList()
        {
            IListItemsCollectionPage exception = null;
            try
            {

                //Fields for references
                // Initiative_x0020_Title
                // Status
                // @odata.etag
                // field_2 = Task Name
                // TechLeadLookupId = ITSys lead



                //Book of work URL:https://graph.microsoft.com/v1.0/sites/optrust.sharepoint.com,d0793ee9-6e52-49cc-b4c0-45fe41483f0d,e15a3499-bc32-4a82-abf3-97e268ec3406/lists/b8b25230-cad7-479f-87f0-0b3a9d7d97b1/items

                //https://graph.microsoft.com/v1.0/sites/optrust.sharepoint.com,d0793ee9-6e52-49cc-b4c0-45fe41483f0d,e15a3499-bc32-4a82-abf3-97e268ec3406/lists/b8b25230-cad7-479f-87f0-0b3a9d7d97b1/items?$expand=fields&filter=fields/TechLeadLookupId eq '41'


                string siteId = "optrust.sharepoint.com,d0793ee9-6e52-49cc-b4c0-45fe41483f0d,e15a3499-bc32-4a82-abf3-97e268ec3406";
                string listId = "b8b25230-cad7-479f-87f0-0b3a9d7d97b1";
                var result = await _graphClient.Sites[$"{siteId}"].Lists[$"{listId}"].Items
                .Request()
                    .Header("Prefer", "HonorNonIndexedQueriesWarningMayFailRandomly")
                    .Expand("fields")
                    .Filter("fields/TechLeadLookupId eq '41'")
                    .GetAsync();
                var tempMatchingResult = result.CurrentPage;

                int resourceId = 1;
                //plannerTasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var leonTasks = await _assignedTaskService.GetAllActiveAssignedTasksByResource(resourceId);

                foreach (var item in tempMatchingResult)
                {

                    var additionalData = item.Fields.AdditionalData;
                    if (additionalData.ContainsKey("TechLeadLookupId"))
                    {
                        var taskTitle = additionalData["Title"].ToString();
                        var status = additionalData["Status"].ToString();
                        switch (status)
                        {
                            case "Cancelled":
                                status = "Complete";
                                break;
                            case "Not Started":
                                status = "To Do";
                                break;
                            case "In Progress":
                                status = "In Progress";
                                break;
                            case "Completed":
                                status = "Complete";
                                break;
                        }
                        var taskList = leonTasks.Where(x => x.TaskName.Contains(taskTitle)).ToList();
                        // Process or store the item as it meets the condition
                        if (additionalData["TechLeadLookupId"].ToString() == "41")
                        {
                            Console.WriteLine(additionalData);
                            if (taskList.Count() == 0)
                            {
                                Console.WriteLine("task Not Exist...creating...");
                                _operationService.CreateNewOperationTask(taskTitle, resourceId);
                                int operationId = _operationService.GetOperationIdByTaskName(taskTitle, resourceId);
                                string taskName = taskTitle;
                                bool isReminder = true;
                                bool linkToOneList = true;
                                int projectId = 0;
                                int filteredResourceId = resourceId;
                                string capacityPercentage = "10";
                                _assignedTaskService.CreateNewOperationsTask(taskName, projectId, operationId, filteredResourceId, capacityPercentage, status, isReminder, false, linkToOneList);
                            }
                            else
                            {
                                Console.WriteLine("task Exist already");
                            }
                        }
                    }
                }

                //when accessing result use:
                //var matchingResult = tempMatchingResult.Where(t => t.Fields.AdditionalData.Contains("BronsoN Lee", StringComparison.OrdinalIgnoreCase));

                return exception;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return exception;
            }

        }


        public async Task<string> PopulateLeonTasksFromPlanner()
        {
            IPlannerUserTasksCollectionPage plannerTasks;
            try
            {
                int resourceId = 1;
                plannerTasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var leonTasks = await _assignedTaskService.GetAllActiveAssignedTasksByResource(resourceId);
                //// Serialize the array to a JSON string
                //string jsonString = JsonConvert.SerializeObject(leonTasks.ToString());

                //// Compute the MD5 hash
                //using (MD5 md5 = MD5.Create())
                //{
                //    byte[] inputBytes = Encoding.ASCII.GetBytes(jsonString);
                //    byte[] hashBytes = md5.ComputeHash(inputBytes);

                //    // Convert the byte array to a hexadecimal string
                //    StringBuilder sb = new StringBuilder();
                //    for (int i = 0; i < hashBytes.Length; i++)
                //    {
                //        sb.Append(hashBytes[i].ToString("X2"));
                //    }
                //    Console.WriteLine(sb.ToString());  // Output: The MD5 hash
                //}

                foreach (var task in plannerTasks)
                {
                    string title;
                    string status = "In Progress";
                    switch (task.PercentComplete)
                    {
                        case 0:
                            status = "To Do";
                            break;
                        case 50:
                            status = "In Progress";
                            break;
                        case 100:
                            status = "Complete";
                            break;
                    }
                    if (task.Title.StartsWith("[*] - "))
                    {
                        //Title came from Leon, likely wont need to be created,
                        //ignore
                        title = task.Title.Substring(6);
                    }
                    else
                    {
                        //title didn't come from leon, likely some other team member else created it in planner outside of Leon
                        //no need to account for "[*] - " as it wont have it anyway
                        title = task.Title;
                    }

                    var taskList = leonTasks.Where(x => x.TaskName.Contains(title)).ToList();
                    if (taskList.Count() == 0)
                    {
                        Console.WriteLine("task Not Exist...creating...");
                        _operationService.CreateNewOperationTask(title, resourceId);
                        int operationId = _operationService.GetOperationIdByTaskName(title, resourceId);
                        string taskName = title;
                        bool isReminder = true;
                        bool linkToPlanner = true;
                        int projectId = 0;
                        int filteredResourceId = resourceId;
                        string capacityPercentage = "10";
                        _assignedTaskService.CreateNewOperationsTask(taskName, projectId, operationId, filteredResourceId, capacityPercentage, status, isReminder, linkToPlanner, false);
                    }
                    else
                    {
                        Console.WriteLine("task Exist already");
                    }
                }
                return "Successfully imported planner tasks.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error importing planner tasks.";

            }
        }

        public async Task<string> PopulateLeonTasksFromProject()
        {
            IPlannerUserTasksCollectionPage plannerTasks;
            try
            {
                //var leonTasks = await _assignedTaskService.GetAllActiveAssignedTasksByResource(resourceId);
                
                
                return "Successfully imported planner tasks.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error importing planner tasks.";

            }
        }

        public async Task CreatePlannerTask(string taskName, string status)
        {
            try
            {
                int percentcomplete = 0;
                switch (status)
                {
                    case "To Do":
                        percentcomplete = 0;
                        break;
                    case "In Progress":
                        percentcomplete = 50;
                        break;
                    case "Complete":
                        percentcomplete = 100;
                        break;
                }

                string assignmentId = await GetCurrentLoggedInUserAssignmentId();
                var requestBody = new PlannerTask
                {
                    // planId is found in URL.
                    // You can find bucketIds from graph explorer.
                    // https://graph.microsoft.com/v1.0/planner/plans/{plan-id}/buckets

                    //itsys planner
                    //PlanId = "TIhaF5ZOnU2_2cfJJzzuVmQAFbYg",
                    //Bucket is optional
                    //BucketId = "6e3H3uJqxECLLRvKCtZ_zmQACt0V",

                    //bronson planner
                    PlanId = "rPpVOHSt9kiV2GPuqjvV42QABxbj",
                    Title = $"[*] - {taskName}",
                    PercentComplete = percentcomplete,
                    Assignments = new PlannerAssignments
                    {
                        AdditionalData = new Dictionary<string, object>
                        {
                            {
                                assignmentId , new PlannerAssignment
                                {
                                    ODataType = "#microsoft.graph.plannerAssignment",
                                    OrderHint = " !",
                                }
                            },
                        },
                    },
                };


                // To initialize your graphClient, see https://learn.microsoft.com/en-us/graph/sdks/create-client?from=snippets&tabs=csharp
                var result = await _graphClient.Planner.Tasks.Request().AddAsync(requestBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public async Task UpdatePlannerTaskPreview(string taskName)
        {
            try
            {
                var tasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var taskNameToSearch = taskName; // Replace with the actual task name
                var matchingTask = tasks.FirstOrDefault(t => t.Title.Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));
                string taskId = "";
                if (matchingTask != null)
                {
                    taskId = matchingTask.Id;
                    // Now you have the task ID!
                }
                //Update in Planner
                //
                var taskToUpdate = await _graphClient.Planner.Tasks[taskId].Details.Request().GetAsync();
                string etag = taskToUpdate.GetEtag();
                if (taskToUpdate != null)
                {
                    var requestBody = new PlannerTaskDetails
                    {
                        PreviewType = Microsoft.Graph.PlannerPreviewType.Description
                    };

                    await _graphClient.Planner.Tasks[taskId].Details.Request().Header("If-Match", etag).UpdateAsync(requestBody);

                }
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }

        public async Task UpdateTaskStatus(string taskName, string status)
        {
            int percentcomplete = 0;
            switch (status)
            {
                case "To Do":
                    percentcomplete = 0;
                    break;
                case "In Progress":
                    percentcomplete = 50;
                    break;
                case "Complete":
                    percentcomplete = 100;
                    break;
            }
            //Get Task in Planner list
            try
            {
                var tasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var taskNameToSearch = taskName; // Replace with the actual task name
                var matchingTask = tasks.FirstOrDefault(t => t.Title.Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));
                string taskId = "";
                if (matchingTask != null)
                {
                    taskId = matchingTask.Id;
                    // Now you have the task ID!
                }
                //Update in Planner
                //
                var taskToUpdate = await _graphClient.Planner.Tasks[taskId].Request().GetAsync();
                if (taskToUpdate != null)
                {
                    var requestBody = new PlannerTask
                    {
                        PercentComplete = percentcomplete
                    };

                    await _graphClient.Planner.Tasks[taskId].Request().Header("If-Match", taskToUpdate.GetEtag()).UpdateAsync(requestBody);

                }
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }

        public async Task UpdateOneListTaskStatus(string taskName, string status)
        {
            int percentcomplete = 0;
            switch (status)
            {
                case "To Do":
                    percentcomplete = 0;
                    break;
                case "In Progress":
                    percentcomplete = 50;
                    break;
                case "Complete":
                    percentcomplete = 100;
                    break;
            }
            //Get Task in Planner list
            try
            {
                var tasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var taskNameToSearch = taskName; // Replace with the actual task name
                var matchingTask = tasks.FirstOrDefault(t => t.Title.Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));
                string taskId = "";
                if (matchingTask != null)
                {
                    taskId = matchingTask.Id;
                    // Now you have the task ID!
                }
                //Update in Planner
                //
                var taskToUpdate = await _graphClient.Planner.Tasks[taskId].Request().GetAsync();
                if (taskToUpdate != null)
                {
                    var requestBody = new PlannerTask
                    {
                        PercentComplete = percentcomplete
                    };

                    await _graphClient.Planner.Tasks[taskId].Request().Header("If-Match", taskToUpdate.GetEtag()).UpdateAsync(requestBody);

                }
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }

        public async Task UpdatePlannerTaskNotes(string taskName, string note, string dateEntered)
        {
            //Get Task in Planner list
            try
            {
                var tasks = await _graphClient.Me.Planner.Tasks.Request().GetAsync();
                var taskNameToSearch = taskName; // Replace with the actual task name
                var matchingTask = tasks.FirstOrDefault(t => t.Title.Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));
                string taskId = "";
                if (matchingTask != null)
                {
                    taskId = matchingTask.Id;
                    // Now you have the task ID!
                }
                //Update in Planner 
                //
                var taskToUpdate = await _graphClient.Planner.Tasks[taskId].Details.Request().GetAsync();
                if (taskToUpdate != null)
                {
                    var requestBody = new PlannerTaskDetails
                    {
                        Description = $"{dateEntered}: {note}\n\n{taskToUpdate.Description}"
                    };

                    await _graphClient.Planner.Tasks[taskId].Details.Request().Header("If-Match", taskToUpdate.GetEtag()).UpdateAsync(requestBody);

                }
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }
        public async Task UpdateOneListTaskNotes(string taskName, string note, string dateEntered)
        {
            IListItemsCollectionPage exception = null;
            //Get Task in Planner list
            try
            {

                string siteId = "optrust.sharepoint.com,d0793ee9-6e52-49cc-b4c0-45fe41483f0d,e15a3499-bc32-4a82-abf3-97e268ec3406";
                string listId = "b8b25230-cad7-479f-87f0-0b3a9d7d97b1";


                var result = await _graphClient.Sites[$"{siteId}"].Lists[$"{listId}"].Items
                .Request()
                    .Header("Prefer", "HonorNonIndexedQueriesWarningMayFailRandomly")
                    .Expand("fields")
                    .Filter("fields/TechLeadLookupId eq '41'")
                    .GetAsync();
                var tempMatchingResult = result.CurrentPage;

                foreach (var item in tempMatchingResult)
                {
                    var id = item.Id.ToString();
                    var additionalData = item.Fields.AdditionalData;
                    if (additionalData["Title"].ToString() == taskName)
                    {
                        var etag = additionalData["@odata.etag"].ToString();
                        //var id = additionalData["Id"].ToString();
                        var status = additionalData["Status"].ToString();
                        var lastComment = additionalData["LastComment"].ToString();
                        //var matchingTask = tempMatchingResult.FirstOrDefault(t => t..Contains(taskNameToSearch, StringComparison.OrdinalIgnoreCase));
                        var requestBody = new FieldValueSet
                        {
                            AdditionalData = new Dictionary<string, object>
                            {
                                {
                                    "LastComment" , note
                                },
                            },
                        };
                        //var itemToUpdate = await _graphClient.Sites[$"{siteId}"].Lists[$"{listId}"].Items[$"{id}"].Request().UpdateAsync(requestBody);

                        //Not what we need, we dont need to update/overwrite comment, we want to add new comment.
                        //WORKING UPDATE TO NEW COMMENT - NOT ADD COMMENT
                        //var itemToUpdate = await _graphClient.Sites[$"{siteId}"].Lists[$"{listId}"].Items[$"{id}"].Fields.Request().UpdateAsync(requestBody);

                        //Console.WriteLine(itemToUpdate);
                    }
                }


                //Update in OneList 
                //
                //var taskToUpdate = await _graphClient.Sites. .Tasks[taskId].Details.Request().GetAsync();
                //if (taskToUpdate != null)
                //{
                //    var requestBody = new PlannerTaskDetails
                //    {
                //        Description = $"{dateEntered}: {note}\n\n{taskToUpdate.Description}"
                //    };

                //    await _graphClient.Planner.Tasks[taskId].Details.Request().Header("If-Match", taskToUpdate.GetEtag()).UpdateAsync(requestBody);

                //}
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }
    }
}
