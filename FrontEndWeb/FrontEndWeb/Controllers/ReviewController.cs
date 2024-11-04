using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class ReviewController : GenericCrudController<Review>
    {
        public ReviewController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {
        }


        // GET: [Controller]/Create
        /// <summary>
        /// Returns the view named Create
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);
                var employeeId = (int)TempData.Peek("employeeId");

                var review = new Review
                {
                    ClienteId = clienteId,
                    EmployeeId = employeeId,
                    Classification = 0,
                    DateTime = DateTime.Now,
                };

                return View(review);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        // POST: [Controller]/Create
        /// <summary>
        /// Sends an item to the backend to be created in the database
        /// </summary>
        /// <param name="objectCreated">The item to be created in the database</param>
        /// <returns>Redirects to the Index view (list all items) or shows error description</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(Review objectCreated, string? controllerRedirect, int? idRedirect, string? actionRedirect = "Index")
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



    }
}
