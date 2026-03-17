using System;
using System.Linq;
using AroniumFactures.Data;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures.Services;

/// <summary>
/// Creates TableAuditLog and all audit triggers when missing.
/// </summary>
public class DbTriggerService : IDbTriggerService
{
    private readonly string _databasePath;

    public DbTriggerService(string databasePath)
    {
        _databasePath = databasePath;
    }

    public void EnsureAuditInfrastructure()
    {
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={_databasePath};Default Timeout=3;");
            using var dbContext = new AppDbContext(optionsBuilder.Options);

            // Check if TableAuditLog already exists; if so, assume triggers are in place.
            const string tableCheckSql = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'TableAuditLog';";
            var exists = dbContext.Database.SqlQueryRaw<int>(tableCheckSql).AsEnumerable().FirstOrDefault() > 0;
            if (exists)
                return;

            using var transaction = dbContext.Database.BeginTransaction();

            // Create TableAuditLog table
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TABLE TableAuditLog (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    TableName    TEXT NOT NULL,
    Operation    TEXT NOT NULL,
    SqlStatement TEXT NOT NULL,
    CreatedAt    TEXT NOT NULL DEFAULT (datetime('now'))
);");

            // Customer triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Customer_AfterDelete
AFTER DELETE ON Customer
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Customer',
    'DELETE',
    'DELETE FROM Customer WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Customer_AfterInsert
AFTER INSERT ON Customer
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Customer',
    'INSERT',
    'INSERT INTO Customer (Id, Code, Name, TaxNumber, Address, PostalCode, City, CountryId, DateCreated, DateUpdated, Email, PhoneNumber, IsEnabled, IsCustomer, IsSupplier, DueDatePeriod, StreetName, AdditionalStreetName, BuildingNumber, PlotIdentification, CitySubdivisionName, CountrySubentity, IsTaxExempt, PriceListId) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END || ', '
    || quote(COALESCE(NEW.Name, '')) || ', '
    || CASE WHEN NEW.TaxNumber IS NULL THEN 'NULL' ELSE quote(NEW.TaxNumber) END || ', '
    || CASE WHEN NEW.Address IS NULL THEN 'NULL' ELSE quote(NEW.Address) END || ', '
    || CASE WHEN NEW.PostalCode IS NULL THEN 'NULL' ELSE quote(NEW.PostalCode) END || ', '
    || CASE WHEN NEW.City IS NULL THEN 'NULL' ELSE quote(NEW.City) END || ', '
    || CASE WHEN NEW.CountryId IS NULL THEN 'NULL' ELSE CAST(NEW.CountryId AS TEXT) END || ', '
    || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END || ', '
    || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END || ', '
    || CASE WHEN NEW.Email IS NULL THEN 'NULL' ELSE quote(NEW.Email) END || ', '
    || CASE WHEN NEW.PhoneNumber IS NULL THEN 'NULL' ELSE quote(NEW.PhoneNumber) END || ', '
    || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsCustomer AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsSupplier AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DueDatePeriod AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.StreetName IS NULL THEN 'NULL' ELSE quote(NEW.StreetName) END || ', '
    || CASE WHEN NEW.AdditionalStreetName IS NULL THEN 'NULL' ELSE quote(NEW.AdditionalStreetName) END || ', '
    || CASE WHEN NEW.BuildingNumber IS NULL THEN 'NULL' ELSE quote(NEW.BuildingNumber) END || ', '
    || CASE WHEN NEW.PlotIdentification IS NULL THEN 'NULL' ELSE quote(NEW.PlotIdentification) END || ', '
    || CASE WHEN NEW.CitySubdivisionName IS NULL THEN 'NULL' ELSE quote(NEW.CitySubdivisionName) END || ', '
    || CASE WHEN NEW.CountrySubentity IS NULL THEN 'NULL' ELSE quote(NEW.CountrySubentity) END || ', '
    || COALESCE(CAST(NEW.IsTaxExempt AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.PriceListId IS NULL THEN 'NULL' ELSE CAST(NEW.PriceListId AS TEXT) END || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Customer_AfterUpdate
AFTER UPDATE ON Customer
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Customer',
    'UPDATE',
    'UPDATE Customer SET '
    || 'Code = ' || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END
    || ', Name = ' || quote(COALESCE(NEW.Name, ''))
    || ', TaxNumber = ' || CASE WHEN NEW.TaxNumber IS NULL THEN 'NULL' ELSE quote(NEW.TaxNumber) END
    || ', Address = ' || CASE WHEN NEW.Address IS NULL THEN 'NULL' ELSE quote(NEW.Address) END
    || ', PostalCode = ' || CASE WHEN NEW.PostalCode IS NULL THEN 'NULL' ELSE quote(NEW.PostalCode) END
    || ', City = ' || CASE WHEN NEW.City IS NULL THEN 'NULL' ELSE quote(NEW.City) END
    || ', CountryId = ' || CASE WHEN NEW.CountryId IS NULL THEN 'NULL' ELSE CAST(NEW.CountryId AS TEXT) END
    || ', DateCreated = ' || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END
    || ', DateUpdated = ' || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END
    || ', Email = ' || CASE WHEN NEW.Email IS NULL THEN 'NULL' ELSE quote(NEW.Email) END
    || ', PhoneNumber = ' || CASE WHEN NEW.PhoneNumber IS NULL THEN 'NULL' ELSE quote(NEW.PhoneNumber) END
    || ', IsEnabled = ' || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL')
    || ', IsCustomer = ' || COALESCE(CAST(NEW.IsCustomer AS TEXT), 'NULL')
    || ', IsSupplier = ' || COALESCE(CAST(NEW.IsSupplier AS TEXT), 'NULL')
    || ', DueDatePeriod = ' || COALESCE(CAST(NEW.DueDatePeriod AS TEXT), 'NULL')
    || ', StreetName = ' || CASE WHEN NEW.StreetName IS NULL THEN 'NULL' ELSE quote(NEW.StreetName) END
    || ', AdditionalStreetName = ' || CASE WHEN NEW.AdditionalStreetName IS NULL THEN 'NULL' ELSE quote(NEW.AdditionalStreetName) END
    || ', BuildingNumber = ' || CASE WHEN NEW.BuildingNumber IS NULL THEN 'NULL' ELSE quote(NEW.BuildingNumber) END
    || ', PlotIdentification = ' || CASE WHEN NEW.PlotIdentification IS NULL THEN 'NULL' ELSE quote(NEW.PlotIdentification) END
    || ', CitySubdivisionName = ' || CASE WHEN NEW.CitySubdivisionName IS NULL THEN 'NULL' ELSE quote(NEW.CitySubdivisionName) END
    || ', CountrySubentity = ' || CASE WHEN NEW.CountrySubentity IS NULL THEN 'NULL' ELSE quote(NEW.CountrySubentity) END
    || ', IsTaxExempt = ' || COALESCE(CAST(NEW.IsTaxExempt AS TEXT), 'NULL')
    || ', PriceListId = ' || CASE WHEN NEW.PriceListId IS NULL THEN 'NULL' ELSE CAST(NEW.PriceListId AS TEXT) END
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // Document triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Document_AfterDelete
AFTER DELETE ON Document
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Document',
    'DELETE',
    'DELETE FROM Document WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Document_AfterInsert
AFTER INSERT ON Document
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Document',
    'INSERT',
    'INSERT INTO Document (Id, Number, UserId, CustomerId, OrderNumber, Date, StockDate, Total, IsClockedOut, DocumentTypeId, WarehouseId, ReferenceDocumentNumber, DateCreated, DateUpdated, InternalNote, Note, DueDate, Discount, DiscountType, PaidStatus, DiscountApplyRule, ServiceType) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || quote(COALESCE(NEW.Number, '')) || ', '
    || COALESCE(CAST(NEW.UserId AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.CustomerId IS NULL THEN 'NULL' ELSE CAST(NEW.CustomerId AS TEXT) END || ', '
    || CASE WHEN NEW.OrderNumber IS NULL THEN 'NULL' ELSE quote(NEW.OrderNumber) END || ', '
    || CASE WHEN NEW.Date IS NULL THEN 'NULL' ELSE quote(NEW.Date) END || ', '
    || CASE WHEN NEW.StockDate IS NULL THEN 'NULL' ELSE quote(NEW.StockDate) END || ', '
    || COALESCE(CAST(NEW.Total AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsClockedOut AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DocumentTypeId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.WarehouseId AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.ReferenceDocumentNumber IS NULL THEN 'NULL' ELSE quote(NEW.ReferenceDocumentNumber) END || ', '
    || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END || ', '
    || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END || ', '
    || CASE WHEN NEW.InternalNote IS NULL THEN 'NULL' ELSE quote(NEW.InternalNote) END || ', '
    || CASE WHEN NEW.Note IS NULL THEN 'NULL' ELSE quote(NEW.Note) END || ', '
    || CASE WHEN NEW.DueDate IS NULL THEN 'NULL' ELSE quote(NEW.DueDate) END || ', '
    || COALESCE(CAST(NEW.Discount AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DiscountType AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.PaidStatus AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DiscountApplyRule AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.ServiceType AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Document_AfterUpdate
AFTER UPDATE ON Document
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Document',
    'UPDATE',
    'UPDATE Document SET '
    || 'Number = ' || quote(COALESCE(NEW.Number, ''))
    || ', UserId = ' || COALESCE(CAST(NEW.UserId AS TEXT), 'NULL')
    || ', CustomerId = ' || CASE WHEN NEW.CustomerId IS NULL THEN 'NULL' ELSE CAST(NEW.CustomerId AS TEXT) END
    || ', OrderNumber = ' || CASE WHEN NEW.OrderNumber IS NULL THEN 'NULL' ELSE quote(NEW.OrderNumber) END
    || ', Date = ' || CASE WHEN NEW.Date IS NULL THEN 'NULL' ELSE quote(NEW.Date) END
    || ', StockDate = ' || CASE WHEN NEW.StockDate IS NULL THEN 'NULL' ELSE quote(NEW.StockDate) END
    || ', Total = ' || COALESCE(CAST(NEW.Total AS TEXT), 'NULL')
    || ', IsClockedOut = ' || COALESCE(CAST(NEW.IsClockedOut AS TEXT), 'NULL')
    || ', DocumentTypeId = ' || COALESCE(CAST(NEW.DocumentTypeId AS TEXT), 'NULL')
    || ', WarehouseId = ' || COALESCE(CAST(NEW.WarehouseId AS TEXT), 'NULL')
    || ', ReferenceDocumentNumber = ' || CASE WHEN NEW.ReferenceDocumentNumber IS NULL THEN 'NULL' ELSE quote(NEW.ReferenceDocumentNumber) END
    || ', DateCreated = ' || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END
    || ', DateUpdated = ' || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END
    || ', InternalNote = ' || CASE WHEN NEW.InternalNote IS NULL THEN 'NULL' ELSE quote(NEW.InternalNote) END
    || ', Note = ' || CASE WHEN NEW.Note IS NULL THEN 'NULL' ELSE quote(NEW.Note) END
    || ', DueDate = ' || CASE WHEN NEW.DueDate IS NULL THEN 'NULL' ELSE quote(NEW.DueDate) END
    || ', Discount = ' || COALESCE(CAST(NEW.Discount AS TEXT), 'NULL')
    || ', DiscountType = ' || COALESCE(CAST(NEW.DiscountType AS TEXT), 'NULL')
    || ', PaidStatus = ' || COALESCE(CAST(NEW.PaidStatus AS TEXT), 'NULL')
    || ', DiscountApplyRule = ' || COALESCE(CAST(NEW.DiscountApplyRule AS TEXT), 'NULL')
    || ', ServiceType = ' || COALESCE(CAST(NEW.ServiceType AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // DocumentItem triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItem_AfterDelete
AFTER DELETE ON DocumentItem
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItem',
    'DELETE',
    'DELETE FROM DocumentItem WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItem_AfterInsert
AFTER INSERT ON DocumentItem
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItem',
    'INSERT',
    'INSERT INTO DocumentItem (Id, DocumentId, ProductId, Quantity, ExpectedQuantity, PriceBeforeTax, Price, Discount, DiscountType, ProductCost, PriceBeforeTaxAfterDiscount, PriceAfterDiscount, Total, TotalAfterDocumentDiscount, DiscountApplyRule) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DocumentId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.ProductId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Quantity AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.ExpectedQuantity AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.PriceBeforeTax AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Price AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Discount AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DiscountType AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.ProductCost AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.PriceBeforeTaxAfterDiscount AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.PriceAfterDiscount AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Total AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.TotalAfterDocumentDiscount AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DiscountApplyRule AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItem_AfterUpdate
AFTER UPDATE ON DocumentItem
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItem',
    'UPDATE',
    'UPDATE DocumentItem SET '
    || 'DocumentId = ' || COALESCE(CAST(NEW.DocumentId AS TEXT), 'NULL')
    || ', ProductId = ' || COALESCE(CAST(NEW.ProductId AS TEXT), 'NULL')
    || ', Quantity = ' || COALESCE(CAST(NEW.Quantity AS TEXT), 'NULL')
    || ', ExpectedQuantity = ' || COALESCE(CAST(NEW.ExpectedQuantity AS TEXT), 'NULL')
    || ', PriceBeforeTax = ' || COALESCE(CAST(NEW.PriceBeforeTax AS TEXT), 'NULL')
    || ', Price = ' || COALESCE(CAST(NEW.Price AS TEXT), 'NULL')
    || ', Discount = ' || COALESCE(CAST(NEW.Discount AS TEXT), 'NULL')
    || ', DiscountType = ' || COALESCE(CAST(NEW.DiscountType AS TEXT), 'NULL')
    || ', ProductCost = ' || COALESCE(CAST(NEW.ProductCost AS TEXT), 'NULL')
    || ', PriceBeforeTaxAfterDiscount = ' || COALESCE(CAST(NEW.PriceBeforeTaxAfterDiscount AS TEXT), 'NULL')
    || ', PriceAfterDiscount = ' || COALESCE(CAST(NEW.PriceAfterDiscount AS TEXT), 'NULL')
    || ', Total = ' || COALESCE(CAST(NEW.Total AS TEXT), 'NULL')
    || ', TotalAfterDocumentDiscount = ' || COALESCE(CAST(NEW.TotalAfterDocumentDiscount AS TEXT), 'NULL')
    || ', DiscountApplyRule = ' || COALESCE(CAST(NEW.DiscountApplyRule AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // DocumentItemTax triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItemTax_AfterDelete
AFTER DELETE ON DocumentItemTax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItemTax',
    'DELETE',
    'DELETE FROM DocumentItemTax WHERE DocumentItemId = ' || CAST(OLD.DocumentItemId AS TEXT) || ' AND TaxId = ' || CAST(OLD.TaxId AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItemTax_AfterInsert
AFTER INSERT ON DocumentItemTax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItemTax',
    'INSERT',
    'INSERT INTO DocumentItemTax (DocumentItemId, TaxId, Amount) VALUES ('
    || COALESCE(CAST(NEW.DocumentItemId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.TaxId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Amount AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER DocumentItemTax_AfterUpdate
AFTER UPDATE ON DocumentItemTax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'DocumentItemTax',
    'UPDATE',
    'UPDATE DocumentItemTax SET '
    || 'DocumentItemId = ' || COALESCE(CAST(NEW.DocumentItemId AS TEXT), 'NULL')
    || ', TaxId = ' || COALESCE(CAST(NEW.TaxId AS TEXT), 'NULL')
    || ', Amount = ' || COALESCE(CAST(NEW.Amount AS TEXT), 'NULL')
    || ' WHERE DocumentItemId = ' || CAST(OLD.DocumentItemId AS TEXT) || ' AND TaxId = ' || CAST(OLD.TaxId AS TEXT) || ';'
  );
END;");

            // Payment triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Payment_AfterDelete
AFTER DELETE ON Payment
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Payment',
    'DELETE',
    'DELETE FROM Payment WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Payment_AfterInsert
AFTER INSERT ON Payment
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Payment',
    'INSERT',
    'INSERT INTO Payment (Id, DocumentId, PaymentTypeId, Amount, Date, UserId, ZreportId, DateCreated) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.DocumentId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.PaymentTypeId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Amount AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.Date IS NULL THEN 'NULL' ELSE quote(NEW.Date) END || ', '
    || COALESCE(CAST(NEW.UserId AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.ZreportId IS NULL THEN 'NULL' ELSE CAST(NEW.ZreportId AS TEXT) END || ', '
    || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Payment_AfterUpdate
AFTER UPDATE ON Payment
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Payment',
    'UPDATE',
    'UPDATE Payment SET '
    || 'DocumentId = ' || COALESCE(CAST(NEW.DocumentId AS TEXT), 'NULL')
    || ', PaymentTypeId = ' || COALESCE(CAST(NEW.PaymentTypeId AS TEXT), 'NULL')
    || ', Amount = ' || COALESCE(CAST(NEW.Amount AS TEXT), 'NULL')
    || ', Date = ' || CASE WHEN NEW.Date IS NULL THEN 'NULL' ELSE quote(NEW.Date) END
    || ', UserId = ' || COALESCE(CAST(NEW.UserId AS TEXT), 'NULL')
    || ', ZreportId = ' || CASE WHEN NEW.ZreportId IS NULL THEN 'NULL' ELSE CAST(NEW.ZreportId AS TEXT) END
    || ', DateCreated = ' || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // PaymentType triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER PaymentType_AfterDelete
AFTER DELETE ON PaymentType
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'PaymentType',
    'DELETE',
    'DELETE FROM PaymentType WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER PaymentType_AfterInsert
AFTER INSERT ON PaymentType
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'PaymentType',
    'INSERT',
    'INSERT INTO PaymentType (Id, Name, Code, IsCustomerRequired, IsFiscal, IsSlipRequired, IsChangeAllowed, Ordinal, IsEnabled, IsQuickPayment, OpenCashDrawer, ShortcutKey, MarkAsPaid) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || quote(COALESCE(NEW.Name, '')) || ', '
    || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END || ', '
    || COALESCE(CAST(NEW.IsCustomerRequired AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsFiscal AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsSlipRequired AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsChangeAllowed AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Ordinal AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsQuickPayment AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.OpenCashDrawer AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.ShortcutKey IS NULL THEN 'NULL' ELSE quote(NEW.ShortcutKey) END || ', '
    || COALESCE(CAST(NEW.MarkAsPaid AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER PaymentType_AfterUpdate
AFTER UPDATE ON PaymentType
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'PaymentType',
    'UPDATE',
    'UPDATE PaymentType SET '
    || 'Name = ' || quote(COALESCE(NEW.Name, ''))
    || ', Code = ' || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END
    || ', IsCustomerRequired = ' || COALESCE(CAST(NEW.IsCustomerRequired AS TEXT), 'NULL')
    || ', IsFiscal = ' || COALESCE(CAST(NEW.IsFiscal AS TEXT), 'NULL')
    || ', IsSlipRequired = ' || COALESCE(CAST(NEW.IsSlipRequired AS TEXT), 'NULL')
    || ', IsChangeAllowed = ' || COALESCE(CAST(NEW.IsChangeAllowed AS TEXT), 'NULL')
    || ', Ordinal = ' || COALESCE(CAST(NEW.Ordinal AS TEXT), 'NULL')
    || ', IsEnabled = ' || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL')
    || ', IsQuickPayment = ' || COALESCE(CAST(NEW.IsQuickPayment AS TEXT), 'NULL')
    || ', OpenCashDrawer = ' || COALESCE(CAST(NEW.OpenCashDrawer AS TEXT), 'NULL')
    || ', ShortcutKey = ' || CASE WHEN NEW.ShortcutKey IS NULL THEN 'NULL' ELSE quote(NEW.ShortcutKey) END
    || ', MarkAsPaid = ' || COALESCE(CAST(NEW.MarkAsPaid AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // Product triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Product_AfterDelete
AFTER DELETE ON Product
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Product',
    'DELETE',
    'DELETE FROM Product WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Product_AfterInsert
AFTER INSERT ON Product
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Product',
    'INSERT',
    'INSERT INTO Product (Id, ProductGroupId, Name, Code, Plu, MeasurementUnit, Price, IsTaxInclusivePrice, CurrencyId, IsPriceChangeAllowed, IsService, IsUsingDefaultQuantity, IsEnabled, Description, DateCreated, DateUpdated, Cost, Markup, Image, Color, AgeRestriction, LastPurchasePrice, Rank) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.ProductGroupId IS NULL THEN 'NULL' ELSE CAST(NEW.ProductGroupId AS TEXT) END || ', '
    || quote(COALESCE(NEW.Name, '')) || ', '
    || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END || ', '
    || CASE WHEN NEW.Plu IS NULL THEN 'NULL' ELSE CAST(NEW.Plu AS TEXT) END || ', '
    || CASE WHEN NEW.MeasurementUnit IS NULL THEN 'NULL' ELSE quote(NEW.MeasurementUnit) END || ', '
    || COALESCE(CAST(NEW.Price AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.IsTaxInclusivePrice IS NULL THEN 'NULL' ELSE CAST(NEW.IsTaxInclusivePrice AS TEXT) END || ', '
    || CASE WHEN NEW.CurrencyId IS NULL THEN 'NULL' ELSE CAST(NEW.CurrencyId AS TEXT) END || ', '
    || COALESCE(CAST(NEW.IsPriceChangeAllowed AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsService AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsUsingDefaultQuantity AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.Description IS NULL THEN 'NULL' ELSE quote(NEW.Description) END || ', '
    || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END || ', '
    || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END || ', '
    || COALESCE(CAST(NEW.Cost AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Markup AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.Image IS NULL THEN 'NULL' ELSE ('X''' || hex(NEW.Image) || '''') END || ', '
    || quote(COALESCE(NEW.Color, '')) || ', '
    || CASE WHEN NEW.AgeRestriction IS NULL THEN 'NULL' ELSE CAST(NEW.AgeRestriction AS TEXT) END || ', '
    || COALESCE(CAST(NEW.LastPurchasePrice AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Rank AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Product_AfterUpdate
AFTER UPDATE ON Product
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Product',
    'UPDATE',
    'UPDATE Product SET '
    || 'ProductGroupId = ' || CASE WHEN NEW.ProductGroupId IS NULL THEN 'NULL' ELSE CAST(NEW.ProductGroupId AS TEXT) END
    || ', Name = ' || quote(COALESCE(NEW.Name, ''))
    || ', Code = ' || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END
    || ', Plu = ' || CASE WHEN NEW.Plu IS NULL THEN 'NULL' ELSE CAST(NEW.Plu AS TEXT) END
    || ', MeasurementUnit = ' || CASE WHEN NEW.MeasurementUnit IS NULL THEN 'NULL' ELSE quote(NEW.MeasurementUnit) END
    || ', Price = ' || COALESCE(CAST(NEW.Price AS TEXT), 'NULL')
    || ', IsTaxInclusivePrice = ' || CASE WHEN NEW.IsTaxInclusivePrice IS NULL THEN 'NULL' ELSE CAST(NEW.IsTaxInclusivePrice AS TEXT) END
    || ', CurrencyId = ' || CASE WHEN NEW.CurrencyId IS NULL THEN 'NULL' ELSE CAST(NEW.CurrencyId AS TEXT) END
    || ', IsPriceChangeAllowed = ' || COALESCE(CAST(NEW.IsPriceChangeAllowed AS TEXT), 'NULL')
    || ', IsService = ' || COALESCE(CAST(NEW.IsService AS TEXT), 'NULL')
    || ', IsUsingDefaultQuantity = ' || COALESCE(CAST(NEW.IsUsingDefaultQuantity AS TEXT), 'NULL')
    || ', IsEnabled = ' || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL')
    || ', Description = ' || CASE WHEN NEW.Description IS NULL THEN 'NULL' ELSE quote(NEW.Description) END
    || ', DateCreated = ' || CASE WHEN NEW.DateCreated IS NULL THEN 'NULL' ELSE quote(NEW.DateCreated) END
    || ', DateUpdated = ' || CASE WHEN NEW.DateUpdated IS NULL THEN 'NULL' ELSE quote(NEW.DateUpdated) END
    || ', Cost = ' || COALESCE(CAST(NEW.Cost AS TEXT), 'NULL')
    || ', Markup = ' || COALESCE(CAST(NEW.Markup AS TEXT), 'NULL')
    || ', Image = ' || CASE WHEN NEW.Image IS NULL THEN 'NULL' ELSE ('X''' || hex(NEW.Image) || '''') END
    || ', Color = ' || quote(COALESCE(NEW.Color, ''))
    || ', AgeRestriction = ' || CASE WHEN NEW.AgeRestriction IS NULL THEN 'NULL' ELSE CAST(NEW.AgeRestriction AS TEXT) END
    || ', LastPurchasePrice = ' || COALESCE(CAST(NEW.LastPurchasePrice AS TEXT), 'NULL')
    || ', Rank = ' || COALESCE(CAST(NEW.Rank AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // Stock triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Stock_AfterDelete
AFTER DELETE ON Stock
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Stock',
    'DELETE',
    'DELETE FROM Stock WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Stock_AfterInsert
AFTER INSERT ON Stock
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Stock',
    'INSERT',
    'INSERT INTO Stock (Id, ProductId, WarehouseId, Quantity) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.ProductId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.WarehouseId AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.Quantity AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Stock_AfterUpdate
AFTER UPDATE ON Stock
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Stock',
    'UPDATE',
    'UPDATE Stock SET '
    || 'ProductId = ' || COALESCE(CAST(NEW.ProductId AS TEXT), 'NULL')
    || ', WarehouseId = ' || COALESCE(CAST(NEW.WarehouseId AS TEXT), 'NULL')
    || ', Quantity = ' || COALESCE(CAST(NEW.Quantity AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            // Tax triggers
            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Tax_AfterDelete
AFTER DELETE ON Tax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Tax',
    'DELETE',
    'DELETE FROM Tax WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Tax_AfterInsert
AFTER INSERT ON Tax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Tax',
    'INSERT',
    'INSERT INTO Tax (Id, Name, Rate, Code, IsFixed, IsTaxOnTotal, IsEnabled) VALUES ('
    || COALESCE(CAST(NEW.Id AS TEXT), 'NULL') || ', '
    || quote(COALESCE(NEW.Name, '')) || ', '
    || COALESCE(CAST(NEW.Rate AS TEXT), 'NULL') || ', '
    || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END || ', '
    || COALESCE(CAST(NEW.IsFixed AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsTaxOnTotal AS TEXT), 'NULL') || ', '
    || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL') || ');'
  );
END;");

            dbContext.Database.ExecuteSqlRaw(@"
CREATE TRIGGER Tax_AfterUpdate
AFTER UPDATE ON Tax
FOR EACH ROW
BEGIN
  INSERT INTO TableAuditLog (TableName, Operation, SqlStatement)
  VALUES (
    'Tax',
    'UPDATE',
    'UPDATE Tax SET '
    || 'Name = ' || quote(COALESCE(NEW.Name, ''))
    || ', Rate = ' || COALESCE(CAST(NEW.Rate AS TEXT), 'NULL')
    || ', Code = ' || CASE WHEN NEW.Code IS NULL THEN 'NULL' ELSE quote(NEW.Code) END
    || ', IsFixed = ' || COALESCE(CAST(NEW.IsFixed AS TEXT), 'NULL')
    || ', IsTaxOnTotal = ' || COALESCE(CAST(NEW.IsTaxOnTotal AS TEXT), 'NULL')
    || ', IsEnabled = ' || COALESCE(CAST(NEW.IsEnabled AS TEXT), 'NULL')
    || ' WHERE Id = ' || CAST(OLD.Id AS TEXT) || ';'
  );
END;");

            transaction.Commit();
        }
        catch (Exception ex)
        {
             System.Diagnostics.Debug.WriteLine(
        $"DbTriggerService.EnsureAuditInfrastructure failed: {ex.Message}");
        }
    }
}

