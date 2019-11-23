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

        static void Main(string[] args)
        {

            string username = "Servidor";
            MessageQueue appQueue;

            if (MessageQueue.Exists(@".\Private$\MSMQ_queue"))
            {
                appQueue = new MessageQueue(@".\Private$\MSMQ_queue");
            }
            else
            {
                appQueue = MessageQueue.Create(@".\Private$\MSMQ_queue");
            }


            //appQueue.
            appQueue.Purge();
            Console.WriteLine("Chat inicializado");
            string input = "";

            /*Action<object> receiveMsgsAction = (object obj) =>
            {
                while (input != "parar")
                {
                    Console.Clear();
                    Console.WriteLine(Receive(appQueue));
                }
            };*/

            Action<object> receiveMsgsAction = (object obj) =>
            {

                int msgs_amount = 0;

                while (input != "sair")
                {
                    Message[] msgs = appQueue.GetAllMessages();

                    if (msgs_amount != msgs.Length)
                    {
                        msgs_amount = msgs.Length;
                        StringBuilder builder = new StringBuilder();
                        bool first = true;

                        foreach (Message msg in msgs)
                        {
                            if (!first)
                            {
                                builder.Append("\n");
                            }
                            else
                            {
                                first = false;
                            }
                            msg.Formatter = new BinaryMessageFormatter();
                            builder.Append(msg.Body.ToString());
                        }

                        Console.Clear();
                        Console.WriteLine("Chat inicializado");
                        Console.WriteLine(builder.ToString());
                    }

                }
            };

            Task receiveMsgs = Task.Factory.StartNew(receiveMsgsAction, "receiver");
            /*
            input = Console.ReadLine();
            while (!input.Equals("parar"))
            {

                Send(input, appQueue, username);

                input = Console.ReadLine();
            }
            */
            Console.ReadKey();
        }

        static string Receive(MessageQueue appQueue)
        {
            Message[] msgs = appQueue.GetAllMessages();
            StringBuilder builder = new StringBuilder();
            bool first = true;

            foreach (Message msg in msgs)
            {
                if (!first)
                {
                    builder.Append("\n");
                }
                else
                {
                    first = false;
                }
                msg.Formatter = new BinaryMessageFormatter();
                builder.Append(msg.Body.ToString());
            }

            return builder.ToString();
        }
        static void Send(string MessageToBeSent, MessageQueue appQueue, string username)
        {
            Message msg = new System.Messaging.Message();

            msg.Formatter = new BinaryMessageFormatter();
            msg.Body = MessageToBeSent;
            msg.Label = username;

            if (MessageToBeSent.StartsWith("HP:"))
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
