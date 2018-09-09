using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            foreWindow = Hotkey.GetForegroundWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Hotkey.Regist(this, HotkeyModifiers.MOD_ALT, Key.OemTilde, () =>
            {
                OnHotkey();
            });
        }

        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            vm.Image_Urls.Clear();
            PicFinder finder = new PicFinder();

            foreach (string str in finder.GetImages(vm.Keyword))
            {
                vm.Image_Urls.Add(new ViewModel() { Image_Url = str });
            }
        }

        private void OnHotkey()
        {
            foreWindow = Hotkey.GetForegroundWindow();
            if (!this.IsVisible)
            {
                this.Show();
                this.Activate();
            }
            Hotkey.GetCursorPos(out Hotkey.POINT mouse_point);
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
            Hotkey.SetForegroundWindow(foreWindow);
            Hotkey.keybd_event(17, 0, 0, 0);
            Hotkey.keybd_event(86, 0, 0, 0);
            Thread.Sleep(10);
            Hotkey.keybd_event(86, 0, 2, 0);
            Hotkey.keybd_event(17, 0, 2, 0);
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
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TbIcon.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
