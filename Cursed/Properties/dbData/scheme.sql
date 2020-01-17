IF OBJECT_ID(N'dbo.ProductCatalog', N'U') IS NULL 
BEGIN 
CREATE TABLE ProductCatalog
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[Name] NVARCHAR(50) NOT NULL,
  	[Type] VARCHAR(16) NOT NULL,
  	[CAS] INT NOT NULL,
  	[LicenseRequired] BIT NULL,

  	CONSTRAINT PK_ProductCatalog_Id_Clustered PRIMARY KEY CLUSTERED (Id ASC)
  )
;

 END;


IF OBJECT_ID(N'dbo.Recipe', N'U') IS NULL BEGIN 
CREATE TABLE Recipe
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[Content] NVARCHAR(MAX) NOT NULL,
  	[TechApproval] BIT NULL,
  	[GovermentApproval] BIT NULL

  	CONSTRAINT PK_Recipe_Id_Clustered PRIMARY KEY CLUSTERED (Id)
  )
; END;

IF OBJECT_ID(N'dbo.RecipeInheritance', N'U') IS NULL BEGIN 
CREATE TABLE RecipeInheritance
  (
  	[ParentId] INT NOT NULL, 
  	[ChildId] INT NOT NULL

  	CONSTRAINT FK_RecipeInheritance_Recipe_PId FOREIGN KEY (ParentId) REFERENCES Recipe(Id),
  	CONSTRAINT FK_RecipeInheritance_Recipe_CId FOREIGN KEY (ChildId) REFERENCES Recipe(Id)
  )
; END;

IF OBJECT_ID(N'dbo.Facility', N'U') IS NULL BEGIN 
CREATE TABLE Facility
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[Name] NVARCHAR(50) NOT NULL,
  	[Latitude] DECIMAL(8,6) NULL,
  	[Longitude] DECIMAL(8,6) NULL,

  	CONSTRAINT PK_Facility_Id_Clustered PRIMARY KEY CLUSTERED (Id)
  )
; END;

IF OBJECT_ID(N'dbo.TechProcess', N'U') IS NULL BEGIN 
CREATE TABLE TechProcess
  (
  	[FacilityId] INT NOT NULL,
  	[RecipeId] INT NOT NULL,   	
  	[DayEfficiency] DECIMAL(9,2) NOT NULL

  	CONSTRAINT [CK_TechProcess_FacilityId_RecipeId] PRIMARY KEY CLUSTERED ([RecipeId] ASC, [FacilityId] ASC),
  	CONSTRAINT FK_TechProcess_Recipe_Id FOREIGN KEY (RecipeId) REFERENCES Recipe(Id),
  	CONSTRAINT FK_TechProcess_Facility_Id FOREIGN KEY (FacilityId) REFERENCES Facility(Id)
  )
; END;

IF OBJECT_ID(N'dbo.Company', N'U') IS NULL BEGIN 
CREATE TABLE Company
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[Name] NVARCHAR(50) NOT NULL

  	CONSTRAINT PK_Company_Id_Clustered PRIMARY KEY CLUSTERED (Id)
  )
; END;

IF OBJECT_ID(N'dbo.Storage', N'U') IS NULL BEGIN 
CREATE TABLE Storage
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[Name] NVARCHAR(50) NOT NULL,
  	[Latitude] DECIMAL(8,6) NULL,
  	[Longitude] DECIMAL(8,6) NULL,
	[CompanyId]	INT NOT NULL,

  	CONSTRAINT PK_Storage_Id_Clustered PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT FK_Storage_Company_Id FOREIGN KEY (CompanyId) REFERENCES Company(Id)
  )
; END;


IF OBJECT_ID(N'dbo.Product', N'U') IS NULL BEGIN 
CREATE TABLE Product
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[UId] INT NOT NULL, 
  	[Quantity] DECIMAL(9,2) NOT NULL,
  	[QuantityUnit] NVARCHAR(3) NOT NULL,
  	[Price] DECIMAL(9,2) NOT NULL,
  	[StorageId] INT NOT NULL

  	CONSTRAINT PK_Product_Id_Clustered PRIMARY KEY CLUSTERED (Id),
  	CONSTRAINT FK_Product_ProductCatalog_Id FOREIGN KEY (UId) REFERENCES ProductCatalog(Id),
  	CONSTRAINT FK_Product_Storage_Id FOREIGN KEY (StorageId) REFERENCES Storage(Id)
  )
; END;

IF OBJECT_ID(N'dbo.RecipeProductChanges', N'U') IS NULL BEGIN 
CREATE TABLE RecipeProductChanges
  (
  	[RecipeId] INT NOT NULL, 
  	[ProductId] INT NOT NULL,
  	[Type] VARCHAR(16) NOT NULL,
  	[Quantity] DECIMAL(9,2) NOT NULL

  	CONSTRAINT FK_RecipeProductChanges_Recipe_Id FOREIGN KEY (RecipeId) REFERENCES Recipe(Id),
  	CONSTRAINT FK_RecipeProductChanges_ProductCatalog_Id FOREIGN KEY (ProductId) REFERENCES ProductCatalog(Id),
  	CONSTRAINT CK_RecipeProductChanges_RecipeId_ProductId_Type PRIMARY KEY (RecipeId, ProductId, Type)
  )
; END;

IF OBJECT_ID(N'dbo.License', N'U') IS NULL BEGIN 
CREATE TABLE License
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[ProductId] INT NOT NULL, 
  	[GovermentNum] INT NOT NULL,
  	[Date] DATE NOT NULL

  	CONSTRAINT PK_License_Id_Clustered PRIMARY KEY CLUSTERED (Id),
  	CONSTRAINT FK_License_Product_Id FOREIGN KEY (ProductId) REFERENCES ProductCatalog(Id)
  )
; END;


IF OBJECT_ID(N'dbo.TransactionBatch', N'U') IS NULL BEGIN 
CREATE TABLE TransactionBatch
  (
  	[Id] INT IDENTITY(1,1) NOT NULL,
  	[CompanyId] INT NOT NULL, 
  	[Date] DATE NOT NULL,
  	[Type] VARCHAR(16) NOT NULL,
  	[IsOpen] BIT NOT NULL,
  	[Comment] NVARCHAR(MAX) NULL

  	CONSTRAINT PK_TransactionBatch_Id_Clustered PRIMARY KEY CLUSTERED (Id),
  	CONSTRAINT FK_Operation_Company_Id FOREIGN KEY (CompanyId) REFERENCES Company(Id)
  )
; END;


IF OBJECT_ID(N'dbo.Operation', N'U') IS NULL BEGIN 
CREATE TABLE Operation
  (
  	[Id] INT IDENTITY(1,1) NOT NULL, 
  	[ProductId] INT NOT NULL, 
  	[TransactionId] INT NOT NULL,
  	[Quantity] DECIMAL(9,2) NOT NULL,
  	[Price] DECIMAL(9,2) NOT NULL,
  	[StorageFromId] INT NOT NULL,
  	[StorageToId] INT NOT NULL

  	CONSTRAINT PK_Operation_Id_Clustered PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Operation_Product_Id FOREIGN KEY (ProductId) REFERENCES ProductCatalog(Id),
    CONSTRAINT FK_Operation_Transaction_Id FOREIGN KEY (TransactionId) REFERENCES TransactionBatch(Id),
    CONSTRAINT FK_Operation_StorageFrom_Id FOREIGN KEY (StorageFromId) REFERENCES Storage(Id),
    CONSTRAINT FK_Operation_StorageTo_Id FOREIGN KEY (StorageToId) REFERENCES Storage(Id)
  )
; END;