using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class TelephoneExtension: BaseEntity 
    {
        [StringLength(450)]
        [Index(IsUnique = true)]
        [Required(ErrorMessage = "Valid Number is required")]
        public int Number { get; set; }

        public string EmployeeDetailId { get; set; }
        [Display(Name = "Current Employee")]
        [Required(ErrorMessage = "Valid Employee is required")]
        public virtual EmployeeDetail CurrentEmployee { get; set; }
    }
}
