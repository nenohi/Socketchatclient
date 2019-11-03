using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Example
{
    /// <summary>
    /// screan.xaml の相互作用ロジック
    /// </summary>
    public partial class Screan : Window
    {
        //Window window = new Window();

        Style labelstyle = new Style();
        int lcnt = 0;
        int[] fntarry = new int[50];
        int[] msgarry = new int[50];
        int[] texthi = new int[25];
        int[] textcnt = new int[25];
        int waido = 0;

        #region DependencyProperties

        #region AltF4Cancel

        public bool AltF4Cancel
        {
            get { return (bool)GetValue(AltF4CancelProperty); }
            set { SetValue(AltF4CancelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AltF4Cancel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AltF4CancelProperty =
            DependencyProperty.Register(
                "AltF4Cancel",
                typeof(bool),
                typeof(Screan),
                new PropertyMetadata(true));

        #endregion

        #region ShowSystemMenu

        public bool ShowSystemMenu
        {
            get { return (bool)GetValue(ShowSystemMenuProperty); }
            set { SetValue(ShowSystemMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSystemMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSystemMenuProperty =
            DependencyProperty.Register(
                "ShowSystemMenu",
                typeof(bool),
                typeof(Screan),
                new PropertyMetadata(
                    false,
                    ShowSystemMenuPropertyChanged));

        private static void ShowSystemMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is Screan window)
            {

                window.SetShowSystemMenu((bool)e.NewValue);

            }

        }

        #endregion

        #region ClickThrough

        public bool ClickThrough
        {
            get { return (bool)GetValue(ClickThroughProperty); }
            set { SetValue(ClickThroughProperty, value); }
        }


        // Using a DependencyProperty as the backing store for ClickThrough.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickThroughProperty =
            DependencyProperty.Register(
                "ClickThrough",
                typeof(bool),
                typeof(Screan),
                new PropertyMetadata(
                    true,
                    ClickThroughPropertyChanged));

        private static void ClickThroughPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is Screan window)
            {

                window.SetClickThrough((bool)e.NewValue);

            }

        }

        #endregion

        #endregion

        #region const values

        private const int GWL_STYLE = (-16); // ウィンドウスタイル
        private const int GWL_EXSTYLE = (-20); // 拡張ウィンドウスタイル

        //private const int WS_SYSMENU = 0x00080000; // システムメニュを表示する
        private const int WS_EX_TRANSPARENT = 0x00000020; // 透過ウィンドウスタイル

        private const int WM_SYSKEYDOWN = 0x0104; // Alt + 任意のキー の入力

        private const int VK_F4 = 0x73;

        #endregion

        #region Win32Apis

        [DllImport("user32")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);

        #endregion


        protected override void OnSourceInitialized(EventArgs e)
        {

            //システムメニュを非表示
            this.SetShowSystemMenu(this.ShowSystemMenu);

            //クリックをスルー
            this.SetClickThrough(this.ClickThrough);

            //Alt + F4 を無効化
            var handle = new WindowInteropHelper(this).Handle;
            var hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(WndProc);
            base.OnSourceInitialized(e);

            var width = 0;
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                //ディスプレイのデバイス名を表示
                //Console.WriteLine("デバイス名:{0}", s.DeviceName);
                //ディスプレイの左上の座標を表示
                //Console.WriteLine("X:{0} Y:{1}", s.Bounds.X, s.Bounds.Y);
                //ディスプレイの大きさを表示
                //Console.WriteLine("高さ:{0} 幅:{1}", s.Bounds.Height, s.Bounds.Width);
                
                this.Width += s.Bounds.Width;
                waido += s.Bounds.Width;
            }
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr IParam, ref bool handled)
        {

            //Alt + F4 が入力されたら
            if (msg == WM_SYSKEYDOWN && wParam.ToInt32() == VK_F4)
            {

                if (this.AltF4Cancel)
                {

                    //処理済みにセットする
                    //(Windowは閉じられなくなる)
                    handled = true;

                }

            }

            return IntPtr.Zero;

        }

        /// <summary>
        /// システムメニュの表示を切り替える
        /// </summary>
        /// <param name="value"><see langword="true"/> = 表示, <see langword="false"/> = 非表示</param>
        protected void SetShowSystemMenu(bool value)
        {

            try
            {

                var handle = new WindowInteropHelper(this).Handle;

                int windowStyle = GetWindowLong(handle, GWL_STYLE);

                if (value)
                {
                  //  windowStyle |= WS_SYSMENU; //フラグの追加
                }
                else
                {
                //    windowStyle &= ~WS_SYSMENU; //フラグを消す
                }

                SetWindowLong(handle, GWL_STYLE, windowStyle);

            }
            catch
            {

            }

        }

        /// <summary>
        /// クリックスルーの設定
        /// </summary>
        /// <param name="value"><see langword="true"/> = クリックをスルー, <see langword="false"/>=クリックを捉える</param>
        protected void SetClickThrough(bool value)
        {

            try
            {

                var handle = new WindowInteropHelper(this).Handle;

                int extendStyle = GetWindowLong(handle, GWL_EXSTYLE);

                if (value)
                {
                    extendStyle |= WS_EX_TRANSPARENT; //フラグの追加
                }
                else
                {
                    extendStyle &= ~WS_EX_TRANSPARENT; //フラグを消す
                }

                SetWindowLong(handle, GWL_EXSTYLE, extendStyle);

            }
            catch
            {

            }

        }



        public Screan()
        {
            InitializeComponent();
        }

        public void Addtext(JObject msg)
        {
            int lcount = 0;
            double wid = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            Boolean labelscln = true;
            var message = msg.ToObject<Msgcreat>();
            int fontsz = 75;

            message.Color = message.Color.Replace("#", "");
            if (message.Color.Length == 3)
            {
                string tmpcolor = "";
                for (int i = 0; message.Color.Length < i; i++)
                {
                    tmpcolor = message.Color[i].ToString();
                }
            }
            double textsize;
            Boolean mir = false;
            //wid = 1920;
            switch (message.Size)
            {
                case "small":
                    fontsz = 50;
                    break;
                case "medium":
                    fontsz = 100;
                    break;
                case "big":
                    fontsz = 125;
                    break;
                default:
                    break;
            }
            int ttxetsize = 0;
            Boolean findtext = true;
            for (int i = 0; i < texthi.Length && findtext; i++)
            {
                if (textcnt[i] == 0)
                {
                    textcnt[i] = 1;
                    texthi[i] = fontsz;
                    lcount = i;
                    findtext = false;
                }
                else
                {
                    ttxetsize += texthi[i];
                }

            }
            if (Regex.IsMatch(message.Text, "/mr"))
            {
                message.Text.Replace("/mr", "");
                string[] newmassage = new string[message.Text.Length];
                for (int i = message.Text.Length-1,l = 0; i >= 0;i--,l++)
                {
                    newmassage[l] = message.Text[i].ToString();
                }
                message.Text = newmassage.ToString();
            }
            Console.Write(wid);
            textsize = message.Text.Length*fontsz;
            if (mir)
            {
                wid = -textsize;
            }

            Label label1 = new Label
            {
                Content = message.Text,
                Margin = new Thickness(wid,  ttxetsize, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = fontsz,
                Foreground= System.Windows.Media.Brushes.White,
        };
            
            Label source = (Label)label1;
            
            grid1.Children.Add(label1);

            TranslateTransform transform = source.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                source.RenderTransformOrigin = source.PointFromScreen(new System.Windows.Point());
                transform = new TranslateTransform();
                source.RenderTransform = transform;
            }
            if (mir)
            {
                transform.BeginAnimation(TranslateTransform.XProperty,
                new DoubleAnimation(-(message.Text.Length*(fontsz/10)+wid), TimeSpan.FromMilliseconds(7000)),
                HandoffBehavior.Compose);
            }
            else
            {
                transform.BeginAnimation(TranslateTransform.XProperty,
                new DoubleAnimation(-(textsize + wid), TimeSpan.FromMilliseconds(7000)),
                HandoffBehavior.Compose);

            }



            //label1.Width = Auto;
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(7000) };
            timer.Start();
            timer.Tick += (s, args) =>
            {
                // タイマーの停止
                timer.Stop();
                grid1.Children.Remove(label1);
                //textcnt[lcount] = 0;

                // 以下に待機後の処理を書く
            };
            DispatcherTimer timer12 = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3500) };
            timer12.Start();
            timer12.Tick += (s, args) =>
            {
                // タイマーの停止
                timer12.Stop();
                //grid1.Children.Remove(label1);
                textcnt[lcount] = 0;

                // 以下に待機後の処理を書く
            };

            //Console.WriteLine(label1);
            //Console.WriteLine(label1);

        }
    }
}
