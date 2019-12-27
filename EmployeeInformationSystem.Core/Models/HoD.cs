using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class HoD : BaseEntity
    {
        [Required]
        [Display(Name = "Head of Department")]
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        [Required]
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
