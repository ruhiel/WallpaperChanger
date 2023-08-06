using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class WallPaper
    {
        const uint SPI_SETDESKWALLPAPER = 20;
        const uint SPIF_UPDATEINIFILE = 1;
        const uint SPIF_SENDWININICHANGE = 2;
        const uint SPI_GETDESKWALLPAPER = 0x73;
        const int MAX_PATH = 260;

        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

        public static bool Change(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    var image = Image.FromFile(filePath);
                    image.Dispose();

                    var sb = new StringBuilder(filePath);
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, (uint)sb.Length, sb, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                var sb = new StringBuilder("");
                SystemParametersInfo(SPI_SETDESKWALLPAPER, (uint)sb.Length, sb, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                return true;
            }
        }
    }
}
