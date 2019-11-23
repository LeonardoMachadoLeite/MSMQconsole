using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{ 
    class Client
    {

        static void Main(string[] args)
        {

            string username = "Client";
            //Thread.Sleep(2000);
            string server_ip = "192.168.15.1";
            Console.WriteLine("Trying to connect to " + server_ip);
            MessageQueue appQueue = new MessageQueue(@"FormatName:DIRECT=TCP:192.168.15.1\Private$\MSMQ_queue"); // 192.168.7.1  192.168.0.10
            string input = "";

            Action<object> receiveMsgsAction = (object obj) =>
            {

                Console.WriteLine("Thread start");

                Message msg = appQueue.Peek();
                msg.Formatter = new BinaryMessageFormatter();
                Console.WriteLine(msg.Body.ToString());


            };

            Task receiveMsgs = Task.Factory.StartNew(receiveMsgsAction,"receiver");

            Console.WriteLine("Você entrou na conversa");
            input = Console.ReadLine();
            while (!input.Equals("sair"))
            {

                Send(input,appQueue,username);

                input = Console.ReadLine();
            }

            Console.WriteLine("Conversa encerrada");
            Console.ReadKey();
        }



        /*
        static string Receive(MessageQueue appQueue)
        {
            Message[] msgs = appQueue.GetAllMessages();
            bool first = true;
            StringBuilder aux = new StringBuilder();

            foreach (Message msg in msgs) {
                msg.Formatter = new BinaryMessageFormatter();
                if (!first)
                {
                    aux.Append("\n");
                }
                else
                {
                    first = false;
                }
                aux.Append(msg.Body.ToString());
            }

            return aux.ToString();
        }
        */
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
