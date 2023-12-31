﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WallpaperChanger.Model
{
    public class WallpaperService : IDisposable
    {
        // アクセス修飾子がprivateのstatic変数に生成したインスタンスを保存する
        private static WallpaperService? _SingleInstance = null;

        private Timer _Timer = new Timer(30000);
        private CircularCounter _CircularCounter = new CircularCounter(0);
        private SettingController _SettingController = SettingController.GetInstance();

        private WallpaperService()
        {
            var setting = _SettingController.GetSetting();
            _CircularCounter = new CircularCounter(setting.PathList.Count);
            // タイマーにイベントを登録
            _Timer.Interval = setting.Interval;
            _Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        // インスタンスの取得はstaticプロパティもしくはstaticメソッドから行えるようにする
        // staticメソッドの場合
        public static WallpaperService GetInstance()
        {
            if (_SingleInstance is null)
            {
                _SingleInstance = new WallpaperService();
            }
            return _SingleInstance;
        }

        public bool Enabled
        {
            set
            {
                _Timer.Enabled = value;
            }
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            var setting = _SettingController.GetSetting();

            _CircularCounter.MaxValue = setting.PathList.Count;

            _CircularCounter.Shuffle = setting.Shuffle;

            var pathList = setting.PathList;

            if (pathList.Count > 0)
            {
                var path = pathList[_CircularCounter!.GetCounter()];

                WallPaper.Change(path);
            }

            _Timer.Interval = setting.Interval;
        }
        public double Interval
        {
            set
            {
                if (value <= 0)
                {
                    return;
                }
                _Timer.Interval = value;
            }
        }
        public void Stop()
        {
            _Timer.Stop();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
