﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name= "Head of Department(HoD)")]
        public virtual EmployeeDetail HOD { get; set; }

        public string DepartmentId { get; set; }
        [Required(ErrorMessage = "Valid Department is required")]
        public virtual Department Department { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Posting From")]
        public DateTime? From { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Posting Till")]
        public DateTime? To { get; set; }
    }
}
