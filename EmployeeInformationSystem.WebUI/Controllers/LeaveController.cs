using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
        IRepository<LeaveMaster> LeaveMasterContext;
        IRepository<EmployeeLeaveDetails> EmployeeLeaveDetailsContext;
        IRepository<EmployeeLeaveBalance> EmployeeLeaveBalanceContext;

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
        IRepository<LeaveType> leaveTypeContext,
        IRepository<LeaveMaster> leaveMasterContext,
        IRepository<EmployeeLeaveDetails> employeeLeaveDetailsContext,
        IRepository<EmployeeLeaveBalance> employeeLeaveBalanceContext
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
            LeaveMasterContext = leaveMasterContext;
            EmployeeLeaveDetailsContext = employeeLeaveDetailsContext;
            EmployeeLeaveBalanceContext = employeeLeaveBalanceContext;
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


            List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.OrganisationList = orgList;

            List<LeaveType> LeaveType = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.LevelTypeList = LeaveType;




            return View();
        }

        public ActionResult ViewLeaveQuota()
        {
            DataTable dt_ = null;
            var LQ_data = (from leaveMaster in LeaveMasterContext.Collection().ToList()

                           join orgC in OrganisationContext.Collection().ToList()
                           on leaveMaster.OrganisationId equals orgC.Id

                           join lt in LeaveTypeContext.Collection().ToList()
                           on leaveMaster.LeaveTypeId equals lt.Id

                           select new
                           {
                               Organisation = orgC.Name,
                               LeaveType = lt.Name,
                               leaveMaster.AnnualQuota,
                               leaveMaster.ValidFrom,
                               leaveMaster.ValidTill,
                               leaveMaster.Id


                           }

                       ).OrderBy(x => x.Organisation).ToList();

            dt_ = ToDataTable(LQ_data);
            ViewBag.targetmodel = "LeaveMaster";
            ViewBag.HeadingName = "View Leave Quota ";
            return View("ShowResponse", dt_);
        }

        public ActionResult Edit(string idData, string targetmodel)
        {


            var viewname = "";
            switch (targetmodel)
            {
                case "LeaveType":
                    LeaveType lt = new LeaveType();
                    var LT_data = LeaveTypeContext.Collection().Where(a => a.Id == idData).ToList();
                    foreach (var item in LT_data)
                    {
                        lt.Name = item.Name;
                        lt.Id = item.Id;
                    }
                    viewname = "LeaveType";
                    ViewBag.Name = viewname;
                    ViewBag.HeadingName = "Update Leave Type";
                    ViewBag.HeadingColor = "bg-warning ";
                    return View(viewname, lt);

                case "LeaveMaster":

                    LeaveMaster lm = new LeaveMaster();
                    var LQ_data = (from leaveMaster in LeaveMasterContext.Collection().ToList()

                                   join orgC in OrganisationContext.Collection().ToList()
                                   on leaveMaster.OrganisationId equals orgC.Id

                                   join ltC in LeaveTypeContext.Collection().ToList()
                                   on leaveMaster.LeaveTypeId equals ltC.Id

                                   select new
                                   {
                                       Organisation = orgC.Name,
                                       LeaveType = ltC.Name,
                                       leaveMaster.AnnualQuota,
                                       leaveMaster.ValidFrom,
                                       leaveMaster.ValidTill,
                                       leaveMaster.Id


                                   }

                        ).Where(x => x.Id == idData).ToList();

                    foreach (var item in LQ_data)
                    {
                        lm.AnnualQuota = item.AnnualQuota;
                        lm.ValidFrom = item.ValidFrom;
                        lm.ValidTill = item.ValidTill;
                        ViewBag.LName = item.LeaveType;
                        ViewBag.OName = item.Organisation;
                        lm.Id = item.Id;

                    }

                    ViewBag.Action = "UpdateLeaveMaster";
                    List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
                    ViewBag.OrganisationList = orgList;

                    List<LeaveType> LeaveType = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
                    ViewBag.LevelTypeList = LeaveType;

                    viewname = "LeaveMaster";
                    return View(viewname, lm);
                    //break;

            }
            return View(viewname);
        }

        public ActionResult Delete(string idData, string targetmodel)
        {
            var viewname = "";
            switch (targetmodel)
            {
                case "LeaveType":
                    LeaveType lt = new LeaveType();
                    var LT_data = LeaveTypeContext.Collection().Where(a => a.Id == idData).ToList();
                    foreach (var item in LT_data)
                    {
                        lt.Name = item.Name;
                        lt.Id = item.Id;
                    }
                    viewname = "LeaveType";
                    ViewBag.Name = "DeleteLeaveType";
                    ViewBag.HeadingName = "Delete Leave Type";
                    ViewBag.HeadingColor = "bg-danger";
                    return View(viewname, lt);
                //break;


                case "LeaveMaster":

                    LeaveMaster lm = new LeaveMaster();
                    var LQ_data = (from leaveMaster in LeaveMasterContext.Collection().ToList()

                                   join orgC in OrganisationContext.Collection().ToList()
                                   on leaveMaster.OrganisationId equals orgC.Id

                                   join ltC in LeaveTypeContext.Collection().ToList()
                                   on leaveMaster.LeaveTypeId equals ltC.Id

                                   select new
                                   {
                                       Organisation = orgC.Name,
                                       LeaveType = ltC.Name,
                                       leaveMaster.AnnualQuota,
                                       leaveMaster.ValidFrom,
                                       leaveMaster.ValidTill,
                                       leaveMaster.Id


                                   }

                        ).Where(x => x.Id == idData).ToList();

                    foreach (var item in LQ_data)
                    {
                        lm.AnnualQuota = item.AnnualQuota;
                        lm.ValidFrom = item.ValidFrom;
                        lm.ValidTill = item.ValidTill;
                        ViewBag.LName = item.LeaveType;
                        ViewBag.OName = item.Organisation;
                        lm.Id = item.Id;

                    }

                    ViewBag.Action = "DeleteLeaveMaster";
                    List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
                    ViewBag.OrganisationList = orgList;

                    List<LeaveType> LeaveType = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
                    ViewBag.LevelTypeList = LeaveType;

                    viewname = "LeaveMaster";
                    return View(viewname, lm);
            }
            return View(viewname);
        }

        #region  Leave Master Code Start 
        [HttpPost]
        public ActionResult AddLeaveQuota(LeaveMaster l)
        {

            if (ModelState.IsValid)
            {
                LeaveMasterContext.Insert(l);
                LeaveMasterContext.Commit();
                ViewBag.Msg = "Leave Quota added succesfully";


            }
            ViewBag.ReportTitle = "- ";
            return View("LeaveMaster");
        }

        [HttpPost]
        public ActionResult UpdateLeaveMaster(LeaveMaster l)
        {
            if (ModelState.IsValid)
            {
                LeaveMasterContext.Update(l);
                LeaveMasterContext.Commit();
                ViewBag.Msg = "Leave Quota updated succesfully";
            }
            return View("LeaveMaster");
        }

        [HttpPost]
        public ActionResult DeleteLeaveMaster(LeaveMaster l)
        {
            if (ModelState.IsValid)
            {
                LeaveMasterContext.Delete(l.Id);
                LeaveMasterContext.Commit();
                ViewBag.Msg = "Leave Quota deleted succesfully";
            }
            return View("LeaveMaster");

        }


        #endregion

        #region  Leave type Code Start 
        public ActionResult LeaveType()
        {
            ViewBag.HeadingName = "Add Leave Type";
            ViewBag.HeadingColor = "bg-success";
            ViewBag.Name = "";
            return View("LeaveType");
        }

        [HttpPost]
        public ActionResult AddLeaveType(LeaveType lt)
        {
            string returnText = "Error";
            if (ModelState.IsValid)
            {
                LeaveTypeContext.Insert(lt);
                LeaveTypeContext.Commit();
                returnText = "Success";
            }
            if (returnText == "Success")
            {
                ViewBag.HeadingName = "Add Leave Type";
                ViewBag.HeadingColor = "bg-success";
                ViewBag.Msg = "Leave Type added succesfully";
            }
            return View("LeaveType");
        }

        public ActionResult ViewLeaveType()
        {
            DataTable dt_ = null;
            var LQ_data = (from LeaveType in LeaveTypeContext.Collection().ToList()
                           select new
                           {
                               LeaveType.Name,
                               LeaveType.CreatedAt,
                               LeaveType.LastUpdateAt,
                               LeaveType.LastUpdateBy,
                               LeaveType.Id
                           }).ToList();

            dt_ = ToDataTable(LQ_data);
            ViewBag.ReportTitle = " Leave Type ";
            ViewBag.targetmodel = "LeaveType";
            ViewBag.HeadingName = "View Leave Type";
            ViewBag.HeadingColor = "bg-info";
            return View("ShowResponse", dt_);
        }

        [HttpPost]
        public ActionResult UpdateLeaveType(LeaveType lt)
        {
            string returnText = "Error";
            if (ModelState.IsValid)
            {
                LeaveTypeContext.Update(lt);
                LeaveTypeContext.Commit();
                returnText = "Success";
            }
            if (returnText == "Success")
            {
                ViewBag.HeadingName = "Update Leave Type";
                ViewBag.HeadingColor = "bg-warning ";
                ViewBag.Msg = "Leave Type Update succesfully";
            }
            else
            {
                return View("LeaveType");
            }
            return View("LeaveType");
        }

        [HttpPost]
        public ActionResult DeleteLeaveType(LeaveType lt)
        {
            string returnText = "Error";
            if (ModelState.IsValid)
            {
                LeaveTypeContext.Delete(lt.Id);
                LeaveTypeContext.Commit();
                returnText = "Success";
            }
            if (returnText == "Success")
            {
                ViewBag.HeadingName = "Delete Leave Type";
                ViewBag.HeadingColor = "bg-danger";
                ViewBag.Msg = "Leave Type Delete succesfully";
            }
            else
            {
                return View("LeaveType");
            }
            return View("LeaveType");
        }
        #endregion


        #region 'Apply Leave'
        [HttpGet]
        public ActionResult ApplyLeave()
        {
            List<EmployeeDetail> EmployeeList = EmployeeDetailContext.Collection().Where(q => q.WorkingStatus == true).ToList();
            ViewBag.EmlployeeTypeList = EmployeeList;

            List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.OrganisationList = orgList;

            List<LeaveType> LeaveType = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.LevelTypeList = LeaveType;

            ViewBag.HeadingName = "Apply Leave";
            ViewBag.HeadingColor = "bg-success";
            ViewBag.Name = "";
            return View();
        }

        [HttpPost]
        public ActionResult ApplyLeaveApplication(EmployeeLeaveDetails Eld)
        {
            string returnText = "Error";
            if (ModelState.IsValid)
            {
                EmployeeLeaveDetailsContext.Insert(Eld);
                EmployeeLeaveDetailsContext.Commit();
                returnText = "Success";
            }
            if (returnText == "Success")
            {
                EmpLeaveBalCount(Eld);
                ViewBag.HeadingName = "Add Leave Type";
                ViewBag.HeadingColor = "bg-success";
                ViewBag.Msg = "Leave Apply Succesfully";
            }
            return View("ApplyLeave");
        }

        public PartialViewResult GetOrganisation_Leave(string EmployeeId)
        {

            var Edata = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId).ToList();
            LeaveMaster lm = new LeaveMaster();

            
            foreach (var item in Edata)
            {
                lm.Organisation = item.Organisation;
                ViewBag.orgName = item.Organisation.Name;
                ViewBag.orgID = item.Organisation.Id;
            }

            var LeaveBalance = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()
                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id
                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id
                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id
                          select new
                          {                             
                              Organisation = org.Name,
                              Leave = le.Name,
                              el.NoOfDays,
                              FromDate = el.LeaveFrom,
                              ToDate = el.LeaveTill,
                              el.Purpose
                          }
                      ).ToList();

        
            //LeaveType lt = new LeaveType();
            //var LT_data = LeaveTypeContext.Collection().Where(a => a.Id == idData).ToList();
            //foreach (var item in LT_data)
            //{
            //    lt.Name = item.Name;
            //    lt.Id = item.Id;
            //}

            //var employeeModel = new EmployeeModel();
            //employeeModel.listEmp = GetEmployeeDataFromDB();
            //var emp = employeeModel.listEmp.Where(e => e.EmployeeId == EmployeeId).FirstOrDefault();
            ////Set default emp records  
            //employeeModel.EmployeeId = emp.EmployeeId;
            //employeeModel.EmpName = emp.EmpName;
            //employeeModel.Salary = emp.Salary;
            // return PartialView("_OrgPartial", employeeModel);
            return PartialView("_OrgPartial", lm);
        }


        #endregion




        public ActionResult ViewLeaveDetails()
        {
            return View();
        }

        public string GetOrgForDropDown()
        {
            string jsondata = String.Empty;

            List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
            DataTable dt = ToDataTable(orgList);
            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;
        }

        public string GetEmpName(MultiSelect Oid)
        {

            string jsondata = String.Empty;
            DataTable dt = null;
            var employees1 = (from employee in EmployeeDetailContext.Collection().ToList()

                              join org in OrganisationContext.Collection().ToList()
                              on employee.OrganisationId equals org.Id into xx
                              from y in xx.DefaultIfEmpty()
                              where Oid.orgList.Contains(employee.OrganisationId)
                              select new
                              {
                                  employee.EmployeeCode,
                                  FullName = employee.FirstName + " " + (employee.MiddleName == "" ? "" : employee.MiddleName + " ") + employee.LastName,
                                  Organisation = y == null ? "" : y.Name,
                                  employee.OrganisationId,
                                  employee.Id

                              })
               .Distinct().ToList();

            dt = ToDataTable(employees1);

            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;
        }

        [HttpPost]
        public ActionResult ViewLeaveDetails(MultiSelect m)
        {

            var data = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()
                         
                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id


                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id

                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id

                          where (m.orgList.Contains(el.OrganisationId) && m.empList.Contains(el.EmployeeId))

                          select new
                          {
                              FullName = emp.FirstName + " " + (emp.MiddleName == "" ? "" : emp.MiddleName + " ") + emp.LastName,
                              Organisation = org.Name,
                              Leave = le.Name,
                              el.NoOfDays,
                              FromDate = el.LeaveFrom,
                              ToDate = el.LeaveTill,
                              el.Purpose

                          }


                      ).ToList();

            DataTable dt = ToDataTable(data);

            ViewBag.HeadingName = "Employee Leave Details";
            ViewBag.HeadingColor = "bg-success";

            return View("ShowResponse", dt);

        }


        public ActionResult ViewAllLeaveDetails()
        {

            var data = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()
                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id


                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id

                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id



                          select new
                          {
                              FullName = emp.FirstName + " " + (emp.MiddleName == "" ? "" : emp.MiddleName + " ") + emp.LastName,
                              Organisation = org.Name,
                              Leave = le.Name,
                              el.NoOfDays,
                              FromDate = el.LeaveFrom,
                              ToDate = el.LeaveTill,
                              el.Purpose

                          }


                      ).ToList();

            DataTable dt = ToDataTable(data);

            ViewBag.HeadingName = "Employee Leave Details";
            ViewBag.HeadingColor = "bg-success";

            return View("ShowResponse", dt);

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

        public string EmpLeaveBalCount(EmployeeLeaveDetails e)
        {
            var check = (from eLB in EmployeeLeaveBalanceContext.Collection().ToList()
                         where eLB.EmployeeId
                         == e.EmployeeId
                         select eLB
                        ).Count();

            if (check == 0)
            {

                var data = (from emp in EmployeeDetailContext.Collection().ToList()

                            join org in OrganisationContext.Collection().ToList()
                            on emp.OrganisationId equals org.Id

                            join lm in LeaveMasterContext.Collection().ToList()
                            on emp.OrganisationId equals lm.OrganisationId

                            //where lm.LeaveTypeId == e.LeaveTypeId && org.Id == e.OrganisationId
                            select new
                            {
                                TotalLeaveCount = lm.AnnualQuota,
                                AvailableLeaveCount = Convert.ToInt32(lm.AnnualQuota) - Convert.ToInt32(e.NoOfDays),
                                lm.LeaveTypeId,
                                EmployeeId = emp.Id,
                                OrganisationId=emp.OrganisationId



                            }
                            ).ToList();

                EmployeeLeaveBalance Eb = new EmployeeLeaveBalance();
                foreach (var i in data.Where(x=>x.EmployeeId==e.EmployeeId))
                {
                    Eb.EmployeeId = i.EmployeeId;
                    Eb.LeaveTypeId = i.LeaveTypeId;
                    Eb.AvailableLeaveCount = i.AvailableLeaveCount.ToString();
                    Eb.TotalLeaveCount = i.TotalLeaveCount.ToString();
                }
                EmployeeLeaveBalanceContext.Insert(Eb);
                EmployeeLeaveBalanceContext.Commit();

                return "success";
            }

            else
            {
                return "success";
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
    }
}