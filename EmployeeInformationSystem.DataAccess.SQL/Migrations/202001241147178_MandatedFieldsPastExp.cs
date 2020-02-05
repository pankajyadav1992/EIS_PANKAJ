namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MandatedFieldsPastExp : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PastExperiences", "Organisation", c => c.String(nullable: false));
            AlterColumn("dbo.PastExperiences", "Position", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PastExperiences", "Position", c => c.String());
            AlterColumn("dbo.PastExperiences", "Organisation", c => c.String());
        }
    }
}
