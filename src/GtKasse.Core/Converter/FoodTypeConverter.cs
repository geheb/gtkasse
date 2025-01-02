using GtKasse.Core.Models;

namespace GtKasse.Core.Converter;

public sealed class FoodTypeConverter
{
    public string TypeToString(FoodType type)
    {
        return type switch
        {
            FoodType.Drink => "Getränk",
            FoodType.Dish => "Speise",
            FoodType.Donation => "Spende",
            _ => $"Unbekannt: {type}"
        };
    }
}
