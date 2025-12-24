USE [bd_rhu_copenor]
GO

/****** Object:  Table [dbo].[SaasInvitations]    Script Date: 17/11/2025 16:56:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SaasInvitations](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[EmailNormalized]  AS (upper([Email])) PERSISTED,
	[TenantId] [uniqueidentifier] NOT NULL,
	[InvitedById] [uniqueidentifier] NOT NULL,
	[InvitationToken] [nvarchar](255) NOT NULL,
	[ExpiresAt] [datetime2](7) NOT NULL,
	[AcceptedAt] [datetime2](7) NULL,
	[IsAccepted] [bit] NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SaasInvitations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SaasInvitations] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[SaasInvitations] ADD  DEFAULT (CONVERT([bit],(0),0)) FOR [IsAccepted]
GO

ALTER TABLE [dbo].[SaasInvitations] ADD  DEFAULT (N'User') FOR [Role]
GO

ALTER TABLE [dbo].[SaasInvitations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[SaasInvitations] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO

ALTER TABLE [dbo].[SaasInvitations]  WITH CHECK ADD  CONSTRAINT [FK_SaasInvitations_SaasTenants_TenantId] FOREIGN KEY([TenantId])
REFERENCES [dbo].[SaasTenants] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SaasInvitations] CHECK CONSTRAINT [FK_SaasInvitations_SaasTenants_TenantId]
GO

ALTER TABLE [dbo].[SaasInvitations]  WITH CHECK ADD  CONSTRAINT [FK_SaasInvitations_tuse1_InvitedById] FOREIGN KEY([InvitedById])
REFERENCES [dbo].[tuse1] ([id])
GO

ALTER TABLE [dbo].[SaasInvitations] CHECK CONSTRAINT [FK_SaasInvitations_tuse1_InvitedById]
GO

