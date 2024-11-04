using AutoMapper;
using BackEnd.Configurations;
using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly GymContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthController> logger,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings,
            GymContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }


        [HttpPost]
        [Route("RegisterUserCliente")]
        public async Task<IActionResult> RegisterUserCliente([FromBody] UserRegisterDTO userDTO, bool createCliente = true)
        {
            _logger.LogInformation($"Registration Attempt for {userDTO}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //Create User
                var user = _mapper.Map<User>(userDTO);
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                //Adds Role
                var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");

                if (!result.Succeeded || !roleResult.Succeeded)
                {
                    return BadRequest($"Registration Attempt Failed");
                }

                //Create Cliente with the UserId
                if (createCliente)
                {
                    Cliente emptyCliente = new() { UserId = user.Id };
                    _context.Clientes.Add(emptyCliente);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return Problem($"Something went wrong in the {nameof(RegisterUserCliente)}\n {e.Message}", statusCode: 500);
            }
            return Ok(userDTO);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserLoginResponseDTO>> Login([FromBody] UserLoginDTO userDTO)
        {
            _logger.LogInformation($"Login Attempt for {userDTO}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //Checks if the user credentials are valid
                //var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
                var user = await _userManager.FindByEmailAsync(userDTO.Email);

                if (user == null)
                    return Unauthorized($"Email Not Found\n{userDTO}");

                var result = await _signInManager.CheckPasswordSignInAsync(user, userDTO.Password, false);
                //var result = await _userManager.CheckPasswordAsync(user, userDTO.Password);

                if (!result.Succeeded)
                    return Unauthorized($"Passoword Incorrect\n{userDTO}");
                else
                    _logger.LogInformation($"Login successfully: {user.Email}");


                //Gets data for the Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

                //Gets the claims for the token
                var claims = new List<Claim>()
                {
                    new(ClaimTypes.Sid, user.Id),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                };

                //Adds the roles of the user to the claims
                //Don't let loggin if it doesn't have any role
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.IsNullOrEmpty())
                    foreach (var role in roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));
                else
                    return Unauthorized("You don't have any role assigned, contact support!");

                //Adds the data to the Token Descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "",
                    Audience = "",
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };
                //It doesn't have public key, so the api is the only one who will see the claims
                //Generates the token
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                //Prepares user DTO to the FrontEnd
                var userLoginResponseDTO = new UserLoginResponseDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Token = tokenString,
                };
                //Adds Roles if they exist
                if (roles != null)
                    userLoginResponseDTO.Roles = roles;

                //Sees if Cliente with the UserId exists
                var readedCliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (readedCliente != null)
                    userLoginResponseDTO.ClienteId = readedCliente.Id;
                //Sees if Employee with the UserId exists
                var readedEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id);
                if (readedEmployee != null)
                    userLoginResponseDTO.EmployeeId = readedEmployee.Id;

                //Return Data to the FrontEnd
                return Ok(userLoginResponseDTO);
            }
            catch (Exception e)
            {
                return Problem($"Something went wrong in the {nameof(Login)},\n {e.Message}", statusCode: 500);
            }
        }


    }
}
