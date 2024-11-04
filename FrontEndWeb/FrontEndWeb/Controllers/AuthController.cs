using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using FrontEndWeb.Models.DTOs;
using NuGet.Protocol;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FrontEndWeb.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;


namespace FrontEndWeb.Controllers
{
    public class AuthController : Controller
    {
        protected readonly IHttpClientFactoryWithJwtService _httpClientFactoryWithJwtService;
        protected string? controllerName = "Auth"; //controllerName to be added to the baseUrl to connect to the backend endpoint
        protected readonly JsonSerializerOptions _jsonOptions;

        public AuthController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IOptions<JsonOptions> jsonOptions)
        {
            _httpClientFactoryWithJwtService = httpClientFactoryWithJwtService; //start httpClientFactory so that it can be used in the methods
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }


        /// <summary>
        /// View to Register a User Cliente
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Post to Register a User Cliente
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        // GET: AuthController/Create
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDTO userDTO)
        {
            try
            {
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/RegisterUserCliente", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(usedUrl, userDTO);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Login));
                }
                return Problem(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return Problem($"Falha a registar, ver Consola,\n {e.Message}", statusCode: 500);
            }
        }


        /// <summary>
        /// View to Login a User
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Post to Login a User
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDTO userDTO)
        {
            try
            {
                //Makes the Request to the BackEnd
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/Login", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(usedUrl, userDTO);

                //If Login is Successfull
                if (response.IsSuccessStatusCode)
                {
                    //Deserializes Response
                    var objectStringReceived = await response.Content.ReadAsStringAsync();
                    var userLoginResponseDTO = JsonSerializer.Deserialize<UserLoginResponseDTO>(objectStringReceived, _jsonOptions);

                    //Makes Claims
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Sid, userLoginResponseDTO.Id), // Example: Storing UserId
                        new(ClaimTypes.Name, userLoginResponseDTO.Email),
                        new(ClaimTypes.Email, userLoginResponseDTO.Email), // Example: Storing UserEmail
                        new("jwt", userLoginResponseDTO.Token) // Storing the JWT token
                        // Additional claims can be added based on the received user information
                    };
                    //Adds Roles to the Claims
                    foreach (var role in userLoginResponseDTO.Roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));

                    //Adds ClienteId or EmployeeId to the claims
                    if (userLoginResponseDTO.ClienteId != -1)
                        claims.Add(new Claim("ClienteId", userLoginResponseDTO.ClienteId.ToString()));
                    if (userLoginResponseDTO.EmployeeId != -1)
                        claims.Add(new Claim("EmployeeId", userLoginResponseDTO.EmployeeId.ToString()));

                    //Builds Principal With the Claims
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    //SignsIn with the Principal (creates cookie for authentication)
                    await HttpContext.SignInAsync(principal);


                    //If the User is Cliente, is redirected to Index, if itn't Cliente and is Employee then, goes to BOIndex
                    if(principal.IsInRole("Cliente"))
                        return RedirectToAction("Index", "Home"); // Redirect to authenticated page
                    else if(principal.IsInRole("Employee"))
                        return RedirectToAction("BOIndex", "Home"); // Redirect to authenticated page
                }
                return Unauthorized(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return Problem($"Falha a logar, ver Consola,\n {e.Message}", statusCode: 500);
            }
        }


        // GET: AuthController/Create
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //SignsOut (removes authentication cookie from the browser)
            await HttpContext.SignOutAsync();

            return RedirectToAction("IndexVisitor", "Home"); // Redirect to authenticated page
        }
    }
}
