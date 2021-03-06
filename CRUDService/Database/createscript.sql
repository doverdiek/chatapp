USE [Chatapplication]
GO
/****** Object:  Table [dbo].[Conversations]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conversations](
	[ConversationId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Conversations] PRIMARY KEY CLUSTERED 
(
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[MessageId] [uniqueidentifier] NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[ConversationId] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Picture] [varchar](max) NULL,
	[PasswordHash] [varbinary](max) NULL,
	[PasswordSalt] [varbinary](max) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users_Conversations]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_Conversations](
	[UserId] [uniqueidentifier] NOT NULL,
	[ConversationId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Users_Conversations] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Conversations] ADD  CONSTRAINT [DF_Conversations_ConversationId]  DEFAULT (newid()) FOR [ConversationId]
GO
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_MessageId]  DEFAULT (newid()) FOR [MessageId]
GO
ALTER TABLE [dbo].[Messages] ADD  CONSTRAINT [DF_Messages_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_UserId]  DEFAULT (newid()) FOR [UserId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Conversations] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([ConversationId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Conversations]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [FK_Messages_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [FK_Messages_Users]
GO
ALTER TABLE [dbo].[Users_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Users_Conversations_Conversations] FOREIGN KEY([ConversationId])
REFERENCES [dbo].[Conversations] ([ConversationId])
GO
ALTER TABLE [dbo].[Users_Conversations] CHECK CONSTRAINT [FK_Users_Conversations_Conversations]
GO
ALTER TABLE [dbo].[Users_Conversations]  WITH CHECK ADD  CONSTRAINT [FK_Users_Conversations_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Users_Conversations] CHECK CONSTRAINT [FK_Users_Conversations_Users]
GO
/****** Object:  StoredProcedure [dbo].[Ins_Conversation]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_Conversation] @UserId1 uniqueidentifier, @UserId2 uniqueidentifier
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @NewConversationIdTable TABLE (ConversationId uniqueidentifier)

	insert into Conversations(ConversationId) 
	OUTPUT inserted.ConversationId INTO @NewConversationIdTable(ConversationId)
	Values(NewId())

	DECLARE @NewConversationId uniqueidentifier = (SELECT TOP(1) ConversationId FROM @NewConversationIdTable)
	insert into Users_Conversations(UserId, ConversationId) VALUES(@UserId1, @NewConversationId)
	insert into Users_Conversations(UserId, ConversationId) VALUES(@UserId2, @NewConversationId)

	SELECT @NewConversationId 'ConversationId'
END


GO
/****** Object:  StoredProcedure [dbo].[Sel_ConversationMessages]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Sel_ConversationMessages] 
@UserId uniqueidentifier,
@ConversationId uniqueidentifier
AS
BEGIN

	SET NOCOUNT ON;
	
	SELECT * FROM Messages
	WHERE ConversationId = @ConversationId
	AND @UserId IN (SELECT userid from Users_Conversations
					WHERE ConversationId = @ConversationId
					)
END
GO
/****** Object:  StoredProcedure [dbo].[Sel_Conversations]    Script Date: 20-Jun-19 12:27:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Sel_Conversations] @UserId uniqueidentifier
AS
BEGIN

	SET NOCOUNT ON;
	SELECT Users.FirstName, Users.LastName, Users.UserId, Users_Conversations.ConversationId FROM Users
	INNER JOIN Users_Conversations ON Users.UserId = Users_Conversations.UserId
	WHERE conversationid IN (SELECT ConversationId FROM Users_Conversations
	WHERE UserId = @UserId)
	AND Users.UserId != @UserId
END


GO
