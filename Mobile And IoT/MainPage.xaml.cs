using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Mobile_And_IoT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int MAX_COUNT = 10;
        private int count;
        public MainPage()
        {
            this.InitializeComponent();
            SayWhoIAm();

            // Timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick; ;
            timer.Start();

            // Initial Picture
            TakePhoto();
        }

        private void SayWhoIAm()
        {
            string deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

            WhoAmI.Text = String.Format("Ich bin vom Typ {0} :-) ", deviceFamily);

            switch (deviceFamily)
            {
                case "Windows.IoT":
                    break;
                case "Windows.Desktop":
                    break;
                case "Windows.Mobile":
                    break;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            CountDown.Text = String.Format("Nächstes Bild in {0} Sekunden", MAX_COUNT - count);
            count++;
            if (count == MAX_COUNT)
            {
                TakePhoto();
                count = 0;
            }
        }

        private async void TakePhoto()
        {
            DeviceInformation cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Front);

            if (cameraDevice == null)
            {
                ErrorText.Text = "No camera";
                return;
            }

            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

            MediaCapture mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(settings);

            StorageFile photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync("MyPhoto", CreationCollisionOption.GenerateUniqueName);
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

            await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

            IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(photoStream);
            ImageControl.Source = bitmap;
        }

        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

    }
}
