using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class EmployeeDetail : BaseEntity
    {
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public string OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime DateofJoiningDGH { get; set; }

        public SeatingLocation SeatingLocation { get; set; }

    }

    public enum EmployeeType
    {
        Deputationist,
        Adviser,
        Consultant,
        Contractual,
        TraineeOfficer,
        Others
    }

    public enum SeatingLocation
    {
        GroundFloor,
        IstFloor,
        IIndFloor,
        IIIrdFloor,
        IVFloor,
        Others
    }
}
