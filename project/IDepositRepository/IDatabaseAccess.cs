using System.ServiceModel;
using System;

namespace DepositService
{
    public interface IDatabaseAccess
    {
        void storeDeposit(Deposit deposit);
        void deleteDeposit(Guid depositId);
        Deposit getDeposit(Guid depositId);
    }
}