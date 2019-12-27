using EmployeeInformationSystem.Core.Contracts;
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
        IRepository<Designation> DesignationContext;
        IRepository<Level> LevelContext;
        IRepository<Degree> DegreeContext;
        IRepository<HoD> HoDContext;
        IRepository<Organisation> OrganisationContext;
        IRepository<PayScale> PayScaleContext;
        IRepository<Department> DepartmentContext;
        IRepository<TelephoneExtension> TelephoneExtensionContext;

        public EmployeeDataController(IRepository<EmployeeDetail> employeeDetailContext,
        IRepository<Discipline> disciplineContext,
        IRepository<Level> levelContext,
        IRepository<Degree> degreeContext,
        IRepository<Designation> designationContext,
        IRepository<HoD> hoDContext,
        IRepository<Organisation> organisationContext,
        IRepository<PayScale> payScaleContext,
        IRepository<Department> departmentContext,
        IRepository<TelephoneExtension> telephoneExtensionContext)
        {
            EmployeeDetailContext = employeeDetailContext;
            DisciplineContext = disciplineContext;
            LevelContext = levelContext;
            DegreeContext = degreeContext;
            DesignationContext = designationContext;
            HoDContext = hoDContext;
            OrganisationContext = organisationContext;
            PayScaleContext = payScaleContext;
            DepartmentContext = departmentContext;
            TelephoneExtensionContext = telephoneExtensionContext;
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult AjaxAdd(string targetPage)
        {
            object genericObject = null;
            switch (targetPage)
            {
                case "Deputationist":
                    genericObject = new DeputationistsViewModel()
                    {
                        EmployeeDetails = new EmployeeDetail(),
                        Degrees = DegreeContext.Collection(),
                        Organisations = OrganisationContext.Collection(),
                        Disciplines = DisciplineContext.Collection(),
                        Designations = DesignationContext.Collection(),
                        Levels = LevelContext.Collection(), // Add proper support for separating levels
                        PayScales = PayScaleContext.Collection(),
                        Departments = DepartmentContext.Collection(),
                        HoDs = HoDContext.Collection() //Add proper support for determining HoDs
                    };
                    break;
                case "Consultant": break;
                case "Contractual": break;
                default: break;
            }
            return View(targetPage, genericObject);
        }
    }
}