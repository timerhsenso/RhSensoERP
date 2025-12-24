USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[tuse1]    Script Date: 17/11/2025 16:36:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tuse1](
	[cdusuario] [varchar](30) NOT NULL,
	[dcusuario] [varchar](50) NOT NULL,
	[senhauser] [nvarchar](20) NULL,
	[nmimpcche] [varchar](50) NULL,
	[tpusuario] [char](1) NOT NULL,
	[nomatric] [char](8) NULL,
	[cdempresa] [int] NULL,
	[cdfilial] [int] NULL,
	[nouser] [int] NULL,
	[email_usuario] [varchar](100) NULL,
	[flativo] [char](1) NOT NULL,
	[id] [uniqueidentifier] NOT NULL,
	[normalizedusername] [varchar](30) NULL,
	[idfuncionario] [uniqueidentifier] NULL,
	[flnaorecebeemail] [char](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[cdusuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_tuse1_id] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tuse1] ADD  DEFAULT (N'ABC') FOR [senhauser]
GO

ALTER TABLE [dbo].[tuse1] ADD  DEFAULT ('S') FOR [flativo]
GO

ALTER TABLE [dbo].[tuse1] ADD  DEFAULT (newsequentialid()) FOR [id]
GO

ALTER TABLE [dbo].[tuse1]  WITH CHECK ADD  CONSTRAINT [FK_tuse1_func1_idfuncionario] FOREIGN KEY([idfuncionario])
REFERENCES [dbo].[func1] ([id])
GO

ALTER TABLE [dbo].[tuse1] CHECK CONSTRAINT [FK_tuse1_func1_idfuncionario]
GO

