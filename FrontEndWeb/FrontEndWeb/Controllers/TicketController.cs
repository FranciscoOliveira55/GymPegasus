using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontEndWeb.Controllers
{
    public class TicketController : GenericCrudController<Ticket>
    {
        public TicketController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        { }



        // GET: [Controller]/Delete/{id}
        /// <summary>
        /// Sends an item to the backend to be deleted in the database
        /// </summary>
        /// <param name="id">The id of the item to be deleted</param>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public virtual async Task<IActionResult> Delete(int id, int? secondId, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            string suffix = (secondId != null) ? $"?secondId={secondId}" : "";
            Uri usedUrl = new($"{controllerName}/Public/{id}{suffix}", UriKind.Relative);
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
