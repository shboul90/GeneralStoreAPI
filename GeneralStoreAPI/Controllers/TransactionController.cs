using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Data_POCOs.Transactions;
using GeneralStoreAPI.Models.Data_POCOs.Products;
using GeneralStoreAPI.Models.Data_POCOs.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace GeneralStoreAPI.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly StoreDBContext _contentTransactions = new StoreDBContext();
        private readonly StoreDBContext _contentProducts = new StoreDBContext();
        private readonly StoreDBContext _contentCustomers = new StoreDBContext();

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Transaction transaction)
        {
            if (transaction is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _contentProducts.Products.FindAsync(transaction.ProductSKU) is null)
            {
                return BadRequest("Product doesnt exist");
            }

            if (await _contentCustomers.Customers.FindAsync(transaction.CustomerId) is null)
            {
                return BadRequest("Customer doesnt exist");
            }

            var product = await _contentProducts.Products.FindAsync(transaction.ProductSKU);

            if (!product.IsInStock)
            {
                return BadRequest("Sorry this product is out of stock...");
            }

            if (transaction.ItemCount > product.NumberInInventory)
            {
                return BadRequest($"Sorrt there is only {product.NumberInInventory} in stock please lower the amount requested");
            }

            product.NumberInInventory = product.NumberInInventory - transaction.ItemCount;

            var transactionEntity = new Transaction
            {
                ID = transaction.ID,
                CustomerId = transaction.CustomerId,
                ProductSKU = transaction.ProductSKU,
                ItemCount = transaction.ItemCount,
                DateOfTransaction = transaction.DateOfTransaction,
            };

            _contentTransactions.Transactions.Add(transactionEntity);

            if (await _contentTransactions.SaveChangesAsync() > 0 && await _contentProducts.SaveChangesAsync() > 0)
            {
                return Ok($"Transaction ID: {transactionEntity.ID} was added to the database successfully");
            }

            return InternalServerError();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var transactions = await _contentTransactions.Transactions.ToListAsync();

            return Ok(transactions);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var transactions = await _contentTransactions.Transactions.FindAsync(id);

            if (transactions is null)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int customerID, int param2)
        {
            if (customerID < 0)
            {
                return BadRequest();
            }

            var allTransactions = await _contentTransactions.Transactions.ToListAsync();

            if (allTransactions is null)
            {
                return NotFound();
            }

            List<Transaction> customerTransactions = new List<Transaction>();

            foreach(Transaction t in allTransactions)
            {
                if (t.CustomerId == customerID)
                {
                    customerTransactions.Add(t);
                }
            }

            if (customerTransactions is null)
            {
                return NotFound();
            }

            return Ok(customerTransactions);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromBody] DateTime DateFrom, [FromBody] DateTime DateTo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(DateFrom>=DateTo)
            {
                return BadRequest("DateFrom should be less than DateTo");
            }

            var allTransactions = await _contentTransactions.Transactions.ToListAsync();

            if (allTransactions is null)
            {
                return NotFound();
            }

            List<Transaction> dateRangeTransactions = new List<Transaction>();

            foreach (Transaction t in allTransactions)
            {
                if (t.DateOfTransaction >= DateFrom && t.DateOfTransaction <= DateTo)
                {
                    dateRangeTransactions.Add(t);
                }
            }

            if (dateRangeTransactions is null)
            {
                return NotFound();
            }

            return Ok(dateRangeTransactions);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string ProductSKU)
        {
            if (await _contentProducts.Products.FindAsync(ProductSKU) is null)
            {
                return BadRequest("Product doesnt exist");
            }

            var allTransactions = await _contentTransactions.Transactions.ToListAsync();

            if (allTransactions is null)
            {
                return NotFound();
            }

            List<Transaction> productTransactions = new List<Transaction>();

            foreach (Transaction t in allTransactions)
            {
                if (t.ProductSKU == ProductSKU)
                {
                    productTransactions.Add(t);
                }
            }

            if (productTransactions is null)
            {
                return NotFound();
            }

            double sales;
            double totalSales = 0;
            int itemCountTotal = 0;

            foreach (Transaction x in productTransactions)
            {
                sales = x.ItemCount * x.Product.Cost;
                totalSales = totalSales + sales;
                itemCountTotal = itemCountTotal + x.ItemCount;
            }

            return Ok($"The total sales for product ID: {ProductSKU} was ${totalSales} or a total number of {itemCountTotal}");
        }
    }
}
