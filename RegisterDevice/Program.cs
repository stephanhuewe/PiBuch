using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace RegisterDevice
{
    class Program
    {
        static RegistryManager registryManager;
        static string connectionString = "HostName=BuchRaspberryHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=pRdaxMdjOhizyojQEcByslA9OUbt8F3sykERrbsk1lc=";

        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private static async Task AddDeviceAsync()
        {
            string deviceId = "MeinRaspberry";
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Neuer Geräteschlüssel: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
