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
        [StringLength(450)]
        [Index("IX_DesignationAndDepartment", 1, IsUnique = true)]
        [Required]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Index("IX_DesignationAndDepartment", 2, IsUnique = true)]
        [Required]
        public string DepartmentId { get; set; }
        public virtual Department Department { get; set; }

    }
}
