﻿using EmployeeInformationSystem.Core.Contracts;
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
        IRepository<Level> LevelContext;
        IRepository<Organisation> OrganisationContext;
        IRepository<TelephoneExtension> TelephoneExtensionContext;

        public EmployeeDataController(IRepository<EmployeeDetail> employeeDetailContext,
        IRepository<Discipline> disciplineContext,
        IRepository<Level> levelContext,
        IRepository<Organisation> organisationContext,
        IRepository<TelephoneExtension> telephoneExtensionContext)
        {
            EmployeeDetailContext = employeeDetailContext;
            DisciplineContext = disciplineContext;
            LevelContext = levelContext;
            OrganisationContext = organisationContext;
            TelephoneExtensionContext = telephoneExtensionContext;
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult AjaxAdd(string targetPage)
        {
            object genericObject=null;
            switch (targetPage)
            {
                case "Deputationist":
                    genericObject = new DeputationistsViewModel();
                    break;
                case "Consultant": break;
                case "Contractual": break;
                default: break;
            }
            return View(targetPage, genericObject);
        }
    }
}