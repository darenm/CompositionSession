using System;
using Windows.UI.Xaml.Data;
using MediaSample.Services;

namespace MediaSample.Converters
{
    public class PosterConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as Poster;
        }

        #endregion
    }
}