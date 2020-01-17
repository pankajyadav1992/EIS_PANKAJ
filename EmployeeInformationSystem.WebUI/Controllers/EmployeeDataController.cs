using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    public class EmployeeDataController : Controller
    {
        IRepository<EmployeeDetail> EmployeeDetailContext;
        IRepository<Discipline> DisciplineContext;
        IRepository<Designation> DesignationContext;
        IRepository<DependentDetail> DependentDetailContext;
        IRepository<Level> LevelContext;
        IRepository<Degree> DegreeContext;
        IRepository<HoD> HoDContext;
        IRepository<Organisation> OrganisationContext;
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
            PostingDetailContext = postingDetailContext;
            PayScaleContext = payScaleContext;
            PromotionDetailContext = promotionDetailContext;
            DepartmentContext = departmentContext;
            QualificationDetailContext = qualificationDetailContext;
            TelephoneExtensionContext = telephoneExtensionContext;
        }

        public ActionResult AddEmployee()
        {
            return View();
        }

        public ActionResult AjaxAdd(string targetPage)
        {
            object genericObject = null;
            List<SelectListItem> employeeType = new List<SelectListItem>();
            switch (targetPage)
            {
                case "Deputationist":
                    employeeType.Add(new SelectListItem() { Text = "Deputationist", Value = "0" });
                    genericObject = new DataViewModel()
                    {
                        EmployeeDetails = new EmployeeDetail()
                        {
                            MaritalStatus = MaritalStatus.Single,
                            WorkingStatus = true
                        },
                        EmployeeType = employeeType,
                        Degrees = DegreeContext.Collection(),
                        Organisations = OrganisationContext.Collection().Where(o => o.Name != "DGH"), // Remove DGH from list of organisation dropdowns
                        Disciplines = DisciplineContext.Collection(),
                        Designations = DesignationContext.Collection(),
                        Levels = LevelContext.Collection().Where(l => l.Organisation.Name == "DGH"), // This Passes only DGH Specific Levels
                        PayScales = PayScaleContext.Collection(),
                        Departments = DepartmentContext.Collection(),
                        HoDs = HoDContext.Collection() //Add proper support for determining current HoDs based on date
                    };
                    break;
                case "Consultant":
                    employeeType.Add(new SelectListItem() { Text = "Adviser", Value = "1" });
                    employeeType.Add(new SelectListItem() { Text = "Consultant", Value = "2" });
                    genericObject = new DataViewModel()
                    {
                        EmployeeDetails = new EmployeeDetail()
                        {
                            MaritalStatus = MaritalStatus.Single,
                            WorkingStatus = true
                        },
                        EmployeeType = employeeType,
                        Degrees = DegreeContext.Collection(),
                        Disciplines = DisciplineContext.Collection(),
                        Designations = DesignationContext.Collection().Where(d => d.Organisation.Name == "DGH"),
                        Levels = LevelContext.Collection().Where(l => l.Organisation.Name == "DGH"), //This Passes only DGH Specific Levels
                        PayScales = PayScaleContext.Collection().Where(p => p.Organisation.Name == "DGH"),
                        Departments = DepartmentContext.Collection(),
                        HoDs = HoDContext.Collection() //Add proper support for determining current HoDs based on date
                    };
                    break;
                case "Contractual":
                    return RedirectToAction("ViewEmployee", new { EmployeeId = "1f4f5af8-e961-4ac8-bbdd-886da0ed0c2d" });
                //break;
                default: break;
            }
            return View(targetPage, genericObject);
        }

        [HttpPost]
        public ActionResult AjaxAdd(string targetPage, DataViewModel viewModel, HttpPostedFileBase file)
        {
            ViewBag.addStatus = false;
            string returnText = "Error";
            string EmpId = null;
            switch (targetPage)
            {
                case "Deputationist":
                    if (!ModelState.IsValid)
                    {
                        String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                           .Select(v => v.ErrorMessage + " " + v.Exception));
                        returnText = "<div class=\"alert alert-danger\" role=\"alert\"> " + messages + " </div>";
                    }
                    else
                    {
                        //Join Aadhaar string
                        viewModel.EmployeeDetails.AadhaarNumer = viewModel.AadhaarPart1 + viewModel.AadhaarPart2 + viewModel.AadhaarPart3;
                        //Add profile photo
                        if (file != null)
                        {
                            viewModel.EmployeeDetails.ProfilePhoto = viewModel.EmployeeDetails.Id + Path.GetExtension(file.FileName);
                            file.SaveAs(Server.MapPath("//Content//profile_pics//") + viewModel.EmployeeDetails.ProfilePhoto);
                        }
                        //Insert employee first
                        EmployeeDetailContext.Insert(viewModel.EmployeeDetails);
                        EmpId = viewModel.EmployeeDetails.Id;
                        EmployeeDetailContext.Commit();
                        //Dependents next
                        if (null != viewModel.DependentDetails) //BUG FIX: foreach fails on empty lists
                        {
                            foreach (DependentDetail dependent in viewModel.DependentDetails)
                            {
                                dependent.EmployeeId = EmpId;
                                DependentDetailContext.Insert(dependent);
                            }
                            DependentDetailContext.Commit();
                        }
                        //Telephone Extension 
                        //Check if exists
                        TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.Number == viewModel.TelephoneExtensions.Number);
                        viewModel.TelephoneExtensions.EmployeeId = EmpId;
                        if (null == telephoneExtension && 9999 != viewModel.TelephoneExtensions.Number)
                        //Incase the telephone number is NULL, Model state is valid but it goes forward and doesn't insert anything
                        {
                            TelephoneExtensionContext.Insert(viewModel.TelephoneExtensions);
                        }
                        else if (null != telephoneExtension)
                        {
                            telephoneExtension.EmployeeId = viewModel.TelephoneExtensions.EmployeeId;
                            telephoneExtension.CurrentOwner = null;
                        }
                        TelephoneExtensionContext.Commit();
                        // Qualification Details
                        if (null != viewModel.QualificationDetails) //BUG FIX: foreach fails on empty lists
                        {
                            foreach (QualificationDetail qualification in viewModel.QualificationDetails)
                            {
                                qualification.EmployeeId = EmpId;
                                QualificationDetailContext.Insert(qualification);
                            }
                            QualificationDetailContext.Commit();
                        }
                        //Promotion Details
                        if (null != viewModel.PromotionDetails) //BUG FIX: foreach fails on empty lists
                        {
                            foreach (PromotionDetail promotion in viewModel.PromotionDetails)
                            {
                                promotion.EmployeeId = EmpId;
                                PromotionDetailContext.Insert(promotion);
                            }
                            PromotionDetailContext.Commit();
                        }
                        //Posting Details
                        if (null != viewModel.PostingDetails) //BUG FIX: foreach fails on empty lists
                        {
                            foreach (PostingDetail posting in viewModel.PostingDetails)
                            {
                                posting.EmployeeId = EmpId;
                                PostingDetailContext.Insert(posting);
                            }
                            PostingDetailContext.Commit();
                        }
                        ViewBag.addStatus = true;
                        ViewBag.EmployeeCode = viewModel.EmployeeDetails.EmployeeCode;
                        ViewBag.EmployeeId = EmpId;
                    }
                    break;
                case "Consultant": break;
                case "Contractual": break;
                default: break;
            }
            if (ViewBag.addStatus)
            {
                return View("AddSuccess");
            }
            return Content(returnText);
        }

        public ActionResult ViewEmployee(string EmployeeId)
        {
            if (null == EmployeeId) { EmployeeId = "c3361310-47f8-40ad-a046-833266bdef38"; }
            EmployeeDetail employee = EmployeeDetailContext.Find(EmployeeId);
            if (null != employee)
            {
                DataViewModel viewModel = new DataViewModel()
                {
                    EmployeeDetails = employee,
                    DependentDetails = DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList(),
                    TelephoneExtensions = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.EmployeeId == employee.Id),
                    QualificationDetails = QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList(),
                    PromotionDetails = PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList(),
                    PostingDetails = PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                };
                return View(viewModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public JsonResult GetOrganisationDependentInfo(string organisationId, string infoType)
        {
            List<IdNamePair> idNamePairs = new List<IdNamePair>();
            switch (infoType)
            {
                case "payscale":
                    idNamePairs = (from payscale in PayScaleContext.Collection()
                                   where payscale.OrganisationId == organisationId
                                   select new IdNamePair
                                   {
                                       Id = payscale.Id,
                                       Name = payscale.Scale
                                   }).ToList();
                    break;
                case "level":
                    idNamePairs = LevelContext.Collection().Where(l =>
                    l.OrganisationId == organisationId).Select(l => new IdNamePair { Id = l.Id, Name = l.Name }).ToList();
                    break;
                case "designation":
                    idNamePairs = (from designation in DesignationContext.Collection()
                                   where designation.OrganisationId == organisationId
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

        // Placeholder class for passing generic Id & Name values via AJAX
        public class IdNamePair
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}