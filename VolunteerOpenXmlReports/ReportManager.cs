using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

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
        private List<string> headerList = new();

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

        public byte[] ProcessData<T>(List<T> listData, string reportName, string sheetTitle = null)
        {
            this.reportName = reportName;
            this.sheetTitle = sheetTitle;

            ReadSettingsFromConfiguration();

            this.rowIndexDataStart = ShowHeader ? ++rowIndexDataStart : rowIndexDataStart++;

            var dataTable = Utility.ListToDataTable(listData, templateHeaderCustomColumnNameList);

            if (ShowHeader)
            {
                headerList = Utility.CreateHeaderList(dataTable.Columns, templateHeaderCustomColumnNameList);
            }

            return CreateSpreahsheetWorkbook(headerList, dataTable);
        }

        private void ReadSettingsFromConfiguration()
        {
            var someArray = Configuration.GetSection($"{reportName}CellNames").GetChildren().Select(x => x.Value).ToArray();

            ProcessReportSettings($"{reportName}CellNames", someArray);

            someArray = Configuration.GetSection($"{reportName}CellFormat").GetChildren().Select(x => x.Value).ToArray();

            ProcessReportSettings($"{reportName}CellFormat", someArray);
        }

        private void ProcessReportSettings(string reportGroup, string[] someArray)
        {
            foreach (var item in someArray)
            {
                switch (reportGroup)
                {
                    case "VolunteerReportCellNames":
                        {
                            templateHeaderCustomColumnNameList.Add(item);
                            break;
                        }
                    case "VolunteerReportCellFormat":
                        {
                            templateHeaderCustomColumnFormatList.Add(Convert.ToUInt32(item));
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        private byte[] CreateSpreahsheetWorkbook(List<string> headerList, DataTable dataTable)
        {
            byte[] byteArray;

            using (var memoryStream = new MemoryStream())
            {
                //using (var spreadSheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                //{
                //    WorkbookPart workbookpart = spreadSheetDocument.AddWorkbookPart();
                //    workbookpart.Workbook = new Workbook();

                //    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                //    worksheetPart.Worksheet = new Worksheet(new SheetData());

                //    // create sheet data
                //    var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                //    WriteHeader(sheetData, headerList);

                //    //WriteDataContent();
                
                //}               


                using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                //using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("C:\\test\\mytest.xlsx", SpreadsheetDocumentType.Workbook))
                {
                    // Add a WorkbookPart to the document.
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    // Add Sheets to the Workbook.
                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                    // Append a new worksheet and associate it with the workbook.
                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "baubau"//ViewBag.Title
                    };

                    WriteHeader(sheetData, headerList);

                    WriteData(sheetData, dataTable);

                    sheets.Append(sheet);

                    workbookpart.Workbook.Save();
                }

                byteArray = memoryStream.ToArray();
            }

            return byteArray;
        }

        private void WriteData(SheetData sheetData, DataTable dataTable)
        {
            foreach (DataRow rowItem in dataTable.Rows)
            {
                var cellItems = new List<Cell>();

                foreach (var colItem in rowItem.ItemArray)
                {
                    var cell = new Cell()
                    {
                        CellValue = new CellValue(colItem.ToString()),
                        DataType = CellValues.String,
                        //StyleIndex = boldDefStyle
                    };

                    cellItems.Add(cell);
                }

                sheetData.AppendChild(new Row(cellItems));
            }
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
                    //StyleIndex = boldDefStyle
                };

                cellItems.Add(cell);
            }

            sheetData.AppendChild(new Row(cellItems));
        }
    }
}
