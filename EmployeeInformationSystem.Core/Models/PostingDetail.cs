using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PostingDetail: BaseEntity
    {
        public string EmployeeDetailId { get; set; }
        [Required(ErrorMessage = "Valid Employee ID is required")]
        public virtual EmployeeDetail Employee{get;set;}

        // Nullable
        public string HODId { get; set; }
        public virtual EmployeeDetail HOD { get; set; }

        public string DepartmentId { get; set; }
        [Required(ErrorMessage = "Valid Department is required")]
        public virtual Department Department { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
