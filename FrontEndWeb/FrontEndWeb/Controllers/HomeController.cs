using FrontEndWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace FrontEndWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Shows Index View For Clientes
        /// </summary>
        /// <returns>Gives Index view (home page)</returns>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Shows Index View For Employees (BackOffice)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult BOIndex()
        {
            return View();
        }

        /// <summary>
        /// Shows Index View For Visitors
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult IndexVisitor()
        {
            return View();
        }

        /// <summary>
        /// Shows Privacy Policy View
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
