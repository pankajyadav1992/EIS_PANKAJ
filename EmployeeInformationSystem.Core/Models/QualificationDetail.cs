using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class QualificationDetail : BaseEntity
    {
        public string EmployeeId { get; set; }
        [Required]
        public virtual EmployeeDetail Employee { get; set; }
        
        public string DegreeId { get; set; }
        [Required]
        public virtual Degree Degree { get; set; }

        public string Institution { get; set; }

        public string University { get; set; }

         
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? From { get; set; }

        [Display(Name = "Date of Passing")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? To { get; set; }
        
        public string Grade { get; set; }
    }
}
