using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : GenericCrudController<Employee>
    {
        public EmployeeController(
            GymContext context,
            ILogger<GenericCrudController<Employee>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }



        /// <summary>
        /// Reads Employees in the database with the id
        /// Includes Reviews and the Cliente of each Review
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("EmployeeWithReviews/{id}")]
        public async Task<ActionResult<IEnumerable<Employee>>> ReadEmployeeWithReviews(int id)
        {
            // Read a Object from Database with an Id
            if (table == null)
                return NotFound();

            //Includes Related Entites if Needed
            table.Include(employee => employee.Reviews) // Include the Reviews collection for each Cliente
                .ThenInclude(review => review.Cliente)
                .ToList(); // Then include the Employee for each Review

            //Selects the Item with the property
            try
            {
                var readedObject = await table.FindAsync(id);

                if (readedObject == null)
                    return NotFound();

                return Ok(readedObject);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


    }
}
