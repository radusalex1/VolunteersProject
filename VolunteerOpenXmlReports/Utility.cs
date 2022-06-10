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
        public static DataTable ListToDataTable<T>(IList<T> lst, List<string> customHeaderList)
        {
            //var currentDT = CreateTable<T>(customHeaderList);
            //Type entType = typeof(T);
            //DataTable table = new DataTable();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            //create header
            int idx = 0;
            foreach (PropertyDescriptor prop in properties)
            {
                if (IsValidColumn(customHeaderList[idx]))
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
                    if (IsValidColumn(customHeaderList[idx]))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }

                    idx++;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        private static bool IsValidColumn(string customColumnName)
        {
            return !string.IsNullOrEmpty(customColumnName);
        }
       

        //private static object CreateTable<T>(List<string> customHeaderList)
        //{
        //    throw new NotImplementedException();
        //}

        ///
        internal static List<string> CreateHeaderList(DataColumnCollection columns, List<string> templateHeaderCustomColumnNamelist)
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
