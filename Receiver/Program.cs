using System;
using System.Threading.Tasks;
using NServiceBus;


class Program
{
    static async Task Main()
    {
        Console.Title = "SimpleReceiver";
        var endpointConfiguration = new EndpointConfiguration("Drk.Samples.Sqs.SimpleReceiver");
        endpointConfiguration.UseSerialization<XmlSerializer>();

        endpointConfiguration.EnableInstallers();
        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.S3("drk.bucketname", "drk");
        transport.QueueNamePrefix("drk");

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}