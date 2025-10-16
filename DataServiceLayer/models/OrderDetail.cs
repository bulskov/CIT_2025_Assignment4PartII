using System;
namespace DataServiceLayer.Models;

public class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int Discount { get; set; }
    
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}