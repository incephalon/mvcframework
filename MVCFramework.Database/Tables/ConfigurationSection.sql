CREATE TABLE [dbo].[ConfigurationSection] (
    [ConfigurationSectionID] INT           IDENTITY (1, 1) NOT NULL,
    [ConfigurationID]        INT           NOT NULL,
    [Name]                   NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ConfigurationSection] PRIMARY KEY CLUSTERED ([ConfigurationSectionID] ASC)
);

