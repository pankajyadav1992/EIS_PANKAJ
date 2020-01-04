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
                case "Consultant": break;
                case "Contractual": break;
                default: break;
            }
            return View(targetPage, genericObject);
        }

        [HttpPost]
        public ActionResult AjaxAdd(string targetPage, DataViewModel viewModel)
        {
            string returnText = "Error";
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
                        EmployeeDetailContext.Commit();
                        //Dependents next
                        foreach(DependentDetail dependent in viewModel.DependentDetails)
                        {
                            DependentDetailContext.Insert(dependent);
                        }
                        DependentDetailContext.Commit();
                        //Telephone Extension 
                        //Check if exists
                        TelephoneExtension telephoneExtension = TelephoneExtensionContext.Collection().FirstOrDefault(t => t.Number == viewModel.TelephoneExtensions.Number);
                        if (null == telephoneExtension)
                        {
                            TelephoneExtensionContext.Insert(viewModel.TelephoneExtensions);
                        }
                        else
                        {
                            telephoneExtension.EmployeeId = viewModel.TelephoneExtensions.EmployeeId;
                            telephoneExtension.CurrentOwner = null;
                        }
                        TelephoneExtensionContext.Commit();
                        // Qualification Details
                        foreach (QualificationDetail qualification in viewModel.QualificationDetails)
                        {
                            QualificationDetailContext.Insert(qualification);
                        }
                        QualificationDetailContext.Commit();
                        //Promotion Details
                        foreach (PromotionDetail promotion in viewModel.PromotionDetails)
                        {
                            PromotionDetailContext.Insert(promotion);
                        }
                        PromotionDetailContext.Commit();
                        //Posting Details
                        foreach (PostingDetail posting in viewModel.PostingDetails)
                        {
                            PostingDetailContext.Insert(posting);
                        }
                        PostingDetailContext.Commit();
                        returnText = "Success";
                    }
                    break;
                case "Consultant": break;
                case "Contractual": break;
                default: break;
            }
            return Content(returnText);
        }
    }
}