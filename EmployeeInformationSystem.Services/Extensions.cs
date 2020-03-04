using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInformationSystem.Services
{
    public static class Extensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            if (null == enumValue)
            {
                return String.Empty;
            }
            else
            {
                DisplayAttribute displayAttribute = enumValue.GetType()
                                                             .GetMember(enumValue.ToString())
                                                             .First()
                                                             .GetCustomAttribute<DisplayAttribute>();

                string displayName = displayAttribute?.GetName();

                return displayName ?? enumValue.ToString();

                //return enumValue.GetType()?
                //            .GetMember(enumValue.ToString())?
                //            .First()?
                //            .GetCustomAttribute<DisplayAttribute>()?
                //            .Name;
            }
        }
    }
}
