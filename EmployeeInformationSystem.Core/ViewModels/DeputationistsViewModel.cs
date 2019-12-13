using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.ViewModels
{
    public class DeputationistsViewModel
    {
        public EmployeeDetail EmployeeDetails { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart1 { get; set; }

        public CustomEmployeeType EmployeeType { get; set; }

        [StringLength(6, MinimumLength = 6, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart2 { get; set; }

        [StringLength(6, MinimumLength = 6, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        public string AadhaarPart3 { get; set; }
        public PostingDetail PostingDetails { get; set; }
        public PromotionDetail PromotionDetails { get; set; }
        public IEnumerable<Organisation> Organisations { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Level> Levels { get; set; }
        public IEnumerable<PayScale> PayScales { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "Telephone Extension must be of 4 characters")]
        public TelephoneExtension TelephoneExtensions { get; set; }
    }

    public enum CustomEmployeeType
    {
        Deputationist = 0
    }
}
