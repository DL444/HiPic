using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPic
{
    static class Rand
    {
        public static Random rand = new Random(DateTime.Now.Millisecond);
    }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keyword"));
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
}
