using GeneralStoreAPI.Models.Data_POCOs.Customers;
using GeneralStoreAPI.Models.Data_POCOs.Products;
using GeneralStoreAPI.Models.Data_POCOs.Transactions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class StoreDBContext : DbContext
    {
        public StoreDBContext() : base("DefaultConnection")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}