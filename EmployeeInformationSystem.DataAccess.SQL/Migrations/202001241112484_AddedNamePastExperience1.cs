namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNamePastExperience1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PastExperiences",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        Organisation = c.String(),
                        Position = c.String(),
                        Location = c.String(),
                        Compensation = c.String(),
                        From = c.DateTime(storeType: "date"),
                        To = c.DateTime(storeType: "date"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PastExperiences", "EmployeeId", "dbo.EmployeeDetails");
            DropIndex("dbo.PastExperiences", new[] { "EmployeeId" });
            DropTable("dbo.PastExperiences");
        }
    }
}
