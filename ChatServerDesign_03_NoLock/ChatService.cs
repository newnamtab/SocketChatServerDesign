using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// Her er ChatService behandlingen udskildt fra ClientHandleren
// Herved opnås dels en adskildelse af funktionaliteten og grænsefladen (dialogen) til klienten.
// Desuden udskildes en fælles resource, således at ChatService kan fungere som en resource-monitor
// for en resource delt af flere tråde

// I denne udgave er det stadig bare en samling af klienternes StreamWriter der bruges som fælles resource
// Der er ikke trådsikring med lock / Monitor i denne udgave

namespace ChatServerDesign_03_NoLock
{
    public class ChatService
    {
        private List<StreamWriter> clientWriters = new List<StreamWriter>();
        private string name;

        public ChatService(string name)
        {
            this.name = name;
        }
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
