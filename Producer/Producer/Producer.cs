using Common;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    public class Producer
    {
        const string queueName = "ServiceBusQueue";
        public static void Main(string[] args)
        {
            try
            {
                CreateQueue();
                SendMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message}");
            }
            Console.Read();
        }

        private static void SendMessage()
        {
            try
            {
                Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "finomial", string.Empty);
                TimeSpan timeSpan = new TimeSpan(480, 30, 30);
                TokenScope tokenScope = TokenScope.Namespace;

                TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    "SendAccess",
                    "bFgONs8CcQcV9kt1EApD5Oc691OHHU/dMGvTKa4CLuI=",
                    timeSpan, 
                    tokenScope
                );
                //NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);
                var messagingFactory = MessagingFactory.Create(uri, tokenProvider);
                QueueClient qClient = messagingFactory.CreateQueueClient(queueName);


                for (int i = 0; i < 5; i++)
                {
                    var msg = GetMessage();
                    Console.WriteLine(
                        $"Queued {msg.Value} at {msg.QueuedTime} by {msg.ByPerson}"
                    );
                    var brMsg = new BrokeredMessage(msg);
                    qClient.Send(brMsg);
                    System.Threading.Thread.Sleep(2500);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static QueueMessage GetMessage()
        {
            Random random = new Random();
            var data = new QueueMessage
            {
                ByPerson = "Mainak",
                QueuedTime = DateTime.Now,
                Value = random.Next(1, 100)
            };
            return data;
        }

        private static Uri CreateQueue()
        {
            try
            {
                Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "finomial", string.Empty);
                string name = "RootManageSharedAccessKey";
                string key = "NZWpkyILur8B4z6djHRFcyzqGR6S7q8iVY1jtTNvzk8=";
                TimeSpan timeSpan = new TimeSpan(480, 30, 30);
                TokenScope tokenScope = TokenScope.Namespace;

                TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                    name, key, timeSpan, tokenScope
                );
                NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);

                //var qs = namespaceManager.GetQueues();
                if (!namespaceManager.QueueExists(queueName))
                {
                    QueueDescription queueDesc = new QueueDescription(queueName)
                    {
                        DefaultMessageTimeToLive = TimeSpan.FromDays(3),
                        //AutoDeleteOnIdle = TimeSpan.FromHours(1),
                        MaxSizeInMegabytes = 1024,
                        LockDuration = TimeSpan.FromMinutes(5),
                        EnableDeadLetteringOnMessageExpiration = true,
                        RequiresDuplicateDetection = false
                    };
                    namespaceManager.CreateQueue(queueDesc);
                }

                return uri;
            }
            catch(Exception e)
            {
                throw e;
            }
            
        }

        
    }
}
