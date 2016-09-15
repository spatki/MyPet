using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Repository;
using ProcessAccelerator.Core.Service;

namespace ProcessAccelerator.Service
{
    public class CrudService<T> : ICrudService<T> where T : Entity, new()
    {
        protected IRepo<T> repo;

        public CrudService(IRepo<T> repo)
        {
            this.repo = repo;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return repo.GetAll();
        }

        public virtual T Get(int id)
        {
            var entity = repo.Get(id);
            LoadDependencies(entity);
            return entity;
        }

        public virtual int Create(T item)
        {
            var newItem = repo.Insert(item);
            repo.Save();
            return newItem.ID;
        }

        public virtual void Save()
        {
            repo.Save();
        }

        public virtual void Delete(int id)
        {
            repo.Delete(repo.Get(id));
            repo.Save();
        }

        public void Restore(int id)
        {
            repo.Restore(repo.Get(id));
            repo.Save();
        }

        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> predicate, bool showDeleted = false)
        {
            return repo.Where(predicate, showDeleted);
        }

        public IRepo<T> getRepo()
        {
            return repo;
        }

        public virtual bool validate(T entity, out List<ValidationMessage> messages)
        {
            messages = new List<ValidationMessage>();
            return true;
        }

        public virtual void LoadDependencies(T entity) { }

    }
}
