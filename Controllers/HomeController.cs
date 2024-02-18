using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using testCSharp.Extenstions;
using testCSharp.Models;
using testCSharp.Services;
using static System.Net.WebRequestMethods;

namespace testCSharp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;
        public HomeController(ILogger<HomeController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _apiService.GetEmployees("/api/gettimeentries?code=", "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==");
            var employeesList = employees.ToList();

            Dictionary<string, long> newSortedEmployees = _apiService.SortWorkingHoursByEmployee(employeesList);

            newSortedEmployees = newSortedEmployees.OrderByDescending(e => e.Value).ToDictionary(e => e.Key, e => e.Value);

            return View(newSortedEmployees);
        }

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
