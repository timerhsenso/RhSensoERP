USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[fucn1]    Script Date: 17/11/2025 16:50:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[fucn1](
	[cdfuncao] [varchar](30) NOT NULL,
	[dcfuncao] [varchar](80) NULL,
	[cdsistema] [char](10) NOT NULL,
	[dcmodulo] [varchar](100) NULL,
	[descricaomodulo] [varchar](100) NULL,
 CONSTRAINT [pk_fucn1_todos] PRIMARY KEY CLUSTERED 
(
	[cdsistema] ASC,
	[cdfuncao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[fucn1]  WITH CHECK ADD  CONSTRAINT [fk_fucn1_cdsistema] FOREIGN KEY([cdsistema])
REFERENCES [dbo].[tsistema] ([cdsistema])
GO

ALTER TABLE [dbo].[fucn1] CHECK CONSTRAINT [fk_fucn1_cdsistema]
GO

