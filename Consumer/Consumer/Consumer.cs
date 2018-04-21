using Common;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class Consumer
    {
        public static void Main(string[] args)
        {
            string queueName = ConfigurationManager.AppSettings["QueueName"];
            string serviceNamespace = ConfigurationManager.AppSettings["ServiceNamespace"];

            string policyName = ConfigurationManager.AppSettings["SAS_PolicyName"];
            string key = ConfigurationManager.AppSettings["SAS_Key"];


            // Continuously process messages sent to the "TestQueue" 
            while (true)
            {
                Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, string.Empty);
                TimeSpan timeSpan = new TimeSpan(480, 30, 30);
                TokenScope tokenScope = TokenScope.Namespace;

                TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    policyName,
                    key,
                    timeSpan,
                    tokenScope
                );
                var messagingFactory = MessagingFactory.Create(uri, tokenProvider);
                QueueClient qClient = messagingFactory.CreateQueueClient(queueName, ReceiveMode.PeekLock);
                BrokeredMessage message = qClient.Receive();

                if (message != null)
                {
                    try
                    {
                        QueueMessage msg = message.GetBody<QueueMessage>();
                        Console.WriteLine(
                            $"Received {msg.Value} from {msg.ByPerson} queued at {msg.QueuedTime}"
                        );
                        //Console.WriteLine("MessageID: " + message.MessageId);
                        // Remove message from queue
                        message.Complete();

                        Console.WriteLine();

                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        // Indicate a problem, unlock message in queue
                        message.Abandon();
                    }
                }
            }
        }
    }
}
