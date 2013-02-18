CREATE TABLE [dbo].[NavigationItem] (
    [NavigationItemID] INT            IDENTITY (1, 1) NOT NULL,
    [NavigationID]     INT            NOT NULL,
    [ParentItemID]     INT            NULL,
    [ShowInMenu]       BIT            CONSTRAINT [DF_NavigationItem_ShowInMenu] DEFAULT ((1)) NOT NULL,
    [Order]            SMALLINT       CONSTRAINT [DF_NavigationItem_Order] DEFAULT ((0)) NOT NULL,
    [Text]             NVARCHAR (128) NOT NULL,
    [Url]              NVARCHAR (256) NULL,
    [Icon]             VARCHAR (128)  NULL,
    CONSTRAINT [PK_NavigationItem] PRIMARY KEY CLUSTERED ([NavigationItemID] ASC),
    CONSTRAINT [FK_NavigationItem_Navigation] FOREIGN KEY ([NavigationID]) REFERENCES [dbo].[Navigation] ([NavigationID]),
    CONSTRAINT [FK_NavigationItem_NavigationItem] FOREIGN KEY ([ParentItemID]) REFERENCES [dbo].[NavigationItem] ([NavigationItemID])
);



