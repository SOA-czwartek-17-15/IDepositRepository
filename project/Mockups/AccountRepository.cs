using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class AccountRepository : IAccountRepository
    {
        public bool CreateAccount(Account details)
        {
            return true;
        }

        public Account GetAccountInformation(string accountNumber)
        {
            var acc = new Account();
            acc.AccountNumber = "67 1234 5678 0000 0000 1234 5678";
            acc.ClientId = Guid.NewGuid();
            acc.Money = 12421;
            acc.Type = AccountType.Deposit;
            acc.Percentage = 2.8;
            acc.EndDate = new System.DateTime(2014, 03, 24);
            acc.EndDate = new System.DateTime(2013, 03, 24);

            return acc;
        }

        public Account GetAccountById(Guid accountId)
        {
            var acc = new Account();
            acc.AccountNumber = "67 1234 5678 0000 0000 1234 5678";
            acc.ClientId = Guid.NewGuid();
            acc.Money = 12421;
            acc.Type = AccountType.Deposit;
            acc.Percentage = 2.8;
            acc.EndDate = new System.DateTime(2014, 03, 24);
            acc.EndDate = new System.DateTime(2013, 03, 24);

            return acc;
        }

        public bool ChangeAccountBalance(Guid accountId, long amount)
        {
            return true;
        }
    }
}
