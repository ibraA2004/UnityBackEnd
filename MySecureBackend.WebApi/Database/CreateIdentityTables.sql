-- =============================================================================
-- ASP.NET Core Identity Tables Creation Script for Azure SQL Database
-- Database: db2244960
-- Server: avansict2244960.database.windows.net
-- 
-- BELANGRIJK: Selecteer 'db2244960' in de database dropdown BOVEN in SSMS!
--             Azure SQL ondersteunt geen USE statement!
-- =============================================================================

-- Create Identity Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'auth')
BEGIN
    EXEC('CREATE SCHEMA [auth]')
END
GO

-- Create AspNetRoles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetRoles] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NULL,
        [NormalizedName] NVARCHAR(256) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [RoleNameIndex] ON [auth].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL
END
GO

-- Create AspNetUsers Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetUsers] (
        [Id] NVARCHAR(450) NOT NULL,
        [UserName] NVARCHAR(256) NULL,
        [NormalizedUserName] NVARCHAR(256) NULL,
        [Email] NVARCHAR(256) NULL,
        [NormalizedEmail] NVARCHAR(256) NULL,
        [EmailConfirmed] BIT NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [SecurityStamp] NVARCHAR(MAX) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [PhoneNumber] NVARCHAR(MAX) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL,
        [TwoFactorEnabled] BIT NOT NULL,
        [LockoutEnd] DATETIMEOFFSET(7) NULL,
        [LockoutEnabled] BIT NOT NULL,
        [AccessFailedCount] INT NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    )
    CREATE INDEX [EmailIndex] ON [auth].[AspNetUsers] ([NormalizedEmail])
    CREATE UNIQUE INDEX [UserNameIndex] ON [auth].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL
END
GO

-- Create AspNetRoleClaims Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoleClaims' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetRoleClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [auth].[AspNetRoleClaims] ([RoleId])
END
GO

-- Create AspNetUserClaims Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserClaims' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetUserClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [auth].[AspNetUserClaims] ([UserId])
END
GO

-- Create AspNetUserLogins Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserLogins' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetUserLogins] (
        [LoginProvider] NVARCHAR(450) NOT NULL,
        [ProviderKey] NVARCHAR(450) NOT NULL,
        [ProviderDisplayName] NVARCHAR(MAX) NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [auth].[AspNetUserLogins] ([UserId])
END
GO

-- Create AspNetUserRoles Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetUserRoles] (
        [UserId] NVARCHAR(450) NOT NULL,
        [RoleId] NVARCHAR(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [auth].[AspNetUserRoles] ([RoleId])
END
GO

-- Create AspNetUserTokens Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserTokens' AND schema_id = SCHEMA_ID('auth'))
BEGIN
    CREATE TABLE [auth].[AspNetUserTokens] (
        [UserId] NVARCHAR(450) NOT NULL,
        [LoginProvider] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(450) NOT NULL,
        [Value] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
END
GO

PRINT 'Identity tables created successfully!'
PRINT 'Schema: auth'
PRINT 'Tables: AspNetUsers, AspNetRoles, AspNetUserClaims, AspNetRoleClaims, AspNetUserLogins, AspNetUserRoles, AspNetUserTokens'
GO

-- =============================================================================
-- Game Data Tables for Unity 2D Game
-- =============================================================================

-- Create dbo schema for game tables (default schema)
-- dbo schema already exists by default in SQL Server

-- Create Environment2D Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Environment2D' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Environment2D] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [OwnerUserId] NVARCHAR(450) NOT NULL,
        [MaxLength] INT NOT NULL,
        [MaxHeight] INT NOT NULL,
        [BackgroundIndex] INT NOT NULL DEFAULT -1, -- -1 = geen background, 0-3 = specifieke backgrounds
        CONSTRAINT [PK_Environment2D] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Environment2D_AspNetUsers_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_Environment2D_OwnerUserId] ON [dbo].[Environment2D] ([OwnerUserId])
END
GO

-- Create Object2D Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Object2D' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Object2D] (
        [Id] NVARCHAR(450) NOT NULL,
        [EnvironmentId] NVARCHAR(450) NOT NULL,
        [PrefabId] NVARCHAR(256) NOT NULL,
        [PositionX] FLOAT NOT NULL,
        [PositionY] FLOAT NOT NULL,
        [ScaleX] FLOAT NOT NULL DEFAULT 1.0,
        [ScaleY] FLOAT NOT NULL DEFAULT 1.0,
        [RotationZ] FLOAT NOT NULL DEFAULT 0.0,
        [SortingLayer] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Object2D] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Object2D_Environment2D_EnvironmentId] FOREIGN KEY ([EnvironmentId]) REFERENCES [dbo].[Environment2D] ([Id]) ON DELETE CASCADE
    )
    CREATE INDEX [IX_Object2D_EnvironmentId] ON [dbo].[Object2D] ([EnvironmentId])
    CREATE INDEX [IX_Object2D_PrefabId] ON [dbo].[Object2D] ([PrefabId])
END
GO

PRINT 'Game data tables created successfully!'
PRINT 'Tables: Environment2D, Object2D'
PRINT ''
PRINT '✅ All tables created! Your database is ready to use.'
