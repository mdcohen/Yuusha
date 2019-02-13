using System;

namespace Yuusha
{
    static class Program
    {
        public static Client Client;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Client client = new Client())
            {
                Client = client;
                client.Run();
            }
        }
    }
}

