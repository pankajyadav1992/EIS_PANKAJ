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

        public List<string> GetColumnList(string infoType)
        {
            List<string> columns = new List<string>();
            switch (infoType)
            {
                case "personalDetails":
                    columns.AddRange(new List<string>() { "CPF Number/Employee Code", "Employee Category", "Name", "Vintage",
                        "Date of Birth", "Date of Superannuation", "Marital Status", "Marriage Date", "Gender","Blood Group",
                    "Passport Number", "Passport Validity", "UAN Number", "Deputed Location", "PAN Number", "Aadhaar Number", "Dependent Details"});
                    break;
                case "contactDetails":
                    columns.AddRange(new List<string>() { "EMail ID", "Alternate EMail ID", "Mobile Number", "Residence Phone Number",
                        "Residence Address", "Permanent Address", "Emergency Contact Person", "Emergency Phone Number",
                        "Seating Location", "Telephone Extension" });
                    break;
                case "professionalDetails":
                    columns.AddRange(new List<string>() { "Working Status", "Organisation", "Qualification Details", "Primary Expertise",
                    "Discipline", "Date of Joining parent organisation", "Date of Relieving from Last Office", "Date of Joining DGH",
                    "Deputation/Engagement Period", "Date of Separation from DGH", "Reason for Leaving DGH"});
                    break;
                case "promotionDetails":
                    columns.AddRange(new List<string>() { "Promotion Details", "Posting Details", "Current Basic Pay", "DGH Level" });
                    break;
            }
            return columns;
        }

    }
}
