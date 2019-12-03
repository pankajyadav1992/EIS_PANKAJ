using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.DataAccess.SQL
{
    public class DataContext: DbContext
    {
        public DataContext():
            base("name=DefaultConnection")
        {
            Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());

            //For Production
            //Database.SetInitializer<DataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<>
        }
    }
}
