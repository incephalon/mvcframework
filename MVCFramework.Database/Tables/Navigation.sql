CREATE TABLE [dbo].[Navigation] (
    [NavigationID] INT              NOT NULL,
    [IsDefault]    BIT              CONSTRAINT [DF_Navigation_IsDefault] DEFAULT ((0)) NOT NULL,
    [Name]         VARCHAR (50)     NOT NULL,
    [TenantUID]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Navigation] PRIMARY KEY CLUSTERED ([NavigationID] ASC)
);

