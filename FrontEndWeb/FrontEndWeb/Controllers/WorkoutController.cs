using AutoMapper;
using Azure;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Composition;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class WorkoutController : GenericCrudController<Workout>
    {
        public WorkoutController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {
        }

        /// <summary>
        /// List The Workout with ClienteId == to the Logged Cliente
        /// Include WorkoutPlan And WorkoutExercises
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListClienteWorkouts()
        {
            try
            {
                var clienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value);

                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/ClienteId/{clienteId}?includeEntities=WorkoutPlan&includeEntities=WorkoutExercises", UriKind.Relative);
                HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

                if (response.IsSuccessStatusCode)
                {
                    var workoutListStringReceived = await response.Content.ReadAsStringAsync();
                    var workoutListReceived = JsonSerializer.Deserialize<IEnumerable<Workout>>(workoutListStringReceived, _jsonOptions);

                    return View(workoutListReceived);
                }
                //If the Cliente doesn't have workout, return View with empty list
                return View(new List<Workout>() { });
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }



        /// <summary>
        /// Page Similar to Create/Edit
        /// It has a form for Workout with Exercises
        /// Most properties are already sent to the view with a value
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public ActionResult StartWorkoutWithPlan()
        {
            //Gets the Json workoutPlan from the TempData
            string workoutPlanString = (string)TempData.Peek("WorkoutPlan");
            WorkoutPlan workoutPlan = JsonSerializer.Deserialize<WorkoutPlan>(workoutPlanString, _jsonOptions);

            //Creates Workout Object
            Workout workout = new()
            {
                ClienteId = int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value),
                WorkoutPlanId = workoutPlan.Id,
                WorkoutPlan = workoutPlan, //Adds workoutPlan to display info on view
                HourInit = DateTime.Now
            };

            //Creates WorkoutExercises With the WorkoutPlanExercises
            List<WorkoutExercise> workoutExercises = new();
            foreach(WorkoutPlanExercise workoutPlanExercise in workoutPlan.WorkoutPlanExercises)
            {
                workoutExercises.Add(new WorkoutExercise() 
                {
                ExerciseTemplateId = workoutPlanExercise.ExerciseTemplateId,
                ExerciseTemplate = workoutPlanExercise.ExerciseTemplate,
                WorkoutId = workout.Id,
                Effort = workoutPlanExercise.Effort,
                Repeat = workoutPlanExercise.Repeat,
                Done = false
                });
            }
            workout.WorkoutExercises = workoutExercises;

            //return Json(workout);

            return View(workout);
        }


        // POST: [Controller]/Create
        /// <summary>
        /// Sends an item to the backend to be created in the database
        /// </summary>
        /// <param name="objectCreated">The item to be created in the database</param>
        /// <returns>Redirects to the Index view (list all items) or shows error description</returns>
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(Workout objectCreated, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
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
