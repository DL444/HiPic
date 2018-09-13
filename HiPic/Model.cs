using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max)));
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
        static readonly DirectoryInfo favoriteJsonFileDirectory = new DirectoryInfo
            (System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\HiPic");
        static readonly FileInfo favoriteJsonFile = new FileInfo
            (favoriteJsonFileDirectory.FullName + "\\Favorites.json");

        public static void SerializeJson(this FavoritesViewModel vm)
        {
            string jsonString = JsonConvert.SerializeObject(vm);
            favoriteJsonFileDirectory.Create();
            using (StreamWriter file = favoriteJsonFile.CreateText())
            {
                file.Write(jsonString);
            }
        }

        public static FavoritesViewModel DeserializeJson()
        {
            if (favoriteJsonFile.Exists)
            {
                string jsonString;
                using (StreamReader reader = favoriteJsonFile.OpenText())
                {
                    jsonString = reader.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<FavoritesViewModel>(jsonString);
            }
            else
            {
                return new FavoritesViewModel();
            }
        }
    }
}
