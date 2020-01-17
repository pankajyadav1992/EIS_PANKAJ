namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedQualificationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QualificationDetails", "Specialization", c => c.String());
            AddColumn("dbo.QualificationDetails", "Class", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.QualificationDetails", "Class");
            DropColumn("dbo.QualificationDetails", "Specialization");
        }
    }
}
