GO
/****** Object:  Database [FastEvents]    Script Date: 07/05/2021 20:18:03 ******/
CREATE DATABASE [FastEvents]
GO
ALTER DATABASE [FastEvents] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FastEvents].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FastEvents] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FastEvents] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FastEvents] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FastEvents] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FastEvents] SET ARITHABORT OFF 
GO
ALTER DATABASE [FastEvents] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FastEvents] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FastEvents] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FastEvents] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FastEvents] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FastEvents] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FastEvents] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FastEvents] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FastEvents] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FastEvents] SET  DISABLE_BROKER 
GO
ALTER DATABASE [FastEvents] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FastEvents] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FastEvents] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FastEvents] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FastEvents] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FastEvents] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [FastEvents] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FastEvents] SET RECOVERY FULL 
GO
ALTER DATABASE [FastEvents] SET  MULTI_USER 
GO
ALTER DATABASE [FastEvents] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FastEvents] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FastEvents] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FastEvents] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FastEvents] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FastEvents] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'FastEvents', N'ON'
GO
ALTER DATABASE [FastEvents] SET QUERY_STORE = OFF
GO
USE [FastEvents]
GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 07/05/2021 20:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticket](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[event_id] [bigint] NOT NULL,
	[qrc_filename] [varchar](50) NOT NULL,
	[owner_uuid] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Event]    Script Date: 07/05/2021 20:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Event](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](50) NOT NULL,
	[organizer] [varchar](50) NOT NULL,
	[start_date] [datetime] NOT NULL,
	[end_date] [datetime] NOT NULL,
	[capacity] [int] NOT NULL,
	[location] [varchar](200) NOT NULL,
	[description] [varchar](500) NOT NULL,
	[picture_filename] [varchar](50) NOT NULL,
	[owner_uuid] [varchar](50) NOT NULL,
	[category] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[EventView]    Script Date: 07/05/2021 20:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventView]
AS
SELECT        COUNT(dbo.Ticket.id) AS number_tickets, dbo.Event.id, dbo.Event.name, dbo.Event.organizer, dbo.Event.start_date, dbo.Event.end_date, dbo.Event.capacity, dbo.Event.location, dbo.Event.description, 
                         dbo.Event.picture_filename, dbo.Event.owner_uuid, dbo.Event.category
FROM            dbo.Event LEFT OUTER JOIN
                         dbo.Ticket ON dbo.Event.id = dbo.Ticket.event_id
GROUP BY dbo.Event.id, dbo.Event.name, dbo.Event.organizer, dbo.Event.start_date, dbo.Event.end_date, dbo.Event.capacity, dbo.Event.location, dbo.Event.description, dbo.Event.picture_filename, dbo.Event.owner_uuid, 
                         dbo.Event.category
GO
/****** Object:  Table [dbo].[Stat]    Script Date: 07/05/2021 20:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stat](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[event_id] [bigint] NOT NULL,
	[date] [datetime] NOT NULL,
 CONSTRAINT [PK_Stat] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[StatByEvent]    Script Date: 07/05/2021 20:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[StatByEvent]
AS
SELECT        COUNT(dbo.Stat.id) AS number_view, dbo.Event.name, dbo.Event.id
FROM            dbo.Stat LEFT OUTER JOIN
                         dbo.Event ON dbo.Stat.event_id = dbo.Event.id
GROUP BY dbo.Stat.event_id, dbo.Event.name, dbo.Event.id
GO
SET IDENTITY_INSERT [dbo].[Event] ON 

INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (1, N'Pop 80', N'Mairie de Paris', CAST(N'2021-07-09T20:00:00.000' AS DateTime), CAST(N'2021-07-10T00:00:00.000' AS DateTime), 500, N'211 Avenue Jean Jaurès, 75019 Paris', N'Un concert Pop des années 80 avec les plus grands artistes du top 50', N'73686.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (2, N'Quantum physics pour les nuls', N'Fac de Science', CAST(N'2021-07-15T16:00:00.000' AS DateTime), CAST(N'2021-07-15T18:00:00.000' AS DateTime), 300, N'17 Rue de l''Aubrac, 75012 Paris', N'Venez découvrir la physique quantique pour les nuls', N'espace-de-ouest-lyonnais-projection-87b8124a.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Conference')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (3, N'Arthur et sa troupe', N'Arthur & Co', CAST(N'2021-07-24T15:00:00.000' AS DateTime), CAST(N'2021-07-24T18:00:00.000' AS DateTime), 70, N'5 Rue Blainville, 75005 Paris', N'Arthur fait son show avec ses camardes.
Show familiale, rire assuré', N'cabaret-el-musical-de.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Show')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (7, N'Les 3 charrue', N'Fest', CAST(N'2021-07-09T09:00:00.000' AS DateTime), CAST(N'2021-07-09T10:10:00.000' AS DateTime), 1000, N'Bois de Boulogne, 75016 Paris', N'Du son et de la bonne humeur', N'téléchargement.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Festival')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (8, N'Web Comedy Awards', N'W9', CAST(N'2021-07-30T21:30:00.000' AS DateTime), CAST(N'2021-07-30T23:30:00.000' AS DateTime), 150, N'1 Boulevard Saint-Martin, 75003 Paris', N'Une cérémonie regroupant les plus grands talents du web pour une soirée de folie', N'Allogram_28.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Ceremony')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (9, N'Les pères Noël verts', N'Croix Rouge', CAST(N'2021-12-25T18:00:00.000' AS DateTime), CAST(N'2021-12-25T19:00:00.000' AS DateTime), 100, N'Théâtre 13 / Jardin, 103 A Boulevard Auguste Blanqui, 75013 Paris', N'Un Noël joyeux pour tous', N'noel_4.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Charity')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (10, N'Course pour le climat', N'WWF', CAST(N'2021-08-15T15:00:00.000' AS DateTime), CAST(N'2021-08-15T17:30:00.000' AS DateTime), 400, N'Bois de Vincennes, Paris', N'Une course pour le climat', N'o6nrjHud5yiMZ0UJJpWoJK83MrVhzX3-bWIvZoCXN4hmb.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Competition')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (11, N'Picasso le retour', N'Mairie de Paris', CAST(N'2021-08-08T08:00:00.000' AS DateTime), CAST(N'2021-08-15T20:00:00.000' AS DateTime), 300, N'Musée Beaubourg', N'Picasso dans une exposition encore jamais vue', N'installation-srgm-hilma-2480x1396-1024x576.png', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Exhibition')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (12, N'Arthur et le cirque du soleil', N'Arthur & Co', CAST(N'2021-10-20T17:00:00.000' AS DateTime), CAST(N'2021-10-25T17:00:00.000' AS DateTime), 200, N'Cirque du Soleil', N'Un show qui va vous en mettre plein les yeux', N'9ad-5872-8cc2-179f71f4d313-4437076-1-1024x576.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Show')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (13, N'Concert 1', N'asso', CAST(N'2021-07-05T08:11:00.000' AS DateTime), CAST(N'2021-08-05T08:11:00.000' AS DateTime), 200, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (14, N'Concert 2', N'asso', CAST(N'2021-07-05T08:12:00.000' AS DateTime), CAST(N'2021-08-05T08:12:00.000' AS DateTime), 100, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (15, N'Concert 3', N'asso', CAST(N'2021-07-05T08:12:00.000' AS DateTime), CAST(N'2021-08-05T08:12:00.000' AS DateTime), 200, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (16, N'Concert 4', N'asso', CAST(N'2021-07-05T08:12:00.000' AS DateTime), CAST(N'2021-08-05T08:12:00.000' AS DateTime), 200, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (17, N'Concert 5', N'asso', CAST(N'2021-07-05T08:13:00.000' AS DateTime), CAST(N'2021-08-05T08:13:00.000' AS DateTime), 200, N'Epita', N'Concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (18, N'Concert 6 ', N'asso', CAST(N'2021-07-05T08:13:00.000' AS DateTime), CAST(N'2021-08-05T08:13:00.000' AS DateTime), 200, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
INSERT [dbo].[Event] ([id], [name], [organizer], [start_date], [end_date], [capacity], [location], [description], [picture_filename], [owner_uuid], [category]) VALUES (19, N'Concert 7 ', N'asso', CAST(N'2021-07-05T08:13:00.000' AS DateTime), CAST(N'2021-08-05T08:13:00.000' AS DateTime), 200, N'Epita', N'concert', N'event_place_holder.jpg', N'8c096c29-2553-4ca8-94e5-65893c52ee6e', N'Concert')
SET IDENTITY_INSERT [dbo].[Event] OFF
GO
SET IDENTITY_INSERT [dbo].[Stat] ON 

INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (1, 1, CAST(N'2021-05-07T18:55:30.693' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (2, 2, CAST(N'2021-05-07T18:57:47.117' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (3, 3, CAST(N'2021-05-07T19:00:17.243' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (4, 7, CAST(N'2021-05-07T19:09:00.950' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (5, 8, CAST(N'2021-05-07T19:11:45.697' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (6, 8, CAST(N'2021-05-07T19:25:23.580' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (17, 9, CAST(N'2021-05-07T20:03:19.683' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (18, 9, CAST(N'2021-05-07T20:04:48.677' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (19, 10, CAST(N'2021-05-07T20:06:06.857' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (20, 11, CAST(N'2021-05-07T20:07:47.660' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (21, 12, CAST(N'2021-05-07T20:09:47.750' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (22, 12, CAST(N'2021-05-07T20:09:59.800' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (23, 1, CAST(N'2021-05-07T20:10:10.037' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (24, 2, CAST(N'2021-05-07T20:10:13.203' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (25, 3, CAST(N'2021-05-07T20:10:16.217' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (26, 7, CAST(N'2021-05-07T20:10:20.250' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (27, 13, CAST(N'2021-05-07T20:12:00.053' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (28, 14, CAST(N'2021-05-07T20:12:18.227' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (29, 15, CAST(N'2021-05-07T20:12:43.810' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (30, 16, CAST(N'2021-05-07T20:13:00.917' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (31, 17, CAST(N'2021-05-07T20:13:20.510' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (32, 18, CAST(N'2021-05-07T20:13:37.293' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (33, 19, CAST(N'2021-05-07T20:13:56.510' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (34, 3, CAST(N'2021-05-07T20:15:19.803' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (35, 3, CAST(N'2021-05-07T20:15:25.833' AS DateTime))
INSERT [dbo].[Stat] ([id], [event_id], [date]) VALUES (36, 3, CAST(N'2021-05-07T20:15:44.400' AS DateTime))
SET IDENTITY_INSERT [dbo].[Stat] OFF
GO
ALTER TABLE [dbo].[Stat]  WITH CHECK ADD  CONSTRAINT [FK_Stat_Event] FOREIGN KEY([event_id])
REFERENCES [dbo].[Event] ([id])
GO
ALTER TABLE [dbo].[Stat] CHECK CONSTRAINT [FK_Stat_Event]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_Event] FOREIGN KEY([event_id])
REFERENCES [dbo].[Event] ([id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Tickets_Event]
GO
USE [master]
GO
ALTER DATABASE [FastEvents] SET  READ_WRITE 
GO
