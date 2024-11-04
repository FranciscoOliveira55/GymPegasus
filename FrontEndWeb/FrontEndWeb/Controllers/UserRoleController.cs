using FrontEndWeb.Configurations;
using FrontEndWeb.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class UserRoleController : Controller
    {
        private string controllerName;
        protected readonly IHttpClientFactoryWithJwtService _httpClientFactoryWithJwtService;
        protected readonly JsonSerializerOptions _jsonOptions;

        public UserRoleController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IOptions<JsonOptions> jsonOptions)
        {
            _httpClientFactoryWithJwtService = httpClientFactoryWithJwtService; //start httpClientFactory so that it can be used in the methods
            this.controllerName = "UserRole";//Name of the controller to be added to the baseUrl to call the backend endpoint
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }




        // GET: [Controller]/Index
        /// <summary>
        /// Asks the backend for all the items in the database (in the table)
        /// </summary>
        /// <returns>A view with the list of the items in the database (in the table), or 404 if no items are found</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOIndex()
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectListStringReceived = await response.Content.ReadAsStringAsync();
                var objectListReceived = JsonSerializer.Deserialize<IEnumerable<UserRoleReadedBODTO>>(objectListStringReceived, _jsonOptions);

                return View(objectListReceived);
            }
            return NoContent();
        }


        // GET: [Controller]/Create
        /// <summary>
        /// Returns the view named Create
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOCreate()
        {
            return View();
        }


        // POST: [Controller]/Create
        /// <summary>
        /// Sends an item to the backend to be created in the database
        /// </summary>
        /// <param name="objectCreated">The item to be created in the database</param>
        /// <returns>Redirects to the Index view (list all items) or shows error description</returns>
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BOCreate(UserRoleDTO objectCreated, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            try
            {
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(usedUrl, objectCreated);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(controllerRedirect))
                        return RedirectToAction(actionRedirect, controllerRedirect, new { id = idRedirect ?? 0 });
                    else
                        return RedirectToAction(actionRedirect, new { id = idRedirect ?? 0 });
                }

                return Content(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Content($"Falha a Criar {controllerName}, ver Consola");
            }
        }




        // GET: [Controller]/Delete/{id}
        /// <summary>
        /// Sends an item to the backend to be deleted in the database
        /// </summary>
        /// <param name="id">The id of the item to be deleted</param>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BODelete(string userId, string roleId, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{userId}/{roleId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.DeleteAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(controllerRedirect))
                    return RedirectToAction(actionRedirect, controllerRedirect, new { id = idRedirect ?? 0 });
                else
                    return RedirectToAction(actionRedirect, new { id = idRedirect ?? 0 });
            }
            return Content(await response.Content.ReadAsStringAsync());
        }


    }
}
