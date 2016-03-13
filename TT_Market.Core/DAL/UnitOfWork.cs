using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TT_Market.Core.Identity;
using DayOfWeek = System.DayOfWeek;

namespace TT_Market.Core.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private static readonly ApplicationDbContext _ttContext;
        private Hashtable _repositories;

        static UnitOfWork()
        {
            _ttContext = new ApplicationDbContext();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _ttContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

        public void SubmitChanges()
        {
            _ttContext.SaveChanges();
        }

        private bool _disposed;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _ttContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
