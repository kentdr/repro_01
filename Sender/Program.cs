﻿using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "SimpleSender";

        #region ConfigureEndpoint

        var endpointConfiguration = new EndpointConfiguration("Drk.Samples.Sqs.SimpleSender");
        endpointConfiguration.UseSerialization<XmlSerializer>();

        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.S3("drk.bucketname", "drk");
        transport.QueueNamePrefix("drk");

        #endregion

        transport.Routing().RouteToEndpoint(typeof(MyCommand), "Drk.Samples.Sqs.SimpleReceiver");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        await SendMessages(endpointInstance);

        await endpointInstance.Stop();
    }

    static async Task SendMessages(IMessageSession messageSession)
    {
        Console.WriteLine(@"Press
[1] to send a command
[2] to send a command with a large body
[3] to publish an event
[4] to publish an event with a large body
[Esc] to exit.");

        while (true)
        {
            var input = Console.ReadKey();
            Console.WriteLine();

            switch (input.Key)
            {
                case ConsoleKey.D1:
                    await messageSession.Send(new MyCommand());
                    break;
                case ConsoleKey.D2:
                    await messageSession.Send(new MyCommand
                    {
                        Data = new byte[257 * 1024]
                    });
                    break;
                case ConsoleKey.D3:
                    await messageSession.Publish(new MyEvent());
                    break;
                case ConsoleKey.D4:
                    await messageSession.Publish(new MyEvent()
                    {
                        Data = new byte[257 * 1024]
                    });
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }
}