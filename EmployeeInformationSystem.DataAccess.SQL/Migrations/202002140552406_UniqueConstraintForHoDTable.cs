namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueConstraintForHoDTable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.HoDs", new[] { "DepartmentId" });
            AlterColumn("dbo.HoDs", "Designation", c => c.String(nullable: false, maxLength: 450));
            CreateIndex("dbo.HoDs", new[] { "Designation", "DepartmentId" }, unique: true, name: "IX_DesignationAndDepartment");
        }
        
        public override void Down()
        {
            DropIndex("dbo.HoDs", "IX_DesignationAndDepartment");
            AlterColumn("dbo.HoDs", "Designation", c => c.String(nullable: false));
            CreateIndex("dbo.HoDs", "DepartmentId");
        }
    }
}
