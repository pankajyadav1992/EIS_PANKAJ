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
    [Authorize(Roles = "Manage Leave")]
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


            ViewBag.targetmodel = "LeaveMaster";
            ViewBag.HeadingName = "Add Leave Quota";
            ViewBag.HeadingColor = "bg-success";

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
            ViewBag.HeadingName = "View Leave Quota";
            ViewBag.HeadingColor = "bg-info";
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
                               //LeaveType.CreatedAt,
                               //LeaveType.LastUpdateAt,
                               //LeaveType.LastUpdateBy,
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
                try
                {
                    LeaveTypeContext.Delete(lt.Id);
                    LeaveTypeContext.Commit();
                    returnText = "Success";
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
                }
                catch (Exception ex)
                {
                    // throw ex;
                    return View("ErrorPage");
                }              
            }
            
            return View("LeaveType");
        }
        #endregion


        #region 'Apply Leave'
        [HttpGet]
        public ActionResult AddLeavesDue()
        {
            List<EmployeeDetail> EmployeeList = EmployeeDetailContext.Collection().Where(q => q.WorkingStatus == true).OrderBy(e => e.FirstName).ToList();
            ViewBag.EmlployeeTypeList = EmployeeList;

            List<LeaveType> LeaveTypeList = LeaveTypeContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.TypeList = LeaveTypeList;
            ViewBag.HeadingName = "Add Leaves Due";
            ViewBag.HeadingColor = "bg-success";
            ViewBag.Name = "";
            return View();
        }

        [HttpPost]
        public ActionResult AddLeavesDue(EmployeeLeaveBalance ELB)
        {
            if (ModelState.IsValid)
            {
                var chkleavedata = EmployeeLeaveBalanceContext.Collection().ToList().Where(a => a.LeaveTypeId == ELB.LeaveTypeId && a.EmployeeId == ELB.EmployeeId).Count();
                if (chkleavedata != 0)
                {
                    var leavedata = EmployeeLeaveBalanceContext.Collection().ToList().Where(a => a.LeaveTypeId == ELB.LeaveTypeId && a.EmployeeId == ELB.EmployeeId).ToList();
                    EmployeeLeaveBalance Eb = EmployeeLeaveBalanceContext.Collection().FirstOrDefault(q => q.EmployeeId == ELB.EmployeeId && q.LeaveTypeId == ELB.LeaveTypeId);
                    foreach (var i in leavedata)
                    {
                        Eb.Id = i.Id;
                        Eb.EmployeeId = i.EmployeeId;
                        Eb.LeaveTypeId = i.LeaveTypeId;
                        Eb.AvailableLeaveCount = Convert.ToString(Convert.ToInt32(i.AvailableLeaveCount) + Convert.ToInt32(ELB.AvailableLeaveCount));
                        Eb.TotalLeaveCount = Convert.ToString(Convert.ToInt32(i.TotalLeaveCount) + Convert.ToInt32(ELB.AvailableLeaveCount));
                    }
                    EmployeeLeaveBalanceContext.Update(Eb);
                    EmployeeLeaveBalanceContext.Commit();
                    ViewBag.HeadingName = "Add Leaves Due";
                    ViewBag.HeadingColor = "bg-success";
                    ViewBag.Msg = "Add Leaves Due Apply Succesfully";
                }
                else
                {
                    //var OrgId =EmployeeDetailContext.Collection().ToList().Where(a => a.LeaveTypeId == ELB.LeaveTypeId).ToList();
                    //var AnnualQuota=LeaveMasterContext.Collection().ToList().Where(a => a.LeaveTypeId == ELB.LeaveTypeId).ToList();
                    //EmployeeLeaveBalance Eb = new EmployeeLeaveBalance();
                    //foreach (var i in LeaveQuota)
                    //{
                    //    Eb.Id = Eb.Id;
                    //    Eb.EmployeeId = ELB.EmployeeId;
                    //    Eb.LeaveTypeId = ELB.EmployeeId;
                    //    Eb.AvailableLeaveCount = Convert.ToString(Convert.ToInt32(i.AnnualQuota) + Convert.ToInt32(ELB.AvailableLeaveCount));
                    //    Eb.TotalLeaveCount = Convert.ToString(Convert.ToInt32(i.AnnualQuota) + Convert.ToInt32(ELB.AvailableLeaveCount));
                    //}
                    //EmployeeLeaveBalanceContext.Insert(Eb);
                    //EmployeeLeaveBalanceContext.Commit();
                    //ViewBag.HeadingName = "Add Leaves Due";
                    //ViewBag.HeadingColor = "bg-success";
                    //ViewBag.Msg = "Add Leaves Due Apply Succesfully";
                }
            }
            return View("AddLeavesDue");
        }




        [HttpGet]
        public ActionResult ApplyLeave()
        {
            List<EmployeeDetail> EmployeeList = EmployeeDetailContext.Collection().Where(q => q.WorkingStatus == true).OrderBy(e => e.FirstName).ToList();
            ViewBag.EmlployeeTypeList = EmployeeList;
            ViewBag.LevelTypeList = null;
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
                //Apply Leave First Time By Employee

                var LeaveAnnualQuota = LeaveMasterContext.Collection().ToList().Where(a => a.LeaveTypeId == Eld.LeaveTypeId && a.OrganisationId == Eld.OrganisationId).
                    Select(a => a.AnnualQuota).SingleOrDefault();
                //Apply Leave Second Time By Employee
                var AvailableLeave = EmployeeLeaveBalanceContext.Collection().ToList().Where(aa => aa.EmployeeId == Eld.EmployeeId && aa.LeaveTypeId == Eld.LeaveTypeId).
                    Select(aa => aa.AvailableLeaveCount).SingleOrDefault();
                if (Convert.ToInt32(AvailableLeave) >= Convert.ToInt32(Eld.NoOfDays) || AvailableLeave == null && Convert.ToInt32(LeaveAnnualQuota) >= Convert.ToInt32(Eld.NoOfDays))
                {
                    EmployeeLeaveDetailsContext.Insert(Eld);
                    EmployeeLeaveDetailsContext.Commit();
                    returnText = "Success";
                    if (returnText == "Success")
                    {
                        EmpLeaveBalCount(Eld);
                        ViewBag.HeadingName = "Apply Leave";
                        ViewBag.HeadingColor = "bg-success";
                        ViewBag.Msg = "Leave Apply Succesfully";
                    }
                }
                else
                {
                    List<EmployeeDetail> EmployeeList = EmployeeDetailContext.Collection().Where(q => q.WorkingStatus == true).OrderBy(e => e.FirstName).ToList();
                    ViewBag.EmlployeeTypeList = EmployeeList;
                    ViewBag.Message = "Check Available Leave In Leave Type ";
                    //return View("ApplyLeave");
                }
            }
            return View("ApplyLeave");
        }

        public PartialViewResult GetOrganisation_Leave(string EmployeeId)
        {
            var Edata = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).ToList().Count();
            LeaveMaster lm = new LeaveMaster();
            if (Edata != 0)
            {
                var org = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).Select(q => new { q.OrganisationId, q.Organisation.Name }).ToList();
                foreach (var item in org)
                {
                    ViewBag.orgName = item.Name;
                    ViewBag.orgID = item.OrganisationId;
                }
                var LeaveType = (from edc in EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).ToList()
                                 join oc in OrganisationContext.Collection().ToList()
                                 on edc.OrganisationId equals oc.Id
                                 join lmc in LeaveMasterContext.Collection().ToList()
                                 on oc.Id equals lmc.OrganisationId
                                 join ltc in LeaveTypeContext.Collection().ToList()
                                 on lmc.LeaveTypeId equals ltc.Id
                                 select new LeaveBalance
                                 {
                                     LeaveId = ltc.Id,
                                     LeaveType = ltc.Name
                                 }
                                 ).ToList();
                ViewBag.LevelTypeList = LeaveType;
                var check = (from eLB in EmployeeLeaveBalanceContext.Collection().ToList() where eLB.EmployeeId == EmployeeId select eLB).Count();
                if (check == 0)
                {
                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()
                                    join
                                    lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId
                                    join
                                    ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id
                                    where (edc.Id == EmployeeId)
                                    select new LeaveBalance
                                    {
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,
                                        //LeaveType= ltc.Name,
                                        // LeaveCount= lmc.AnnualQuota,
                                        // // edc.Organisation,
                                        // // edc.FirstName,
                                        // EmpId=  edc.Id,
                                    }).ToList();
                    ViewBag.Balance = LeaveBal;
                }
                else
                {
                    var balleave = EmployeeLeaveBalanceContext.Collection().Where(e => e.EmployeeId == EmployeeId).Select(e => e.LeaveTypeId).ToList();
                    // var query = LeaveTypeContext.Collection().Select(i => i.Id).Where(x => !exceptionList.Contains(x)).ToList(); 
                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()
                                    join lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId
                                    join ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id
                                    where (edc.Id == EmployeeId && !balleave.Contains(ltc.Id))
                                    select new LeaveBalance
                                    {
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,
                                    })
                                    .Concat
                                    (from ltc in LeaveTypeContext.Collection().ToList()
                                     join
                                     elbc in EmployeeLeaveBalanceContext.Collection().ToList()
                                     on ltc.Id equals elbc.LeaveTypeId
                                     where (elbc.EmployeeId == EmployeeId)
                                     select new LeaveBalance
                                     {
                                         LeaveCount = elbc.AvailableLeaveCount,
                                         LeaveType = ltc.Name,
                                     }
                                    ).ToList();
                    ViewBag.Balance = LeaveBal;
                }
            }
            ViewBag.Message = "";
            return PartialView("_OrgPartial");
        }



        public string Get_Leave_Employee(string EmployeeId)
        {
            string jsondata = String.Empty;
            var Edata = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).ToList().Count();
            LeaveMaster lm = new LeaveMaster();
            if (Edata != 0)
            {
                var org = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).Select(q => new { q.OrganisationId, q.Organisation.Name }).ToList();
                foreach (var item in org)
                {
                    ViewBag.orgName = item.Name;
                    ViewBag.orgID = item.OrganisationId;
                }
                var LeaveType = (from edc in EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId && q.OrganisationId != null).ToList()
                                 join oc in OrganisationContext.Collection().ToList()
                                 on edc.OrganisationId equals oc.Id
                                 join lmc in LeaveMasterContext.Collection().ToList()
                                 on oc.Id equals lmc.OrganisationId
                                 join ltc in LeaveTypeContext.Collection().ToList()
                                 on lmc.LeaveTypeId equals ltc.Id
                                 select new LeaveBalance
                                 {
                                     LeaveId = ltc.Id,
                                     LeaveType = ltc.Name
                                 }
                                 ).ToList();
                ViewBag.LevelTypeList = LeaveType;
                var check = (from eLB in EmployeeLeaveBalanceContext.Collection().ToList() where eLB.EmployeeId == EmployeeId select eLB).Count();
                if (check == 0)
                {
                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()
                                    join
                                    lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId
                                    join
                                    ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id
                                    where (edc.Id == EmployeeId)
                                    select new LeaveBalance
                                    {
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,
                                    }).ToList();
                    DataTable dt = ToDataTable(LeaveBal);
                    jsondata = JsonConvert.SerializeObject(dt);
                }
                else
                {
                    var balleave = EmployeeLeaveBalanceContext.Collection().Where(e => e.EmployeeId == EmployeeId).Select(e => e.LeaveTypeId).ToList();
                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()
                                    join lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId
                                    join ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id
                                    where (edc.Id == EmployeeId && !balleave.Contains(ltc.Id))
                                    select new LeaveBalance
                                    {
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,
                                    })
                                    .Concat
                                    (from ltc in LeaveTypeContext.Collection().ToList()
                                     join
                                     elbc in EmployeeLeaveBalanceContext.Collection().ToList()
                                     on ltc.Id equals elbc.LeaveTypeId
                                     where (elbc.EmployeeId == EmployeeId)
                                     select new LeaveBalance { LeaveCount = elbc.AvailableLeaveCount,  LeaveType = ltc.Name,}   ).ToList();
                    // ViewBag.Balance = LeaveBal;
                    DataTable dt = ToDataTable(LeaveBal);
                    jsondata = JsonConvert.SerializeObject(dt);
                }                
            }         
            return jsondata;
        }



        public string Get_Leave_List_Employee(string EmployeeId)
        {
            string jsondata = String.Empty;
            var OrgId = EmployeeDetailContext.Collection().Where(q => q.Id == EmployeeId).ToList();
            LeaveMaster lm = new LeaveMaster();
            foreach (var item in OrgId)
            {
                lm.OrganisationId = item.OrganisationId;
            }
            var LeaveType = LeaveMasterContext.Collection().Where(q => q.OrganisationId == lm.OrganisationId)
                .Select(q => new { q.LeaveTypeId, q.LeaveType.Name }).ToList();                  
            DataTable dt = ToDataTable(LeaveType);
            jsondata = JsonConvert.SerializeObject(dt); 
            return jsondata;
        }


        #endregion




        public ActionResult ViewLeaveDetails()
        {
            return View();
        }

        public ActionResult ViewOrgLeaveDetails()
        {
            List<Organisation> orgList = OrganisationContext.Collection().OrderBy(e => e.Name).ToList();
            ViewBag.OrganisationList = orgList;
            return View();
        }

        public string ViewOrgLeaveReport(LeaveReportsViewModel m)
        {
            string jsondata = String.Empty;

            var data = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()

                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id


                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id

                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id

                          where (el.OrganisationId == m.orgId)


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


                      ).Where(x => Convert.ToDateTime(x.FromDate) >= Convert.ToDateTime(m.Fromdate) && Convert.ToDateTime(x.FromDate) <= Convert.ToDateTime(m.ToDate)
                      || Convert.ToDateTime(x.ToDate) >= Convert.ToDateTime(m.Fromdate) && Convert.ToDateTime(x.ToDate) <= Convert.ToDateTime(m.ToDate)
                      )
                      .OrderBy(x => x.FullName).ToList();

            DataTable dt = ToDataTable(data);

            ViewBag.HeadingName = "Organizations Leave Details";
            ViewBag.HeadingColor = "bg-success";

            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;

        }



        public ActionResult DurationWiseLeaves()
        {
            return View();
        }

        public string DurationWiseReport(LeaveReportsViewModel m)
        {
            string jsondata = String.Empty;

            var data = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()

                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id


                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id

                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id

                          //where (el.OrganisationId == m.orgId)


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


                      ).Where(x => Convert.ToDateTime(x.FromDate) >= Convert.ToDateTime(m.Fromdate) && Convert.ToDateTime(x.FromDate) <= Convert.ToDateTime(m.ToDate)
                      || Convert.ToDateTime(x.ToDate) >= Convert.ToDateTime(m.Fromdate) && Convert.ToDateTime(x.ToDate) <= Convert.ToDateTime(m.ToDate)
                      )
                      .OrderBy(x => x.FullName).ToList();

            DataTable dt = ToDataTable(data);

            ViewBag.HeadingName = "Duration Wise Leave Details";
            ViewBag.HeadingColor = "bg-success";

            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;
        }

        public ActionResult DailyLeave()
        {
            ViewBag.CurrentDate = DateTime.Now.ToString("dd-MM-yyyy");
            return View();
        }

        public string DailyLeaveReport(LeaveReportsViewModel m)
        {
            string jsondata = String.Empty;
            string currentdate = DateTime.Now.ToString("dd-MM-yyyy");

            var data = (
                          from el in EmployeeLeaveDetailsContext.Collection().ToList()

                          join emp in EmployeeDetailContext.Collection().ToList()
                          on el.EmployeeId equals emp.Id


                          join org in OrganisationContext.Collection().ToList()
                          on el.OrganisationId equals org.Id

                          join le in LeaveTypeContext.Collection().ToList()
                          on el.LeaveTypeId equals le.Id

                          //where (el.OrganisationId == m.orgId)


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


                      ).Where(x => Convert.ToDateTime(x.FromDate) <= Convert.ToDateTime(currentdate) && Convert.ToDateTime(x.ToDate) >= Convert.ToDateTime(currentdate)
                      
                      )
                      .OrderBy(x => x.FullName).ToList();

            DataTable dt = ToDataTable(data);

            ViewBag.HeadingName = "Daily Leave Details";
            ViewBag.HeadingColor = "bg-success";

            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;
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


        public string ViewLeaveDetailsByAjax(MultiSelect m)
        {
            string jsondata = String.Empty;

            DataTable dt = new DataTable();

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
                              el.Purpose,
                              YearFrom = Convert.ToDateTime(el.LeaveFrom).ToString("yyyy"),
                              YearTill = Convert.ToDateTime(el.LeaveTill).ToString("yyyy")


                          }


                      ).OrderBy(x => x.FullName).ToList();

            if (m.YearValue == "0")
            {

                dt = ToDataTable(data);

            }
            else
            {
                var newdata = data.Where(x => Convert.ToInt64(x.YearFrom) >= Convert.ToInt64(m.YearValue) && Convert.ToInt64(x.YearTill) <= Convert.ToInt64(m.YearValue)).ToList();
                dt = ToDataTable(newdata);
            }

            ViewBag.HeadingName = "Employee Leave Details";
            ViewBag.HeadingColor = "bg-success";

            jsondata = JsonConvert.SerializeObject(dt);

            return jsondata;

        }




        public ActionResult ViewEmpLeaveBalance()
        {
            return View();
        }

        public string EmpLeaveBalance(MultiSelect m)
        {
            string jsondata = String.Empty;
            DataTable dt = null;
            List<LeaveBalance> ResultList = new List<LeaveBalance>();

            for (int i = 0; i < m.empList.Count(); i++)
            {


                var check = (from eLB in EmployeeLeaveBalanceContext.Collection().ToList()
                             where eLB.EmployeeId == m.empList[i]

                             select eLB.EmployeeId
                            ).Distinct().Count();
                if (check == 0)
                {
                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()
                                    join
                                    lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId
                                    join
                                    ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id

                                    join org in OrganisationContext.Collection().ToList()
                                    on edc.OrganisationId equals org.Id

                                    where (edc.Id == m.empList[i])
                                    select new
                                    {

                                        EmpName = edc.FirstName + " " + (edc.MiddleName == "" ? "" : edc.MiddleName + " ") + edc.LastName,
                                        OrganisationName = org.Name,
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,

                                    }).OrderBy(x => x.EmpName).ToList();



                    foreach (var data in LeaveBal)
                    {
                        LeaveBalance item = new LeaveBalance();
                        item.EmpName = data.EmpName;
                        item.OrganisationName = data.OrganisationName;
                        item.LeaveType = data.LeaveType;
                        item.LeaveCount = data.LeaveCount;
                        ResultList.Add(item);

                    }
                }
                else
                {
                    var balleave = (from lb in EmployeeLeaveBalanceContext.Collection().ToList()
                                    where (lb.EmployeeId == m.empList[i])
                                    select lb.LeaveTypeId).ToList();




                    var LeaveBal = (from edc in EmployeeDetailContext.Collection().ToList()

                                    join lmc in LeaveMasterContext.Collection().ToList()
                                    on edc.OrganisationId equals lmc.OrganisationId

                                    join ltc in LeaveTypeContext.Collection().ToList()
                                    on lmc.LeaveTypeId equals ltc.Id

                                    join org in OrganisationContext.Collection().ToList()
                                on edc.OrganisationId equals org.Id
                                    where ((edc.Id == m.empList[i]) && !balleave.Contains(ltc.Id))
                                    select new LeaveBalance
                                    {
                                        EmpName = edc.FirstName + " " + (edc.MiddleName == "" ? "" : edc.MiddleName + " ") + edc.LastName,
                                        OrganisationName = org.Name,
                                        LeaveType = ltc.Name,
                                        LeaveCount = lmc.AnnualQuota,
                                    })
                                        .Concat
                                        (from ltc in LeaveTypeContext.Collection().ToList()

                                         join elbc in EmployeeLeaveBalanceContext.Collection().ToList()
                                         on ltc.Id equals elbc.LeaveTypeId

                                         join edc in EmployeeDetailContext.Collection().ToList()
                                         on elbc.EmployeeId equals edc.Id

                                         join org in OrganisationContext.Collection().ToList()
                                         on edc.OrganisationId equals org.Id

                                         where (elbc.EmployeeId == m.empList[i])
                                         select new LeaveBalance
                                         {
                                             EmpName = edc.FirstName + " " + (edc.MiddleName == "" ? "" : edc.MiddleName + " ") + edc.LastName,
                                             OrganisationName = org.Name,
                                             LeaveCount = elbc.AvailableLeaveCount,
                                             LeaveType = ltc.Name,
                                         }
                                        ).OrderBy(x => x.EmpName).ToList();


                    foreach (var data in LeaveBal)
                    {
                        LeaveBalance item = new LeaveBalance();
                        item.EmpName = data.EmpName;
                        item.OrganisationName = data.OrganisationName;
                        item.LeaveType = data.LeaveType;
                        item.LeaveCount = data.LeaveCount;
                        ResultList.Add(item);

                    }

                }
            }

            dt = ToDataTable(ResultList);

            jsondata = JsonConvert.SerializeObject(dt);
            return jsondata;
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
                            where emp.Id == e.EmployeeId && org.Id == e.OrganisationId && lm.LeaveTypeId == e.LeaveTypeId
                            select new
                            {
                                TotalLeaveCount = lm.AnnualQuota,
                                AvailableLeaveCount = Convert.ToInt32(lm.AnnualQuota) - Convert.ToInt32(e.NoOfDays),
                                e.LeaveTypeId,
                                EmployeeId = emp.Id,
                                OrganisationId = emp.OrganisationId
                            }
                            ).ToList();
                EmployeeLeaveBalance Eb = new EmployeeLeaveBalance();
                foreach (var i in data.Where(x => x.EmployeeId == e.EmployeeId))
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
                var alredyexist = EmployeeLeaveBalanceContext.Collection().Where(q => q.EmployeeId == e.EmployeeId && q.LeaveTypeId == e.LeaveTypeId).ToList().Count();
                if (alredyexist != 0)
                {
                    var updateid = EmployeeLeaveBalanceContext.Collection().Where(q => q.EmployeeId == e.EmployeeId && q.LeaveTypeId == e.LeaveTypeId).Select(q => q.Id).SingleOrDefault();
                    var availableLeaveCount = EmployeeLeaveBalanceContext.Collection().Where(q => q.EmployeeId == e.EmployeeId && q.LeaveTypeId == e.LeaveTypeId).Select(q => q.AvailableLeaveCount).SingleOrDefault();
                    var totalLeaveCount = EmployeeLeaveBalanceContext.Collection().Where(q => q.EmployeeId == e.EmployeeId && q.LeaveTypeId == e.LeaveTypeId).Select(q => q.TotalLeaveCount).SingleOrDefault();
                    var data = (from elb in EmployeeLeaveBalanceContext.Collection().ToList()
                                where elb.LeaveTypeId == e.LeaveTypeId && elb.EmployeeId == e.EmployeeId
                                select new
                                {
                                    TotalLeaveCount = totalLeaveCount,
                                    AvailableLeaveCount = Convert.ToInt32(availableLeaveCount) - Convert.ToInt32(e.NoOfDays),
                                    e.LeaveTypeId,
                                    EmployeeId = e.EmployeeId,
                                    OrganisationId = e.OrganisationId
                                }
                                   ).ToList();

                    EmployeeLeaveBalance Eb = EmployeeLeaveBalanceContext.Collection().FirstOrDefault(q => q.EmployeeId == e.EmployeeId && q.LeaveTypeId == e.LeaveTypeId);
                    // Student student = db.Students.Find(s => s.StudentID == ViewModel.StudentID);
                    //var Eb = new EmployeeLeaveBalance();
                    foreach (var i in data.Where(x => x.EmployeeId == e.EmployeeId))
                    {
                        Eb.Id = updateid;
                        Eb.EmployeeId = i.EmployeeId;
                        Eb.LeaveTypeId = i.LeaveTypeId;
                        Eb.AvailableLeaveCount = i.AvailableLeaveCount.ToString();
                        Eb.TotalLeaveCount = i.TotalLeaveCount.ToString();
                    }
                    EmployeeLeaveBalanceContext.Update(Eb);
                    EmployeeLeaveBalanceContext.Commit();
                    return "success";
                }
                else
                {
                    var data = (from emp in EmployeeDetailContext.Collection().ToList()
                                join org in OrganisationContext.Collection().ToList()
                                on emp.OrganisationId equals org.Id
                                join lm in LeaveMasterContext.Collection().ToList()
                                on emp.OrganisationId equals lm.OrganisationId
                                where lm.LeaveTypeId == e.LeaveTypeId && org.Id == e.OrganisationId
                                select new
                                {
                                    TotalLeaveCount = lm.AnnualQuota,
                                    AvailableLeaveCount = Convert.ToInt32(lm.AnnualQuota) - Convert.ToInt32(e.NoOfDays),
                                    e.LeaveTypeId,
                                    EmployeeId = emp.Id,
                                    OrganisationId = emp.OrganisationId
                                }
                                ).ToList();
                    EmployeeLeaveBalance Eb = new EmployeeLeaveBalance();
                    foreach (var i in data.Where(x => x.EmployeeId == e.EmployeeId))
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
            }
        }










    }
}