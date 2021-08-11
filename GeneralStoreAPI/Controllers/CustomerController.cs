using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Data_POCOs.Customers;
using System;
using System.Collections.Generic;
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
                return Ok();
            }

            return InternalServerError();
        }
    }
}
