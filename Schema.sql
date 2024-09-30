--USE [Leon_Dev]
--Drop Table AssignedTasks
--Drop Table AssignedTaskNotes
--Drop Table Projects
--Drop Table Operations
--Drop Table DashboardStatus
--Drop Table Resources


--RESOURCES TABLE
USE [Leon_Dev]
GO

/****** Object:  Table [dbo].[Resources]    Script Date: 2024-03-21 3:19:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Resources](
	[ResourceID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [varchar](255) NULL,
	[ShowDetails] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO





--PROJECTS TABLS
USE [Leon_Dev]
GO

/****** Object:  Table [dbo].[Projects]    Script Date: 2024-03-21 3:19:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Projects](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [varchar](255) NULL,
	[BusinessOwner] [varchar](255) NULL,
	[InitiativeID] [varchar](255) NULL,
	[ResourceID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Projects]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO





/****** Object:  View [dbo].[ResourceProjectTasks]    Script Date: 2024-03-09 3:00:02 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE VIEW [dbo].[ResourceProjectTasks] AS
--SELECT 
--    r.ResourceName,
--    p.ProjectName,
--    t.TaskName,
--    r.Capacity
--FROM 
--    Resources r
--JOIN 
--    Allocations a ON r.ResourceID = a.ResourceID
--JOIN 
--    Tasks t ON a.TaskID = t.TaskID
--JOIN 
--    Projects p ON t.ProjectID = p.ProjectID;
--GO




USE [Leon_Dev]
GO

/****** Object:  Table [dbo].[AssignedTasks]    Script Date: 2024-03-21 3:19:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AssignedTasks](
	[TaskID] [int] IDENTITY(1,1) NOT NULL,
	[TaskName] [varchar](255) NULL,
	[ProjectID] [int] NULL,
	[OperationID] [int] NULL,
	[ResourceID] [int] NULL,
	[CapacityPercentage] [int] NULL,
	[Status] [nvarchar](max) NULL,
	[ShowNewTaskButton] [bit] NULL,
	[IsReminder] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AssignedTasks] ADD  CONSTRAINT [DF_AssignedTasks_IsReminder]  DEFAULT ((0)) FOR [IsReminder]
GO

ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([OperationID])
REFERENCES [dbo].[Operations] ([OperationID])
GO

ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO

ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO







/****** Object:  View [dbo].[ResourceCapacityView]    Script Date: 2024-03-09 3:00:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ResourceCapacityView] AS
SELECT
    R.ResourceID,
    R.EmployeeName,
    SUM(AT.CapacityPercentage) AS TotalCapacity
FROM
    Resources R
LEFT JOIN
    AssignedTasks AT ON R.ResourceID = AT.ResourceID
GROUP BY
    R.ResourceID, R.EmployeeName;
GO
/****** Object:  Table [dbo].[DashboardStatus]    Script Date: 2024-03-09 3:00:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DashboardStatus](
	[DashboardId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[DashboardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 2024-03-09 3:00:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[OperationID] [int] IDENTITY(1,1) NOT NULL,
	[OperationName] [varchar](255) NULL,
	[BusinessOwner] [varchar](255) NULL,
	[ResourceID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([OperationID])
REFERENCES [dbo].[Operations] ([OperationID])
GO
ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
--ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD  CONSTRAINT [FK_DashboardStatus_Task] FOREIGN KEY([TaskID])
--REFERENCES [dbo].[DashboardStatus] ([DashboardId])
--GO
--ALTER TABLE [dbo].[AssignedTasks] CHECK CONSTRAINT [FK_DashboardStatus_Task]
--GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO


--Notes for AssignedTasks
CREATE TABLE [dbo].[AssignedTaskNotes](
    [NoteID] [int] IDENTITY(1,1) NOT NULL,
    [TaskID] [int] NOT NULL,
    [ResourceID] [int] NOT NULL,
    [NoteText] [nvarchar](max) NULL,
    [CreationDate] [datetime] NOT NULL,
    CONSTRAINT [PK_AssignedTaskNotes] PRIMARY KEY CLUSTERED 
    (
        [NoteID] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
    CONSTRAINT [FK_AssignedTaskNotes_To_AssignedTasks] FOREIGN KEY ([TaskID])
        REFERENCES [dbo].[AssignedTasks] ([TaskID]),
    CONSTRAINT [FK_AssignedTaskNotes_To_Resources] FOREIGN KEY ([ResourceID])
        REFERENCES [dbo].[Resources] ([ResourceID])
) ON [PRIMARY]
GO

--End of Schema Create
--
--
--
--
--
--
--INSERTDATA FOR TESTING


--TableData for Testing
-- Inserting data into Resources
INSERT INTO Resources (EmployeeName)
VALUES
( 'Alice Johnson'),
( 'Bob Smith'),
( 'Charlie Brown'),
( 'Diana Prince'),
( 'Ethan Hunt'),
( 'Fiona Glenanne'),
( 'George Jetson'),
( 'Helen Parr'),
( 'Ian Malcolm'),
( 'Jane Porter');

INSERT INTO DashboardStatus(Name)
VALUES
( 'To Do'),
( 'In Progress'),
( 'On Hold'),
( 'Complete');

-- Inserting data into Projects
INSERT INTO Projects (ProjectName, BusinessOwner, InitiativeID, ResourceID)
VALUES
('Project Alpha', 'Finance', 'INV-101-2024', 1),
('Project Beta', 'People Team', 'INV-102-2024', 2),
('Project Gamma', 'CMG', 'INV-134-2024', 1),
('Project Delta', 'PM&A', 'INV-153-2024', 3),
('Project Epsilon', 'Investment Risk', 'INV-231-2024', 4);

-- Inserting data into Operations
INSERT INTO Operations (OperationName, BusinessOwner, ResourceID)
VALUES
('Troubleshooting Docusign Reports', 'MExPO', 5),
('Operation Yankee', 'Investment Finance', 6),
('Workday Integration Failures', 'People Team', 7),
('File360 Kodak Scanner Issues', 'MExPO', 8),
('FCM processing Errors', 'OMO', 9);

-- Inserting data into AssignedTasks
INSERT INTO AssignedTasks (TaskName, ProjectID, OperationID, ResourceID, CapacityPercentage, Status)
VALUES
('Design Database', 1, NULL, 1, 50.00, 'To Do'),
('Develop Frontend', 1, NULL, 2, 30.00, 'To Do'),
('Setup Server', NULL, 1, 5, 40.00, 'To Do'),
('Implement Security', 2, NULL, 2, 70.00, 'To Do'),
('Quality Assurance', 3, NULL, 1, 50.00, 'To Do'),
('User Training', NULL, 2, 6, 60.00, 'To Do'),
('Deploy Application', 4, NULL, 3, 80.00, 'To Do'),
('Market Research', NULL, 3, 7, 20.00, 'To Do'),
('Data Analysis', 5, NULL, 4, 100.00, 'In Progress'),
('Customer Support', NULL, 4, 8, 50.00, 'Complete');

-- Inserting data into AssignedTaskNotes
INSERT INTO [dbo].[AssignedTaskNotes] ([TaskID], [ResourceID], [NoteText], [CreationDate])
VALUES
(2, 2, 'Note for Task 1 by Resource 2', GETDATE()),
(2, 2, 'Note for Task 2 by Resource 2', GETDATE()),
(3, 3, 'Note for Task 3 by Resource 3', GETDATE()),
(4, 4, 'Note for Task 4 by Resource 4', GETDATE()),
(5, 5, 'Note for Task 5 by Resource 5', GETDATE()),
(6, 6, 'Note for Task 6 by Resource 6', GETDATE()),
(7, 7, 'Note for Task 7 by Resource 7', GETDATE()),
(8, 8, 'Note for Task 8 by Resource 8', GETDATE()),
(9, 9, 'Note for Task 9 by Resource 9', GETDATE()),
(10, 10, 'Note for Task 10 by Resource 10', GETDATE());














--LatestSchema Build
USE [Leon_Dev]
GO
/****** Object:  Table [dbo].[AssignedTaskNotes]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedTaskNotes](
	[NoteID] [int] IDENTITY(1,1) NOT NULL,
	[TaskID] [int] NOT NULL,
	[ResourceID] [int] NOT NULL,
	[NoteText] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AssignedTaskNotes] PRIMARY KEY CLUSTERED 
(
	[NoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedTasks]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedTasks](
	[TaskID] [int] IDENTITY(1,1) NOT NULL,
	[TaskName] [varchar](255) NULL,
	[ProjectID] [int] NULL,
	[OperationID] [int] NULL,
	[ResourceID] [int] NULL,
	[CapacityPercentage] [int] NULL,
	[Status] [nvarchar](max) NULL,
	[ShowNewTaskButton] [bit] NULL,
	[IsReminder] [bit] NULL,
	[DateEntered] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DashboardStatus]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DashboardStatus](
	[DashboardId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[DashboardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[OperationID] [int] IDENTITY(1,1) NOT NULL,
	[OperationName] [varchar](255) NULL,
	[BusinessOwner] [varchar](255) NULL,
	[ResourceID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Projects](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [varchar](255) NULL,
	[BusinessOwner] [varchar](255) NULL,
	[InitiativeID] [varchar](255) NULL,
	[ResourceID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resources]    Script Date: 2024-03-25 4:54:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resources](
	[ResourceID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [varchar](255) NULL,
	[ShowDetails] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssignedTaskNotes] ADD  CONSTRAINT [DF_AssignedTaskNotes_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[AssignedTasks] ADD  CONSTRAINT [DF_AssignedTasks_IsReminder]  DEFAULT ((0)) FOR [IsReminder]
GO
ALTER TABLE [dbo].[AssignedTasks] ADD  CONSTRAINT [DF_AssignedTasks_DateEntered]  DEFAULT (getdate()) FOR [DateEntered]
GO
ALTER TABLE [dbo].[AssignedTaskNotes]  WITH CHECK ADD  CONSTRAINT [FK_AssignedTaskNotes_To_AssignedTasks] FOREIGN KEY([TaskID])
REFERENCES [dbo].[AssignedTasks] ([TaskID])
GO
ALTER TABLE [dbo].[AssignedTaskNotes] CHECK CONSTRAINT [FK_AssignedTaskNotes_To_AssignedTasks]
GO
ALTER TABLE [dbo].[AssignedTaskNotes]  WITH CHECK ADD  CONSTRAINT [FK_AssignedTaskNotes_To_Resources] FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[AssignedTaskNotes] CHECK CONSTRAINT [FK_AssignedTaskNotes_To_Resources]
GO
ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[AssignedTasks]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[Operations]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD FOREIGN KEY([ResourceID])
REFERENCES [dbo].[Resources] ([ResourceID])
GO
