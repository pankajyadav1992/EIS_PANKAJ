namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedEmployeeKeyId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PostingDetails", name: "Employee_Id", newName: "EmployeeId");
            RenameColumn(table: "dbo.PromotionDetails", name: "EmployeeDetailId", newName: "EmployeeId");
            RenameColumn(table: "dbo.TelephoneExtensions", name: "EmployeeDetailId", newName: "EmployeeId");
            RenameIndex(table: "dbo.PostingDetails", name: "IX_Employee_Id", newName: "IX_EmployeeId");
            RenameIndex(table: "dbo.PromotionDetails", name: "IX_EmployeeDetailId", newName: "IX_EmployeeId");
            RenameIndex(table: "dbo.TelephoneExtensions", name: "IX_EmployeeDetailId", newName: "IX_EmployeeId");
            DropColumn("dbo.PostingDetails", "EmployeeDetailId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostingDetails", "EmployeeDetailId", c => c.String());
            RenameIndex(table: "dbo.TelephoneExtensions", name: "IX_EmployeeId", newName: "IX_EmployeeDetailId");
            RenameIndex(table: "dbo.PromotionDetails", name: "IX_EmployeeId", newName: "IX_EmployeeDetailId");
            RenameIndex(table: "dbo.PostingDetails", name: "IX_EmployeeId", newName: "IX_Employee_Id");
            RenameColumn(table: "dbo.TelephoneExtensions", name: "EmployeeId", newName: "EmployeeDetailId");
            RenameColumn(table: "dbo.PromotionDetails", name: "EmployeeId", newName: "EmployeeDetailId");
            RenameColumn(table: "dbo.PostingDetails", name: "EmployeeId", newName: "Employee_Id");
        }
    }
}
