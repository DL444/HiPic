using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HiPic
{
    /// <summary>
    /// Interaction logic for FavoritesPage.xaml
    /// </summary>
    public partial class FavoritesPage : Page
    {
        readonly MainWindow mainWindow;
        readonly FavoritesViewModel vm;

        public FavoritesPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            vm = FavoritesVmSerializer.DeserializeJson();
            InitializeComponent();
            this.DataContext = vm;
        }

        private void FavoriteItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (ViewModel) ImageList.SelectedItem;
            mainWindow.InsertImage(new Uri(selectedItem.Image_Url));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveFavorite((ViewModel) ((MenuItem) sender).DataContext);
            vm.SerializeJson();
        }
    }
}
