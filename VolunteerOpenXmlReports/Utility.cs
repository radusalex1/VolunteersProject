using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerOpenXmlReports
{
    internal class Utility
    {
        /// <summary>
        /// Create datatable from a list of objects depends on the custom header list.
        /// </summary>
        /// <typeparam name="T">Object to be mapped to datatable.</typeparam>
        /// <param name="lst">List of objects.</param>
        /// <param name="customHeaderList">Custom header list that will include or not a column in the target datatable. 
        /// If it will includeded it will set the right name of the header columns  </param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IList<T> lst, List<string> customHeaderList)
        {            
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            //create header
            int idx = 0;
            foreach (PropertyDescriptor prop in properties)
            {
                if (IsColumnVisible(customHeaderList[idx]))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                idx++;
            }

            //create table content
            idx = 0;
            foreach (T item in lst)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (IsColumnVisible(customHeaderList[idx]))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }

                    idx++;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        private static bool IsColumnVisible(string customColumnName)
        {
            return !string.IsNullOrEmpty(customColumnName);
        }
             
        internal static List<string> CreateHeaderList(DataColumnCollection columns)
        {
            //todo - refactor this method
            //return templateHeaderCustomColumnNamelist;

            var columnList = new List<String>();

            foreach (var itemColumn in columns)
            {
                columnList.Add(itemColumn.ToString());
            }

            return columnList;
        }
    }
}
