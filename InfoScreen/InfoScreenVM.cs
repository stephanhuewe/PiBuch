using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;

namespace InfoScreen.Client
{
    public class InfoScreenVm : INotifyPropertyChanged
    {
        private string _person;
        private string _image;
        private string _welcomeText;
        private BitmapImage _bitmapImage;

        public string WelcomeText
        {
            get { return _welcomeText; }
            set
            {
                _welcomeText = value;
                OnPropertyChanged("WelcomeText");
            }
        }

        public string Person
        {
            get { return _person; }
            set
            {
                _person = value;
                OnPropertyChanged("Person");
            }
        }

        public string ImageUrl
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("BitmapImage");
            }
        }

        public BitmapImage BitmapImage
        {
            get {
                BitmapImage bitmap = new BitmapImage();

                if (!String.IsNullOrWhiteSpace(ImageUrl))
                {
                    bitmap.UriSource = new Uri(ImageUrl, UriKind.Absolute);
                }
                return bitmap;
            }
            set
            {
                _bitmapImage = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
