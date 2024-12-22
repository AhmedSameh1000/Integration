IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [dataBases] (
        [Id] int NOT NULL IDENTITY,
        [DbName] nvarchar(max) NOT NULL,
        [ConnectionString] nvarchar(max) NOT NULL,
        [IsLocal] bit NOT NULL,
        CONSTRAINT [PK_dataBases] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [modules] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [TableFromName] nvarchar(max) NOT NULL,
        [TableToName] nvarchar(max) NOT NULL,
        [ToPrimaryKeyName] nvarchar(max) NOT NULL,
        [fromPrimaryKeyName] nvarchar(max) NOT NULL,
        [LocalIdName] nvarchar(max) NULL,
        [CloudIdName] nvarchar(max) NULL,
        [ToDbId] int NOT NULL,
        [FromDbId] int NOT NULL,
        [SyncType] int NOT NULL,
        [ToInsertFlagName] nvarchar(max) NULL,
        [ToUpdateFlagName] nvarchar(max) NULL,
        [FromInsertFlagName] nvarchar(max) NULL,
        [FromUpdateFlagName] nvarchar(max) NULL,
        CONSTRAINT [PK_modules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_modules_dataBases_FromDbId] FOREIGN KEY ([FromDbId]) REFERENCES [dataBases] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_modules_dataBases_ToDbId] FOREIGN KEY ([ToDbId]) REFERENCES [dataBases] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [columnFroms] (
        [Id] int NOT NULL IDENTITY,
        [ColumnFromName] nvarchar(max) NOT NULL,
        [ColumnToName] nvarchar(max) NOT NULL,
        [ModuleId] int NOT NULL,
        [isReference] bit NOT NULL,
        [TableReferenceName] nvarchar(max) NULL,
        CONSTRAINT [PK_columnFroms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_columnFroms_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [modules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [conditionFroms] (
        [Id] int NOT NULL IDENTITY,
        [Operation] nvarchar(max) NOT NULL,
        [ModuleId] int NOT NULL,
        CONSTRAINT [PK_conditionFroms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_conditionFroms_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [modules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [ConditionTos] (
        [Id] int NOT NULL IDENTITY,
        [Operation] nvarchar(max) NOT NULL,
        [ModuleId] int NOT NULL,
        CONSTRAINT [PK_ConditionTos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ConditionTos_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [modules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE TABLE [References] (
        [Id] int NOT NULL IDENTITY,
        [TableFromName] nvarchar(max) NOT NULL,
        [LocalName] nvarchar(max) NOT NULL,
        [PrimaryName] nvarchar(max) NOT NULL,
        [ModuleId] int NOT NULL,
        CONSTRAINT [PK_References] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_References_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [modules] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_columnFroms_ModuleId] ON [columnFroms] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_conditionFroms_ModuleId] ON [conditionFroms] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_ConditionTos_ModuleId] ON [ConditionTos] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_modules_FromDbId] ON [modules] ([FromDbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_modules_ToDbId] ON [modules] ([ToDbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    CREATE INDEX [IX_References_ModuleId] ON [References] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241113135535_Init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241113135535_Init', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114100648_Types')
BEGIN
    ALTER TABLE [dataBases] ADD [dataBaseType] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241114100648_Types')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241114100648_Types', N'7.0.20');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241126114904_Removelocal')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dataBases]') AND [c].[name] = N'IsLocal');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [dataBases] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [dataBases] DROP COLUMN [IsLocal];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241126114904_Removelocal')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241126114904_Removelocal', N'7.0.20');
END;
GO

COMMIT;
GO

