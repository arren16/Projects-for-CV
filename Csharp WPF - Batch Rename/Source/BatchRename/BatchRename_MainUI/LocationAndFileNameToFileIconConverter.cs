using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;

namespace BatchRename_MainUI
{
    public static class ConverterHelper
    {
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

    }
    internal class LocationAndFileNameToFileIconConverter : IMultiValueConverter
    {
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string? location = "";
            string? filename = "";
            bool isFolder = false;

            bool isInvalidValues = (
                values == null
                || values.Length == 0
                || values[0] == DependencyProperty.UnsetValue
                || values[1] == DependencyProperty.UnsetValue
                || values[2] == DependencyProperty.UnsetValue
            );

            string folder = AppDomain.CurrentDomain.BaseDirectory;
            char delim = folder.Last();
            char unwantedDelim = (delim == '/') ? '\\' : '/';

            string fileIconPath = "resource/icon/file-16px.png";
            
            ImageSource imageSource = new BitmapImage(new Uri(fileIconPath, UriKind.Relative));

            if (!isInvalidValues)
            {
                    location = System.Convert.ToString(values![0]);
                    filename = System.Convert.ToString(values![1]);
                    isFolder = System.Convert.ToBoolean(values[2]);

                    string filepath = string.Concat(location, delim, filename);
                    filepath = filepath.Replace(unwantedDelim, delim);

                try
                {
                    if (isFolder) 
                    {
                        string folderIconPath = "resource/icon/folder-24px.png";
                        imageSource = new BitmapImage(new Uri(folderIconPath, UriKind.Relative));
                    }
                    else 
                    {
                        var icon = Icon.ExtractAssociatedIcon(filepath);
                        imageSource = icon!.ToImageSource();
                    }
                }
                catch
                {
                    // Do nothing
                }
            }
            else
            {
                // Do nothing
            }

            return imageSource;
        }
    }
}
