-- =============================================
-- Create Table: TodoList
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TodoList' AND xtype='U')
BEGIN
    CREATE TABLE TodoList (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(255),
        Colour NVARCHAR(7) NOT NULL DEFAULT '#FFFFFF',

        Created DATETIMEOFFSET NOT NULL,
        CreatedBy NVARCHAR(100),
        LastModified DATETIMEOFFSET,
        LastModifiedBy NVARCHAR(100)
    );
END
GO 

-- =============================================
-- Create Table: TodoItem
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TodoItem' AND xtype='U')
BEGIN
    CREATE TABLE TodoItem (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ListId INT NULL,
        Title NVARCHAR(255),
        Note NVARCHAR(MAX),
        Priority INT NOT NULL DEFAULT 0,
        Reminder DATETIMEOFFSET,

        Done BIT NOT NULL DEFAULT 0,

        Created DATETIMEOFFSET NOT NULL,
        CreatedBy NVARCHAR(100),
        LastModified DATETIMEOFFSET,
        LastModifiedBy NVARCHAR(100)
    );
END
GO

-- =============================================
-- Create Table: UserRole
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRole' AND xtype='U')
BEGIN
    CREATE TABLE UserRole (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId NVARCHAR(100),
        Role NVARCHAR(100),

        Created DATETIMEOFFSET NOT NULL,
        CreatedBy NVARCHAR(100),
        LastModified DATETIMEOFFSET,
        LastModifiedBy NVARCHAR(100)
    );
END
GO

-- =============================================
-- Create Table: DomainEvent
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DomainEvent' AND xtype='U')
BEGIN
    CREATE TABLE DomainEvent (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EventId UNIQUEIDENTIFIER NOT NULL,
        EventType NVARCHAR(255) NOT NULL,
        EventData NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        CreatedAt DATETIMEOFFSET NOT NULL,
        ProcessedAt DATETIMEOFFSET NULL,
        Result NVARCHAR(255) NULL,
        Created DATETIMEOFFSET NOT NULL,
        CreatedBy NVARCHAR(100),
        LastModified DATETIMEOFFSET,
        LastModifiedBy NVARCHAR(100)
    );
END
GO


IF NOT EXISTS (SELECT 1 FROM UserRole WHERE UserId = 'i53MEl0y7jOwIh0BvmHqd0PMDnf2' AND Role = 'Administrator')
BEGIN
    Insert into UserRole (UserId, Role, Created, CreatedBy) values ('i53MEl0y7jOwIh0BvmHqd0PMDnf2', 'Administrator', GETDATE(), 'SYSTEM');
END
GO