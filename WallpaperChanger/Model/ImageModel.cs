using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WallpaperChanger.Model
{
    public class ImageModel : BindableBase
    {
        private  BitmapImage? _Source;

        public BitmapImage? Source
        {
            get => _Source;
            set => SetProperty(ref _Source, value);
        }
    }
}
