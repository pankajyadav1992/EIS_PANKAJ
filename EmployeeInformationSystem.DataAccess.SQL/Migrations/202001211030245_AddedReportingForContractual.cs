namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReportingForContractual : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostingDetails", "Reporting", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostingDetails", "Reporting");
        }
    }
}
