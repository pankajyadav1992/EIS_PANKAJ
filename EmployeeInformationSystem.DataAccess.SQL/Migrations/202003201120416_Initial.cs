namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EIS.Degrees",
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
                "EIS.Departments",
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
                "EIS.DependentDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DependentName = c.String(nullable: false, maxLength: 1000),
                        DateofBirth = c.DateTime(),
                        Relationship = c.String(maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EIS.EmployeeDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeCode = c.String(nullable: false, maxLength: 450),
                        Title = c.String(maxLength: 1000),
                        FirstName = c.String(nullable: false, maxLength: 1000),
                        MiddleName = c.String(maxLength: 1000),
                        LastName = c.String(maxLength: 1000),
                        EmployeeType = c.Decimal(nullable: false, precision: 10, scale: 0),
                        OrganisationId = c.String(maxLength: 128),
                        DateOfBirth = c.DateTime(),
                        DateOfSuperannuation = c.DateTime(),
                        DateofJoiningParentOrg = c.DateTime(),
                        DateofRelievingLastOffice = c.DateTime(),
                        DateofJoiningDGH = c.DateTime(),
                        DateofLeavingDGH = c.DateTime(),
                        ReasonForLeaving = c.Decimal(precision: 10, scale: 0),
                        DeputationPeriod = c.String(maxLength: 1000),
                        SeatingLocation = c.Decimal(precision: 10, scale: 0),
                        MobileNumber = c.String(maxLength: 20),
                        ResidenceNumber = c.String(maxLength: 100),
                        ResidenceAddress = c.String(maxLength: 1000),
                        PermanentAddress = c.String(maxLength: 1000),
                        EmailID = c.String(maxLength: 100),
                        BloodGroup = c.Decimal(precision: 10, scale: 0),
                        ProfilePhoto = c.String(maxLength: 1000),
                        WorkingStatus = c.Decimal(nullable: false, precision: 1, scale: 0),
                        Gender = c.Decimal(nullable: false, precision: 10, scale: 0),
                        DisciplineId = c.String(maxLength: 128),
                        PrimaryExpertise = c.String(maxLength: 2000),
                        LevelId = c.String(maxLength: 128),
                        CurrentBasicPay = c.String(maxLength: 1000),
                        PANNumber = c.String(maxLength: 10),
                        AadhaarNumber = c.String(maxLength: 12),
                        PassportNumber = c.String(maxLength: 50),
                        PassportValidity = c.DateTime(),
                        VehicleNumber = c.String(maxLength: 1000),
                        MaritalStatus = c.Decimal(precision: 10, scale: 0),
                        MarriageDate = c.DateTime(),
                        AlternateEmailID = c.String(maxLength: 100),
                        EmergencyPerson = c.String(maxLength: 1000),
                        EmergencyContact = c.String(maxLength: 100),
                        UANNumber = c.String(maxLength: 12),
                        DeputedLocation = c.Decimal(precision: 10, scale: 0),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Levels", t => t.LevelId)
                .ForeignKey("EIS.Disciplines", t => t.DisciplineId)
                .ForeignKey("EIS.Organisations", t => t.OrganisationId)
                .Index(t => t.EmployeeCode, unique: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.DisciplineId)
                .Index(t => t.LevelId);
            
            CreateTable(
                "EIS.Levels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        OrganisationId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => new { t.Name, t.OrganisationId }, unique: true, name: "IX_LevelAndOrganisation");
            
            CreateTable(
                "EIS.Organisations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        HeadOffice = c.String(maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "EIS.Disciplines",
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
                "EIS.Designations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        OrganisationId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => new { t.Name, t.OrganisationId }, unique: true, name: "IX_DesignationAndOrganisation");
            
            CreateTable(
                "EIS.EmployeeAsHoDs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        HODId = c.String(nullable: false, maxLength: 128),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EIS.HoDs", t => t.HODId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.HODId);
            
            CreateTable(
                "EIS.HoDs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Designation = c.String(nullable: false, maxLength: 450),
                        DepartmentId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => new { t.Designation, t.DepartmentId }, unique: true, name: "IX_DesignationAndDepartment");
            
            CreateTable(
                "EIS.PastExperiences",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        Organisation = c.String(nullable: false, maxLength: 1000),
                        Position = c.String(nullable: false, maxLength: 1000),
                        Location = c.String(maxLength: 1000),
                        Compensation = c.String(maxLength: 1000),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "EIS.PayScales",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Scale = c.String(nullable: false, maxLength: 450),
                        OrganisationId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => new { t.Scale, t.OrganisationId }, unique: true, name: "IX_ScaleAndOrganisation");
            
            CreateTable(
                "EIS.PostingDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        HODId = c.String(maxLength: 128),
                        Reporting = c.String(maxLength: 1000),
                        DepartmentId = c.String(nullable: false, maxLength: 128),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EIS.HoDs", t => t.HODId)
                .Index(t => t.EmployeeId)
                .Index(t => t.HODId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "EIS.PromotionDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DesignationId = c.String(nullable: false, maxLength: 128),
                        PayScaleId = c.String(maxLength: 128),
                        LevelId = c.String(maxLength: 128),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Designations", t => t.DesignationId, cascadeDelete: true)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("EIS.Levels", t => t.LevelId)
                .ForeignKey("EIS.PayScales", t => t.PayScaleId)
                .Index(t => t.EmployeeId)
                .Index(t => t.DesignationId)
                .Index(t => t.PayScaleId)
                .Index(t => t.LevelId);
            
            CreateTable(
                "EIS.QualificationDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DegreeId = c.String(nullable: false, maxLength: 128),
                        Duration = c.String(maxLength: 1000),
                        Specialization = c.String(maxLength: 1000),
                        Grade = c.String(maxLength: 1000),
                        Class = c.String(maxLength: 1000),
                        Institution = c.String(maxLength: 1000),
                        University = c.String(maxLength: 1000),
                        From = c.DateTime(),
                        To = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.Degrees", t => t.DegreeId, cascadeDelete: true)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.DegreeId);
            
            CreateTable(
                "EIS.TelephoneExtensions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Number = c.Decimal(nullable: false, precision: 10, scale: 0),
                        EmployeeId = c.String(maxLength: 128),
                        CurrentOwner = c.String(maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                        LastUpdateAt = c.DateTime(nullable: false),
                        LastUpdateBy = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("EIS.EmployeeDetails", t => t.EmployeeId)
                .Index(t => t.Number, unique: true)
                .Index(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EIS.TelephoneExtensions", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.QualificationDetails", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.QualificationDetails", "DegreeId", "EIS.Degrees");
            DropForeignKey("EIS.PromotionDetails", "PayScaleId", "EIS.PayScales");
            DropForeignKey("EIS.PromotionDetails", "LevelId", "EIS.Levels");
            DropForeignKey("EIS.PromotionDetails", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.PromotionDetails", "DesignationId", "EIS.Designations");
            DropForeignKey("EIS.PostingDetails", "HODId", "EIS.HoDs");
            DropForeignKey("EIS.PostingDetails", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.PostingDetails", "DepartmentId", "EIS.Departments");
            DropForeignKey("EIS.PayScales", "OrganisationId", "EIS.Organisations");
            DropForeignKey("EIS.PastExperiences", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.EmployeeAsHoDs", "HODId", "EIS.HoDs");
            DropForeignKey("EIS.HoDs", "DepartmentId", "EIS.Departments");
            DropForeignKey("EIS.EmployeeAsHoDs", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.Designations", "OrganisationId", "EIS.Organisations");
            DropForeignKey("EIS.DependentDetails", "EmployeeId", "EIS.EmployeeDetails");
            DropForeignKey("EIS.EmployeeDetails", "OrganisationId", "EIS.Organisations");
            DropForeignKey("EIS.EmployeeDetails", "DisciplineId", "EIS.Disciplines");
            DropForeignKey("EIS.EmployeeDetails", "LevelId", "EIS.Levels");
            DropForeignKey("EIS.Levels", "OrganisationId", "EIS.Organisations");
            DropIndex("EIS.TelephoneExtensions", new[] { "EmployeeId" });
            DropIndex("EIS.TelephoneExtensions", new[] { "Number" });
            DropIndex("EIS.QualificationDetails", new[] { "DegreeId" });
            DropIndex("EIS.QualificationDetails", new[] { "EmployeeId" });
            DropIndex("EIS.PromotionDetails", new[] { "LevelId" });
            DropIndex("EIS.PromotionDetails", new[] { "PayScaleId" });
            DropIndex("EIS.PromotionDetails", new[] { "DesignationId" });
            DropIndex("EIS.PromotionDetails", new[] { "EmployeeId" });
            DropIndex("EIS.PostingDetails", new[] { "DepartmentId" });
            DropIndex("EIS.PostingDetails", new[] { "HODId" });
            DropIndex("EIS.PostingDetails", new[] { "EmployeeId" });
            DropIndex("EIS.PayScales", "IX_ScaleAndOrganisation");
            DropIndex("EIS.PastExperiences", new[] { "EmployeeId" });
            DropIndex("EIS.HoDs", "IX_DesignationAndDepartment");
            DropIndex("EIS.EmployeeAsHoDs", new[] { "HODId" });
            DropIndex("EIS.EmployeeAsHoDs", new[] { "EmployeeId" });
            DropIndex("EIS.Designations", "IX_DesignationAndOrganisation");
            DropIndex("EIS.Disciplines", new[] { "Name" });
            DropIndex("EIS.Organisations", new[] { "Name" });
            DropIndex("EIS.Levels", "IX_LevelAndOrganisation");
            DropIndex("EIS.EmployeeDetails", new[] { "LevelId" });
            DropIndex("EIS.EmployeeDetails", new[] { "DisciplineId" });
            DropIndex("EIS.EmployeeDetails", new[] { "OrganisationId" });
            DropIndex("EIS.EmployeeDetails", new[] { "EmployeeCode" });
            DropIndex("EIS.DependentDetails", new[] { "EmployeeId" });
            DropIndex("EIS.Departments", new[] { "Name" });
            DropIndex("EIS.Degrees", new[] { "Name" });
            DropTable("EIS.TelephoneExtensions");
            DropTable("EIS.QualificationDetails");
            DropTable("EIS.PromotionDetails");
            DropTable("EIS.PostingDetails");
            DropTable("EIS.PayScales");
            DropTable("EIS.PastExperiences");
            DropTable("EIS.HoDs");
            DropTable("EIS.EmployeeAsHoDs");
            DropTable("EIS.Designations");
            DropTable("EIS.Disciplines");
            DropTable("EIS.Organisations");
            DropTable("EIS.Levels");
            DropTable("EIS.EmployeeDetails");
            DropTable("EIS.DependentDetails");
            DropTable("EIS.Departments");
            DropTable("EIS.Degrees");
        }
    }
}
