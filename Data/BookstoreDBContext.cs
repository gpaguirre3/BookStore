using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;


namespace Data
{
    public class BookstoreDBContext(DbContextOptions<BookstoreDBContext> options) : DbContext(options)
    {
        public DbSet<Models.Book> Books { get; set; }
        public DbSet<Models.Author> Authors { get; set; }
        public DbSet<Models.Genre> Genres { get; set; }
        public DbSet<Models.BookGenre> BookGenres { get; set; }
        public DbSet<Models.Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
 
            modelBuilder.Entity<Models.Book>().Navigation(e => e.Author).AutoInclude();
            modelBuilder.Entity<Models.Book>().Navigation(e => e.Genres).AutoInclude();
            modelBuilder.Entity<Models.Loan>().Navigation(l => l.Book).AutoInclude();

            modelBuilder.Entity<Models.Loan>().HasOne(l => l.Book);
            modelBuilder.Entity<Models.Book>().HasOne(b => b.Author);
            modelBuilder.Entity<Models.Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books);

            modelBuilder.Entity<Models.BookGenre>().HasNoKey();
        }
    }
}
