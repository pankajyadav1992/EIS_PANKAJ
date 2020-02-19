using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.DataAccess.SQL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController()
        {
            base.SetGlobalParameters();
            ViewBag.UserName = UserName;
            ViewBag.ProfilePhoto = ProfilePicture;
            ViewBag.Role = Role;
            
        }
        public ActionResult Index()
        {
            IRepository<EmployeeDetail> _localEmployeeContext = new SQLRepository<EmployeeDetail>(new DataContext());

            ViewBag.TotalEmployees = _localEmployeeContext.Collection().Count();
            ViewBag.ActiveEmployees = _localEmployeeContext.Collection().Where(e => e.WorkingStatus == true).Count();
            ViewBag.InActiveEmployees = (int)ViewBag.TotalEmployees - (int)ViewBag.ActiveEmployees;
            DateTime recentCutOff = DateTime.Now.AddDays(-30);
            ViewBag.RecentJoinees = _localEmployeeContext.Collection().Where(e => e.DateofJoiningDGH > recentCutOff).Count();
            return View();
        }
    }
}