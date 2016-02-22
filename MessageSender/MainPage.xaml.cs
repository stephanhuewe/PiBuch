using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Text;
using Microsoft.Azure.Devices.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MessageSender
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Timer for temperature
            DispatcherTimer tempTimer = new DispatcherTimer();
            tempTimer.Interval = TimeSpan.FromMilliseconds(5000);   
            tempTimer.Tick += TempTimer_Tick;
            tempTimer.Start();
        }

        private void TempTimer_Tick(object sender, object e)
        {
            SendDeviceToCloudMessagesAsync();
        }

        static async void SendDeviceToCloudMessagesAsync()
        {
            string iotHubUri = "BuchRaspberryHub.azure-devices.net";
            string deviceId = "MeinRaspberry";
            string deviceKey = "<Key>";

            var deviceClient = DeviceClient.Create(iotHubUri, AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Http1);

            string str = String.Format("Temperatur um {0} : {1} Grad Celsius", DateTime.Now, GetTemperature());
            Message message = new Message(Encoding.ASCII.GetBytes(str));
            await deviceClient.SendEventAsync(message);

        }

        private static double GetTemperature()
        {
            double min = 17.0;
            double max = 23.0;
            Random random = new Random();
            return Math.Round(random.NextDouble() * (max - min) + min, 2);
        }
    }
}
