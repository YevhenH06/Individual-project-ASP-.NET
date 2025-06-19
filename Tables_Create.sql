CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Login NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL
);

CREATE TABLE Buyer (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    UserId INT UNIQUE,
    FOREIGN KEY (UserId) REFERENCES [User](Id)
);

CREATE TABLE Production (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    DisplayTime DATETIME NOT NULL,
    ExpiryTime DATETIME NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL
);

CREATE TABLE Discount (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DiscountName NVARCHAR(100) NOT NULL,
    DiscountValue DECIMAL(5,2) NOT NULL, 
    DiscountedPrice DECIMAL(10,2) NOT NULL, 
    ProductionId INT UNIQUE,
    FOREIGN KEY (ProductionId) REFERENCES Production(Id)
);

CREATE TABLE Basket (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    DiscountedPrice DECIMAL(10,2) NOT NULL,
    BuyerFirstName NVARCHAR(50) NOT NULL,
    BuyerLastName NVARCHAR(50) NOT NULL,
    BuyerId INT NOT NULL,
    DiscountId INT UNIQUE,
    FOREIGN KEY (BuyerId) REFERENCES Buyer(Id),
    FOREIGN KEY (DiscountId) REFERENCES Discount(Id)
);
