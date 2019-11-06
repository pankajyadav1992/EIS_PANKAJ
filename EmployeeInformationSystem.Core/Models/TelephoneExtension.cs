﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class TelephoneExtension: BaseEntity 
    {
        public int Number { get; set; }

        public string EmployeeDetailId { get; set; }
        public virtual EmployeeDetail CurrentEmployee { get; set; }
    }
}