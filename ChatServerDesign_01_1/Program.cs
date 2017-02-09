using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServerDesign_01_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Indtast Port - telnet er 23");
            string strPort = Console.ReadLine();
            if (strPort.Trim() == "") strPort = "12000";
            int port = int.Parse(strPort);
            new Server(port);
        }
    }
}
