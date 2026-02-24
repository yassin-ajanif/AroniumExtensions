using System.IO;
using AroniumFactures.Data;
using AroniumFactures.Repositories;
using AroniumFactures.Services;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures;

public static class ServiceProvider
{
    private static AppDbContext? _dbContext;
    private static IDocumentRepository? _documentRepository;
    private static ICustomerRepository? _customerRepository;
    private static IApplicationPropertyRepository? _applicationPropertyRepository;
    private static IProductRepository? _productRepository;
    private static IInvoiceService? _invoiceService;
    private static ICustomerService? _customerService;
    private static IApplicationService? _applicationService;
    private static IProductService? _productService;
    private static ILocalSettingsService? _localSettingsService;
    private static IPdfService? _pdfService;
    private static IUpdateService? _updateService;
    private static IGoogleDriveConnectionService? _googleDriveConnectionService;
    private static IAuditLogExportService? _auditLogExportService;
    private static AuditLogExportScheduler? _auditLogExportScheduler;

    public static void Initialize(string databasePath)
    {
        // Create DbContext
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={databasePath};Mode=ReadOnly;Default Timeout=3;");
        _dbContext = new AppDbContext(optionsBuilder.Options);

        // Create repositories
        _documentRepository = new DocumentRepository(_dbContext);
        _customerRepository = new CustomerRepository(_dbContext);
        _applicationPropertyRepository = new ApplicationPropertyRepository(_dbContext);
        _productRepository = new ProductRepository(_dbContext);

        // Create services
        _invoiceService = new InvoiceService(_documentRepository);
        _customerService = new CustomerService(_customerRepository);
        _applicationService = new ApplicationService(_applicationPropertyRepository);
        _productService = new ProductService(_productRepository);
        _localSettingsService = new LocalSettingsService();
        _pdfService = new PdfService();
        _updateService = new UpdateService();
        _googleDriveConnectionService = new GoogleDriveConnectionService();
        _auditLogExportService = new AuditLogExportService(_dbContext);
        _auditLogExportScheduler = new AuditLogExportScheduler(_auditLogExportService, _googleDriveConnectionService);
    }

    public static AppDbContext DbContext => _dbContext ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IInvoiceService InvoiceService => _invoiceService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static ICustomerService CustomerService => _customerService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IApplicationService ApplicationService => _applicationService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IProductService ProductService => _productService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static ILocalSettingsService LocalSettingsService => _localSettingsService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IPdfService PdfService => _pdfService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IUpdateService UpdateService => _updateService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IGoogleDriveConnectionService GoogleDriveConnectionService => _googleDriveConnectionService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static IAuditLogExportService AuditLogExportService => _auditLogExportService ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
    public static AuditLogExportScheduler AuditLogExportScheduler => _auditLogExportScheduler ?? throw new System.InvalidOperationException("ServiceProvider not initialized");
}
