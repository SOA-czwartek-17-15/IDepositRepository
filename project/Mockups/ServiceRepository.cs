using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ServiceRepository : IServiceRepository
    {
        public void RegisterService(String Name, String Address)
        {
            return;
        }

        public string GetServiceLocation(String Name)
        {
            if(Name.Equals("IClientRepository"))
                return "net.tcp://127.0.0.1:11111/ClientRepository";
            else if (Name.Equals("IAccountRepository"))
                return "net.tcp://127.0.0.1:11111/AccountRepository";
            else
                return "";
        }

        public void Unregister(String Name)
        {
           return;
        }

        public void Alive(String Name)
        {
            return;
        }
    }
}
