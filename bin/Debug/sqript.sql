USE [master]
GO
/****** Object:  Database [AutoParts]    Script Date: 24.03.2026 16:23:27 ******/
CREATE DATABASE [AutoParts]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AutoParts', FILENAME = N'D:\SQL SERVER\MSSQL17.MSSQLSERVER\MSSQL\DATA\AutoParts.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AutoParts_log', FILENAME = N'D:\SQL SERVER\MSSQL17.MSSQLSERVER\MSSQL\DATA\AutoParts_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [AutoParts] SET COMPATIBILITY_LEVEL = 170
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AutoParts].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AutoParts] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [AutoParts] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [AutoParts] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [AutoParts] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [AutoParts] SET ARITHABORT OFF 
GO
ALTER DATABASE [AutoParts] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [AutoParts] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [AutoParts] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [AutoParts] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [AutoParts] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [AutoParts] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [AutoParts] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [AutoParts] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [AutoParts] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [AutoParts] SET  DISABLE_BROKER 
GO
ALTER DATABASE [AutoParts] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [AutoParts] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [AutoParts] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [AutoParts] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [AutoParts] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [AutoParts] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [AutoParts] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [AutoParts] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [AutoParts] SET  MULTI_USER 
GO
ALTER DATABASE [AutoParts] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [AutoParts] SET DB_CHAINING OFF 
GO
ALTER DATABASE [AutoParts] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [AutoParts] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [AutoParts] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [AutoParts] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [AutoParts] SET OPTIMIZED_LOCKING = OFF 
GO
ALTER DATABASE [AutoParts] SET QUERY_STORE = ON
GO
ALTER DATABASE [AutoParts] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [AutoParts]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 24.03.2026 16:23:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandID] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[Phone] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[TotalAmount] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Article] [varchar](50) NOT NULL,
	[ProductName] [varchar](200) NOT NULL,
	[BrandID] [int] NULL,
	[CategoryID] [int] NULL,
	[Description] [varchar](500) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[StockQuantity] [int] NOT NULL,
	[MinStockLevel] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderDetails]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderDetails](
	[PurchaseOrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrders]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrders](
	[PurchaseOrderID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[Status] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockMovements]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockMovements](
	[MovementID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[MovementType] [varchar](10) NOT NULL,
	[Quantity] [int] NOT NULL,
	[MovementDate] [datetime] NOT NULL,
	[Reference] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[MovementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierName] [varchar](150) NOT NULL,
	[ContactPerson] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 24.03.2026 16:23:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[FullName] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[RoleID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Brands] ON 
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (1, N'Bosch')
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (2, N'Castrol')
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (3, N'Mobil')
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (4, N'Philips')
GO
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (5, N'Ермак')
GO
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (1, N'Двигатель')
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (2, N'Кузов')
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (3, N'Масла и тех. жидкости')
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (4, N'Аксессуары')
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (5, N'Автохимия')
GO
INSERT [dbo].[Categories] ([CategoryID], [CategoryName]) VALUES (6, N'Тормозная система')
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[Customers] ON 
GO
INSERT [dbo].[Customers] ([CustomerID], [FullName], [Phone], [Email], [Address]) VALUES (1, N'Полное Имя Клиента', N'+79992221313', N'gggg@gmail.com', N'г. Омск')
GO
INSERT [dbo].[Customers] ([CustomerID], [FullName], [Phone], [Email], [Address]) VALUES (2, N'Охмак Дмитрий Яковлевич', N'+79932318512', N'oxman@gmail.com', N'г. Омск')
GO
INSERT [dbo].[Customers] ([CustomerID], [FullName], [Phone], [Email], [Address]) VALUES (3, N'Решедько Вячеслав Алексеевич', N'+79052225414', N'reschedko.vyacheslav@mail.ru', N'Таджикистан')
GO
SET IDENTITY_INSERT [dbo].[Customers] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDetails] ON 
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (1, 1, 5, 2, CAST(2400.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (2, 2, 1, 1, CAST(850.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (3, 3, 2, 3, CAST(2400.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (4, 3, 3, 2, CAST(5600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (5, 3, 6, 2, CAST(600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (6, 4, 7, 1, CAST(150.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (7, 5, 12, 4, CAST(22.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (8, 6, 8, 2, CAST(35.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (9, 7, 15, 1, CAST(25.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (10, 8, 9, 3, CAST(22.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (11, 9, 4, 2, CAST(75.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (12, 10, 11, 1, CAST(9.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (13, 11, 13, 5, CAST(11.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (14, 12, 10, 1, CAST(130.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (15, 13, 14, 2, CAST(7.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (16, 14, 16, 1, CAST(65.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (17, 15, 17, 1, CAST(18.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (18, 16, 18, 3, CAST(46.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (19, 17, 19, 2, CAST(12.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (20, 18, 20, 1, CAST(4.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (21, 1, 21, 1, CAST(45.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (22, 2, 22, 2, CAST(35.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (23, 4, 23, 1, CAST(18.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (24, 5, 24, 1, CAST(55.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (25, 6, 25, 1, CAST(28.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (26, 19, 4, 1, CAST(250.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (27, 19, 12, 1, CAST(130.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (28, 20, 6, 1, CAST(600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (29, 20, 1, 2, CAST(850.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (30, 21, 21, 1, CAST(18.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (31, 21, 10, 1, CAST(24.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (32, 21, 16, 1, CAST(32.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (33, 22, 16, 1, CAST(32.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (34, 22, 10, 1, CAST(24.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (35, 22, 8, 1, CAST(42.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (36, 22, 31, 1, CAST(15.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (37, 23, 22, 1, CAST(45.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (38, 24, 6, 1, CAST(600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (39, 24, 19, 1, CAST(12.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (40, 25, 18, 1, CAST(46.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (41, 25, 5, 1, CAST(2400.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (42, 25, 14, 1, CAST(28.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (43, 26, 10, 1, CAST(24.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (44, 27, 31, 3, CAST(15.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (45, 27, 30, 1, CAST(32.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[OrderDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (1, 3, CAST(N'2025-11-18T08:59:33.570' AS DateTime), N'Новый', CAST(4800.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (2, 2, CAST(N'2025-11-18T08:59:42.930' AS DateTime), N'Новый', CAST(850.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (3, 1, CAST(N'2025-11-18T08:59:59.000' AS DateTime), N'В обработке', CAST(19600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (4, 2, CAST(N'2025-11-19T09:15:00.000' AS DateTime), N'Новый', CAST(1250.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (5, 1, CAST(N'2025-11-19T10:30:00.000' AS DateTime), N'В обработке', CAST(9800.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (6, 3, CAST(N'2025-11-19T11:45:00.000' AS DateTime), N'Выполнен', CAST(5600.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (7, 1, CAST(N'2025-11-20T08:20:00.000' AS DateTime), N'Новый', CAST(3200.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (8, 2, CAST(N'2025-11-20T12:10:00.000' AS DateTime), N'Отменен', CAST(450.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (9, 3, CAST(N'2025-11-20T14:55:00.000' AS DateTime), N'В обработке', CAST(7200.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (10, 2, CAST(N'2025-11-21T09:05:00.000' AS DateTime), N'Выполнен', CAST(1890.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (11, 1, CAST(N'2025-11-21T16:40:00.000' AS DateTime), N'Новый', CAST(5400.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (12, 3, CAST(N'2025-11-22T10:25:00.000' AS DateTime), N'В обработке', CAST(3100.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (13, 1, CAST(N'2025-11-22T13:30:00.000' AS DateTime), N'Выполнен', CAST(8700.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (14, 2, CAST(N'2025-11-23T11:00:00.000' AS DateTime), N'Новый', CAST(2200.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (15, 3, CAST(N'2025-11-23T17:15:00.000' AS DateTime), N'Отменен', CAST(900.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (16, 1, CAST(N'2025-11-24T08:45:00.000' AS DateTime), N'В обработке', CAST(6400.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (17, 2, CAST(N'2025-11-24T15:20:00.000' AS DateTime), N'Новый', CAST(1800.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (18, 3, CAST(N'2025-11-25T12:50:00.000' AS DateTime), N'Выполнен', CAST(4300.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (19, 2, CAST(N'2026-01-24T16:53:01.083' AS DateTime), N'Выполнен', CAST(380.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (20, 1, CAST(N'2026-03-12T15:06:49.460' AS DateTime), N'Новый', CAST(2300.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (21, 2, CAST(N'2026-03-12T15:07:34.473' AS DateTime), N'Новый', CAST(74.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (22, 3, CAST(N'2026-03-12T15:07:43.750' AS DateTime), N'Новый', CAST(113.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (23, 2, CAST(N'2026-03-12T15:07:50.783' AS DateTime), N'Новый', CAST(45.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (24, 1, CAST(N'2026-03-12T15:07:57.470' AS DateTime), N'Новый', CAST(612.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (25, 3, CAST(N'2026-03-12T15:08:13.617' AS DateTime), N'Новый', CAST(2474.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (26, 2, CAST(N'2026-03-12T15:08:17.567' AS DateTime), N'Новый', CAST(24.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[Orders] ([OrderID], [CustomerID], [OrderDate], [Status], [TotalAmount]) VALUES (27, 2, CAST(N'2026-03-12T15:08:25.463' AS DateTime), N'Новый', CAST(77.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (1, N'BOSCH-123', N'Масляный фильтр', 1, 1, N'Фильтр для двигателя', CAST(850.00 AS Decimal(10, 2)), 25, 5)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (2, N'CAST-10W40', N'Моторное масло 10W-40', 2, 1, N'Полусинтетика 4л', CAST(2400.00 AS Decimal(10, 2)), 9, 3)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (3, N'MOB-5W30', N'Моторное масло 5W-30', 3, 1, N'Синтетика 5л', CAST(5600.00 AS Decimal(10, 2)), 10, 3)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (4, N'ЕРМАК-01', N'Держатель для телефона', 5, 4, N'Держатель для телефона магнитный', CAST(250.00 AS Decimal(10, 2)), 21, 3)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (5, N'ЕРМАК-02', N'Подстаканник', 2, 4, N'Подстаканник в воздуходув', CAST(2400.00 AS Decimal(10, 2)), 9, 3)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (6, N'PHL-H7', N'Лампа H4', 4, 4, N'Галогеновая лампа для фар', CAST(600.00 AS Decimal(10, 2)), 46, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (7, N'BOSCH-0986', N'Свеча зажигания BOSCH', 1, 1, N'Свеча зажигания BOSCH WR7DC', CAST(58.00 AS Decimal(10, 2)), 120, 20)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (8, N'CASTROL-0514', N'Моторное масло Castrol Edge', 2, 3, N'Синтетическое масло 5W-30 4L', CAST(42.00 AS Decimal(10, 2)), 84, 15)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (9, N'MOBIL-5W40', N'Моторное масло Mobil Super', 3, 3, N'Моторное масло 5W-40 5L', CAST(38.00 AS Decimal(10, 2)), 60, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (10, N'PHILIPS-H7', N'Лампы ближнего света Philips', 4, 2, N'Лампы H7 55W', CAST(24.00 AS Decimal(10, 2)), 92, 20)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (11, N'ERMAK-CC01', N'Автоковрики Ермак', 5, 4, N'Коврики в салон универсальные', CAST(35.00 AS Decimal(10, 2)), 90, 20)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (12, N'BOSCH-S4004', N'Аккумулятор BOSCH', 1, 1, N'Аккумулятор 60Ah 540A', CAST(130.00 AS Decimal(10, 2)), 34, 7)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (13, N'CASTROL-DOT4', N'Тормозная жидкость Castrol', 2, 3, N'Тормозная жидкость DOT 4 1L', CAST(15.00 AS Decimal(10, 2)), 70, 15)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (14, N'MOBIL-ATF', N'Трансмиссионное масло Mobil', 3, 3, N'ATF жидкость 1L', CAST(28.00 AS Decimal(10, 2)), 44, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (15, N'PHILIPS-HB3', N'Лампы дальнего света Philips', 4, 2, N'Лампы HB3 60W', CAST(26.00 AS Decimal(10, 2)), 80, 18)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (16, N'ERMAK-ANTIFR', N'Антифриз Ермак', 5, 3, N'Антифриз концентрат 5L', CAST(32.00 AS Decimal(10, 2)), 53, 12)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (17, N'BOSCH-WIPER', N'Щетки стеклоочистителя Bosch', 1, 4, N'Щетки стеклоочистителя 600mm', CAST(28.00 AS Decimal(10, 2)), 75, 15)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (18, N'CASTROL-5W20', N'Моторное масло Castrol Magnatec', 2, 3, N'Масло 5W-20 4L', CAST(46.00 AS Decimal(10, 2)), 49, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (19, N'MOBIL-GREASE', N'Смазка Mobil', 3, 5, N'Универсальная смазка 400г', CAST(12.00 AS Decimal(10, 2)), 119, 25)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (20, N'PHILIPS-P21W', N'Лампы габаритные Philips', 4, 2, N'Лампы P21W 5W', CAST(183.00 AS Decimal(10, 2)), 200, 40)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (21, N'ERMAK-WAX', N'Воск для кузова Ермак', 5, 5, N'Воск-полироль 500ml', CAST(18.00 AS Decimal(10, 2)), 64, 20)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (22, N'BOSCH-FILTER', N'Воздушный фильтр Bosch', 1, 1, N'Воздушный фильтр двигателя', CAST(45.00 AS Decimal(10, 2)), 69, 15)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (23, N'CASTROL-10W40', N'Моторное масло Castrol 10W-40', 2, 3, N'Масло минеральное 10W-40 4L', CAST(35.00 AS Decimal(10, 2)), 60, 12)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (24, N'MOBIL-CLEAN', N'Очиститель топливной системы', 3, 5, N'Очиститель инжектора 300ml', CAST(18.00 AS Decimal(10, 2)), 90, 20)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (25, N'PHILIPS-DRL', N'Лампы ДХО Philips', 4, 2, N'Лампы дневные ходовые LED', CAST(55.00 AS Decimal(10, 2)), 40, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (26, N'ERMAK-WASH', N'Автошампунь Ермак', 5, 5, N'Шампунь для бесконтактной мойки 5L', CAST(28.00 AS Decimal(10, 2)), 75, 15)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (27, N'BOSCH-SENSOR', N'Датчик ABS Bosch', 1, 6, N'Датчик ABS передний', CAST(85.00 AS Decimal(10, 2)), 30, 8)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (28, N'CASTROL-CVT', N'Масло для вариатора Castrol', 2, 3, N'Масло CVT 1L', CAST(65.00 AS Decimal(10, 2)), 40, 10)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (29, N'MOBIL-ATF132', N'Трансмиссионное масло ATF 132', 3, 3, N'Масло ATF для Mercedes 1L', CAST(42.00 AS Decimal(10, 2)), 35, 7)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (30, N'PHILIPS-FOG', N'Противотуманные лампы Philips', 4, 2, N'Лампы H11 для ПТФ', CAST(32.00 AS Decimal(10, 2)), 54, 12)
GO
INSERT [dbo].[Products] ([ProductID], [Article], [ProductName], [BrandID], [CategoryID], [Description], [Price], [StockQuantity], [MinStockLevel]) VALUES (31, N'ERMAK-TIRE', N'Средство для шин Ермак', 5, 5, N'Чернитель для покрышек 500ml', CAST(15.00 AS Decimal(10, 2)), 96, 25)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseOrderDetails] ON 
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (1, 1, 4, 10, CAST(3.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (2, 1, 4, 10, CAST(2.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (3, 2, 7, 20, CAST(40.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (4, 2, 9, 15, CAST(18.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (5, 3, 12, 30, CAST(10.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (6, 4, 1, 25, CAST(8.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (7, 4, 2, 20, CAST(12.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (8, 5, 5, 50, CAST(6.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (9, 6, 8, 40, CAST(7.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (10, 6, 10, 15, CAST(25.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (11, 7, 14, 100, CAST(5.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (12, 8, 16, 60, CAST(20.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (13, 9, 18, 35, CAST(42.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (14, 10, 20, 80, CAST(3.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (15, 11, 22, 45, CAST(30.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (16, 12, 24, 25, CAST(50.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (17, 13, 26, 70, CAST(15.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (18, 14, 28, 55, CAST(22.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (19, 15, 30, 40, CAST(35.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (20, 11, 3, 30, CAST(40.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[PurchaseOrderDetails] ([PurchaseOrderDetailID], [PurchaseOrderID], [ProductID], [Quantity], [UnitPrice]) VALUES (21, 15, 6, 25, CAST(28.00 AS Decimal(10, 2)))
GO
SET IDENTITY_INSERT [dbo].[PurchaseOrderDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[PurchaseOrders] ON 
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (1, 1, CAST(N'2025-11-18T09:00:40.547' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (2, 1, CAST(N'2025-11-18T09:00:00.000' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (3, 3, CAST(N'2025-11-19T10:20:00.000' AS DateTime), N'В пути')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (4, 5, CAST(N'2025-11-19T14:15:00.000' AS DateTime), N'Создан')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (5, 2, CAST(N'2025-11-20T08:45:00.000' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (6, 4, CAST(N'2025-11-21T11:30:00.000' AS DateTime), N'В пути')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (7, 6, CAST(N'2025-11-22T09:10:00.000' AS DateTime), N'Создан')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (8, 7, CAST(N'2025-11-22T16:40:00.000' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (9, 8, CAST(N'2025-11-23T12:25:00.000' AS DateTime), N'В пути')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (10, 10, CAST(N'2025-11-24T10:00:00.000' AS DateTime), N'Создан')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (11, 9, CAST(N'2025-11-25T13:50:00.000' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (12, 1, CAST(N'2025-11-26T08:30:00.000' AS DateTime), N'В пути')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (13, 2, CAST(N'2025-11-27T15:20:00.000' AS DateTime), N'Создан')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (14, 4, CAST(N'2025-11-28T11:10:00.000' AS DateTime), N'Получен')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (15, 6, CAST(N'2025-11-29T09:45:00.000' AS DateTime), N'В пути')
GO
INSERT [dbo].[PurchaseOrders] ([PurchaseOrderID], [SupplierID], [OrderDate], [Status]) VALUES (16, 3, CAST(N'2025-11-30T14:00:00.000' AS DateTime), N'Создан')
GO
SET IDENTITY_INSERT [dbo].[PurchaseOrders] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (1, N'Администратор')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (3, N'Клиент')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (2, N'Менеджер')
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[StockMovements] ON 
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (1, 5, N'Расход', 2, CAST(N'2025-11-18T08:59:33.580' AS DateTime), N'Заказ №1')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (2, 1, N'Расход', 1, CAST(N'2025-11-18T08:59:42.930' AS DateTime), N'Заказ №2')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (3, 2, N'Расход', 3, CAST(N'2025-11-18T08:59:59.003' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (4, 3, N'Расход', 2, CAST(N'2025-11-18T08:59:59.003' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (5, 6, N'Расход', 2, CAST(N'2025-11-18T08:59:59.007' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (6, 4, N'Приход', 10, CAST(N'2025-11-18T09:00:49.027' AS DateTime), N'Закупка №1')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (7, 5, N'Расход', 2, CAST(N'2025-11-18T08:50:00.000' AS DateTime), N'Заказ №1')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (8, 1, N'Расход', 1, CAST(N'2025-11-18T08:51:00.000' AS DateTime), N'Заказ №2')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (9, 2, N'Расход', 3, CAST(N'2025-11-18T08:52:00.000' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (10, 3, N'Расход', 2, CAST(N'2025-11-18T08:53:00.000' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (11, 6, N'Расход', 2, CAST(N'2025-11-18T08:54:00.000' AS DateTime), N'Заказ №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (12, 4, N'Приход', 10, CAST(N'2025-11-18T09:00:00.000' AS DateTime), N'Закупка №1')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (13, 7, N'Расход', 1, CAST(N'2025-11-19T10:15:00.000' AS DateTime), N'Заказ №4')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (14, 12, N'Расход', 4, CAST(N'2025-11-19T11:20:00.000' AS DateTime), N'Заказ №5')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (15, 8, N'Приход', 50, CAST(N'2025-11-19T12:00:00.000' AS DateTime), N'Закупка №2')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (16, 15, N'Расход', 1, CAST(N'2025-11-19T14:30:00.000' AS DateTime), N'Заказ №7')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (17, 9, N'Расход', 3, CAST(N'2025-11-20T09:10:00.000' AS DateTime), N'Заказ №8')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (18, 4, N'Приход', 20, CAST(N'2025-11-20T10:00:00.000' AS DateTime), N'Закупка №3')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (19, 11, N'Расход', 1, CAST(N'2025-11-20T11:45:00.000' AS DateTime), N'Заказ №10')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (20, 13, N'Расход', 5, CAST(N'2025-11-21T08:20:00.000' AS DateTime), N'Заказ №11')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (21, 10, N'Расход', 1, CAST(N'2025-11-21T09:30:00.000' AS DateTime), N'Заказ №12')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (22, 14, N'Приход', 100, CAST(N'2025-11-21T12:00:00.000' AS DateTime), N'Закупка №4')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (23, 16, N'Расход', 1, CAST(N'2025-11-21T15:15:00.000' AS DateTime), N'Заказ №14')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (24, 17, N'Расход', 1, CAST(N'2025-11-22T10:10:00.000' AS DateTime), N'Заказ №15')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (25, 18, N'Расход', 3, CAST(N'2025-11-22T11:30:00.000' AS DateTime), N'Заказ №16')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (26, 19, N'Приход', 80, CAST(N'2025-11-22T14:00:00.000' AS DateTime), N'Закупка №5')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (27, 20, N'Расход', 1, CAST(N'2025-11-23T09:45:00.000' AS DateTime), N'Заказ №18')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (28, 21, N'Расход', 1, CAST(N'2025-11-23T11:00:00.000' AS DateTime), N'Заказ №1')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (29, 22, N'Приход', 40, CAST(N'2025-11-23T13:30:00.000' AS DateTime), N'Закупка №6')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (30, 23, N'Расход', 1, CAST(N'2025-11-24T08:40:00.000' AS DateTime), N'Заказ №4')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (31, 24, N'Приход', 60, CAST(N'2025-11-24T16:00:00.000' AS DateTime), N'Закупка №7')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (32, 4, N'Расход', 1, CAST(N'2026-01-24T16:53:01.090' AS DateTime), N'Заказ №19')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (33, 12, N'Расход', 1, CAST(N'2026-01-24T16:53:01.090' AS DateTime), N'Заказ №19')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (34, 6, N'Расход', 1, CAST(N'2026-03-12T15:06:49.467' AS DateTime), N'Заказ №20')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (35, 1, N'Расход', 2, CAST(N'2026-03-12T15:06:49.467' AS DateTime), N'Заказ №20')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (36, 21, N'Расход', 1, CAST(N'2026-03-12T15:07:34.480' AS DateTime), N'Заказ №21')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (37, 10, N'Расход', 1, CAST(N'2026-03-12T15:07:34.480' AS DateTime), N'Заказ №21')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (38, 16, N'Расход', 1, CAST(N'2026-03-12T15:07:34.480' AS DateTime), N'Заказ №21')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (39, 16, N'Расход', 1, CAST(N'2026-03-12T15:07:43.750' AS DateTime), N'Заказ №22')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (40, 10, N'Расход', 1, CAST(N'2026-03-12T15:07:43.750' AS DateTime), N'Заказ №22')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (41, 8, N'Расход', 1, CAST(N'2026-03-12T15:07:43.750' AS DateTime), N'Заказ №22')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (42, 31, N'Расход', 1, CAST(N'2026-03-12T15:07:43.750' AS DateTime), N'Заказ №22')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (43, 22, N'Расход', 1, CAST(N'2026-03-12T15:07:50.783' AS DateTime), N'Заказ №23')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (44, 6, N'Расход', 1, CAST(N'2026-03-12T15:07:57.473' AS DateTime), N'Заказ №24')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (45, 19, N'Расход', 1, CAST(N'2026-03-12T15:07:57.473' AS DateTime), N'Заказ №24')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (46, 18, N'Расход', 1, CAST(N'2026-03-12T15:08:13.617' AS DateTime), N'Заказ №25')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (47, 5, N'Расход', 1, CAST(N'2026-03-12T15:08:13.620' AS DateTime), N'Заказ №25')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (48, 14, N'Расход', 1, CAST(N'2026-03-12T15:08:13.620' AS DateTime), N'Заказ №25')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (49, 10, N'Расход', 1, CAST(N'2026-03-12T15:08:17.567' AS DateTime), N'Заказ №26')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (50, 31, N'Расход', 3, CAST(N'2026-03-12T15:08:25.470' AS DateTime), N'Заказ №27')
GO
INSERT [dbo].[StockMovements] ([MovementID], [ProductID], [MovementType], [Quantity], [MovementDate], [Reference]) VALUES (51, 30, N'Расход', 1, CAST(N'2026-03-12T15:08:25.470' AS DateTime), N'Заказ №27')
GO
SET IDENTITY_INSERT [dbo].[StockMovements] OFF
GO
SET IDENTITY_INSERT [dbo].[Suppliers] ON 
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (1, N'ООО "АвтоПоставка"', N'Наливкин Виталий', N'+70000000002', N'nalivkin@autopostavka.ru', N'Омск, ул. Добролюбрва, 15')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (2, N'ООО "АвтоДеталь"', N'Смирнов Андрей Петрович', N'79151234567', N'smirnov@avtodetal.ru', N'Москва, ул. Ленина, д. 15')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (3, N'ИП "Масло и фильтры"', N'Козлова Марина Игоревна', N'79039876543', N'kozlova@oil-filters.ru', N'Санкт-Петербург, пр. Невский, д. 42')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (4, N'ЗАО "АвтоТрейд"', N'Волков Денис Сергеевич', N'79267778899', N'volkov@autotrade.org', N'Екатеринбург, ул. Мира, д. 7')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (5, N'ООО "Кузовные запчасти"', N'Николаева Ольга Викторовна', N'79501231234', N'nikolaeva@bodyparts.ru', N'Казань, ул. Баумана, д. 23')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (6, N'ИП "Химия для авто"', N'Федоров Иван Михайлович', N'79162345678', N'fedorov@auto-him.ru', N'Новосибирск, ул. Кирова, д. 56')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (7, N'ООО "Светотехника"', N'Соколова Анна Дмитриевна', N'79055556677', N'sokolova@lightauto.ru', N'Ростов-на-Дону, ул. Садовая, д. 12')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (8, N'ЗАО "Тормозные системы"', N'Павлов Алексей Владимирович', N'79218889900', N'pavlov@brakesystem.ru', N'Уфа, ул. Октябрьская, д. 34')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (9, N'ООО "Аксессуары Плюс"', N'Григорьева Елена Александровна', N'79104567890', N'grigorieva@akses-plus.ru', N'Воронеж, ул. Пушкинская, д. 9')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (10, N'ИП "Масляный мир"', N'Белов Сергей Николаевич', N'79091234567', N'belov@oilworld.ru', N'Краснодар, ул. Красная, д. 100')
GO
INSERT [dbo].[Suppliers] ([SupplierID], [SupplierName], [ContactPerson], [Phone], [Email], [Address]) VALUES (11, N'ООО "АвтоЭксперт"', N'Морозова Татьяна Павловна', N'79233334455', N'morozova@autoexpert.ru', N'Самара, ул. Куйбышева, д. 77')
GO
SET IDENTITY_INSERT [dbo].[Suppliers] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [RoleID]) VALUES (1, N'admin', N'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Большой Босс', N'admin@autoshop.com', N'+70000000000', 1)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [RoleID]) VALUES (2, N'manager', N'6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17', N'Нормальный Менеджер', N'manager@autoshop.com', N'+70000000001', 2)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [RoleID]) VALUES (3, N'test', N'test', N'test', N'test@gmail.com', N'+79992232323', 3)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [RoleID]) VALUES (4, N'sss', N'sss', N'sss', N'sss@gmail.com', N'+79992223322', 2)
GO
INSERT [dbo].[Users] ([UserID], [Username], [Password], [FullName], [Email], [Phone], [RoleID]) VALUES (6, N'zzz', N'17f165d5a5ba695f27c023a83aa2b3463e23810e360b7517127e90161eebabda', N'zzz', N'zzz@gmail.com', N'+79992223322', 2)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Products__4943444AC79AE178]    Script Date: 24.03.2026 16:23:28 ******/
ALTER TABLE [dbo].[Products] ADD UNIQUE NONCLUSTERED 
(
	[Article] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__8A2B6160BC4C4799]    Script Date: 24.03.2026 16:23:28 ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4B8387A90]    Script Date: 24.03.2026 16:23:28 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('Новый') FOR [Status]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [StockQuantity]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [MinStockLevel]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[PurchaseOrders] ADD  DEFAULT ('Создан') FOR [Status]
GO
ALTER TABLE [dbo].[StockMovements] ADD  DEFAULT (getdate()) FOR [MovementDate]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([BrandID])
REFERENCES [dbo].[Brands] ([BrandID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
GO
ALTER TABLE [dbo].[PurchaseOrderDetails]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[PurchaseOrderDetails]  WITH CHECK ADD FOREIGN KEY([PurchaseOrderID])
REFERENCES [dbo].[PurchaseOrders] ([PurchaseOrderID])
GO
ALTER TABLE [dbo].[PurchaseOrders]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[StockMovements]  WITH CHECK ADD CHECK  (([MovementType]='Расход' OR [MovementType]='Приход'))
GO
USE [master]
GO
ALTER DATABASE [AutoParts] SET  READ_WRITE 
GO
