using PropertyChanged;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CookingBookWPF.Models
{
    [AddINotifyPropertyChangedInterface]
    internal class Ingredient : INotifyPropertyChanged
    {
        public int IngredientId { get; set; }
        private string _name = string.Empty;
        [MaxLength(20)]
        public string Name
        {
            get => _name;
            set
            {
                if (!Regex.IsMatch(value, @"[\dA-Za-z$./;,+=@!#%^&*()_№:?]"))
                    _name = value.TrimStart().Replace("  ", " ");
            }
        }

        public int Quantity { get; set; }

        public string QuantityType { get; set; } = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}
