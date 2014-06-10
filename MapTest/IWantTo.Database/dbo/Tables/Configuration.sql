CREATE TABLE [dbo].[Configuration] (
	[Id]		INT NOT NULL IDENTITY (1, 1),
	[Version]	INT NOT NULL
	CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([Id] ASC)
)
