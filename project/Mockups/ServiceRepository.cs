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
        public void RegisterService(String Name, String Address, String Binding)
        {
            Console.WriteLine("Registered service " + Name + " at " + Address + " with binding " + Binding);

            return;
        }

        public string GetServiceLocation(String Name, String Binding)
        {
            Console.WriteLine("Someone asks about location of " + Name + ", binding: " + Binding);

            if(Name.Equals("IClientRepository"))
                return "net.tcp://127.0.0.1:11111/IClientRepository";
            else if (Name.Equals("IAccountRepository"))
                return "net.tcp://127.0.0.1:11111/IAccountRepository";
            else
                return "";
        } 

        public void Unregister(String Name, String Binding)
        {
            Console.WriteLine(Name + ", binding: " + Binding + " unregistered.");

            return;
        }

        public void Alive(String Name, String Binding)
        {
            Console.WriteLine("Received alive from " + Name + " on binding " + Binding);

            return;
        }

        public void RegisterService(String Name, String Address)
        {
            Console.WriteLine("Registered service " + Name + " at " + Address);

            return;
        }

        public string GetServiceLocation(String Name)
        {
            Console.WriteLine("Someone asks about location of " + Name);

            if (Name.Equals("IClientRepository"))
                return "net.tcp://127.0.0.1:11111/IClientRepository";
            else if (Name.Equals("IAccountRepository"))
                return "net.tcp://127.0.0.1:11111/IAccountRepository";
            else
                return "";
        }

        public void Unregister(String Name)
        {
            Console.WriteLine(Name + " unregistered.");

            return;
        }

        public void Alive(String Name)
        {
            Console.WriteLine("Received alive from " + Name);

            return;
        }
    }
}
