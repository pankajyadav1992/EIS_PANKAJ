using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using System;
using System.Collections.Generic;
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

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult AjaxAdd(string targetPage)
        {
            object genericObject = null;
            switch (targetPage)
            {
                case "Deputationist":
                    genericObject = new DataViewModel()
                    {
                        EmployeeDetails = new EmployeeDetail()
                        {
                            MaritalStatus = MaritalStatus.Single,
                            WorkingStatus = true
                        },
                        Degrees = DegreeContext.Collection(),
                        Organisations = OrganisationContext.Collection(),
                        Disciplines = DisciplineContext.Collection(),
                        Designations = DesignationContext.Collection(),
                        Levels = LevelContext.Collection(), // Add proper support for separating levels
                        PayScales = PayScaleContext.Collection(),
                        Departments = DepartmentContext.Collection(),
                        HoDs = HoDContext.Collection() //Add proper support for determining HoDs
                    };
                    break;
                case "Consultant":
                    ViewBag.addStatus = true;
                    ViewBag.EmployeeCode = "125118";
                    targetPage = "AddSuccess";
                    break;
                case "Contractual":
                    return RedirectToAction("ViewEmployee", new { EmployeeId = "1f4f5af8-e961-4ac8-bbdd-886da0ed0c2d" });
                    //break;
                default: break;
            }
            return View(targetPage, genericObject);
        }

        [HttpPost]
        public ActionResult AjaxAdd(string targetPage, DataViewModel viewModel)
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
                        //Insert employee first
                        EmployeeDetailContext.Insert(viewModel.EmployeeDetails);
                        EmpId = viewModel.EmployeeDetails.Id;
                        EmployeeDetailContext.Commit();
                        //Dependents next
                        if (null != viewModel.DependentDetails)
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
                        if (null != viewModel.QualificationDetails)
                        {
                            foreach (QualificationDetail qualification in viewModel.QualificationDetails)
                            {
                                qualification.EmployeeId = EmpId;
                                QualificationDetailContext.Insert(qualification);
                            }
                            QualificationDetailContext.Commit();
                        }
                        //Promotion Details
                        if (null != viewModel.PromotionDetails)
                        {
                            foreach (PromotionDetail promotion in viewModel.PromotionDetails)
                            {
                                promotion.EmployeeId = EmpId;
                                PromotionDetailContext.Insert(promotion);
                            }
                            PromotionDetailContext.Commit();
                        }
                        //Posting Details
                        if (null != viewModel.PostingDetails)
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
            EmployeeId = "1f4f5af8-e961-4ac8-bbdd-886da0ed0c2d";
            EmployeeDetail employee = EmployeeDetailContext.Find(EmployeeId);
            if(null != employee)
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
    }
}