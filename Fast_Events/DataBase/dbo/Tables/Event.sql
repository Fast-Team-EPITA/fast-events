CREATE TABLE [dbo].[Event] (
    [id]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [name]             VARCHAR (50)  NOT NULL,
    [organizer]        VARCHAR (50)  NOT NULL,
    [start_date]       DATETIME      NOT NULL,
    [end_date]         DATETIME      NOT NULL,
    [capacity]         INT           NOT NULL,
    [location]         VARCHAR (200)  NOT NULL,
    [description]      VARCHAR (500) NOT NULL,
    [picture_filename] VARCHAR (50)  NOT NULL,
    [owner_uuid]       VARCHAR (50)  NOT NULL,
    [category]         VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([id] ASC)
);



