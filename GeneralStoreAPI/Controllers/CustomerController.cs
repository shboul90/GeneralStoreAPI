using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Data_POCOs.Customers;
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
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] Customer customer)
        {
            if (!ModelState.IsValid || customer is null)
            {
                return BadRequest();
            }

            var customerEntity = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
            };

            _context.Customers.Add(customerEntity);

            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok($"{customer.FullName} has been added successfully!");
            }

            return InternalServerError();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var customers = await _context.Customers.ToListAsync();

            return Ok(customers);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] Customer newCustomerData, [FromUri] int id)
        {
            if (newCustomerData.ID != id || newCustomerData is null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            customer.FirstName = newCustomerData.FirstName;
            customer.LastName = newCustomerData.LastName;

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"Customer ID: {customer.ID} has been updated to {customer.FullName}");
            }

            return InternalServerError();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);

                if (await _context.SaveChangesAsync() == 1)
                {
                    return Ok($"Customer ID: {id} has been removed");
                }
            }

            return InternalServerError();
        }
    }
}
