using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Contracts
{
    public interface IRepository<T> where T:BaseEntity
    {
        IQueryable<T> Collection();
        void Commit();
        void Delete(string Id);
        T Find(string Id, Boolean asNoTracking = false);
        void Insert(T t);
        void Update(T t);
    }
}
