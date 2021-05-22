using Lab4.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favourites> Favourites { get; set; }

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>().Property(m => m.Title).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.Description).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.Genre).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.DurationMinutes).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.ReleaseYear).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.Director).IsRequired();
            modelBuilder.Entity<Movie>().Property(m => m.AddedAt).IsRequired().HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Movie>().Property(m => m.Watched).IsRequired();

            modelBuilder.Entity<Comment>().Property(c => c.Text).IsRequired();
            modelBuilder.Entity<Comment>().Property(c => c.Important).IsRequired().HasDefaultValue(false);
            modelBuilder.Entity<Comment>().Property(c => c.MovieId).IsRequired();
        }
    }
}
