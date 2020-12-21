using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
            }
        }
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (row[property.Name] != System.DBNull.Value)
                    {
                        if (property.PropertyType == typeof(System.String))
                            property.SetValue(item, Convert.ToString(row[property.Name]), null);
                        else if (property.PropertyType == typeof(System.Int32))
                            property.SetValue(item, Convert.ToInt32(row[property.Name]), null);
                        else if (property.PropertyType == typeof(System.DayOfWeek))
                        {
                            DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                            property.SetValue(item, day, null);
                        }
                        else
                            property.SetValue(item, row[property.Name], null);
                    }
                }
            }
            return item;
        }
    }
  
}
