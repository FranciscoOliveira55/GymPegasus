using AutoMapper;
using BackEnd.Configurations;
using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using BackEnd.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using JsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected readonly GymContext _context;
        protected readonly ILogger<UserController> _logger;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly IMapper _mapper;
        protected readonly UserManager<User> _userManager;
        protected readonly DbSet<User> table;

        public UserController(
            GymContext context,
            ILogger<UserController> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper,
            UserManager<User> userManager
            )
        {
            _context = context;
            _logger = logger;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
            _mapper = mapper;
            table = _context.Set<User>();
            _userManager = userManager;
        }

        //GET: api/[controller]
        /// <summary>
        /// Reads all the Users in the database (in the table)
        /// </summary>
        /// <returns>A list of the items in the database (in the table), or 404 if no items are found</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserQueryDTO>>> ReadAll()
        {
            // Read all Users from table
            if (table == null)
                return NotFound();

            var readedUsers = await table.OrderBy(user => user.Email).ToListAsync();

            if (!readedUsers.Any())
                return NotFound();

            List<UserQueryDTO> userQueryDTOs = new();
            foreach (var readedUser in readedUsers)
            {
                userQueryDTOs.Add(_mapper.Map<UserQueryDTO>(readedUser));
            }
            return Ok(userQueryDTOs);
        }

        /// <summary>
        /// Reads an user in the database
        /// </summary>
        /// <param name="id">The id of the user to read</param>
        /// <returns>The user readed (or 404 response if the item is not found)</returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserQueryDTO>> ReadById(string id)
        {
            var readedUser = await _userManager.FindByIdAsync(id);
            if (readedUser == null)
                return NotFound();

            var userQueryDTO = _mapper.Map<UserQueryDTO>(readedUser);

            return Ok(userQueryDTO);
        }

        /// <summary>
        /// Reads an user in the database
        /// </summary>
        /// <param name="email">The email of the user to read</param>
        /// <returns>The user readed (or 404 response if the item is not found)</returns>
        [Authorize]
        [HttpGet("Email/{email}")]
        public async Task<ActionResult<UserQueryDTO>> ReadByEmail(string email)
        {
            var readedUser = await _userManager.FindByEmailAsync(email);

            if (readedUser == null)
                return NotFound();

            var userQueryDTO = _mapper.Map<UserQueryDTO>(readedUser);

            return Ok(userQueryDTO);
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRegisterBODTO userDTO)
        {
            _logger.LogInformation($"Back Office Registration Attempt for {userDTO}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //Create User
                var user = _mapper.Map<User>(userDTO);
                var userResult = await _userManager.CreateAsync(user, userDTO.Password);

                if (userResult.Succeeded)
                {
                    if (userDTO.Roles != null && userDTO.Roles.Any())
                    {
                        //Adds Roles
                        foreach (var role in userDTO.Roles)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, role);

                            if (roleResult.Succeeded)
                            {
                                //Create Empty Cliente with the UserId
                                if (role.Equals("Cliente"))
                                {
                                    Cliente emptyCliente = new() { UserId = user.Id };
                                    _context.Clientes.Add(emptyCliente);
                                    await _context.SaveChangesAsync();
                                } //Create Empty Employee with the UserId
                                else if (role.Equals("Employee"))
                                {
                                    Employee emptyEmployee = new()
                                    { UserId = user.Id, Position = EmployeePosition.Generic };
                                    _context.Employees.Add(emptyEmployee);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                                return Problem($"User Role Registration Attempt Failed");
                        }
                    }
                }
                else
                    return Problem($"User Registration Attempt Failed");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
            return Ok(userDTO);
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
        public async Task<IActionResult> UpdateById(string id, [FromBody] UserQueryDTO userDTO)
        {
            try
            {
                //Don't let update User1 or Admin1
                if ((id).Equals("1") ||
                    (id).Equals("3"))
                {
                    return Problem("You can't update the original Admin or User");
                }


                // Validate and Update an Object to the Database
                if (!(userDTO.Id).Equals(id))
                    return BadRequest();

                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                    return NotFound();

                user.UserName = userDTO.UserName;
                user.Email = userDTO.Email;
                user.PhoneNumber = userDTO.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                    return Ok();
                else
                    return Problem("Problem Updating User");
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
        public async Task<IActionResult> DeleteById(string id)
        {
            try
            {
                //Don't let delete User1 or Admin1
                if ((id).Equals("1") ||
                    (id).Equals("3"))
                {
                    return Problem("You can't remove the original Admin or User");
                }
                var deletedUser = await _userManager.FindByIdAsync(id);

                var result = await _userManager.DeleteAsync(deletedUser);

                if (result.Succeeded)
                    return Ok();
                else
                    return Problem("Problem Deleting User");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }
    }
}
