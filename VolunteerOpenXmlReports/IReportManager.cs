using System.Collections.Generic;

namespace VolunteerOpenXmlReports
{
    public interface IReportManager

    {
        byte[] ProcessData<T>(List<T> listData, string reportName, string sheetTile = null);        
    }
}