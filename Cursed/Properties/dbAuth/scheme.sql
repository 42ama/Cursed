IF OBJECT_ID(N'dbo.UserAuth', N'U') IS NULL BEGIN 
CREATE TABLE UserAuth
  (
  	[Login] VARCHAR(39) NOT NULL, 
  	[PasswordHash] VARCHAR(48) NOT NULL

  	CONSTRAINT PK_UserAuth_Login_Clustered PRIMARY KEY CLUSTERED (Login)
  )
; END;

IF OBJECT_ID(N'dbo.Role', N'U') IS NULL BEGIN 
CREATE TABLE Role
  (
  	[Name] VARCHAR(50) NOT NULL

  	CONSTRAINT PK_Role_Name_Clustered PRIMARY KEY CLUSTERED (Name)
  )
; END;
IF db_id(N'pharmaceuticsAuth') IS NULL
BEGIN
CREATE DATABASE pharmaceuticsAuth
END;

IF OBJECT_ID(N'dbo.UserData', N'U') IS NULL BEGIN 
CREATE TABLE UserData
  (
  	[Login] VARCHAR(39) NOT NULL,
  	[RoleName] VARCHAR(50) NOT NULL

  	CONSTRAINT PK_UserData_Login_Clustered PRIMARY KEY CLUSTERED (Login),
  	CONSTRAINT FK_UserData_UserAuth_Login FOREIGN KEY (Login) REFERENCES UserAuth(Login),
  	CONSTRAINT FK_UserData_Role_RoleName FOREIGN KEY (RoleName) REFERENCES Role(Name)
  )
; END;


IF OBJECT_ID(N'dbo.LogRecord', N'U') IS NULL BEGIN 
CREATE TABLE LogRecord
  (
  	[Id] INT IDENTITY(1,1) NOT NULL,
  	[Record] VARCHAR(MAX) NOT NULL,
  	[UserLogin] VARCHAR(39) NOT NULL,
  	[UserIP] VARCHAR(15) NOT NULL,
  	[Date] DATETIME NOT NULL

  	CONSTRAINT PK_LogRecord_Id_Clustered PRIMARY KEY CLUSTERED (Id),
  	CONSTRAINT FK_LogRecord_UserData_Login FOREIGN KEY (UserLogin) REFERENCES UserData(Login),
  )
; END;