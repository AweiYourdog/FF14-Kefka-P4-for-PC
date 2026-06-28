using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UMADOverlay.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (v is true) ^ Invert ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => (v is Visibility.Visible) ^ Invert;
    }

    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush TrueBrush  { get; set; } = Brushes.Transparent;
        public Brush FalseBrush { get; set; } = Brushes.Transparent;
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is true ? TrueBrush : FalseBrush;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }

    /// <summary>string → Visibility: empty/null → Collapsed</summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => string.IsNullOrEmpty(v as string) ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }

    /// <summary>
    /// bool → Color.  ConverterParameter = "TrueHex|FalseHex"  e.g. "FFCC00|3a3a3a"
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public static readonly BoolToColorConverter Instance = new();
        public object Convert(object v, Type t, object p, CultureInfo c)
        {
            var parts = (p as string ?? "FFFFFF|444444").Split('|');
            var hex   = v is true ? parts[0] : (parts.Length > 1 ? parts[1] : "444444");
            return (Color)ColorConverter.ConvertFromString("#" + hex);
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }

    /// <summary>System.Windows.Media.Color → SolidColorBrush</summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public static readonly ColorToBrushConverter Instance = new();
        public object Convert(object v, Type t, object p, CultureInfo c)
            => v is Color col ? new SolidColorBrush(col) : Brushes.Transparent;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }
}
