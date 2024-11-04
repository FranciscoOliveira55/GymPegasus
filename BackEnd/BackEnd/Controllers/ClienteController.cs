using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : GenericCrudController<Cliente>
    {
        public ClienteController(
            GymContext context,
            ILogger<GenericCrudController<Cliente>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }




        //PUT: api/[controller]/{id}
        /// <summary>
        /// Updates the ClienteTicks
        /// </summary>
        /// <param name="id">The id of the item to update</param>
        /// <param name="updatedObject">The updated item</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("UpdateSubscription/{id}")]
        public async Task<IActionResult> UpdateSubscriptionById(int id, [FromBody] long subscriptionTimeTicks)
        {
            // Validate and Update an Object to the Database
            //if (!(updatedObject.Id).Equals(id)) //maybe add id to EntityBase
            //    return BadRequest();

            //Updates only marked properties
            Cliente cliente = await table.FindAsync(id);

            if (cliente != null)
            {
                if (cliente.SubscriptionExpiration > DateTime.Now)
                    cliente.SubscriptionExpiration += TimeSpan.FromTicks(subscriptionTimeTicks);
                else
                    cliente.SubscriptionExpiration = DateTime.Now + TimeSpan.FromTicks(subscriptionTimeTicks);
                table.Entry(cliente).Property("SubscriptionExpiration").IsModified = true;
            }
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error updating the object.\n{e.Message}");
            }
        }

    }
}
