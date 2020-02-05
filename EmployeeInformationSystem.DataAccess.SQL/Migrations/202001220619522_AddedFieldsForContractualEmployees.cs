namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldsForContractualEmployees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeDetails", "UANNumber", c => c.String(maxLength: 12));
            AddColumn("dbo.EmployeeDetails", "DeputedLocation", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmployeeDetails", "DeputedLocation");
            DropColumn("dbo.EmployeeDetails", "UANNumber");
        }
    }
}
