﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MatchingServer
{
    class Server
    {
        private Socket[] connectedClients = null;
		private Socket socketServer = null;
		private AsyncCallback m_fnReceiveHandler;
		private AsyncCallback m_fnSendHandler;
		private AsyncCallback m_fnAcceptHandler;
        
        
		public Server() 
        {
			m_fnReceiveHandler = handleDataReceive;
			m_fnSendHandler = handleDataSend;
			m_fnAcceptHandler = handleClientConnectionRequest;
		}
		
		public void StartServer(UInt16 port) 
        {
            // Creation server socket
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            // Assign address to server socket
            socketServer.Bind(new IPEndPoint(IPAddress.Any, port));

            // Waiting onnection request 
            socketServer.Listen(5);

            // Accept connection
            socketServer.BeginAccept(m_fnAcceptHandler, null);

            Console.WriteLine("Server open");
		}
		
		public void StopServer() 
        {
            socketServer.Close();
		}
		
		private void handleClientConnectionRequest(IAsyncResult ar) 
        {
			
			
		}

		private void handleDataReceive(IAsyncResult ar) 
        {
		
		}

		private void handleDataSend(IAsyncResult ar) 
        {
		
		}


    }
}
