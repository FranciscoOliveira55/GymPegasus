using FrontEndWeb.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using FrontEndWeb.Models.Enums;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using FrontEndWeb.Configurations;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace FrontEndWeb.Controllers
{
    public class EmployeeController : GenericCrudController<Employee>
    {
        public EmployeeController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {
        }



        /// <summary>
        /// Asks the BackEnd for all the Employees with Postion = 1 (Instructor)
        /// Also Includes the Reviews List and User Data
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListInstructors()
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/position/2?includeEntities=User&includeEntities=Reviews", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectListStringReceived = await response.Content.ReadAsStringAsync();
                var objectListReceived = JsonSerializer.Deserialize<IEnumerable<Employee>>(objectListStringReceived, _jsonOptions);

                return View(objectListReceived);
            }
            return View(new List<Employee>() { });
        }


        /// <summary>
        /// Asks the BackEnd for the Employee with the Id
        /// Includes The Reviews List And The Cliente Of Each Review
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListInstructorReviews(int id)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/EmployeeWithReviews/{id}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived = await response.Content.ReadAsStringAsync();
                var objectReceived = JsonSerializer.Deserialize<Employee>(objectStringReceived, _jsonOptions);

                TempData["employeeId"] = id;
                return View(objectReceived);
            }
            return NoContent();
        }
    }
}
