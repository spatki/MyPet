using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ProcessAccelerator.Core.Repository
{
    public interface IRepo<T>
    {
        T Get(int id);
        IQueryable<T> GetAll();
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, bool showDeleted = false);
        T Insert(T o);
        void Save();
        void Delete(T o);
        void Restore(T o);
        DbContext getDBContext();
        int executeStoredCommand(string SQL);
    }
}
