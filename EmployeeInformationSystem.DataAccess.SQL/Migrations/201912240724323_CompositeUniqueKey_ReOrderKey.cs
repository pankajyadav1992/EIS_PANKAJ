namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompositeUniqueKey_ReOrderKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Designations", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.Designations", new[] { "Name" });
            DropIndex("dbo.Designations", new[] { "OrganisationId" });
            DropIndex("dbo.PayScales", new[] { "Scale" });
            DropIndex("dbo.PayScales", new[] { "OrganisationId" });
            AlterColumn("dbo.Designations", "OrganisationId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Designations", new[] { "Name", "OrganisationId" }, unique: true, name: "IX_DesignationAndOrganisation");
            CreateIndex("dbo.PayScales", new[] { "Scale", "OrganisationId" }, unique: true, name: "IX_ScaleAndOrganisation");
            AddForeignKey("dbo.Designations", "OrganisationId", "dbo.Organisations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Designations", "OrganisationId", "dbo.Organisations");
            DropIndex("dbo.PayScales", "IX_ScaleAndOrganisation");
            DropIndex("dbo.Designations", "IX_DesignationAndOrganisation");
            AlterColumn("dbo.Designations", "OrganisationId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PayScales", "OrganisationId");
            CreateIndex("dbo.PayScales", "Scale", unique: true);
            CreateIndex("dbo.Designations", "OrganisationId");
            CreateIndex("dbo.Designations", "Name", unique: true);
            AddForeignKey("dbo.Designations", "OrganisationId", "dbo.Organisations", "Id");
        }
    }
}
