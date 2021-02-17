using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    public class LeaveController : BaseController
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
        IRepository<EmployeeAsHoD> EmployeeAsHoDDetailContext;
        IRepository<LeaveType> LeaveTypeContext;

        public LeaveController(IRepository<EmployeeDetail> employeeDetailContext,
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
        IRepository<TelephoneExtension> telephoneExtensionContext,
        IRepository<EmployeeAsHoD> employeeAsHoDDetailContext,
        IRepository<LeaveType> leaveTypeContext
        )
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

            EmployeeAsHoDDetailContext = employeeAsHoDDetailContext;

            LeaveTypeContext = leaveTypeContext;
            //Setting Parameters for Page

            base.SetGlobalParameters();
            ViewBag.UserName = UserName;
            ViewBag.ProfilePhoto = ProfilePicture;
            ViewBag.Role = Role;
        }

        // GET: Leave
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeaveMaster()
        {
           List<Organisation> orgList =  OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.OrganisationList = orgList;
           
            List<LeaveType> LeaveType = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.LevelTypeList = LeaveType;


            return View();
        }



        public ActionResult LeaveType()
        {
            return View("LeaveType");
        }

        [HttpPost]
        public ActionResult AddLeaveType(LeaveType lt)
        {
            if (ModelState.IsValid)
            {
                LeaveTypeContext.Insert(lt);
                
                LeaveTypeContext.Commit();
      

            }
            return View("LeaveType");
        }
    }
}