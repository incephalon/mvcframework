CREATE TABLE [dbo].[Configuration] (
    [ConfigurationID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([ConfigurationID] ASC)
);

