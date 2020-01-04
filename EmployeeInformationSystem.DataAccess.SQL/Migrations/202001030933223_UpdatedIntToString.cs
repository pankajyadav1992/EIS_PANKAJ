namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedIntToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmployeeDetails", "MobileNumber", c => c.String());
            AlterColumn("dbo.EmployeeDetails", "ResidenceNumber", c => c.String());
            AlterColumn("dbo.EmployeeDetails", "EmergencyContact", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeDetails", "EmergencyContact", c => c.Int());
            AlterColumn("dbo.EmployeeDetails", "ResidenceNumber", c => c.Int());
            AlterColumn("dbo.EmployeeDetails", "MobileNumber", c => c.Int());
        }
    }
}
