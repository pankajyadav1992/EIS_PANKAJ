using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class PromotionDetail : BaseEntity
    {
        //NOT Nullable
        [Required(ErrorMessage = "Valid Employee is required")]
        public string EmployeeId { get; set; }
        public virtual EmployeeDetail Employee { get; set; }

        //NOT Nullable
        [Required(ErrorMessage = "Valid Designation is required")]
        public string DesignationId { get; set; }
        public virtual Designation Designation { get; set; }

        //Nullable
        public string PayScaleId { get; set; }
        public virtual PayScale PayScale { get; set; }

        //Nullable
        public string LevelId { get; set; }
        public virtual Level Level { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "Effective From")]
        public Nullable<DateTime> From { get; set; }
        
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public Nullable<DateTime> To { get; set; }
    }
}
