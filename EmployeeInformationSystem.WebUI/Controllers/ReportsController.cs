using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using EmployeeInformationSystem.Services;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    [Authorize(Roles = "Reports")]
    public class ReportsController : BaseController
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

        public ReportsController(IRepository<EmployeeDetail> employeeDetailContext,
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

        // GET: Reports

        public ActionResult ActiveEmployees()
        {
            ViewBag.Title = "Active Employees Report";
            return View("ReportSelection", GetViewModel());
        }

        [HttpPost]
        public ActionResult ActiveEmployees(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportSelection", reportSelection);
            }
            else
            {
                ViewBag.Title = "Active Employees Report";
                List<EmployeeDetail> employees = new List<EmployeeDetail>();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                                                        .Where(e => e.WorkingStatus && reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                                                        .ToList()
                                                                                                 join posting in PostingDetailContext.Collection()
                                                                                                                                     .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                                                     .ToList()
                                                                                                                                      on employee.Id equals posting.EmployeeId
                                                                                                 orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                                                 select employee).Distinct().ToList();
                else if (!reportSelection.From.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                            .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                            .ToList()
                                                                      join posting in PostingDetailContext.Collection()
                                                                                                          .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                          .ToList()
                                                                                                           on employee.Id equals posting.EmployeeId
                                                                      orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                      where ((employee.WorkingStatus && employee.DateofJoiningDGH <= reportSelection.To) ||
                                                                      (!employee.WorkingStatus && employee.DateofLeavingDGH >= reportSelection.To)) &&
                                                                      (posting.From <= reportSelection.To || !posting.From.HasValue)
                                                                      select employee).Distinct().ToList();
                else if (!reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                              .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                              .ToList()
                                                                    join posting in PostingDetailContext.Collection()
                                                                                                        .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                        .ToList()
                                                                                                         on employee.Id equals posting.EmployeeId
                                                                    orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                    where employee.WorkingStatus && employee.DateofJoiningDGH >= reportSelection.From &&
                                                                    (posting.From >= reportSelection.From || !posting.From.HasValue)
                                                                    select employee).Distinct().ToList();
                else employees = (from employee in EmployeeDetailContext.Collection()
                                                                        .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                        .ToList()
                                  join posting in PostingDetailContext.Collection()
                                                                       .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                       .ToList()
                                                                        on employee.Id equals posting.EmployeeId
                                  orderby employee.FirstName, employee.MiddleName, employee.LastName
                                  where employee.DateofJoiningDGH <= reportSelection.To && (employee.WorkingStatus || (!employee.WorkingStatus && employee.DateofLeavingDGH > reportSelection.To)) &&
                                  (posting.To >= reportSelection.From || !posting.To.HasValue)
                                  select employee).Distinct().ToList();

                DataTable dt_ = GetDataTable(employees, reportSelection);
                return View("GeneratedReportView", dt_);
            }
        }

        public ActionResult InActiveEmployees()
        {
            ViewBag.Title = "InActive Employees Report";
            return View("ReportSelection", GetViewModel());
        }

        [HttpPost]
        public ActionResult InActiveEmployees(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportSelection", reportSelection);
            }
            else
            {
                ViewBag.Title = "InActive Employees Report";
                List<EmployeeDetail> employees = new List<EmployeeDetail>();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                                                        .Where(e => !e.WorkingStatus && reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                                                        .ToList()
                                                                                                 join posting in PostingDetailContext.Collection()
                                                                                                                                     .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                                                     .ToList()
                                                                                                                                      on employee.Id equals posting.EmployeeId
                                                                                                 orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                                                 select employee).Distinct().ToList();
                else if (!reportSelection.From.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                            .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                            .ToList()
                                                                      join posting in PostingDetailContext.Collection()
                                                                                                          .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                          .ToList()
                                                                                                           on employee.Id equals posting.EmployeeId
                                                                      orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                      where !employee.WorkingStatus && employee.DateofLeavingDGH <= reportSelection.To &&
                                                                      (posting.From <= reportSelection.To || !posting.From.HasValue)
                                                                      select employee).Distinct().ToList();
                else if (!reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                              .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                              .ToList()
                                                                    join posting in PostingDetailContext.Collection()
                                                                                                        .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                        .ToList()
                                                                                                         on employee.Id equals posting.EmployeeId
                                                                    orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                    where !employee.WorkingStatus &&
                                                                    (employee.DateofLeavingDGH >= reportSelection.From || (!employee.DateofLeavingDGH.HasValue && employee.DateofJoiningDGH > reportSelection.From)) &&
                                                                    (posting.From >= reportSelection.From || !posting.From.HasValue)
                                                                    select employee).Distinct().ToList();
                else employees = (from employee in EmployeeDetailContext.Collection()
                                                                        .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                        .ToList()
                                  join posting in PostingDetailContext.Collection()
                                                                       .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                       .ToList()
                                                                        on employee.Id equals posting.EmployeeId
                                  orderby employee.FirstName, employee.MiddleName, employee.LastName
                                  where !employee.WorkingStatus && (employee.DateofLeavingDGH >= reportSelection.From && employee.DateofLeavingDGH <= reportSelection.To) &&
                                  ((posting.From >= reportSelection.From && posting.From <= reportSelection.From) || !posting.From.HasValue)
                                  select employee).Distinct().ToList();

                DataTable dt_ = GetDataTable(employees, reportSelection);

                return View("GeneratedReportView", dt_);
            }
        }

        public ActionResult AllEmployees()
        {
            ViewBag.Title = "All Employees Report";
            return View("ReportSelection", GetViewModel());
        }

        [HttpPost]
        public ActionResult AllEmployees(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View("ReportSelection", reportSelection);
            }
            else
            {
                ViewBag.Title = "All Employees Report";
                List<EmployeeDetail> employees = new List<EmployeeDetail>();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                                                        .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                                                        .ToList()
                                                                                                 join posting in PostingDetailContext.Collection()
                                                                                                                                     .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                                                     .ToList()
                                                                                                                                      on employee.Id equals posting.EmployeeId
                                                                                                 orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                                                 select employee).Distinct().ToList();
                else if (!reportSelection.From.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                            .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                            .ToList()
                                                                      join posting in PostingDetailContext.Collection()
                                                                                                          .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                          .ToList()
                                                                                                           on employee.Id equals posting.EmployeeId
                                                                      orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                      where (employee.DateofJoiningDGH <= reportSelection.To || (!employee.WorkingStatus && employee.DateofLeavingDGH < reportSelection.To)) &&
                                                                      (posting.From <= reportSelection.To || (!posting.From.HasValue && posting.To < reportSelection.To))
                                                                      select employee).Distinct().ToList();
                else if (!reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                              .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                                                              .ToList()
                                                                    join posting in PostingDetailContext.Collection()
                                                                                                        .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                        .ToList()
                                                                                                         on employee.Id equals posting.EmployeeId
                                                                    orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                    where employee.DateofJoiningDGH >= reportSelection.From &&
                                                                    (posting.From >= reportSelection.From || !posting.From.HasValue)
                                                                    select employee).Distinct().ToList();
                else employees = (from employee in EmployeeDetailContext.Collection()
                                                                        .Where(e => reportSelection.Categories.Contains(e.EmployeeType))
                                                                        .ToList()
                                  join posting in PostingDetailContext.Collection()
                                                                       .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                       .ToList()
                                                                        on employee.Id equals posting.EmployeeId
                                  orderby employee.FirstName, employee.MiddleName, employee.LastName
                                  where (employee.DateofJoiningDGH >= reportSelection.From && employee.DateofJoiningDGH <= reportSelection.To) &&
                                  ((posting.From >= reportSelection.From || !posting.From.HasValue) && (posting.To <= reportSelection.To || !posting.To.HasValue))
                                  select employee).Distinct().ToList();

                DataTable dt_ = GetDataTable(employees, reportSelection);

                return View("GeneratedReportView", dt_);
            }
        }

        public ActionResult CustomReports()
        {
            ViewBag.Title = "Custom Reports";
            return View();
        }

        public ActionResult SelectCustomReport(string targetPage)
        {
            return PartialView(targetPage, GetViewModel(targetPage));
        }
        //vaibhav
        [HttpPost]
        public ActionResult PromotionReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null; 
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                  join org in OrganisationContext.Collection().ToList()
                                //// where(employee.OrganisationId == org.Id)
                                on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.Title + ' ' + employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      PromotionDetails = ManPowerEtraDetai("Promotion Details", employee, reportSelection),

                                     
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      employee.DateofJoiningDGH,
                                      employee.DateofLeavingDGH,
                                      employee.ReasonForLeaving,
                                      employee.WorkingStatus,
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,

                                  }).Distinct().Where(x => x.EmployeeTypeId==EmployeeType.Deputationist
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {

                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)) :
                     (x.DateofJoiningDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To))).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.DateofLeavingDGH >= reportSelection.From) :
                     (x.DateofJoiningDGH >= reportSelection.From)).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x =>
                      reportSelection?.Working == "separated" ? (x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To) :
                       (x.DateofJoiningDGH >= reportSelection.From && x.DateofJoiningDGH <= reportSelection.To)).ToList();
                    dt_ = ToDataTable(emp);
                }

                dt_ = ToDataTable(employees1);
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");

                return View("GeneratedReportView", dt_);
            }
           
        }


        [HttpPost]
        public ActionResult Quali_Exp_Pay_Report(ReportSelectionViewModel reportSelection)
        {

            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
               
                DataTable dt_ = null;
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                  join org in OrganisationContext.Collection().ToList()
                                //// where(employee.OrganisationId == org.Id)
                                on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.Title + ' ' + employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      QualificationDetails = ManPowerEtraDetai("Qualification Details", employee, reportSelection),

                                      CurrentBasicPay = employee.CurrentBasicPay,
                                      PastExperience = employee.OrganisationId != null ? ManPowerEtraDetai("Past Experience", employee, reportSelection) : "",

                                      employee.WorkingStatus,
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();
                dt_ = ToDataTable(employees1);
                return View("GeneratedReportView", dt_);
            }
        }

        [HttpPost]
        public ActionResult Past_Emp_Report(ReportSelectionViewModel reportSelection)
            {

            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                  join org in OrganisationContext.Collection().ToList()
                                //// where(employee.OrganisationId == org.Id)
                                on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.Title + ' ' + employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      employee.DateofJoiningDGH,
                                      employee.DateofLeavingDGH,
                                      employee.ReasonForLeaving,
                                      employee.WorkingStatus,
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(x=>x.WorkingStatus == false && x.EmployeeTypeId!=EmployeeType.Deputationist).ToList();
                dt_ = ToDataTable(employees1);
                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofJoiningDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofJoiningDGH < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofJoiningDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofJoiningDGH >= reportSelection.From && x.DateofJoiningDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                return View("GeneratedReportView", dt_);
            }
        }
        [HttpPost]
        public ActionResult TenureReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                 
                                  join org in OrganisationContext.Collection().ToList()
                                  //// where(employee.OrganisationId == org.Id)
                                  on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.Title + ' ' + employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),

                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      employee.DateofJoiningDGH,
                                      employee.DateOfSuperannuation,
                                      employee.ReasonForLeaving,
                                      employee.DateofLeavingDGH,
                                      employee.DeputationPeriod,
                                      employee.WorkingStatus,
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,
                                  })
                .Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString()) 
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();


                dt_ = ToDataTable(employees1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation <= reportSelection.To || (!x.WorkingStatus && x.DateOfSuperannuation < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation >= reportSelection.From && x.DateOfSuperannuation <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                return View("GeneratedReportView", dt_);
            }
        }
        [HttpPost]
        public ActionResult SuperannuationReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;
                var orgnullcheck = reportSelection.Organisation.Contains("-1");
                var levelnullcheck = reportSelection.Level.Contains("-1");
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                
                                  //join disci in DisciplineContext.Collection().ToList()
                                  //  on employee.DisciplineId equals disci.Id into yy
                                  //from y1 in yy.DefaultIfEmpty()
                                  join level in LevelContext.Collection().ToList()
                                       on employee.LevelId equals level.Id into yy1
                                  from y2 in yy1.DefaultIfEmpty()
                                  join org in OrganisationContext.Collection().ToList()
                                  // where(employee.OrganisationId == org.Id)
                                  on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.Title + ' ' + employee.FirstName+ " " + (employee.MiddleName==""?"":employee.MiddleName+" ")  + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      employee.DateofJoiningDGH,
                                      employee.DateOfSuperannuation,
                                      employee.DateofLeavingDGH,
                                      employee.ReasonForLeaving,
                                      employee.DeputationPeriod,
                                      employee.WorkingStatus,
                                      employee.LevelId,
                                      employee.OrganisationId,
                                  })
                .Distinct().Where(x => (reportSelection.Organisation.Contains(x.OrganisationId) || (orgnullcheck==true?x.OrganisationId is null : x.OrganisationId == "-1"))  && 
              (reportSelection.Level.Contains(x.LevelId) || (levelnullcheck==true?x.LevelId is null: x.LevelId== "-1"))
               &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();


                dt_ = ToDataTable(employees1);
               
                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation <= reportSelection.To || (!x.WorkingStatus && x.DateOfSuperannuation < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateOfSuperannuation >= reportSelection.From && x.DateOfSuperannuation <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                return View("GeneratedReportView", dt_);
            }
        }
        public string ManPowerEtraDetai(string column,EmployeeDetail employee, ReportSelectionViewModel reportSelection)
        {
            string columdetail = "";
            switch (column)
            {
                case "Qualification Details":
                    string[] qualifications = (from qualification in QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList()
                                               where !qualification.From.HasValue || ((qualification.From >= reportSelection.From || !reportSelection.From.HasValue) && (qualification.From <= reportSelection.To || !reportSelection.To.HasValue))
                                               select "Degree:" + (!string.IsNullOrEmpty(qualification.Duration) ? qualification.Duration + " " : "") + qualification.Degree.Name +
                                               (!string.IsNullOrEmpty(qualification.Specialization) ? "(" + qualification.Specialization + ")" : "") +
                                               (!string.IsNullOrEmpty(qualification.Grade) ? ", Grade/Percentage: " + qualification.Grade : "") +
                                               (!string.IsNullOrEmpty(qualification.Class) ? ", Class: " + qualification.Class : "") +
                                               (!string.IsNullOrEmpty(qualification.Institution) ? ", Institution: " + qualification.Institution : "") +
                                               (!string.IsNullOrEmpty(qualification.University) ? ", University: " + qualification.University : "") +
                                               (qualification.From.HasValue ? ", From: " + qualification.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                               (qualification.To.HasValue ? ", To: " + qualification.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                                   ).ToArray();
                    columdetail=String.Join("NUMBER", qualifications);
                    break;
                case "Vintage":
                    columdetail = GetCustomVintage(employee, reportSelection.From, reportSelection.To);
                    break;
                case "Designation":
                    string designation = (from promotion in PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                          group promotion by promotion.EmployeeId into p
                                          select p.OrderByDescending(l => l.From).FirstOrDefault().Designation.Name).SingleOrDefault();
                    columdetail = designation;
                    break;
                case "Department":
                    string department = (from posting in PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                         group posting by posting.EmployeeId into p
                                         select p.OrderByDescending(l => l.From).FirstOrDefault().Department.Name).SingleOrDefault();
                    columdetail = department;
                    break;
                case "Dependent Details":
                    string[] dependents = (from depedent in DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList()
                                           where !depedent.DateofBirth.HasValue || ((depedent.DateofBirth >= reportSelection.From || !reportSelection.From.HasValue) && (depedent.DateofBirth <= reportSelection.To || !reportSelection.To.HasValue))
                                           select "Name: " + depedent.DependentName
                                           + (depedent.DateofBirth.HasValue ? ", DOB: " + depedent.DateofBirth.Value.ToString("dd'-'MM'-'yyyy") : "")
                                           + ", Relation: " + depedent.Relationship).ToArray();
                    columdetail = String.Join("NUMBER", dependents);
                    break;
                case "Telephone Extension":
                    TelephoneExtension extension = TelephoneExtensionContext.Collection().Where(t => t.EmployeeId == employee.Id)
                                                                                                            .FirstOrDefault();
                    columdetail = extension==null?"": extension.Number.ToString();
                    break;
                case "Promotion Details":
                    string[] promotions = (from promotion in PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                           where !promotion.From.HasValue || ((promotion.From >= reportSelection.From || !reportSelection.From.HasValue) && (promotion.From <= reportSelection.To || !reportSelection.To.HasValue))
                                           select "Designation: " + promotion.Designation.Name +
                                           (!string.IsNullOrEmpty(promotion.PayScaleId) ? ", PayScale: " + promotion.PayScale.Scale : "") +
                                           (!string.IsNullOrEmpty(promotion.LevelId) ? ", Level: " + promotion.Level.Name : "") +
                                           (promotion.From.HasValue ? ", From: " + promotion.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                               (promotion.To.HasValue ? ", To: " + promotion.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                           ).ToArray();
                    columdetail = String.Join("NUMBER", promotions);
                    break;
                case "Pay Scale":
                    string[] payscale =(from pay in PayScaleContext.Collection().Where(x => x.OrganisationId==employee.OrganisationId).ToList()
                                           select pay.Scale).ToArray();

                    columdetail = String.Join("NUMBER", payscale);
                    break;
                case "Past Experience":
                    string[] past_exp = (from exp in PastExperienceContext.Collection().Where(x => x.EmployeeId == employee.Id).ToList()
                                         select (!string.IsNullOrEmpty(exp.Position)?
                                         "Postion: " + exp.Position:"")+
                                         (!string.IsNullOrEmpty(exp.Organisation) ? ", Organisation: " + exp.Organisation : "")+
                                            (exp.From.HasValue ? ", From: " + exp.From : "")+
                                             (exp.To.HasValue ? ", To: " + exp.To : "")
                                         ).ToArray();

                    columdetail = String.Join("NUMBER", past_exp);
                    break;
                //case "Department":
                //    var department = (from dept in PostingDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList()
                //                           select dept.Department );

                //    columdetail = String.Join("NUMBER", department.Select(x=>x.Name));
                //    break;
                //case "Designation  
                //    var desigation= (from promo in PromotionDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList()
                //                     select promo.Designation);
                //    columdetail = String.Join("NUMBER", desigation.Select(x => x.Name));
                //    break;
                case "Posting Details":
                    string[] postings = (from posting in PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                         where !posting.From.HasValue || ((posting.From >= reportSelection.From || !reportSelection.From.HasValue) && (posting.From <= reportSelection.To || !reportSelection.To.HasValue))
                                         select "Department: " + posting.Department.Name +
                                         (!string.IsNullOrEmpty(posting.HODId) ? ", HoD: " + posting.HOD.Designation : "") +
                                         (!string.IsNullOrEmpty(posting.Reporting) ? ", Reporting: " + posting.Reporting : "") +
                                         (posting.From.HasValue ? ", From: " + posting.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                             (posting.To.HasValue ? ", To: " + posting.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                           ).ToArray();
                    columdetail = String.Join("NUMBER", postings);
                    break;

            }
            return columdetail;
            }
            [HttpPost]
        public ActionResult ManPowerReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
             else
                {
                ViewBag.Title = "Man Power Report";
                DataTable dt_ = null;
                var type = reportSelection.CustomColumns.ToList();
               // var status = reportSelection?.Working == "working" ? true : reportSelection?.Working == "separated" ? false : null;
                // List<EmployeeDetail> employees = new List<EmployeeDetail>();
                
                    var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                      //join dept in PostingDetailContext.Collection().ToList()
                                      //    on employee.Id equals dept.EmployeeId into xx1
                                      //from x1 in xx1.DefaultIfEmpty()
                                      //join promo in PromotionDetailContext.Collection().ToList()
                                      //  on employee.Id equals promo.EmployeeId into xx2
                                      //from x2 in xx2.DefaultIfEmpty()
                                      join org in OrganisationContext.Collection().ToList()
                                     // where(employee.OrganisationId == org.Id)
                                     on employee.OrganisationId equals org.Id into xx
                                      //into xx
                                      from y in xx.DefaultIfEmpty()
                                      join disci in DisciplineContext.Collection().ToList()
                                      on employee.DisciplineId equals disci.Id into yy
                                      from y1 in yy.DefaultIfEmpty()
                                      join level in LevelContext.Collection().ToList()
                                        on employee.LevelId equals level.Id into yy1
                                      from y2 in yy1.DefaultIfEmpty()

                                      select new
                                      {
                                          employee.EmployeeCode,
                                          FullName = employee.Title + ' ' + employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                          EmployeeType=employee.EmployeeType.GetDisplayName(),
                                          Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                          Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                          employee.AadhaarNumber,
                                          BloodGroup=employee.BloodGroup.GetDisplayName(),
                                          QualificationDetails= ManPowerEtraDetai("Qualification Details",employee,reportSelection),
                                          PromotionDetails= ManPowerEtraDetai("Promotion Details", employee, reportSelection),
                                          PostingDetails = ManPowerEtraDetai("Posting Details", employee, reportSelection),
                                          DependentDetails = ManPowerEtraDetai("Dependent Details", employee, reportSelection),
                                          TelephoneExtension = ManPowerEtraDetai("Telephone Extension", employee, reportSelection),
                                          Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                          Organisation =y == null ?"": y.Name ,
                                          employee.DateOfBirth,
                                          employee.DateOfSuperannuation,
                                          employee.DateofJoiningParentOrg,
employee.DateofRelievingLastOffice,employee.DateofJoiningDGH,employee.DateofLeavingDGH,employee.ReasonForLeaving,employee.DeputationPeriod,
                                          SeatingLocation= employee.SeatingLocation.GetDisplayName(),
                                       
employee.MobileNumber,employee.ResidenceNumber,employee.ResidenceAddress,employee.PermanentAddress,employee.EmailID,
employee.ProfilePhoto,employee.WorkingStatus,
employee.Gender,
                                          Discipline=y1==null?"":y1.Name,
                                          employee.PrimaryExpertise,
                                          Level=y2==null?"":y2.Name ,
                                          employee.CurrentBasicPay,employee.PANNumber,
employee.PassportNumber,employee.PassportValidity,employee.VehicleNumber,employee.MaritalStatus,employee.MarriageDate,
employee.AlternateEmailID,employee.EmergencyPerson,employee.EmergencyContact,employee.UANNumber,
                                          DeputedLocation = employee.DeputedLocation.GetDisplayName(),
                                          employee.CreatedAt,
employee.LastUpdateAt,employee.LastUpdateBy,employee.VehicleType,
                                          VehicleCategory=employee.VehicleCategory.GetDisplayName(),employee.EmergencyRelation,
                                          EmployeeTypeId = employee.EmployeeType,
                                      }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                      (reportSelection?.Working == "working" ?  p.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : p.WorkingStatus == true || p.WorkingStatus == false)
                       
).ToList();

                
                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue) {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                   
                var emp= employees1.Where(x =>
                reportSelection?.Working == "separated"?(x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)):
                (x.DateofJoiningDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To))).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                  var emp=  employees1.Where(x =>
                   reportSelection?.Working == "separated" ?(x.DateofLeavingDGH >= reportSelection.From):
                   (x.DateofJoiningDGH >= reportSelection.From)).ToList();
                    dt_ = ToDataTable(emp);
                }
                else 
                {
                  var emp= employees1.Where(x =>
                   reportSelection?.Working == "separated" ? (x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To):
                    (x.DateofJoiningDGH >= reportSelection.From && x.DateofJoiningDGH <= reportSelection.To)).ToList();
                    dt_ = ToDataTable(emp);
                }

                dt_.Columns.Remove("EmployeeTypeId");

                return View("GeneratedReportView", dt_);
            }
        }

            [HttpPost]
        public ActionResult DeputationistVintageReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                ViewBag.Title = "Deputationist Vintage Report";
                List<EmployeeDetail> employees = new List<EmployeeDetail>();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                                                        .Where(e => e.EmployeeType == EmployeeType.Deputationist)
                                                                                                                                        .ToList()
                                                                                                 join posting in PostingDetailContext.Collection()
                                                                                                                                     .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                                                     .ToList()
                                                                                                                                      on employee.Id equals posting.EmployeeId
                                                                                                 orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                                                 select employee).Distinct().ToList();
                else if (!reportSelection.From.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                            .Where(e => e.EmployeeType == EmployeeType.Deputationist)
                                                                                                            .ToList()
                                                                      join posting in PostingDetailContext.Collection()
                                                                                                          .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                          .ToList()
                                                                                                           on employee.Id equals posting.EmployeeId
                                                                      orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                      where (employee.DateofJoiningDGH <= reportSelection.To || (!employee.WorkingStatus && employee.DateofLeavingDGH < reportSelection.To)) &&
                                                                      (posting.From <= reportSelection.To || (!posting.From.HasValue && posting.To < reportSelection.To))
                                                                      select employee).Distinct().ToList();
                else if (!reportSelection.To.HasValue) employees = (from employee in EmployeeDetailContext.Collection()
                                                                                                              .Where(e => e.EmployeeType == EmployeeType.Deputationist)
                                                                                                              .ToList()
                                                                    join posting in PostingDetailContext.Collection()
                                                                                                        .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                                                        .ToList()
                                                                                                         on employee.Id equals posting.EmployeeId
                                                                    orderby employee.FirstName, employee.MiddleName, employee.LastName
                                                                    where employee.DateofJoiningDGH >= reportSelection.From &&
                                                                    (posting.From >= reportSelection.From || !posting.From.HasValue)
                                                                    select employee).Distinct().ToList();
                else employees = (from employee in EmployeeDetailContext.Collection()
                                                                        .Where(e => e.EmployeeType == EmployeeType.Deputationist)
                                                                        .ToList()
                                  join posting in PostingDetailContext.Collection()
                                                                       .Where(p => reportSelection.Departments.Contains(p.DepartmentId))
                                                                       .ToList()
                                                                        on employee.Id equals posting.EmployeeId
                                  orderby employee.FirstName, employee.MiddleName, employee.LastName
                                  where (employee.DateofJoiningDGH >= reportSelection.From && employee.DateofJoiningDGH <= reportSelection.To) &&
                                  ((posting.From >= reportSelection.From || !posting.From.HasValue) && (posting.To <= reportSelection.To || !posting.To.HasValue))
                                  select employee).Distinct().ToList();
              //  reportSelection.CustomColumns.
                DataTable dt_ = GetDataTable(employees, reportSelection);

                return View("GeneratedReportView", dt_);
            }
        }
       
            private DataTable GetDataTable(List<EmployeeDetail> employees, ReportSelectionViewModel reportSelection)
        {
            DataTable dt_ = new DataTable("Report");
            DataColumn dtColumn;
            DataRow dataRow;
            IEnumerable<string> allColumns = new List<string>();

            if (null != reportSelection.PersonalDetailsColumns) allColumns = reportSelection.PersonalDetailsColumns;

            if (null != reportSelection.ContactDetailsColumns) allColumns = allColumns.Concat(reportSelection.ContactDetailsColumns);

            if (null != reportSelection.ProfessionalDetailsColumns) allColumns = allColumns.Concat(reportSelection.ProfessionalDetailsColumns);

            if (null != reportSelection.PromotionDetailsColumns) allColumns = allColumns.Concat(reportSelection.PromotionDetailsColumns);

            //In case custom reports are generated 
            if (null != reportSelection.CustomColumns) allColumns = allColumns.Concat(reportSelection.CustomColumns);

            foreach (string column in allColumns)
            {
                dtColumn = new DataColumn();
                dtColumn.DataType = typeof(string);
                dtColumn.ColumnName = reportSelection.AllColumnsKeys[column];
                dtColumn.Caption = reportSelection.AllColumnsKeys[column];
                dtColumn.ReadOnly = false;
                dtColumn.Unique = false;
                // Add column to the DataColumnCollection.  
                dt_.Columns.Add(dtColumn);
            }

            foreach (EmployeeDetail employee in employees)
            {
                dataRow = dt_.NewRow();
                // Temp variables for date calculations
                ManipulateData manipulateData = new ManipulateData();
                DateTime VintageStartDate = (employee.DateofJoiningDGH ?? (reportSelection.From ?? DateTime.Now));
                foreach (string column in allColumns)
                {
                    switch (column)
                    {
                        case "WorkingStatus":
                            dataRow["Working Status"] = employee.WorkingStatus ? "Working" : "Separated";
                            break;
                        case "Dependent Details":
                            string[] dependents = (from depedent in DependentDetailContext.Collection().Where(d => d.EmployeeId == employee.Id).ToList()
                                                   where !depedent.DateofBirth.HasValue || ((depedent.DateofBirth >= reportSelection.From || !reportSelection.From.HasValue) && (depedent.DateofBirth <= reportSelection.To || !reportSelection.To.HasValue))
                                                   select "Name: " + depedent.DependentName
                                                   + (depedent.DateofBirth.HasValue ? ", DOB: " + depedent.DateofBirth.Value.ToString("dd'-'MM'-'yyyy") : "")
                                                   + ", Relation: " + depedent.Relationship).ToArray();
                            if (null != dependents) dataRow["Dependent Details"] = String.Join("NUMBER", dependents);
                            break;
                        case "Telephone Extension":
                            TelephoneExtension extension = TelephoneExtensionContext.Collection().Where(t => t.EmployeeId == employee.Id)
                                                                                                                    .FirstOrDefault();
                            if (null != extension) dataRow["Telephone Extension"] = extension.Number.ToString();
                            break;
                        case "Qualification Details":
                            string[] qualifications = (from qualification in QualificationDetailContext.Collection().Where(q => q.EmployeeId == employee.Id).ToList()
                                                       where !qualification.From.HasValue || ((qualification.From >= reportSelection.From || !reportSelection.From.HasValue) && (qualification.From <= reportSelection.To || !reportSelection.To.HasValue))
                                                       select "Degree:" + (!string.IsNullOrEmpty(qualification.Duration) ? qualification.Duration + " " : "") + qualification.Degree.Name +
                                                       (!string.IsNullOrEmpty(qualification.Specialization) ? "(" + qualification.Specialization + ")" : "") +
                                                       (!string.IsNullOrEmpty(qualification.Grade) ? ", Grade/Percentage: " + qualification.Grade : "") +
                                                       (!string.IsNullOrEmpty(qualification.Class) ? ", Class: " + qualification.Class : "") +
                                                       (!string.IsNullOrEmpty(qualification.Institution) ? ", Institution: " + qualification.Institution : "") +
                                                       (!string.IsNullOrEmpty(qualification.University) ? ", University: " + qualification.University : "") +
                                                       (qualification.From.HasValue ? ", From: " + qualification.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                                       (qualification.To.HasValue ? ", To: " + qualification.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                                           ).ToArray();
                            if (null != qualifications) dataRow["Qualification Details"] = String.Join("NUMBER", qualifications);
                            break;
                        case "Organisation":
                            if (!string.IsNullOrEmpty(employee.OrganisationId)) dataRow["Organisation"] = employee.Organisation.Name;
                            break;
                        case "Discipline":
                            if (!string.IsNullOrEmpty(employee.DisciplineId)) dataRow["Discipline"] = employee.Discipline.Name;
                            break;
                        case "Promotion Details":
                            string[] promotions = (from promotion in PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                                   where !promotion.From.HasValue || ((promotion.From >= reportSelection.From || !reportSelection.From.HasValue) && (promotion.From <= reportSelection.To || !reportSelection.To.HasValue))
                                                   select "Designation: " + promotion.Designation.Name +
                                                   (!string.IsNullOrEmpty(promotion.PayScaleId) ? ", PayScale: " + promotion.PayScale.Scale : "") +
                                                   (!string.IsNullOrEmpty(promotion.LevelId) ? ", Level: " + promotion.Level.Name : "") +
                                                   (promotion.From.HasValue ? ", From: " + promotion.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                                       (promotion.To.HasValue ? ", To: " + promotion.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                                   ).ToArray();
                            if (null != promotions) dataRow["Promotion Details"] = String.Join("NUMBER", promotions);
                            break;
                        case "Posting Details":
                            string[] postings = (from posting in PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                                 where !posting.From.HasValue || ((posting.From >= reportSelection.From || !reportSelection.From.HasValue) && (posting.From <= reportSelection.To || !reportSelection.To.HasValue))
                                                 select "Department: " + posting.Department.Name +
                                                 (!string.IsNullOrEmpty(posting.HODId) ? ", HoD: " + posting.HOD.Designation : "") +
                                                 (!string.IsNullOrEmpty(posting.Reporting) ? ", Reporting: " + posting.Reporting : "") +
                                                 (posting.From.HasValue ? ", From: " + posting.From.Value.ToString("dd'-'MM'-'yyyy") : "") +
                                                     (posting.To.HasValue ? ", To: " + posting.To.Value.ToString("dd'-'MM'-'yyyy") : "")
                                                   ).ToArray();
                            if (null != postings) dataRow["Posting Details"] = String.Join("NUMBER", postings);
                            break;
                        case "DGHLevel":
                            if (!string.IsNullOrEmpty(employee.LevelId)) dataRow["DGH Level"] = employee.DGHLevel.Name;
                            break;
                        case "Vintage":
                            dataRow["Vintage"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To);
                            break;
                        case "Designation":
                            string designation = (from promotion in PromotionDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                                  group promotion by promotion.EmployeeId into p
                                                  select p.OrderByDescending(l => l.From).FirstOrDefault().Designation.Name).SingleOrDefault();
                            if (!string.IsNullOrEmpty(designation)) dataRow["Designation"] = designation;
                            break;
                        case "Department":
                            string department = (from posting in PostingDetailContext.Collection().Where(p => p.EmployeeId == employee.Id).ToList()
                                                 group posting by posting.EmployeeId into p
                                                 select p.OrderByDescending(l => l.From).FirstOrDefault().Department.Name).SingleOrDefault();
                            if (!string.IsNullOrEmpty(department)) dataRow["Department"] = department;
                            break;

                        // Fields for Custom Reports
                        case "Vintage1Year":
                            dataRow["Vintage in 1 Year"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To, 1);
                            break;
                        case "Vintage2Years":
                            dataRow["Vintage in 2 Years"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To, 2);
                            break;
                        case "Vintage3Years":
                            dataRow["Vintage in 3 Years"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To, 3);
                            break;
                        case "Vintage4Years":
                            dataRow["Vintage in 4 Years"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To, 4);
                            break;
                        case "Vintage5Years":
                            dataRow["Vintage in 5 Years"] = GetCustomVintage(employee, reportSelection.From, reportSelection.To, 5);
                            break;

                        default:
                            object propertyValue = employee.GetType().GetProperty(column).GetValue(employee, null);
                            if (null != propertyValue)
                            {
                                if (propertyValue.GetType().IsEnum) dataRow[reportSelection.AllColumnsKeys[column]] = ((Enum)propertyValue).GetDisplayName();
                                else if (propertyValue.GetType() == typeof(DateTime?) || propertyValue.GetType() == typeof(DateTime)) dataRow[reportSelection.AllColumnsKeys[column]] = Convert.ToDateTime(propertyValue).Date.ToString("dd'-'MM'-'yyyy");
                                else dataRow[reportSelection.AllColumnsKeys[column]] = propertyValue.ToString();
                            }
                            break;
                    }
                }
                dt_.Rows.Add(dataRow);
            }
            return dt_;
        }

        private ReportSelectionViewModel GetViewModel(string customReportType = null)
        {
            ReportSelectionViewModel reportSelectionViewModel;
            if (customReportType.IsNullOrWhiteSpace()) reportSelectionViewModel = new ReportSelectionViewModel();
            else reportSelectionViewModel = new ReportSelectionViewModel(customReportType);
           
                reportSelectionViewModel.AllDepartments = (from department in DepartmentContext.Collection()
                                                           orderby department.Name
                                                           select new SelectListItem() { Value = department.Id, Text = department.Name }).AsEnumerable<SelectListItem>();
            
                if(customReportType== "SuperannuationReport")
            {
                
                var organisation = (from m in OrganisationContext.Collection()
                                     select new
                                     {
                                         Value =m.Id,
                                         Text = m.Name
                                     }).ToList();
                var allItem = new
                {
                    Value = "-1",
                    Text = "No Organisation"
                };
                organisation.Add(allItem);
                var levelcontext= (from m in LevelContext.Collection()
                            select new
                            {
                                Value = m.Id,
                                Text = m.Name
                            }).ToList();
                var allLevelItem = new
                {
                    Value = "-1",
                    Text = "No Level"
                };
                levelcontext.Add(allLevelItem);
                reportSelectionViewModel.AllOrganizations = (from org in organisation
                                                            
                                                             orderby org.Text

                                                             select new SelectListItem() { Value = org.Value, Text = org.Text })
                                                          
                                                           .AsEnumerable<SelectListItem>();

                reportSelectionViewModel.AllLevels = (from level in levelcontext
                                                      orderby level.Text
                                                             select new SelectListItem() { Value = level.Value, Text = level.Text }).AsEnumerable<SelectListItem>();
               
            }
            return reportSelectionViewModel;
        }

        private string GetCustomVintage(EmployeeDetail employee, DateTime? ReportFromDate, DateTime? ReportToDate, int addOnFactor = 0)
        {
            ManipulateData manipulateData = new ManipulateData();
            DateTime VintageStartDate = (employee.DateofJoiningDGH ?? (ReportFromDate ?? DateTime.Now));
            DateTime VintageEndDate = (ReportToDate ?? DateTime.Now);
            string vintage = "Not Available";

            if (employee.WorkingStatus) vintage = manipulateData.DateDifference((VintageEndDate.AddYears(addOnFactor) > employee.DateOfSuperannuation ? employee.DateOfSuperannuation.Value : VintageEndDate.AddYears(addOnFactor)), VintageStartDate);
            else if (employee.DateofLeavingDGH.HasValue) vintage = manipulateData.DateDifference(employee.DateofLeavingDGH.Value, VintageStartDate);
            else if (VintageEndDate.AddYears(addOnFactor) < employee.DateOfSuperannuation) vintage = manipulateData.DateDifference(VintageEndDate.AddYears(addOnFactor), VintageStartDate);// If neither of 2 are available, use report's cutoff date
            else if (employee.DateOfSuperannuation.HasValue) vintage = manipulateData.DateDifference(employee.DateOfSuperannuation.Value, VintageStartDate);// If nothing is available, use superannuation date

            return vintage;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);

                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;

        }
    }
}