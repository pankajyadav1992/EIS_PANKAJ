using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class EmployeeLeaveBalance:BaseEntity
    {
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        public string LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }

        [Display(Name = "Total Leave Count")]
        public string TotalLeaveCount { get; set; }

        [Display(Name = "Available")]
        public string AvailableLeaveCount { get; set; }
    }
}
