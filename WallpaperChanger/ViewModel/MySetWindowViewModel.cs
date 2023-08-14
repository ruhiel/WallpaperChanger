using MahApps.Metro.Controls.Dialogs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using WallpaperChanger.Model;

namespace WallpaperChanger.ViewModel
{
    public class MySetWindowViewModel
    {
        public ReactiveCommand<DragEventArgs> DropCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ReactiveCommand<DragEventArgs> PreviewDragOverCommand { get; } = new ReactiveCommand<DragEventArgs>();
        public ObservableCollection<ImageModel> ImageList { get; set; } = new ObservableCollection<ImageModel>();
        public ReactiveProperty<string> MySetName { get; set; } = new ReactiveProperty<string>();
        public ObservableCollection<string> MySetNameList { get; set; } = new ObservableCollection<string>();
        public ReactiveCommand DeleteImageCommand { get; } = new ReactiveCommand();
        public ReactiveCommand OpenFileFolderCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<string> SelectedMySet { get; } = new ReactiveProperty<string>();
        public ReactiveCommand AddMySetCommand { get; } = new ReactiveCommand();
        public IDialogCoordinator? MahAppsDialogCoordinator { get; set; }
        private SettingController _SettingController = SettingController.GetInstance();
        public MySetWindowViewModel()
        {
            var setting = _SettingController.GetSetting();

            MySetNameList = new ObservableCollection<string>(setting.MySet.Select(x => x.Key));

            SelectedMySet.Subscribe(e =>
            {
                if (string.IsNullOrEmpty(e))
                {
                    return;
                }

                var setting = _SettingController.GetSetting();

                if (!setting.MySet.ContainsKey(e))
                {
                    return;
                }

                ImageList.Clear();

                foreach (var file in setting.MySet[e])
                {
                    var model = new ImageModel(ImageUtil.Convert(file), file);

                    ImageList.Add(model);
                }
            });

            AddMySetCommand.Subscribe(e =>
            {
                if (string.IsNullOrEmpty(MySetName.Value))
                {
                    return;
                }

                var setting = _SettingController.GetSetting();

                MySetNameList.Add(MySetName.Value);

                setting.MySet.Add(MySetName.Value, new List<string>());

                _SettingController.SaveSetting(setting);

                SelectedMySet.Value = MySetName.Value;

                MySetName.Value = string.Empty;
            });

            DropCommand.Subscribe(e =>
            {
                var setting = _SettingController.GetSetting();

                if (!setting.MySet.ContainsKey(SelectedMySet.Value))
                {
                    return;
                }

                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var pathList = setting.MySet[SelectedMySet.Value];

                foreach (var file in files)
                {
                    if (pathList.Contains(file))
                    {
                        continue;
                    }

                    var model = new ImageModel(ImageUtil.Convert(file), file);

                    ImageList.Add(model);

                    pathList.Add(file);
                }

                _SettingController.SaveSetting(setting);
            });

            PreviewDragOverCommand.Subscribe(e =>
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
            });

            DeleteImageCommand.Subscribe(async e =>
            {
                var setting = _SettingController.GetSetting();

                if (!setting.MySet.ContainsKey(SelectedMySet.Value))
                {
                    return;
                }

                var pathList = setting.MySet[SelectedMySet.Value];

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

                    pathList.Remove(model.FullPath);

                    _SettingController.SaveSetting(setting);
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
        }
    }
}
