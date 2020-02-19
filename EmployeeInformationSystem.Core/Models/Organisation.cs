using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class Organisation: BaseEntity
    {
        [StringLength(450)]
        [Index(IsUnique = true)]
        [Required(ErrorMessage = "Valid Organisation Name is required")]
        [Display(Name = "Organisation Name")]
        public string Name { get; set; }

        [StringLength(1000)]
        [Display(Name = "Head Office address")]
        public string HeadOffice { get; set; }
    }
}
