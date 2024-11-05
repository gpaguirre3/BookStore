using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IRepository<T> where T : BaseModel
    {
        public IQueryable<T> Queryable { get; }

        public DbSet<T> Entity { get; }

        public BookstoreDBContext Context { get; }

        public List<T> GetAll();

        public T? Get(params object[]? id);

        public bool Add(T entity);

        public bool Update(T entity);

        public bool Delete(T entity);
    }
}
