using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Messdatenerfassung_Temperatur
{
    public sealed partial class MainPage : Page
    {
        #region Inits

        private const string SPI_CONTROLLER_NAME = "SPI0";  // Use SPI0
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       // Pin 24 (GPIO 8)

        byte[] readBuffer = new byte[3];
        byte[] writeBuffer = new byte[3] { 0x68, 0x00, 0x00 }; // Initialize 00001101 00000000, Data-Sheet FIGURE 6-1, Channel 0

        private SpiDevice SpiMCP;

        private DispatcherTimer tempTimer;
        int res;
        #endregion


        public MainPage()
        {
            InitializeComponent();

            InitSPI();

            // Timer for temp
            tempTimer = new DispatcherTimer();
            tempTimer.Interval = TimeSpan.FromMilliseconds(500);
            tempTimer.Tick += TempTimer_Tick;
            tempTimer.Start();
        }

        private void TempTimer_Tick(object sender, object e)
        {
            DisplayTextBoxContents();
        }

        private async void InitSPI()
        {
            try
            {
                SpiConnectionSettings settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = 500000; // 0,5 MHz
                settings.Mode = SpiMode.Mode0;

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
                DeviceInformationCollection deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
                SpiMCP = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
            }

            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        public void DisplayTextBoxContents()
        {
            SpiMCP.TransferFullDuplex(writeBuffer, readBuffer);
            res = ConvertToInt(readBuffer);

            double voltage;
            voltage = res * (5000.0 / 1024);                        // mV / 5V
            voltage = ((voltage - 750) / 10.0) + 25;                // Reference temperature: 750 mV = 25 °C                                                                    

            Temperatur.Text = String.Format("Aktuelle Temperatur: {0} °C", Math.Round(voltage, 2).ToString());

        }

        public int ConvertToInt(byte[] data)
        {
            // Read 24-Bit Package (3*8 Bit)
            // We just need the last two bytes
            // Output from sensor is 10 Bit            
            int result = data[0] & 0x03;
            result <<= 8;
            result += data[1];
            return result;
        }
    }
}
