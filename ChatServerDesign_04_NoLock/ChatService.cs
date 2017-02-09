using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Her er ChatService behandlingen udskildt fra ClientHandleren
// Herved opn�s dels en adskildelse af funktionaliteten og gr�nsefladen (dialogen) til klienten.
// Desuden udskildes en f�lles resource, s�ledes at ChatService kan fungere som en resource-monitor
// for en resource delt af flere tr�de

// I denne udgave erstattes klienternes StreamWritere med ClientHandler'ne, s�ledes at de selv f�r besked
// n�r en besked skal broardcastes, hermed kan de ogs� selv tage ansvaret for skrivningen og evt l�se i den forbindelse

// L�sningen her bygger p� observer-m�nsteret, hvor der abonneres p� en h�ndelse / forandring i et objekt forekommer.

// Der er ikke tr�dsikring med lock / Monitor i denne udgave

namespace ChatServerDesign_04_NoLock
{
    public class ChatService
    {
        private List<ClientHandler> broardcastClients = new List<ClientHandler>();
        private string name;

        public ChatService(string name)
        {
            this.name = name;
        }
        public string Name
        {
            get { return this.name; }
        }

        public void TilmeldBroardcasting (ClientHandler client)
        {
            broardcastClients.Add(client);
        }
        public void AfmeldBroardcasting(ClientHandler client)
        {
            broardcastClients.Remove(client);
        }

        public void BroadCastBesked (string msg)
        {
            foreach (ClientHandler client in broardcastClients)
            {
                client.BroadcastAction(msg);
            }
        }
    }
}
