using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace DepositService
{
    public class mockAccess : IDatabaseAccess
    {
        public void storeDeposit(Deposit deposit)
        {
            return;
        }

        public void deleteDeposit(Guid depositId)
        {
            return;
        }

        public Deposit getDeposit(Guid depositId)
        {
            var dep = new Deposit();
            dep.Id = Guid.NewGuid();
            dep.ClientId = Guid.NewGuid();
            dep.AccountNumber = "123456789";
            dep.Type = "standard";
            dep.Interest = 3.0;
            dep.CapitalizationPeriod = 30;
            dep.Money = 2000.0;
            dep.OpeningDate = "05-07-2014";
            dep.Duration = "4 months";
            return dep;
        }
    }
}