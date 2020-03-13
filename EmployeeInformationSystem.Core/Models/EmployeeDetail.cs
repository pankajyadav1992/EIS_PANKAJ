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
        [Required(ErrorMessage = "Valid CPF No./Employee Code is required")]
        [Display(Name = "CPF Number/Employee Code")]
        public string EmployeeCode { get; set; }

        [StringLength(1000)]
        public string Title { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Valid First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(1000)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(1000)]
        [Display(Name = "Last  Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Valid Employee Category is required")]
        [Display(Name = "Employee Category")]
        public EmployeeType EmployeeType { get; set; }

        //Nullable
        [Display(Name = "Parent Organisation")]
        public string OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        //Make REQUIRED in View Model
        [Display(Name = "Date of Birth")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Date of Superannuation")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateOfSuperannuation { get; set; }

        [Display(Name = "Date of Joining parent organisation")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateofJoiningParentOrg { get; set; }

        [Display(Name = "Date of Relieving from Last Office")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateofRelievingLastOffice { get; set; }

        //Make required in View Model
        [Display(Name = "Date of Joining DGH")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateofJoiningDGH { get; set; }

        [Display(Name = "Date of Leaving DGH")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateofLeavingDGH { get; set; }

        [Display(Name = "Reason for Leaving DGH")]
        public Nullable<ReasonForLeaving> ReasonForLeaving { get; set; }

        [StringLength(1000)]
        [Display(Name = "Deputation/Engagement Period")]
        public string DeputationPeriod { get; set; }

        [Display(Name = "Seating Location")]
        public Nullable<SeatingLocation> SeatingLocation { get; set; }

        [StringLength(20)]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Please enter valid mobile number")]
        public string MobileNumber { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter valid contact number")]
        [Display(Name = "Residence Phone Number")]
        public string ResidenceNumber { get; set; }

        [StringLength(1000)]
        [Display(Name = "Residence Address")]
        public string ResidenceAddress { get; set; }

        [StringLength(1000)]
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "EMail ID")]
        public string EmailID { get; set; }

        [Display(Name = "Blood Group")]
        public Nullable<BloodGroup> BloodGroup { get; set; }

        [StringLength(1000)]
        [Display(Name = "Profile Photo")]
        public string ProfilePhoto { get; set; }

        [Display(Name = "Working Status")]
        public Boolean WorkingStatus { get; set; }

        [Display(Name = "Date of Separation from DGH")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DateOfSeparation { get; set; }

        [Required(ErrorMessage = "Valid Gender is required")]
        public Gender Gender { get; set; }

        //public string Qualification { get; set; }
        /* Taken care via seperate Table
         */

        //Nullable
        public string DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }

        [StringLength(2000)]
        [Display(Name = "Primary Expertise")]
        public string PrimaryExpertise { get; set; }

        //Nullable & Add support for Org Level in Promotion details
        public string LevelId { get; set; }
        [Display(Name = "DGH Level")]
        public virtual Level DGHLevel { get; set; }

        [StringLength(1000)]
        [Display(Name = "Current Basic Pay")]
        public string CurrentBasicPay { get; set; }

        // Enable Length control at UI
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PAN Number must be of 10 characters")]
        [Display(Name = "PAN Number")]
        public string PANNumber { get; set; }

        // Enable Length control at UI
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar Number must be of 12 characters")]
        [Display(Name = "Aadhaar Number")]
        public string AadhaarNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Display(Name = "Passport Validity")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? PassportValidity { get; set; }

        [StringLength(1000)]
        [Display(Name = "Vehicle Number")]
        public string VehicleNumber { get; set; }

        [Display(Name = "Marital Status")]
        public Nullable<MaritalStatus> MaritalStatus { get; set; }

        [Display(Name = "Marriage Date")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? MarriageDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Alternate EMail ID")]
        public string AlternateEmailID { get; set; }

        [StringLength(1000)]
        [Display(Name = "Emergency Contact Person")]
        public string EmergencyPerson { get; set; }


        [StringLength(100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter valid contact number")]
        [Display(Name = "Emergency Phone Number")]
        public string EmergencyContact { get; set; }

        /*
         * Additional fields for Contractual Staff
         */

        [StringLength(12, MinimumLength = 12, ErrorMessage = "UAN Number must be of 12 characters")]
        [Display(Name = "UAN Number")]
        public string UANNumber { get; set; }

        [Display(Name = "Deputed Location")]
        public DeputeLocations? DeputedLocation { get; set; }

        [NotMapped]
        public string GetFullName
        {
            get
            {
                string _fullName = (this.Title == null ? "" : this.Title + " ") + (this.FirstName == null ? "" : this.FirstName + " ") + (this.MiddleName == null ? "" : this.MiddleName + " ") + this.LastName ?? "";
                return _fullName;
            }
        }
    }


    public enum EmployeeType
    {
        [Display(Name = "Deputationist")]
        Deputationist,
        [Display(Name = "Advisor")]
        Advisor,
        [Display(Name = "Consultant")]
        Consultant,
        [Display(Name = "Contractual - DGH Staff")]
        ContractualDGHStaff,
        [Display(Name = "Contractual - MoPNG Staff")]
        ContractualMoPNGStaff,
        [Display(Name = "Trainee Officer")]
        TraineeOfficer,
        [Display(Name = "Others")]
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
        [Display(Name = "Trainee Officer")]
        Others
    }

    public enum DeputeLocations
    {
        [Display(Name = "DGH, Noida")]
        DGHNoida,
        [Display(Name = "MoPN&G, New Delhi")]
        MoPNGDelhi,
        [Display(Name = "SDC, Bhubhaneshwar")]
        SDCBhubhaneshwar,
        [Display(Name = "Scope Minar, New Delhi")]
        ScopeMinarDelhi
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

    public enum MaritalStatus
    {
        Married,
        Single,
        Others
    }
}
