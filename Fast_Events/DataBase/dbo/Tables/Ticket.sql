CREATE TABLE [dbo].[Ticket] (
    [id]           BIGINT       IDENTITY (1, 1) NOT NULL,
    [event_id]     BIGINT       NOT NULL,
    [qrc_filename] VARCHAR (50) NOT NULL,
    [owner_uuid]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Tickets_Event] FOREIGN KEY ([event_id]) REFERENCES [dbo].[Event] ([id])
);

