using System.Data;
using Dapper;
namespace BaseTemplate.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Open();

        var initScript = @"
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
                    LastModified DATETIMEOFFSET NOT NULL,
                    LastModifiedBy NVARCHAR(100)
                );
            END

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
                    Reminder DATETIME,

                    Done BIT NOT NULL DEFAULT 0,

                    Created DATETIMEOFFSET NOT NULL,
                    CreatedBy NVARCHAR(100),
                    LastModified DATETIMEOFFSET NOT NULL,
                    LastModifiedBy NVARCHAR(100)
                );
            END
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
                    LastModified DATETIMEOFFSET NOT NULL,
                    LastModifiedBy NVARCHAR(100)
                );
            END

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
            
            ";

        // Execute SQL to create tables and other database objects
        dbConnection.Execute(initScript);
        dbConnection.Close();
    }
}

