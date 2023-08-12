using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WallpaperChanger.Model;

namespace WallpaperChanger.ViewModel
{
    public class MainWindowViewModel
    {
        public ReactiveCommand<DragEventArgs> DropCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ReactiveCommand<DragEventArgs> PreviewDragOverCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ObservableCollection<ImageModel> ImageList { get; set; } = new ObservableCollection<ImageModel>();
        public ReactiveCommand DeleteImageCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<int> Hour { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Minute { get; } = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<double> Interval { get; set; } 
        private SettingController _SettingController = SettingController.GetInstance();

        public MainWindowViewModel()
        {
            Create();

            var setting = _SettingController.GetSetting();

            var timeSpan = TimeSpan.FromMilliseconds(setting.Interval);

            foreach(var path in setting.PathList)
            {
                var model = new ImageModel();
                model.Source = ImageUtil.Convert(path);
                ImageList.Add(model);
            }

            Hour.Value = timeSpan.Hours;
            Minute.Value = timeSpan.Minutes;

            DropCommand.Subscribe(e =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                var setting = _SettingController.GetSetting();

                foreach (var file in files)
                {
                    if (setting.PathList.Contains(file))
                    {
                        continue;
                    }

                    setting.PathList.Add(file);


                    var model = new ImageModel();

                    model.Source = ImageUtil.Convert(file);

                    ImageList.Add(model);

                    _SettingController.SaveSetting(setting);
                }
            });

            PreviewDragOverCommand.Subscribe(e =>
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
            });

            DeleteImageCommand.Subscribe(e =>
            {
                var model = e as ImageModel;

                if(model is null)
                {
                    return;
                }

                ImageList.Remove(model);
            });

            Interval = Hour.CombineLatest(Minute, (hours, minutes) =>
            {
                var timeSpan = new TimeSpan(hours, minutes, 0);
                return timeSpan.TotalMilliseconds;
            }).ToReadOnlyReactiveProperty();

            Interval.Subscribe(e =>
            {
                WallpaperService.GetInstance().Interval = e;
                var setting = _SettingController.GetSetting();
                setting.Interval = e;
                _SettingController.SaveSetting(setting);
            });
        }

        private void Create()
        {
            var fullPath = System.Windows.Forms.Application.ExecutablePath;

            var fileName = Path.GetFileNameWithoutExtension(fullPath);

            //作成するショートカットのパス
            var shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                $"{fileName}.lnk");

            //ショートカットのリンク先
            var targetPath = fullPath ?? string.Empty;

            //WshShellを作成
            var t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            dynamic? shell = Activator.CreateInstance(t);

            //WshShortcutを作成
            var shortcut = shell.CreateShortcut(shortcutPath);

            //リンク先
            shortcut.TargetPath = targetPath;
            //アイコンのパス
            shortcut.IconLocation = fullPath + ",0";
            //その他のプロパティも同様に設定できるため、省略

            //ショートカットを作成
            shortcut.Save();

            //後始末
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
        }
    }

}
