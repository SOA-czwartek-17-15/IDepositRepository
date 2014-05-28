using System.ServiceModel;
using System;

using Contracts;

namespace DepositService
{
    public interface IDatabaseAccess
    {
        void StoreDeposit(Deposit deposit);
        void DeleteDeposit(Guid depositId);
        Deposit GetDeposit(Guid depositId);
    }
}