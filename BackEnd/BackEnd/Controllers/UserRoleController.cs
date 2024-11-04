using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using BackEnd.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text.Json;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        protected readonly GymContext _context;
        protected readonly ILogger<UserController> _logger;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly IMapper _mapper;
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly DbSet<IdentityUserRole<string>> table;

        public UserRoleController(
            GymContext context,
            ILogger<UserController> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _logger = logger;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
            _mapper = mapper;
            table = _context.UserRoles;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleReadedDTO>>> ReadAll()
        {
            // Read all Users from table
            if (table == null)
                return NotFound();

            var readedUserRoles = await _context.UserRoles
                .Join(
                    _context.Users,
                    ur => ur.UserId,
                    u => u.Id,
                    (ur, u) => new { UserRole = ur, User = u }
                )
                .Join(
                    _context.Roles,
                    joined => joined.UserRole.RoleId,
                    r => r.Id,
                    (joined, r) => new UserRoleReadedDTO
                    {
                        UserName = joined.User.UserName,
                        Email = joined.User.Email,
                        UserId = joined.UserRole.UserId,
                        RoleId = joined.UserRole.RoleId,
                        Role = r.Name
                    }
                )
                .OrderBy(joined => joined.Email)
                .ToListAsync();

            return Ok(readedUserRoles);
        }


        /// <summary>
        /// Adds a Role to an User
        /// </summary>
        /// <param name="userRoleDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddRoleToUserByEmail([FromBody] UserRoleDTO userRoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var user = await _userManager.FindByEmailAsync(userRoleDTO.UserEmail);
                //Adds Roles
                if (user != null)
                {
                    //If an user already has a role, deny the operation
                    var rolesOfUser = await _userManager.GetRolesAsync(user);
                    if (!rolesOfUser.IsNullOrEmpty())
                    {
                        return Problem("An User can't have multiple roles");
                    }

                    var roleResult = await _userManager.AddToRoleAsync(user, userRoleDTO.RoleName);
                    if (!roleResult.Succeeded)
                        return Problem($"User Role Registration Attempt Failed");
                }
                return Ok(userRoleDTO);
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
        [HttpDelete("{userId}/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUserById(string userId, string roleId)
        {
            try
            {
                //Don't let delete role from User1 or Admin1
                if ((userId).Equals("1") ||
                    (userId).Equals("3"))
                {
                    return Problem("You can't remove role from the original Admin or User");
                }

                var userToRemoveRole = await _userManager.FindByIdAsync(userId);
                var roleToRemove = await _roleManager.FindByIdAsync(roleId);

                var result = await _userManager.RemoveFromRoleAsync(userToRemoveRole, roleToRemove.Name);

                if (result.Succeeded)
                    return Ok();
                else
                    return Problem("Problem Removing Role From User");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred: {e.Message}");
            }
        }



    }
}
