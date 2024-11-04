using Stripe;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FrontEndWeb.Configurations;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using FrontEndWeb.Models;
using Microsoft.EntityFrameworkCore;
using Stripe.Issuing;
var builder = WebApplication.CreateBuilder(args);


//// Add session services
//builder.Services.AddSession(options =>
//{
//    // Set session timeout (optional)
//    options.IdleTimeout = TimeSpan.FromMinutes(30); // Example: 30 minutes session timeout
//    options.Cookie.HttpOnly = true; // Ensure the session cookie is accessible only through HTTP
//});


//Adds automapper
builder.Services.AddAutoMapper(typeof(MapperInitilizer));

//configure ApiSettingsService ## Change here if it to connect to a remote API
//var apiSettings = (builder.Configuration.GetSection("Api")).GetSection("LocalApi");
var apiSettings = (builder.Configuration.GetSection("Api")).GetSection("RemoteApi");
builder.Services.Configure<ApiSettings>(apiSettings);

//Adds cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SuperAuthCookie"; // Change this to your desired cookie name
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Set your desired expiration time
        //options.LoginPath = "/Auth/Login"; // Set the login page route
        //options.LogoutPath = "/Auth/Logout"; // Set the logout page route
    });

//Adds authorization
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

//Registers HttpClientFactoryWithJwtService as a service
builder.Services.AddScoped<IHttpClientFactoryWithJwtService, HttpClientFactoryWithJwtService>();

//Registers QRCodeService as a service
builder.Services.AddScoped<IQRCodeService, QRCodeService>();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();


//Builds App
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(o => o
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    //.AllowCredentials()
    );

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//Middleware to Intercept Actions
app.Use(async (context, next) =>
{

    //Check If Should Redirect
    var user = context.User;
    bool isAuthenticated = (user.Identity is not null) && (user.Identity.IsAuthenticated is true);
    bool getLoginOrRegister = ((context.Request.Path.StartsWithSegments("/Auth/Login") is true) || (context.Request.Path.StartsWithSegments("/Auth/Register") is true));
    bool getLogout = (context.Request.Path.StartsWithSegments("/Auth/Logout") is true);
    bool getHomeIndexVisitor = (context.Request.Path.StartsWithSegments("/Home/IndexVisitor") is true);
    bool getHomeIndex = (context.Request.Path.StartsWithSegments("/Home/Index") is true) && getHomeIndexVisitor is false;
    bool getHomeBOIndex = (context.Request.Path.StartsWithSegments("/Home/BOIndex") is true);
    bool getHomePrivacy = (context.Request.Path.StartsWithSegments("/Home/Privacy") is true);
    bool isCliente = (user.IsInRole("Cliente") is true);
    bool isEmployee = (user.IsInRole("Employee") is true);
    bool getSubscriptionProductOrderConfirmation = (context.Request.Path.StartsWithSegments("/SubscriptionProduct/OrderConfirmation") is true);
    bool getSubscriptionProductFailed = (context.Request.Path.StartsWithSegments("/SubscriptionProduct/Failed") is true);
    bool getSubscriptionProductSuccess = (context.Request.Path.StartsWithSegments("/SubscriptionProduct/Success") is true);
    bool accountAccessDenied = (context.Request.Path.StartsWithSegments("/Account/AccessDenied") is true);

    Console.WriteLine($"==========================================");
    Console.WriteLine($"Path: {context.Request.Path}");
    Console.WriteLine($"isAuthenticated: {isAuthenticated}");
    Console.WriteLine($"getLoginOrRegister: {getLoginOrRegister}");
    Console.WriteLine($"getLogout: {getLogout}");
    Console.WriteLine($"getHomeIndexVisitor: {getHomeIndexVisitor}");
    Console.WriteLine($"getHomeIndex: {getHomeIndex}");
    Console.WriteLine($"getHomeBOIndex: {getHomeBOIndex}");
    Console.WriteLine($"getHomePrivacy: {getHomePrivacy}");
    Console.WriteLine($"isCliente: {isCliente}");
    Console.WriteLine($"isEmployee: {isEmployee}");
    Console.WriteLine($"getSubscriptionProductOrderConfirmation: {getSubscriptionProductOrderConfirmation}");
    Console.WriteLine($"getSubscriptionProductSuccess: {getSubscriptionProductSuccess}");
    Console.WriteLine($"getSubscriptionProductFailed: {getSubscriptionProductFailed}");
    Console.WriteLine($"accountAccessDenied: {accountAccessDenied}");
    Console.WriteLine($"==========================================");

    //I should be redirected to Index or BOIndex IF
    if (isAuthenticated //i'm authenticated     And     Either
    && (getLoginOrRegister //i'm going to register or login     OR
    || (getHomeIndexVisitor //i'm going to IndexVisitor     OR
    || (getHomeIndex && (isCliente is false)) //i'm going to Index not as a Cliente     OR
    || (getHomeBOIndex && (isEmployee is false)) //i'm going to BOIndex not as a Employee
    || (accountAccessDenied)))//i'm going to account access denied
    )
    {
        if (isCliente)
        {
            context.Response.Redirect("/Home/Index");
            return;
        }
        else if (isEmployee)
        {
            context.Response.Redirect("/Home/BOIndex");
            return;
        }
    }
    //I should be redirected to IndexVisitor IF
    else if ((!isAuthenticated) //i'm not authenticated     And
    && (!getLoginOrRegister)  //i'm not going to Login or Register      And
    && (!getHomeIndexVisitor) //i'm not going to IndexVisitor       And
    && (!getSubscriptionProductOrderConfirmation) //i'm not going to OrderConfirmation      And
    && (!getSubscriptionProductFailed) //i'm not going to CheckoutFailed        And
    && (!getSubscriptionProductSuccess) //i'm not going to CheckoutSuccess
    && (!getHomePrivacy)) //i'm not going to Privacy
    {
        // Redirect to the login endpoint
        context.Response.Redirect("/Home/IndexVisitor");
        return;
    }
    else
        await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=BOIndex}/{id?}");

app.Run();
