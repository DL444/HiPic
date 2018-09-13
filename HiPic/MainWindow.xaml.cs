using System;
using System.Collections.Generic;
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
        const string apiRoot = "https://www.doutula.com/api/search";

        readonly WindowViewModel vm;
        readonly FavoritesViewModel favoVm;
        
        IntPtr foreWindow;
        BitmapImage bmp;
        
        // Invariant: this field is only assigned once in OnSourceInitialized
        HotKeyManager hkmgr;

        public MainWindow()
        {
            InitializeComponent();
            vm = (WindowViewModel) this.DataContext;
            FavoritesPage favoPage = new FavoritesPage(this);
            FavoriteFrame.Navigate(favoPage);
            favoVm = (FavoritesViewModel) favoPage.DataContext;
            foreWindow = WinApi.GetForegroundWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            hkmgr = new HotKeyManager(this);
            hkmgr.Register(HotKeyModifiers.Alt, Key.OemTilde, this.OnHotkey);
            hkmgr.Register(HotKeyModifiers.None, Key.Escape, this.Hide);
        }

        private async void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            ActionBtn.IsEnabled = false;
            vm.Image_Urls.Clear();

            List<string> images;
            try
            {
                images = await PicFinder.GetImages(apiRoot, vm.Keyword);
            }
            catch(System.Net.WebException)
            {
                ActionBtn.IsEnabled = true;
                return;
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
            InsertImage(new Uri(((ViewModel)Image_List.SelectedItem).Image_Url));
        }

        public void InsertImage(Uri imageUri)
        {
            bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = imageUri;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            favoVm.AddFavorite((ViewModel) ((System.Windows.Controls.MenuItem) sender).DataContext);
            favoVm.SerializeJson();
        }

        public void SetBitmap(BitmapImage image)
        {
            bmp = image;
        }
    }
}
