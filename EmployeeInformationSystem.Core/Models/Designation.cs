using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class Designation : BaseEntity
    {
        [StringLength(450)]
        [Index("IX_DesignationAndOrganisation", 1, IsUnique = true)]
        [Required(ErrorMessage = "Valid Designation Name is required")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Valid Organisation Name is required")]
        [Index("IX_DesignationAndOrganisation", 2, IsUnique = true)]
        public string OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
