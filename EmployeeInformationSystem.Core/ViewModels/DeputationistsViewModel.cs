using EmployeeInformationSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.ViewModels
{
    public class DeputationistsViewModel
    {
        public EmployeeDetail EmployeeDetails { get; set; }
        public IEnumerable<Organisation> Organisations { get; set; }
        public IEnumerable<Discipline> Disciplines { get; set; }
        public IEnumerable<Level> Levels { get; set; }
        public IEnumerable<TelephoneExtension> TelephoneExtensions { get; set; }
    }
}
