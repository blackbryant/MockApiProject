using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mockAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace mockAPI.DataContext
{
    public class AppDbContext : IdentityDbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<EventRegistration> EventRegistrations { get; set; } = null!;

        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.PublicationDate).IsRequired();
                entity.Property(b => b.Genre).HasMaxLength(50);
                entity.Property(b => b.ISBN).IsRequired().HasMaxLength(13);
                entity.Property(b => b.Summary).HasMaxLength(1000);
                
            });

        }


        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     modelBuilder.Entity<Product>(entity =>
        //     {
        //         entity.HasKey(p => p.Id);
        //         entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
        //         entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        //         entity.Property(p => p.CategoryId).IsRequired();

        //     });


        //     modelBuilder.Entity<EventRegistration>(entity =>
        //     {
        //         entity.HasKey(e => e.Id);
        //         entity.Property(e => e.GUID).IsRequired();
        //         entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
        //         entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        //         entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
        //         entity.Property(e => e.EventDate).IsRequired();
        //         entity.Property(e => e.DaysAttending).IsRequired();
        //         entity.Property(e => e.Notes).HasMaxLength(500);
        //     });


        // }


    }
}