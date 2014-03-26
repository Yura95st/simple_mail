USE [simple_mail]
GO
/****** Object:  Table [dbo].[message_users]    Script Date: 3/26/2014 5:22:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[message_users](
	[msg_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_message_users] PRIMARY KEY CLUSTERED 
(
	[msg_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[messages]    Script Date: 3/26/2014 5:22:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[messages](
	[msg_id] [int] IDENTITY(1,1) NOT NULL,
	[author_id] [int] NOT NULL,
	[subject] [varchar](1000) NULL,
	[text] [text] NOT NULL,
	[pub_date] [datetime] NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_messages] PRIMARY KEY CLUSTERED 
(
	[msg_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[users]    Script Date: 3/26/2014 5:22:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](200) NOT NULL,
	[email] [varchar](200) NOT NULL,
	[password] [varchar](100) NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[message_users]  WITH CHECK ADD  CONSTRAINT [FK_message_users_messages] FOREIGN KEY([msg_id])
REFERENCES [dbo].[messages] ([msg_id])
GO
ALTER TABLE [dbo].[message_users] CHECK CONSTRAINT [FK_message_users_messages]
GO
ALTER TABLE [dbo].[message_users]  WITH CHECK ADD  CONSTRAINT [FK_message_users_users] FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[message_users] CHECK CONSTRAINT [FK_message_users_users]
GO
