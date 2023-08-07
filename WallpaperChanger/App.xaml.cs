using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using WallpaperChanger.Model;

namespace WallpaperChanger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private MainWindow? _win = null;  //２重起動防止用
        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.ContextMenuStrip? _menu = null;
        //常駐終了時に開放するために保存しておく
        private System.Windows.Forms.NotifyIcon? _notifyIcon = null;
        private WallpaperService _service = WallpaperService.GetInstance();
        /// <summary>
        /// 常駐開始時の初期化処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //継承元のOnStartupを呼び出す
            base.OnStartup(e);

            //アイコンの取得
            var icon = GetResourceStream(new Uri("images_117786.ico", UriKind.Relative)).Stream;

            //コンテキストメニューを作成
            _menu = CreateMenu();

            //通知領域にアイコンを表示

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
                ContextMenuStrip = _menu
            };

            //アイコンがクリックされたら設定画面を表示
            _notifyIcon.MouseClick += (s, er) =>
            {
                if (er.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ShowMainWindow();
                }
            };

            _service.Start();
        }

        /// <summary>
        /// 常駐終了時の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            _menu?.Dispose();
            _notifyIcon?.Dispose();
            _service.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// 設定画面を表示
        /// </summary>
        private void ShowMainWindow()
        {
            if (_win == null)
            {
                _win = new MainWindow();

                //ウィンドウを画面中央に表示
                _win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                //Windowsを表示する
                _win.Show();

                //閉じるボタンが押された時のイベント処理を登録
                _win.Closing += (s, e) =>
                {
                    _win.Hide();        //非表示にする
                    e.Cancel = true;    //閉じるをキャンセルする
                };
            }
            else
            {
                //Windowsを表示する
                _win.Show();
            }
        }

        /// <summary>
        /// コンテキストメニューの表示
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("各種設定", null, (s, e) => { ShowMainWindow(); });
            menu.Items.Add($"{System.Diagnostics.Process.GetCurrentProcess().ProcessName}を終了", null, (s, e) => { Shutdown(); });
            return menu;
        }
    }

}