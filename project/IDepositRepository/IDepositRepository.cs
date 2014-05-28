using System.ServiceModel;
using System;

using Contracts;

namespace DepositService
{
    [ServiceContract]
    public interface IDepositRepository
    {
        [OperationContract(Name="CreateDepositForClient")]
        Guid CreateDeposit(Guid clientId, Deposit deposit);

        [OperationContract(Name="CreateDepositForAccount")]
        Guid CreateDeposit(string accountNumber, Deposit deposit);

        [OperationContract]
        Deposit GetDepositInformation(Guid depositId);

        [OperationContract]
        void RemoveDeposit(Guid depositId);
    }
}