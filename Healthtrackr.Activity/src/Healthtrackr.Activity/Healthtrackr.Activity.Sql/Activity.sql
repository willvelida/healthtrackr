CREATE TABLE [dbo].[ActivitySummary]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [CaloriesBurned] INT NULL,
  [ActivityCalories] INT NULL,
  [Distance] DECIMAL(10,2) NULL,
  [MinutesFairlyActive] INT NULL,
  [Floors] INT NULL,
  [MinutesLightlyActive] INT NULL,
  [MinutesSedentary] INT NULL,
  [Steps] INT NULL,
  [MinutesVeryActive] INT NULL,
  [Date] DATE NOT NULL,
)
GO;

CREATE TABLE [dbo].[Activity]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [ActivityName] VARCHAR(50) NULL,
  [Calories] INT NULL,
  [Duration] INT NULL,
  [Date] DATE NOT NULL,
  [Time] TIME NULL,
  [Steps] INT NULL,
)
GO;