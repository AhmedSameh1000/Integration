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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE TABLE [dataBases] (
        [Id] int NOT NULL IDENTITY,
        [DbName] nvarchar(max) NOT NULL,
        [ConnectionString] nvarchar(max) NOT NULL,
        [dataBaseType] int NOT NULL,
        CONSTRAINT [PK_dataBases] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE TABLE [modules] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [priority] int NOT NULL,
        [TableFromName] nvarchar(max) NULL,
        [TableToName] nvarchar(max) NULL,
        [ToPrimaryKeyName] nvarchar(max) NULL,
        [fromPrimaryKeyName] nvarchar(max) NULL,
        [LocalIdName] nvarchar(max) NULL,
        [CloudIdName] nvarchar(max) NULL,
        [ToDbId] int NOT NULL,
        [FromDbId] int NOT NULL,
        [SyncType] int NOT NULL,
        [ToInsertFlagName] nvarchar(max) NOT NULL,
        [ToUpdateFlagName] nvarchar(max) NOT NULL,
        [FromInsertFlagName] nvarchar(max) NOT NULL,
        [FromUpdateFlagName] nvarchar(max) NOT NULL,
        [ToDeleteFlagName] nvarchar(max) NOT NULL,
        [FromDeleteFlagName] nvarchar(max) NOT NULL,
        [condition] nvarchar(max) NULL,
        [isDisabled] bit NOT NULL,
        CONSTRAINT [PK_modules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_modules_dataBases_FromDbId] FOREIGN KEY ([FromDbId]) REFERENCES [dataBases] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_modules_dataBases_ToDbId] FOREIGN KEY ([ToDbId]) REFERENCES [dataBases] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE TABLE [Operations] (
        [Id] int NOT NULL IDENTITY,
        [LocalId] nvarchar(max) NULL,
        [CloudId] nvarchar(max) NULL,
        [TableFrom] nvarchar(max) NULL,
        [TableTo] nvarchar(max) NULL,
        [PriceFrom] nvarchar(max) NULL,
        [PriceTo] nvarchar(max) NULL,
        [ItemId] nvarchar(max) NULL,
        [fromInsertFlag] nvarchar(max) NULL,
        [fromUpdateFlag] nvarchar(max) NULL,
        [fromDeleteFlag] nvarchar(max) NULL,
        [ToInsertFlag] nvarchar(max) NULL,
        [ToUpdateFlag] nvarchar(max) NULL,
        [ToDeleteFlag] nvarchar(max) NULL,
        [Condition] nvarchar(max) NULL,
        [fromPriceInsertDate] nvarchar(max) NULL,
        [customerId] nvarchar(max) NULL,
        [storeId] nvarchar(max) NULL,
        [OPToItemPrimary] nvarchar(max) NULL,
        [operationType] int NOT NULL,
        [ModuleId] int NOT NULL,
        CONSTRAINT [PK_Operations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Operations_modules_ModuleId] FOREIGN KEY ([ModuleId]) REFERENCES [modules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_columnFroms_ModuleId] ON [columnFroms] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_modules_FromDbId] ON [modules] ([FromDbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_modules_ToDbId] ON [modules] ([ToDbId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_Operations_ModuleId] ON [Operations] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    CREATE INDEX [IX_References_ModuleId] ON [References] ([ModuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20241211142707_Init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241211142707_Init', N'7.0.20');
END;
GO

COMMIT;
GO

