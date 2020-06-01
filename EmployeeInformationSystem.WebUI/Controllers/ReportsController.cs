using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using EmployeeInformationSystem.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            return PartialView(targetPage, GetViewModel());
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
                            ManipulateData manipulateData = new ManipulateData();
                            if (employee.DateofJoiningDGH.HasValue)
                            {
                                if (employee.WorkingStatus) dataRow["Vintage"] = manipulateData.DateDifference(DateTime.Now.Date, employee.DateofJoiningDGH ?? DateTime.Now.Date);
                                else if (employee.DateofLeavingDGH.HasValue) dataRow["Vintage"] = manipulateData.DateDifference(employee.DateofLeavingDGH ?? DateTime.Now.Date, employee.DateofJoiningDGH ?? DateTime.Now.Date);
                            }
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

        private ReportSelectionViewModel GetViewModel()
        {
            return new ReportSelectionViewModel()
            {
                AllDepartments = (from department in DepartmentContext.Collection()
                                  orderby department.Name
                                  select new SelectListItem() { Value = department.Id, Text = department.Name }).AsEnumerable<SelectListItem>()
            };
        }
    }
}