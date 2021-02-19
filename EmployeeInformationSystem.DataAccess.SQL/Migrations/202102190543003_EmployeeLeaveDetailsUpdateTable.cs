namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeLeaveDetailsUpdateTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("EIS.EmployeeLeaveDetails", "OrganisationId", c => c.String(maxLength: 128));
            AddColumn("EIS.EmployeeLeaveDetails", "StationLeaveReason", c => c.String());
            CreateIndex("EIS.EmployeeLeaveDetails", "OrganisationId");
            AddForeignKey("EIS.EmployeeLeaveDetails", "OrganisationId", "EIS.Organisations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("EIS.EmployeeLeaveDetails", "OrganisationId", "EIS.Organisations");
            DropIndex("EIS.EmployeeLeaveDetails", new[] { "OrganisationId" });
            DropColumn("EIS.EmployeeLeaveDetails", "StationLeaveReason");
            DropColumn("EIS.EmployeeLeaveDetails", "OrganisationId");
        }
    }
}
