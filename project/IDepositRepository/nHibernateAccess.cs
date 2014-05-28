using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contracts;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;

namespace DepositService
{
    public class NHibernateAccess : IDatabaseAccess
    {
        public NHibernateAccess()
        {
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();
            new SchemaExport(config).Execute(false, false, false);
        }

        public void StoreDeposit(Deposit deposit)
        {
            ISessionFactory factory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();

            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save((Deposit)deposit);
                    transaction.Commit();
                }
            }
        }

        public void DeleteDeposit(Guid depositId)
        {
            ISessionFactory factory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();

            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var s = (Deposit)session.Load(typeof(Deposit), depositId);
                    session.Delete(s);
                    transaction.Commit();
                }
            }
        }

        public Deposit GetDeposit(Guid depositId)
        {
            ISessionFactory factory = new NHibernate.Cfg.Configuration().Configure().BuildSessionFactory();

            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    Deposit s = (Deposit)session.Get(typeof(Deposit), depositId);
                    
                    return s;
                }
            }
        }
    }
}