CREATE TABLE [dbo].[Navigation] (
    [NavigationID] INT              IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)     NOT NULL,
    [TenantUID]    UNIQUEIDENTIFIER NOT NULL,
    [RoleUID]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Navigation] PRIMARY KEY CLUSTERED ([NavigationID] ASC),
    CONSTRAINT [FK_Navigation_Role] FOREIGN KEY ([RoleUID]) REFERENCES [dbo].[Role] ([RoleUID]),
    CONSTRAINT [FK_Navigation_Tenant] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Tenant] ([TenantUID]),
    CONSTRAINT [UQ_Navigation] UNIQUE NONCLUSTERED ([TenantUID] ASC, [RoleUID] ASC)
);



