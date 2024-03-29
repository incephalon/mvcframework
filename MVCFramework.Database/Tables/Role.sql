﻿CREATE TABLE [dbo].[Role] (
    [RoleUID]   UNIQUEIDENTIFIER CONSTRAINT [DF_Role_RoleUID] DEFAULT (newid()) NOT NULL,
    [Name]      NVARCHAR (50)    NOT NULL,
    [TenantUID] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleUID] ASC),
    CONSTRAINT [FK_Role_Tenant] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Tenant] ([TenantUID])
);



