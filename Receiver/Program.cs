using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NServiceBus;


class Program
{
    static async Task Main()
    {
        Console.Title = "SimpleReceiver";
        var endpointConfiguration = new EndpointConfiguration("Drk.Samples.Sqs.SimpleReceiver");
        endpointConfiguration.EnableInstallers();
        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.S3("drk.bucketname", "drk");
        transport.QueueNamePrefix("drk");

        var sub = transport.EnableMessageDrivenPubSubCompatibilityMode();
        sub.RegisterPublisher(typeof(MyEvent), "Drk.Samples.Sqs.SimpleReceiver");

        var p = endpointConfiguration.UsePersistence<SqlPersistence>();
        p.ConnectionBuilder(() => new SqlConnection(@"Server=localhost,1433;Initial Catalog=NsbSamplesSqlPersistence;User Id=SA;Password=yourStrong(!)Password;Encrypt=false"));
        p.SqlDialect<SqlDialect.MsSqlServer>();
        p.SqlDialect<SqlDialect.MsSqlServer>();
        var subscriptions = p.SubscriptionSettings();
        subscriptions.CacheFor(TimeSpan.FromMinutes(5));

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}