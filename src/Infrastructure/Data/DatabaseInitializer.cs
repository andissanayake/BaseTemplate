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
            ";

        // Execute SQL to create tables and other database objects
        dbConnection.Execute(initScript);
        dbConnection.Close();
    }
}

