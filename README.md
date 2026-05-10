# E-commerce BookShop Web MVC

An online bookstore web application built with ASP.NET Core MVC, supporting authentication, product management, shopping cart, order processing, and Google Login integration.

---

## 📌 Project Overview

This project was developed to practice and demonstrate full-stack web development skills using ASP.NET Core MVC and SQL Server.

The application allows users to:

* Browse and search books
* View product details
* Add items to cart
* Place orders
* Login/Register accounts
* Authenticate using Google OAuth
* Manage products and orders through admin panel

---

## 🚀 Technologies Used

### Backend

* ASP.NET Core MVC
* Entity Framework Core
* LINQ
* Dependency Injection

### Frontend

* HTML5
* CSS3
* Bootstrap
* JavaScript
* Razor View Engine

### Database

* SQL Server

### Authentication

* ASP.NET Identity
* Google OAuth Login

### Tools

* Visual Studio 2022
* Git & GitHub

---

## ✨ Features

### User Features

* User registration and login
* Google Login authentication
* Browse books by category
* Search books
* Add to cart
* Checkout and order management
* Responsive UI

### Admin Features

* Manage products
* Manage categories
* Manage orders
* Manage users

---

## 📂 Project Structure

```bash
Controllers/
Models/
Views/
Data/
Repositories/
wwwroot/
```

---

## ⚙️ Installation & Setup

### 1. Clone repository

```bash
git clone https://github.com/jonepnix-dotcom/E-commerce_BookShop_WebMVC.git
```

---

### 2. Open project

Open solution using:

* Visual Studio 2022
* SQL Server Management Studio (SSMS)

---

### 3. Restore database from `.bak`

Database backup file is included in:

```bash
Database/dbbookshop.bak
```

Open SSMS and run:

```sql
RESTORE DATABASE dbbookshop
FROM DISK = 'YOUR_PATH/dbbookshop.bak'
WITH REPLACE;
```

Example:

```sql
RESTORE DATABASE dbbookshop
FROM DISK = 'D:\Project\Database\dbbookshop.bak'
WITH REPLACE;
```

---

### 4. Configure connection string

Update connection string in:

```bash
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "MyConnect": "Data Source=localhost;Initial Catalog=dbbookshop;Persist Security Info=True;User ID=sa;Password=YOUR_PASSWORD;Trust Server Certificate=True"
}
```

---

### 5. Configure Google Authentication

Create OAuth credentials from:

https://console.cloud.google.com/

Then update:

```json
"GoogleKeys": {
  "ClientId": "YOUR_CLIENT_ID",
  "ClientSecret": "YOUR_CLIENT_SECRET"
}
```

---

### 6. Run project

Press:

```bash
F5
```

or run:

```bash
dotnet run
```

## 📈 Learning Outcomes

Through this project, I learned:

* ASP.NET Core MVC architecture
* Authentication & Authorization
* Entity Framework Core
* Repository Pattern
* CRUD operations
* Session & Cookies
* Google OAuth Integration
* Responsive UI development
* Git/GitHub workflow

---


## 👨‍💻 Author

Your Name

* Email: [jonepnix@gmail.com](mailto:jonepnix@gmail.com)
* GitHub: https://github.com/jonepnix-dotcom

---

## 📄 License

This project is for learning and portfolio purposes.
