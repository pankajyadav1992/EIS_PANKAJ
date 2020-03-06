using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using System.Data;
using System.IO;
using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using System.Text.RegularExpressions;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MasterDataController : BaseController
    {
        IRepository<EmployeeDetail> EmployeeDetailContext;
        IRepository<Discipline> DisciplineContext;
        IRepository<Designation> DesignationContext;
        IRepository<DependentDetail> DependentDetailContext;
        IRepository<Level> LevelContext;
        IRepository<Degree> DegreeContext;
        IRepository<HoD> HoDContext;
        IRepository<Organisation> OrganisationContext;
        IRepository<PastExperience> PastExperienceContext;
        IRepository<PayScale> PayScaleContext;
        IRepository<PostingDetail> PostingDetailContext;
        IRepository<PromotionDetail> PromotionDetailContext;
        IRepository<Department> DepartmentContext;
        IRepository<QualificationDetail> QualificationDetailContext;
        IRepository<TelephoneExtension> TelephoneExtensionContext;
        // Logging.. 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MasterDataController(IRepository<EmployeeDetail> employeeDetailContext,
        IRepository<Discipline> disciplineContext,
        IRepository<DependentDetail> dependentDetailContext,
        IRepository<Level> levelContext,
        IRepository<Degree> degreeContext,
        IRepository<Designation> designationContext,
        IRepository<HoD> hoDContext,
        IRepository<Organisation> organisationContext,
        IRepository<PastExperience> pastExperienceContext,
        IRepository<PayScale> payScaleContext,
        IRepository<PostingDetail> postingDetailContext,
        IRepository<PromotionDetail> promotionDetailContext,
        IRepository<Department> departmentContext,
        IRepository<QualificationDetail> qualificationDetailContext,
        IRepository<TelephoneExtension> telephoneExtensionContext)
        {
            EmployeeDetailContext = employeeDetailContext;
            DisciplineContext = disciplineContext;
            LevelContext = levelContext;
            DegreeContext = degreeContext;
            DependentDetailContext = dependentDetailContext;
            DesignationContext = designationContext;
            HoDContext = hoDContext;
            OrganisationContext = organisationContext;
            PastExperienceContext = pastExperienceContext;
            PostingDetailContext = postingDetailContext;
            PayScaleContext = payScaleContext;
            PromotionDetailContext = promotionDetailContext;
            DepartmentContext = departmentContext;
            QualificationDetailContext = qualificationDetailContext;
            TelephoneExtensionContext = telephoneExtensionContext;

            //Setting Parameters for Page
            base.SetGlobalParameters();
            ViewBag.UserName = UserName;
            ViewBag.ProfilePhoto = ProfilePicture;
            ViewBag.Role = Role;

        }


        // GET: MasterData
        public ActionResult Index()
        {
            List<EmployeeDetail> employees = EmployeeDetailContext.Collection().OrderBy(e => e.FirstName).ToList();
            ViewBag.Employees = employees;
            return View();
        }

        [HttpPost]
        public ActionResult AjaxBulkUpload(HttpPostedFileBase file)
        {
            var returnText = " < div class=\"alert alert-danger\" role=\"alert\"> Nothing Recieved </div>";
            int inserted = 0, skipped = 0, failed = 0;
            if (null != file && 0 < file.ContentLength)
            {
                // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                // to get started. This is how we avoid dependencies on ACE or Interop:
                Stream stream = file.InputStream;

                // We return the interface, so that
                IExcelDataReader reader = null;

                var extension = Path.GetExtension(file.FileName).ToLower();
                returnText = "Successfully Recieved correct file";

                if (".xls" == extension)
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (".xlsx" == extension)
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    returnText = "<div class=\"alert alert-danger\" role=\"alert\"> Wrong File Format uploaded!</div>";
                    return Content(returnText);
                }

                DataTable dt_ = new DataTable();
                dt_ = reader.AsDataSet().Tables[0];

                if ("Dep, Consultants, Associates" != dt_.TableName)
                {
                    returnText = "<div class=\"alert alert-danger\" role=\"alert\"> Wrong File Format uploaded! </div>";
                    return Content(returnText);
                }

                // Get Column names
                List<string> columnNames = new List<string>();

                for (int i = 0; i < dt_.Columns.Count; i++)
                {
                    log.Info("Sheet 1, Column no. " + i + ": " + dt_.Rows[0][i].ToString());
                    columnNames.Add(dt_.Rows[0][i].ToString());
                }
                for (int row_ = 1; row_ < dt_.Rows.Count; row_++)
                {
                    //Employee Code
                    string cpfNo = dt_.Rows[row_][columnNames.FindIndex(c => c == "CPF No.")].ToString();

                    EmployeeDetail employeeToInsert = EmployeeDetailContext.Collection().FirstOrDefault(e => e.EmployeeCode == cpfNo);
                    bool employeeInserted = false;

                    if (null != employeeToInsert)
                    {
                        log.Info("Employee with CPF:" + cpfNo + " already exists. Skipping Entry");
                        skipped++;
                    }
                    else
                    {

                        string DGHId = (from organisation in OrganisationContext.Collection()
                                        where organisation.Name == "DGH"
                                        select organisation.Id).FirstOrDefault();
                        if (string.IsNullOrEmpty(DGHId))
                        {
                            Organisation dghOrganisation = new Organisation() { Name = "DGH" };
                            OrganisationContext.Insert(dghOrganisation);
                            OrganisationContext.Commit();
                            DGHId = dghOrganisation.Id;
                            log.Info("Added Organisation : DGH");
                        }

                        //Organisation
                        string organisationName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Parent Org.")].ToString();
                        string organisationId = (from organisation in OrganisationContext.Collection()
                                                 where organisation.Name == organisationName
                                                 select organisation.Id).FirstOrDefault();
                        if (null == organisationId && "Not Applicable" != organisationName)
                        {
                            Organisation newOrganisation = new Organisation() { Name = organisationName };
                            OrganisationContext.Insert(newOrganisation);
                            OrganisationContext.Commit();
                            organisationId = newOrganisation.Id;
                            log.Info("Added Organisation :" + organisationName);
                        }

                        // Discipline
                        string disciplineName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Discipline Name")].ToString();
                        string disciplineId = (from discipline in DisciplineContext.Collection()
                                               where discipline.Name == disciplineName
                                               select discipline.Id).FirstOrDefault();
                        if (null == disciplineId && !string.IsNullOrEmpty(disciplineName))
                        {
                            Discipline newDiscipline = new Discipline() { Name = disciplineName };
                            DisciplineContext.Insert(newDiscipline);
                            DisciplineContext.Commit();
                            disciplineId = newDiscipline.Id;
                            log.Info("Added Discipline :" + disciplineName);
                        }

                        //Level
                        string dghLevelName = dt_.Rows[row_][columnNames.FindIndex(c => c == "DGH Level")].ToString();
                        string dghLevelId = (from level in LevelContext.Collection()
                                             where level.Name == dghLevelName
                                             select level.Id).FirstOrDefault();
                        if (null == dghLevelId && !string.IsNullOrEmpty(dghLevelName))
                        {
                            Level newLevel = new Level() { Name = dghLevelName, OrganisationId = DGHId };
                            LevelContext.Insert(newLevel);
                            LevelContext.Commit();
                            dghLevelId = newLevel.Id;
                            log.Info("Added Level :" + dghLevelName);
                        }

                        // Other Fields

                        //Employee Type
                        EmployeeType employeeType = EmployeeType.Others;
                        switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Employee Category")].ToString())
                        {
                            case "Deputationist": employeeType = EmployeeType.Deputationist; break;
                            case "Consultant": employeeType = EmployeeType.Consultant; break;
                            case "Adviser": employeeType = EmployeeType.Advisor; break;
                            case "Trainee Officer": employeeType = EmployeeType.TraineeOfficer; break;
                        }

                        //Status
                        Boolean status = "Working" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Status")].ToString() ? true : false;

                        ReasonForLeaving reasonForSeparation = ReasonForLeaving.Others;

                        if (!status)
                        {
                            switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Reason of Separation")].ToString())
                            {
                                case "Contract Expired": reasonForSeparation = ReasonForLeaving.ContractExpired; break;
                                case "Demise": reasonForSeparation = ReasonForLeaving.Demise; break;
                                case "Repatriation": reasonForSeparation = ReasonForLeaving.Repatriation; break;
                                case "Resignation": reasonForSeparation = ReasonForLeaving.Resignation; break;
                                case "Superannuation": reasonForSeparation = ReasonForLeaving.Superannuation; break;
                                case "Termination": reasonForSeparation = ReasonForLeaving.Termination; break;
                                case "Transfer": reasonForSeparation = ReasonForLeaving.Repatriation; break;
                            }
                        }

                        // Basic Pay
                        string currentBasicPay = dt_.Rows[row_][columnNames.FindIndex(c => c == "Basic Pay")].ToString();

                        /*
                         * Dates Section
                         */
                        string tempDateString;

                        // DOJ DGH
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "DOJ DGH")].ToString() ?? null;
                        DateTime? dojDGH = null;
                        if (!string.IsNullOrEmpty(tempDateString)) dojDGH = Convert.ToDateTime(tempDateString);

                        // DOJ Parent
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "DOJ Parent Org.")].ToString() ?? null;
                        DateTime? dojParentOrg = null;
                        if (!string.IsNullOrEmpty(tempDateString)) dojParentOrg = Convert.ToDateTime(tempDateString);

                        // DOB
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Date of Birth")].ToString() ?? null;
                        DateTime? dob = null;
                        if (!string.IsNullOrEmpty(tempDateString)) dob = Convert.ToDateTime(tempDateString);

                        //DoSuperannuation
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Dt. Superannuation")].ToString() ?? null;
                        DateTime? doSuperannuation = null;
                        if (!string.IsNullOrEmpty(tempDateString)) doSuperannuation = Convert.ToDateTime(tempDateString);

                        // DoLastRelieveing
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Last Office Relieving Date")].ToString() ?? null;
                        DateTime? doLastRelieveing = null;
                        if (!string.IsNullOrEmpty(tempDateString)) doLastRelieveing = Convert.ToDateTime(tempDateString);

                        //Passport Validity
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Passport Validity")].ToString() ?? null;
                        DateTime? passportValidity = null;
                        if (!string.IsNullOrEmpty(tempDateString)) passportValidity = Convert.ToDateTime(tempDateString);

                        //Marriage Date
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Marriage Date")].ToString() ?? null;
                        DateTime? marriageDate = null;
                        if (!string.IsNullOrEmpty(tempDateString)) marriageDate = Convert.ToDateTime(tempDateString);

                        //Deputation Period
                        string deputationPeriod = dt_.Rows[row_][columnNames.FindIndex(c => c == "Deputation Period")].ToString();

                        //Seating Location
                        SeatingLocation? seatingLocation = null;
                        switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Sitting Location")].ToString())
                        {
                            case "Ground Floor": seatingLocation = SeatingLocation.GroundFloor; break;
                            case "I Floor": seatingLocation = SeatingLocation.IstFloor; break;
                            case "II Floor": seatingLocation = SeatingLocation.IIndFloor; break;
                            case "III Floor": seatingLocation = SeatingLocation.IIIrdFloor; break;
                            case "IV Floor": seatingLocation = SeatingLocation.IVFloor; break;
                            case "V Floor": seatingLocation = SeatingLocation.VFloor; break;
                            case "SDC, BBSR": seatingLocation = SeatingLocation.SDC_BBSR; break;
                        }

                        // Numbers
                        string mobileNumber = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Mobile No.")].ToString(), @"[^0-9]", "");
                        string residenceNumber = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Res. No.")].ToString(), @"[^0-9]", "");

                        //Address
                        string residenceAddress = dt_.Rows[row_][columnNames.FindIndex(c => c == "Res Add.")].ToString();
                        string permanentAddress = dt_.Rows[row_][columnNames.FindIndex(c => c == "Permanent Add.")].ToString();

                        //Email
                        string emailID = dt_.Rows[row_][columnNames.FindIndex(c => c == "Email ID")].ToString();
                        string altEmailID = dt_.Rows[row_][columnNames.FindIndex(c => c == "Alternate Email ID")].ToString();

                        //Blood Group
                        BloodGroup? bloodGroup = null;
                        switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Blood Group")].ToString())
                        {
                            case "A+ve": bloodGroup = BloodGroup.APositive; break;
                            case "A-ve": bloodGroup = BloodGroup.ANegative; break;
                            case "B+ve": bloodGroup = BloodGroup.BPositive; break;
                            case "AB+ve": bloodGroup = BloodGroup.ABPositive; break;
                            case "AB-ve": bloodGroup = BloodGroup.ABNegative; break;
                            case "O+ve": bloodGroup = BloodGroup.OPositive; break;
                            case "O-ve": bloodGroup = BloodGroup.ONegative; break;
                        }

                        //Gender
                        Gender gender = "MALE" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Gender")].ToString() ? Gender.Male : Gender.Female;

                        //Primary Expertise
                        string primaryExpertise = dt_.Rows[row_][columnNames.FindIndex(c => c == "Primary Expertise")].ToString();

                        //PAN
                        string PAN = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "PAN No")].ToString(), @"(\s+|-)", "").ToUpper();

                        //Aadhaar
                        string Aadhaar = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Aadhar Number")].ToString(), @"(\s+|-)", "");

                        //Passport
                        string passport = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Passport No")].ToString(), @"(\s+|-)", "").ToUpper();

                        //Vehicle Number
                        string vehicleNumber = dt_.Rows[row_][columnNames.FindIndex(c => c == "Vehicle No")].ToString();

                        //Marital Status
                        MaritalStatus? maritalStatus = null;
                        if ("Married" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Marital Status")].ToString()) maritalStatus = MaritalStatus.Married;
                        else if ("Single" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Marital Status")].ToString()) maritalStatus = MaritalStatus.Single;

                        //Emergency Person
                        string emergencyPerson = dt_.Rows[row_][columnNames.FindIndex(c => c == "Emergency Person")].ToString();
                        string emergencyContact = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Emergency Contact No")].ToString(), @"[^0-9]", "");

                        //Title, Name
                        string nameString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Name")].ToString();
                        string title = nameString.Contains('.') ? nameString.Substring(0, nameString.IndexOf('.')) : null;
                        string firstName = null, middleName = null, lastName = null;
                        bool containsTitle = false;

                        if (string.IsNullOrEmpty(title) || title.Length < 2 || title.Contains(' ')) title = (gender == Gender.Male) ? "Mr." : (maritalStatus == MaritalStatus.Married ? "Mrs." : "Ms.");
                        else
                        {
                            title += ".";
                            containsTitle = true;
                        }

                        try
                        {
                            nameString = Regex.Replace(nameString.Trim().ToUpper(), @"\.", ". ");
                            if (nameString.Contains(' '))
                            {
                                string[] subNames = nameString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = containsTitle ? 1 : 0; i < subNames.Length - 1; i++)
                                {
                                    if (1 == subNames[i].Length) subNames[i] += ".";
                                    if (string.IsNullOrEmpty(firstName)) firstName = subNames[i];
                                    else middleName = middleName + (!string.IsNullOrEmpty(middleName) ? " " : "") + subNames[i];
                                }
                                lastName = subNames[subNames.Length - 1];
                            }
                            else firstName = nameString;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed Name processing for CPF:" + cpfNo + ", With Error :" + ex.ToString());
                            firstName = nameString;
                        }
                        //Create Employee Object
                        try
                        {
                            employeeToInsert = new EmployeeDetail()
                            {
                                EmployeeCode = cpfNo,
                                Title = string.IsNullOrEmpty(title) ? null : title,
                                FirstName = string.IsNullOrEmpty(firstName) ? null : firstName,
                                MiddleName = string.IsNullOrEmpty(middleName) ? null : middleName,
                                LastName = string.IsNullOrEmpty(lastName) ? null : lastName,
                                EmployeeType = employeeType,
                                OrganisationId = string.IsNullOrEmpty(organisationId) ? null : organisationId,
                                DateOfBirth = dob,
                                DateOfSuperannuation = doSuperannuation,
                                DateofJoiningParentOrg = dojParentOrg,
                                DateofRelievingLastOffice = doLastRelieveing,
                                DateofJoiningDGH = dojDGH,
                                DeputationPeriod = string.IsNullOrEmpty(deputationPeriod) ? null : deputationPeriod,
                                SeatingLocation = seatingLocation,
                                MobileNumber = string.IsNullOrEmpty(mobileNumber) ? null : mobileNumber,
                                ResidenceNumber = string.IsNullOrEmpty(residenceNumber) ? null : residenceNumber,
                                ResidenceAddress = string.IsNullOrEmpty(residenceAddress) ? null : residenceAddress,
                                PermanentAddress = string.IsNullOrEmpty(permanentAddress) ? null : permanentAddress,
                                EmailID = string.IsNullOrEmpty(emailID) ? null : emailID,
                                AlternateEmailID = string.IsNullOrEmpty(altEmailID) ? null : altEmailID,
                                BloodGroup = bloodGroup,
                                WorkingStatus = status,
                                Gender = gender,
                                DisciplineId = string.IsNullOrEmpty(disciplineId) ? null : disciplineId,
                                PrimaryExpertise = string.IsNullOrEmpty(primaryExpertise) ? null : primaryExpertise,
                                LevelId = string.IsNullOrEmpty(dghLevelId) ? null : dghLevelId,
                                CurrentBasicPay = string.IsNullOrEmpty(currentBasicPay) ? null : currentBasicPay,
                                PANNumber = string.IsNullOrEmpty(PAN) ? null : PAN,
                                AadhaarNumber = string.IsNullOrEmpty(Aadhaar) ? null : Aadhaar,
                                PassportNumber = string.IsNullOrEmpty(passport) ? null : passport,
                                PassportValidity = passportValidity,
                                VehicleNumber = string.IsNullOrEmpty(vehicleNumber) ? null : vehicleNumber,
                                MaritalStatus = maritalStatus,
                                MarriageDate = marriageDate,
                                EmergencyContact = string.IsNullOrEmpty(emergencyContact) ? null : emergencyContact,
                                EmergencyPerson = string.IsNullOrEmpty(emergencyPerson) ? null : emergencyPerson,
                            };
                            if (!status) employeeToInsert.ReasonForLeaving = reasonForSeparation;
                            EmployeeDetailContext.Insert(employeeToInsert);
                            log.Info("Successfully inserted Employee with CPF:" + cpfNo);
                            inserted++;
                            employeeInserted = true;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed Employee Creation for CPF:" + cpfNo + ", With Error :" + ex.ToString());
                            failed++;
                        }

                        if (employeeInserted) //Add remaining data if employee successfully inserted
                        {
                            string employeeId = employeeToInsert.Id;
                            //Telephone Extension
                            var tempString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Extension")].ToString();
                            int extensionNumber = int.Parse(!string.IsNullOrEmpty(tempString) ? tempString : "0");
                            //Check if valid
                            if (999 < extensionNumber && 10000 > extensionNumber)
                            {
                                //Check if exists
                                TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.Number == extensionNumber);
                                if (null == telephoneExtension)
                                //Incase the telephone number is NULL doesn't insert anything
                                {
                                    TelephoneExtensionContext.Insert(new TelephoneExtension()
                                    {
                                        Number = extensionNumber,
                                        EmployeeId = employeeId
                                    });
                                    log.Info("Added Telephone no. :" + extensionNumber + " for Employee CPF:" + cpfNo);
                                }
                                else if (telephoneExtension.EmployeeId != employeeId)
                                {
                                    telephoneExtension.EmployeeId = employeeId;
                                    telephoneExtension.CurrentOwner = null;
                                    TelephoneExtensionContext.Update(telephoneExtension);
                                    log.Info("Update Telephone no. :" + extensionNumber + " for Employee CPF:" + cpfNo);
                                }
                                else log.Info("Skipped Telephone no. :" + extensionNumber + " for Employee CPF:" + cpfNo);
                            }

                            //Qualification details
                            string qualificationDetails = dt_.Rows[row_][columnNames.FindIndex(c => c == "Qualification")].ToString();
                            if (!string.IsNullOrEmpty(qualificationDetails))
                            {
                                List<string> qualificationList = new List<string>();
                                if (qualificationDetails.Contains(',')) qualificationList = qualificationDetails.Split(',').ToList();
                                else qualificationList.Add(qualificationDetails);
                                foreach (string qualification in qualificationList.FindAll(q => !string.IsNullOrEmpty(q)))
                                {
                                    string degreeName = null, specialization = null, duration = null;
                                    tempString = qualification.Trim();
                                    if (tempString.Contains('<'))
                                    {
                                        duration = tempString.ToUpper().Split('<', '>')[1];
                                        tempString = Regex.Replace(tempString, "(\\<.*\\>)", "");
                                    }
                                    if (tempString.Contains('('))
                                    {
                                        degreeName = tempString.ToUpper().Substring(0, tempString.IndexOf('('));
                                        specialization = tempString.ToUpper().Split('(', ')')[1];
                                    }
                                    else
                                    {
                                        degreeName = tempString.ToUpper();
                                    }
                                    //Degree
                                    string degreeId = (from degree in DegreeContext.Collection()
                                                       where degree.Name == degreeName
                                                       select degree.Id).FirstOrDefault();
                                    if (null == degreeId && !string.IsNullOrEmpty(degreeName))
                                    {
                                        Degree newDegree = new Degree() { Name = degreeName };
                                        DegreeContext.Insert(newDegree);
                                        DegreeContext.Commit();
                                        degreeId = newDegree.Id;
                                        log.Info("Added Degree :" + degreeName);
                                    }
                                    QualificationDetailContext.Insert(new QualificationDetail()
                                    {
                                        EmployeeId = employeeId,
                                        DegreeId = degreeId,
                                        Specialization = string.IsNullOrEmpty(specialization) ? null : specialization,
                                        Duration = string.IsNullOrEmpty(duration) ? null : duration
                                    });
                                }
                            }

                            // Promotion Details, only if valid organisation info is present

                            if (!string.IsNullOrEmpty(organisationId))
                            {
                                //Date
                                tempString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Last Promotion Dt.")].ToString() ?? null;
                                DateTime? doLastPromotion = null;
                                if (!string.IsNullOrEmpty(tempString)) doLastPromotion = Convert.ToDateTime(tempString);

                                //Organisation captured above

                                // Level
                                string levelName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Level")].ToString();
                                string levelId = (from level in LevelContext.Collection()
                                                  where level.Name == levelName && level.OrganisationId == organisationId
                                                  select level.Id).FirstOrDefault();
                                if (null == levelId && !string.IsNullOrEmpty(levelName))
                                {
                                    Level newLevel = new Level()
                                    {
                                        Name = levelName,
                                        OrganisationId = organisationId
                                    };
                                    LevelContext.Insert(newLevel);
                                    LevelContext.Commit();
                                    levelId = newLevel.Id;
                                    log.Info("Added Level : " + levelName + ", for Organisation: " + organisationName);
                                }

                                // Designation
                                string designationName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Designation")].ToString();
                                string designationId = (from designation in DesignationContext.Collection()
                                                        where designation.Name == designationName && designation.OrganisationId == organisationId
                                                        select designation.Id).FirstOrDefault();
                                if (null == designationId && !string.IsNullOrEmpty(designationName))
                                {
                                    Designation newDesignation = new Designation()
                                    {
                                        Name = designationName,
                                        OrganisationId = organisationId
                                    };
                                    DesignationContext.Insert(newDesignation);
                                    DesignationContext.Commit();
                                    designationId = newDesignation.Id;
                                    log.Info("Added Designation : " + designationName + ", for Organisation: " + organisationName);
                                }

                                // PayScale
                                string payScale = dt_.Rows[row_][columnNames.FindIndex(c => c == "Pay Scale")].ToString();
                                string payScaleId = (from pyScale in PayScaleContext.Collection()
                                                     where pyScale.Scale == payScale && pyScale.OrganisationId == organisationId
                                                     select pyScale.Id).FirstOrDefault();
                                if (null == payScaleId && !string.IsNullOrEmpty(payScale))
                                {
                                    PayScale newPayScale = new PayScale()
                                    {
                                        Scale = payScale,
                                        OrganisationId = organisationId
                                    };
                                    PayScaleContext.Insert(newPayScale);
                                    PayScaleContext.Commit();
                                    payScaleId = newPayScale.Id;
                                    log.Info("Added PayScale : " + payScale + ", for Organisation: " + organisationName);
                                }

                                PromotionDetailContext.Insert(new PromotionDetail()
                                {
                                    EmployeeId = employeeId,
                                    DesignationId = designationId,
                                    LevelId = levelId,
                                    PayScaleId = payScaleId,
                                    From = doLastPromotion
                                });
                                log.Info("Added Promotion : " + designationName + ", for Employee CPF: " + cpfNo);

                                // Checking for additional promotions
                                string designationAtJoining = dt_.Rows[row_][columnNames.FindIndex(c => c == "Designation at Joining DGH")].ToString();

                                if (!string.IsNullOrEmpty(designationAtJoining))
                                {
                                    // Designation
                                    string oldDesignationId = (from designation in DesignationContext.Collection()
                                                               where designation.Name == designationAtJoining && designation.OrganisationId == organisationId
                                                               select designation.Id).FirstOrDefault();
                                    if (null == oldDesignationId)
                                    {
                                        Designation newDesignation = new Designation()
                                        {
                                            Name = designationAtJoining,
                                            OrganisationId = organisationId
                                        };
                                        DesignationContext.Insert(newDesignation);
                                        DesignationContext.Commit();
                                        oldDesignationId = newDesignation.Id;
                                        log.Info("Added Designation : " + designationAtJoining + ", for Organisation: " + organisationName);
                                    }

                                    //Dates
                                    tempString = dt_.Rows[row_][columnNames.FindIndex(c => c == "DOJ DGH")].ToString() ?? null;
                                    DateTime? from = null;
                                    if (!string.IsNullOrEmpty(tempString)) from = Convert.ToDateTime(tempString);

                                    tempString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Last Promotion Dt.")].ToString() ?? null;
                                    DateTime? to = null;
                                    if (!string.IsNullOrEmpty(tempString)) to = Convert.ToDateTime(tempString).AddDays(-1);

                                    if (null != from && null != to)
                                    {
                                        PromotionDetailContext.Insert(new PromotionDetail()
                                        {
                                            EmployeeId = employeeId,
                                            DesignationId = oldDesignationId,
                                            From = from,
                                            To = to
                                        });
                                        log.Info("Added Promotion : " + designationAtJoining + ", for Employee CPF: " + cpfNo);
                                    }
                                }
                            }

                            // Posting Details

                            //Department
                            string departmentName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Department")].ToString();
                            if (string.IsNullOrEmpty(departmentName)) departmentName = "Not Available"; // BUG FIX for empty departments
                            string departmentId = (from department in DepartmentContext.Collection()
                                                   where department.Name == departmentName
                                                   select department.Id).FirstOrDefault();
                            if (null == departmentId && !string.IsNullOrEmpty(departmentName))
                            {
                                Department newDepartment = new Department() { Name = departmentName };
                                DepartmentContext.Insert(newDepartment);
                                DepartmentContext.Commit();
                                departmentId = newDepartment.Id;
                                log.Info("Added Department : " + departmentName);
                            }

                            //HoD
                            string hoDName = dt_.Rows[row_][columnNames.FindIndex(c => c == "HoD")].ToString();
                            string hoDId = (from hod in HoDContext.Collection()
                                            where hod.Designation == hoDName
                                            select hod.Id).FirstOrDefault();
                            if (null == hoDId && !string.IsNullOrEmpty(hoDName) && null != departmentId)
                            {
                                HoD newHoD = new HoD() { Designation = hoDName, DepartmentId = departmentId };
                                HoDContext.Insert(newHoD);
                                HoDContext.Commit();
                                hoDId = newHoD.Id;
                                log.Info("Added HoD : " + hoDName + ", for Department :" + departmentName);
                            }

                            // Reporting Officer
                            string reportingOfficer = dt_.Rows[row_][columnNames.FindIndex(c => c == "Reporting Officer")].ToString();
                            string reportingOfficerId = (from hod in HoDContext.Collection()
                                                         where hod.Designation == reportingOfficer
                                                         select hod.Id).FirstOrDefault();
                            if (null == reportingOfficerId && !string.IsNullOrEmpty(reportingOfficer))
                            {
                                // Map reporting officer to  "Internal" Department as these are not created via HoD code above
                                string internalDepartmentId = (from department in DepartmentContext.Collection()
                                                               where department.Name == "Internal"
                                                               select department.Id).FirstOrDefault();
                                if (null == internalDepartmentId)
                                {
                                    Department newDepartment = new Department() { Name = "Internal" };
                                    DepartmentContext.Insert(newDepartment);
                                    DepartmentContext.Commit();
                                    internalDepartmentId = newDepartment.Id;
                                    log.Info("Added Department : Internal");
                                }
                                HoD newHoD = new HoD() { Designation = reportingOfficer, DepartmentId = internalDepartmentId };
                                HoDContext.Insert(newHoD);
                                HoDContext.Commit();
                                reportingOfficerId = newHoD.Id;
                                log.Info("Added HoD : " + reportingOfficer + ", for Department : Internal");
                            }

                            PostingDetailContext.Insert(new PostingDetail()
                            {
                                EmployeeId = employeeId,
                                DepartmentId = departmentId,
                                HODId = reportingOfficerId,
                                From = dojDGH
                            });
                        }
                        else log.Error("Skipped adding dependent data for employee CPF: " + cpfNo);
                    }
                }
                EmployeeDetailContext.Commit();
                TelephoneExtensionContext.Commit();
                QualificationDetailContext.Commit();
                PromotionDetailContext.Commit();
                PostingDetailContext.Commit();

                // STAFF Employees Table
                dt_ = reader.AsDataSet().Tables[1];

                // Get Column names
                columnNames = new List<string>();

                for (int i = 0; i < dt_.Columns.Count; i++)
                {
                    log.Info("Sheet 2, Column no. " + i + ": " + dt_.Rows[0][i].ToString());
                    columnNames.Add(dt_.Rows[0][i].ToString());
                }
                for (int row_ = 1; row_ < dt_.Rows.Count; row_++)
                {

                    //Employee Code
                    string cpfNo = dt_.Rows[row_][columnNames.FindIndex(c => c == "Emp ID")].ToString();

                    // If empty, pass Name
                    if (string.IsNullOrEmpty(cpfNo)) cpfNo = dt_.Rows[row_][columnNames.FindIndex(c => c == "Name")].ToString(); ;


                    EmployeeDetail employeeToInsert = !string.IsNullOrEmpty(cpfNo) ? (EmployeeDetailContext.Collection().FirstOrDefault(e => e.EmployeeCode == cpfNo)) : null;
                    bool employeeInserted = false;

                    if (null != employeeToInsert)
                    {
                        log.Info("Staff Employee with CPF:" + cpfNo + " already exists. Skipping Entry");
                        skipped++;
                    }
                    else
                    {
                        string DGHId = (from organisation in OrganisationContext.Collection()
                                        where organisation.Name == "DGH"
                                        select organisation.Id).FirstOrDefault();
                        if (string.IsNullOrEmpty(DGHId))
                        {
                            Organisation dghOrganisation = new Organisation() { Name = "DGH" };
                            OrganisationContext.Insert(dghOrganisation);
                            OrganisationContext.Commit();
                            DGHId = dghOrganisation.Id;
                            log.Info("Added Organisation : DGH");
                        }

                        //Organisation
                        string organisationName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Employer")].ToString();
                        string organisationId = (from organisation in OrganisationContext.Collection()
                                                 where organisation.Name == organisationName
                                                 select organisation.Id).FirstOrDefault();
                        if (null == organisationId && "Not Applicable" != organisationName && !string.IsNullOrEmpty(organisationName))
                        {
                            Organisation newOrganisation = new Organisation() { Name = organisationName };
                            OrganisationContext.Insert(newOrganisation);
                            OrganisationContext.Commit();
                            organisationId = newOrganisation.Id;
                            log.Info("Added Organisation :" + organisationName);
                        }

                        // Discipline
                        string disciplineName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Position")].ToString();
                        string disciplineId = (from discipline in DisciplineContext.Collection()
                                               where discipline.Name == disciplineName
                                               select discipline.Id).FirstOrDefault();
                        if (null == disciplineId && !string.IsNullOrEmpty(disciplineName))
                        {
                            Discipline newDiscipline = new Discipline() { Name = disciplineName };
                            DisciplineContext.Insert(newDiscipline);
                            DisciplineContext.Commit();
                            disciplineId = newDiscipline.Id;
                            log.Info("Added Discipline :" + disciplineName);
                        }

                        //Level
                        string dghLevelName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Category")].ToString();
                        if (!string.IsNullOrEmpty(dghLevelName)) dghLevelName = "Staff Category: " + dghLevelName;
                        string dghLevelId = (from level in LevelContext.Collection()
                                             where level.Name == dghLevelName
                                             select level.Id).FirstOrDefault();
                        if (null == dghLevelId && !string.IsNullOrEmpty(dghLevelName))
                        {
                            Level newLevel = new Level() { Name = dghLevelName, OrganisationId = DGHId };
                            LevelContext.Insert(newLevel);
                            LevelContext.Commit();
                            dghLevelId = newLevel.Id;
                            log.Info("Added Level :" + dghLevelName);
                        }

                        // Other Fields

                        //Employee Type & Deputed Location
                        DeputeLocations deputedLocation = DeputeLocations.DGHNoida;
                        EmployeeType employeeType = EmployeeType.ContractualDGHStaff;

                        switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Deputed Location")].ToString())
                        {
                            case "MOP&NG Delhi":
                                deputedLocation = DeputeLocations.MoPNGDelhi;
                                employeeType = EmployeeType.ContractualMoPNGStaff;
                                break;
                            case "Scope Minar Delhi": deputedLocation = DeputeLocations.ScopeMinarDelhi; break;
                            case "SDC Bhubaneswar": deputedLocation = DeputeLocations.SDCBhubhaneshwar; break;
                        }

                        //Status
                        Boolean status = "Working" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Status")].ToString() ? true : false;

                        ReasonForLeaving reasonForSeparation = ReasonForLeaving.Others;

                        if (!status)
                        {
                            switch (dt_.Rows[row_][columnNames.FindIndex(c => c == "Reason of Separation")].ToString())
                            {
                                case "Contract Expired": reasonForSeparation = ReasonForLeaving.ContractExpired; break;
                                case "Demise": reasonForSeparation = ReasonForLeaving.Demise; break;
                                case "Repatriation": reasonForSeparation = ReasonForLeaving.Repatriation; break;
                                case "Resignation": reasonForSeparation = ReasonForLeaving.Resignation; break;
                                case "Superannuation": reasonForSeparation = ReasonForLeaving.Superannuation; break;
                                case "Termination": reasonForSeparation = ReasonForLeaving.Termination; break;
                                case "Transfer": reasonForSeparation = ReasonForLeaving.Repatriation; break;
                            }
                        }

                        /*
                         * Dates Section
                         */
                        string tempDateString;

                        // DOJ DGH
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "D.O.J.")].ToString() ?? null;
                        DateTime? dojDGH = null;
                        if (!string.IsNullOrEmpty(tempDateString)) dojDGH = Convert.ToDateTime(tempDateString);

                        // DOB
                        tempDateString = dt_.Rows[row_][columnNames.FindIndex(c => c == "D.O.B")].ToString() ?? null;
                        DateTime? dob = null;
                        if (!string.IsNullOrEmpty(tempDateString)) dob = Convert.ToDateTime(tempDateString);

                        //Gender
                        Gender gender = "Male" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Gender")].ToString() ? Gender.Male : Gender.Female;

                        //PAN
                        string PAN = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Pan No.")].ToString(), @"(\s+|-)", "").ToUpper();

                        //Aadhaar
                        string Aadhaar = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Aadhaar No.")].ToString(), @"(\s+|-)", "");

                        //Marital Status
                        MaritalStatus? maritalStatus = null;
                        if ("Married" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Marital")].ToString()) maritalStatus = MaritalStatus.Married;
                        else if ("Unmarried" == dt_.Rows[row_][columnNames.FindIndex(c => c == "Marital")].ToString()) maritalStatus = MaritalStatus.Single;

                        //Emergency Person
                        string emergencyPerson = dt_.Rows[row_][columnNames.FindIndex(c => c == "Emergency Contact Person")].ToString();
                        string emergencyContact = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Emergency No.")].ToString(), @"[^0-9]", "");

                        //Title, Name
                        string nameString = dt_.Rows[row_][columnNames.FindIndex(c => c == "Name")].ToString();
                        string title = nameString.Contains('.') ? nameString.Substring(0, nameString.IndexOf('.')) : null;
                        string firstName = null, middleName = null, lastName = null;
                        bool containsTitle = false;

                        if (string.IsNullOrEmpty(title) || title.Length < 2 || title.Contains(' ')) title = (gender == Gender.Male) ? "Mr." : (maritalStatus == MaritalStatus.Married ? "Mrs." : "Ms.");
                        else
                        {
                            title += ".";
                            containsTitle = true;
                        }

                        try
                        {
                            nameString = Regex.Replace(nameString.Trim().ToUpper(), @"\.", ". ");
                            if (nameString.Contains(' '))
                            {
                                string[] subNames = nameString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = containsTitle ? 1 : 0; i < subNames.Length - 1; i++)
                                {
                                    if (1 == subNames[i].Length) subNames[i] += ".";
                                    if (string.IsNullOrEmpty(firstName)) firstName = subNames[i];
                                    else middleName = middleName + (!string.IsNullOrEmpty(middleName) ? " " : "") + subNames[i];
                                }
                                lastName = subNames[subNames.Length - 1];
                            }
                            else firstName = nameString;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed Name processing for CPF:" + cpfNo + ", With Error :" + ex.ToString());
                            firstName = nameString;
                        }

                        // Numbers
                        string mobileNumber = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "Mobile No.")].ToString(), @"[^0-9]", "");

                        //Address
                        string residenceAddress = dt_.Rows[row_][columnNames.FindIndex(c => c == "Residential Address")].ToString();
                        string permanentAddress = dt_.Rows[row_][columnNames.FindIndex(c => c == "Permanent Address")].ToString();

                        //UAN No.
                        string UANNo = Regex.Replace(dt_.Rows[row_][columnNames.FindIndex(c => c == "UAN No.")].ToString(), @"[^0-9]", "");

                        //Create Employee Object
                        try
                        {
                            employeeToInsert = new EmployeeDetail()
                            {
                                EmployeeCode = string.IsNullOrEmpty(cpfNo) ? nameString : cpfNo,
                                Title = string.IsNullOrEmpty(title) ? null : title,
                                FirstName = string.IsNullOrEmpty(firstName) ? null : firstName,
                                MiddleName = string.IsNullOrEmpty(middleName) ? null : middleName,
                                LastName = string.IsNullOrEmpty(lastName) ? null : lastName,
                                EmployeeType = employeeType,
                                OrganisationId = string.IsNullOrEmpty(organisationId) ? null : organisationId,
                                DateOfBirth = dob,
                                DateofJoiningDGH = dojDGH,
                                DeputedLocation = deputedLocation,
                                MobileNumber = string.IsNullOrEmpty(mobileNumber) ? null : mobileNumber,
                                ResidenceAddress = string.IsNullOrEmpty(residenceAddress) ? null : residenceAddress,
                                PermanentAddress = string.IsNullOrEmpty(permanentAddress) ? null : permanentAddress,
                                WorkingStatus = status,
                                Gender = gender,
                                DisciplineId = string.IsNullOrEmpty(disciplineId) ? null : disciplineId,
                                LevelId = string.IsNullOrEmpty(dghLevelId) ? null : dghLevelId,
                                PANNumber = string.IsNullOrEmpty(PAN) ? null : PAN,
                                AadhaarNumber = string.IsNullOrEmpty(Aadhaar) ? null : Aadhaar,
                                MaritalStatus = maritalStatus,
                                EmergencyContact = string.IsNullOrEmpty(emergencyContact) ? null : emergencyContact,
                                EmergencyPerson = string.IsNullOrEmpty(emergencyPerson) ? null : emergencyPerson,
                                UANNumber = string.IsNullOrEmpty(UANNo) ? null : UANNo
                            };
                            if (!status) employeeToInsert.ReasonForLeaving = reasonForSeparation;
                            EmployeeDetailContext.Insert(employeeToInsert);
                            log.Info("Successfully inserted Staff Employee with CPF:" + cpfNo);
                            inserted++;
                            employeeInserted = true;
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed staff Employee Creation for CPF:" + cpfNo + ", With Error :" + ex.ToString());
                            failed++;
                        }

                        if (employeeInserted) //Add remaining data if employee successfully inserted
                        {
                            string employeeId = employeeToInsert.Id;
                            string tempString = null;


                            //Qualification details
                            string qualificationDetails = dt_.Rows[row_][columnNames.FindIndex(c => c == "Qualification")].ToString();
                            if (!string.IsNullOrEmpty(qualificationDetails))
                            {
                                List<string> qualificationList = new List<string>();
                                if (qualificationDetails.Contains(',')) qualificationList = qualificationDetails.Split(',').ToList();
                                else qualificationList.Add(qualificationDetails);
                                foreach (string qualification in qualificationList.FindAll(q => !string.IsNullOrEmpty(q)))
                                {
                                    string degreeName = null, specialization = null, duration = null;
                                    tempString = qualification.Trim();
                                    if (tempString.Contains('<'))
                                    {
                                        duration = tempString.ToUpper().Split('<', '>')[1];
                                        tempString = Regex.Replace(tempString, "(\\<.*\\>)", "");
                                    }
                                    if (tempString.Contains('('))
                                    {
                                        degreeName = tempString.ToUpper().Substring(0, tempString.IndexOf('('));
                                        specialization = tempString.ToUpper().Split('(', ')')[1];
                                    }
                                    else
                                    {
                                        degreeName = tempString.ToUpper();
                                    }
                                    //Degree
                                    string degreeId = (from degree in DegreeContext.Collection()
                                                       where degree.Name == degreeName
                                                       select degree.Id).FirstOrDefault();
                                    if (null == degreeId && !string.IsNullOrEmpty(degreeName))
                                    {
                                        Degree newDegree = new Degree() { Name = degreeName };
                                        DegreeContext.Insert(newDegree);
                                        DegreeContext.Commit();
                                        degreeId = newDegree.Id;
                                        log.Info("Added Degree :" + degreeName);
                                    }
                                    QualificationDetailContext.Insert(new QualificationDetail()
                                    {
                                        EmployeeId = employeeId,
                                        DegreeId = degreeId,
                                        Specialization = string.IsNullOrEmpty(specialization) ? null : specialization,
                                        Duration = string.IsNullOrEmpty(duration) ? null : duration
                                    });
                                }
                            }

                            // Promotion Details, only if valid organisation info is present

                            if (!string.IsNullOrEmpty(organisationId))
                            {

                                //Organisation set to DGH for Contractual Employees
                                organisationName = "DGH";
                                organisationId = DGHId;

                                // Level captured above

                                // Designation
                                string designationName = "Contractual Staff";
                                string designationId = (from designation in DesignationContext.Collection()
                                                        where designation.Name == designationName && designation.OrganisationId == organisationId
                                                        select designation.Id).FirstOrDefault();
                                if (null == designationId && !string.IsNullOrEmpty(designationName))
                                {
                                    Designation newDesignation = new Designation()
                                    {
                                        Name = designationName,
                                        OrganisationId = organisationId
                                    };
                                    DesignationContext.Insert(newDesignation);
                                    DesignationContext.Commit();
                                    designationId = newDesignation.Id;
                                    log.Info("Added Designation : " + designationName + ", for Organisation: " + organisationName);
                                }

                                // PayScale
                                string payScale = dt_.Rows[row_][columnNames.FindIndex(c => c == "Salary")].ToString();
                                string payScaleId = (from pyScale in PayScaleContext.Collection()
                                                     where pyScale.Scale == payScale && pyScale.OrganisationId == organisationId
                                                     select pyScale.Id).FirstOrDefault();
                                if (null == payScaleId && !string.IsNullOrEmpty(payScale))
                                {
                                    PayScale newPayScale = new PayScale()
                                    {
                                        Scale = payScale,
                                        OrganisationId = organisationId
                                    };
                                    PayScaleContext.Insert(newPayScale);
                                    PayScaleContext.Commit();
                                    payScaleId = newPayScale.Id;
                                    log.Info("Added PayScale : " + payScale + ", for Organisation: " + organisationName);
                                }

                                PromotionDetailContext.Insert(new PromotionDetail()
                                {
                                    EmployeeId = employeeId,
                                    DesignationId = designationId,
                                    LevelId = dghLevelId,
                                    PayScaleId = payScaleId,
                                    From = dojDGH
                                });
                                log.Info("Added Promotion : " + designationName + ", for Employee CPF: " + cpfNo);

                            }

                            // Posting Details

                            //Department
                            string departmentName = dt_.Rows[row_][columnNames.FindIndex(c => c == "Department")].ToString();
                            if (string.IsNullOrEmpty(departmentName)) departmentName = "Not Available"; // BUG FIX for empty departments
                            string departmentId = (from department in DepartmentContext.Collection()
                                                   where department.Name == departmentName
                                                   select department.Id).FirstOrDefault();
                            if (null == departmentId && !string.IsNullOrEmpty(departmentName))
                            {
                                Department newDepartment = new Department() { Name = departmentName };
                                DepartmentContext.Insert(newDepartment);
                                DepartmentContext.Commit();
                                departmentId = newDepartment.Id;
                                log.Info("Added Department : " + departmentName);
                            }

                            // Reporting Officer
                            string reportingOfficer = dt_.Rows[row_][columnNames.FindIndex(c => c == "Reporting")].ToString();

                            PostingDetailContext.Insert(new PostingDetail()
                            {
                                EmployeeId = employeeId,
                                DepartmentId = departmentId,
                                Reporting = reportingOfficer,
                                From = dojDGH
                            });
                        }
                        else log.Error("Skipped adding dependent data for employee CPF: " + cpfNo);
                    }
                }
                EmployeeDetailContext.Commit();
                TelephoneExtensionContext.Commit();
                QualificationDetailContext.Commit();
                PromotionDetailContext.Commit();
                PostingDetailContext.Commit();

                returnText = "<div class=\"alert alert-success\" role=\"alert\"> " + returnText + " & Inserted: <b>" + inserted.ToString() + "</b>, Skipped: <b>" + skipped.ToString() + "</b>, Failed: <b>" + failed.ToString() + "</b> employee records</div>";
                reader.Close();
                reader.Dispose();
            }
            return Content(returnText);
        }

    }
}