using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{ 
    class Client
    {

        static MessageQueue MyQueue;
        static MessageQueue ServerQueue;
        static string localIP;

        static void Main(string[] args)
        {

            Console.WriteLine("Digite o ip do servidor:");
            string serverIP = Console.ReadLine();
            Console.WriteLine("Trying to connect to " + serverIP);

            ServerQueue = new MessageQueue(String.Format(@"FormatName:DIRECT=TCP:{0}\Private$\MSMQ_queue", serverIP)); // 192.168.7.1  192.168.0.10

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            Send(String.Format("Add:{0}",localIP));

            if (MessageQueue.Exists(@".\Private$\MSMQ_queue"))
            {
                MyQueue = new MessageQueue(@".\Private$\MSMQ_queue");
            }
            else
            {
                MyQueue = MessageQueue.Create(@".\Private$\MSMQ_queue");
            }

            Console.WriteLine("Digite o seu nome de usuário:");
            string username = Console.ReadLine();

            string input = "";

            Action<object> receiveMsgsAction = (object obj) =>
            {
                while (input != "sair")
                {

                    Message msg = MyQueue.Receive();
                    msg.Formatter = new BinaryMessageFormatter();
                    Console.WriteLine(msg.Body.ToString());

                }
            };

            Task receiveMsgs = Task.Factory.StartNew(receiveMsgsAction,"receiver");

            Console.WriteLine("Você entrou na conversa");
            while (!input.Equals("sair"))
            {

                Send(String.Format("{0}:{1}", username, input));

                input = Console.ReadLine();
            }

            Send(String.Format("Remove:{0}", localIP));

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
        static void Send(string MessageToBeSent)
        {
            Message msg = new System.Messaging.Message();

            msg.Formatter = new BinaryMessageFormatter();
            msg.Body = MessageToBeSent;
            msg.Label = localIP;

            if (MessageToBeSent.StartsWith("(HP)"))
            {
                msg.Priority = MessagePriority.Highest;
            }
            else
            {
                msg.Priority = MessagePriority.Normal;
            }

            ServerQueue.Send(msg);
        }

    }


}
