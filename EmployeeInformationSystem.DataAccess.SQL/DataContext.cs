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
        public DataContext(): base("name=DefaultConnection")
        {
            Database.SetInitializer<DataContext>(new DropCreateDatabaseIfModelChanges<DataContext>());

            //For Production
            //Database.SetInitializer<DataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeDetail>().HasOptional(e => e.Organisation);
            modelBuilder.Entity<EmployeeDetail>().HasOptional(e => e.Discipline);
            modelBuilder.Entity<EmployeeDetail>().HasOptional(e => e.DGHLevel);

            modelBuilder.Entity<PostingDetail>().HasOptional(p => p.HOD);

            modelBuilder.Entity<PromotionDetail>().HasOptional(p => p.PayScale);
            modelBuilder.Entity<PromotionDetail>().HasOptional(p => p.Level);
        }
    }
}
