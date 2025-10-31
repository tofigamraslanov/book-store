# 📚 BulkyBook - E-Commerce Bookstore

A full-featured e-commerce web application for managing and selling books online, built with ASP.NET Core MVC following clean architecture principles.

## ✨ Features

### 🛍️ Customer Features
- **Product Browsing**: Browse books with detailed information including title, author, ISBN, description, and cover type
- **Shopping Cart**: Add books to cart with dynamic pricing based on quantity
- **Tiered Pricing**: Automatic price calculation (1-49 books, 50-99 books, 100+ books)
- **Order Management**: Place orders with detailed shipping information
- **Order Tracking**: View order status and payment status
- **User Authentication**: Register/login with email or social media (Facebook, Google)
- **User Roles**: Individual customers and company customers with different privileges

### 👨‍💼 Admin Features
- **Product Management**: CRUD operations for books with image upload
- **Category Management**: Organize books by categories
- **Cover Type Management**: Manage different cover types (Hardcover, Paperback, etc.) with stored procedures
- **Company Management**: Manage authorized companies for B2B transactions
- **User Management**: Administer users and assign roles (Admin, Employee, Individual Customer, Company Customer)
- **Order Management**: 
  - View all orders with filtering capabilities
  - Update order status (Pending, Approved, In Process, Shipped, Cancelled, Refunded)
  - Process payments and manage payment status
  - Update shipping details (carrier, tracking number)
  - Cancel orders and process refunds

### 💳 Payment Integration
- **Stripe Payment Gateway**: Secure payment processing for individual customers
- **Braintree Payment Gateway**: Alternative payment processing option
- **Delayed Payment**: Company customers can pay within 30 days of order placement

### 📧 Communication
- **Email Notifications**: SendGrid integration for transactional emails
- **SMS Notifications**: Twilio integration for order updates

## 🏗️ Architecture

The project follows **Clean Architecture** and **Repository Pattern** with clear separation of concerns:

```
BulkyBook/
│
├── BulkyBookWeb/              # Presentation Layer (ASP.NET Core MVC)
│   ├── Areas/
│   │   ├── Admin/            # Admin area controllers and views
│   │   ├── Customer/         # Customer area controllers and views
│   │   └── Identity/         # Authentication and authorization
│   ├── ViewModels/           # View-specific models
│   ├── ViewComponents/       # Reusable UI components
│   └── wwwroot/              # Static files (CSS, JS, images)
│
├── BulkyBook.DataAccess/      # Data Access Layer
│   ├── Data/                 # DbContext
│   ├── Repositories/         # Repository pattern implementation
│   ├── Migrations/           # EF Core migrations
│   └── Initializer/          # Database seeding and initialization
│
├── BulkyBook.Entities/        # Domain Layer
│   └── Models                # Domain entities (Product, Order, User, etc.)
│
└── BulkyBook.Utilities/       # Cross-cutting Concerns
    ├── Payment gateways      # Stripe, Braintree integration
    ├── Email service         # SendGrid integration
    └── Helper classes        # Static details, extensions
```

## 🛠️ Tech Stack

### Backend
- **Framework**: ASP.NET Core 5.0 MVC
- **Language**: C# (.NET 5.0)
- **ORM**: Entity Framework Core 5.0
- **Database**: SQL Server (Azure SQL Database support)
- **Authentication**: ASP.NET Core Identity
- **Design Patterns**: 
  - Repository Pattern
  - Unit of Work Pattern
  - Dependency Injection

### Frontend
- **View Engine**: Razor Pages
- **UI Framework**: Bootstrap
- **JavaScript**: jQuery
- **Rich Text Editor**: TinyMCE (for product descriptions)
- **DataTables**: For advanced table features

### Third-Party Integrations
- **Payment Processing**:
  - Stripe.net (v39.91.0)
  - Braintree
- **Email Service**: SendGrid
- **SMS Service**: Twilio (v5.72.1)
- **Social Authentication**:
  - Facebook Authentication
  - Google Authentication

### Development Tools
- **IDE**: Visual Studio 2022
- **Version Control**: Git
- **Package Manager**: NuGet

## 📦 Database Schema

The application uses the following main entities:

- **Product**: Books with pricing tiers, categories, and cover types
- **Category**: Book categories
- **CoverType**: Cover type classification (with stored procedures)
- **Company**: B2B customer companies
- **ApplicationUser**: Extended identity user with company association
- **ShoppingCart**: User shopping cart items
- **OrderHeader**: Order master data
- **OrderDetails**: Order line items

## 🚀 Getting Started

### Prerequisites
- .NET 5.0 SDK or later
- SQL Server (LocalDB, Express, or Azure SQL Database)
- Visual Studio 2022 (recommended) or VS Code
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/tofigamraslanov/book-store.git
   cd book-store
   ```

2. **Update database connection string**
   
   Edit `appsettings.json` in the `BulkyBookWeb` project:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=BulkyBook;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Configure third-party services** (Optional)
   
   Update the following sections in `appsettings.json`:
   - Stripe keys (for payment processing)
   - SendGrid key (for email notifications)
   - Twilio credentials (for SMS notifications)
   - Facebook/Google OAuth credentials (for social login)

4. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

5. **Apply database migrations**
   ```bash
   dotnet ef database update --project BulkyBook.DataAccess --startup-project BulkyBookWeb
   ```

6. **Run the application**
   ```bash
   dotnet run --project BulkyBookWeb
   ```

7. **Access the application**
   
   Navigate to `https://localhost:5001` in your browser

### Default Users

The application includes a database initializer that creates default users and roles. Check `DbInitializer.cs` for default credentials.

## 🔐 Security Features

- **ASP.NET Core Identity**: Secure user authentication and authorization
- **Role-Based Access Control**: Multiple user roles with different permissions
- **HTTPS Enforcement**: Secure communication
- **Password Hashing**: Secure password storage
- **Anti-Forgery Tokens**: CSRF protection
- **Session Management**: Secure session handling with cookies

## 📱 Responsive Design

The application is fully responsive and works seamlessly on:
- Desktop computers
- Tablets
- Mobile devices

## 🧪 Key Functionalities

### Shopping Cart System
- Add/remove items
- Update quantities
- Real-time price calculation
- Session-based cart persistence

### Order Processing Workflow
1. Customer adds items to cart
2. Reviews cart and proceeds to checkout
3. Enters/confirms shipping information
4. Selects payment method
5. Completes payment (immediate for individuals, delayed for companies)
6. Order confirmation and email notification
7. Admin processes and ships order
8. Customer receives tracking information

### Payment Flow
- **Individual Customers**: Pay via Stripe at checkout
- **Company Customers**: Receive 30-day payment terms
- **Payment Status Tracking**: Pending → Approved/Rejected
- **Refund Processing**: Available for cancelled orders

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

## 📄 License

This project is licensed under the MIT License.

## 👤 Author

**Tofig Amraslanov**
- GitHub: [@tofigamraslanov](https://github.com/tofigamraslanov)

## 🙏 Acknowledgments

- Built following clean architecture principles
- Implements industry-standard design patterns
- Uses modern ASP.NET Core best practices

---

⭐ Star this repository if you find it helpful!
