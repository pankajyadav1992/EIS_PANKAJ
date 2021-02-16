namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class leavetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EIS.EmployeeLeaveBalances",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(maxLength: 128),
                        LeaveTypeId = c.String(maxLength: 128),
                        TotalLeaveCount = c.String(),
                        AvailableLeaveCount = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId)
                .ForeignKey("EIS.LeaveTypes", t => t.LeaveTypeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.LeaveTypeId);
            
            CreateTable(
                "EIS.LeaveTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "EIS.EmployeeLeaveDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(maxLength: 128),
                        LeaveTypeId = c.String(maxLength: 128),
                        NoOfDays = c.String(),
                        LeaveFrom = c.String(),
                        LeaveTill = c.String(),
                        PrefixFrom = c.String(),
                        PrefixTill = c.String(),
                        SuffixFrom = c.String(),
                        SuffixTill = c.String(),
                        Purpose = c.String(),
                        StationLeave = c.Decimal(nullable: false, precision: 1, scale: 0),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId)
                .ForeignKey("EIS.LeaveTypes", t => t.LeaveTypeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.LeaveTypeId);
            
            CreateTable(
                "EIS.LeaveMasters",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LeaveTypeId = c.String(maxLength: 128),
                        OrganisationId = c.String(maxLength: 128),
                        AnnualQuota = c.String(),
                        ValidFrom = c.String(),
                        ValidTill = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.LeaveTypes", t => t.LeaveTypeId)
                .ForeignKey("EIS.Organisations", t => t.OrganisationId)
                .Index(t => t.LeaveTypeId)
                .Index(t => t.OrganisationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EIS.LeaveMasters", "OrganisationId", "EIS.Organisations");
            DropForeignKey("EIS.LeaveMasters", "LeaveTypeId", "EIS.LeaveTypes");
            DropForeignKey("EIS.EmployeeLeaveDetails", "LeaveTypeId", "EIS.LeaveTypes");
            DropForeignKey("EIS.EmployeeLeaveDetails", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.EmployeeLeaveBalances", "LeaveTypeId", "EIS.LeaveTypes");
            DropForeignKey("EIS.EmployeeLeaveBalances", "EmployeeId", "EIS.EmployeeDetails");
            DropIndex("EIS.LeaveMasters", new[] { "OrganisationId" });
            DropIndex("EIS.LeaveMasters", new[] { "LeaveTypeId" });
            DropIndex("EIS.EmployeeLeaveDetails", new[] { "LeaveTypeId" });
            DropIndex("EIS.EmployeeLeaveDetails", new[] { "EmployeeId" });
            DropIndex("EIS.LeaveTypes", new[] { "Name" });
            DropIndex("EIS.EmployeeLeaveBalances", new[] { "LeaveTypeId" });
            DropIndex("EIS.EmployeeLeaveBalances", new[] { "EmployeeId" });
            DropTable("EIS.LeaveMasters");
            DropTable("EIS.EmployeeLeaveDetails");
            DropTable("EIS.LeaveTypes");
            DropTable("EIS.EmployeeLeaveBalances");
        }
    }
}
