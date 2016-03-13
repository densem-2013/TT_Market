﻿using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using TT_Market.Core.Identity;

namespace TT_Market.Core.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _ttContext;
        protected DbSet<T> DbSet;
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public Repository(ApplicationDbContext ttContext)
        {
            this._ttContext = ttContext;
            this.DbSet = ttContext.Set<T>();
        }


        #region IRepository<T> Members

        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
            _ttContext.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<T> list)
        {
            var collection = _ttContext.Set(typeof(T)).Local;
            foreach (T entity in list)
            {
                collection.Add(entity);
            }
            _ttContext.SaveChanges();
        }
        public virtual void Update(T entity)
        {
            _ttContext.Entry(entity).State = EntityState.Modified;
            _ttContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
            _ttContext.SaveChanges();
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }
        public virtual T GetById(string id)
        {
            return DbSet.Find(id);
        }


        #endregion

  

    }
}
