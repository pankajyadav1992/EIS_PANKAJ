using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace EmployeeInformationSystem.WebUI.Controllers
{
    public class BaseController : Controller
    {
        public string UserName, ProfilePicture, Role;
        
        public void SetGlobalParameters()
        {
            // Setting parameters for Home Page
            IRepository<EmployeeDetail> _localEmployeeContext = new SQLRepository<EmployeeDetail>(new DataContext());
            string  UserId;

            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated) UserId = System.Web.HttpContext.Current.User.Identity.Name;
            else UserId = "Anonymous";

            Role = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).HasClaim(ClaimTypes.Role, "Admin") ? "Administrator" : "User";

            UserName = (from employee in _localEmployeeContext.Collection()
                        where employee.EmployeeCode == UserId
                        select employee.FirstName + " " + employee.LastName).FirstOrDefault() ?? "Anonymous";
            ProfilePicture = (from employee in _localEmployeeContext.Collection()
                              where employee.EmployeeCode == UserId
                              select employee.ProfilePhoto).FirstOrDefault() ?? "./Content/img/no-profile-pic-icon.jpg";
        }
    }
}