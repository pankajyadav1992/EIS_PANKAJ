using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class Organisation: BaseEntity
    {
        [Required(ErrorMessage = "Valid Organisation Name is required")]
        [Display(Name = "Organisation Name")]
        public string Name { get; set; }

        [Display(Name = "Head Office address")]
        public string HeadOffice { get; set; }
    }
}
