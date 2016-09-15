using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;

namespace ProcessAccelerator.Core.Service
{
    public interface ICrudService<T> where T : Entity, new()
    {
        int Create(T item);
        void Save();
        void Delete(int id);
        T Get(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Where(Expression<Func<T, bool>> func, bool showDeleted = false);
        void Restore(int id);
        IRepo<T> getRepo();
        bool validate(T entity, out List<ValidationMessage> messages);
    }
}
