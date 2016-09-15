using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Infra;
using Omu.ValueInjecter;

namespace ProcessAccelerator.Data
{
    public class Repo<T> : IRepo<T> where T : Entity, new()
    {
        protected readonly DbContext dbContext;

        public Repo(IDbContextFactory f)
        {
            dbContext = f.GetContext();
            dbContext.Configuration.ProxyCreationEnabled = false;
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public T Insert(T o)
        {
            var t = dbContext.Set<T>().Create();
            t.InjectFrom(o);
            dbContext.Set<T>().Add(t);
            return t;
        }

        public virtual void Delete(T o)
        {
            if (o is IDel)
                (o as IDel).IsDeleted = true;
            else
                dbContext.Set<T>().Remove(o);
        }

        public T Get(int id)
        {
            return dbContext.Set<T>().Find(id);
        }

        public void Restore(T o)
        {
            if (o is IDel)
                IoC.Resolve<IDelRepo<T>>().Restore(o);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool showDeleted = false)
        {
            if (typeof(IDel).IsAssignableFrom(typeof(T)))
                return IoC.Resolve<IDelRepo<T>>().Where(predicate, showDeleted);
            return dbContext.Set<T>().Where(predicate);
        }

        public virtual IQueryable<T> GetAll()
        {
            if (typeof(IDel).IsAssignableFrom(typeof(T)))
                return IoC.Resolve<IDelRepo<T>>().GetAll();
            return dbContext.Set<T>();
        }

        public virtual DbContext getDBContext()
        {
            return dbContext;
        }

        public virtual int executeStoredCommand(string SQL)
        {
            try
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext;
                int records = objCtx.ExecuteStoreCommand(SQL);
                return records;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
