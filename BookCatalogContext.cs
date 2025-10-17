using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using AdminWPF.Models;
using System.Configuration;

namespace AdminWPF
{
    internal class BookCatalogContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookGenre> GenreAuthors { get; set; }
        public BookCatalogContext() : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConfigurationManager.ConnectionStrings["BookCatalog"].ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // BookAuthor (issues with property to column mapping)
            modelBuilder.Entity<BookAuthor>()
                .Property(ba => ba.BookId)
                .HasColumnName("book_id");

            modelBuilder.Entity<BookAuthor>()
                .Property(ba => ba.AuthorId)
                .HasColumnName("author_id");

            // GenreAuthor (issues with property to column mapping)
            modelBuilder.Entity<BookGenre>()
                .Property(ba => ba.BookId)
                .HasColumnName("book_id");

            modelBuilder.Entity<BookGenre>()
                .Property(ba => ba.GenreId)
                .HasColumnName("genre_id");

            // Composite keys
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookGenre>()
                .HasKey(ba => new { ba.BookId, ba.GenreId });

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);

            //genre 
            modelBuilder.Entity<BookGenre>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<BookGenre>()
                .HasOne(ba => ba.Genre)
                .WithMany(a => a.BookGenres)
                .HasForeignKey(ba => ba.GenreId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
