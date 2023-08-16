using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic.Logging;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WallpaperChanger.Model;
using WallpaperChanger.View;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WallpaperChanger.ViewModel
{
    public class MainWindowViewModel
    {
        public ReactiveCommand<DragEventArgs> DropCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ReactiveCommand<DragEventArgs> PreviewDragOverCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ObservableCollection<ImageModel> ImageList { get; set; } = new ObservableCollection<ImageModel>();
        public ReactiveCommand DeleteImageCommand { get; } = new ReactiveCommand();
        public ReactiveCommand OpenFileFolderCommand { get; } = new ReactiveCommand();
        public ReactiveCommand MySetSettingCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<int> Hour { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Minute { get; } = new ReactiveProperty<int>(1);
        public ReadOnlyReactiveProperty<double> Interval { get; set; }
        public ReactiveProperty<bool> Shuffle { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> StartUp { get; set; } = new ReactiveProperty<bool>();
        public ObservableCollection<string> MySetNameList { get; set; } = new ObservableCollection<string>();
        public ReactiveProperty<string> SelectedMySet { get; } = new ReactiveProperty<string>();
        public ReactiveCommand MySetApplyCommand { get; } = new ReactiveCommand();
        public IDialogCoordinator? MahAppsDialogCoordinator { get; set; }
        public AsyncReactiveCommand ViewLoaded { get; } = new AsyncReactiveCommand();

        private SettingController _SettingController = SettingController.GetInstance();
        public MainWindowViewModel()
        {
            var setting = _SettingController.GetSetting();

            Shuffle.Value = setting.Shuffle;

            StartUp.Value = setting.StartUp;

            var timeSpan = TimeSpan.FromMilliseconds(setting.Interval);

            Hour.Value = timeSpan.Hours;
            Minute.Value = timeSpan.Minutes;

            ViewLoaded.Subscribe(async e =>
            {
                var controller = await MahAppsDialogCoordinator.ShowProgressAsync(this, "読み込み中", "読み込み中");
                SetMySetNameList();

                var setting = _SettingController.GetSetting();

                foreach (var path in setting.PathList)
                {
                    var model = new ImageModel(ImageUtil.Convert(path), path);
                    ImageList.Add(model);
                }

                await controller.CloseAsync();
            });

            DropCommand.Subscribe(e =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                AddAll(files);
            });

            PreviewDragOverCommand.Subscribe(e =>
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
            });

            DeleteImageCommand.Subscribe(async e =>
            {
                var model = e as ImageModel;

                if (model is null || MahAppsDialogCoordinator is null)
                {
                    return;
                }

                var metroDialogSettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "はい",
                    NegativeButtonText = "いいえ",
                    AnimateHide = true,
                    AnimateShow = true,
                    ColorScheme = MetroDialogColorScheme.Theme,
                };

                var diagResult = await MahAppsDialogCoordinator.ShowMessageAsync(this, "削除の確認", "壁紙一覧から削除します。よろしいですか？", MessageDialogStyle.AffirmativeAndNegative, settings: metroDialogSettings);
                if (diagResult == MessageDialogResult.Affirmative)
                {
                    ImageList.Remove(model);
                }
            });

            OpenFileFolderCommand.Subscribe(e =>
            {
                var model = e as ImageModel;

                if (model is null)
                {
                    return;
                }

                var path = Path.GetDirectoryName(model.FullPath);
                if (path is not null)
                {
                    Process.Start("explorer.exe", path);
                }
            });

            MySetSettingCommand.Subscribe(e =>
            {
                var dialog = new MySetWindow();
                dialog.ShowDialog();

                SetMySetNameList();
            });

            Interval = Hour.CombineLatest(Minute, (hours, minutes) =>
            {
                var timeSpan = new TimeSpan(hours, minutes, 0);
                return timeSpan.TotalMilliseconds;
            }).ToReadOnlyReactiveProperty();

            Interval.Subscribe(e =>
            {
                if (e <= 0)
                {
                    return;
                }
                WallpaperService.GetInstance().Interval = e;
                var setting = _SettingController.GetSetting();
                setting.Interval = e;
                _SettingController.SaveSetting(setting);
            });

            Shuffle.Subscribe(e =>
            {
                var setting = _SettingController.GetSetting();
                setting.Shuffle = e;
                _SettingController.SaveSetting(setting);
            });

            StartUp.Subscribe(e =>
            {
                StartUpUtil.Setting(e);
                var setting = _SettingController.GetSetting();
                setting.StartUp = e;
                _SettingController.SaveSetting(setting);
            });

            MySetApplyCommand.Subscribe(async e =>
            {
                var controller = await MahAppsDialogCoordinator.ShowProgressAsync(this, "読み込み中", string.Empty);

                var setting = _SettingController.GetSetting();

                if (!setting.MySet.ContainsKey(SelectedMySet.Value))
                {
                    return;
                }

                Clear();

                AddAll(setting.MySet[SelectedMySet.Value]);

                await controller.CloseAsync();
            });
        }
        private void AddAll(IEnumerable<string> files)
        {
            var setting = _SettingController.GetSetting();

            foreach (var file in files)
            {
                if (setting.PathList.Contains(file))
                {
                    continue;
                }

                setting.PathList.Add(file);

                var model = new ImageModel(ImageUtil.Convert(file), file);

                ImageList.Add(model);
            }

            _SettingController.SaveSetting(setting);
        }

        private void Clear()
        {
            var setting = _SettingController.GetSetting();
            setting.PathList.Clear();
            ImageList.Clear();
            _SettingController.SaveSetting(setting);
        }

        private void SetMySetNameList()
        {
            var setting = _SettingController.GetSetting();

            MySetNameList = new ObservableCollection<string>(setting.MySet.Select(x => x.Key));
        }
    }

}
