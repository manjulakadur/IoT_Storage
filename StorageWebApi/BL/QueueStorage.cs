using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using StorageWebApi.Services;

namespace StorageWebApi.BL
{
    public class QueueStorage : IQueueStorage
    {
        private readonly IConfiguration _configuration;
        public QueueStorage(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

         public async Task<bool> SendMessage(string queueName,string msg)
        {
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            // Create the queue
            await queueClient.CreateIfNotExistsAsync();

            if (await queueClient.ExistsAsync())
            {
                await queueClient.SendMessageAsync(msg);
                return true;
            }
            else
            {
                //Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                return false;
            }
            /*var queueClient = new QueueClient(connectionString, queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
            queueClient.CreateIfNotExists();
            if (queueClient.Exists())
            {
                await queueClient.SendMessageAsync(message);
            }*/
        }

        public async Task<List<string>> ReceiveMessages(string queueName)
        {
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            var queueClient = new QueueClient(connectionString, queueName);
            //QueueMessage msg;
            List<string> msg = new List<string>();
            if (queueClient.Exists())
            {
                QueueMessage[] retrievedMessages = await queueClient.ReceiveMessagesAsync();
                msg.Add(retrievedMessages[0].Body.ToString());
                //foreach (QueueMessage message in retrievedMessages)
                //{
               
                //    msg.Add(message.MessageId);
                //}                
            }
            return msg;

        }

        public async Task UpdateMessage(string queueName)
        {
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            var queueClient = new QueueClient(connectionString, queueName);

            if (queueClient.Exists())
            {
                QueueMessage[] message = queueClient.ReceiveMessages();
                await queueClient.UpdateMessageAsync(message[0].MessageId,
                message[0].PopReceipt,
                "Updated contents",
                TimeSpan.FromSeconds(60.0)  // Make it invisible for another 60 seconds
            );
            }
        }

        public async Task DeleteMessage(string queueName)
        {
            string connectionString = _configuration.GetValue<string>("MyConfig:StorageConnection");
            var queueClient = new QueueClient(connectionString, queueName);

            if (queueClient.Exists())
            {
                QueueMessage[] retrievedMessage = queueClient.ReceiveMessages();
                await queueClient.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);  // Make it invisible for another 60 seconds
            
            }
        }
    }
}
