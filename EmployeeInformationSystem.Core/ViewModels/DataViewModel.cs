using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeInformationSystem.Core.ViewModels
{
    public class DataViewModel
    {
        public EmployeeDetail EmployeeDetails { get; set; }
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart1 { get; set; }

        public List<SelectListItem> EmployeeType { get; set; }
        //public CustomEmployeeType EmployeeType { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart2 { get; set; }

        [StringLength(4, MinimumLength = 4 , ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart3 { get; set; }
        public List<PastExperience> PastExperiences { get; set; }
        public List<PostingDetail> PostingDetails { get; set; }
        public List<PromotionDetail> PromotionDetails { get; set; }
        public List<QualificationDetail> QualificationDetails { get; set; }
        public List<DependentDetail> DependentDetails { get; set; }
        public IEnumerable<Organisation> Organisations { get; set; }
        public IEnumerable<Degree> Degrees { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Designation> Designations { get; set; }
        public IEnumerable<Level> Levels { get; set; }
        public IEnumerable<PayScale> PayScales { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<HoD> HoDs { get; set; }

        //[StringLength(4, MinimumLength = 4, ErrorMessage = "Telephone Extension must be of 4 characters")]
        public TelephoneExtension TelephoneExtensions { get; set; }
    }
}
