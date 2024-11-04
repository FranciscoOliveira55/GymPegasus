using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace FrontEndWeb.Controllers
{
    public class SubscriptionProductController : GenericCrudController<SubscriptionProduct>
    {
        public SubscriptionProductController(
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        { }



        /// <summary>
        /// List SubscriptionProducts in the DataBase (same as Index, but with a presentable view)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> ListSubscriptionProducts()
        {
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"{controllerName}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);

            if (response.IsSuccessStatusCode)
            {
                var objectListStringReceived = await response.Content.ReadAsStringAsync();
                var objectListReceived = JsonSerializer.Deserialize<IEnumerable<SubscriptionProduct>>(objectListStringReceived, _jsonOptions);

                return View(objectListReceived);
            }
            return NoContent();
        }




        // GET: [Controller]/OrderConfirmation
        /// <summary>
        /// Evaluates the payment and makes the "postPayment actions" (adds time to the cliente subscription)
        /// </summary>
        /// <returns>Redirects to Success or Failed actions</returns>
        [AllowAnonymous] //It comes to this point reddirected from stripe
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            var subscriptionPurchaseTimeTicksString = ((string)TempData.Peek("SubscriptionPurchaseTimeTicks") ?? "0");

            if (session.PaymentStatus.Equals("paid") && !subscriptionPurchaseTimeTicksString.Equals("0"))
            {
                //Add Time To Cliente Subscription
                long subscriptionPurchaseTimeTicks = long.Parse(subscriptionPurchaseTimeTicksString);
                long subscriptionPurchasePrice = long.Parse(((string)TempData.Peek("SubscriptionPurchasePrice") ?? "0"));
                int subscriptionPurchaseProductId = int.Parse(((string)TempData.Peek("SubscriptionPurchaseProductId") ?? "-1"));
                int subscriptionPurchaseClienteId = int.Parse((string)TempData.Peek("SubscriptionPurchaseClienteId") ?? "-1");

                Console.WriteLine($"========# Order Confirmation Data #========");
                Console.WriteLine($"SubscriptionPurchaseTimeTicks: {subscriptionPurchaseTimeTicks}");
                Console.WriteLine($"SubscriptionPurchasePrice: {subscriptionPurchasePrice}");
                Console.WriteLine($"SubscriptionPurchaseProductId: {subscriptionPurchaseProductId}");
                Console.WriteLine($"SubscriptionPurchaseClienteId: {subscriptionPurchaseClienteId}");
                Console.WriteLine($"===========================================");

                try
                {
                    //Get the ClienteId from the authCookie
                    //Updates Cliente Subscription Expiration
                    HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                    Uri usedUrl = new($"Cliente/UpdateSubscription/{subscriptionPurchaseClienteId}", UriKind.Relative);
                    HttpResponseMessage responseUpdateClienteSubscription = await httpClient.PutAsJsonAsync(usedUrl, subscriptionPurchaseTimeTicks);

                    //If ClienteSubscriptionUpdate is Successful, creates PurchaseRecord
                    if (responseUpdateClienteSubscription.IsSuccessStatusCode)
                    {
                        //CreatesRecord
                        var purchaseRecord = new SubscriptionProductPurchaseRecord()
                        {
                            PurchaseDate = DateTime.Now,
                            PurchaseTimeTicks = subscriptionPurchaseTimeTicks,
                            PurchasePrice = subscriptionPurchasePrice,
                            ClienteId = subscriptionPurchaseClienteId,
                            SubscriptionProductId = subscriptionPurchaseProductId,
                        };

                        //SendsRecord to BackEnd
                        HttpClient httpClientR = _httpClientFactoryWithJwtService.CreateClientWithJwt();
                        Uri usedUrlR = new($"SubscriptionProductPurchaseRecord/PublicCreate", UriKind.Relative);
                        HttpResponseMessage responseCreatePurchaseRecord = await httpClientR.PostAsJsonAsync(usedUrlR, purchaseRecord);
                        //If it fails
                        if (!responseCreatePurchaseRecord.IsSuccessStatusCode)
                            return Problem($"Failed to Create Purchase Record.");
                    }
                    else
                        return Problem($"Failed to Update Cliente Subscription.");

                }
                catch (Exception e)
                {
                    return Problem($"Failed to Update Cliente Subscription or PurchaseRecord \n {e.Message}");
                }

                var transaction = session.PaymentIntentId.ToString();

                //Remove TempData Variables
                TempData.Remove("SubscriptionPurchaseTimeTicks");
                TempData.Remove("SubscriptionPurchasePrice");

                return Redirect(nameof(Success));
            }

            //Remove TempData Variables
            TempData.Remove("SubscriptionPurchaseTimeTicks");
            TempData.Remove("SubscriptionPurchasePrice");

            return Redirect(nameof(Failed));
        }


        // GET: [Controller]/Success
        /// <summary>
        /// Gives the Success view
        /// </summary>
        /// <returns>The Success view</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }


        // GET: [Controller]/Failed
        /// <summary>
        /// Gives the Failed view
        /// </summary>
        /// <returns>The Failed view</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Failed()
        {
            return View();
        }


        // GET: [Controller]/Checkout
        /// <summary>
        /// Redirects to the Stripe page and passes the data of the products to be bought
        /// </summary>
        /// <returns>Redirects to the OrderConfirmation action, (or Failed action in case of cancelled purchase)</returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            //Checks if the Product is in the BD
            HttpClient httpClient = _httpClientFactoryWithJwtService.CreateClientWithJwt();
            Uri usedUrl = new($"SubscriptionProduct/Id/{id}", UriKind.Relative);
            HttpResponseMessage response = await httpClient.GetAsync(usedUrl);


            //If it's in BD
            if (response.IsSuccessStatusCode)
            {
                var subscriptionProductListStringReceived = await response.Content.ReadAsStringAsync();
                var subscriptionProductListReceived = JsonSerializer.Deserialize<IEnumerable<SubscriptionProduct>>(subscriptionProductListStringReceived, _jsonOptions);
                long subscriptionPurchaseTimeTicks = 0;
                long subscriptionPurchasePrice = 0;
                int subscriptionPurchaseProductId = 0;

                //Gets the Cliente email from the Auth Cookie
                string customerEmail = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                if (customerEmail == null)
                    Unauthorized();

                //Get Domain
                var url = HttpContext.Request.GetDisplayUrl();
                var path = HttpContext.Request.Path;
                var domain = url.Split(path)[0] + "/";

                //Creates the Purchase Session
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"{controllerName}/{nameof(OrderConfirmation)}",
                    CancelUrl = domain + $"{controllerName}/{nameof(Failed)}",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    CustomerEmail = customerEmail
                };

                //Adds the Products to the Purchase Session
                foreach (var item in subscriptionProductListReceived)
                {
                    var sessionListItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Price, //Price is in cents
                            Currency = "EUR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Title.ToString(),
                                Description = item.Description,
                            }
                        },
                        Quantity = 1
                    };
                    options.LineItems.Add(sessionListItem);
                    subscriptionPurchaseTimeTicks += item.SubscriptionTimeTicks;
                    subscriptionPurchasePrice += item.Price;
                    subscriptionPurchaseProductId = item.Id;
                }

                //Sends the data to stripe and goes to the stripe page
                var service = new SessionService();
                Session session = service.Create(options);


                //Saves the sessionId on tempData to be retrieved in the next request
                TempData["Session"] = session.Id;
                TempData["SubscriptionPurchaseTimeTicks"] = subscriptionPurchaseTimeTicks.ToString();
                TempData["SubscriptionPurchasePrice"] = subscriptionPurchasePrice.ToString();
                TempData["SubscriptionPurchaseProductId"] = subscriptionPurchaseProductId.ToString();
                TempData["SubscriptionPurchaseClienteId"] = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value;

                Response.Headers.Add("Location", session.Url);
                Console.WriteLine(session.Url);

                //Returns Redirected
                return new StatusCodeResult(303);
            }
            return Problem("Invalid SubscriptionProduct");
        }

    }
}
