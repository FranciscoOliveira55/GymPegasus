using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using FrontEndWeb.Models.DTOs;
using FrontEndWeb.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class ScheduleController : GenericCrudController<Schedule>
    {
        private readonly IQRCodeService _qRCodeService;
        //private readonly IWebHostEnvironment _env;

        public ScheduleController(
            IQRCodeService qRCodeService,
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions//,
            //IWebHostEnvironment env
            ) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {
            _qRCodeService = qRCodeService;
            //_env = env;
        }


        /// <summary>
        /// Lists Schedules in a pretty way
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListWeeklySchedules()
        {
            try
            {
                var clienteId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value;
                TempData["ClienteId"] = clienteId ?? "0";

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}?includeEntities=Event&includeEntities=Tickets", UriKind.Relative);
                HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

                if (response.IsSuccessStatusCode)
                {
                    var scheduleListStringReceived = await response.Content.ReadAsStringAsync();
                    var scheduleListReceived = JsonSerializer.Deserialize<IEnumerable<Schedule>>(scheduleListStringReceived, _jsonOptions);

                    return View(scheduleListReceived);
                }

                //Saves Current ClienteId in temp data to create Tickets
                //If there aren't any schedules, return View with empty list
                return View(new List<Schedule>() { });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }



        // POST: [Controller]/Create
        /// <summary>
        /// Creates a Ticket for the Logged Client for the Given Schedule
        /// </summary>
        /// <param name="objectCreated">The item to be created in the database</param>
        /// <returns>Redirects to the Index view (list all items) or shows error description</returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> CreateTicketForClienteWithScheduleId(int id)
        {
            try
            {
                var clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

                Ticket ticketToCreate = new()
                {
                    ClienteId = clienteId,
                    ScheduleId = id
                };

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"Ticket", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(usedUrl, ticketToCreate);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(ListWeeklySchedules));
                }

                return Problem(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Problem($"Falha a Criar {controllerName}, ver Consola");
            }
        }


        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public IActionResult GetInScheduleWithQRCode(int scheduleId)
        {
            //Urls to call the api
            var baseUrl = _apiSettings.BaseUrl;
            var clienteId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value;

            var getInApiUrl = $"{baseUrl}Ticket/GetInScheduleWithQRCode/{clienteId}/{scheduleId}";

            //Generates GetIn QRCode
            Bitmap getInQRCodeImage = _qRCodeService.QRCodeGenerateImage(getInApiUrl);
            byte[] getInQRCodeByteArray = _qRCodeService.QRCodeImageToByteArray(getInQRCodeImage);

            //Saves the Urls in tempData to test with buttons (after release, remove this)
            if (/*_env.IsDevelopment()*/ false)
            {
                TempData["IsDevelopment"] = true;
                TempData["GetInApiUrl"] = getInApiUrl;
            }

            return View(getInQRCodeByteArray);
        }

    }
}
