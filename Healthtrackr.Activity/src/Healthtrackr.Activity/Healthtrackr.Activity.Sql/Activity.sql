CREATE TABLE [dbo].[ActivityHeartRateZones]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [Name] VARCHAR(50) NOT NULL,
  [Minutes] INT NOT NULL,
  [MaxHR] INT NOT NULL,
  [MinHR] INT NOT NULL,
  [CaloriesOut] DECIMAL(10,5) NOT NULL,
  [Date] DATE NOT NULL,
)
GO;

CREATE TABLE [dbo].[ActivityDistances]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [ActivityType] VARCHAR(50) NOT NULL,
  [Distance] DECIMAL(10,5) NOT NULL,
  [Date] DATE NOT NULL,
)
GO;

CREATE TABLE [dbo].[ActivitySummary]
(
  [Id] INT IDENTITY NOT NULL PRIMARY KEY,
  [CaloriesEstimationMu] INT NOT NULL,
  [CaloriesBMR] INT NOT NULL,
  [CaloriesOut] INT NOT NULL,
  [ActivityCalories] INT NOT NULL,
  [Elevation] DECIMAL(10,5) NOT NULL,
  [FairlyActiveMinutes] INT NOT NULL,
  [Floors] INT NOT NULL,
  [LightlyActiveMinutes] INT NOT NULL,
  [MarginalCalories] INT NOT NULL,
  [RestingHeartRate] INT NOT NULL,
  [SedentaryMinutes] INT NOT NULL,
  [Steps] INT NOT NULL,
  [VeryActiveMinutes] INT NOT NULL,
  [Date] DATE NOT NULL,
  [ActivityDistancesId] INT FOREIGN KEY REFERENCES [dbo].[ActivityDistances] ([Id]),
  [ActivityHeartRateZonesId] INT FOREIGN KEY REFERENCES [dbo].[ActivityHeartRateZones] ([Id]),
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
  [ActivitySummaryId] INT FOREIGN KEY REFERENCES [dbo].[ActivitySummary] ([Id]),
)
GO;