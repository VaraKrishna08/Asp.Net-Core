--Create Product
CREATE TABLE Product (
    ProductID INT NOT NULL IDENTITY(1,1),
    ProductName VARCHAR(100) NOT NULL,
    ProductPrice DECIMAL(10,2) NOT NULL,
    ProductCode VARCHAR(100) NOT NULL,
    Description VARCHAR(100) NOT NULL,
    UserID INT NOT NULL,
    PRIMARY KEY (ProductID),
    FOREIGN KEY (UserID) REFERENCES UserDemo(UserID)
);

--Insert Product
INSERT INTO Product (ProductName, ProductPrice, ProductCode, Description, UserID) VALUES
('Laptop',999.99,'LP1234','PowerfulLaptop',1),
('Smartphone',799.00,'SM5678','LatestSmartphone',2),
('Headphones',99.99,'HP3456','WirelessHeadphones',3),
('Smartwatch',299.99,'SW7890','FitnessTracker',4),
('GamingConsole',499.99,'GC1234','Next-genConsole',5);

SELECT * FROM Product;

--Create User
CREATE TABLE UserDemo (
    UserID INT NOT NULL IDENTITY(1,1),
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Password VARCHAR(100) NOT NULL,
    MobileNo VARCHAR(15) NOT NULL,
    Address VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL,
    PRIMARY KEY (UserID)
);

--Insert User
INSERT INTO UserDemo(UserName, Email, Password, MobileNo, Address, IsActive) VALUES
('User1', 'user1@example.com', 'hashed_password1', '1234567890', 'Address 1', 1),
('User2', 'user2@example.com', 'hashed_password2', '9876543210', 'Address 2', 1),
('User3', 'user3@example.com', 'hashed_password3', '1234567891', 'Address 3', 0),
('User4', 'user4@example.com', 'hashed_password4', '9876543211', 'Address 4', 1),
('User5', 'user5@example.com', 'hashed_password5', '1234567892', 'Address 5', 1);

SELECT * FROM UserDemo;

--Create Order
CREATE TABLE OrderDemo(
    OrderID INT NOT NULL IDENTITY(1,1),
    OrderDate DATETIME NOT NULL,
	OrderNumber VARCHAR(10) NOT  NULL,
    CustomerID INT NOT NULL,
    PaymentMode VARCHAR(100) NULL,
    TotalAmount DECIMAL(10,2) NULL,
    ShippingAddress VARCHAR(100) NOT NULL,
    UserID INT NOT NULL,
    PRIMARY KEY (OrderID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (UserID) REFERENCES UserDemo(UserID)
);

--Insert Order
INSERT INTO OrderDemo(OrderDate,OrderNumber, CustomerID, PaymentMode, TotalAmount, ShippingAddress, UserID) VALUES
('2023-11-25 12:34:56','BN1', 1, 'Credit Card', 99.99, '123 Main St', 1),
('2023-12-01 15:00:00','BN2', 2, 'Cash on Delivery', 199.99, '456 Elm St', 2),
('2023-12-10 09:30:00','BN3', 3, 'Debit Card', 299.99, '789 Oak St', 3),
('2023-12-15 11:15:00', 'BN4',4, 'UPI', 499.99, '101 Pine St', 4),
('2023-12-20 14:45:00', 'BN5' ,5,'Net Banking', 599.99, '202 Cedar St', 5);

SELECT * FROM OrderDemo;

--Create OrderDetail
CREATE TABLE OrderDetail (
    OrderDetailID INT NOT NULL IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    UserID INT NOT NULL,
    PRIMARY KEY (OrderDetailID),
    FOREIGN KEY (OrderID) REFERENCES OrderDemo(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    FOREIGN KEY (UserID) REFERENCES UserDemo(UserID)
);

--Insert OrderDetail
INSERT INTO OrderDetail (OrderID, ProductID, Quantity, Amount, TotalAmount, UserID) VALUES
(1, 1, 2, 19.99, 39.98, 1),
(2, 2, 1, 29.99, 29.99, 2),
(3, 3, 3, 39.99, 119.97, 3),
(4, 4, 4, 49.99, 199.96, 4),
(5, 5, 5, 59.99, 299.95, 5);

SELECT * FROM OrderDetail;


--Create Bill
CREATE TABLE Bills (
    BillID INT NOT NULL IDENTITY(1,1),
    BillNumber VARCHAR(100) NOT NULL,
    BillDate DATETIME NOT NULL,
    OrderID INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Discount DECIMAL(10,2) NULL,
    NetAmount DECIMAL(10,2) NOT NULL,
    UserID INT NOT NULL,
    PRIMARY KEY (BillID),
    FOREIGN KEY (OrderID) REFERENCES OrderDemo(OrderID),
    FOREIGN KEY (UserID) REFERENCES UserDemo(UserID)
);

--Insert Bill
INSERT INTO Bills (BillNumber, BillDate, OrderID, TotalAmount, Discount, NetAmount, UserID) VALUES
('BILL-001', '2023-11-25 12:00:00', 1, 500.00, 50.00, 450.00, 1),
('BILL-002', '2023-11-26 15:30:00', 2, 300.00, 20.00, 280.00, 2),
('BILL-003', '2023-11-27 10:15:00', 3, 700.00, NULL, 700.00, 3),
('BILL-004', '2023-11-28 18:45:00', 4, 400.00, 30.00, 370.00, 4),
('BILL-005', '2023-11-29 12:30:00', 5, 600.00, NULL, 600.00, 5);

SELECT * FROM Bills;

--Create Customer
CREATE TABLE Customer (
    CustomerID INT NOT NULL IDENTITY(1,1),
    CustomerName VARCHAR(100) NOT NULL,
    HomeAddress VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    MobileNo VARCHAR(15) NOT NULL,
    GST_NO VARCHAR(15) NOT NULL,
    CityName VARCHAR(100) NOT NULL,
    PinCode VARCHAR(15) NOT NULL,
    NetAmount DECIMAL(10,2) NOT NULL,
    UserID INT NOT NULL,
    PRIMARY KEY (CustomerID),
    FOREIGN KEY (UserID) REFERENCES UserDemo(UserID)
);

--Insert Customer
INSERT INTO Customer (CustomerName, HomeAddress, Email, MobileNo, GST_NO, CityName, PinCode, NetAmount, UserID) VALUES
('Customer 1', 'Address 1', 'customer1@example.com', '1234567890', 'GST12345678912', 'City 1', '123456', 0.00, 1),
('Customer 2', 'Address 2', 'customer2@example.com', '9876543210', 'GST98765432101', 'City 2', '234567', 0.00, 2),
('Customer 3', 'Address 3', 'customer3@example.com', '1234567891', 'GST12345678913', 'City 3', '345678', 0.00, 3),
('Customer 4', 'Address 4', 'customer4@example.com', '9876543211', 'GST98765432102', 'City 4', '456789', 0.00, 4),
('Customer 5', 'Address 5', 'customer5@example.com', '1234567892', 'GST12345678914', 'City 5', '567890', 0.00, 5);

SELECT * FROM Customer;


--Select All Product
CREATE PROC [dbo].[PR_PRODUCT_SELECTALL]
AS
BEGIN
	SELECT
			[dbo].[Product].[ProductID],
			[dbo].[Product].[ProductName],
			[dbo].[Product].[ProductPrice],
			[dbo].[Product].[ProductCode],
			[dbo].[Product].[Description],
			[dbo].[UserDemo].[UserID]
		FROM
			[dbo].[Product]
		INNER JOIN [dbo].[UserDemo]
		ON [dbo].[Product].[UserID]=[dbo].[UserDemo].[UserID]
		ORDER BY [dbo].[Product].[ProductName],
				 [dbo].[Product].[ProductPrice],
				 [dbo].[Product].[ProductCode],
				 [dbo].[Product].[Description]
END;
EXEC [dbo].[PR_PRODUCT_SELECTALL]

--Select by PK Product
CREATE PROC [dbo].[PR_PRODUCT_SELECTBYPK]
@ProductID INT
AS
BEGIN
	SELECT
		[dbo].[Product].[ProductID],
		[dbo].[Product].[ProductName],
		[dbo].[Product].[ProductPrice],
		[dbo].[Product].[ProductCode],
		[dbo].[Product].[Description],
		[dbo].[Product].[UserID]
	FROM 
		[dbo].[Product]
	WHERE 
		[dbo].[Product].[ProductID] = @ProductID
END;
EXEC [dbo].[PR_PRODUCT_SELECTBYPK] 1

--Insert Product
CREATE PROC [dbo].[PR_PRODUCT_INSERT]
@ProductName VARCHAR(100),
@ProductPrice DECIMAL(10,2),
@ProductCode VARCHAR(100),
@Description VARCHAR(100),
@UserID INT
AS
BEGIN
	INSERT INTO
		[dbo].[Product] 
		(
			[ProductName],
			[ProductPrice],
			[ProductCode],
			[Description],
			[UserID]
		)
		VALUES
		(
			@ProductName,
			@ProductPrice,
			@ProductCode,
			@Description,
			@UserID
		)
END;
EXEC [dbo].[PR_PRODUCT_INSERT] 'Product F',19.99,'PRA006','Description for Product F',2
EXEC [dbo].[PR_PRODUCT_SELECTALL]

--Update Product
CREATE PROC [dbo].[PR_PRODUCT_UPDATE]
@ProductID INT,
@ProductName VARCHAR(100),
@ProductPrice DECIMAL(10,2),
@ProductCode VARCHAR(50),
@Description VARCHAR(255),
@UserID INT
AS
BEGIN
    UPDATE [dbo].[Product]
    SET 
        [ProductName] = @ProductName,		
        [ProductPrice] = @ProductPrice,
        [ProductCode] = @ProductCode,
        [Description] = @Description,
        [UserID] = @UserID
    WHERE 
        [ProductID] = @ProductID;
END;
EXEC [dbo].[PR_PRODUCT_UPDATE] 6,'Product F', 29.99, 'PRA006', 'Description for Product F', 2
EXEC [dbo].[PR_PRODUCT_SELECTALL]

--Delete Product
CREATE PROC [dbo].[PR_PRODUCT_DELETE]
@ProductID INT
AS
DELETE FROM		
	[dbo].[Product]
WHERE 
	[dbo].[Product].[ProductID] = @ProductID
EXEC [dbo].[PR_PRODUCT_DELETE] 6
EXEC [dbo].[PR_PRODUCT_SELECTALL]

CREATE PROCEDURE [dbo].[PR_PRODUCT_DROPDOWN]
AS
BEGIN
    SELECT
		[dbo].[Product].[ProductID],
        [dbo].[Product].[ProductName]
    FROM
        [dbo].[Product]
END;



--Select All User
CREATE PROC [dbo].[PR_USERDEMO_SELECTALL]
AS
BEGIN
	SELECT
			[dbo].[UserDemo].[UserID],
			[dbo].[UserDemo].[UserName],
			[dbo].[UserDemo].[Email],
			[dbo].[UserDemo].[Password],
			[dbo].[UserDemo].[MobileNo],
			[dbo].[UserDemo].[Address],
			[dbo].[UserDemo].[IsActive]
		FROM
			[dbo].[UserDemo]
END;
EXEC [dbo].[PR_USERDEMO_SELECTALL]

--Select by PK User
CREATE PROC [dbo].[PR_USERDEMO_SELECTBYPK]
@UserID INT
AS
BEGIN
	SELECT
		[dbo].[UserDemo].[UserID],
		[dbo].[UserDemo].[UserName],
		[dbo].[UserDemo].[Email],
		[dbo].[UserDemo].[Password],
		[dbo].[UserDemo].[MobileNo],
		[dbo].[UserDemo].[Address],
		[dbo].[UserDemo].[IsActive]
	FROM 
		[dbo].[UserDemo]
	WHERE 
		[dbo].[UserDemo].[UserID] = @UserID
END;
EXEC [dbo].[PR_USERDEMO_SELECTBYPK] 1

--Insert User
CREATE PROC [dbo].[PR_USERDEMO_INSERT]
@UserName VARCHAR(100),
@Email VARCHAR(100),
@Password VARCHAR(100),
@MobileNo VARCHAR(15),
@Address VARCHAR(100),
@IsActive BIT
AS
BEGIN
	INSERT INTO
		[dbo].[UserDemo] 
		(
			[UserName],
			[Email],
			[Password],
			[MobileNo],
			[Address],
			[IsActive]
		)
		VALUES
		(
			@UserName,
			@Email,
			@Password,
			@MobileNo,
			@Address,
			@IsActive
		)
END;
EXEC [dbo].[PR_USERDEMO_INSERT] 'Daisy Miller','daisy.miller@example.com','daisypass','6789012345','987 Birch St',1
EXEC [dbo].[PR_USERDEMO_SELECTALL]

--Update User
CREATE PROC [dbo].[PR_USERDEMO_UPDATE]
@UserID INT,
@UserName VARCHAR(100),
@Email VARCHAR(100),
@Password VARCHAR(100),
@MobileNo VARCHAR(15),
@Address VARCHAR(100),
@IsActive BIT
AS
BEGIN
    UPDATE [dbo].[UserDemo]
    SET 
        [UserName] = @UserName,		
        [Email] = @Email,
        [Password] = @Password,
        [MobileNo] = @MobileNo,
        [Address] = @Address,
		[IsActive] = @IsActive
    WHERE 
        [UserID] = @UserID;
END;
EXEC [dbo].[PR_USERDEMO_UPDATE] 2,'Jane Smith','jane.smith@example.com','password123','987-654-3210','456 Elm St. Othertown, USA',1
EXEC [dbo].[PR_USERDEMO_SELECTALL]

--Delete User
CREATE PROC [dbo].[PR_USERDEMO_DELETE]
@UserID INT
AS
DELETE FROM		
	[dbo].[UserDemo]
WHERE 
	[dbo].[UserDemo].[UserID] = @UserID
EXEC [dbo].[PR_USERDEMO_DELETE] 6
EXEC [dbo].[PR_USERDEMO_SELECTALL]

--Dropdown User
CREATE PROCEDURE [dbo].[PR_USERDEMO_DROPDOWN]
AS
BEGIN
    SELECT
		[dbo].[UserDemo].[UserID],
        [dbo].[UserDemo].[UserName]
    FROM
        [dbo].[UserDemo]
END;
select *from OrderDemo
--Select All Order
CREATE PROC [dbo].[PR_ORDERDEMO_SELECTALL]
AS
BEGIN
    SELECT
        [dbo].[OrderDemo].[OrderID],             
        [dbo].[OrderDemo].[OrderDate],           
        [dbo].[OrderDemo].[OrderNumber],         
        [dbo].[Customer].[CustomerID],           
        [dbo].[OrderDemo].[PaymentMode],         
        [dbo].[OrderDemo].[TotalAmount],         
        [dbo].[OrderDemo].[ShippingAddress],     
        [dbo].[UserDemo].[UserID]                
    FROM
        [dbo].[OrderDemo]
    INNER JOIN [dbo].[UserDemo]
        ON [dbo].[OrderDemo].[UserID] = [dbo].[UserDemo].[UserID]  
    INNER JOIN [dbo].[Customer]
        ON [dbo].[OrderDemo].[CustomerID] = [dbo].[Customer].[CustomerID]  
    ORDER BY 
        [dbo].[OrderDemo].[OrderDate],           
        [dbo].[OrderDemo].[OrderNumber],         
        [dbo].[OrderDemo].[PaymentMode],         
        [dbo].[OrderDemo].[TotalAmount],         
        [dbo].[OrderDemo].[ShippingAddress];     
END;
EXEC [dbo].[PR_ORDERDEMO_SELECTALL]

--Select by PK Order
CREATE PROC [dbo].[PR_ORDERDEMO_SELECTBYPK]
@OrderID INT
AS
BEGIN
	SELECT
		[dbo].[OrderDemo].[OrderID],
		[dbo].[OrderDemo].[OrderDate],
		[dbo].[OrderDemo].[OrderNumber],
		[dbo].[OrderDemo].[CustomerID],
		[dbo].[OrderDemo].[PaymentMode],
		[dbo].[OrderDemo].[TotalAmount],
		[dbo].[OrderDemo].[ShippingAddress],
		[dbo].[OrderDemo].[UserID]
	FROM 
		[dbo].[OrderDemo]
	WHERE 
		[dbo].[OrderDemo].[OrderID] = @OrderID
END;
EXEC [dbo].[PR_ORDERDEMO_SELECTBYPK] 5

--Insert Order
CREATE PROC [dbo].[PR_ORDERDEMO_INSERT]
@OrderDate DATE,
@OrderNumber VARCHAR(20),
@CustomerID INT,
@PaymentMode VARCHAR(100),
@TotalAmount DECIMAL(10,2),
@ShippingAddress VARCHAR(100),
@UserID INT
AS
BEGIN
	INSERT INTO
		[dbo].[OrderDemo] 
		(
			[OrderDate],
			[OrderNumber],
			[CustomerID],
			[PaymentMode],
			[TotalAmount],
			[ShippingAddress],
			[UserID]
		)
		VALUES
		(
			GETDATE(),
			@OrderNumber,
			@CustomerID,
			@PaymentMode,
			@TotalAmount,
			@ShippingAddress,
			@UserID
		)
END;
EXEC [dbo].[PR_ORDERDEMO_INSERT] '2024-09-11','ORD1007',1,'Cash',500.00,'101 Adventure Rd',1;                     
EXEC [dbo].[PR_ORDERDEMO_SELECTALL]

--Update Order
CREATE PROC [dbo].[PR_ORDERDEMO_UPDATE]
    @OrderID INT,
    @OrderDate DATETIME,
    @OrderNumber VARCHAR(20),
    @CustomerID INT,
    @PaymentMode VARCHAR(100),
    @TotalAmount DECIMAL(10,2),
    @ShippingAddress VARCHAR(100),
    @UserID INT
AS
BEGIN
    UPDATE [dbo].[OrderDemo]
    SET 
        [OrderDate] = @OrderDate,  
        [OrderNumber] = @OrderNumber,
        [CustomerID] = @CustomerID,
        [PaymentMode] = @PaymentMode,
        [TotalAmount] = @TotalAmount,
        [ShippingAddress] = @ShippingAddress,
        [UserID] = @UserID
    WHERE 
        [OrderID] = @OrderID;
END;
EXEC [dbo].[PR_ORDERDEMO_UPDATE] 4,'2024-09-11 15:00:00','BN123',1,'Online',600.00,'102 Adventure Rd',2;      
EXEC [dbo].[PR_ORDERDEMO_SELECTALL]

--Delete Order
CREATE PROC [dbo].[PR_ORDERDEMO_DELETE]
@OrderID INT
AS
DELETE FROM		
	[dbo].[OrderDemo]
WHERE 
	[dbo].[OrderDemo].[OrderID] = @OrderID
EXEC [dbo].[PR_ORDERDEMO_DELETE] 7
EXEC [dbo].[PR_ORDERDEMO_SELECTALL]
--Select All Order detail
create PROC [dbo].[PR_ORDERDETAIL_SELECTALL]
AS
BEGIN
	SELECT
			[dbo].[OrderDetail].[OrderDetailID],
			[dbo].[OrderDemo].[OrderID],
			[dbo].[Product].[ProductID],
			[dbo].[OrderDetail].[Quantity],
			[dbo].[OrderDetail].[Amount],
			[dbo].[OrderDetail].[TotalAmount],
			[dbo].[UserDemo].[UserID]
		FROM
			[dbo].[OrderDetail]
		INNER JOIN [dbo].[OrderDemo]
		ON [dbo].[OrderDetail].[OrderID]=[dbo].[OrderDemo].[OrderID]
		INNER JOIN [dbo].[Product]
		ON [dbo].[OrderDetail].[ProductID]=[dbo].[Product].[ProductID]
		INNER JOIN [dbo].[UserDemo]
		ON [dbo].[OrderDetail].[UserID]=[dbo].[UserDemo].[UserID]
		ORDER BY [dbo].[OrderDetail].[Quantity],
				 [dbo].[OrderDetail].[Amount],
				 [dbo].[OrderDetail].[TotalAmount]
END;
EXEC [dbo].[PR_ORDERDETAIL_SELECTALL]

--Select by PK OrderDetail
CREATE PROC [dbo].[PR_ORDERDETAIL_SELECTBYPK]
@OrderDetailID INT
AS
BEGIN
	SELECT
		[dbo].[OrderDetail].[OrderDetailID],
		[dbo].[OrderDetail].[OrderID],
		[dbo].[OrderDetail].[ProductID],
		[dbo].[OrderDetail].[Quantity],
		[dbo].[OrderDetail].[Amount],
		[dbo].[OrderDetail].[TotalAmount],
		[dbo].[OrderDetail].[UserID]
	FROM 
		[dbo].[OrderDetail]
	WHERE 
		[dbo].[OrderDetail].[OrderDetailID] = @OrderDetailID
END;
EXEC [dbo].[PR_ORDERDETAIL_SELECTBYPK] 1

--Insert OrderDetail
CREATE PROC [dbo].[PR_ORDERDETAIL_INSERT]
@OrderID INT,
@ProductID INT,
@Quantity INT,
@Amount DECIMAL(10,2),
@TotalAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
	INSERT INTO
		[dbo].[OrderDetail] 
		(
			[OrderID],
			[ProductID],
			[Quantity],
			[Amount],
			[TotalAmount],
			[UserID]
		)
		VALUES
		(
			@OrderID,
			@ProductID,
			@Quantity,
			@Amount,
			@TotalAmount,
			@UserID
		)
END;
EXEC [dbo].[PR_ORDERDETAIL_INSERT] 3,3,2,19.99,39.98,2
EXEC [dbo].[PR_ORDERDETAIL_SELECTALL]

--Update OrderDetail
CREATE PROC [dbo].[PR_ORDERDETAIL_UPDATE]
@OrderDetailID INT,
@OrderID INT,
@ProductID INT,
@Quantity DECIMAL(10,2),
@Amount DECIMAL(10,2),
@TotalAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
    UPDATE [dbo].[OrderDetail]
    SET 		
        [OrderID] = @OrderID,
        [ProductID] = @ProductID,
        [Quantity] = @Quantity,
		[Amount] = @Amount,
        [TotalAmount] = @TotalAmount,
		[UserID] = @UserID
    WHERE 
        [OrderDetailID] = @OrderDetailID;
END;
EXEC [dbo].[PR_ORDERDETAIL_UPDATE] 5,3,2,2,29.99,49.98,2
EXEC [dbo].[PR_ORDERDETAIL_SELECTALL]

--Delete OrderDetail
CREATE PROC [dbo].[PR_ORDERDETAIL_DELETE]
@OrderDetailID INT
AS
DELETE FROM		
	[dbo].[OrderDetail]
WHERE 
	[dbo].[OrderDetail].[OrderDetailID] = @OrderDetailID
EXEC [dbo].[PR_ORDERDETAIL_DELETE]8
EXEC [dbo].[PR_ORDERDETAIL_SELECTALL]

--Select All Bills
CREATE PROC [dbo].[PR_BILLS_SELECTALL]
AS
BEGIN
	SELECT
			[dbo].[Bills].[BillID],
			[dbo].[Bills].[BillNumber],
			[dbo].[Bills].[BillDate],
			[dbo].[OrderDemo].[OrderID],
			[dbo].[Bills].[TotalAmount],
			[dbo].[Bills].[Discount],
			[dbo].[Bills].[NetAmount],
			[dbo].[UserDemo].[UserID]
		FROM
			[dbo].[Bills]
		INNER JOIN [dbo].[OrderDemo]
		ON [dbo].[Bills].[OrderID]=[dbo].[OrderDemo].[OrderID]
		INNER JOIN [dbo].[UserDemo]
		ON [dbo].[Bills].[UserID]=[dbo].[UserDemo].[UserID]
		ORDER BY [dbo].[Bills].[BillNumber],
				 [dbo].[Bills].[BillDate],
				 [dbo].[Bills].[TotalAmount],
				 [dbo].[Bills].[Discount],
				 [dbo].[Bills].[NetAmount]
END;
EXEC [dbo].[PR_BILLS_SELECTALL]

--Select by PK Bills
CREATE PROC [dbo].[PR_BILLS_SELECTBYPK]
@BillID INT
AS
BEGIN
	SELECT
		[dbo].[Bills].[BillID],
		[dbo].[Bills].[BillNumber],
		[dbo].[Bills].[BillDate],
		[dbo].[Bills].[OrderID],
		[dbo].[Bills].[TotalAmount],
		[dbo].[Bills].[Discount],
		[dbo].[Bills].[NetAmount],
		[dbo].[Bills].[UserID]
	FROM 
		[dbo].[Bills]
	WHERE 
		[dbo].[Bills].[BillID] = @BillID
END;
EXEC [dbo].[PR_BILLS_SELECTBYPK] 3

--Insert Bills
CREATE PROC [dbo].[PR_BILLS_INSERT]
@BillNumber VARCHAR(100),
@OrderID INT,
@TotalAmount DECIMAL(10,2),
@Discount DECIMAL(10,2),
@NetAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
	INSERT INTO
		[dbo].[Bills] 
		(
			[BillNumber],
			[BillDate],
			[OrderID],
			[TotalAmount],
			[Discount],
			[NetAmount],
			[UserID]
		)
		VALUES
		(
			@BillNumber,
			GETDATE(),
			@OrderID,
			@TotalAmount,
			@Discount,
			@NetAmount,
			@UserID
		)
END;
EXEC [dbo].[PR_BILLS_INSERT] 'BILL006',2,400.00,10.00,390.00,1
EXEC [dbo].[PR_BILLS_SELECTALL]

--Update Bills
CREATE PROC [dbo].[PR_BILLS_UPDATE]
@BillID INT,
@BillNumber VARCHAR(100),
@BillDate DATETIME,
@OrderID INT,
@TotalAmount DECIMAL(10,2),
@Discount DECIMAL(10,2),
@NetAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
    UPDATE [dbo].[Bills]
    SET 		
        [BillNumber] = @BillNumber,
        [BillDate] = GETDATE(),
        [OrderID] = @OrderID,
		[TotalAmount] = @TotalAmount,
        [Discount] = @Discount,
		[NetAmount] = @NetAmount,
		[UserID] = @UserID
    WHERE 
        [BillID] = @BillID
END;
EXEC [dbo].[PR_BILLS_UPDATE] 2,'BILL002','',2,300.00,20.00,280.00,1
EXEC [dbo].[PR_BILLS_SELECTALL]

--Delete Bills
CREATE PROC [dbo].[PR_BILLS_DELETE]
@BillID INT
AS
DELETE FROM		
	[dbo].[Bills]
WHERE 
	[dbo].[Bills].[BillID] = @BillID
EXEC [dbo].[PR_BILLS_DELETE] 4
EXEC [dbo].[PR_BILLS_SELECTALL]

--Select All Customer
create PROC [dbo].[PR_CUSTOMER_SELECTALL]
AS
BEGIN
	SELECT
			[dbo].[Customer].[CustomerID],
			[dbo].[Customer].[CustomerName],
			[dbo].[Customer].[HomeAddress],
			[dbo].[Customer].[Email],
			[dbo].[Customer].[MobileNo],
			[dbo].[Customer].[GST_NO],
			[dbo].[Customer].[CityName],
			[dbo].[Customer].[PinCode],
			[dbo].[Customer].[NetAmount],
			[dbo].[UserDemo].[UserID]
		FROM
			[dbo].[Customer]
		INNER JOIN [dbo].[UserDemo]
		ON [dbo].[Customer].[UserID]=[dbo].[UserDemo].[UserID]
		ORDER BY [dbo].[Customer].[CustomerName],
				 [dbo].[Customer].[HomeAddress],
				 [dbo].[Customer].[Email],
				 [dbo].[Customer].[MobileNo],
				 [dbo].[Customer].[GST_NO],
				 [dbo].[Customer].[CityName],
				 [dbo].[Customer].[PinCode],
				 [dbo].[Customer].[NetAmount]
END;
EXEC [dbo].[PR_CUSTOMER_SELECTALL]

--Select by PK Customer
CREATE PROC [dbo].[PR_CUSTOMER_SELECTBYPK]
@CustomerID INT
AS
BEGIN
	SELECT
		[dbo].[Customer].[CustomerID],
		[dbo].[Customer].[CustomerName],
		[dbo].[Customer].[HomeAddress],
		[dbo].[Customer].[Email],
		[dbo].[Customer].[MobileNo],
		[dbo].[Customer].[GST_NO],
		[dbo].[Customer].[CityName],
		[dbo].[Customer].[PinCode],
		[dbo].[Customer].[NetAmount],
		[dbo].[Customer].[UserID]
	FROM 
		[dbo].[Customer]
	WHERE 
		[dbo].[Customer].[CustomerID] = @CustomerID
END;
EXEC [dbo].[PR_CUSTOMER_SELECTBYPK] 1

--Insert Customer
CREATE PROC [dbo].[PR_CUSTOMER_INSERT]
@CustomerName VARCHAR(100),
@HomeAddress VARCHAR(100),
@Email VARCHAR(100),
@MobileNo VARCHAR(15),
@GST_NO VARCHAR(15),
@CityName VARCHAR(100),
@PinCode VARCHAR(100),
@NetAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
	INSERT INTO
		[dbo].[Customer] 
		(
			[CustomerName],
			[HomeAddress],
			[Email],
			[MobileNo],
			[GST_NO],
			[CityName],
			[PinCode],
			[NetAmount],
			[UserID]
		)
		VALUES
		(
			@CustomerName,
			@HomeAddress,
			@Email,
			@MobileNo,
			@GST_NO,
			@CityName,
			@PinCode,
			@NetAmount,
			@UserID
		)
END;
EXEC [dbo].[PR_CUSTOMER_INSERT] 'Daisy Miller','987 Birch St','daisy.miller@example.com','6789012345','GST678','Candyland','12345',400.00,1
EXEC [dbo].[PR_CUSTOMER_SELECTALL]

--Update Customer
CREATE PROC [dbo].[PR_CUSTOMER_UPDATE]
@CustomerID INT,
@CustomerName VARCHAR(100),
@HomeAddress VARCHAR(100),
@Email VARCHAR(100),
@MobileNo VARCHAR(15),
@GST_NO VARCHAR(15),
@CityName VARCHAR(100),
@PinCode VARCHAR(15),
@NetAmount DECIMAL(10,2),
@UserID INT
AS
BEGIN
    UPDATE [dbo].[Customer]
    SET 		
        [CustomerName] = @CustomerName,
        [HomeAddress] = @HomeAddress,
        Email = @Email,
		[MobileNo] = @MobileNo,
		[GST_NO] = @GST_NO,
        [CityName] = @CityName,
		[PinCode] = @PinCode,
		[NetAmount] = @NetAmount,
		[UserID] = @UserID
    WHERE 
        [CustomerID] = @CustomerID
END;
EXEC [dbo].[PR_CUSTOMER_UPDATE] 3,'Charlie Chocolate','789 Candy Ln','charlie@example.com','555-555-555','GST2345678','Candyland','12345',500.00,3
EXEC [dbo].[PR_CUSTOMER_SELECTALL]

--Delete Customer
CREATE PROC [dbo].[PR_CUSTOMER_DELETE]
@CustomerID INT
AS
DELETE FROM		
	[dbo].[Customer]
WHERE 
	[dbo].[Customer].[CustomerID] = @CustomerID
EXEC [dbo].[PR_CUSTOMER_DELETE] 6
EXEC [dbo].[PR_CUSTOMER_SELECTALL]

--Dropdown Customer
CREATE PROCEDURE [dbo].[PR_CUSTOMER_DROPDOWN]
AS
BEGIN
    SELECT
		[dbo].[Customer].[CustomerID],
        [dbo].[Customer].[CustomerName]
    FROM
        [dbo].[Customer]
END;
select * from product;
select * from orderdemo;
select * from userdemo;
select * from orderdetail;select * from customer;
select * from bills;
CREATE PROCEDURE [dbo].[PR_OrderDemo_DROPDOWN]
AS
BEGIN
    SELECT
		[dbo].OrderDemo.OrderID,
        [dbo].OrderDemo.OrderNumber
    FROM
        [dbo].OrderDemo
END;

CREATE PROCEDURE [dbo].[PR_User_Register]
    @UserName NVARCHAR(50),
    @Password NVARCHAR(50),
    @Email NVARCHAR(500),
    @MobileNo VARCHAR(50),
    @Address VARCHAR(50)
AS
BEGIN
    INSERT INTO [dbo].[User]
    (
        [UserName],
        [Password],
        [Email],
        [MobileNo],
        [Address]
    )
    VALUES
    (
        @UserName,
        @Password,
        @Email,
        @MobileNo,
        @Address
    );
END