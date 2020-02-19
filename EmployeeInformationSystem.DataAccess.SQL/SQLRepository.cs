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
            catch (Exception ex)
            {
                throw;
                //foreach (var validationErrors in dbEx.EntityValidationErrors)
                //{
                //    foreach (var validationError in validationErrors.ValidationErrors)
                //    {
                //        Trace.TraceInformation("Property: {0} Error: {1}",
                //                                validationError.PropertyName,
                //                                validationError.ErrorMessage);
                //    }
                //}
                // DbEntityValidationException dbEx
            }
        }

        public void Insert(T t)
        {
            t.LastUpdateAt = DateTime.Now;
            _dbset.Add(t);
        }

        public void Update(T t)
        {
            _dbset.Attach(t);
            t.LastUpdateAt = DateTime.Now;
            _context.Entry(t).State = EntityState.Modified;
        }

        public T Find(string Id, bool asNoTracking = false)
        {
            T tToReturn = null;
            if (asNoTracking)
            {
                try { tToReturn = _dbset.AsNoTracking().Single(t => t.Id == Id); }
                catch { tToReturn = null; }
            }
            else tToReturn= _dbset.Find(Id);
            return tToReturn;
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
