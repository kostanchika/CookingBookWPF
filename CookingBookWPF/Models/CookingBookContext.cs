using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.IO;

namespace CookingBookWPF.Models
{
    internal class CookingBookContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CookerProfile> CookerProfiles { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public CookingBookContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string cookingBookPath = Path.Combine(documentsPath, "CookingBook");
            Directory.CreateDirectory(cookingBookPath);
            optionsBuilder.UseSqlite("Data Source=" + cookingBookPath + "\\Database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(u => u.CookerProfile).WithOne().HasForeignKey<CookerProfile>("UserId").IsRequired(false);
            modelBuilder.Entity<CookerProfile>().HasKey(c => c.CookerProfileId);
            modelBuilder.Entity<Recipe>().HasKey(r => r.RecipeId);
            modelBuilder.Entity<Favorite>().HasKey(f => f.FavoriteId);
            modelBuilder.Entity<Subscription>().HasKey(s => s.SubscriptionId);
            modelBuilder.Entity<Ingredient>().HasKey(i => i.IngredientId);
            modelBuilder.Entity<Step>().HasKey(s => s.StepId);
        }
    }
}
