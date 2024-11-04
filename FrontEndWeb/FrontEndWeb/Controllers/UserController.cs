using FrontEndWeb.Configurations;
using FrontEndWeb.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using FrontEndWeb.Models;

namespace FrontEndWeb.Controllers
{
    public class UserController : Controller
    {
        private string controllerName;
        protected readonly IHttpClientFactoryWithJwtService _httpClientFactoryWithJwtService;
        protected readonly JsonSerializerOptions _jsonOptions;

        public UserController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IOptions<JsonOptions> jsonOptions)
        {
            _httpClientFactoryWithJwtService = httpClientFactoryWithJwtService; //start httpClientFactory so that it can be used in the methods
            this.controllerName = "User";//Name of the controller to be added to the baseUrl to call the backend endpoint
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }

        /// <summary>
        /// Gives User Profile View
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.Sid).Value;

            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{userId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var userStringReceived = await response.Content.ReadAsStringAsync();

                var userReceived = JsonSerializer.Deserialize<UserQueryDTO>(userStringReceived, _jsonOptions);

                return View(userReceived);
            }
            return NoContent();
        }



        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.Sid).Value;

            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{userId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived =
                    await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<UserQueryDTO>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
            }
            return NoContent();
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserQueryDTO userEdited, string postActionName = nameof(Profile))
        {
            try
            {
                string userId = HttpContext.User.FindFirst(ClaimTypes.Sid).Value;

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/{userId}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(usedUrl, userEdited);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(postActionName, new { id = userId });
                }
                return Content(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Content($"Falha a Editar {controllerName}, ver Consola");
            }
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
                var objectListReceived = JsonSerializer.Deserialize<IEnumerable<UserQueryDTO>>(objectListStringReceived, _jsonOptions);

                return View(objectListReceived);
            }
            return NoContent();
        }


        // GET: [Controller]/Details/{id}
        /// <summary>
        /// Asks the backend for an item with an id in the database (in the table)
        /// </summary>
        /// <param name="id">The id of the item to be readed</param>
        /// <returns>A view with the item readed (or 404 if it is not found)</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BODetails(string id)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{id}", UriKind.Relative);

            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived = await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<UserQueryDTO>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
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
        public virtual async Task<IActionResult> BOCreate(UserRegisterBODTO objectCreated, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            try
            {
                return Json(objectCreated);

                //Removes Unchecked Roles (false)
                while (objectCreated.Roles != null && objectCreated.Roles.Contains("false"))
                {
                    objectCreated.Roles.Remove("false");
                }

                return Json(objectCreated);

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


        // GET: [Controller]/Edit/{id}
        /// <summary>
        /// Asks the backend if an item with an id exists (so i can update it)
        /// </summary>
        /// <param name="id">The id of the item to be updated</param>
        /// <returns>The view to update the item (if it exists in the database), 404 otherwise</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOEdit(string id)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{id}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived =
                    await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<UserQueryDTO>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
            }
            return NoContent();
        }


        // POST: [Controller]/Edit/{id}
        /// <summary>
        /// Sends an item to the backend to be updated in the database
        /// </summary>
        /// <param name="id">The id of the item to be updated</param>
        /// <param name="objectEdited">The item updated</param>
        /// <param name="postActionName">The name of the action to be redirected afterwards</param>
        /// <returns>Redirects to an action given on the postActionName paramater, or error description</returns>
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BOEdit(string id, UserQueryDTO userEdited, string postActionName = nameof(BOIndex))
        {
            try
            {
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/{id}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(usedUrl, userEdited);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction(postActionName, new { id = id });
                }
                return Content(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Content($"Falha a Editar {controllerName}, ver Consola");
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
        public virtual async Task<IActionResult> BODelete(string id, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{id}", UriKind.Relative);
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
