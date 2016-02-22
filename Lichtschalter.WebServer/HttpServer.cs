using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Gpio;

namespace Lichtschalter.WebServer
{
    public sealed class HttpServer : IDisposable
    {
        private const uint BufferSize = 8192;
        private int port = 8000;
        private readonly StreamSocketListener listener;

        private GpioPin _pin;
        private int LED_PIN = 5;

        public HttpServer(int serverPort)
        {
            listener = new StreamSocketListener();
            port = serverPort;
            listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);

        }
        public void StartServer()
        {
            listener.BindServiceNameAsync(port.ToString());
            InitGPIO();
        }

        public void Dispose()
        {
            listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            string OnString = String.Empty;
            string OffString = "checked";

            string state = "Unspecified";

            if (request.Contains("index.html?state=on"))
            {
                OnString = "checked";
                OffString = string.Empty;
                TurnOnOffLight(GpioPinValue.Low);
            }
            else if (request.Contains("index.html?state=off"))
            {
                OnString = String.Empty;
                OffString = "checked";
                TurnOnOffLight(GpioPinValue.High);
            }
                        
            string html = String.Format("<html><head><title>Lichtschalter</title></head><body><h1>Lichtschalter</h2><form action=\"index.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" {0} onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" {1} onclick=\"this.form.submit()\"> Off</form></body></html>", OnString, OffString);

            // Show html 
            using (Stream resp = os.AsStreamForWrite())
            {
                byte[] bodyArray = Encoding.UTF8.GetBytes(html);
                MemoryStream stream = new MemoryStream(bodyArray);
                string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                  "Content-Length: {0}\r\n" +
                                  "Connection: close\r\n\r\n",
                                  stream.Length);
                byte[] headerArray = Encoding.UTF8.GetBytes(header);
                await resp.WriteAsync(headerArray, 0, headerArray.Length);
                await stream.CopyToAsync(resp);
                await resp.FlushAsync();
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
            _pin.Write(GpioPinValue.High);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void TurnOnOffLight(GpioPinValue pinValue)
        {
            var temp = _pin.Read();
            _pin.Write(pinValue);
        }
    }
}
