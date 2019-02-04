using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImgDiffTool
{
    public static class Extention
    {
        public static BitmapImage ToBitmapImage(this string source)
        {
            if (!File.Exists(source))
                return null;

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(source);
            image.EndInit();
            return image;
        }
    }
}
