using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using EmployeeInformationSystem.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    public class EmployeeDataController : BaseController
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

        public EmployeeDataController(IRepository<EmployeeDetail> employeeDetailContext,
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

        [Authorize(Roles= "Add")]
        public ActionResult AddEmployee()
        {
            return View();
        }

        [Authorize(Roles = "Add,Edit")]
        public ActionResult AjaxAdd(string targetPage, string mode, string EmployeeId)
        {
            // Empty ViewModel & Select List
            DataViewModel viewModel = null;
            List<SelectListItem> employeeType = new List<SelectListItem>() { new SelectListItem() { Text = "--Select Employee Type--", Value = "" } };
            string organisationName = "DGH";
            //If Method called for editing, Check if Emp ID passed along with it
            if (mode == "Edit" & null != EmployeeId)
            {
                EmployeeDetail employee = EmployeeDetailContext.Find(EmployeeId); //Fetch Employee from DB
                if (null != employee)
                {
                    viewModel = new DataViewModel() // Create View Model based on the Employee & its related fields
                    {
                        EmployeeDetails = employee,
                        DependentDetails = DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList(),
                        TelephoneExtensions = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.EmployeeId == employee.Id),
                        QualificationDetails = QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList(),
                        PastExperiences = PastExperienceContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                        PromotionDetails = PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                        PostingDetails = PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                    };
                    if (null != employee.AadhaarNumber)
                    {
                        viewModel.AadhaarPart1 = employee.AadhaarNumber.Substring(0, 4);
                        viewModel.AadhaarPart2 = employee.AadhaarNumber.Substring(4, 4);
                        viewModel.AadhaarPart3 = employee.AadhaarNumber.Substring(8, 4);
                    }
                    organisationName = null != employee.Organisation ? employee.Organisation.Name : "DGH"; // Temp Bug fix to resolve issue with empty Org Names
                }
                else
                {
                    return HttpNotFound(); // Employee not in DB
                }

            }
            else // If passed for Addition
            {
                viewModel = new DataViewModel() // Create empty view model with basic common parameters of new Employee
                {
                    EmployeeDetails = new EmployeeDetail()
                    {
                        MaritalStatus = MaritalStatus.Single,
                        WorkingStatus = true
                    }
                };
            }
            switch (targetPage)
            {
                case "Deputationist":
                    employeeType.Add(new SelectListItem() { Text = "Deputationist", Value = "0" }); // Add fields to drop down accordingly
                    break;
                case "Consultant":
                    employeeType.Add(new SelectListItem() { Text = "Adviser", Value = "1" });
                    employeeType.Add(new SelectListItem() { Text = "Consultant", Value = "2" });
                    break;
                case "Contractual":
                    employeeType.Add(new SelectListItem() { Text = "Contractual - DGH Staff", Value = "3" });
                    employeeType.Add(new SelectListItem() { Text = "Contractual - MoPNG Staff", Value = "4" });
                    employeeType.Add(new SelectListItem() { Text = "Trainee Officer", Value = "5" });
                    employeeType.Add(new SelectListItem() { Text = "Others", Value = "5" });
                    break;
                default: break;
            }
            viewModel.EmployeeType = employeeType; // Add appropiate employeeType dropdown to ViewModel
            viewModel.Degrees = DegreeContext.Collection().OrderBy(d => d.Name); // Master Data for Degrees dropdown
            viewModel.Disciplines = DisciplineContext.Collection().OrderBy(d => d.Name); // Master Data for Disciplines dropdown
            /*
             * Designations, Levels, PayScales are filtered to contain only DGH Specific data.
             * The fields for other organisations are handled via separate AJAX-JSON Method
             */
            viewModel.Designations = DesignationContext.Collection().Where(d => d.Organisation.Name == organisationName).OrderBy(d => d.Name); //DGH Specific OR Org specific if for Editing
            viewModel.Levels = LevelContext.Collection().Where(l => l.Organisation.Name == organisationName).OrderBy(l => l.Name); //DGH Specific OR Org specific if for Editing
            viewModel.PayScales = PayScaleContext.Collection().Where(p => p.Organisation.Name == organisationName).OrderBy(p => p.Scale); //DGH Specific OR Org specific if for Editing
            viewModel.Organisations = OrganisationContext.Collection().Where(o => o.Name != "DGH").OrderBy(o => o.Name); // All organisations except DGH must be in dropdown
            viewModel.Departments = DepartmentContext.Collection().OrderBy(d => d.Name); // Master Data for Departments dropdown
            viewModel.HoDs = HoDContext.Collection().OrderBy(d => d.Designation); //Add proper support for determining current HoDs based on date
            ViewBag.Mode = mode;
            return View(targetPage, viewModel);
        }

        [Authorize(Roles = "Add")]
        [HttpPost]
        public ActionResult AjaxAdd(DataViewModel viewModel, HttpPostedFileBase file)
        {
            ViewBag.Mode = "Add";
            string returnText = "Error";
            string EmpId = null;

            /*
            CHECK FOR UNIQUE EMPLOYEE-CODE
            */
            EmpId = (from employee in EmployeeDetailContext.Collection()
                     where employee.EmployeeCode == viewModel.EmployeeDetails.EmployeeCode
                     select employee.Id).FirstOrDefault();
            if (null != EmpId)
            {
                ModelState.AddModelError("", "An Employee with identical Employee Code/CPF already exists! Please select another value");
            }
            if (!ModelState.IsValid)
            {
                String messages = String.Join("\n", ModelState.Values.SelectMany(v => v.Errors)
                                                   .Select(v => v.ErrorMessage + " " + v.Exception));
                returnText = "<div class=\"alert alert-danger\" role=\"alert\"> " + messages + " </div>";
            }
            else
            {
                //Join Aadhaar string. BUG FIX: If no Aadhaar values are supplied, Don't join
                if (null != viewModel.AadhaarPart1 && null != viewModel.AadhaarPart2 && null != viewModel.AadhaarPart3)
                { viewModel.EmployeeDetails.AadhaarNumber = viewModel.AadhaarPart1 + viewModel.AadhaarPart2 + viewModel.AadhaarPart3; }
                //Insert profile photo
                if (file != null)
                {
                    viewModel.EmployeeDetails.ProfilePhoto = viewModel.EmployeeDetails.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//profile_pics//") + viewModel.EmployeeDetails.ProfilePhoto);
                }
                //Insert employee first
                EmployeeDetailContext.Insert(viewModel.EmployeeDetails, UserName);
                EmpId = viewModel.EmployeeDetails.Id;
                EmployeeDetailContext.Commit();
                //Dependents next
                if (null != viewModel.DependentDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (DependentDetail dependent in viewModel.DependentDetails.Where(d => d.EmployeeId != "TBD"))
                    {
                        DependentDetailContext.Insert(dependent, UserName);
                    }
                    DependentDetailContext.Commit();
                }
                //Telephone Extension 
                //Check if exists
                TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.Number == viewModel.TelephoneExtensions.Number);
                if (null == telephoneExtension && 9999 != viewModel.TelephoneExtensions.Number)
                //Incase the telephone number is NULL, Model state is valid but it goes forward and doesn't insert anything
                {
                    TelephoneExtensionContext.Insert(viewModel.TelephoneExtensions, UserName);
                }
                else if (null != telephoneExtension)
                {
                    telephoneExtension.EmployeeId = viewModel.TelephoneExtensions.EmployeeId;
                    telephoneExtension.CurrentOwner = null;
                    TelephoneExtensionContext.Update(telephoneExtension, UserName);
                }
                TelephoneExtensionContext.Commit();
                // Qualification Details
                if (null != viewModel.QualificationDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (QualificationDetail qualification in viewModel.QualificationDetails.Where(q => q.EmployeeId != "TBD"))
                    {
                        QualificationDetailContext.Insert(qualification, UserName);
                    }
                    QualificationDetailContext.Commit();
                }
                //Promotion Details
                if (null != viewModel.PromotionDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PromotionDetail promotion in viewModel.PromotionDetails.Where(p => p.EmployeeId != "TBD"))
                    {
                        PromotionDetailContext.Insert(promotion, UserName);
                    }
                    PromotionDetailContext.Commit();
                }
                //Past Experience Details
                if (null != viewModel.PastExperiences) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PastExperience experience in viewModel.PastExperiences.Where(p => p.EmployeeId != "TBD"))
                    {
                        PastExperienceContext.Insert(experience, UserName);
                    }
                    PastExperienceContext.Commit();
                }
                //Posting Details
                if (null != viewModel.PostingDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PostingDetail posting in viewModel.PostingDetails.Where(p => p.EmployeeId != "TBD"))
                    {
                        PostingDetailContext.Insert(posting, UserName);
                    }
                    PostingDetailContext.Commit();
                }
                returnText = "Success";
                ViewBag.EmployeeCode = viewModel.EmployeeDetails.EmployeeCode;
                ViewBag.EmployeeId = EmpId;
            }
            if (returnText == "Success")
            {
                return View("Success");
            }
            else
            {
                return Content(returnText);
            }
        }

        [Authorize(Roles = "Edit")]
        public ActionResult EditEmployee()
        {
            List<EmployeeDetail> employees = EmployeeDetailContext.Collection().OrderBy(e => e.FirstName).ToList();
            ViewBag.Employees = employees;
            ViewBag.Target = "Edit";
            ViewBag.Title = "Edit Employee";
            return View("SelectEmployee");
        }

        [Authorize(Roles = "Edit")]
        [HttpPost]
        public ActionResult AjaxEdit(DataViewModel viewModel, HttpPostedFileBase file)
        {
            ViewBag.Mode = "Edit";
            string returnText = "Error";

            /*
            CHECK FOR UNIQUE EMPLOYEE-ID
            */
            EmployeeDetail existingEmployee = EmployeeDetailContext.Find(viewModel.EmployeeDetails.Id, true);
            // Check If New Employee created
            if (null == existingEmployee)
            {
                ModelState.AddModelError("", "Employee details not found in Database!");
            }
            else if (viewModel.EmployeeDetails.EmployeeCode != existingEmployee.EmployeeCode && existingEmployee.Id != (from employee in EmployeeDetailContext.Collection()
                                                                                                                        where employee.EmployeeCode == viewModel.EmployeeDetails.EmployeeCode
                                                                                                                        select employee.Id).First())
            {
                ModelState.AddModelError("", "An Employee with identical Employee Code/CPF already exists! Please select another value");
            }
            // Check If Model Validation failed
            if (!ModelState.IsValid)
            {
                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                   .Select(v => v.ErrorMessage + " " + v.Exception));
                returnText = "<div class=\"alert alert-danger\" role=\"alert\"> " + messages + " </div>";
            }
            else
            {
                //Join Aadhaar string. BUG FIX: If no Aadhaar values are supplied, Don't join
                if (null != viewModel.AadhaarPart1 && null != viewModel.AadhaarPart2 && null != viewModel.AadhaarPart3)
                { viewModel.EmployeeDetails.AadhaarNumber = viewModel.AadhaarPart1 + viewModel.AadhaarPart2 + viewModel.AadhaarPart3; }
                //Upadate existing profile photo
                if (file != null)
                {
                    viewModel.EmployeeDetails.ProfilePhoto = viewModel.EmployeeDetails.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//profile_pics//") + viewModel.EmployeeDetails.ProfilePhoto);
                }
                // Update Employee
                EmployeeDetailContext.Update(viewModel.EmployeeDetails, UserName);
                EmployeeDetailContext.Commit();

                //Dependents
                if (null != viewModel.DependentDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (DependentDetail dependent in viewModel.DependentDetails)
                    {
                        var existingDependent = DependentDetailContext.Find(dependent.Id, true); // Check if exists
                        if ("TBD" != dependent.EmployeeId)
                        {
                            if (null != existingDependent) DependentDetailContext.Update(dependent, UserName); // If not TBD & exists, Update
                            else DependentDetailContext.Insert(dependent, UserName); // If not TBD & doesn't exists, Insert

                        }
                        else if (null != existingDependent) DependentDetailContext.Delete(dependent.Id); // If TBD & exists, Delete
                    }
                    DependentDetailContext.Commit();
                }

                //Telephone Extension 
                //Check if exists
                TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.Number == viewModel.TelephoneExtensions.Number);
                if (null == telephoneExtension && 9999 != viewModel.TelephoneExtensions.Number)
                //Incase the telephone number is NULL, Model state is valid but it goes forward and doesn't insert anything
                {
                    TelephoneExtensionContext.Insert(viewModel.TelephoneExtensions, UserName); // Insert new Extension entry
                }
                else if (null != telephoneExtension)
                {
                    if (telephoneExtension.EmployeeId != viewModel.TelephoneExtensions.EmployeeId) // Update Telephone extension if exists
                    {
                        telephoneExtension.EmployeeId = viewModel.TelephoneExtensions.EmployeeId;
                        telephoneExtension.CurrentOwner = null;
                        TelephoneExtensionContext.Update(telephoneExtension, UserName);
                    }
                    else if (9999 == viewModel.TelephoneExtensions.Number && telephoneExtension.EmployeeId == viewModel.TelephoneExtensions.EmployeeId)
                    {
                        TelephoneExtensionContext.Delete(telephoneExtension.Id); // Deletion Specified
                    }
                }
                TelephoneExtensionContext.Commit();

                // Qualification Details
                if (null != viewModel.QualificationDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (QualificationDetail qualification in viewModel.QualificationDetails)
                    {
                        var existingQualification = QualificationDetailContext.Find(qualification.Id, true); // Check if exists
                        if ("TBD" != qualification.EmployeeId)
                        {
                            if (null != existingQualification) QualificationDetailContext.Update(qualification, UserName); // If not TBD & exists, Update
                            else QualificationDetailContext.Insert(qualification, UserName); // If not TBD & doesn't exists, Insert

                        }
                        else if (null != existingQualification) QualificationDetailContext.Delete(qualification.Id); // If TBD & exists, Delete
                    }
                    QualificationDetailContext.Commit();
                }
                //Promotion Details
                if (null != viewModel.PromotionDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PromotionDetail promotion in viewModel.PromotionDetails)
                    {
                        var existingPromotion = PromotionDetailContext.Find(promotion.Id, true);
                        if ("TBD" != promotion.EmployeeId)
                        {
                            if (null != existingPromotion) PromotionDetailContext.Update(promotion, UserName); // If not TBD & exists, Update
                            else PromotionDetailContext.Insert(promotion, UserName); // If not TBD & doesn't exists, Insert

                        }
                        else if (null != existingPromotion) PromotionDetailContext.Delete(promotion.Id); // If TBD & exists, Delete
                    }
                    PromotionDetailContext.Commit();
                }
                //Past Experience Details
                if (null != viewModel.PastExperiences) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PastExperience experience in viewModel.PastExperiences.Where(p => p.EmployeeId != "TBD"))
                    {
                        var existingExperience = PastExperienceContext.Find(experience.Id, true);
                        if ("TBD" != experience.EmployeeId)
                        {
                            if (null != existingExperience) PastExperienceContext.Update(experience, UserName); // If not TBD & exists, Update
                            else PastExperienceContext.Insert(experience, UserName); // If not TBD & doesn't exists, Insert

                        }
                        else if (null != existingExperience) PastExperienceContext.Delete(experience.Id); // If TBD & exists, Delete
                    }
                    PastExperienceContext.Commit();
                }
                //Posting Details
                if (null != viewModel.PostingDetails) //BUG FIX: foreach fails on empty lists
                {
                    foreach (PostingDetail posting in viewModel.PostingDetails)
                    {
                        var existingPosting = PostingDetailContext.Find(posting.Id, true);
                        if ("TBD" != posting.EmployeeId)
                        {
                            if (null != existingPosting) PostingDetailContext.Update(posting, UserName); // If not TBD & exists, Update
                            else PostingDetailContext.Insert(posting, UserName); // If not TBD & doesn't exists, Insert

                        }
                        else if (null != existingPosting) PostingDetailContext.Delete(posting.Id); // If TBD & exists, Delete
                    }
                    PostingDetailContext.Commit();
                }
                returnText = "Success";
                ViewBag.EmployeeCode = viewModel.EmployeeDetails.EmployeeCode;
                ViewBag.EmployeeId = viewModel.EmployeeDetails.Id;
            }
            if (returnText == "Success")
            {
                return View("Success");
            }
            else
            {
                return Content(returnText);
            }
        }


        [Authorize(Roles = "View")]
        public ActionResult ViewEmployee(string EmployeeId)
        {
            ViewBag.Target = "View";
            ViewBag.Title = "View Employee";
            if (null == EmployeeId)
            {
                List<EmployeeDetail> employees = EmployeeDetailContext.Collection().OrderBy(e => e.FirstName).ToList();
                ViewBag.Employees = employees;
                return View("SelectEmployee");
            }
            EmployeeDetail employee = EmployeeDetailContext.Find(EmployeeId);
            if (null != employee)
            {
                ManipulateData manipulateData = new ManipulateData();
                DataViewModel viewModel = new DataViewModel()
                {
                    EmployeeDetails = employee,
                    DependentDetails = DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList(),
                    TelephoneExtensions = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.EmployeeId == employee.Id),
                    QualificationDetails = QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList(),
                    PastExperiences = PastExperienceContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                    PromotionDetails = PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                    PostingDetails = PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                };
                ViewBag.Vintage = "Not Available";
                if (employee.DateofJoiningDGH.HasValue)
                {
                    if(employee.WorkingStatus) ViewBag.Vintage = manipulateData.DateDifference(DateTime.Now.Date, employee.DateofJoiningDGH ?? DateTime.Now.Date);
                    else if(employee.DateOfSeparation.HasValue) ViewBag.Vintage = manipulateData.DateDifference(employee.DateOfSeparation ?? DateTime.Now.Date, employee.DateofJoiningDGH ?? DateTime.Now.Date);
                }
                return View(viewModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [Authorize(Roles = "Delete")]
        public ActionResult DeleteEmployee(string EmployeeId)
        {
            ViewBag.Target = "Delete";
            ViewBag.Title = "Delete Employee";
            if (null == EmployeeId)
            {
                List<EmployeeDetail> employees = EmployeeDetailContext.Collection().OrderBy(e => e.FirstName).ToList();
                ViewBag.Employees = employees;
                return View("SelectEmployee");
            }
            EmployeeDetail employee = EmployeeDetailContext.Find(EmployeeId);
            if (null != employee)
            {
                DataViewModel viewModel = new DataViewModel()
                {
                    EmployeeDetails = employee,
                    DependentDetails = DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList(),
                    TelephoneExtensions = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.EmployeeId == employee.Id),
                    QualificationDetails = QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList(),
                    PastExperiences = PastExperienceContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                    PromotionDetails = PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                    PostingDetails = PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                };
                return View("ViewEmployee", viewModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [Authorize(Roles = "Delete")]
        public ActionResult AjaxDelete(string EmployeeId)
        {
            string returnText = "<div class=\"alert alert-danger\" role=\"alert\"> An Error has occured </div>";
            EmployeeDetail employee = null != EmployeeId ? EmployeeDetailContext.Find(EmployeeId) : null;
            if (null != employee)
            {
                // Dependents
                var dependentsId = (from dependent in DependentDetailContext.Collection()
                                    where dependent.EmployeeId == employee.Id
                                    select dependent.Id).ToList() ?? new List<string>();
                foreach (var dependentId in dependentsId)
                {
                    DependentDetailContext.Delete(dependentId);
                }
                DependentDetailContext.Commit();

                // Telephone Extension
                TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.EmployeeId == employee.Id);
                if (null != telephoneExtension)
                {
                    TelephoneExtensionContext.Delete(telephoneExtension.Id);
                    TelephoneExtensionContext.Commit();
                }

                // Qualification Details
                var qualificationsId = (from qualification in QualificationDetailContext.Collection()
                                        where qualification.EmployeeId == employee.Id
                                        select qualification.Id).ToList() ?? new List<string>();
                foreach (var qualificationId in qualificationsId)
                {
                    QualificationDetailContext.Delete(qualificationId);
                }
                QualificationDetailContext.Commit();

                //Past Experiences
                var experiencesId = (from experience in PastExperienceContext.Collection()
                                        where experience.EmployeeId == employee.Id
                                        select experience.Id).ToList() ?? new List<string>();
                foreach (var experienceId in experiencesId)
                {
                    PastExperienceContext.Delete(experienceId);
                }
                PastExperienceContext.Commit();

                // Posting Details
                var postingsId = (from posting in PostingDetailContext.Collection()
                                     where posting.EmployeeId == employee.Id
                                     select posting.Id).ToList() ?? new List<string>();
                foreach (var postingId in postingsId)
                {
                    PostingDetailContext.Delete(postingId);
                }
                PostingDetailContext.Commit();

                //Promotion Details
                var promotionsId = (from promotion in PromotionDetailContext.Collection()
                                  where promotion.EmployeeId == employee.Id
                                  select promotion.Id).ToList() ?? new List<string>();
                foreach (var promotionId in promotionsId)
                {
                    PromotionDetailContext.Delete(promotionId);
                }
                PromotionDetailContext.Commit();

                // Finally Employee
                EmployeeDetailContext.Delete(employee.Id);
                EmployeeDetailContext.Commit();

                returnText = "<div class=\"alert alert-success\" role=\"alert\"> Successfully deleted Employee "+ employee.GetFullName +" </div>";
            }
            else
            {
                return HttpNotFound();
            }

            return Content(returnText);
        }

        [Authorize(Roles = "Add,Edit")]
        public JsonResult GetOrganisationDependentInfo(string organisationId, string infoType)
        {
            List<IdNamePair> idNamePairs = new List<IdNamePair>();
            switch (infoType)
            {
                case "payscale":
                    idNamePairs = (from payscale in PayScaleContext.Collection()
                                   where payscale.OrganisationId == organisationId
                                   orderby payscale.Scale
                                   select new IdNamePair
                                   {
                                       Id = payscale.Id,
                                       Name = payscale.Scale
                                   }).ToList();
                    break;
                case "level":
                    idNamePairs = LevelContext.Collection().Where(l =>
                    l.OrganisationId == organisationId).Select(l => new IdNamePair { Id = l.Id, Name = l.Name }).OrderBy(l => l.Name).ToList();
                    break;
                case "designation":
                    idNamePairs = (from designation in DesignationContext.Collection()
                                   where designation.OrganisationId == organisationId
                                   orderby designation.Name
                                   select new IdNamePair()
                                   {
                                       Id = designation.Id,
                                       Name = designation.Name
                                   }).ToList();
                    break;
                default: break;
            }
            return Json(idNamePairs, JsonRequestBehavior.AllowGet);
        }

        // Temporary class for passing generic Id & Name values via AJAX
        public class IdNamePair
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}