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
    public class SubscriptionProductController : GenericCrudController<SubscriptionProduct>
    {
        public SubscriptionProductController(
            GymContext context,
            ILogger<GenericCrudController<SubscriptionProduct>> logger,
            IOptions<JsonOptions> jsonOptions,
            IMapper mapper
            ) : base(context, logger, jsonOptions, mapper)
        { }

    }
}
