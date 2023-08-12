using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class SettingController
    {
        private static SettingController? _SingleInstance = null;
        private SettingController()
        {
        }

        public static SettingController GetInstance()
        {
            if (_SingleInstance is null)
            {
                _SingleInstance = new SettingController();
            }
            return _SingleInstance;
        }

        public SettingModel GetSetting()
        {
            if (File.Exists(JsonPath))
            {
                SettingModel? model;
                using (StreamReader sr = new StreamReader(JsonPath))
                {
                    // ファイルの内容を1つの文字列に読み込み
                    var jsonData = sr.ReadToEnd();

                    model = JsonConvert.DeserializeObject<SettingModel>(jsonData);
                }

                if(model == null)
                {
                    model = new SettingModel();
                    SaveSetting(model);
                    return model;
                }
                else
                {
                    return model;
                }
            }
            else
            {
                var model = new SettingModel();
                SaveSetting(model);
                return model;
            }
        }

        public void SaveSetting(SettingModel setting)
        {
            var jsonData = JsonConvert.SerializeObject(setting);

            using (StreamWriter sw = new StreamWriter(JsonPath))
            {
                sw.Write(jsonData);
            }
        }

        private string JsonPath => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty, "settings.json");
    }
}
