namespace CookingBookWPF.Models
{
    internal class Subscription
    {
        public int SubscriptionId { get; set; }
        public User User { get; set; } = null!;
        public CookerProfile CookerProfile { get; set; } = null!;
        private Subscription() { }
        public Subscription(User user, CookerProfile cooker)
        {
            User = user;
            CookerProfile = cooker;
        }
    }
}
