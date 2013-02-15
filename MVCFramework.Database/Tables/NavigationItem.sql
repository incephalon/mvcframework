CREATE TABLE [dbo].[NavigationItem] (
    [NavigationItemID]       INT            NOT NULL,
    [NavigationID]           INT            NOT NULL,
    [ParentNavigationItemID] INT            NULL,
    [ShowInMenu]             BIT            CONSTRAINT [DF_NavigationItem_ShowInMenu] DEFAULT ((1)) NOT NULL,
    [Order]                  SMALLINT       CONSTRAINT [DF_NavigationItem_Order] DEFAULT ((0)) NOT NULL,
    [Text]                   NVARCHAR (128) NOT NULL,
    [Url]                    NVARCHAR (256) NULL,
    [Icon]                   VARCHAR (256)  NULL,
    CONSTRAINT [PK_NavigationItem] PRIMARY KEY CLUSTERED ([NavigationItemID] ASC)
);

