namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableInts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EmployeeDetails", "MobileNumber", c => c.Int());
            AlterColumn("dbo.EmployeeDetails", "ResidenceNumber", c => c.Int());
            AlterColumn("dbo.EmployeeDetails", "EmergencyContact", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeDetails", "EmergencyContact", c => c.Int(nullable: false));
            AlterColumn("dbo.EmployeeDetails", "ResidenceNumber", c => c.Int(nullable: false));
            AlterColumn("dbo.EmployeeDetails", "MobileNumber", c => c.Int(nullable: false));
        }
    }
}
