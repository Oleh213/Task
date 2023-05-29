using System;
using Microsoft.EntityFrameworkCore;
using Task.Context;

namespace Task.DBContext
{
	public class DogsContext : DbContext
    {
		public DogsContext(DbContextOptions<DogsContext> options) : base(options)
        {
		}

        public DbSet<Dog> Dogs { get; set; }

        public DbSet<Logger> Loggers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>().HasKey(s => new { s.DogId });

            modelBuilder.Entity<Logger>().HasKey(s => new { s.LoggerId });
        }
    }
}

