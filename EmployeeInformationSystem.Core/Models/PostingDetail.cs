using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PostingDetail: BaseEntity
    {
        [Required(ErrorMessage = "Valid Employee ID is required")]
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee{get;set;}

        // Nullable. Consider Linking to HoD Table??
        [Display(Name = "Head of Department(HoD)")]
        public string HODId { get; set; }
        public virtual EmployeeDetail HOD { get; set; }

        // This is ONLY for contractual employees reporting to MoPNG & DGH
        public string Reporting { get; set; }

        [Required(ErrorMessage = "Valid Department is required")]
        public string DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Posting From")]
        public DateTime? From { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Posting Till")]
        public DateTime? To { get; set; }
    }
}
