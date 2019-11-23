using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MSMQconsole
{
    class Client_Queue
    {

        public string Ip { get; set; }
        private MessageQueue Queue;

        public Client_Queue(string ip)
        {
            this.Ip = ip;
            Console.WriteLine(String.Format(@"FormatName:DIRECT=TCP:{0}\Private$\MSMQ_queue", ip));
            Queue = new MessageQueue(String.Format(@"FormatName:DIRECT=TCP:{0}\Private$\MSMQ_queue",ip));
        }

        public void sendMsg(Message msg)
        {
            this.Queue.Send(msg);
        }

    }

}
