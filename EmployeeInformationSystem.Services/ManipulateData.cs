using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeInformationSystem.Services
{
    public class ManipulateData
    {
        public string DateDifference(DateTime d1, DateTime d2)
        {
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            DateTime fromDate, toDate;
            int year, month, day, increment = 0;

            //Bigger Date
            if (d1 > d2)
            {
                fromDate = d2;
                toDate = d1;
            }
            else
            {
                fromDate = d1;
                toDate = d2;
            }

            //Days
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
            {
                day = toDate.Day - fromDate.Day;
            }

            //Month
            if ((fromDate.Month + increment) > toDate.Month)
            {
                month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }

            //Year
            year = toDate.Year - (fromDate.Year + increment);

            return year + (1 == year ? " Year, " : " Years, ") + month + (1 == month ? " Month, " : " Months, ") + day + (1 == day ? " Day " : " Days");
        }

        public Dictionary<string, string> GetColumnList(string infoType)
        {
            Dictionary<string, string> columns;
            switch (infoType)
            {
                case "personalDetails":
                    columns = new Dictionary<string, string>()
                    {
                        {"EmployeeCode", "CPF Number/Employee Code" },{"EmployeeType", "Employee Category" }, {"Title", "Title" }, {"GetName", "Name" },{"Vintage", "Vintage" },
                        {"DateOfBirth", "Date of Birth" },{"DateOfSuperannuation", "Date of Superannuation" }, {"MaritalStatus","Marital Status" }, {"MarriageDate","Marriage Date" },
                        {"Gender","Gender" },{"BloodGroup","Blood Group"},{"PassportNumber", "Passport Number" },{"PassportValidity", "Passport Validity" },{"UANNumber", "UAN Number" },
                        {"VehicleNumber", "Vehicle Number"},{"DeputedLocation", "Deputed Location" },{"PANNumber", "PAN Number" },{"AadhaarNumber", "Aadhaar Number" },{"Dependent Details", "Dependent Details"}
                    };

                    break;
                case "contactDetails":
                    columns = new Dictionary<string, string>()
                    {
                        {"EmailID", "EMail ID" },{"AlternateEmailID", "Alternate EMail ID" },{"MobileNumber", "Mobile Number" },{"ResidenceNumber", "Residence Phone Number" },
                        {"ResidenceAddress", "Residence Address" },{"PermanentAddress", "Permanent Address" }, {"EmergencyPerson","Emergency Contact Person" }, {"EmergencyContact","Emergency Phone Number" },
                        {"SeatingLocation","Seating Location" },{"Telephone Extension","Telephone Extension"}
                    };

                    break;
                case "professionalDetails":
                    columns = new Dictionary<string, string>()
                    {
                        {"WorkingStatus", "Working Status" },{"Organisation", "Organisation" },{"Qualification Details", "Qualification Details" },{"PrimaryExpertise", "Primary Expertise" },
                        {"Discipline", "Discipline" },{"DateofJoiningParentOrg","Date of Joining parent organisation" }, {"DateofRelievingLastOffice","Date of Relieving from Last Office" }, {"DateofJoiningDGH","Date of Joining DGH" },
                        {"DeputationPeriod","Deputation/Engagement Period" },{"DateofLeavingDGH","Date of Separation from DGH"}, {"ReasonForLeaving","Reason for Leaving DGH" }
                    };
                    break;
                case "promotionDetails":
                    columns = new Dictionary<string, string>()
                    {
                        {"Designation", "Designation" },{"Department", "Department" },{"Promotion Details", "Promotion Details" },{"Posting Details", "Posting Details" },{"CurrentBasicPay", "Current Basic Pay" },{"DGHLevel", "DGH Level" }
                    };
                    break;

                case "DeputationistVintageReport":
                    columns = new Dictionary<string, string>()
                    {
                        {"EmployeeCode", "CPF Number/Employee Code" },{"GetFullName", "Name" },{"Designation", "Designation" },{"Department", "Department" },{"DateofJoiningDGH","Date of Joining DGH" },{"WorkingStatus", "Working Status" },
                        {"ReasonForLeaving","Reason for Leaving DGH" }, {"DateOfSuperannuation", "Date of Superannuation" }, {"DeputationPeriod","Deputation/Engagement Period" },{"DateofLeavingDGH","Date of Separation from DGH"},{"Vintage", "Vintage" },
                        {"Vintage1Year", "Vintage in 1 Year" }, {"Vintage2Years", "Vintage in 2 Years" },{"Vintage3Years", "Vintage in 3 Years" }, {"Vintage4Years", "Vintage in 4 Years" }, {"Vintage5Years", "Vintage in 5 Years" }

                    };
                    break;
                default:
                    columns = new Dictionary<string, string>();
                    break;
            }
            return columns;
        }

    }
}
