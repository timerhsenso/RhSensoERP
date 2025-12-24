USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[usrh1]    Script Date: 17/11/2025 16:52:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[usrh1](
	[cdusuario] [varchar](30) NOT NULL,
	[cdgruser] [varchar](30) NOT NULL,
	[dtinival] [datetime] NOT NULL,
	[dtfimval] [datetime] NULL,
	[cdsistema] [char](10) NULL,
	[idusuario] [uniqueidentifier] NULL,
	[idgrupodeusuario] [uniqueidentifier] NULL,
	[id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [UK_usrh1_cdusuario_cdsistema_cdgruser_dtinival] UNIQUE NONCLUSTERED 
(
	[cdusuario] ASC,
	[cdsistema] ASC,
	[cdgruser] ASC,
	[dtinival] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_usrh1_id] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[usrh1] ADD  DEFAULT (newsequentialid()) FOR [id]
GO

ALTER TABLE [dbo].[usrh1]  WITH CHECK ADD  CONSTRAINT [FK_usrh1_gurh1_idgrupodeusuario] FOREIGN KEY([idgrupodeusuario])
REFERENCES [dbo].[gurh1] ([id])
GO

ALTER TABLE [dbo].[usrh1] CHECK CONSTRAINT [FK_usrh1_gurh1_idgrupodeusuario]
GO

ALTER TABLE [dbo].[usrh1]  WITH CHECK ADD  CONSTRAINT [FK_usrh1_tuse1_idusuario] FOREIGN KEY([idusuario])
REFERENCES [dbo].[tuse1] ([id])
GO

ALTER TABLE [dbo].[usrh1] CHECK CONSTRAINT [FK_usrh1_tuse1_idusuario]
GO

