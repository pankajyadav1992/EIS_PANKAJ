using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
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
        IRepository<LeaveMaster> leaveMasterContext
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
            return View("ShowResponse", dt_);
        }

        public ActionResult Edit(string idData,string targetmodel)
        {
                
          
            var viewname = "";
            switch (targetmodel)
            {    case "LeaveType":
                    LeaveType lt = new LeaveType();
                   
                    var LT_data = LeaveTypeContext.Collection().Where(a => a.Id == idData).ToList();
                   
                    foreach(var item in LT_data)
                    {
                        lt.Name = item.Name;
                        lt.Id = item.Id;
                    }
                    viewname = "LeaveType";
                    ViewBag.Name= viewname; 
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

                    }
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
                    return View(viewname, lt);
                    //break;
            }
            return View(viewname);
        }


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


        #region  Leave type Code Start 
        public ActionResult LeaveType()
        {
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
                // return View("Success");
                // return Content(returnText);
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
                // return View("Success");
                // return Content(returnText);
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
                // return View("Success");
                // return Content(returnText);
                ViewBag.Msg = "Leave Type Delete succesfully";
            }
            else
            {
                return View("LeaveType");
            }
            return View("LeaveType");
        }



        #endregion

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