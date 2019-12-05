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

            modelBuilder.Entity<TelephoneExtension>().HasOptional(p => p.CurrentEmployee);
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<PayScale> PayScales { get; set; }
        public DbSet<PostingDetail> PostingDetails { get; set; }
        public DbSet<PromotionDetail> PromotionDetails { get; set; }
        public DbSet<TelephoneExtension> TelephoneExtensions { get; set; }
    }
}
