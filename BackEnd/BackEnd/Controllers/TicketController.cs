using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : GenericCrudController<Ticket>
    {
        public TicketController(
            GymContext context,
            ILogger<GenericCrudController<Ticket>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }

        [AllowAnonymous]
        [HttpGet("GetInScheduleWithQRCode/{clienteId}/{scheduleId}")]
        public async Task<ActionResult> GetInScheduleWithQRCode(int clienteId, int scheduleId)
        {
            // Validate the Ticket
            var readedTicket = await table.FindAsync(clienteId, scheduleId);

            //If ticket is Valid, can enter, otherwise, can't
            if (readedTicket != null)
                return Ok(new
                {
                    CanAcess = true,
                    ClienteId = clienteId,
                    ScheduleId = scheduleId,
                });
            else
                return Unauthorized(new
                {
                    CanAcess = false,
                    ClienteId = clienteId,
                    ScheduleId = scheduleId,
                });
        }



        //DELETE: api/[controller]/{id}
        /// <summary>
        /// Deletes an item in the database
        /// </summary>
        /// <param name="id">The id of the item to delete</param>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpDelete("Public/{id}")]
        public async Task<IActionResult> PublicDeleteById(int id, int? secondId)
        {
            try
            {
                // Validate and Delete an Object from the Database
                if (table == null)
                    return NotFound();

                Ticket deletedObject = null;

                if (secondId == null)
                    deletedObject = await table.FindAsync(id);
                else
                    deletedObject = await table.FindAsync(id, secondId);

                if (deletedObject == null)
                    return NotFound();

                table.Remove(deletedObject);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


    }
}

