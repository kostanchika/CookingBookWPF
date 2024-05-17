using System.ComponentModel.DataAnnotations;

namespace CookingBookWPF.Models
{
    internal class User
    {
        public int UserId { get; set; }
        [MaxLength(23)]
        public string Login { get; set; }
        [MaxLength(23)]
        public string Password { get; set; }
        public CookerProfile? CookerProfile { get; set; }
        public User()
        {
            Login = string.Empty;
            Password = string.Empty;
            CookerProfile = null;
        }
    }
}
