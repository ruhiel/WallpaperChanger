using System;
using System.Collections.Generic;
using System.IO;
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

        public string FullPath { get; set; }

        public string FileName
        {
            get
            {
                return string.IsNullOrEmpty(FullPath) ? string.Empty : Path.GetFileName(FullPath);
            }
        }

        public string LastWriteTime
        {
            get
            {
                if (string.IsNullOrEmpty(FullPath))
                {
                    return string.Empty;
                }
                
                var dt = File.GetLastWriteTime(FullPath);
                return dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        public ImageModel(BitmapImage? source, string fullPath)
        {
            Source = source;

            FullPath = fullPath;
        }
    }
}
