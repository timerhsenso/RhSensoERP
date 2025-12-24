USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[gurh1]    Script Date: 17/11/2025 16:51:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[gurh1](
	[cdgruser] [varchar](30) NOT NULL,
	[dcgruser] [varchar](60) NULL,
	[cdsistema] [char](10) NOT NULL,
	[id] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cdgruser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_gurh1_id] UNIQUE NONCLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[gurh1] ADD  DEFAULT (newsequentialid()) FOR [id]
GO

ALTER TABLE [dbo].[gurh1]  WITH CHECK ADD  CONSTRAINT [FK_gurh1_tsistema_cdsistema] FOREIGN KEY([cdsistema])
REFERENCES [dbo].[tsistema] ([cdsistema])
GO

ALTER TABLE [dbo].[gurh1] CHECK CONSTRAINT [FK_gurh1_tsistema_cdsistema]
GO

