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
        public string[] Path { get; set; } = new string[0];

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
            _CircularCounter = new CircularCounter(Path.Length);
            // タイマーにイベントを登録
            _Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _Timer.Start();
        }

        private void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            var path = Path[_CircularCounter!.GetCounter()];

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
