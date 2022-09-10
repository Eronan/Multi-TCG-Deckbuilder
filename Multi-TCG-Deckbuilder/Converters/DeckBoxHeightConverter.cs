using Multi_TCG_Deckbuilder.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Multi_TCG_Deckbuilder.Converters
{
    /// <summary>
    /// Converts a Relative File Path to an Absolute File Path or Empty Image
    /// </summary>
    public sealed class DeckBoxHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse(parameter.ToString()!) * ((double)value / 8.0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 40;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}