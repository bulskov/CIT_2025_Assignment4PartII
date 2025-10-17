using DataServiceLayer.Models;
using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

namespace DataServiceLayer
{
    public class NorthWindContext : DbContext
    {
        public NorthWindContext(DbContextOptions<NorthWindContext> options) : base(options) { }
        public NorthWindContext() { } // allow parameterless for quick local use

        public DbSet<Category> Categories
        { get; set; }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetails> OrderDetails => Set<OrderDetails>();

        protected override void OnConfiguring(DbContextOptionsBuilder b)
        {

            b.UseNpgsql("Host=localhost;Database=northwind;Username=postgres;Password=2RJ&Ao2g@qJy0Vk6;Include Error Detail=true")

            .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder m)
        {
            // categories

            m.Entity<Category>(e =>
            {
                e.ToTable("categories");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("categoryid").ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasColumnName("categoryname").IsRequired();
                e.Property(x => x.Description).HasColumnName("description");
            });

            // products
            m.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("productid");
                e.Property(x => x.Name).HasColumnName("productname").IsRequired();
                e.Property(x => x.SupplierId).HasColumnName("supplierid");
                e.Property(x => x.CategoryId).HasColumnName("categoryid");
                e.Property(x => x.QuantityPerUnit).HasColumnName("quantityperunit");
                e.Property(x => x.UnitPrice).HasColumnName("unitprice");
                e.Property(x => x.UnitsInStock).HasColumnName("unitsinstock");

                e.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId);

                //e.HasOne(x => x.Supplier)
                //.WithMany()
                //.HasForeignKey(x => x.SupplierId);
            });

            // orders
            m.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("orderid");
                e.Property(x => x.CustomerId).HasColumnName("customerid");
                e.Property(x => x.EmployeeId).HasColumnName("employeeid");
                e.Property(x => x.Date).HasColumnName("orderdate");
                e.Property(x => x.Required).HasColumnName("requireddate");
                e.Property(x => x.ShippedDate).HasColumnName("shippeddate");
                e.Property(x => x.Freight).HasColumnName("freight");
                e.Property(x => x.ShipName).HasColumnName("shipname");
                e.Property(x => x.ShipAddress).HasColumnName("shipaddress");
                e.Property(x => x.ShipCity).HasColumnName("shipcity");
                e.Property(x => x.ShipPostalCode).HasColumnName("shippostalcode");
                e.Property(x => x.ShipCountry).HasColumnName("shipcountry");
            });

            // orderdetails (composite key)
            m.Entity<OrderDetails>(e =>
            {
                e.ToTable("orderdetails");
                e.HasKey(x => new { x.OrderId, x.ProductId });
                e.Property(x => x.OrderId).HasColumnName("orderid");
                e.Property(x => x.ProductId).HasColumnName("productid");
                e.Property(x => x.UnitPrice).HasColumnName("unitprice");
                e.Property(x => x.Quantity).HasColumnName("quantity");
                e.Property(x => x.Discount).HasColumnName("discount");

                e.HasOne(x => x.Order).WithMany(o => o.OrderDetails).HasForeignKey(x => x.OrderId);
                e.HasOne(x => x.Product).WithMany(p => p.OrderDetails).HasForeignKey(x => x.ProductId);
            });
        }
    }

}

    

