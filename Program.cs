using System;
using System.Collections.Generic;
using CvMega.Helper;

namespace CvMega
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CvMega Client ===");
            Console.WriteLine();

            // Example: Set the host for the client
            Client.currentHost = "http://localhost:8080/";
            Console.WriteLine($"Host set to: {Client.currentHost}");
            Console.WriteLine();

            // Example: Send a GET request with parameters
            Console.WriteLine("Sending test request...");
            var parameters = new Dictionary<string, string>
            {
                { "test", "value" }
            };

            byte[] response = Client.SendGet(parameters);
            if (response != null)
            {
                Console.WriteLine($"Response received: {response.Length} bytes");
            }
            else
            {
                Console.WriteLine("No response received.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}