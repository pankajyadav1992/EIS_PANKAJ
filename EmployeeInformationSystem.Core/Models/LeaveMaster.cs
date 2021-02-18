﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeInformationSystem.Core.Models
{
    public class LeaveMaster:BaseEntity
    {
        public string LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public string OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Display(Name = "Annual Quota")]
        public string AnnualQuota { get; set; }

        [Display(Name = "Valid From")]
        public string ValidFrom { get; set; }

        [Display(Name = "Valid Till")]
        public string ValidTill { get; set; }

      
    }

    
}
