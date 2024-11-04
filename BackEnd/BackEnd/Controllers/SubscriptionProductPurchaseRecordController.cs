using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionProductPurchaseRecordController : GenericCrudController<SubscriptionProductPurchaseRecord>
    {
        public SubscriptionProductPurchaseRecordController(
            GymContext context,
            ILogger<GenericCrudController<SubscriptionProductPurchaseRecord>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }


        //POST: api/[controller]
        /// <summary>
        /// Creates an item in the database
        /// </summary>
        /// <param name="createdObject">The item to create</param>
        /// <returns>The item created</returns>
        [AllowAnonymous]
        [HttpPost("PublicCreate/")]
        public virtual async Task<ActionResult> PublicCreate([FromBody] SubscriptionProductPurchaseRecord createdObject)
        {
            try
            {
                // Validate and Add an Object to the Database
                table.Add(createdObject);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Create), /*new { id = createdObject.Id },*/ createdObject);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


    }
}
