using MovieStore.Domain.Abstract;
using MovieStore.Domain.Entities;
using System.Collections.Generic;

namespace MovieStore.Domain.Concrete {
    public class EFMovieRepository : IMovieRepository {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Movie> Movies {
            get { return context.Movies; }
        }

        public void SaveMovie(Movie movie) {
            if (movie.MovieID == 0) {
                context.Movies.Add(movie);
            } else {
                Movie dbEntry =
                context.Movies.Find(movie.MovieID);
                if (dbEntry != null) {
                    dbEntry.Name = movie.Name;
                    dbEntry.Description = movie.Description;
                    dbEntry.Price = movie.Price;
                    dbEntry.Category = movie.Category;
                    //dbEntry.ImageData = movie.ImageData;
                    //dbEntry.ImageMimeType = movie.ImageMimeType;
                }
            }
            context.SaveChanges();
        }

        public Movie DeleteMovie(int movieID) {
            Movie dbEntry = context.Movies.Find(movieID);
            if (dbEntry != null) {
                context.Movies.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}