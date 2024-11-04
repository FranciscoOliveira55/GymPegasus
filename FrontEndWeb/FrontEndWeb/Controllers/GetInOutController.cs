using AutoMapper;
using FrontEndWeb.Configurations;
using FrontEndWeb.Models;
using FrontEndWeb.Models.DTOs;
using FrontEndWeb.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
namespace FrontEndWeb.Controllers
{
    public class GetInOutController : GenericCrudController<GetInOut>
    {
        private readonly IQRCodeService _qRCodeService;
        //private readonly IWebHostEnvironment _env;

        public GetInOutController(
            IQRCodeService qRCodeService,
            IHttpClientFactoryWithJwtService httpClientFactoryWithJwtService,
            IMapper mapper,
            IOptions<ApiSettings> apiSettings,
            IOptions<JsonOptions> jsonOptions//,
            //IWebHostEnvironment env
            ) :
            base(httpClientFactoryWithJwtService, mapper, apiSettings, jsonOptions)
        {
            _qRCodeService = qRCodeService;
            //_env = env;
        }


        /// <summary>
        /// Creates 2 QRCodes to getIn and getOut and sends them to the view
        /// The User then Shows the QRCode to the CheckPoint in the Gym to GetIn or Out Of the Gym
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public IActionResult CheckPointGetInOut()
        {
            //Urls to call the api
            var baseUrl = _apiSettings.BaseUrl;
            var clienteId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClienteId").Value;

            var getInApiUrl = $"{baseUrl}{controllerName}/CreateWithQRCode/{clienteId}/{GetInOutType.In}";
            var getOutApiUrl = $"{baseUrl}{controllerName}/CreateWithQRCode/{clienteId}/{GetInOutType.Out}";

            //Generates GetIn QRCode
            Bitmap getInQRCodeImage = _qRCodeService.QRCodeGenerateImage(getInApiUrl);
            byte[] getInQRCodeByteArray = _qRCodeService.QRCodeImageToByteArray(getInQRCodeImage);

            //Generates GetOut QRCode
            Bitmap getOutQRCodeImage = _qRCodeService.QRCodeGenerateImage(getOutApiUrl);
            byte[] getOutQRCodeByteArray = _qRCodeService.QRCodeImageToByteArray(getOutQRCodeImage);

            //Saves the QRCodes in an Object to be passed to the View
            GetInOutQRCodeToViewDTO getInOutQRCodeToViewDTO = new()
            {
                GetInQRCode = getInQRCodeByteArray,
                GetOutQRCode = getOutQRCodeByteArray
            };

            //Saves the Urls in tempData to test with buttons (after release, remove this)
            if (/*_env.IsDevelopment()*/ false)
            {
                TempData["IsDevelopment"] = true;
                TempData["GetInApiUrl"] = getInApiUrl;
                TempData["GetOutApiUrl"] = getOutApiUrl;
            }

            return View(getInOutQRCodeToViewDTO);
        }
    }
}
