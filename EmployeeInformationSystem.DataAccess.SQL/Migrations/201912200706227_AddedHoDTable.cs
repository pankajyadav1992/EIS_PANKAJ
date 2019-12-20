namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHoDTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HoDs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DepartmentId = c.String(nullable: false, maxLength: 128),
                        From = c.DateTime(storeType: "date"),
                        To = c.DateTime(storeType: "date"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HoDs", "EmployeeId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.HoDs", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.HoDs", new[] { "DepartmentId" });
            DropIndex("dbo.HoDs", new[] { "EmployeeId" });
            DropTable("dbo.HoDs");
        }
    }
}
