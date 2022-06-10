using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
            //APP_SETTINGS_SECTION = reportName;
            //APP_SETTINGS_KEY = "cellNames";

            //var section = Configuration.GetSection($"{APP_SETTINGS_SECTION}:{APP_SETTINGS_KEY}");
            //var cellNames = section.Get<string[]>();

            var someArray = Configuration.GetSection($"{reportName}CellNames").GetChildren().Select(x => x.Value).ToArray();

            ProcessReportSettings($"{reportName}CellNames", someArray);

            someArray = Configuration.GetSection($"{reportName}CellFormat").GetChildren().Select(x => x.Value).ToArray();

            ProcessReportSettings($"{reportName}CellFormat", someArray);
        }

        private void ProcessReportSettings(string reportGroup, string[] someArray)
        {
            foreach (var item in someArray)
            {
                //if (!string.IsNullOrEmpty(item))
                //{
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
                //}
            }
        }

        private byte[] CreateSpreahsheetWorkbook(List<string> headerList, object dataTable)
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

                //    worksheetPart.Worksheet.Save();

                //    // create the sheet properties
                //    var sheetsCount = spreadSheetDocument.WorkbookPart.Workbook.Sheets.Count() + 100;

                //    spreadSheetDocument.WorkbookPart.Workbook.Sheets.AppendChild(new Sheet()
                //    {
                //        Id = spreadSheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                //        SheetId = (uint)spreadSheetDocument.WorkbookPart.Workbook.Sheets.Count() + 1,
                //        Name = "MyFirstSheet"
                //    });

                //    // save the workbook
                //    spreadSheetDocument.WorkbookPart.Workbook.Save();
                //}

                //works
                //using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create("c:\\Test\\Test1.xlsx", SpreadsheetDocumentType.Workbook))
                //{
                //    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                //    workbookpart.Workbook = new Workbook();

                //    // Add a WorksheetPart to the WorkbookPart.
                //    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                //    worksheetPart.Worksheet = new Worksheet(new SheetData());

                //    // Add Sheets to the Workbook.
                //    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                //    // Append a new worksheet and associate it with the workbook.
                //    Sheet sheet = new Sheet()
                //    {
                //        Id = spreadsheetDocument.WorkbookPart.
                //        GetIdOfPart(worksheetPart),
                //        SheetId = 1,
                //        Name = "mySheet"
                //    };
                //    sheets.Append(sheet);

                //    workbookpart.Workbook.Save();
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

                    //Row row = new Row() { RowIndex = 1 };
                    //Cell header1 = new Cell() { CellReference = "A1", CellValue = new CellValue("Interval Period Timestamp"), DataType = CellValues.String };
                    //row.Append(header1);
                    //Cell header2 = new Cell() { CellReference = "B1", CellValue = new CellValue("Settlement Interval"), DataType = CellValues.String };
                    //row.Append(header2);
                    //Cell header3 = new Cell() { CellReference = "C1", CellValue = new CellValue("Aggregated Consumption Factor"), DataType = CellValues.String };
                    //row.Append(header3);
                    //Cell header4 = new Cell() { CellReference = "D1", CellValue = new CellValue("Loss Adjusted Aggregated Consumption"), DataType = CellValues.String };
                    //row.Append(header4);

                    //sheetData.Append(row);

                    WriteHeader(sheetData, headerList);

                    sheets.Append(sheet);

                    workbookpart.Workbook.Save();

                    // Close the document.
                    spreadsheetDocument.Close();
                    //return View();

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
                    //StyleIndex = boldDefStyle
                };

                cellItems.Add(cell);
            }

            sheetData.AppendChild(new Row(cellItems));
        }
    }
}
