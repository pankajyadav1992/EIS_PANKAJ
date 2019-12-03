using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        internal DataContext _context;
        internal DbSet<T> _dbset;

        public SQLRepository(DataContext dataContext)
        {
            this._context = dataContext;
            this._dbset = _context.Set<T>();
        }

        public IQueryable<T> Collection()
        {
            return _dbset;
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
        }

        public void Insert(T t)
        {
            _dbset.Add(t);
        }

        public void Update(T t)
        {
            _dbset.Attach(t);
            _context.Entry(t).State = EntityState.Modified;
        }

        public T Find(string Id)
        {
            return _dbset.Find(Id);
        }

        public void Delete(string Id)
        {
            var t = Find(Id);
            if (_context.Entry(t).State == EntityState.Detached)
                _dbset.Attach(t);

            _dbset.Remove(t);
        }
    }
}
