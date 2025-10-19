using System;
namespace DataServiceLayer.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? ShipName { get; set; }
    }
}
