using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PayScale: BaseEntity
    {
        [StringLength(450)]
        [Index("IX_ScaleAndOrganisation", 1, IsUnique = true)]
        [Required(ErrorMessage = "Valid Pay Scale is required")]
        public string Scale { get; set; }


        [Index("IX_ScaleAndOrganisation", 2, IsUnique = true)]
        [Required]
        public string OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
