using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ClientRepository : IClientRepository
    {
        public Guid CreateClient(ClientInformation clientInfo)
        {
            return Guid.NewGuid();
        }

        public ClientInformation GetClientInformation(Guid clientID)
        {
            var ci = new ClientInformation();
            ci.FirstName = "Maciej";
            ci.LastName = "Burkowski";
            ci.Country = "Poland";
            ci.City = "Cracow";
            ci.Street = "Mackiewicza Street";
            ci.PostCode = "31-213";
            ci.BirthPlace = "Cracow";
            ci.BirthDate = new System.DateTime(1992, 11, 4);

            return ci;
        }

        public IEnumerable<Guid> SearchForClientsBy(ClientInformation someClientInfo)
        {
            return null;
        }
    }
}
