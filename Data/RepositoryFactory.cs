using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class RepositoryFactory
    {
        public static IRepository<T> CreateRepository<T>() where T : BaseModel
        {
            var options = new DbContextOptionsBuilder<BookstoreDBContext>()
                .UseSqlServer("Server=localhost,1433;Database=Bookstore;Trusted_Connection=False;TrustServerCertificate=true;User Id=sa;Password=Passw0rd")
                .Options;

            var context = new BookstoreDBContext(options);
            context.Database.EnsureCreated();

            return new Repository<T>(context);
        }
    }
}
