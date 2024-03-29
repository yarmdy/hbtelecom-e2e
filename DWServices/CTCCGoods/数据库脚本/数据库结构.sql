USE [CTCCGoods]
GO
/****** Object:  Table [dbo].[cattachment]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cattachment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ovid] [int] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[url] [nvarchar](1024) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cclass]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cclass](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_cclass] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cdesignset]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cdesignset](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[uid] [int] NOT NULL,
	[wid] [int] NOT NULL,
 CONSTRAINT [PK_cdesignset] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cgoods]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cgoods](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[cid] [int] NOT NULL,
	[pid] [int] NULL,
 CONSTRAINT [PK_cgoods] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[corder]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[corder](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[vid] [int] NULL,
	[createuid] [int] NOT NULL,
	[createtime] [datetime] NOT NULL,
	[receiveuid] [int] NOT NULL,
	[status] [int] NOT NULL,
	[statustime] [datetime] NOT NULL,
	[verifyno] [int] NULL,
	[verifyrole] [int] NULL,
	[sendall] [bit] NULL,
 CONSTRAINT [PK_corder] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[corder_goods]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[corder_goods](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[oid] [int] NOT NULL,
	[gid] [int] NOT NULL,
	[gnum] [int] NOT NULL,
 CONSTRAINT [PK_corder_goods] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[corder_sendgoods]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[corder_sendgoods](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ovid] [int] NOT NULL,
	[gid] [int] NOT NULL,
	[gnum] [int] NOT NULL,
	[sendtime] [datetime] NOT NULL,
	[receivetime] [datetime] NULL,
 CONSTRAINT [PK_corder_sendgoods] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[corder_verifyflow]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[corder_verifyflow](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[oid] [int] NOT NULL,
	[uid] [int] NOT NULL,
	[verifyno] [int] NOT NULL,
	[createtime] [datetime] NOT NULL,
	[status] [int] NOT NULL,
	[des] [nvarchar](255) NULL,
	[endtime] [datetime] NULL,
	[ovid] [int] NULL,
	[duid] [int] NULL,
	[localstatus] [int] NULL,
	[sendname] [nvarchar](50) NULL,
	[plantime] [datetime] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cstock]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cstock](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[wid] [int] NOT NULL,
	[gid] [int] NOT NULL,
	[stock] [int] NOT NULL,
	[purchased] [int] NOT NULL,
	[require] [int] NOT NUll,
 CONSTRAINT [PK_cstock] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cstockio]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cstockio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[wid] [int] NOT NULL,
	[gid] [int] NOT NULL,
	[ionumber] [int] NOT NULL,
	[purchased] [int] NOT NULL,
	[createtime] [datetime] NOT NULL,
	[uid] [int] NOT NULL,
	[stype] [int] NOT NULL,
	[oid] [int] NULL,
	[require] [int] NOT NUll,
 CONSTRAINT [PK_cstockio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cuser]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cuser](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[utype] [int] NOT NULL,
	[status] [int] NOT NULL,
	[pwd] [varchar](50) NOT NULL,
	[wid] [int] NULL,
	[tel] [varchar](50) NULL,
	[contacts] [nvarchar](50) NULL,
 CONSTRAINT [PK_cuser_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[cverifyflow]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cverifyflow](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[vtime] [datetime] NOT NULL,
	[vrule] [ntext] NOT NULL,
 CONSTRAINT [PK_cverifyflow] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cwarehouse]    Script Date: 2018/6/13 17:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cwarehouse](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_cwarehouse] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
