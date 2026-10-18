using System.Globalization;

namespace BugTrackLite.Converters;

// Renvoie true si le texte n'est pas vide : sert à n'afficher les messages d'erreur que s'il y en a un.
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
