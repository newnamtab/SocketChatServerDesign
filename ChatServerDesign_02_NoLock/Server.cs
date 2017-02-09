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

// Udenne udgave er chat-service behandlingen udskildt fra ClientHandleren
// Herved opnås dels en adskildelse af funktionaliteten og grænsefladen (dialogen) til klienten.
// Desuden udskildes en fælles resource (Liste med klienternes streamwritere). 
// I denne udgave er det blevet server-klassen der fungerer som resouce-monitor for
// den fælles resource.
// Dette passer også med at det er server-klassen, der er creator og eneste mulige
// overlevende.

namespace ChatServerDesign_03_NoLock
{
    public class Server
    {
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

				ClientHandler handler = new ClientHandler(clientSocket, this);
								
				// Her sættes op til af behandling foregår parallel
				// så der kan fortsættes med en ny
                Thread clientTråd = new Thread(handler.RunClient);
				clientTråd.Start();
            }
        }

        private List<StreamWriter> clientWriters = new List<StreamWriter>();
        private string name = "Socket chatrum";

        public string Name
        {
            get { return this.name; }
        }

        public void TilmeldBroardcasting (StreamWriter writer)
        {
            clientWriters.Add(writer);
        }
        public void AfmeldBroardcasting(StreamWriter writer)
        {
            clientWriters.Remove(writer);
        }

        public void BroadCastBesked (string msg)
        {
            foreach (StreamWriter writer in clientWriters)
            {
                writer.WriteLine("Broadcast:" + msg );
                writer.Flush();
            }
        }
    }
}
