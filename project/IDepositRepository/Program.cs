using Contracts;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Timers;
using ZMQ;

using Newtonsoft.Json;

namespace DepositService
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static String svcRepositoryAddr;
        private static Timer connectionTimer = null;
        private static Timer aliveTimer = null;
        
        static void Main(string[] args)
        { 
            // load log4net configuration

            log4net.Config.XmlConfigurator.Configure();

            log.Info("Starting to log.");

            // get ServiceRepository address from App.config

            svcRepositoryAddr = ConfigurationManager.AppSettings["svcRepositoryAddress"];

            // create the timer which will remind service repository that we're alive every 4 secs

            aliveTimer = new Timer();
            aliveTimer.Interval = (1000) * (2);
            aliveTimer.Elapsed += new ElapsedEventHandler(KeepAlive);

            // create the timer which will try to connect to service repository every 4 secs if something goes wrong

            connectionTimer = new Timer();
            connectionTimer.Interval = (1000) * (4);
            connectionTimer.Elapsed += new ElapsedEventHandler(TryToConnect);
                    
            // create service host

            var dr = new DepositRepository(svcRepositoryAddr, new MockAccess());
            var sh = new ServiceHost(dr, new Uri[] { new Uri(ConfigurationManager.AppSettings["selfAddress"]) });

            try
            {
                // setup service endpoint

                sh.AddServiceEndpoint(typeof(IDepositRepository), new NetTcpBinding(SecurityMode.None), ConfigurationManager.AppSettings["selfAddress"]);

                // start web service and wait for clients

                sh.Open();

                log.Info("Service is up.");

                // setup message entry point

                var drZMQ = new DepositRepositoryZMQ(dr, ConfigurationManager.AppSettings["selfAddressZMQ"]);
                             
                log.Info("Sending a message to IServiceRepository");
                    
                // register service by message

                using (var context = new Context())
                {
                    using (var client = context.Socket(SocketType.REQ))
                    {
                        client.Connect(ConfigurationManager.AppSettings["svcRepositoryAddressZMQ"]);

                        var jsonMessage = new JSONMessage();
                        jsonMessage.Function = "RegisterService";
                        jsonMessage.Parameters = new String[] { "IDepositRepository", ConfigurationManager.AppSettings["selfAddressForSvcRepositoryZMQ"], "NetTcpBinding" };
                        jsonMessage.Service = "IDepositRepository";

                        var request = JsonConvert.SerializeObject(jsonMessage);

                        var status = client.Send(request, Encoding.Unicode);

                        string reply = client.Recv(Encoding.Unicode, 4000);

                        if (reply == null)
                        {
                            connectionTimer.Start();
                        }
                    }
                }

                log.Info("Service registered.");

                // start notifying about being alive
                    
                aliveTimer.Start();

                /*Deposit dep = new Deposit();
                dep.Type = "standard";
                dep.Interest = 3.0;
                dep.CapitalizationPeriod = 30;
                dep.Money = 2000.0;
                dep.OpeningDate = "05-07-2014";
                dep.Duration = "4 months";

                Guid newDepositID = dr.CreateDeposit(new Guid("566d9b1e-d322-47f2-a8ad-26553af82385"), dep);*/

                // press ENTER to finish

                Console.ReadLine();

                // unregister service before closing

                var cf_unreg = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), svcRepositoryAddr);
                var svcRepository_unreg = cf_unreg.CreateChannel();
                svcRepository_unreg.Unregister("IDepositRepository");
                cf_unreg.Abort();

                log.Info("Service unregistered.");

                // stop the timer and close our service

                aliveTimer.Stop();
                connectionTimer.Stop();
                sh.Close();

                log.Info("Service finished.");
            }
            catch (CommunicationException commError)
            {
                log.Info("Communication error: " + commError.Message);
                sh.Abort();
                Console.ReadLine();
            }
        }

        private static void TryToConnect(object sender, EventArgs e)
        {
            log.Info("Trying to connect to IServiceRepository");

            connectionTimer.Stop();

            using (var context = new Context())
            {
                using (var client = context.Socket(SocketType.REQ))
                {
                    client.Connect(ConfigurationManager.AppSettings["svcRepositoryAddressZMQ"]);

                    var jsonMessage = new JSONMessage();
                    jsonMessage.Function = "RegisterService";
                    jsonMessage.Parameters = new String[] { "IDepositRepository", ConfigurationManager.AppSettings["selfAddressForSvcRepositoryZMQ"], "NetTcpBinding" };
                    jsonMessage.Service = "IDepositRepository";

                    var request = JsonConvert.SerializeObject(jsonMessage);

                    var status = client.Send(request, Encoding.Unicode);

                    string reply = client.Recv(Encoding.Unicode, 4000);

                    if (reply == null)
                    {
                        connectionTimer.Start();
                    }
                    else
                    {
                        aliveTimer.Start();
                    }
                }
            }
        }
        
        private static void KeepAlive(object sender, EventArgs e)
        {
            log.Info("Sending alive");

            // creating new channel each time
            
            using (var context = new Context())
                using (var client = context.Socket(SocketType.REQ))
                {
                    client.Connect(ConfigurationManager.AppSettings["svcRepositoryAddressZMQ"]);

                    var jsonMessage = new JSONMessage();
                    jsonMessage.Function = "Alive";
                    jsonMessage.Parameters = new String[] { "IDepositRepository" };
                    jsonMessage.Service = "IDepositRepository";

                    var request = JsonConvert.SerializeObject(jsonMessage);

                    client.Send(request, Encoding.Unicode);

                    string reply = client.Recv(Encoding.Unicode, 4000);

                    if(reply == null)
                    {
                        aliveTimer.Stop();
                        connectionTimer.Start();
                    }
                }
        }
    }
}
