using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

// I denne udgave er klient-håndteringen udskildt i en selvstændig klasse
// Der oprettes et selvstændigt ClientHandler objekt for hver forbindelse
// og der behandlingen startes så op i hver sin tråd
// Hver ClientHandler objekt har een aktiv tråd, sålænge forbindelsen holdes åben

// Udenne udgave er ChatService behandlingen udskildt fra ClientHandleren
// Herved opnås dels en adskildelse af funktionaliteten og grænsefladen (dialogen) til klienten.
// Desuden udskildes en fælles resource (Liste med klienterne), 
// således at ChatService kan fungere som en resource-monitor for en resource delt af flere tråde

namespace ChatServerDesign_04_NoLock
{
    public class Server
    {
        private ChatService chatService = new ChatService("Socket chat-rum");

        public Server(int port)
        {
            System.Console.WriteLine("Server startet på port:" + port);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            while (true)
            {
                System.Console.WriteLine("Server klar");

                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Der er gået en i fælden");

				ClientHandler handler = new ClientHandler(clientSocket, chatService);
								
				// Her sættes op til af behandling foregår parallel
				// så der kan fortsættes med en ny
                Thread clientTråd = new Thread(handler.RunClient);
				clientTråd.Start();
            }
        }
	}
}
