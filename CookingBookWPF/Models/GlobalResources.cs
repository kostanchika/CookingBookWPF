using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CookingBookWPF.Models
{
    internal static class GlobalResources
    {
        public static User User { get; set; } = null!;
        public static CookerProfile? CookerProfile { get; set; } = null!;
        public readonly static string[] CookingTimes = [
                "Все",
                "менее 30 минут",
                "30 минут",
                "1 час",
                "2 часа",
                "3 часа",
                "более 3 часов"
            ];
        public readonly static string[] Cuisines = [
                "Все",
                "Русская",
                "Английская",
                "Итальянская",
                "Китайская",
                "Японская",
                "Мексиканская",
                "Армянская",
                "Белорусская",
                "Испанская",
                "Бразильская",
                "Корейская",
                "Украинская",
                "Французская",
            ];
        public static string[] Categories = [
                "Все",
                "Первое",
                "Второе",
                "Мясное",
                "Вегетарианское",
                "Десерт",
                "Второе",
                "Лёгкая закуска",
                "На праздник",
                "На каждый день",
                "Для гостей",
                "Постное",
                "Экзотика"
            ];
        public readonly static string[] Difficulties = [
                "Все",
                "Простой",
                "Продвинутый",
                "Сложный",
                "Профессиональный",
            ];
        public readonly static string[] QuantityTypes = [
                "г",
                "кг",
                "мл",
                "л",
                "ч. л.",
                "ст. л.",
                "дольки",
                "штуки",
                "чашки",
            ];
        public readonly static string[] Sortings = [
                "По умолчанию",
                "По названию",
                "По избранным",
                "По калорийности",
                "По времени"
            ];

        public static ObservableCollection<string> Ingredients = [];

        static GlobalResources()
        {
            LoadIngredients();
        }

        public static void LoadIngredients()
        {
            var ingredientsList = DatabaseController.LoadIngredients();
        }
    }
}
