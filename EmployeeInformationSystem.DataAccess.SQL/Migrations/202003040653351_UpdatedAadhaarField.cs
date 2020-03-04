namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedAadhaarField : DbMigration
    {
        public override void Up()
        {
            RenameColumn("EIS.EmployeeDetails", "AadhaarNumer", "AadhaarNumber");
        }
        
        public override void Down()
        {
            RenameColumn("EIS.EmployeeDetails", "AadhaarNumber", "AadhaarNumer");
        }
    }
}
