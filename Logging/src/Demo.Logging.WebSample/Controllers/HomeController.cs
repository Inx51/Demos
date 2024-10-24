using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Demo.Logging.WebSample.Models;

namespace Demo.Logging.WebSample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Hello user {@user}", User);
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Now this is the {page} page", nameof(Privacy));
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}