CREATE TABLE [dbo].[ActivitySummary]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [CaloriesBurned] INT NOT NULL,
  [ActivityCalories] INT NOT NULL,
  [Distance] DECIMAL(10,2) NOT NULL,
  [MinutesFairlyActive] INT NOT NULL,
  [Floors] INT NOT NULL,
  [MinutesLightlyActive] INT NOT NULL,
  [MinutesSedentary] INT NOT NULL,
  [Steps] INT NOT NULL,
  [MinutesVeryActive] INT NOT NULL,
  [Date] DATE NOT NULL,
)
GO;

CREATE TABLE [dbo].[Activity]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [ActivityName] VARCHAR(50) NOT NULL,
  [Calories] INT NOT NULL,
  [Duration] INT NOT NULL,
  [Date] DATE NOT NULL,
  [Time] DATETIME NOT NULL,
  [Steps] INT NOT NULL,
)
GO;