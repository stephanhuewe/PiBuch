using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Blinklicht
{
    public sealed partial class MainPage : Page
    {
        private GpioPin _pin;
        private GpioPinValue _pinValue;
        private int LED_PIN = 5;
        DispatcherTimer _timer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            InitGPIO();
        }

        private void Timer_Tick(object sender, object e)
        {
            var temp = _pin.Read();
            if (temp == GpioPinValue.High)
            {
                _pin.Write(GpioPinValue.Low);
            }
            else
            {
                _pin.Write(GpioPinValue.High);
            }
        }

        private void InitGPIO()
        {
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                _pin = null;
                return;
            }
            _pin = gpio.OpenPin(LED_PIN);
            _pinValue = GpioPinValue.High;
            _pin.Write(_pinValue);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (_pin != null)
            {
                _timer.Start();
            }
        }

    }
}
