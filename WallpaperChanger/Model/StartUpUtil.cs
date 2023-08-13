using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperChanger.Model
{
    public class StartUpUtil
    {
        public static void Setting(bool startUp)
        {
            var fullPath = System.Windows.Forms.Application.ExecutablePath;

            var fileName = Path.GetFileNameWithoutExtension(fullPath);

            //作成するショートカットのパス
            var shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                $"{fileName}.lnk");

            var exists = File.Exists(shortcutPath);

            if (startUp && !exists)
            {
                // ショートカットのリンク先
                var targetPath = fullPath ?? string.Empty;

                // WshShellを作成
                var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));

                if (t is null)
                {
                    throw new Exception("WshShell作成に失敗");
                }

                dynamic? shell = Activator.CreateInstance(t);

                if (shell is null)
                {
                    throw new Exception("WshShell作成に失敗");
                }

                // WshShortcutを作成
                var shortcut = shell.CreateShortcut(shortcutPath);

                // リンク先
                shortcut.TargetPath = targetPath;
                // アイコンのパス
                shortcut.IconLocation = fullPath + ",0";
                // その他のプロパティも同様に設定できるため、省略

                // ショートカットを作成
                shortcut.Save();

                // 後始末
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
            }
            else if (!startUp && exists)
            {
                File.Delete(shortcutPath);
            }
        }
    }
}
