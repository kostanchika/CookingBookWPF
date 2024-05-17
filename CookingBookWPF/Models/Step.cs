using PropertyChanged;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CookingBookWPF.Models
{
    [AddINotifyPropertyChangedInterface]
    internal class Step : INotifyPropertyChanged
    {
        public int StepId { get; set; }
        public byte[] Image { get; set; } = null!;

        private string _description = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        [MaxLength(100)]
        public string Description
        {
            get => _description;
            set => _description = value.TrimStart().Replace("  ", " ");
        }
    }
}
