using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.ViewModels
{
    public class ReportsViewModel
    {
        public List<EmployeeDetail> EmployeeDetails { get; set; }
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
        public IEnumerable<TelephoneExtension> TelephoneExtensions { get; set; }
    }
}
