PackageManagerConsole Commands:
If DB Changes are made, you need to run this command in PackageManagerConsole in VisualStudio to pull in new DB changes into the Models folder:
Scaffold-DbContext "Name=Leon_Dev" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context Leon_Context -Force

.NET Core Blazor




Overallview*************************
select DISTINCT(r.ResourceId), r.EmployeeName, (SELECT SUM(CapacityPercentage)  from AssignedTasks Where ResourceID = 2) as Capacity from Resources r
inner join AssignedTasks a on r.ResourceID = a.ResourceID
Where r.resourceId = 2