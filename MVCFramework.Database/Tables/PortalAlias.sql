CREATE TABLE [dbo].[PortalAlias] (
    [PortalAliasID] INT              IDENTITY (1, 1) NOT NULL,
    [TenantUID]     UNIQUEIDENTIFIER NOT NULL,
    [Url]           VARCHAR (256)    NOT NULL,
    CONSTRAINT [PK_PortalAlias] PRIMARY KEY CLUSTERED ([PortalAliasID] ASC),
    CONSTRAINT [FK_PortalAlias_Portal] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Portal] ([TenantUID])
);

