using AutoMapper;
using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : GenericCrudController<Schedule>
    {
        public ScheduleController(
            GymContext context,
            ILogger<GenericCrudController<Schedule>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }
    }
}
