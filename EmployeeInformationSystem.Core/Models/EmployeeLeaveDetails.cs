using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class EmployeeLeaveDetails : BaseEntity
    {
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        public string LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        
        [Display(Name = "No of Day")]
        public string NoOfDays { get; set; }

        [Display(Name = "Leave From")]
        public string LeaveFrom { get; set; }

        [Display(Name = "Leave Till")]
        public string LeaveTill { get; set; }

        [Display(Name = "Prefix From")]
        public string PrefixFrom { get; set; }

        [Display(Name = "Prefix Till")]
        public string PrefixTill { get; set; }

        [Display(Name = "Suffix From")]
        public string SuffixFrom { get; set; }

        [Display(Name = "Suffix Till")]
        public string SuffixTill { get; set; }
        public string Purpose { get; set; }

        public Boolean StationLeave { get; set; } 

    }
}
