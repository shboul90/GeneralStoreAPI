using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Data_POCOs.Products;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly StoreDBContext _context = new StoreDBContext();

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Product product)
        {
            if (!ModelState.IsValid || product is null)
            {
                return BadRequest();
            }

            var productEntity = new Product
            {
                Name = product.Name,
                Cost = product.Cost,
                NumberInInventory = product.NumberInInventory,
            };

            _context.Products.Add(productEntity);

            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok($"Product: {productEntity.Name} was adeed to the database");
            }

            return InternalServerError();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] string sku)
        {
            var product = await _context.Products.SingleOrDefaultAsync(d => d.SKU == sku);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
            //_context.Dogs.Single(d => d.ID == id); will blow up app
        }
    }
}
