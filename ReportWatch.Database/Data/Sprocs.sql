USE [ReportWatch]
GO
/****** Object:  StoredProcedure [dbo].[SetDayPriceHigh]    Script Date: 10/20/2011 22:13:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetDayPriceHigh]
@DateReport DateTime 
AS
BEGIN
	UPDATE ReportSet
	SET DayPriceHigh =
		(SELECT IsNull(MAX(DayPriceHigh),0)
		FROM DayPriceSet
		WHERE DayPriceSet.SymbolName = ReportSet.SymbolName
		AND DayPriceSet.DayPriceDate = 
			(SELECT MIN(DayPriceDate) 
			FROM DayPriceSet
			WHERE DayPriceSet.SymbolName = ReportSet.SymbolName
			AND DayPriceDate >= ReportDate))
	WHERE ReportSet.SymbolName IN
		(SELECT SymbolName
		FROM SymbolSet
		WHERE ReportDate = @DateReport)
END
GO
/****** Object:  StoredProcedure [dbo].[CalculateDayDifference]    Script Date: 10/20/2011 22:13:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CalculateDayDifference]
@symbolName varchar(10)
AS
DECLARE @indexName varchar(10)
BEGIN

	SET NOCOUNT ON;

	SELECT @indexName = IndexSymbolName FROM SymbolSet WHERE SymbolName = @symbolName

	DELETE FROM [ReportWatch].[dbo].[DayDifferenceSet] WHERE SymbolName = @symbolName

	INSERT INTO [ReportWatch].[dbo].[DayDifferenceSet]
	SELECT NewID()
	   ,@symbolName
	   ,DayChangeSet.DayChangeDate
	   ,DayChangeSet.DayChangeOpen-IndexChangeSet.DayChangeOpen
	   ,DayChangeSet.DayChangeHigh-IndexChangeSet.DayChangeHigh
	   ,DayChangeSet.DayChangeLow-IndexChangeSet.DayChangeLow
	   ,DayChangeSet.DayChangeClose-IndexChangeSet.DayChangeClose
	   ,DayChangeSet.DayChangeAdjustedClose-IndexChangeSet.DayChangeAdjustedClose
	   ,DayChangeSet.DayChangeVolume-IndexChangeSet.DayChangeVolume
		FROM DayChangeSet
		INNER JOIN (SELECT * FROM DayChangeSet WHERE SymbolName = @IndexName) AS IndexChangeSet ON 
		DayChangeSet.DayChangeDate = IndexChangeSet.DayChangeDate
		WHERE DayChangeSet.SymbolName = @symbolName
		ORDER BY DayChangeSet.DayChangeDate
END
GO
/****** Object:  StoredProcedure [dbo].[CalculateDayChange]    Script Date: 10/20/2011 22:13:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CalculateDayChange]

@symbolName varchar(10)

AS
DECLARE @dayPriceDatePrevious [datetime2](7)
DECLARE @dayPriceOpenPrevious [decimal](10, 4)
DECLARE @dayPriceHighPrevious [decimal](10, 4)
DECLARE @dayPriceLowPrevious [decimal](10, 4)
DECLARE @dayPriceClosePrevious [decimal](10, 4)
DECLARE @dayPriceAdjustedClosePrevious [decimal](10, 4)
DECLARE @dayPriceVolumePrevious [bigint]

DECLARE @dayPriceDate [datetime2](7)
DECLARE @dayPriceOpen [decimal](10, 4)
DECLARE @dayPriceHigh [decimal](10, 4)
DECLARE @dayPriceLow [decimal](10, 4)
DECLARE @dayPriceClose [decimal](10, 4)
DECLARE @dayPriceAdjustedClose [decimal](10, 4)
DECLARE @dayPriceVolume [bigint]

BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM DayChangeSet WHERE SymbolName = @symbolName

DECLARE daychange_cursor CURSOR FAST_FORWARD FOR 
	SELECT 	DayPriceDate,DayPriceOpen,DayPriceHigh,DayPriceLow,DayPriceClose,DayPriceAdjustedClose,DayPriceVolume
	FROM DayPriceSet
	WHERE DayPriceSet.SymbolName = @symbolName
	ORDER BY DayPriceSet.DayPriceDate

OPEN daychange_cursor

FETCH NEXT FROM daychange_cursor INTO @dayPriceDatePrevious,@dayPriceOpenPrevious,@dayPriceHighPrevious,@dayPriceLowPrevious,@dayPriceClosePrevious,@dayPriceAdjustedClosePrevious,@dayPriceVolumePrevious
FETCH NEXT FROM daychange_cursor INTO @dayPriceDate,@dayPriceOpen,@dayPriceHigh,@dayPriceLow,@dayPriceClose,@dayPriceAdjustedClose,@dayPriceVolume

WHILE @@FETCH_STATUS = 0
BEGIN

	INSERT INTO DayChangeSet
			   ([DayChangeId]
			   ,[SymbolName]
			   ,[DayChangeDate]
			   ,[DayChangeOpen]
			   ,[DayChangeHigh]
			   ,[DayChangeLow]
			   ,[DayChangeClose]
			   ,[DayChangeAdjustedClose]
			   ,[DayChangeVolume])
		 VALUES
			   (NEWID()
			   ,@symbolName
			   ,@dayPriceDate
			   ,(@dayPriceOpen-@dayPriceOpenPrevious)/@dayPriceOpenPrevious
			   ,(@dayPriceHigh-@dayPriceHighPrevious)/@dayPriceHighPrevious
			   ,(@dayPriceLow-@dayPriceLowPrevious)/@dayPriceLowPrevious
			   ,(@dayPriceClose-@dayPriceClosePrevious)/@dayPriceClosePrevious
			   ,(@dayPriceAdjustedClose-@dayPriceAdjustedClosePrevious)/@dayPriceAdjustedClosePrevious
			   ,(@dayPriceVolume-@dayPriceVolumePrevious)/@dayPriceVolumePrevious)

SET @dayPriceDatePrevious = @dayPriceDate
SET @dayPriceOpenPrevious = @dayPriceOpen
SET @dayPriceHighPrevious = @dayPriceHigh
SET @dayPriceLowPrevious = @dayPriceLow
SET @dayPriceClosePrevious = @dayPriceClose
SET @dayPriceAdjustedClosePrevious = @dayPriceAdjustedClose
SET @dayPriceVolumePrevious = @dayPriceVolume

FETCH NEXT FROM daychange_cursor INTO @dayPriceDate,@dayPriceOpen,@dayPriceHigh,@dayPriceLow,@dayPriceClose,@dayPriceAdjustedClose,@dayPriceVolume

END

CLOSE daychange_cursor
DEALLOCATE daychange_cursor 

END
GO
