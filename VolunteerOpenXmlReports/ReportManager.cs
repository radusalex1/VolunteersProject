using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace VolunteerOpenXmlReports
{
    public class ReportManager : IReportManager
    {
        public IConfiguration Configuration { get; private set; }

        private const uint SheetId = 1;
        private const bool ShowHeader = true;

        private readonly List<string> templateHeaderCustomColumnNameList = new();
        private readonly List<UInt32Value> templateHeaderCustomColumnFormatList = new();
        private int rowIndexDataStart = 1;
        private string reportName;
        private string sheetTitle;
        private List<string> headerList = new ();

        private UInt32Value noStyle = UInt32Value.FromUInt32(999);
        private UInt32Value stringDefStyle = UInt32Value.FromUInt32(0);
        private UInt32Value boldDefStyle = UInt32Value.FromUInt32(1);
        private UInt32Value dateTimeDefStyle = UInt32Value.FromUInt32(2);
        private UInt32Value twoDecTouDefStyle = UInt32Value.FromUInt32(3);
        private UInt32Value noDecNoTouStyle = UInt32Value.FromUInt32(4);
        private UInt32Value noDecTouStyle = UInt32Value.FromUInt32(5);
        private UInt32Value fourDecTouStyle = UInt32Value.FromUInt32(6);
        private UInt32Value generalStyle = UInt32Value.FromUInt32(7);

        private string APP_SETTINGS_SECTION;// = "Data";
        private string APP_SETTINGS_KEY;// = "Folders";

        public ReportManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public byte[] ProcessData<T>(List<T> listData, string reportName, string sheetTile = null)
        {
            this.reportName = reportName;
            this.sheetTitle = sheetTitle;

            ReadSettingsFromConfiguration();

            this.rowIndexDataStart = ShowHeader ? ++rowIndexDataStart : rowIndexDataStart++;

            var dataTable = Utility.ListToDataTable(listData, templateHeaderCustomColumnNameList);

            if(ShowHeader)
            {
                headerList = Utility.CreateHeaderList(dataTable.Columns, templateHeaderCustomColumnNameList);
            }

            return CreateSpreahsheetWorkbook(headerList, dataTable);
        }

        private void ReadSettingsFromConfiguration()
        {
            APP_SETTINGS_SECTION = reportName;
            APP_SETTINGS_KEY = "cellNames";

            var section = Configuration.GetSection($"{APP_SETTINGS_SECTION}:{APP_SETTINGS_KEY}");
            //var cellNames = section.Get<string[]>();
        }

        private byte[] CreateSpreahsheetWorkbook(List<string> headerList, object dataTable)
        {
            byte[] byteArray;

            using (var memoryStream = new MemoryStream())
            {
                using (var spreadSheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = spreadSheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // create sheet data
                    var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                    WriteHeader(sheetData, headerList);

                    //WriteDataContent();
                }

                byteArray = memoryStream.ToArray();
            }

            return byteArray;
        }

        private void WriteHeader(SheetData sheetData, List<string> headerList)
        {           
            var cellItems = new List<Cell>();

            foreach (var item in headerList)
            {
                var cell = new Cell()
                {
                    CellValue = new CellValue(item),
                    DataType = CellValues.String,
                    StyleIndex = boldDefStyle
                };

                cellItems.Add(cell);
            }

            sheetData.AppendChild(new Row(cellItems));
        }
    }
}
