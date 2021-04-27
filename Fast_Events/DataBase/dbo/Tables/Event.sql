CREATE TABLE [dbo].[Event] (
    [id]               BIGINT       NOT NULL,
    [name]             VARCHAR (50) NOT NULL,
    [organizer]        VARCHAR (50) NOT NULL,
    [start_date]       DATETIME     NOT NULL,
    [end_date]         DATETIME     NOT NULL,
    [capacity]         INT          NOT NULL,
    [location]         VARCHAR (50) NULL,
    [description]      VARCHAR (50) NULL,
    [picture_filename] VARCHAR (50) NULL,
    [owner_uuid]       VARCHAR (32) NOT NULL,
    [category]         VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([id] ASC)
);

