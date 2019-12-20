namespace EmployeeInformationSystem.DataAccess.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Degrees",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.DependentDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DependentName = c.String(nullable: false),
                        DateofBirth = c.DateTime(storeType: "date"),
                        Relationship = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.EmployeeDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeCode = c.String(nullable: false, maxLength: 450),
                        Title = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(nullable: false),
                        EmployeeType = c.Int(nullable: false),
                        OrganisationId = c.String(maxLength: 128),
                        DateOfBirth = c.DateTime(storeType: "date"),
                        DateOfSuperannuation = c.DateTime(storeType: "date"),
                        DateofJoiningParentOrg = c.DateTime(storeType: "date"),
                        DateofRelievingLastOffice = c.DateTime(storeType: "date"),
                        DateofJoiningDGH = c.DateTime(storeType: "date"),
                        DateofLeavingDGH = c.DateTime(storeType: "date"),
                        ReasonForLeaving = c.Int(),
                        DeputationPeriod = c.String(),
                        SeatingLocation = c.Int(),
                        MobileNumber = c.Int(nullable: false),
                        ResidenceNumber = c.Int(nullable: false),
                        ResidenceAddress = c.String(),
                        PermanentAddress = c.String(),
                        EmailID = c.String(),
                        BloodGroup = c.Int(),
                        ProfilePhoto = c.String(),
                        WorkingStatus = c.Boolean(nullable: false),
                        DateOfSeparation = c.DateTime(storeType: "date"),
                        Gender = c.Int(nullable: false),
                        DisciplineId = c.String(maxLength: 128),
                        PrimaryExpertise = c.String(),
                        LevelId = c.String(maxLength: 128),
                        CurrentBasicPay = c.String(),
                        PANNumber = c.String(maxLength: 10),
                        AadhaarNumer = c.String(maxLength: 12),
                        PassportNumber = c.String(),
                        PassportValidity = c.DateTime(storeType: "date"),
                        VehicleNumber = c.String(),
                        MaritalStatus = c.Int(),
                        MarriageDate = c.DateTime(storeType: "date"),
                        AlternateEmailID = c.String(),
                        EmergencyPerson = c.String(),
                        EmergencyContact = c.Int(nullable: false),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Levels", t => t.LevelId)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineId)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.EmployeeCode, unique: true)
                .Index(t => t.OrganisationId)
                .Index(t => t.DisciplineId)
                .Index(t => t.LevelId);
            
            CreateTable(
                "dbo.Levels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Disciplines",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Organisations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        HeadOffice = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Designations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 450),
                        OrganisationId = c.String(maxLength: 128),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId)
                .Index(t => t.Name, unique: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.PayScales",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Scale = c.String(nullable: false, maxLength: 450),
                        OrganisationId = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisations", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.Scale, unique: true)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.PostingDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeDetailId = c.String(),
                        HODId = c.String(maxLength: 128),
                        DepartmentId = c.String(nullable: false, maxLength: 128),
                        From = c.DateTime(storeType: "date"),
                        To = c.DateTime(storeType: "date"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                        Employee_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeDetails", t => t.Employee_Id, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeDetails", t => t.HODId)
                .Index(t => t.HODId)
                .Index(t => t.DepartmentId)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.PromotionDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeDetailId = c.String(nullable: false, maxLength: 128),
                        DesignationId = c.String(nullable: false, maxLength: 128),
                        PayScaleId = c.String(maxLength: 128),
                        LevelId = c.String(maxLength: 128),
                        From = c.DateTime(storeType: "date"),
                        To = c.DateTime(storeType: "date"),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Designations", t => t.DesignationId, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeDetailId, cascadeDelete: true)
                .ForeignKey("dbo.Levels", t => t.LevelId)
                .ForeignKey("dbo.PayScales", t => t.PayScaleId)
                .Index(t => t.EmployeeDetailId)
                .Index(t => t.DesignationId)
                .Index(t => t.PayScaleId)
                .Index(t => t.LevelId);
            
            CreateTable(
                "dbo.QualificationDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EmployeeId = c.String(nullable: false, maxLength: 128),
                        DegreeId = c.String(nullable: false, maxLength: 128),
                        Institution = c.String(),
                        University = c.String(),
                        From = c.DateTime(storeType: "date"),
                        To = c.DateTime(storeType: "date"),
                        Grade = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Degrees", t => t.DegreeId, cascadeDelete: true)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.DegreeId);
            
            CreateTable(
                "dbo.TelephoneExtensions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Number = c.Int(nullable: false),
                        EmployeeDetailId = c.String(maxLength: 128),
                        CurrentOwner = c.String(),
                        CreatedAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateAt = c.DateTimeOffset(nullable: false, precision: 7),
                        LastUpdateBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeDetails", t => t.EmployeeDetailId)
                .Index(t => t.Number, unique: true)
                .Index(t => t.EmployeeDetailId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelephoneExtensions", "EmployeeDetailId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.QualificationDetails", "EmployeeId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.QualificationDetails", "DegreeId", "dbo.Degrees");
            DropForeignKey("dbo.PromotionDetails", "PayScaleId", "dbo.PayScales");
            DropForeignKey("dbo.PromotionDetails", "LevelId", "dbo.Levels");
            DropForeignKey("dbo.PromotionDetails", "EmployeeDetailId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.PromotionDetails", "DesignationId", "dbo.Designations");
            DropForeignKey("dbo.PostingDetails", "HODId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.PostingDetails", "Employee_Id", "dbo.EmployeeDetails");
            DropForeignKey("dbo.PostingDetails", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.PayScales", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.Designations", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.DependentDetails", "EmployeeId", "dbo.EmployeeDetails");
            DropForeignKey("dbo.EmployeeDetails", "OrganisationId", "dbo.Organisations");
            DropForeignKey("dbo.EmployeeDetails", "DisciplineId", "dbo.Disciplines");
            DropForeignKey("dbo.EmployeeDetails", "LevelId", "dbo.Levels");
            DropIndex("dbo.TelephoneExtensions", new[] { "EmployeeDetailId" });
            DropIndex("dbo.TelephoneExtensions", new[] { "Number" });
            DropIndex("dbo.QualificationDetails", new[] { "DegreeId" });
            DropIndex("dbo.QualificationDetails", new[] { "EmployeeId" });
            DropIndex("dbo.PromotionDetails", new[] { "LevelId" });
            DropIndex("dbo.PromotionDetails", new[] { "PayScaleId" });
            DropIndex("dbo.PromotionDetails", new[] { "DesignationId" });
            DropIndex("dbo.PromotionDetails", new[] { "EmployeeDetailId" });
            DropIndex("dbo.PostingDetails", new[] { "Employee_Id" });
            DropIndex("dbo.PostingDetails", new[] { "DepartmentId" });
            DropIndex("dbo.PostingDetails", new[] { "HODId" });
            DropIndex("dbo.PayScales", new[] { "OrganisationId" });
            DropIndex("dbo.PayScales", new[] { "Scale" });
            DropIndex("dbo.Designations", new[] { "OrganisationId" });
            DropIndex("dbo.Designations", new[] { "Name" });
            DropIndex("dbo.Organisations", new[] { "Name" });
            DropIndex("dbo.Disciplines", new[] { "Name" });
            DropIndex("dbo.Levels", new[] { "Name" });
            DropIndex("dbo.EmployeeDetails", new[] { "LevelId" });
            DropIndex("dbo.EmployeeDetails", new[] { "DisciplineId" });
            DropIndex("dbo.EmployeeDetails", new[] { "OrganisationId" });
            DropIndex("dbo.EmployeeDetails", new[] { "EmployeeCode" });
            DropIndex("dbo.DependentDetails", new[] { "EmployeeId" });
            DropIndex("dbo.Departments", new[] { "Name" });
            DropIndex("dbo.Degrees", new[] { "Name" });
            DropTable("dbo.TelephoneExtensions");
            DropTable("dbo.QualificationDetails");
            DropTable("dbo.PromotionDetails");
            DropTable("dbo.PostingDetails");
            DropTable("dbo.PayScales");
            DropTable("dbo.Designations");
            DropTable("dbo.Organisations");
            DropTable("dbo.Disciplines");
            DropTable("dbo.Levels");
            DropTable("dbo.EmployeeDetails");
            DropTable("dbo.DependentDetails");
            DropTable("dbo.Departments");
            DropTable("dbo.Degrees");
        }
    }
}
