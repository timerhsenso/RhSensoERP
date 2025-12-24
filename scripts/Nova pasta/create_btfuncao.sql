USE [bd_rhu_adn]
GO

/****** Object:  Table [dbo].[btfuncao]    Script Date: 17/11/2025 16:50:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[btfuncao](
	[cdfuncao] [varchar](30) NOT NULL,
	[cdsistema] [char](10) NOT NULL,
	[nmbotao] [varchar](30) NOT NULL,
	[dcbotao] [varchar](60) NOT NULL,
	[cdacao] [char](1) NOT NULL,
 CONSTRAINT [PK_btfuncao] PRIMARY KEY CLUSTERED 
(
	[cdsistema] ASC,
	[cdfuncao] ASC,
	[nmbotao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[btfuncao]  WITH CHECK ADD  CONSTRAINT [FK_btfuncao_fucn1_cdsistema_cdfuncao] FOREIGN KEY([cdsistema], [cdfuncao])
REFERENCES [dbo].[fucn1] ([cdsistema], [cdfuncao])
GO

ALTER TABLE [dbo].[btfuncao] CHECK CONSTRAINT [FK_btfuncao_fucn1_cdsistema_cdfuncao]
GO

