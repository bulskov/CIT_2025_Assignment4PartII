using DataServiceLayer.DTOs;
using DataServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLayer;

public class DataService
{
    private readonly NorthwindContext _db;

    public DataService(NorthwindContext db)
    {
        _db = db;
    }
    public DataService()
    {
        _db = new NorthwindContext();
    }

    // -------------------- Categories --------------------

    // 1) All categories
    public List<CategoryDto> GetCategories()
    {
        var x = _db.Categories
           .ToList();

        var z = x.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,

        });
        return z.ToList();
    }

    // 2) Category by ID
    public CategoryDto GetCategory(int id)
    {
        return _db.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .AsNoTracking()
            .FirstOrDefault();
    }

    // 3) Add category
    public CategoryDto CreateCategory(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        var entity = new Category
        {
            Name = name,
            Description = description
        };

        _db.Categories.Add(entity);
        _db.SaveChanges(); // sync

        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    // 4) Delete category (true if deleted; false if not found)
    public bool DeleteCategory(int id)
    {
        var entity = _db.Categories.Find(id);
        if (entity is null) return false;

        _db.Categories.Remove(entity);
        _db.SaveChanges(); // sync
        return true;
    }

    // 5) Update category (true if updated; false if not found)
    public bool UpdateCategory(int id, string name, string description)
    {
        var entity = _db.Categories.Find(id);
        if (entity is null) return false;

        entity.Name = name;
        entity.Description = description;

        _db.SaveChanges(); // synchronous save
        return true;
    }

    // -------------------- Products --------------------

    // 6) Product by ID (with category name)

    public ProductDto GetProduct(int id)
    {
        return _db.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitsInStock = p.UnitsInStock,
                Category = p.Category == null
                    ? null
                    : new CategoryDto
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        Description = p.Category.Description
                    }
            })
            .AsNoTracking()
            .FirstOrDefault();

    }

    // 7) Products by category ID (same shape as #6)
    public List<ProductDto> GetProductByCategory(int categoryId)
    {
        return _db.Products
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Id) // ensures First() is "Chai" and Last() is "Lakkalikööri"
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitsInStock = p.UnitsInStock,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .AsNoTracking()
            .ToList();
    }

    // 8) Products containing substring (name + category name only)
    public List<ProductNameWithCategoryDto> GetProductByName(string substring)
    {
        if (string.IsNullOrWhiteSpace(substring)) return new();

        var pattern = $"%{substring.Trim()}%";

        return _db.Products
            .Where(p => EF.Functions.ILike(p.Name, pattern))
            .OrderBy(p => p.Id) // ensures First: NuNuCa…, Last: Flotemysost
            .Select(p => new ProductNameWithCategoryDto
            {
                ProductName = p.Name,
                CategoryName = p.Category != null ? p.Category.Name : null
            })
            .AsNoTracking()
            .ToList();
    }
    // -------------------- Orders --------------------


    public OrderForTestDto GetOrder(int id)
    {
        return _db.Orders
            .Where(o => o.Id == id)
            .Select(o => new OrderForTestDto
            {
                Id = o.Id,
                OrderDetails = o.OrderDetails
                    .OrderBy(od => od.ProductId) // keeps a stable order so First() is deterministic
                    .Select(od => new OrderDetailForTestDto
                    {
                        Product = new ProductForTestDto
                        {
                            Name = od.Product.Name,
                            Category = new CategoryDto
                            {
                                Id = od.Product.Category.Id,
                                Name = od.Product.Category.Name,
                                Description = od.Product.Category.Description
                            }
                        }
                    })
                    .ToList()
            })
            .AsNoTracking()
            .FirstOrDefault();
    }

    // 1) Get single order by ID (with details -> product -> category)
    public Task<OrderWithDetailsDto> GetOrderByIdAsync(int id) =>
        _db.Orders
           .Where(o => o.Id == id)
           .Select(o => new OrderWithDetailsDto
           {
               Id = o.Id,
               OrderDate = o.Date,
               ShipName = o.ShipName,
               ShipCity = o.ShipCity,
               Details = o.OrderDetails.Select(od => new OrderDetailWithProductDto
               {
                   ProductId = od.ProductId,
                   ProductName = od.Product.Name,
                   UnitPrice = od.UnitPrice,
                   Quantity = od.Quantity,
                   Discount = od.Discount,
                   CategoryName = od.Product.Category != null
                       ? o.OrderDetails.Select(x => x.Product.Category!.Name)
                                       .FirstOrDefault(_ => true) // safe access
                       : null
               }).ToList()
           })
           .AsNoTracking()
           .FirstOrDefaultAsync();

    // 2) Get orders by shipping name (summary shape)
    public Task<List<OrderSummaryDto>> GetOrdersByShipNameAsync(string shipName)
    {
        shipName ??= string.Empty;
        var pattern = $"%{shipName.Trim()}%";

        return _db.Orders
            .Where(o => EF.Functions.ILike(o.ShipName ?? string.Empty, pattern))
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderDate = o.Date,
                ShipName = o.ShipName,
                ShipCity = o.ShipCity
            })
            .AsNoTracking()
            .ToListAsync();
    }

    // 3) List all orders (same summary shape)
    public List<OrderSummaryDto> GetOrders()
    {
        return _db.Orders
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderDate = o.Date,
                ShipName = o.ShipName,
                ShipCity = o.ShipCity
            })
            .AsNoTracking()
            .ToList();
    }
    // -------------------- Order Details --------------------

    // 4) Details by Order ID (include product info)
    public List<OrderDetailWithProductDto> GetOrderDetailsByOrderId(int orderId)
    {
        return _db.OrderDetails
            .Where(od => od.OrderId == orderId)
            .Select(od => new OrderDetailWithProductDto
            {
                ProductId = od.ProductId,
                ProductName = od.Product.Name,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Discount = od.Discount,
                CategoryName = od.Product.Category != null
                    ? od.Product.Category.Name
                    : null
            })
            .AsNoTracking()
            .ToList();
    }


    // 5) Details by Product ID (include product & order; order by OrderId)
    public List<OrderDetailForProductDto> GetOrderDetailsByProductId(int productId)
    {
        return _db.OrderDetails
            .AsNoTracking()
            .Where(od => od.ProductId == productId)
            .OrderBy(od => od.Order.Date)     // <— primary sort so First() matches the test
            .ThenBy(od => od.OrderId)         // tiebreaker to keep it deterministic
            .Select(od => new OrderDetailForProductDto
            {
                Order = new OrderForTestDto
                {
                    Date = (DateTime)od.Order.Date      // use your actual property name (Date vs OrderDate)
                },
                UnitPrice = od.UnitPrice,     // from orderdetails (not product)
                Quantity = od.Quantity
            })
            .ToList();
    }
}
}
