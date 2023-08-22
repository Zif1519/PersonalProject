using System;
using System.IO;
using System.Text;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using RPG_Game;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;

// 아래는 excel app을 작동시켜서 조작하는 방식, 엑셀의 양식을 더 많이 사용할 수는 있지만 속도나 메모리적으로 불편함이 있다.
// using Microsoft.Office.Interop.Excel;
// using Range = Microsoft.Office.Interop.Excel.Range;

namespace RPG_Game
{
    public enum GAME_STATUS { }
    public enum ITEM_TYPE { Weapon = 0, SubWeapon, Halmet, Armor, Gloves, Boots, Ring, Amulet, Potion, Food, }
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, PlaceData> _placeDataDictionary = new Dictionary<int, PlaceData>();
            // Dictionary<int, ItemData> _itemDataDictionary = new Dictionary<int, ItemData>();

            InitData(ref _placeDataDictionary);

            Displayer displayer = new Displayer(_placeDataDictionary[1001], _placeDataDictionary);
            displayer.RefreshDisplay();

        }
        static void InitData(ref Dictionary<int, PlaceData> placeDatadictionary)
        {
            // PlaceData 불러오기
            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "PlaceData.xlsx";
            string filePath = projectFolder + "..\\" + "..\\" + "..\\" + fileName;

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null) return;
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                bool isFirstRow = true;
                foreach (Row row in sheetData.Elements<Row>())
                {
                    if (!isFirstRow) // 첫 번째 행이 아닌 경우에만 처리
                    {
                        List<string> rawData = new List<string>();
                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            Console.Write(GetCellValue(cell, workbookPart) + "\t");
                            rawData.Add(GetCellValue(cell, workbookPart));
                        }
                        Console.WriteLine();
                        if (rawData.Count > 0)
                        {
                            string stringForSelections = rawData[3].Replace("{", "").Replace("}", "");
                            int[] intForSelections = stringForSelections.Split(',').Select(int.Parse).ToArray();
                            PlaceData newPlaceData = new PlaceData(rawData[0], rawData[1], rawData[2], intForSelections);

                            placeDatadictionary.Add(int.Parse(rawData[0]),newPlaceData);
                        }
                    }
                    isFirstRow = false; // 첫 번째 행 처리 후 더 이상 isFirstRow는 true가 아님
                }
                document.Dispose();
            }
        }

        private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            string value = cell.CellValue?.InnerText;

            // 셀의 값이 공유문자열인 경우에는 숫자로 저장되며, 이는 공유문자열테이블에서 찾아와야함. 
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                SharedStringTablePart sharedStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (sharedStringPart != null)
                {
                    value = sharedStringPart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
            }

            return value;
        }

    }

    public class WorksheetWriter
    {
        private WorksheetPart _worksheetPart;
        private SheetData _sheetData;

        public WorksheetWriter(WorksheetPart worksheetPart)
        {
            _worksheetPart = worksheetPart ?? throw new ArgumentNullException(nameof(worksheetPart));
            _sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
        }

        public void AppendRow(params string[] values)
        {
            Row row = new Row();
            foreach (var value in values)
            {
                Cell cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(value)
                };
                row.AppendChild(cell);
            }
            _sheetData.AppendChild(row);
        }
    }



    public class Displayer
    {
        PlaceData _currentPlaceData;
        Dictionary<int, PlaceData> _placeDataDictionary;
        public Displayer(PlaceData placeData, Dictionary<int, PlaceData> placeDataDicionary)
        {
            _currentPlaceData = placeData;
            _placeDataDictionary = placeDataDicionary;
        }
        public void RefreshDisplay()
        {
            // 콘솔창 지우기
            Console.Clear();

            // 콘솔에 스테이지 Title을 표시하기
            Console.WriteLine("########################################");
            Console.WriteLine($"          {_currentPlaceData.Name}          ");
            Console.WriteLine("########################################\n\n");

            // 스테이지 설명이 있는 경우 출력.
            if (_currentPlaceData.Description != null)
            {
                Console.WriteLine(_currentPlaceData.Description);
            }
            else { Console.WriteLine("\n\t\t  ...\t\t\n"); }

            // 선택지 출력

            for (int i = 0; i < _currentPlaceData.Selections.Length; i++)
            {
                Console.WriteLine($"{i + 1} : [{_placeDataDictionary[_currentPlaceData.Selections[i]].Name}](으)로 가기");
            }
            Console.WriteLine("\n\n");
            int input = IsValidInput(1, _currentPlaceData.Selections.Length);
            ChangePlace(_placeDataDictionary[_currentPlaceData.Selections[input - 1]]);
        }
        public void ChangePlace(PlaceData placeData)
        {
            _currentPlaceData = placeData;
            RefreshDisplay();
        }
        static int IsValidInput(int min, int max)
        {
            while (true)
            {
                Console.Write($"원하시는 행동의 번호를 입력해 주세요. : ");
                string answer = Console.ReadLine();
                bool isParseSuccess = int.TryParse(answer, out int result);
                if (isParseSuccess)
                {
                    if (result >= min && result <= max) return result;
                }
            }
        }
    }
    public class PlaceData
    {
        public int PlaceID { get; }
        public string Name { get; }
        public string Description { get; }
        public int[] Selections { get; }
        public PlaceData(string id, string name, string description, int[] selections)
        {
            PlaceID = int.Parse(id);
            Name = name;
            Description = description;
            Selections = selections;
        }
    }
    public class QuestData
    {
        public int QuestID { get; }
        public string Name { get; }
        public string Description { get; }
        public int[] Selections { get; }
        public QuestData(string id, string name, string description)
        {
            QuestID = int.Parse(id);
            Name = name;
            Description = description;
        }
    }
    public class InventoryData
    {
    }
    public class StatusData
    {

    }
    public class ItemData
    {
    }
    public class Character
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int MaxHp { get; }
        public int CurHp { get; }
        public int Gold { get; }

        public Character(string name, string job, int level, int atk, int def, int maxhp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            MaxHp = maxhp;
            CurHp = maxhp;
            Gold = gold;
        }
    }
}