using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace LR8_Server
{
    public class ServerObject
    {
        static public TcpListener tcpListener; // сервер для прослушивания 
        List<ClientObject> clients = new List<ClientObject>();
        ClientObject clientObject = null;
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void Listen()
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
            tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                clientObject = new ClientObject(tcpClient, this);
                Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                clientThread.Start();
            }
        }
    }

}
