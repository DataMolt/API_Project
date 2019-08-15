using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Project.Models;
using System.Net.Http;

namespace API_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchResults(string searchQuery)
        {
            _client.BaseAddress = new Uri("http://www.omdbapi.com/");
            var response = await _client.GetAsync($"?apikey=4ce0252c&s={searchQuery}");
            var content = await response.Content.ReadAsAsync<FoundMovies>();
            return View(content);
        }

        public async Task<IActionResult> MovieDetails(string id)
        {
            _client.BaseAddress = new Uri("http://www.omdbapi.com/");

            var response = await _client.GetAsync($"?apikey=4ce0252c&i={id}");
            var content = await response.Content.ReadAsAsync<Movie>();
            return View(content);
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