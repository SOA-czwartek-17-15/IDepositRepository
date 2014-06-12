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
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        string serviceRepositoryAddress = null;
        IDatabaseAccess databaseAccess = null;

        public DepositRepository(string serviceRepositoryAddress, IDatabaseAccess databaseAccess)
        {
            this.serviceRepositoryAddress = serviceRepositoryAddress;
            this.databaseAccess = databaseAccess;
        }

        public Guid CreateDeposit(Guid clientId, Deposit deposit)
        {
            log.Info("New deposit requested for client " + clientId + ".");

            deposit.Id = Guid.NewGuid();
            deposit.ClientId = clientId;

            var srCf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), serviceRepositoryAddress);
            var serviceRepository = srCf.CreateChannel();
            var addr = serviceRepository.GetServiceLocation("IClientService");

            // wersja uwzględniająca binding
            /*Tuple<string,string> res = serviceRepository.GetServiceLocation("IClientRepository");
            if(res.Item2 == "NetTcpBinding")
            {
                var cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None), res.Item1);
            }
            else if (res.Item2 == "BasicHttpBinding")
            {
                var cf = new ChannelFactory<IAccountRepository>(new BasicHttpBinding(SecurityMode.None), res.Item1);
            }*/

            srCf.Abort();

            if (addr == null)
                return Guid.Empty;

            var cf = new ChannelFactory<IClientRepository>(new NetTcpBinding(SecurityMode.None), addr);
            var channel = cf.CreateChannel();

            var birthDate = channel.GetClientInformation(clientId).BirthDate;
            cf.Abort();

            if (birthDate.CompareTo(DateTime.Today.AddYears(-18)) < 0)
            {
                log.Info("Deposit created.");
                databaseAccess.StoreDeposit(deposit);
                return deposit.Id;
            }

            log.Info("The client is not 18 years old.");

            return Guid.Empty;
        }

        public Guid CreateDeposit(string accountNumber, Deposit deposit)
        {
            log.Info("New deposit requested for account " + accountNumber + ".");

            deposit.Id = Guid.NewGuid();
            deposit.AccountNumber = accountNumber;

            var srCf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), serviceRepositoryAddress);
            var serviceRepository = srCf.CreateChannel();
            var addr = serviceRepository.GetServiceLocation("IAccountRepository");

            // wersja uwzględniająca binding
            /*Tuple<string,string> res = serviceRepository.GetServiceLocation("IAccountRepository");
            if(res.Item2 == "NetTcpBinding")
            {
                var cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None), res.Item1);
            }
            else if (res.Item2 == "BasicHttpBinding")
            {
                var cf = new ChannelFactory<IAccountRepository>(new BasicHttpBinding(SecurityMode.None), res.Item1);
            }*/

            srCf.Abort();

            if (addr == null)
                return Guid.Empty;

            var cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None), addr);
            var channel = cf.CreateChannel();

            var money = channel.GetAccountInformation(accountNumber).Money;
            cf.Abort();

            if (money >= deposit.Money)
            {
                log.Info("Deposit created.");
                databaseAccess.StoreDeposit(deposit);
                return deposit.Id;
            }

            log.Info("The client does not have enough funds.");

            return Guid.Empty;
        }

        public Deposit GetDepositInformation(Guid depositId)
        {
            log.Info("Requested information about deposit " + depositId + ".");

            Deposit dep = null;

            try
            {
                dep = databaseAccess.GetDeposit(depositId);
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                log.Info("Tried to get deposit " + depositId + " which does not exist.");
            }

            return dep;
        }

        public void RemoveDeposit(Guid depositId)
        {
            log.Info("Requested removal of deposit " + depositId + ".");

            try
            {
                databaseAccess.DeleteDeposit(depositId);
            }
            catch (NHibernate.ObjectNotFoundException ex)
            {
                log.Info("Tried to remove deposit " + depositId + " which does not exist.");
            }

            return;
        }
    }
}
