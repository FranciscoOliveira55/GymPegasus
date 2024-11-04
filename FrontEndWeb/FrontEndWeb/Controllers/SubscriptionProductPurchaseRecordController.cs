using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class SubscriptionProductPurchaseRecordController : GenericCrudController<SubscriptionProductPurchaseRecord>
    {
        public SubscriptionProductPurchaseRecordController(
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
        public virtual async Task<IActionResult> ProfilePurchaseRecords()
        {
            int clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/ClienteId/{clienteId}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            //If Records with the ClienteId exists, send it in the View
            if (response.IsSuccessStatusCode)
            {
                var purchaseRecordListStringReceived = await response.Content.ReadAsStringAsync();

                var purchaseRecordList = JsonSerializer.Deserialize<IEnumerable<SubscriptionProductPurchaseRecord>>(purchaseRecordListStringReceived, _jsonOptions);

                return View(purchaseRecordList);
            }
            else
            {
                return View(new List<SubscriptionProductPurchaseRecord>() { });
            }
        }
    }
}
