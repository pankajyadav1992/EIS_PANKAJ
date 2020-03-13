﻿using EmployeeInformationSystem.Core.Contracts;
using EmployeeInformationSystem.Core.Models;
using EmployeeInformationSystem.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeInformationSystem.Core.ViewModels
{
    public class ReportSelectionViewModel
    {
        private ManipulateData manipulateData
        {
            get
            {
                return new ManipulateData();
            }
        }

        [Display(Name = "Employee Categories")]
        public IEnumerable<EmployeeType> Categories { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? From { get; set; }

        [Display(Name = "End Date")]
        public DateTime? To { get; set; }

        [Display(Name = "Departments")]
        public IEnumerable<Department> Departments { get; set; }

        [Display(Name = "Personal Details")]
        public IEnumerable<String> PersonalDetailsColumns { get; set; }

        [Display(Name = "Contact Details")]
        public IEnumerable<String> ContactDetailsColumns { get; set; }

        [Display(Name = "Professional Details")]
        public IEnumerable<String> ProfessionalDetailsColumns { get; set; }

        [Display(Name = "Promotion Details")]
        public IEnumerable<String> PromotionDetailsColumns { get; set; }

        public IEnumerable<SelectListItem> AllCategories
        {
            get
            {
                return (from EmployeeType e in Enum.GetValues(typeof(EmployeeType))
                        select new SelectListItem { Value = e.ToString(), Text = e.GetDisplayName() }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllDepartments { get; set; }

        public IEnumerable<SelectListItem> AllPersonalDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("personalDetails")
                        select new SelectListItem { Value = column, Text = column }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllContactDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("contactDetails")
                        select new SelectListItem { Value = column, Text = column }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllProfessionalDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("professionalDetails")
                        select new SelectListItem { Value = column, Text = column }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllPromotionDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("promotionDetails")
                        select new SelectListItem { Value = column, Text = column }).AsEnumerable<SelectListItem>();
            }
        }

    }
}
