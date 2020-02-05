using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PastExperience : BaseEntity
    {
        [Required]
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        [Required]
        public string Organisation { get; set; }

        [Required]
        public string Position { get; set; }

        public string Location { get; set; }
        
        public string Compensation { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? From { get; set; }

        [Display(Name = "Date of Passing")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? To { get; set; }
    }
}
