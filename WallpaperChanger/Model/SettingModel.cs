using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class SettingModel
    {
        public List<string> PathList { get; set; } = new List<string>();

        public double Interval { get; set; } = 1000 * 60;

        public bool Shuffle { get; set; } = false;

        public bool StartUp { get; set; } = false;
    }
}
