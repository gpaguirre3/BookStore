using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public abstract class BaseRepository<T> where T : BaseModel
    {
        private readonly IRepository<T> _repository;

        protected IRepository<T> Repository
        {
            get { return _repository; }
        }

        public BaseRepository()
        {
            _repository = RepositoryFactory.CreateRepository<T>();
        }

        public IQueryable<T> Build(string[] includes)
        {
            var query = _repository.Queryable;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        virtual public List<T> GetAll()
        {
            return _repository.GetAll();
        }

        virtual public T? FindById(params object[] id)
        {
            return _repository.Get(id);
        }

        virtual public bool Create(T entity)
        {
            return _repository.Add(entity);
        }

        virtual public bool Update(T entity)
        {
            return _repository.Update(entity);
        }

        virtual public bool Delete(T entity)
        {
            return _repository.Delete(entity);
        }

        virtual public bool Delete(object[] id)
        {
            var entity = _repository.Get(id);

            if (entity == null)
            {
                return false;
            }

            return _repository.Delete(entity);
        }

        virtual public List<T> FindAll(Func<T, bool> callback)
        {
            return Repository.Entity.Where(callback).ToList();
        }
    }
}
