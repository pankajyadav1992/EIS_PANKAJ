using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PostingDetail
    {
        public string EmployeeDetailId { get; set; }
        public virtual EmployeeDetail Employee{get;set;}

        public string HODId { get; set; }
        public virtual EmployeeDetail HOD { get; set; }

        public string DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
