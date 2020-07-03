namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedEmployeeFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("EIS.EmployeeDetails", "VehicleType", c => c.String(maxLength: 1000));
            AddColumn("EIS.EmployeeDetails", "VehicleCategory", c => c.Decimal(precision: 10, scale: 0));
            AddColumn("EIS.EmployeeDetails", "EmergencyRelation", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("EIS.EmployeeDetails", "EmergencyRelation");
            DropColumn("EIS.EmployeeDetails", "VehicleCategory");
            DropColumn("EIS.EmployeeDetails", "VehicleType");
        }
    }
}
