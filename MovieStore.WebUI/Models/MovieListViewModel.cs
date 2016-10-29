using MovieStore.Domain.Entities;
using System.Collections.Generic;

namespace MovieStore.WebUI.Models {
    public class MovieListViewModel {
        public IEnumerable<Movie> Movies { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}