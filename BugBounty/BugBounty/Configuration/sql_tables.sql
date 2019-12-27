CREATE TABLE [User](
	Id uniqueidentifier primary key NOT NULL,
    Email nvarchar(512) NOT NULL,
	DisplayName nvarchar(512),
	PlatformId int,
	UserRole int,
	IsActive bit NOT NULL,
	IsDeleted bit NOT NULL)
;

CREATE TABLE [UserRole](
	[Id] int IDENTITY(1,1) primary key NOT NULL, 
	[Role] nvarchar(100))
;

CREATE TABLE [Platform](
	[Id] int IDENTITY(1,1) primary key NOT NULL, 
	[Platform] nvarchar(100))
;

CREATE TABLE [Bug] (
	Id uniqueidentifier  primary key NOT NULL,
	Title nvarchar(512) NOT NULL,
	Description nvarchar(512),
	Image nvarchar(512),
	CreatedUserId uniqueidentifier,
	ValidateUserId uniqueidentifier,
	Severity int,
	PlatformId int,
	IsActive bit
);

ALTER TABLE [User] ADD CONSTRAINT [User_fk0] FOREIGN KEY ([PlatformId]) REFERENCES [Platform]([Id]);

ALTER TABLE [User] ADD CONSTRAINT [User_fk1] FOREIGN KEY ([UserRole]) REFERENCES [UserRole]([Id]);


ALTER TABLE [Bug] ADD CONSTRAINT [Bug_fk0] FOREIGN KEY ([CreatedUserId]) REFERENCES [User]([Id]);


ALTER TABLE [Bug] ADD CONSTRAINT [Bug_fk1] FOREIGN KEY ([ValidateUserId]) REFERENCES [User]([Id]);

ALTER TABLE [Bug] ADD CONSTRAINT [Bug_fk2] FOREIGN KEY ([PlatformId]) REFERENCES [Platform]([Id]);

INSERT into [UserRole](Role) values(N'Management')
;
INSERT into [UserRole](Role) values(N'PlatformManager')
;
INSERT into [UserRole](Role) values(N'Engineer')
;

INSERT into [Platform](Platform) values(N'Bold BI')
;
INSERT into [Platform](Platform) values(N'Bold Reports')
;
INSERT into [Platform](Platform) values(N'Data Integration Platform')
;
INSERT into [Platform](Platform) values(N'Xamarin.Forms')
;