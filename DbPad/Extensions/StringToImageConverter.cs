using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace DbPad.Extensions;

public class StringToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string) || !File.Exists((string)value))
        {
            return null;
        }
        return new Bitmap((string)value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "";
    }
}