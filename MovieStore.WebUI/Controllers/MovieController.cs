using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using MovieStore.WebUI.Models;

namespace MovieStore.WebUI.Controllers
{
    public class MovieController : Controller
    {
        private IMovieRepository repository;
        public int PageSize = 4;
        
        public MovieController(IMovieRepository movieRepository) {
            this.repository = movieRepository;
        }

        public ViewResult List(string category, int page = 1) {
            MovieListViewModel model = new MovieListViewModel {
                Movies = repository.Movies
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.MovieID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Movies.Count() : repository.Movies.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            };
            return View(model);
        }
    }
}