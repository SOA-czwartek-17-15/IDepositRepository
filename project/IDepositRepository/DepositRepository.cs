using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DepositService
{
    public class DepositRepository : IDepositRepository
    {
        public Guid CreateDeposit(Guid clientId, Deposit deposit)
        {
            return new Guid();
        }

        public Guid CreateDeposit(string accountNumber, Deposit deposit)
        {
            return new Guid();
        }

        public Deposit GetDepositInformation(Guid depositId)
        {
            return null;
        }

        public void RemoveDeposit(Guid depositId)
        {
            return;
        }
    }
}
