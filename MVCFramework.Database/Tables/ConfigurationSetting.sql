CREATE TABLE [dbo].[ConfigurationSetting] (
    [ConfigurationSettingID] INT            NOT NULL,
    [ConfigurationSectionID] INT            NOT NULL,
    [Name]                   NVARCHAR (50)  NOT NULL,
    [DefaultValue]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ConfigurationSetting] PRIMARY KEY CLUSTERED ([ConfigurationSettingID] ASC)
);

