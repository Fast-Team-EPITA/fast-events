CREATE TABLE [dbo].[Stat] (
    [id]       BIGINT   NOT NULL,
    [event_id] BIGINT   NOT NULL,
    [date]     DATETIME NOT NULL,
    CONSTRAINT [PK_Stat] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Stat_Event] FOREIGN KEY ([event_id]) REFERENCES [dbo].[Event] ([id])
);

