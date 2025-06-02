# E-Commerce Platform

An ASP.NET Core MVC e-commerce web application built with Entity Framework Core, ASP.NET Core Identity, and Bootstrap. Features full shopping cart functionality, user authentication, and Azure deployment capabilities.

## Features

### Core Features
- **Product Catalog**: Browse products with name, price, and description
- **Product Details**: View detailed information about individual products
- **Shopping Cart**: Add products to cart, update quantities, and remove items
- **Session Management**: Cart persists across browser sessions
- **User Authentication**: ASP.NET Core Identity with registration and login
- **Cart Persistence**: User-specific carts that persist across sessions when logged in
- **Cart Merging**: Seamless merge of anonymous cart with user cart upon login

### Technical Features
- **SQL Server Database**: Persistent data storage with Entity Framework Core
- **Repository Pattern**: Clean architecture with separation of concerns
- **Input Sanitization**: Secure handling of user inputs
- **Responsive Design**: Bootstrap-powered UI that works on all devices
- **AJAX Support**: Smooth cart interactions without page reloads
- **Azure Ready**: Configured for deployment to Azure App Service with Azure SQL
- **CI/CD Pipeline**: Azure DevOps pipeline configuration included
- **Application Insights**: Telemetry and monitoring support

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Authentication**: ASP.NET Core Identity
- **Database**: Entity Framework Core with SQL Server
- **Cloud**: Microsoft Azure (App Service, SQL Database, Application Insights)
- **UI**: Razor Views with Bootstrap 5
- **Architecture**: Repository pattern with dependency injection
- **Session Management**: ASP.NET Core Session with distributed cache
- **JavaScript**: jQuery for AJAX interactions
- **DevOps**: Azure DevOps with CI/CD pipelines

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open your browser and navigate to `http://localhost:5000`

### Available URLs

- **Home**: `http://localhost:5000/`
- **Product Catalog**: `http://localhost:5000/Product`
- **Product Details**: `http://localhost:5000/Product/Details/{id}`
- **Shopping Cart**: `http://localhost:5000/Cart`

## Project Structure

```
ECommercePlatform/
├── Controllers/
│   ├── HomeController.cs       # Home page controller
│   ├── ProductController.cs    # Product catalog and details
│   └── CartController.cs       # Shopping cart management
├── Data/
│   └── ApplicationDbContext.cs # Entity Framework context
├── Models/
│   ├── Product.cs             # Product entity model
│   ├── Cart.cs                # Shopping cart model
│   ├── CartItem.cs            # Cart item model
│   └── ErrorViewModel.cs      # Error handling model
├── Repositories/
│   ├── IProductRepository.cs   # Product repository interface
│   ├── ProductRepository.cs    # Product repository implementation
│   ├── ICartRepository.cs      # Cart repository interface
│   └── CartRepository.cs       # Cart repository implementation
├── Services/
│   ├── ICartService.cs         # Cart service interface
│   └── CartService.cs          # Cart business logic
├── Views/
│   ├── Home/
│   │   └── Index.cshtml       # Home page view
│   ├── Product/
│   │   ├── Index.cshtml       # Product catalog view
│   │   └── Details.cshtml     # Product details view
│   ├── Cart/
│   │   └── Index.cshtml       # Shopping cart view
│   └── Shared/
│       └── _Layout.cshtml     # Main layout template
└── wwwroot/                   # Static files (CSS, JS, images)
    └── js/
        └── cart.js            # Cart JavaScript functionality
```

## Database

The application uses SQL Server with Entity Framework Core migrations. The database includes:

### Tables
- **Products**: Store product information (Id, Name, Price, Description)
- **Carts**: Shopping cart sessions (Id, SessionId, CreatedAt, UpdatedAt)
- **CartItems**: Items in shopping carts (Id, CartId, ProductId, Quantity, UnitPrice)

### Sample Data
The database is seeded with sample products:

1. **Laptop** - $999.99
2. **Smartphone** - $699.99
3. **Tablet** - $399.99
4. **Headphones** - $199.99
5. **Smart Watch** - $299.99

### Running Migrations
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

## Development

### Setting Up the Database

1. **Install SQL Server LocalDB** (if not already installed)
2. **Update Connection String** in `appsettings.json` if needed
3. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

### Adding New Products

To add new products, modify the seed data in `Data/ApplicationDbContext.cs`:

```csharp
modelBuilder.Entity<Product>().HasData(
    new Product { Id = 6, Name = "New Product", Price = 99.99m, Description = "Product description" }
);
```

Then create and apply a new migration:
```bash
dotnet ef migrations add AddNewProduct
dotnet ef database update
```

### Testing Cart Functionality

1. **Add Items to Cart**: Use the "Add to Cart" buttons on product pages
2. **View Cart**: Navigate to `/Cart` to see cart contents
3. **Update Quantities**: Modify quantities directly in the cart
4. **Remove Items**: Use the remove buttons or set quantity to 0
5. **Clear Cart**: Use the "Clear Cart" button to empty the cart

### Architecture Overview

The application follows the **Repository Pattern** with **Dependency Injection**:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Contain business logic and validation
- **Repositories**: Handle data access operations
- **Models**: Define data structures and relationships
- **Views**: Present data to users with Razor syntax

### VS Code Extensions

Recommended extensions for development:
- C# (ms-dotnettools.csharp)
- C# Dev Kit (ms-dotnettools.csdevkit)
- ASP.NET Core Switcher (adrianwilczynski.asp-net-core-switcher)
- Essential ASP.NET Core Snippets (doggy8088.netcore-snippets)

## Stage 2 Implementation Summary

### 🎯 **Challenges & Solutions**

**Challenge**: Managing cart state across sessions
- **Solution**: Implemented session-based cart storage with SQL Server persistence
- **Implementation**: Used ASP.NET Core session management with unique session IDs

**Challenge**: Repository pattern implementation
- **Solution**: Created comprehensive repository interfaces and implementations
- **Benefits**: Improved testability, separation of concerns, and maintainability

**Challenge**: Input sanitization and validation
- **Solution**: Implemented robust input validation in CartService
- **Security**: Protected against invalid product IDs, negative quantities, and empty session IDs

### 🔧 **Key Technical Implementations**

1. **Entity Framework Migrations**: Automated database schema management
2. **Dependency Injection**: Clean IoC container setup for repositories and services
3. **Session Management**: Secure cart state persistence across requests
4. **AJAX Integration**: Seamless cart updates without page reloads
5. **Repository Pattern**: Clean separation between data access and business logic

### 📚 **Lessons Learned**

- **EF Core Migrations**: Understanding database evolution and version control
- **Session Management**: Implementing stateful behavior in stateless HTTP
- **Repository Pattern**: Benefits of abstraction layers in enterprise applications
- **Input Validation**: Importance of server-side validation for security
- **AJAX UX**: Enhancing user experience with asynchronous operations
- **Authentication Integration**: Seamless user experience with Identity framework
- **Cloud Architecture**: Designing applications for cloud deployment and scalability

## Azure Deployment

This application is configured for deployment to Microsoft Azure. See [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) for detailed deployment instructions.

### Quick Azure Deployment

1. **Infrastructure Setup**
   ```bash
   az group create --name ecommerce-rg --location "East US"
   az deployment group create --resource-group ecommerce-rg --template-file azure-deploy.json
   ```

2. **Application Deployment**
   ```bash
   dotnet publish -c Release
   az webapp deployment source config-zip --resource-group ecommerce-rg --name your-app-name --src publish.zip
   ```

3. **Database Migration**
   ```bash
   dotnet ef database update --connection "your-azure-sql-connection"
   ```

### Azure Resources Included

- **Azure App Service**: Web application hosting
- **Azure SQL Database**: Production database
- **Application Insights**: Monitoring and telemetry
- **Azure DevOps Pipeline**: Automated CI/CD

## Development Stages

### Stage 1: Basic Product Listing ✅
- Product catalog with Entity Framework Core
- In-memory database for development
- Basic MVC structure with Razor views

### Stage 2: Shopping Cart & Database ✅
- Shopping cart functionality with session management
- SQL Server database with migrations
- Repository pattern implementation
- AJAX cart interactions

### Stage 3: Authentication & Azure Deployment ✅
- ASP.NET Core Identity integration
- User authentication and cart persistence
- Azure deployment configuration
- CI/CD pipeline setup
- Production monitoring with Application Insights

## Future Enhancements

- Order processing and checkout
- Payment integration (Stripe/PayPal)
- Product search and filtering
- Admin panel for product management
- Email notifications
- Advanced security features
- Performance optimization
- Multi-language support

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License.
