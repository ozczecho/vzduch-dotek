using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace VzduchDotek.Net.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem(detail: "Unhandled Error. Check Logs", title: "Global Error Handler");
    }
}
