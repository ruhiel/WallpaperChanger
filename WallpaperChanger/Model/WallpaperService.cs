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
        private Timer _Timer = new Timer(30000);
        private CircularCounter? _CircularCounter;
        private string[] _Path = new string[] {
            @"E:\画像\ブルーアーカイブ\生徒\ミカ\F2Vg_rhbkAEjr8x.jpg"
            , @"E:\画像\ブルーアーカイブ\生徒\ミカ\Fsi-cx5aIAEx0VM.jpg"
            ,@"E:\画像\ブルーアーカイブ\生徒\ミカ\F2hFujebIAAYWUt.jpg"
        };

        public void Start()
        {
            _CircularCounter = new CircularCounter(_Path.Length);
            // タイマーにイベントを登録
            _Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _Timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
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
