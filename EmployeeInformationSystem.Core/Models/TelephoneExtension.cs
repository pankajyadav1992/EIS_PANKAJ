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
        [Index(IsUnique = true)]
        [Required(ErrorMessage = "Valid Number is required")]
        [Display(Name = "Telephone Extension")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Number { get; set; }

        //[Required(ErrorMessage = "Valid Employee is required")]
        [Display(Name = "Current Employee")]
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        /* Either/Or relationship with Owner or Employee.
         * In case the telephone number is alloted to a Employee, fill in via Employee foreign key
         * Else just fill in text name as in case of "Pantry"
         * Handle this relationship in code manipulation
        */
        [StringLength(1000)]
        public string CurrentOwner { get; set; }
    }
}
