using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using System.Timers;

using Contracts;
using log4net;

namespace DepositService
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static IServiceRepository svcRepository;
        private static string svcRepositoryAddr;
        private const string selfAddress = "net.tcp://127.0.0.1:22222/IDepositRepository";
        private const string selfAddressForSvcRepository = "net.tcp://0.0.0.0:22222/IDepositRepository";
        private static Timer connectionTimer = null;
        private static Timer aliveTimer = null;
        
        static void Main(string[] args)
        {
            // load log4net configuration

            log4net.Config.XmlConfigurator.Configure();

            log.Info("Starting to log.");

            // get ServiceRepository address from App.config

            svcRepositoryAddr = System.Configuration.ConfigurationManager.AppSettings["svcRepositoryAddress"];

            // create the timer which will remind service repository that we're alive every 4 secs

            aliveTimer = new Timer();
            aliveTimer.Interval = (1000) * (4);
            aliveTimer.Elapsed += new ElapsedEventHandler(KeepAlive);

            // create service host

            var dr = new DepositRepository(svcRepositoryAddr, new NHibernateAccess());
            var sh = new ServiceHost(dr, new Uri[] { new Uri(selfAddress) });

            try
            {
                // setup service endpoint

                sh.AddServiceEndpoint(typeof(IDepositRepository), new NetTcpBinding(SecurityMode.None), selfAddress);

                // start service and wait for clients

                sh.Open();

                log.Info("Service is up.");
             
                try
                {
                    log.Info("Trying to connect to IServiceRepository");

                    // create channel to service repository

                    var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), svcRepositoryAddr);
                    svcRepository = cf.CreateChannel();

                    // register our service

                    svcRepository.RegisterService("IDepositRepository", selfAddressForSvcRepository);

                    log.Info("Service registered.");

                    // start notifying about being alive
                    
                    aliveTimer.Start();
                }
                catch (CommunicationException commError)
                { 
                    // create the timer which will try to connect to service repository every 4 secs

                    connectionTimer = new Timer();
                    connectionTimer.Interval = (1000) * (4);
                    connectionTimer.Elapsed += new ElapsedEventHandler(TryToConnect);
                    connectionTimer.Start();

                    log.Info("Error while connecting to ServiceRepository. Message: " + commError.Message);
                }

                // press ENTER to finish

                Console.ReadLine();

                // unregister service before closing

                svcRepository.Unregister("IDepositRepository");

                log.Info("Service unregistered.");

                // stop the timer and close our service

                aliveTimer.Stop();
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

            try
            {
                // create channel to service repository

                var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), svcRepositoryAddr);
                svcRepository = cf.CreateChannel();

                // register our service

                svcRepository.RegisterService("IDepositRepository", selfAddressForSvcRepository);

                log.Info("Service registered.");

                connectionTimer.Stop();

                // start notifying about being alive

                aliveTimer.Start();
            }
            catch (CommunicationException commError)
            {
                log.Info("Error while connecting to ServiceRepository. Message: " + commError.Message);
                connectionTimer.Start();
            }
        }
        
        private static void KeepAlive(object sender, EventArgs e)
        {
            log.Info("Sending alive");

            // creating new channel each time
            
            try
            {
                var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), svcRepositoryAddr);
                var channel = cf.CreateChannel();
                channel.Alive("IDepositRepository");
                cf.Abort();

                log.Info("Alive sent");
            }
            catch (CommunicationException commError)
            {
                log.Info("Error while sending alive. Message: " + commError.Message);
                aliveTimer.Stop();
                connectionTimer.Start();
            }
        }
    }
}
