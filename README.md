# Coffee Shop Project

This is a Coffee Shop Management System built using ASP.NET Core. The application provides functionality to manage various aspects of a coffee shop, including products, users, orders, and more, through a clean and user-friendly interface.

## Table of Contents
- [Features](#features)
- [Tables and CRUD Operations](#tables-and-crud-operations)
- [Technologies Used](#technologies-used)
  
## Features
- User authentication and management
- Product management with CRUD operations
- Order management with detailed order tracking
- Export functionality to Excel
- Responsive design for better user experience

## Tables and CRUD Operations

The application consists of the following six tables, each supporting CRUD operations:

1. **Product**
   - **Fields**: ProductID, ProductName, ProductPrice, ProductCode, Description, UserID
   - **CRUD Operations**:
     - **Create**: Add new products to the database.
     - **Read**: View product details and lists.
     - **Update**: Edit existing product information.
     - **Delete**: Remove products from the database.

2. **User**
   - **Fields**: UserID, UserName, UserEmail, UserPassword, UserRole
   - **CRUD Operations**:
     - **Create**: Register new users.
     - **Read**: View user details and lists.
     - **Update**: Edit user information.
     - **Delete**: Remove users from the system.

3. **Order**
   - **Fields**: OrderID, OrderDate, UserID, TotalAmount, ShippingAddress
   - **CRUD Operations**:
     - **Create**: Place new orders.
     - **Read**: View order details and lists.
     - **Update**: Modify existing orders.
     - **Delete**: Cancel orders.

4. **OrderDetail**
   - **Fields**: OrderDetailID, OrderID, ProductID, Quantity, Price
   - **CRUD Operations**:
     - **Create**: Add details to orders.
     - **Read**: View order details for each order.
     - **Update**: Edit order details.
     - **Delete**: Remove order details.

5. **Bill**
   - **Fields**: BillID, OrderID, BillDate, TotalAmount, PaymentMode
   - **CRUD Operations**:
     - **Create**: Generate bills for orders.
     - **Read**: View billing information.
     - **Update**: Modify billing details.
     - **Delete**: Cancel bills.

6. **Customer**
   - **Fields**: CustomerID, CustomerName, CustomerEmail, PhoneNumber
   - **CRUD Operations**:
     - **Create**: Add new customers.
     - **Read**: View customer details and lists.
     - **Update**: Edit customer information.
     - **Delete**: Remove customers from the system.

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- SQL Server
- HTML, CSS, JavaScript
- Bootstrap

