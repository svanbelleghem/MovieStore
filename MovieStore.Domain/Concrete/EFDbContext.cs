using MovieStore.Domain.Entities;
using System.Data.Entity;

namespace MovieStore.Domain.Concrete {
    public class EFDbContext : DbContext {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
