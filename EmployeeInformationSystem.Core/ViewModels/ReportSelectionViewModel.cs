using EmployeeInformationSystem.Core.Contracts;
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
        public IEnumerable<String> Departments { get; set; }

        [Display(Name = "Organisation")]
        public IEnumerable<String> Organisation { get; set; }
        [Display(Name = "Level")]
        public IEnumerable<String> Level { get; set; }

        [Display(Name = "Personal Details")]
        public IEnumerable<String> PersonalDetailsColumns { get; set; }

        [Display(Name = "Contact Details")]
        public IEnumerable<String> ContactDetailsColumns { get; set; }

        [Display(Name = "Professional Details")]
        public IEnumerable<String> ProfessionalDetailsColumns { get; set; }

        [Display(Name = "Promotion Details")]
        public IEnumerable<String> PromotionDetailsColumns { get; set; }

        [Display(Name = "Custom Columns")]
        public IEnumerable<String> CustomColumns { get; set; }

        public IEnumerable<SelectListItem> AllCategories
        {
            get
            {
                return (from EmployeeType e in Enum.GetValues(typeof(EmployeeType))
                        select new SelectListItem { Value = e.ToString(), Text = e.GetDisplayName() }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllDepartments { get; set; }

        public IEnumerable<SelectListItem> AllOrganizations { get; set; }
        public IEnumerable<SelectListItem> AllLevels { get; set; }



        public IEnumerable<SelectListItem> AllPersonalDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("personalDetails")
                        select new SelectListItem { Value = column.Key, Text = column.Value }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllContactDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("contactDetails")
                        select new SelectListItem { Value = column.Key, Text = column.Value }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllProfessionalDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("professionalDetails")
                        select new SelectListItem { Value = column.Key, Text = column.Value }).AsEnumerable<SelectListItem>();
            }
        }
        public IEnumerable<SelectListItem> AllPromotionDetailsColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList("promotionDetails")
                        select new SelectListItem { Value = column.Key, Text = column.Value }).AsEnumerable<SelectListItem>();
            }
        }

        // Addition for custom reports 
        public string CustomReportType { get; set; }

        public string Working { get; set; }

        public IEnumerable<SelectListItem> AllCustomColumns
        {
            get
            {
                return (from column in manipulateData.GetColumnList(this.CustomReportType)
                        select new SelectListItem { Value = column.Key, Text = column.Value }).AsEnumerable<SelectListItem>();
            }
        }

        public Dictionary<string, string> AllColumnsKeys
        {
            get
            {
                return manipulateData.GetColumnList("personalDetails")
                    .Concat(manipulateData.GetColumnList("contactDetails"))
                    .Concat(manipulateData.GetColumnList("professionalDetails"))
                    .Concat(manipulateData.GetColumnList("promotionDetails"))
                    .Concat(manipulateData.GetColumnList(this.CustomReportType))
                    .GroupBy(d => d.Key)
                    .ToDictionary(d => d.Key, d => d.First().Value);
            }
        }

        public ReportSelectionViewModel()
        {
            this.CustomReportType = null;
        }

        public ReportSelectionViewModel(string customReportType)
        {
            this.CustomReportType = customReportType;
        }
    }
}
