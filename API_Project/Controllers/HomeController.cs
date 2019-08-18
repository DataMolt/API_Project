using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_Project.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using API_Project.Data;
using Microsoft.EntityFrameworkCore;
namespace API_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _movieContext;

        public HomeController(IHttpClientFactory httpClientFactory, ApplicationDbContext movieContext)
        {
            _client = httpClientFactory.CreateClient();
            _movieContext = movieContext;
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

        public IActionResult FavoriteMovies()
        {
            return View(_movieContext.FavoriteMovies.ToList());
        }

        public async Task<IActionResult> AddFavorite(string id)
        {
            _client.BaseAddress = new Uri("http://www.omdbapi.com/");
            var response = await _client.GetAsync($"?apikey=4ce0252c&i={id}");
            var content = await response.Content.ReadAsAsync<Movie>();
            _movieContext.FavoriteMovies.Add(content);
            _movieContext.SaveChanges();
            return RedirectToAction("SearchResults");
        }

        public async Task<IActionResult> RemoveFavorite(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var favoriteMovie = await _movieContext.FavoriteMovies.SingleOrDefaultAsync(m => m.MovieId == id);
            if (favoriteMovie == null)
            {
                return NotFound();
            }
            return View(favoriteMovie);
        }

        [HttpPost, ActionName("RemoveFavorite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var favoriteMovie = await _movieContext.FavoriteMovies.SingleOrDefaultAsync(m => m.MovieId == id);
            _movieContext.FavoriteMovies.Remove(favoriteMovie);
            await _movieContext.SaveChangesAsync();
            return RedirectToAction(nameof(favoriteMovie));
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