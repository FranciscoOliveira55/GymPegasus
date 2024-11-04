using Microsoft.AspNetCore.Mvc;
using FrontEndWeb.Models;
using System.Text.Json;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using Microsoft.DotNet.MSIdentity.Shared;
using Azure;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using FrontEndWeb.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using FrontEndWeb.Configurations;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;

namespace FrontEndWeb.Controllers
{
    public abstract class GenericCrudController<T> : Controller where T : EntityBase
    {
        protected readonly IHttpClientFactoryWithJwtService _httpClientFactoryWithJwtService;
        protected readonly IMapper _mapper;
        protected readonly ApiSettings _apiSettings;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected string controllerName; //controllerName to be added to the baseUrl to connect to the backend endpoint

        public GenericCrudController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService, 
            IMapper mapper, 
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions)
        {
            controllerName = typeof(T).Name;//controllerName to be added to the baseUrl to connect to the backend endpoint
            _httpClientFactoryWithJwtService = httpClientFactoryWithJwtService; //start httpClientFactory so that it can be used in the methods
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        }


        // GET: [Controller]/Index
        /// <summary>
        /// Asks the backend for all the items in the database (in the table)
        /// </summary>
        /// <returns>A view with the list of the items in the database (in the table), or 404 if no items are found</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOIndex()
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectListStringReceived = await response.Content.ReadAsStringAsync();
                var objectListReceived = JsonSerializer.Deserialize<IEnumerable<T>>(objectListStringReceived, _jsonOptions);

                return View(objectListReceived);
            }
            return NoContent();
        }


        // GET: [Controller]/Details/{id}
        /// <summary>
        /// Asks the backend for an item with an id in the database (in the table)
        /// </summary>
        /// <param name="id">The id of the item to be readed</param>
        /// <returns>A view with the item readed (or 404 if it is not found)</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BODetails(int id, int? secondId)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            string suffix = (secondId != null) ? $"?secondId={secondId}" : "";
            Uri usedUrl = new($"{controllerName}/{id}{suffix}", UriKind.Relative);

            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived = await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<T>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
            }
            return NoContent();
        }


        // GET: [Controller]/Create
        /// <summary>
        /// Returns the view named Create
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOCreate()
        {
            return View();
        }


        // POST: [Controller]/Create
        /// <summary>
        /// Sends an item to the backend to be created in the database
        /// </summary>
        /// <param name="objectCreated">The item to be created in the database</param>
        /// <returns>Redirects to the Index view (list all items) or shows error description</returns>
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BOCreate(T objectCreated, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            try
            {
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(usedUrl, objectCreated);

                if (response.IsSuccessStatusCode)
                {
                    if(!string.IsNullOrEmpty(controllerRedirect))
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


        // GET: [Controller]/Edit/{id}
        /// <summary>
        /// Asks the backend if an item with an id exists (so i can update it)
        /// </summary>
        /// <param name="id">The id of the item to be updated</param>
        /// <returns>The view to update the item (if it exists in the database), 404 otherwise</returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BOEdit(int id, int? secondId)
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            string suffix = (secondId != null) ? $"?secondId={secondId}" : "";
            Uri usedUrl = new($"{controllerName}/{id}{suffix}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectStringReceived =
                    await response.Content.ReadAsStringAsync();

                var objectReceived = JsonSerializer.Deserialize<T>(objectStringReceived, _jsonOptions);

                return View(objectReceived);
            }
            return NoContent();
        }


        // POST: [Controller]/Edit/{id}
        /// <summary>
        /// Sends an item to the backend to be updated in the database
        /// </summary>
        /// <param name="id">The id of the item to be updated</param>
        /// <param name="objectEdited">The item updated</param>
        /// <param name="postActionName">The name of the action to be redirected afterwards</param>
        /// <returns>Redirects to an action given on the postActionName paramater, or error description</returns>
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> BOEdit(int id, T objectEdited, string postActionName = nameof(BOIndex))
        {
            try
            {
                HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                Uri usedUrl = new($"{controllerName}/{id}", UriKind.Relative);
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(usedUrl, objectEdited);

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction(postActionName, new { id = id });
                }
                return Content(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                return Content($"Falha a Editar {controllerName}, ver Consola");
            }
        }


        // GET: [Controller]/Delete/{id}
        /// <summary>
        /// Sends an item to the backend to be deleted in the database
        /// </summary>
        /// <param name="id">The id of the item to be deleted</param>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public virtual async Task<IActionResult> BODelete(int id,int? secondId, string? controllerRedirect, int? idRedirect, string? actionRedirect = nameof(BOIndex))
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            string suffix = (secondId != null) ? $"?secondId={secondId}" : "";
            Uri usedUrl = new($"{controllerName}/{id}{suffix}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.DeleteAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(controllerRedirect))
                    return RedirectToAction(actionRedirect, controllerRedirect, new { id = idRedirect ?? 0 });
                else
                    return RedirectToAction(actionRedirect, new { id = idRedirect ?? 0 });
            }
            return Content(await response.Content.ReadAsStringAsync());
        }




        /// <summary>
        /// Internal Method to help debug http responses
        /// </summary>
        /// <param name="response">The received http response</param>
        protected async void WriteRequestToConsole(HttpResponseMessage response)
        {
            if (response is null)
            {
                return;
            }
            var request = response.RequestMessage;
            System.Diagnostics.Debug.Write($"{request?.Method} ");
            System.Diagnostics.Debug.Write($"{request?.RequestUri} ");
            System.Diagnostics.Debug.WriteLine($"HTTP/{request?.Version}");
            if (request?.Content != null)
                System.Diagnostics.Debug.WriteLine($"Request: {await request.Content.ReadAsStringAsync()}\n");
            System.Diagnostics.Debug.WriteLine($"Response: {await response.Content.ReadAsStringAsync()}\n");
        }

    }
}
