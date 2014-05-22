using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace IDepositRepository
{
    class ServiceDiscoverer
    {
        private IServiceRepository serviceRepository = null;
        private IClientRepository clientRepository = null;
        private IAccountRepository accountRepository = null;

        private Stopwatch clientRepositoryTimer;
        private Stopwatch accountRepositoryTimer;

        public ServiceDiscoverer(IServiceRepository ServiceRepository)
        {
            serviceRepository = ServiceRepository;
        }

        public IClientRepository GetClientRepository()
        {
            if (clientRepository != null)
            {
                return serviceRepository;


                var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(), svcRepositoryAddr);
                svcRepository = cf.CreateChannel();
                NetTcpBinding binding = new NetTcpBinding();
                ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(binding, new EndpointAddress(serviceRepositoryAddress));
                return cf.CreateChannel();
            }
        }

        public IAccountRepository GetAccountRepository()
        {
            if (accountRepository != null)
                return accountRepository;

            string address = GetServiceRepository().GetServiceAddress("IAccountRepository");
            NetTcpBinding binding = new NetTcpBinding();
            ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(binding, new EndpointAddress(address));
            return cf.CreateChannel();
        }
    }
}
