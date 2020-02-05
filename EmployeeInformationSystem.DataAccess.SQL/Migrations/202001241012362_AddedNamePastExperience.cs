namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNamePastExperience : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmployeeDetails", "MiddleName", c => c.String());
            AlterColumn("dbo.EmployeeDetails", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.EmployeeDetails", "LastName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EmployeeDetails", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.EmployeeDetails", "FirstName", c => c.String());
            DropColumn("dbo.EmployeeDetails", "MiddleName");
        }
    }
}
