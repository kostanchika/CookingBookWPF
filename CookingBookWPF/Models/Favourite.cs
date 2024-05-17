namespace CookingBookWPF.Models
{
    internal class Favorite
    {
        public int FavoriteId { get; set; }
        public User User { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
        private Favorite() { }
        public Favorite(User user, Recipe recipe)
        {
            User = user;
            Recipe = recipe;
        }
    }
}
