namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedContractExpiry : DbMigration
    {
        public override void Up()
        {
            AddColumn("EIS.EmployeeDetails", "DateOfContractExpiry", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EIS.EmployeeDetails", "DateOfContractExpiry");
        }
    }
}
