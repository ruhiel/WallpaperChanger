using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var setting = _SettingController.GetSetting();

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(setting.Interval);

            Hour.Value = timeSpan.Hours;
            Minute.Value = timeSpan.Minutes;

            DropCommand.Subscribe(e =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var filePath = files[0];

                var model = new ImageModel();

                model.Source = ImageUtil.Convert(filePath);

                ImageList.Add(model);

                var setting = _SettingController.GetSetting();

                setting.PathList.Add(filePath);

                _SettingController.SaveSetting(setting);

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
                var setting = _SettingController.GetSetting();
                setting.Interval = e;
                _SettingController.SaveSetting(setting);
            });
        }
    }
}
