using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace BankServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DepositRepository : IDepositRepository
    {
        public long CreateDeposit(long _ClientNumber, string _Type, long _Money)
        {
            return 5;
        }

        public long CreateDeposit(string _AccountNumber, string _Type, long _Money)
        {
            return 5;
        }

        bool RemoveDeposit(long _ClientNumber)
        {
            return true;
        }

        bool RemoveDeposit(string _AccountNumber)
        {
            return true;
        }
    }

    [ServiceContract]
    public interface IDepositRepository
    {
        [OperationContract]
        long CreateDeposit(long _ClientNumber, string _Type, long _Money);
        [OperationContract]
        long CreateDeposit(string _AccountNumber, string _Type, long _Money);
        [OperationContract]
        bool RemoveDeposit(long _ClientNumber);
        [OperationContract]
        bool RemoveDeposit(string _AccountNumber);
    }

    [DataContract(Namespace = "IDepositRepository")]
    public class Deposit
    {
        [DataMember]
        public long ClientNumber { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public long Money { get; set; }
    }
}
