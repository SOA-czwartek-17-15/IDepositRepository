using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace DepositService
{
    public class nHibernateAccess : IDatabaseAccess
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
            return dep;
        }
    }
}