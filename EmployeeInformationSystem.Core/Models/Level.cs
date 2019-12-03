using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class Level : BaseEntity
    {
        [StringLength(450)]
        [Index(IsUnique = true)]
        [Required(ErrorMessage = "Valid Level Name is required")]
        public string Name { get; set; }
    }
}
