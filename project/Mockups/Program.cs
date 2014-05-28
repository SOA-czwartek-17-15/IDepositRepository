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

            var sr = new ServiceHost(typeof(ServiceRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/IServiceRepository")});
            var cr = new ServiceHost(typeof(ClientRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/IClientRepository")});
            var ar = new ServiceHost(typeof(AccountRepository), new Uri[] {new Uri("net.tcp://127.0.0.1:11111/IAccountRepository")});

            try
            {
                // setup service endpoints

                sr.AddServiceEndpoint(typeof(IServiceRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IServiceRepository");
                cr.AddServiceEndpoint(typeof(IClientRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IClientRepository");
                ar.AddServiceEndpoint(typeof(IAccountRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IAccountRepository");

                // start service and wait for clients

                sr.Open();
                cr.Open();
                ar.Open();

                /*var cf = new ChannelFactory<IDepositRepository>(new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:22222/IDepositRepository");
                var channel = cf.CreateChannel();
                channel.CreateDeposit();*/

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
