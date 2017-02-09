using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

// I denne udgave er klient-h�ndteringen udskildt i en selvst�ndig klasse
// Der oprettes et selvst�ndigt ClientHandler objekt for hver forbindelse
// og der behandlingen startes s� op i hver sin tr�d
// Hver ClientHandler objekt har een aktiv tr�d, s�l�nge forbindelsen holdes �ben

// Udenne udgave er chat-service behandlingen udskildt fra ClientHandleren
// Herved opn�s dels en adskildelse af funktionaliteten og gr�nsefladen (dialogen) til klienten.
// Desuden udskildes en f�lles resource (Liste med klienternes streamwritere). 
// I denne udgave er det blevet server-klassen der fungerer som resouce-monitor for
// den f�lles resource.
// Dette passer ogs� med at det er server-klassen, der er creator og eneste mulige
// overlevende.

namespace ChatServerDesign_03_NoLock
{
    public class Server
    {
        public Server(int port)
        {
            System.Console.WriteLine("Server startet p� port:" + port);

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            while (true)
            {
                System.Console.WriteLine("Server klar");

                Socket clientSocket = listener.AcceptSocket();

                System.Console.WriteLine("Der er g�et en i f�lden");

				ClientHandler handler = new ClientHandler(clientSocket, this);
								
				// Her s�ttes op til af behandling foreg�r parallel
				// s� der kan forts�ttes med en ny
                Thread clientTr�d = new Thread(handler.RunClient);
				clientTr�d.Start();
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
