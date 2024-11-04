using BackEnd.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using BackEnd.Configurations;
using AutoMapper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Castle.Core.Configuration;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using BackEnd.Models;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

//Adds automapper
builder.Services.AddAutoMapper(typeof(MapperInitilizer));




//Adds sql server
/* ============================================= */
//var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
//var dbName = Environment.GetEnvironmentVariable("DB_NAME");
//var dbUser = Environment.GetEnvironmentVariable("DB_MSSQL_USER");
//var dbPassword = Environment.GetEnvironmentVariable("DB_MSSQL_SA_PASSWORD");
//var connectionString = $"Data Source={dbHost}; Initial Catalog={dbName};User ID = sa;Password={dbPassword};TrustServerCertificate=True";
//var connectionString = $"Server=tcp:{dbHost}.database.windows.net,1433;Initial Catalog={dbName};Persist Security Info=False;User ID={dbUser};Password={dbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=50;";
builder.Services.AddDbContext<GymContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalGymDB"))
        //options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteGymDB"))
    //options.UseSqlServer(connectionString)
    );
/* ============================================= */


//Adds identity framework
builder.Services
    .AddIdentityCore<User>(q =>
{
    q.Password.RequireDigit = false;
    q.Password.RequireLowercase = false;
    q.Password.RequireUppercase = false;
    q.Password.RequireNonAlphanumeric = false;
    q.Password.RequiredUniqueChars = 0;
    q.Password.RequiredLength = 6;
    q.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<GymContext>()
    .AddDefaultTokenProviders();


//configure jwt SettingsService
var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

//configure jwt authentication
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

//Adds jwt authentication
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        //ValidIssuer = jwtSettings.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Adds authorization
builder.Services.AddAuthorization();

//Adds endpoints of Controllers with json options
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//Adds the token to the swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Gym Pegasus API", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "0auth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


//builds app
//########################################################################################
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Gym Pegasus API V1");
        // Set the default Swagger UI page to the Swagger endpoint
        //o.RoutePrefix = string.Empty; // This makes Swagger UI the default route
    });
}

app.UseHttpsRedirection();

app.UseCors(o => o
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    //.AllowCredentials()
    );

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
