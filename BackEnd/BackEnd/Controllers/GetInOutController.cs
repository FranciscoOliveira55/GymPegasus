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
    [Route("api/[controller]")]
    [ApiController]
    public class GetInOutController : GenericCrudController<GetInOut>
    {
        public GetInOutController(
            GymContext context,
            ILogger<GenericCrudController<GetInOut>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }

        [AllowAnonymous]
        [HttpGet("CreateWithQRCode/{clienteId}/{getInOutType}")]
        public async Task<ActionResult> CreateWithQRCode(int clienteId, GetInOutType getInOutType)
        {
            // Validate and Add an Object to the Database
            GetInOut createdObject = new()
            {
                ClienteId = clienteId,
                GetInOutType = getInOutType,
                DateTime = DateTime.UtcNow
            };

            table.Add(createdObject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Create),
                new
                {
                    CanAcess = true,
                    ClienteId = createdObject.ClienteId,
                    GetInOutType = createdObject.GetInOutType,
                    DateTime = createdObject.DateTime
                });
        }
    }
}
