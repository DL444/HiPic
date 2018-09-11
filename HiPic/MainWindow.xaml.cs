using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HiPic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowViewModel vm = null;
        IntPtr foreWindow;
        BitmapImage bmp;

        public MainWindow()
        {
            InitializeComponent();
            vm = this.DataContext as WindowViewModel;
            this.ShowInTaskbar = false;
            foreWindow = WinApi.GetForegroundWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.OemTilde, () =>
            {
                OnHotkey();
            });
        }

        private async void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            ActionBtn.IsEnabled = false;
            vm.Image_Urls.Clear();
            PicFinder finder = new PicFinder();

            List<string> images = null;
            try
            {
                images = await finder.GetImages(vm.Keyword);
            }
            catch(System.Net.Http.HttpRequestException)
            {
                ActionBtn.IsEnabled = true;
                images = new List<string>();
            }
            foreach (string str in images)
            {
                vm.Image_Urls.Add(new ViewModel() { Image_Url = str });
            }
            ActionBtn.IsEnabled = true;
        }

        private void OnHotkey()
        {
            foreWindow = WinApi.GetForegroundWindow();
            if (!this.IsVisible)
            {
                this.Show();
                this.Activate();
            }
            WinApi.GetCursorPos(out WinApi.POINT mouse_point);
            PresentationSource source = PresentationSource.FromVisual(this);
            this.Left = mouse_point.X / source.CompositionTarget.TransformToDevice.M11;
            this.Top = mouse_point.Y / source.CompositionTarget.TransformToDevice.M22;
            KeywordBox.Focus();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Image_Downloaded(object sender, EventArgs e)
        {
            Send_Pic();
        }

        private void Send_Pic()
        {
            bmp.Freeze();
            Clipboard.SetImage(bmp);
            this.Hide();
            WinApi.SetForegroundWindow(foreWindow);
            WinApi.SendMessage(foreWindow, WM.WM_PASTE, IntPtr.Zero, IntPtr.Zero);
            vm.Keyword = "";
            vm.Image_Urls.Clear();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(((ViewModel)Image_List.SelectedItem).Image_Url);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            if (bmp.IsDownloading)
                bmp.DownloadCompleted += new EventHandler(Image_Downloaded);
            else
            {
                Send_Pic();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            TbIcon.ShowBalloonTip("Tips", "Press Alt + ~ to show the window.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TbIcon.Dispose();
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KeywordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ActionBtn_Click(this, null);
            }
        }
    }
}
