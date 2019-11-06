using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PromotionDetail : BaseEntity
    {
        public string EmployeeDetailId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        public string DesignationId { get; set; }
        public virtual Designation Designation { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
