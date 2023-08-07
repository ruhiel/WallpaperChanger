using System;
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
        private static WallpaperService? singleInstance = null;

        private Timer _Timer = new Timer(30000);
        private CircularCounter? _CircularCounter;
        private string[] _Path = new string[] {
            @"E:\画像\ブルーアーカイブ\生徒\ミカ\F2Vg_rhbkAEjr8x.jpg"
            , @"E:\画像\ブルーアーカイブ\生徒\ミカ\Fsi-cx5aIAEx0VM.jpg"
            ,@"E:\画像\ブルーアーカイブ\生徒\ミカ\F2hFujebIAAYWUt.jpg"
        };
        private WallpaperService()
        {
        }

        // インスタンスの取得はstaticプロパティもしくはstaticメソッドから行えるようにする
        // staticメソッドの場合
        public static WallpaperService GetInstance()
        {
            if (singleInstance == null)
            {
                singleInstance = new WallpaperService();
            }
            return singleInstance;
        }

        public void Start()
        {
            _CircularCounter = new CircularCounter(_Path.Length);
            // タイマーにイベントを登録
            _Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _Timer.Start();
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            var path = _Path[_CircularCounter!.GetCounter()];

            WallPaper.Change(path);
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
