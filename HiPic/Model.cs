using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;

namespace HiPic
{
    class Model
    {
        public string Image_Url { get; set; }
    }

    class ViewModel : INotifyPropertyChanged
    {
        private Model model = new Model();

        public string Image_Url
        {
            get => model.Image_Url;
            set
            {
                model.Image_Url = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image_Url)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class WindowViewModel : INotifyPropertyChanged
    {
        private string _keyword;

        public string Keyword
        {
            get => _keyword;
            set
            {
                _keyword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Keyword)));
            }
        }

        public ObservableCollection<ViewModel> Image_Urls { get; } = new ObservableCollection<ViewModel>();
        //public int Max
        //{
        //    get => _max;
        //    set
        //    {
        //        _max = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Max"));
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class FavoritesViewModel
    {
        public ObservableCollection<ViewModel> ImageUrls { get; } = new ObservableCollection<ViewModel>();

        public void AddFavorite(ViewModel imageVm)
        {
            if (imageVm == null || ImageUrls.Any(x => x.Image_Url == imageVm.Image_Url))
            {
                return;
            }
            ImageUrls.Insert(0, imageVm);
        }
        public bool RemoveFavorite(ViewModel imageVm)
        {
            return ImageUrls.Remove(ImageUrls.FirstOrDefault(x => x.Image_Url == imageVm.Image_Url));
        }
        public void ClearFavorite()
        {
            ImageUrls.Clear();
        }

    }

    static class FavoritesVmSerializer
    {
        public static void SerializeJson(this FavoritesViewModel vm)
        {
            string jsonString = JsonConvert.SerializeObject(vm);
            Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\HiPic");
            using (StreamWriter file = File.CreateText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\HiPic\\Favorites.json"))
            {
                file.Write(jsonString);
            }
        }

        public static FavoritesViewModel DeserializeJson()
        {
            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\HiPic\\Favorites.json"))
            {
                StreamReader reader = new StreamReader(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\HiPic\\Favorites.json");
                string jsonString = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<FavoritesViewModel>(jsonString);
            }
            else
            {
                return new FavoritesViewModel();
            }
        }
    }
}
