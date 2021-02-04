using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using EmployeeInformationSystem.Services;
using EmployeeInformationSystem.WebUI.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        public ActionResult birthdayandAniReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;
                var employees1 = (from employee in EmployeeDetailContext.Collection().Where(x => x.DateOfBirth != null).ToList()
                                  join post in PostingDetailContext.Collection().ToList()
                                  on employee.Id equals post.EmployeeId
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      employee.WorkingStatus,
                                      DateOfBirth = employee.DateOfBirth,
                                      Anniversary = employee.MarriageDate,
                                      employeeId = employee.Id,
                                      DepartmentId = post.DepartmentId,
                                      EmployeeTypeId = employee.EmployeeType

                                  } into s
                                  group s by s.employeeId into g
                                  from y1 in g
                                  select new
                                  {
                                      y1.EmployeeCode,
                                      y1.FullName,

                                      y1.Designation,

                                      DateOfBirth = y1.DateOfBirth?.ToString("dd-MM-yyyy"),
                                      Anniversary = y1.Anniversary?.ToString("dd-MM-yyyy"),
                                      EmployeeType = y1.EmployeeType,
                                      EmployeeId = y1.employeeId,
                                      y1.DepartmentId,
                                      DateOfBirthMonth = y1.DateOfBirth?.ToString("MM"),
                                      AnniversaryMonth = y1.Anniversary?.ToString("MM"),
                                      DateOfBirthSort = y1.DateOfBirth?.ToString("dd-MM"),
                                      y1.Department,
                                      y1.WorkingStatus,
                                      WorkStatus = y1.WorkingStatus == true ? "working" : "separated",
                                      y1.EmployeeTypeId,
                                  }


                                 ).
             Distinct().Where(x => reportSelection.Departments.Contains(x.DepartmentId)
                && reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString()) && reportSelection.Month.Contains(reportSelection?.Type == "Birthday" ?
                x.DateOfBirthMonth : reportSelection?.Type == "Anniversary" ? x.AnniversaryMonth : (x.DateOfBirthMonth)) &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false))
             .OrderByDescending(x => x.DateOfBirthSort).DistinctBy(x => x.EmployeeId).ToList();
                dt_ = ToDataTable(employees1);

                var emp = employees1.ToList();
                dt_ = ToDataTable(emp);
                // dt_ = ToDataTable(employees1);
                dt_.Columns.Remove("EmployeeId");
                dt_.Columns.Remove("DepartmentId");
                dt_.Columns.Remove("DateOfBirthMonth");
                dt_.Columns.Remove("AnniversaryMonth");
                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateOfBirthSort");
                //int dtlen = dt_.Rows.Count;
                //string wstatus;
                //for(int i=0;i<dtlen;i++)
                //{
                //    wstatus = dt_.Rows[i][7].ToString();
                //    if(wstatus == "true")
                //    {
                //        wstatus = "working";
                //    }
                //    else
                //    {
                //        wstatus = "separated";
                //    }
                //    dt_.Rows[i][7] = wstatus;
                //}

                ViewBag.ReportTitle = "- Birthday and Anniversary Report";
                return View("GeneratedReportView", dt_);
            }
        }
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
                                  join promo in PromotionDetailContext.Collection().ToList()
                                  on employee.Id equals promo.EmployeeId

                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      PromotionDetails = ManPowerEtraDetai("Promotion Details", employee, reportSelection),
                                      From = promo.From,
                                      FromDate = promo.From?.ToString("dd-MM-yyyy"),
                                      PayScale = promo.PayScale,
                                      EmployeeId = promo.EmployeeId,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),

                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      DateofLeavingDGH = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,


                                  } into s
                                  group s by s.EmployeeId into g

                                  from y1 in g.DefaultIfEmpty()
                                  select new
                                  {

                                      y1.EmployeeCode,
                                      FullName = y1.FullName,
                                      y1.Department,
                                      y1.Designation,
                                      y1.PromotionDetails,
                                      y1.FromDate,
                                      y1.EmployeeId,
                                      EmployeeTypeId = y1.EmployeeTypeId,
                                      y1.WorkingStatus,
                                      y1.WorkStatus,
                                      y1.From
                                  }

                                  )
                                  .Where(x => x.EmployeeTypeId == EmployeeType.Deputationist
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false))
                                  .OrderByDescending(x => x.From).DistinctBy(x => x.EmployeeId)
                                  .ToList();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {

                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.From <= reportSelection.To || (!x.WorkingStatus && x.From < reportSelection.To)) :
                     (x.From <= reportSelection.To || (!x.WorkingStatus && x.From < reportSelection.To))).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.From >= reportSelection.From) :
                     (x.From >= reportSelection.From)).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x =>
                      reportSelection?.Working == "separated" ? (x.From >= reportSelection.From && x.From <= reportSelection.To) :
                       (x.From >= reportSelection.From && x.From <= reportSelection.To)).ToList();
                    dt_ = ToDataTable(emp);
                }

                // dt_ = ToDataTable(employees1);
                //dt_.Columns.Remove("LevelId");
                //dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("EmployeeId");
                dt_.Columns.Remove("From");


                ViewBag.ReportTitle = "- Promotion Report";
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      QualificationDetails = ManPowerEtraDetai("Qualification Details", employee, reportSelection),

                                      CurrentBasicPay = employee.CurrentBasicPay,
                                      PastExperience = employee.OrganisationId != null ? ManPowerEtraDetai("Past Experience", employee, reportSelection) : "",

                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();

                dt_ = ToDataTable(employees1);
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Qualification Experience or Pay-Scale Report";
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(x => x.WorkingStatus == false && x.EmployeeTypeId != EmployeeType.Deputationist).ToList();
                dt_ = ToDataTable(employees1);
                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("DateofLeavingDGH");

                ViewBag.ReportTitle = "- Employee deputation with DGH Report";
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),

                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      ContractPeriod = employee.DeputationPeriod,
                                      ContractExpiryDate = employee.DateOfContractExpiry?.ToString("dd-Mm-yyyy"),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,
                                  })
                .Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                &&
                                      (x.WorkingStatus == false)).ToList();


                dt_ = ToDataTable(employees1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Tenure Report";
                return View("GeneratedReportView", dt_);
            }
        }



        [HttpPost]
        public ActionResult TenureCompletionReport(ReportSelectionViewModel reportSelection)
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),

                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      ContractPeriod = employee.DeputationPeriod,
                                      ContractExpiryDate = employee.DateOfContractExpiry?.ToString("dd-Mm-yyyy"),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,
                                  })
                .Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                &&
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false) && (Convert.ToDateTime(x.DGHLeavingDate) == Convert.ToDateTime(x.ContractExpiryDate))).ToList();


                dt_ = ToDataTable(employees1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    var todayDate = DateTime.Now.ToString("dd-MM-yyyy");

                    var emp = employees1.Where(x => x.DateofLeavingDGH <= Convert.ToDateTime(todayDate) || (!x.WorkingStatus && x.DateofLeavingDGH < Convert.ToDateTime(todayDate))).ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Tenure Completion Report";
                return View("GeneratedReportView", dt_);
            }
        }



        [HttpPost]

        public ActionResult EarlyRepatriationReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }

            else
            {
                DataTable dt_ = null;

                var employees1 = (
                    from employee in EmployeeDetailContext.Collection().ToList()

                    select new
                    {
                        employee.EmployeeCode,
                        FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                        Department = ManPowerEtraDetai("Department", employee, reportSelection),
                        Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                        employee.EmployeeType,
                        Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                        DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                        employee.DateofLeavingDGH,
                        DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                        employee.ReasonForLeaving,
                        employee.DateOfSuperannuation,
                        SuperannuationDate = employee.DateOfSuperannuation?.ToString("dd-MM-yyyy"),
                        employee.WorkingStatus,
                        WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                        employee.LevelId,
                        employee.OrganisationId,
                        EmployeeTypeId = employee.EmployeeType,
                        ContractExpiryDate = employee.DateOfContractExpiry?.ToString("dd-Mm-yyyy")

                    }

                    ).Distinct().Where(x => x.EmployeeType == EmployeeType.Deputationist
                  &&
                                        (x.WorkingStatus == false) && (x.ReasonForLeaving == ReasonForLeaving.Repatriation) && (Convert.ToDateTime(x.DateofLeavingDGH) < Convert.ToDateTime(x.ContractExpiryDate))).ToList();




                dt_ = ToDataTable(employees1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)).ToList();

                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }

                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("DateOfSuperannuation");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Deputationist Early Repatriation Report ";
                return View("GeneratedReportView", dt_);
            }
        }


        [HttpPost]
        public ActionResult MissingDataReport(ReportSelectionViewModel reportSelection)
        {

            if (!ModelState.IsValid)
            {
                return View("ReportSelection", reportSelection);
            }
            else
            {
                ViewBag.Title = "Missing Data Report";
                List<EmployeeDetail> employees = new List<EmployeeDetail>();
                List<EmployeeDetail> employeesCheck = new List<EmployeeDetail>();

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


                foreach (var data in employees)
                {
                    if (data.EmployeeCode == null)
                    {
                        data.EmployeeCode = "Missing";
                    }
                    if (data.Title == null)
                    {
                        data.Title = "Missing";
                    }
                    if (data.FirstName == null)
                    {
                        data.FirstName = "Missing";
                    }
                   
                    if (data.LastName == null)
                    {
                        data.LastName = "Missing";
                    }

                    if (data.DeputationPeriod == null)
                    {
                        data.DeputationPeriod = "Missing";
                    }

                    if (data.MobileNumber == null)
                    {
                        data.MobileNumber = "Missing";
                    }

                    if (data.ResidenceNumber == null)
                    {
                        data.ResidenceNumber = "Missing";
                    }

                    if (data.ResidenceAddress == null)
                    {
                        data.ResidenceAddress = "Missing";
                    }

                    if (data.PermanentAddress == null)
                    {
                        data.PermanentAddress = "Missing";
                    }

                    if (data.PrimaryExpertise == null)
                    {
                        data.PrimaryExpertise = "Missing";
                    }

                    if (data.CurrentBasicPay == null)
                    {

                        data.CurrentBasicPay = "Missing";
                    }

                    if (data.PANNumber == null)
                    {

                        data.PANNumber = "Missing";
                    }

                    if (data.AadhaarNumber == null)
                    {

                        data.AadhaarNumber = "Missing";
                    }

                    if (data.PassportNumber == null)
                    {
                        data.PassportNumber = "Missing";
                    }



                    if (data.VehicleType == null)
                    {
                        data.VehicleType = "Missing";
                    }

                    if (data.VehicleNumber == null)
                    {
                        data.VehicleNumber = "Missing";

                    }

                    if (data.VehicleCategory == null)
                    {
                        data.VehicleCategory = VehicleCategory.Missing;
                    }

                    if (data.AlternateEmailID == null) {
                        data.AlternateEmailID = "Missing";
                    
                    }

                    if (data.EmergencyPerson == null) {
                        data.EmergencyPerson = "Missing";
                    }

                    if (data.EmergencyRelation == null) {
                        data.EmergencyRelation = "Missing";
                    }

                    if (data.EmergencyContact == null) {
                        data.EmergencyContact = "Missing";
                    }

                    if (data.UANNumber == null) {

                        data.UANNumber = "Missing";
                    }

                    if(data.MaritalStatus==null)
                    {
                        data.MaritalStatus = MaritalStatus.Missing;
                    }


                    if (data.BloodGroup == null)
                    {
                        data.BloodGroup = BloodGroup.Missing;
                    }

                    if (data.SeatingLocation == null)
                    {
                        data.SeatingLocation = SeatingLocation.Missing;
                    }

                    if (data.DeputedLocation == null)
                    {
                        data.DeputedLocation = DeputeLocations.Missing;
                    }
                  
                    


                    //if (data.DateOfBirth == null)
                    //{
                    //    Convert.ToString(data.DateOfBirth) ="Missing";
                    //}
                    //if (data.DateOfSuperannuation == null) {


                    //}


                    //if (data.DateofJoiningParentOrg == null) { }

                    //if (data.DateofRelievingLastOffice == null) { }


                    //if (data.DateOfContractExpiry == null) { }


                    //if (data.DateofJoiningDGH == null) { }
                    //if (data.DateofLeavingDGH == null) { }






                    if (data.EmailID == null)
                    {
                        data.EmailID = "Missing";
                    }
                    employeesCheck.Add(data);

                }


                DataTable dt_ = GetDataTable(employeesCheck, reportSelection);

                ViewBag.ReportTitle = "- Missing Data Report ";

                return View("GeneratedReportView", dt_);
            }
        }


        [HttpPost]

        public ActionResult LoginDetailsReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;

                var context = new ApplicationDbContext();

                var allUsers = context.Users.ToList();


                var employees1 = (
                                  from user in context.Users.ToList()


                                  join employee in EmployeeDetailContext.Collection().ToList()
                                   on user.UserName equals employee.EmployeeCode into xy
                                  from z in xy.DefaultIfEmpty()
                                      //// where(employee.OrganisationId == org.Id)
                                      ///
                                  join org in OrganisationContext.Collection().ToList()
                                  on z.OrganisationId equals org.Id into xx



                                  from y in xx.DefaultIfEmpty()

                                      //into xx



                                  select new
                                  {
                                      z.EmployeeCode,
                                      FullName = z.FirstName + " " + (z.MiddleName == "" ? "" : z.MiddleName + " ") + z.LastName,
                                      Department = ManPowerEtraDetai("Department", z, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", z, reportSelection),

                                      EmployeeType = z.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,

                                      DateofJoiningDGH = z.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      z.DateofLeavingDGH,
                                      DGHLeavingDate = z.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      ContractPeriod = z.DeputationPeriod,
                                      ContractExpiryDate = z.DateOfContractExpiry?.ToString("dd-Mm-yyyy"),
                                      z.WorkingStatus,
                                      WorkStatus = z.WorkingStatus == true ? "working" : "separated",
                                      z.LevelId,
                                      z.OrganisationId,
                                      EmployeeTypeId = z.EmployeeType

                                  }

                    ).Distinct().Where(x =>
                                      (reportSelection?.Working == "working" ? x.WorkingStatus == true :
                                      reportSelection?.Working == "separated" ? false : x.WorkingStatus == true || x.WorkingStatus == false)).ToList();





                dt_ = ToDataTable(employees1);

                ViewBag.ReportTitle = "- Authorisations and Login Reports ";



                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");

                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("DateofJoiningDGH");
                dt_.Columns.Remove("DGHLeavingDate");
                dt_.Columns.Remove("ContractPeriod");
                dt_.Columns.Remove("ContractExpiryDate");



                return View("GeneratedReportView", dt_);

            }
        }

        [HttpPost]
        public ActionResult EarlyTerminationReport(ReportSelectionViewModel reportSelection)
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),

                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      ContractPeriod = employee.DeputationPeriod,
                                      ContractExpiryDate = employee.DateOfContractExpiry?.ToString("dd-Mm-yyyy"),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,
                                  }

                    ).Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                  &&
                                        (x.WorkingStatus == false) && (x.ReasonForLeaving == ReasonForLeaving.Termination) && (Convert.ToDateTime(x.DGHLeavingDate) < Convert.ToDateTime(x.ContractExpiryDate))).ToList();


                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => Convert.ToDateTime(x.ContractExpiryDate) <= reportSelection.To || (!x.WorkingStatus && Convert.ToDateTime(x.ContractExpiryDate) < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => Convert.ToDateTime(x.ContractExpiryDate) >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => Convert.ToDateTime(x.ContractExpiryDate) >= reportSelection.From && Convert.ToDateTime(x.ContractExpiryDate) <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }


                dt_ = ToDataTable(employees1);

                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("WorkStatus");

                ViewBag.ReportTitle = "- Early Termination/Separation Report For Contractuals ";



                return View("GeneratedReportView", dt_);

            }

        }

        [HttpPost]

        public ActionResult LastChangeMadeReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }

            else
            {
                DataTable dt_ = null;

                var employee1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                 select new
                                 {
                                     employee.EmployeeCode,
                                     FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                     employee.LastUpdateBy,
                                     employee.LastUpdateAt,

                                     Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                     Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                     EmployeeType = employee.EmployeeType.GetDisplayName(),
                                     employee.WorkingStatus,
                                     WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                     EmployeeTypeId = employee.EmployeeType,

                                 }).Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())
                  &&
                                        (x.WorkingStatus == true)).ToList();

                dt_ = ToDataTable(employee1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employee1);
                }

                else if (!reportSelection.From.HasValue)
                {
                    var emp = employee1.Where(x => x.LastUpdateAt <= reportSelection.To).ToList();

                    dt_ = ToDataTable(emp);
                }

                else if (!reportSelection.To.HasValue)
                {
                    var emp = employee1.Where(x => x.LastUpdateAt >= reportSelection.From).ToList();

                    dt_ = ToDataTable(emp);

                }

                else
                {
                    var emp = employee1.Where(x => x.LastUpdateAt >= reportSelection.From && x.LastUpdateAt <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }
                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("WorkStatus");

                ViewBag.ReportTitle = "- Last Change Made Report";
                return View("GeneratedReportView", dt_);

            }
        }


        [HttpPost]
        public ActionResult DateOfJoiningReport(ReportSelectionViewModel reportSelection)
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      Organisation = y == null ? "" : y.Name,
                                      employee.DateofJoiningDGH,
                                      DGHJoiningDate = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),

                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),


                                      DateofLeavingDGH = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,

                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.DeputationPeriod,
                                      SuperannuationDate = employee.DateOfSuperannuation?.ToString("dd-MM-yyyy"),
                                      ContractExpiryDate = employee.DateOfContractExpiry?.ToString("dd-Mm-yyyy"),
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
                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofJoiningDGH");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Date of Joining Report";
                return View("GeneratedReportView", dt_);
            }
        }
        [HttpPost]
        public ActionResult SeparationReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                DataTable dt_ = null;
                // var separationreasonnullcheck = reportSelection.Working.Contains("working");
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()

                                  join org in OrganisationContext.Collection().ToList()
                                  //// where(employee.OrganisationId == org.Id)
                                  on employee.OrganisationId equals org.Id into xx
                                  //into xx
                                  from y in xx.DefaultIfEmpty()
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),

                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,

                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),
                                      employee.ReasonForLeaving,
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                      EmployeeTypeId = employee.EmployeeType,
                                  })
                .Distinct().Where(x => reportSelection.CustomColumns.Contains(x.EmployeeTypeId.ToString())

                && reportSelection.SeparationReason.Contains(x.ReasonForLeaving.ToString())).ToList();


                dt_ = ToDataTable(employees1);

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)).ToList();
                    //                 select employee).Distinct().ToList();
                    dt_ = ToDataTable(emp);
                }
                else if (!reportSelection.To.HasValue)
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From).ToList();
                    dt_ = ToDataTable(emp);
                }
                else
                {
                    var emp = employees1.Where(x => x.DateofLeavingDGH >= reportSelection.From && x.DateofLeavingDGH <= reportSelection.To).ToList();
                    dt_ = ToDataTable(emp);
                }



                dt_.Columns.Remove("LevelId");
                dt_.Columns.Remove("OrganisationId");
                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Separation Report";
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Organisation = y == null ? "" : y.Name,
                                      //Discipline = y1 == null ? "" : y1.Name,
                                      //Level = y2 == null ? "" : y2.Name,
                                      DateofJoiningDGH = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateOfSuperannuation,
                                      SuperannuationDate = employee.DateOfSuperannuation?.ToString("dd-MM-yyyy"),
                                      DateofLeavingDGH = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),

                                      employee.ReasonForLeaving,
                                      employee.DeputationPeriod,
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.LevelId,
                                      employee.OrganisationId,
                                  })
                .Distinct().Where(x => (reportSelection.Organisation.Contains(x.OrganisationId) || (orgnullcheck == true ? x.OrganisationId is null : x.OrganisationId == "-1")) &&
              (reportSelection.Level.Contains(x.LevelId) || (levelnullcheck == true ? x.LevelId is null : x.LevelId == "-1"))
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
                dt_.Columns.Remove("DateOfSuperannuation");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Superannuation Report";
                return View("GeneratedReportView", dt_);
            }
        }
        public string ManPowerEtraDetai(string column, EmployeeDetail employee, ReportSelectionViewModel reportSelection)
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
                    columdetail = String.Join("NUMBER", qualifications);
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
                    columdetail = extension == null ? "" : extension.Number.ToString();
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
                    string[] payscale = (from pay in PayScaleContext.Collection().Where(x => x.OrganisationId == employee.OrganisationId).ToList()
                                         select pay.Scale).ToArray();

                    columdetail = String.Join("NUMBER", payscale);
                    break;
                case "Past Experience":
                    string[] past_exp = (from exp in PastExperienceContext.Collection().Where(x => x.EmployeeId == employee.Id).ToList()
                                         select (!string.IsNullOrEmpty(exp.Position) ?
                                         "Postion: " + exp.Position : "") +
                                         (!string.IsNullOrEmpty(exp.Organisation) ? ", Organisation: " + exp.Organisation : "") +
                                            (exp.From.HasValue ? ", From: " + exp.From : "") +
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
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      employee.AadhaarNumber,
                                      BloodGroup = employee.BloodGroup.GetDisplayName(),
                                      QualificationDetails = ManPowerEtraDetai("Qualification Details", employee, reportSelection),
                                      PromotionDetails = ManPowerEtraDetai("Promotion Details", employee, reportSelection),
                                      PostingDetails = ManPowerEtraDetai("Posting Details", employee, reportSelection),
                                      DependentDetails = ManPowerEtraDetai("Dependent Details", employee, reportSelection),
                                      TelephoneExtension = ManPowerEtraDetai("Telephone Extension", employee, reportSelection),
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      Organisation = y == null ? "" : y.Name,
                                      DateOfBirth = employee.DateOfBirth?.ToString("dd-MM-yyyy"),
                                      DateOfSuperannuation = employee.DateOfSuperannuation?.ToString("dd-MM-yyyy"),
                                      DateofJoiningParentOrg = employee.DateofJoiningParentOrg?.ToString("dd-MM-yyyy"),
                                      DateofRelievingLastOffice = employee.DateofRelievingLastOffice?.ToString("dd-MM-yyyy"),
                                      employee.DateofJoiningDGH,
                                      DGHJoinigDate = employee.DateofJoiningDGH?.ToString("dd-MM-yyyy"),
                                      employee.DateofLeavingDGH,
                                      DGHLeavingDate = employee.DateofLeavingDGH?.ToString("dd-MM-yyyy"),

                                      employee.ReasonForLeaving,
                                      employee.DeputationPeriod,
                                      SeatingLocation = employee.SeatingLocation.GetDisplayName(),

                                      employee.MobileNumber,
                                      employee.ResidenceNumber,
                                      employee.ResidenceAddress,
                                      employee.PermanentAddress,
                                      employee.EmailID,
                                      employee.ProfilePhoto,
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.Gender,
                                      Discipline = y1 == null ? "" : y1.Name,
                                      employee.PrimaryExpertise,
                                      Level = y2 == null ? "" : y2.Name,
                                      employee.CurrentBasicPay,
                                      employee.PANNumber,
                                      employee.PassportNumber,
                                      employee.PassportValidity,
                                      employee.VehicleNumber,
                                      employee.MaritalStatus,
                                      MarriageDate = employee.MarriageDate?.ToString("dd-MM-yyyy"),

                                      employee.AlternateEmailID,
                                      employee.EmergencyPerson,
                                      employee.EmergencyContact,
                                      employee.UANNumber,
                                      DeputedLocation = employee.DeputedLocation.GetDisplayName(),
                                      employee.CreatedAt,
                                      employee.LastUpdateAt,
                                      employee.LastUpdateBy,
                                      employee.VehicleType,
                                      VehicleCategory = employee.VehicleCategory.GetDisplayName(),
                                      employee.EmergencyRelation,
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                  (reportSelection?.Working == "working" ? p.WorkingStatus == true :
                                  reportSelection?.Working == "separated" ? false : p.WorkingStatus == true || p.WorkingStatus == false)

).ToList();


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

                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("DateofJoiningDGH");
                dt_.Columns.Remove("DateofLeavingDGH");
                dt_.Columns.Remove("WorkingStatus");
                dt_.Columns.Remove("ProfilePhoto");

                ViewBag.ReportTitle = "- Man Power Report";
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

                ViewBag.ReportTitle = "- Deputationist Vintage Report";

                return View("GeneratedReportView", dt_);
            }
        }

        [HttpPost]
        public ActionResult LocalAddressReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                ViewBag.Title = "Address Report";
                DataTable dt_ = null;
                var type = reportSelection.CustomColumns.ToList();

                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()

                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      employee.MobileNumber,
                                      employee.ResidenceNumber,
                                      employee.ResidenceAddress,
                                      employee.PermanentAddress,
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      EmployeeTypeId = employee.EmployeeType,
                                  }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                  (
                                  reportSelection?.Working == "working" ? p.WorkingStatus == true :
                                  reportSelection?.Working == "separated" ? p.WorkingStatus == false : p.WorkingStatus == true || p.WorkingStatus == false)).ToList();


                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }

                dt_.Columns.Remove("EmployeeTypeId");
                dt_.Columns.Remove("WorkingStatus");

                ViewBag.ReportTitle = "- Address Report";
                return View("GeneratedReportView", dt_);
            }
        }

        [HttpPost]
        public ActionResult FamilyDetailsReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                ViewBag.Title = "Family Details Report";
                DataTable dt_ = null;
                var type = reportSelection.CustomColumns.ToList();

                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                  join Dependt in DependentDetailContext.Collection().ToList()
                                 on employee.Id equals Dependt.EmployeeId
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      Dependt.DependentName,
                                      Dependt.Relationship,
                                      DependentDetails = ManPowerEtraDetai("Dependent Details", employee, reportSelection),
                                      Vintage = ManPowerEtraDetai("Vintage", employee, reportSelection),
                                      DateOfBirth = employee.DateOfBirth?.ToString("dd-MM-yyyy"),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      employee.Gender,
                                      MarriageDate = employee.MarriageDate?.ToString("dd-MM-yyyy"),
                                      employee.AlternateEmailID,
                                      EmployeeTypeId = employee.EmployeeType,
                                      employee.DateofJoiningDGH,
                                      employee.DateofLeavingDGH,
                                  }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                  (reportSelection?.Working == "working" ? p.WorkingStatus == true :
                                  reportSelection?.Working == "separated" ? false : p.WorkingStatus == true || p.WorkingStatus == false)).ToList();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {

                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)) :
                     (x.DateofJoiningDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To))).ToList();
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

                if (dt_ != null)
                {
                    dt_.Columns.Remove("EmployeeTypeId");
                    dt_.Columns.Remove("WorkingStatus");
                    dt_.Columns.Remove("DateofJoiningDGH");
                    dt_.Columns.Remove("DateofLeavingDGH");
                }

                ViewBag.ReportTitle = "- Family Details Report";
                return View("GeneratedReportView", dt_);
            }
        }

        [HttpPost]
        public ActionResult AgeProfileReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                ViewBag.Title = "Age Profile Report";
                DataTable dt_ = null;
                var type = reportSelection.CustomColumns.ToList();
                var DOB = DateTime.Now.Year;
                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()

                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      Age = (DateTime.Now.Year - employee.DateOfBirth?.Year - 1) == null ? "" : (DateTime.Now.Year - employee.DateOfBirth?.Year - 1) + " Years",
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      EmployeeTypeId = employee.EmployeeType,
                                      employee.DateofJoiningDGH,
                                      employee.DateofLeavingDGH,
                                  }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                  (reportSelection?.Working == "working" ? p.WorkingStatus == true :
                                  reportSelection?.Working == "separated" ? p.WorkingStatus == false : p.WorkingStatus == true || p.WorkingStatus == false)).ToList();

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

                if (dt_ != null)
                {
                    dt_.Columns.Remove("EmployeeTypeId");
                    dt_.Columns.Remove("WorkingStatus");
                    dt_.Columns.Remove("DateofJoiningDGH");
                    dt_.Columns.Remove("DateofLeavingDGH");
                }

                ViewBag.ReportTitle = "- Age Profile Report";
                return View("GeneratedReportView", dt_);
            }
        }

        [HttpPost]
        public ActionResult LastPromotionReport(ReportSelectionViewModel reportSelection)
        {
            if (!ModelState.IsValid)
            {
                return View(reportSelection);
            }
            else
            {
                ViewBag.Title = "Last Promotion Report";
                DataTable dt_ = null;
                var type = reportSelection.CustomColumns.ToList();

                var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()
                                  join promotion in PromotionDetailContext.Collection().ToList()
                                 on employee.Id equals promotion.EmployeeId
                                  select new
                                  {
                                      employee.EmployeeCode,
                                      FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                      EmployeeType = employee.EmployeeType.GetDisplayName(),
                                      Department = ManPowerEtraDetai("Department", employee, reportSelection),
                                      Designation = ManPowerEtraDetai("Designation", employee, reportSelection),
                                      PromotionDetails = ManPowerEtraDetai("Promotion Details", employee, reportSelection),
                                      employee.WorkingStatus,
                                      WorkStatus = employee.WorkingStatus == true ? "working" : "separated",
                                      EmployeeTypeId = employee.EmployeeType,
                                      employee.DateofJoiningDGH,
                                      employee.DateofLeavingDGH,
                                  }).Distinct().Where(p => type.Contains(p.EmployeeTypeId.ToString()) &&
                                  (reportSelection?.Working == "working" ? p.WorkingStatus == true :
                                  reportSelection?.Working == "separated" ? false : p.WorkingStatus == true || p.WorkingStatus == false)).ToList();

                if (!reportSelection.From.HasValue && !reportSelection.To.HasValue)
                {
                    dt_ = ToDataTable(employees1);
                }
                else if (!reportSelection.From.HasValue)
                {

                    var emp = employees1.Where(x =>
                     reportSelection?.Working == "separated" ? (x.DateofLeavingDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To)) :
                     (x.DateofJoiningDGH <= reportSelection.To || (!x.WorkingStatus && x.DateofLeavingDGH < reportSelection.To))).ToList();
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

                if (dt_ != null)
                {
                    dt_.Columns.Remove("EmployeeTypeId");
                    dt_.Columns.Remove("WorkingStatus");
                    dt_.Columns.Remove("DateofJoiningDGH");
                    dt_.Columns.Remove("DateofLeavingDGH");
                }

                ViewBag.ReportTitle = "- Last Promotion Report";
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
            else if (customReportType == "MissingDataReport")
            {
                reportSelectionViewModel = new ReportSelectionViewModel();
            }

            else reportSelectionViewModel = new ReportSelectionViewModel(customReportType);

            reportSelectionViewModel.AllDepartments = (from department in DepartmentContext.Collection()
                                                       orderby department.Name
                                                       select new SelectListItem() { Value = department.Id, Text = department.Name }).AsEnumerable<SelectListItem>();

            if (customReportType == "SuperannuationReport")
            {

                var organisation = (from m in OrganisationContext.Collection()
                                    select new
                                    {
                                        Value = m.Id,
                                        Text = m.Name
                                    }).ToList();
                var allItem = new
                {
                    Value = "-1",
                    Text = "No Organisation"
                };
                organisation.Add(allItem);
                var levelcontext = (from m in LevelContext.Collection()
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