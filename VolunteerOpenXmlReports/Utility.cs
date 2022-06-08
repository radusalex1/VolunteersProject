using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerOpenXmlReports
{
    internal class Utility
    {
        public static DataTable ListToDataTable<T> (IList<T> lst, List<string> customHeaderList)
        {
            var currentDT = CreateTable<T>(customHeaderList);
            
            Type entType = typeof(T);

            return new DataTable();
        }

        private static object CreateTable<T>(List<string> customHeaderList)
        {
            throw new NotImplementedException();
        }

        internal static List<string> CreateHeaderList(DataColumnCollection columns, object templateHeaderCustomColumnNamelist)
        {
            throw new NotImplementedException();
        }
    }
}
