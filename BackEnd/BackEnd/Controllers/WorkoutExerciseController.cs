using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutExerciseController : GenericCrudController<WorkoutExercise>
    {
        public WorkoutExerciseController(
            GymContext context,
            ILogger<GenericCrudController<WorkoutExercise>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }
    }
}
