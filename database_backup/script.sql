USE [simple_mail]
GO
/****** Object:  Table [dbo].[message_users]    Script Date: 3/30/2014 2:28:39 AM ******/
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
/****** Object:  Table [dbo].[messages]    Script Date: 3/30/2014 2:28:39 AM ******/
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
/****** Object:  Table [dbo].[users]    Script Date: 3/30/2014 2:28:39 AM ******/
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
/****** Object:  View [dbo].[messages_view]    Script Date: 3/30/2014 2:28:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[messages_view]
AS
SELECT        m.msg_id AS messages_msg_id, m.subject AS messages_subject, m.text AS messages_text, m.pub_date AS messages_pub_date, m.state AS messages_state, 
                         a.user_id AS author_id, a.first_name AS author_first_name, a.email AS author_email, a.state AS author_state, u.user_id AS recipient_user_id, 
                         u.first_name AS recipient_first_name, u.email AS recipient_email, u.state AS recipient_state, mu.state AS messages_recipient_msg_state
FROM            dbo.messages AS m INNER JOIN
                         dbo.users AS a ON m.author_id = a.user_id INNER JOIN
                         dbo.message_users AS mu ON mu.msg_id = m.msg_id INNER JOIN
                         dbo.users AS u ON u.user_id = mu.user_id

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
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "mu"
            Begin Extent = 
               Top = 2
               Left = 388
               Bottom = 114
               Right = 558
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "m"
            Begin Extent = 
               Top = 53
               Left = 46
               Bottom = 182
               Right = 216
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "a"
            Begin Extent = 
               Top = 183
               Left = 346
               Bottom = 312
               Right = 516
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "u"
            Begin Extent = 
               Top = 58
               Left = 648
               Bottom = 187
               Right = 818
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2640
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'messages_view'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'messages_view'
GO
