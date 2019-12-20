﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class DependentDetail: BaseEntity
    {
        public string EmployeeId { get; set; }
        [Required(ErrorMessage = "Valid Employee is required")]
        public virtual EmployeeDetail Employee { get; set; }

        [Required(ErrorMessage ="Valid Dependent Name is required")]
        [Display(Name ="Dependent Name")]
        public string DependentName { get; set; }

        [Display(Name = "Date of Birth")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateofBirth { get; set; }

        public string Relationship { get; set; }
    }
}
