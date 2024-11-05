using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Repository<T>(BookstoreDBContext context) : IRepository<T> where T : BaseModel
    {
        private readonly BookstoreDBContext _context = context;

        public IQueryable<T> Queryable
        {
            get
            {
                return _context.Set<T>().AsQueryable();
            }
        }

        public DbSet<T> Entity
        {
            get
            {
                return _context.Set<T>();
            }
        }

        public BookstoreDBContext Context
        {
            get
            {
                return _context;
            }
        }

        public List<T> GetAll() => [.. Entity];

        public T? Get(params object[]? id) => Entity.Find(id);

        public bool Add(T entity)
        {
            Entity.Attach(entity);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(T entity)
        {
            Entity.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public bool Update(T entity)
        {
            Entity.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }
    }
}
