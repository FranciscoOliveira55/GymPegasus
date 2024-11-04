using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontEndWeb.Controllers
{
    public class WorkoutExerciseController : GenericCrudController<WorkoutExercise>
    {
        public WorkoutExerciseController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        { }
    }
}
