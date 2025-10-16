

namespace DataServiceLayer.Models;

public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }
    public required string QuantityPerUnit { get; set; }
    public int UnitPrice { get; set; }
    public int UnitsInStock { get; set; }

    public Category? Category { get; set; }
}