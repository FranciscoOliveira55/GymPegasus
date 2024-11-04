using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class GenericCrudController<T> : ControllerBase where T : EntityBase
    {
        protected readonly GymContext _context;
        protected readonly DbSet<T> table;
        protected readonly ILogger<GenericCrudController<T>> _logger;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly IMapper _mapper;

        public GenericCrudController(
            GymContext context,
            ILogger<GenericCrudController<T>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper)
        {
            _context = context; //database context
            _logger = logger;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
            _mapper = mapper;
            table = _context.Set<T>(); //table of database (setted by the child classes)
        }


        //GET: api/[controller]
        /// <summary>
        /// Reads all the items in the database (in the table)
        /// </summary>
        /// <returns>A list of the items in the database (in the table), or 404 if no items are found</returns>
        [Authorize]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> ReadAll([FromQuery] string[]? includeEntities)
        {
            try
            {
                // Read all Objects from table
                if (table == null)
                    return NotFound();

                //Includes Related Entites if Needed
                foreach (var includeEntity in includeEntities)
                {
                    //Checks if property exists
                    string includeEntityChecked = char.ToUpper(includeEntity[0]) + includeEntity.Substring(1);
                    var includeProperty = typeof(T).GetProperty(includeEntityChecked, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (includeProperty == null)
                        return BadRequest("Invalid includeEntityName");
                    table.Include(includeEntityChecked).ToList();
                }

                var readedObjects = await table.ToListAsync();

                if (!readedObjects.Any())
                    return NotFound();

                //string readedJsonObjects = JsonSerializer.Serialize(readedObjects, jsonOptions);

                return Ok(readedObjects);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


        //GET: api/[controller]/{id}
        /// <summary>
        /// Reads an item in the database
        /// </summary>
        /// <param name="id">The id of the item to read</param>
        /// <returns>The item readed (or 404 response if the item is not found)</returns>
        [Authorize]
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> ReadById(int id, int? secondId, [FromQuery] string[]? includeEntities)
        {
            try
            {
                // Read a Object from Database with an Id
                if (table == null)
                    return NotFound();

                //Includes Related Entites if Needed
                foreach (var includeEntity in includeEntities)
                {
                    //Checks if property exists
                    string includeEntityChecked = char.ToUpper(includeEntity[0]) + includeEntity.Substring(1);
                    var includeProperty = typeof(T).GetProperty(includeEntityChecked, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (includeProperty == null)
                        return BadRequest("Invalid includeEntityName");
                    table.Include(includeEntityChecked).ToList();
                }

                //var readedObject = await table.SingleOrDefaultAsync(s => s.Id == id);
                T readedObject = null;
                if (secondId == null)
                    readedObject = await table.FindAsync(id);
                else
                    readedObject = await table.FindAsync(id, secondId);

                if (readedObject == null)
                    return NotFound();

                return Ok(readedObject);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }

        /// <summary>
        /// Reads items in the database with an certain property value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{propertyName}/{value}")]
        public virtual async Task<ActionResult<IEnumerable<T>>> ReadByProperty(string propertyName, string value, [FromQuery] string[]? includeEntities)
        {
            try
            {
                // Read a Object from Database with an Id
                if (table == null)
                    return NotFound();

                //Includes Related Entites if Needed
                foreach (var includeEntity in includeEntities)
                {
                    //Checks if property exists
                    string includeEntityChecked = char.ToUpper(includeEntity[0]) + includeEntity.Substring(1);
                    var includeProperty = typeof(T).GetProperty(includeEntityChecked, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (includeProperty == null)
                        return BadRequest("Invalid includeEntityName");
                    table.Include(includeEntityChecked).ToList();
                }

                var tableName = typeof(T).Name + "s"; // Get the table name for the specified entity type

                //Checks if property exists 
                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    return BadRequest("Invalid propertyName");

                //Selects the Item with the property
                var sqlQuery = $"SELECT * FROM {tableName} WHERE {propertyName} = @value";
                _logger.LogInformation($"SQL query:\t {sqlQuery}");

                var readedObjects = await _context.Set<T>()
                    .FromSqlRaw(sqlQuery, new[] { new SqlParameter("@value", value) })
                    .ToListAsync();

                if (!readedObjects.Any())
                    return NotFound();

                return Ok(readedObjects);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }


        //POST: api/[controller]
        /// <summary>
        /// Creates an item in the database
        /// </summary>
        /// <param name="createdObject">The item to create</param>
        /// <returns>The item created</returns>
        [Authorize]
        [HttpPost()]
        public virtual async Task<ActionResult<T>> Create([FromBody] T createdObject)
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


        //PUT: api/[controller]/{id}
        /// <summary>
        /// Updates an item in the database
        /// </summary>
        /// <param name="id">The id of the item to update</param>
        /// <param name="updatedObject">The updated item</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> UpdateById(int id, [FromBody] T updatedObject, [FromQuery] string[]? updatedProperties)
        {
            try
            {
                //Don't let update the original cliente or employee
                if ((typeof(T) == typeof(Cliente) && id==1)||
                    (typeof(T) == typeof(Employee) && id == 1))
                {
                    return Problem("You can't update the original Client or Employee");
                }

                // Validate and Update an Object to the Database
                //if (!(updatedObject.Id).Equals(id)) //maybe add id to EntityBase
                //    return BadRequest();


                if (!updatedProperties.Any())
                {
                    //Updates the whole object
                    table.Update(updatedObject);
                    //table.Entry(updatedObject).State = EntityState.Modified;
                }
                else
                {
                    //Updates only marked properties
                    foreach (var updatedProperty in updatedProperties)
                    {
                        Console.WriteLine("Property");

                        //Makes Property Capital Case
                        string updatedPropertyCapsChecked = char.ToUpper(updatedProperty[0]) + updatedProperty.Substring(1);
                        //Checks if T has that property
                        var updatedPropertyChecked = typeof(T).GetProperty(updatedPropertyCapsChecked, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (updatedPropertyChecked == null)
                            return BadRequest("Invalid updatedPropertyName");
                        //If T has that property, modifies it
                        table.Entry(updatedObject).Property(updatedPropertyCapsChecked).IsModified = true;
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }

        //DELETE: api/[controller]/{id}
        /// <summary>
        /// Deletes an item in the database
        /// </summary>
        /// <param name="id">The id of the item to delete</param>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteById(int id, int? secondId)
        {
            try
            {
                // Validate and Delete an Object from the Database
                if (table == null)
                    return NotFound();

                T deletedObject = null;

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
