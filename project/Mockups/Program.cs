using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    class Program
    {
        static void Main(string[] args)
        {
            // create mockup services

            var sr = new ServiceHost(typeof(ServiceRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/ServiceRepository")});
            var cr = new ServiceHost(typeof(ClientRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/ClientRepository")});
            var ar = new ServiceHost(typeof(AccountRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/AccountRepository")});

            try
            {
                // setup service endpoints

                sr.AddServiceEndpoint(typeof(IServiceRepository), new NetTcpBinding(), "net.tcp://127.0.0.1:11111/ServiceRepository");
                cr.AddServiceEndpoint(typeof(IClientRepository), new NetTcpBinding(), "net.tcp://127.0.0.1:11111/ClientRepository");
                ar.AddServiceEndpoint(typeof(IAccountRepository), new NetTcpBinding(), "net.tcp://127.0.0.1:11111/AccountRepository");

                // start service and wait for clients

                sr.Open();
                cr.Open();
                ar.Open();

                Console.ReadLine();
            }
            catch (CommunicationException commError)
            {
                Console.WriteLine("Communication error: " + commError.Message);
                sr.Abort();
                cr.Abort();
                ar.Abort();
                Console.Read();
            }
        }
    }
}
