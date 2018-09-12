using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for FavoritesPage.xaml
    /// </summary>
    public partial class FavoritesPage : Page
    {
        FavoritesViewModel vm;

        public FavoritesPage()
        {
            vm = FavoritesVmSerializer.DeserializeJson();
            InitializeComponent();
            this.DataContext = vm;
        }

        private void FavoriteItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).InsertImage(new Uri(((ViewModel)ImageList.SelectedItem).Image_Url));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            vm.RemoveFavorite((sender as MenuItem).DataContext as ViewModel);
            vm.SerializeJson();
        }
    }
}
