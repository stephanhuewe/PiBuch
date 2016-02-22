using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;

namespace InfoScreen.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string DOWNLOAD_URL = @"http://www.pibuch.de/samples/InfoScreen.xml";

        static readonly InfoScreenVm ViewModel = new InfoScreenVm();
        
        public MainPage()
        {
            InitializeComponent();

            // Download Data
            Task t = new Task(DownloadPageAsync);
            t.RunSynchronously();

            // Clock For Watch
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += TimerTick;
            timer.Start();

            // Clock For Data
            DispatcherTimer dataTimer = new DispatcherTimer();
            dataTimer.Interval = TimeSpan.FromHours(1);
            dataTimer.Tick += DataTimer_Tick;
            dataTimer.Start();

            // Set VM
            DataContext = ViewModel;
        }

        private void DataTimer_Tick(object sender, object e)
        {
            Task t = new Task(DownloadPageAsync);
            t.RunSynchronously();
        }

        private void TimerTick(object sender, object e)
        {
            Clock.Text = DateTime.Now.ToString(new CultureInfo("de-DE"));
        }

        static async void DownloadPageAsync()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(DOWNLOAD_URL))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                ParseXml(result);
            }
        }

        private static void ParseXml(string result)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            XmlNode root = doc.ChildNodes[1];

            foreach (XmlNode node in root.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Person":
                        ViewModel.Person = node.InnerText;
                        break;

                    case "ImageUrl":
                        ViewModel.ImageUrl = node.InnerText;
                        break;

                    case "WelcomeText":
                        ViewModel.WelcomeText = node.InnerText;
                        break;
                } 
            }
        }
    }
}
