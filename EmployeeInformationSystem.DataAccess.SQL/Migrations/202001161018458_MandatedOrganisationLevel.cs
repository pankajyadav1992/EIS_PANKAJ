namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MandatedOrganisationLevel : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Levels", new[] { "Name" });
            AddColumn("dbo.Levels", "OrganisationId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Levels", new[] { "Name", "OrganisationId" }, unique: true, name: "IX_LevelAndOrganisation");
            AddForeignKey("dbo.Levels", "OrganisationId", "dbo.Organisations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Levels", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.Levels", "IX_LevelAndOrganisation");
            DropColumn("dbo.Levels", "OrganisationId");
            CreateIndex("dbo.Levels", "Name", unique: true);
        }
    }
}
