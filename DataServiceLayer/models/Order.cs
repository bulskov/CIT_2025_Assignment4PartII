using System;
using System.Collections.Generic;
namespace DataServiceLayer.Models;


public class Order
{
public int Id { get; set; }
public required string CustomerId { get; set; }
public int EmployeeId { get; set; }
public DateTime Date { get; set; }
public DateTime Required { get; set; }
public DateTime? ShippedDate { get; set; }
public int Freight { get; set; }
public required string ShipName { get; set; }
public required string ShipAddress { get; set; }
public required string ShipCity { get; set; }
public required string ShipPostalCode { get; set; }
public required string ShipCountry { get; set; }     
   public ICollection<OrderDetails>? OrderDetails { get; set; }


}