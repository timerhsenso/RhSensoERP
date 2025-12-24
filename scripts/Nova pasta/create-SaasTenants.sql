USE [bd_rhu_copenor]
GO

/****** Object:  Table [dbo].[SaasTenants]    Script Date: 17/11/2025 16:55:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SaasTenants](
	[Id] [uniqueidentifier] NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[Domain] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[MaxUsers] [int] NOT NULL,
	[PlanType] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_SaasTenants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT (CONVERT([bit],(1),0)) FOR [IsActive]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT ((10)) FOR [MaxUsers]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT (N'Basic') FOR [PlanType]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[SaasTenants] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO

