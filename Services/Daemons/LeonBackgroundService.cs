////using Leon.Models;
////using Leon.Services;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Graph;

////public class LeonBackgroundService : BackgroundService
////{
////    //Initialize IAMContext and IAMService to be injected
////    Leon_Context leon_context;
////    //private readonly AssignedTaskService _assignedTask;


////    //ActiveDirectoryService Constructor
////    public LeonBackgroundService(Leon_Context leonctx)
////    {
////        //Inject anotherService into LeonWorkerService contructor

////        //Inject IAMContext into LeonWorkerService contructor
////        leon_context = leonctx;
////        leon_context.Database.SetCommandTimeout(120);

////        //_assignedTask = assignedTask;
////    }
////    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
////    {
////        while (!stoppingToken.IsCancellationRequested)
////        {
////            // Your code here.
////            Console.WriteLine(DateTime.Now);
////            if(DateTime.Now.Hour == 9)
////            {
////                Console.WriteLine("Running Code...");
////            }

////            //
////            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

////        }
////    }


////}
//using Leon.Models;
//using Leon.Services;

//public class LeonBackgroundService
//{
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly ILogger<LeonBackgroundService> _logger;
//    private bool _hasRunToday = false;

//    public LeonBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LeonBackgroundService> logger)
//    {
//        _scopeFactory = scopeFactory;
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        await Task.Delay(TimeSpan.FromSeconds(25), stoppingToken); // Runs every second
//        while (!stoppingToken.IsCancellationRequested)
//        {


//            using (var scope = _scopeFactory.CreateScope())
//            {
//                var leon_Context = scope.ServiceProvider.GetRequiredService<Leon_Context>();
//                var _assignedTaskService = scope.ServiceProvider.GetRequiredService<AssignedTaskService>();
//                var _graphApiService = scope.ServiceProvider.GetRequiredService<GraphApiService>();

//                var now = DateTime.Now;
//                if (now.Hour == 22 && now.Minute == 20 && now.Second == 0 && !_hasRunToday)
//                {
//                    _logger.LogInformation("Running Code at {Time}", now);
//                    // Your code here.
//                    var reminders = await _assignedTaskService.GetTodaysTaskReminders();
//                    _graphApiService.SendEmailAsSelf();
//                    // Log the successful completion of your code.
//                    _logger.LogInformation("Successfully ran code at {Time}", DateTime.Now);
                    
//                    _hasRunToday = true; // Set the flag to true after the code runs.

//                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Wait for a minute
//                }
//                else if (now.Hour == 18 && now.Minute == 44)
//                {
//                    _logger.LogInformation("Code Ran Today Already", DateTime.Now);

//                    _hasRunToday = false; // Reset the flag at 8:31 AM.
//                }
//                else
//                {
//                    _logger.LogInformation("Here", DateTime.Now);

//                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Runs every second
//                }
//            }
//        }
//    }
//}

