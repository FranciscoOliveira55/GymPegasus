using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using FrontEndWeb.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class WorkoutPlanController : GenericCrudController<WorkoutPlan>
    {
        public WorkoutPlanController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {}


        /// <summary>
        /// List The WorkoutPlans with ClienteId == to the Logged Cliente
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListClienteWorkoutPlans()
        {
            try
            {
                var clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/ClienteId/{clienteId}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

                if (response.IsSuccessStatusCode)
                {
                    var workoutPlanListStringReceived = await response.Content.ReadAsStringAsync();
                    var workoutPlanListReceived = JsonSerializer.Deserialize<IEnumerable<WorkoutPlan>>(workoutPlanListStringReceived, _jsonOptions);

                    return View(workoutPlanListReceived);
                }
                //If the Cliente doesn't have workoutPlans, return View with empty list
                return View(new List<WorkoutPlan>() { });
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }


        /// <summary>
        /// Shows the particular WorkoutPlan with the Id (similar to Details)
        /// Includes WorkoutPlanExercises and ExerciseTemplate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListWorkoutPlanExercises(int id)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}/WorkoutPlanWithExercises/{id}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var workoutPlanStringReceived = await response.Content.ReadAsStringAsync();
                WorkoutPlan workoutPlanReceived = JsonSerializer.Deserialize<WorkoutPlan>(workoutPlanStringReceived, _jsonOptions);
                
                TempData["WorkoutPlan"] = JsonSerializer.Serialize(workoutPlanReceived, _jsonOptions);

                return View(workoutPlanReceived);
            }
            return NoContent();
        }

    }
}
