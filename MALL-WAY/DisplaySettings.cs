using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace MallWay
{
    // Presentation-only translations: database identifiers and graph data remain stable.
    public sealed class DisplaySettings : ObservableObject
    {
        public static DisplaySettings Current { get; } = new DisplaySettings();
        private readonly Dictionary<string, string> english;
        private bool isEnglish;
        private bool lowVision;
        public event EventHandler Changed;
        public bool IsEnglish => isEnglish;
        public bool LowVision => lowVision;

        private DisplaySettings()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "English.json");
            english = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
        }

        public string Translate(string text)
        {
            if (text == null || !IsEnglish) return text;
            return EnglishText(text);
        }

        public string EnglishText(string text)
        {
            if (text == null) return null;
            if (english.TryGetValue(text, out var translated)) return translated;
            const string entrance = " 입구";
            if (text.EndsWith(entrance, StringComparison.Ordinal))
                return EnglishText(text.Substring(0, text.Length - entrance.Length)) + " entrance";
            return text; // Custom, untranslated mall data stays readable.
        }

        public void SetLanguage(bool englishLanguage)
        {
            isEnglish = englishLanguage;
            foreach (var item in english)
                Application.Current.Resources[item.Key] = Translate(item.Key);
            RaisePropertyChanged(nameof(IsEnglish));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetLowVision(bool enabled)
        {
            lowVision = enabled;
            var resources = Application.Current.Resources;
            foreach (var item in ThemePalette.Colors)
                resources["Color" + item.Key.TrimStart('#')] = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(enabled ? item.Value : item.Key));
            foreach (var size in ThemePalette.FontSizes)
                resources["Font" + size] = enabled ? Math.Max(20d, size * 1.3d) : (double)size;
            resources["ButtonHeight"] = enabled ? 60d : 44d;
            resources["RouteThickness"] = enabled ? 9d : 5d;
            resources["LeftPanelWidth"] = new GridLength(enabled ? 310 : 270);
            resources["RightPanelWidth"] = new GridLength(enabled ? 310 : 252);
            resources["MapDisplayWidth"] = enabled ? 1000d : double.NaN;
            RaisePropertyChanged(nameof(LowVision));
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Both the bound value and the language participate in the binding, so an open
    // page, map marker, or selected category updates immediately without reloading DB data.
    public sealed class TranslateExtension : MarkupExtension
    {
        public string Path { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new MultiBinding { Converter = TranslationConverter.Instance };
            binding.Bindings.Add(new Binding(Path ?? "."));
            binding.Bindings.Add(new Binding(nameof(DisplaySettings.IsEnglish)) { Source = DisplaySettings.Current });
            return binding.ProvideValue(serviceProvider);
        }
    }

    public sealed class TranslationConverter : IMultiValueConverter
    {
        public static TranslationConverter Instance { get; } = new TranslationConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values.Length == 0 || values[0] == DependencyProperty.UnsetValue
                ? "" : DisplaySettings.Current.Translate(values[0]?.ToString());
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
