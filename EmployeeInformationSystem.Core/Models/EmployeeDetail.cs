using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Core.Models
{
    public class EmployeeDetail : BaseEntity
    {
        [StringLength(450)]
        [Index(IsUnique = true)]
        [Required(ErrorMessage ="Valid CPF No. is required")]
        [Display(Name = "CPF Number")]
        public string EmployeeCode { get; set; }

        public Nullable<Title> Title { get; set; }
           
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Valid Last Name is required")]
        [Display(Name = "Last  Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Valid Employee Category is required")]
        [Display(Name = "Employee Category")]
        public EmployeeType EmployeeType { get; set; }

        //Nullable
        public string OrganisationId { get; set; }
        [Display(Name = "Parent Organisation")]
        public virtual Organisation Organisation { get; set; }

        //Make required in View Model
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Date of Superannuation")]
        public DateTime DateOfSuperannuation { get; set; }

        [Display(Name = "Date of Joining parent organisation")]
        public DateTime? DateofJoiningParentOrg { get; set; }

        [Display(Name = "Date of Joining parent organisation")]
        public DateTime? DateofRelievingLastOffice { get; set; }

        //Make required in View Model
        [Display(Name = "Date of Joining DGH")]
        public DateTime? DateofJoiningDGH { get; set; }

        [Display(Name = "Date of Leaving DGH")]
        public DateTime? DateofLeavingDGH { get; set; }

        [Display(Name = "Reason for Leaving DGH")]
        public Nullable<ReasonForLeaving> ReasonForLeaving { get; set; }

        [Display(Name = "Deputation Period")]
        public string DeputationPeriod { get; set; }

        [Display(Name = "Seating Location")]
        public Nullable<SeatingLocation> SeatingLocation { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [StringLength(450)]
        [Display(Name = "Residence Address")]
        public string ResidenceAddress { get; set;}

        [Display(Name = "EMail ID")]
        public string EmailID { get; set; }

        [Display(Name = "Blood Group")]
        public Nullable<BloodGroup> BloodGroup { get; set; }

        [Display(Name = "Profile Photo")]
        public string ProfilePhoto { get; set; }

        [Display(Name = "Working Status")]
        public Boolean WorkingStatus { get; set; }

        [Display(Name = "Date of Separation from DGH")]
        public DateTime? DateOfSeperation { get; set; }

        public Gender Gender { get; set; }

        public string Qualification { get; set; }

        //Nullable
        public string DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        [Display(Name = "Primary Expertise")]
        public string PrimaryExpertise { get; set; }

        //Nullable & Add support for Org Level in Promotion details
        public string LevelId { get; set; }
        [Display(Name = "DGH Level")]
        public virtual Level DGHLevel { get; set; }

        [Display(Name = "Current Basic Pay")]
        public string CurrentBasicPay { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN Number must be of 10 characters")]
        [Display(Name = "PAN Number")]
        public string PANNumber { get; set; }

        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        [Display(Name = "Aadhaar Number")]
        public string AadhaarNumer { get; set; }

        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Display(Name = "Passport Validity")]
        public DateTime? PassportValidity { get; set; }

        [Display(Name = "Vehicle Number")]
        public string VehicleNumber { get; set; }

        [Display(Name = "Marital Status")]
        public Nullable<Boolean> MaritalStatus { get; set; }

        [Display(Name = "Marriage Date")]
        public DateTime? MarriageDate { get; set; }

        [Display(Name = "Alternate EMail ID")]
        public string AlternateEmailID { get; set; }

        [Display(Name = "Emergency Contact Person")]
        public string EmergencyPerson { get; set; }

        [Display(Name = "Emergency Contact Details")]
        public string EmergencyContact { get; set; }
    }

    public enum Title
    {
        [Display(Name = "Mr.")]
        Mr,
        [Display(Name = "Mrs.")]
        Mrs,
        [Display(Name = "Dr.")]
        Dr,
        [Display(Name = "Major General")]
        MajGen,
        [Display(Name = "Others")]
        Others
    }

    public enum EmployeeType
    {
        Deputationist,
        Adviser,
        Consultant,
        Contractual,
        [Display(Name = "Trainee Officer")]
        TraineeOfficer,
        Others
    }

    public enum SeatingLocation
    {
        [Display(Name = "Ground Floor")]
        GroundFloor,
        [Display(Name = "I Floor")]
        IstFloor,
        [Display(Name = "I Floor")]
        IIndFloor,
        [Display(Name = "III Floor")]
        IIIrdFloor,
        [Display(Name = "IV Floor")]
        IVFloor,
        [Display(Name = "V Floor")]
        VFloor,
        [Display(Name = "SDC Bhubneshwar")]
        SDC_BBSR,
        Others
    }

    public enum BloodGroup
    {
        [Display(Name = "A+")]
        APositive,
        [Display(Name = "A-")]
        ANegative,
        [Display(Name = "B+")]
        BPositive,
        [Display(Name = "B-")]
        BNegative,
        [Display(Name = "AB+")]
        ABPositive,
        [Display(Name = "AB-")]
        ABNegative,
        [Display(Name = "O+")]
        OPositive,
        [Display(Name = "O-")]
        ONegative
    }

    public enum Gender
    {
        Male,
        Female
    }


    public enum ReasonForLeaving
    {
        [Display(Name = "Contract Expired")]
        ContractExpired,
        Demise,
        Termination,
        Transfer,
        Repatriation,
        Resignation,
        Superannuation,
        [Display(Name = "Consultancy Completion")]
        ConsultancyCompletion,
        Others
    }
}
