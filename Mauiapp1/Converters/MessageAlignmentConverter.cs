using System.Globalization;
using Microsoft.Maui.Layouts;

namespace Mauiapp1.Converters
{
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFromCurrentUser = (bool)value;

            // Check if the target type is FlexJustify (for FlexLayout)
            if (targetType == typeof(FlexJustify) || parameter as string == "flex")
            {
                return isFromCurrentUser ? FlexJustify.End : FlexJustify.Start;
            }

            // Default for LayoutOptions
            return isFromCurrentUser ? LayoutOptions.End : LayoutOptions.Start;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}