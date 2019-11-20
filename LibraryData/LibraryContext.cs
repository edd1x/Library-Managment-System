using System;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryData
{
    public class LibraryContext: DbContext  
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Book> Book { get; set; }
        public DbSet<BranchHours> BranchHours { get; set; }
        public DbSet<CheckoutHistory> CheckoutHistory { get; set; }
        public DbSet<Checkouts> Checkouts{ get; set; }
        public DbSet<Holds> Holds { get; set; }
        public DbSet<LibraryAsset> LibraryAsset { get; set; }
        public DbSet<LibraryBranch> LibraryBranch{ get; set; }
        public DbSet<LibraryCard> LibraryCards{ get; set; }
        public DbSet<Status> Status{ get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<Patron> Patrons { get; set; }
        
    }
}
