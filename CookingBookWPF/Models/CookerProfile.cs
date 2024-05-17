using PropertyChanged;

namespace CookingBookWPF.Models
{
    [AddINotifyPropertyChangedInterface]
    internal class CookerProfile
    {
        public int CookerProfileId { get; set; }

        private string _profileName = string.Empty;
        public string ProfileName
        {
            get => _profileName;
            set => _profileName = value.TrimStart().Replace("  ", " ");
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set => _fullName = value.TrimStart().Replace("  ", " ");
        }
        public byte[] Avatar { get; set; } = null!;
        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => _description = value.TrimStart().Replace("  ", " ");
        }

        private string _slogan = string.Empty;
        public string Slogan
        {
            get => _slogan;
            set => _slogan = value.TrimStart().Replace("  ", " ");
        }
        private string _jobPlace = string.Empty;
        public string JobPlace
        {
            get => _jobPlace;
            set => _jobPlace = value.TrimStart().Replace("  ", " ");
        }

        public bool IsCompleted
        {
            get => ProfileName != string.Empty &&
                   Description != string.Empty &&
                   Avatar != null;
        }
    }
}
