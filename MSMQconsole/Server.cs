using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MSMQconsole
{
    class Server
    {

        private static Dictionary<string,Client_Queue> Client_Queues;
        private static MessageQueue ServerQueue;
        private static ICollection<Message> AllMsgs;

        static void Main(string[] args)
        {

            //string username = "Servidor";

            if (MessageQueue.Exists(@".\Private$\MSMQ_queue"))
            {
                ServerQueue = new MessageQueue(@".\Private$\MSMQ_queue");
            }
            else
            {
                ServerQueue = MessageQueue.Create(@".\Private$\MSMQ_queue");
            }

            Client_Queues = new Dictionary<string, Client_Queue>();
            AllMsgs = new LinkedList<Message>();
            ServerQueue.Purge();

            Console.WriteLine("Chat inicializado");
            string input = "";

            Action<object> receiveMsgsAction = (object obj) =>
            {

                while (input != "parar")
                {

                    Message msg = ServerQueue.Receive();
                    msg.Formatter = new BinaryMessageFormatter();
                    if (msg.Body.ToString().StartsWith("Add:"))
                    {
                        string new_ip = msg.Body.ToString().Substring(4);
                        Client_Queues[new_ip] = new Client_Queue(new_ip);
                    }
                    else
                    {
                        if (msg.Body.ToString().StartsWith("Remove:"))
                        {
                            string remove_ip = msg.Body.ToString().Substring(4);
                            Client_Queues.Remove(remove_ip);
                        }
                        else
                        {
                            Console.WriteLine(msg.Body.ToString());
                            AllMsgs.Add(msg);
                            foreach (Client_Queue client in Client_Queues.Values)
                            {
                                client.sendMsg(msg);
                            }
                        }
                    }


                }
            };

            Task receiveMsgs = Task.Factory.StartNew(receiveMsgsAction, "receiver");

            while (!input.Equals("parar"))
            {
                input = Console.ReadLine();
            }
            
            Console.ReadKey();
        }

        static void Send(string MessageToBeSent, MessageQueue appQueue, string username)
        {
            Message msg = new System.Messaging.Message();

            msg.Formatter = new BinaryMessageFormatter();
            msg.Body = MessageToBeSent;
            msg.Label = username;

            if (MessageToBeSent.StartsWith("(HP)"))
            {
                msg.Priority = MessagePriority.Highest;
            }
            else
            {
                msg.Priority = MessagePriority.Normal;
            }

            appQueue.Send(msg);
        }

    }
}
