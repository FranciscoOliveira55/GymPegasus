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
    public class ExerciseTemplateController : GenericCrudController<ExerciseTemplate>
    {
        public ExerciseTemplateController(
            GymContext context,
            ILogger<GenericCrudController<ExerciseTemplate>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }
    }
}
