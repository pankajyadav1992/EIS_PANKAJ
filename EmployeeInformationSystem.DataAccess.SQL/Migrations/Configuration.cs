namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeInformationSystem.DataAccess.SQL.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            var historyContextFactory = GetHistoryContextFactory("Oracle.ManagedDataAccess.Client");
            SetHistoryContextFactory("Oracle.ManagedDataAccess.Client",
                                     (dbc, schema) => historyContextFactory.Invoke(dbc, "EIS"));
        }

        protected override void Seed(EmployeeInformationSystem.DataAccess.SQL.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
