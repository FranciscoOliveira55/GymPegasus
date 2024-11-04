using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using FrontEndWeb.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;


namespace FrontEndWeb.Controllers
{
    public class ClienteController : GenericCrudController<Cliente>
    {
        public ClienteController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        { }

        // GET: [Controller]/Details/{id}
        /// <summary>
        /// Asks the backend for an item with an id in the database (in the table)
        /// </summary>
        /// <param name="id">The id of the item to be readed</param>
        /// <returns>A view with the item readed (or 404 if it is not found)</returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public virtual async Task<IActionResult> Profile()
        {
            int clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{clienteId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            //If Cliente with the UserId exists, send it in the View
            if (response.IsSuccessStatusCode)
            {
                var clienteStringReceived = await response.Content.ReadAsStringAsync();

                var clienteReceived = JsonSerializer.Deserialize<Cliente>(clienteStringReceived, _jsonOptions);

                return View(clienteReceived);
            }
            //If Cliente with the UserId doesn't exist, return Unauthorized
            return Unauthorized();
        }


        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            int clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/{clienteId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived =
                    await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<Cliente>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
            }
            return NoContent();
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Cliente clienteEdited, string postActionName = nameof(Profile))
        {
            try
            {
                int clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/{clienteId}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(usedUrl, clienteEdited);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(postActionName, new { id = clienteId });
                }
                return Content(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Content($"Falha a Editar {controllerName}, ver Consola");
            }
        }
    }
}
