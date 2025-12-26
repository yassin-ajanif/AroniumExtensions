# Database Integration Guide

## Overview

Your Avalonia invoice application is now connected to the SQLite database (`database/pos.db`) using Entity Framework Core with a clean architecture pattern:

```
ViewModel → Service → Repository → Database
```

## Architecture

### 1. **Entities** (`Data/Entities/`)
Auto-generated entity models representing database tables:
- `Document` - Invoices/Bills
- `DocumentItem` - Invoice line items  
- `Customer` - Customer information
- `Product` - Products
- `Company`, `Payment`, `Tax`, etc.

### 2. **DbContext** (`Data/AppDbContext.cs`)
Entity Framework Core context managing database connections and entity tracking.

### 3. **Repositories** (`Repositories/`)
Data access layer:
- `IDocumentRepository` / `DocumentRepository` - Invoice CRUD operations
- `ICustomerRepository` / `CustomerRepository` - Customer CRUD operations

### 4. **Services** (`Services/`)
Business logic layer:
- `IInvoiceService` / `InvoiceService` - Invoice business logic
- `ICustomerService` / `CustomerService` - Customer business logic

### 5. **ServiceProvider** (`ServiceProvider.cs`)
Simple service locator providing access to services throughout the app.

## Usage Examples

### Initialize (Already done in App.axaml.cs)
```csharp
ServiceProvider.Initialize();
```

### In Your ViewModel

```csharp
public class YourViewModel
{
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerService _customerService;

    public YourViewModel()
    {
        _invoiceService = ServiceProvider.InvoiceService;
        _customerService = ServiceProvider.CustomerService;
    }

    // Load all invoices
    public async Task LoadInvoices()
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        // Use invoices...
    }

    // Create new invoice
    public async Task CreateInvoice()
    {
        var invoice = new Document
        {
            CustomerId = 1,
            UserId = 1,
            WarehouseId = 1,
            DocumentTypeId = 1,
            Total = 1000,
            DocumentItems = new List<DocumentItem>
            {
                new DocumentItem
                {
                    ProductId = 1,
                    Quantity = 2,
                    Price = 500,
                    Total = 1000
                }
            }
        };

        await _invoiceService.CreateInvoiceAsync(invoice);
    }

    // Get invoice by number
    public async Task<Document?> GetInvoice(string number)
    {
        return await _invoiceService.GetInvoiceByNumberAsync(number);
    }
}
```

### Available Service Methods

#### IInvoiceService
- `GetAllInvoicesAsync()` - Get all invoices
- `GetInvoiceByIdAsync(int id)` - Get invoice by ID
- `GetInvoiceByNumberAsync(string number)` - Get invoice by number
- `GetInvoicesByCustomerAsync(int customerId)` - Get customer invoices
- `CreateInvoiceAsync(Document)` - Create new invoice
- `UpdateInvoiceAsync(Document)` - Update existing invoice
- `DeleteInvoiceAsync(int id)` - Delete invoice
- `GenerateInvoiceNumberAsync()` - Generate unique invoice number

#### ICustomerService
- `GetAllCustomersAsync()` - Get all customers
- `GetCustomerByIdAsync(int id)` - Get customer by ID
- `GetCustomerByNameAsync(string name)` - Get customer by name
- `CreateCustomerAsync(Customer)` - Create new customer
- `UpdateCustomerAsync(Customer)` - Update existing customer
- `DeleteCustomerAsync(int id)` - Delete customer

## Database Schema

Key tables in `pos.db`:
- **Document** - Invoices (Number, Date, Total, CustomerId, etc.)
- **DocumentItem** - Invoice line items (ProductId, Quantity, Price, Total)
- **Customer** - Customer details (Name, TaxNumber, Address, etc.)
- **Product** - Product catalog
- **DocumentType** - Types of documents (Invoice, Quote, etc.)
- **Payment** - Payment records
- **Tax** - Tax definitions

## Connection String

Located in `ServiceProvider.cs`:
```csharp
Data Source=database/pos.db
```

## Notes

- All async operations use `await` for non-blocking database access
- Repositories handle DbContext operations
- Services contain business logic and validation
- ViewModels should only call Services, never Repositories directly
- Database path: `HelloAvalonia/database/pos.db`

## Next Steps

1. Update your ViewModels to use the services
2. Add UI to display invoices from database
3. Implement save functionality for new invoices
4. Add customer selection from database

See `Examples/DatabaseUsageExample.cs` for more detailed examples.




























