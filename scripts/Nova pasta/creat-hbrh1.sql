USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[hbrh1]    Script Date: 17/11/2025 16:51:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[hbrh1](
	[cdgruser] [varchar](30) NOT NULL,
	[cdfuncao] [varchar](30) NOT NULL,
	[cdacoes] [char](20) NOT NULL,
	[cdrestric] [char](1) NOT NULL,
	[cdsistema] [char](10) NULL,
	[id] [uniqueidentifier] NOT NULL,
	[idgrupodeusuario] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[cdgruser] ASC,
	[cdfuncao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_hbrh1_id] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[hbrh1] ADD  DEFAULT (newsequentialid()) FOR [id]
GO

ALTER TABLE [dbo].[hbrh1]  WITH CHECK ADD FOREIGN KEY([cdgruser])
REFERENCES [dbo].[gurh1] ([cdgruser])
GO

ALTER TABLE [dbo].[hbrh1]  WITH CHECK ADD  CONSTRAINT [FK_hbrh1_fucn1_cdsistema_cdfuncao] FOREIGN KEY([cdsistema], [cdfuncao])
REFERENCES [dbo].[fucn1] ([cdsistema], [cdfuncao])
GO

ALTER TABLE [dbo].[hbrh1] CHECK CONSTRAINT [FK_hbrh1_fucn1_cdsistema_cdfuncao]
GO

ALTER TABLE [dbo].[hbrh1]  WITH CHECK ADD  CONSTRAINT [FK_hbrh1_gurh1_idgrupodeusuario] FOREIGN KEY([idgrupodeusuario])
REFERENCES [dbo].[gurh1] ([id])
GO

ALTER TABLE [dbo].[hbrh1] CHECK CONSTRAINT [FK_hbrh1_gurh1_idgrupodeusuario]
GO

