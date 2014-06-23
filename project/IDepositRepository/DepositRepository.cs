using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;
using log4net;

namespace DepositService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DepositRepository : IDepositRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        string _serviceRepositoryAddress = null;
        IDatabaseAccess _databaseAccess = null;

        public DepositRepository(string serviceRepositoryAddress, IDatabaseAccess databaseAccess)
        {
            _serviceRepositoryAddress = serviceRepositoryAddress;
            _databaseAccess = databaseAccess;
        }

        public Guid CreateDepositForClient(Guid clientId, Deposit deposit)
        {
            _log.Info("New deposit requested for client " + clientId + ".");

            deposit.Id = Guid.NewGuid();
            deposit.ClientId = clientId;

            var srCf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), _serviceRepositoryAddress);
            var serviceRepository = srCf.CreateChannel();
            
            List<ServiceAB> locations = serviceRepository.GetServiceLocations("IClientRepository");

            if (locations == null)
                return Guid.Empty;

            ChannelFactory<IClientRepository> cf = null;

            if(locations.ElementAt<ServiceAB>(0).Binding == "NetTcpBinding")
            {
                cf = new ChannelFactory<IClientRepository>(new NetTcpBinding(SecurityMode.None), locations.ElementAt<ServiceAB>(0).Adress);
            }
            else if (locations.ElementAt<ServiceAB>(0).Binding == "BasicHttpBinding")
            {
                cf = new ChannelFactory<IClientRepository>(new BasicHttpBinding(BasicHttpSecurityMode.None), locations.ElementAt<ServiceAB>(0).Adress);
            }

            srCf.Abort();

            var channel = cf.CreateChannel();

            var birthDate = channel.GetClientInformation(clientId).BirthDate;
            cf.Abort();

            if (birthDate.CompareTo(DateTime.Today.AddYears(-18)) < 0)
            {
                _log.Info("Deposit created.");
                _databaseAccess.StoreDeposit(deposit);
                return deposit.Id;
            }

            _log.Info("The client is not 18 years old.");

            return Guid.Empty;
        }

        public Guid CreateDepositForAccount(string accountNumber, Deposit deposit)
        {
            _log.Info("New deposit requested for account " + accountNumber + ".");

            deposit.Id = Guid.NewGuid();
            deposit.AccountNumber = accountNumber;

            var srCf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), _serviceRepositoryAddress);
            var serviceRepository = srCf.CreateChannel();

            List<ServiceAB> locations = serviceRepository.GetServiceLocations("IClientRepository");

            if (locations == null)
                return Guid.Empty;

            ChannelFactory<IAccountRepository> cf = null;

            if (locations.ElementAt<ServiceAB>(0).Binding == "NetTcpBinding")
            {
                cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None), locations.ElementAt<ServiceAB>(0).Adress);
            }
            else if (locations.ElementAt<ServiceAB>(0).Binding == "BasicHttpBinding")
            {
                cf = new ChannelFactory<IAccountRepository>(new BasicHttpBinding(BasicHttpSecurityMode.None), locations.ElementAt<ServiceAB>(0).Adress);
            }

            srCf.Abort();

            var channel = cf.CreateChannel();

            var money = channel.GetAccountInformation(accountNumber).Money;
            cf.Abort();

            if (money >= deposit.Money)
            {
                _log.Info("Deposit created.");
                _databaseAccess.StoreDeposit(deposit);
                return deposit.Id;
            }

            _log.Info("The client does not have enough funds.");

            return Guid.Empty;
        }

        public Deposit GetDepositInformation(Guid depositId)
        {
            _log.Info("Requested information about deposit " + depositId + ".");

            Deposit dep = null;

            try
            {
                dep = _databaseAccess.GetDeposit(depositId);
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                _log.Info("Tried to get deposit " + depositId + " which does not exist.");
            }

            return dep;
        }

        public void RemoveDeposit(Guid depositId)
        {
            _log.Info("Requested removal of deposit " + depositId + ".");

            try
            {
                _databaseAccess.DeleteDeposit(depositId);
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                _log.Info("Tried to remove deposit " + depositId + " which does not exist.");
            }

            return;
        }
    }
}
