using Microsoft.ServiceBus.Messaging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "HostName=BuchRaspberryHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=<Key>";
            string iotHubD2cEndpoint = "messages/events";

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            string[] d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                EventHubReceiver receiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
            Console.ReadLine();
        }

        async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Daten empfangen: '{0}'", data);
            }
        }
    }
}
