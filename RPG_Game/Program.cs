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
using System.Security.Cryptography.X509Certificates;

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
            int nextInput;
            bool isGameOver = false;
            Dictionary<int, PlaceData> _placeDataDictionary = new Dictionary<int, PlaceData>();
            Dictionary<int, CharacterData> _characterDataDictionary = new Dictionary<int, CharacterData>();
            // Dictionary<int, ItemData> _itemDataDictionary = new Dictionary<int, ItemData>();

            InitData(ref _placeDataDictionary, ref _characterDataDictionary);
            Character mainCharacter = new Character(_characterDataDictionary[1001]);
            Displayer displayer = new Displayer(_placeDataDictionary[1003], _placeDataDictionary);
            nextInput = displayer.RefreshDisplay();

            while (!isGameOver)
            {
                switch (nextInput)
                {
                    // 화면의 구성을 변화시켜야 하는 맵의 경우에 case에 추가시키고, 아래의 RefreshDisplay에 int값을 넘김.
                    case 10001:
                        displayer.ChangePlace(_placeDataDictionary[nextInput]);
                        displayer.RefreshDisplay(1);
                        mainCharacter.ShowStatus();
                        Console.WriteLine("\n\n");
                        nextInput = displayer.RefreshDisplay(99);
                        break;
                    default:
                        displayer.ChangePlace(_placeDataDictionary[nextInput]);
                        nextInput = displayer.RefreshDisplay();
                        break;
                }
            }

        }
        static void InitData(ref Dictionary<int, PlaceData> placeDataDictionary, ref Dictionary<int, CharacterData> characterDataDictionary)
        {
            // PlaceData 불러오기
            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string placeDataFileName = "PlaceData.xlsx";
            string placeDataFilePath = projectFolder + "..\\" + "..\\" + "..\\" + placeDataFileName;

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(placeDataFilePath, false))
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

                            placeDataDictionary.Add(int.Parse(rawData[0]), newPlaceData);
                        }
                    }
                    isFirstRow = false; // 첫 번째 행 처리 후 더 이상 isFirstRow는 true가 아님
                }
                document.Dispose();
            }

            // CharacterData 불러오기
            string characterDataFileName = "CharacterData.xlsx";
            string characterDataFilePath = projectFolder + "..\\" + "..\\" + "..\\" + characterDataFileName;
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(characterDataFilePath, false))
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
                            CharacterData newCharacterData = new CharacterData(rawData[0], rawData[1], rawData[2], rawData[3],
                                rawData[4], rawData[5], rawData[6], rawData[7]);
                            characterDataDictionary.Add(int.Parse(rawData[0]), newCharacterData);
                        }
                    }
                    isFirstRow = false; // 첫 번째 행 처리 후 더 이상 isFirstRow는 true가 아님
                }
                document.Dispose();
            }

        }
        static string GetCellValue(Cell cell, WorkbookPart workbookPart)
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
        // 일반적인 맵은 아래의 리프레시 디스플레이로 화면을 만들어서 보내줌.
        public int RefreshDisplay()
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
                Console.WriteLine(_currentPlaceData.Description + "\n\n");
            }
            else { Console.WriteLine("\n\t\t  ...\t\t\n"); }

            // 선택지 출력

            for (int i = 0; i < _currentPlaceData.Selections.Length; i++)
            {
                Console.WriteLine($"{i + 1} : [{_placeDataDictionary[_currentPlaceData.Selections[i]].Name}](으)로 가기");

                // 종류가 다른 선택지 사이에는 1줄 공간을 띄워서 보기 쉽게 설정
                if ((i + 1) < _currentPlaceData.Selections.Length)
                {
                    if (_currentPlaceData.Selections[i] - _currentPlaceData.Selections[i + 1] > 5000)
                    { Console.WriteLine(); }
                }
            }
            Console.WriteLine("\n\n");
            int input = IsValidInput(1, _currentPlaceData.Selections.Length);
            int result = _currentPlaceData.Selections[input - 1];

            return result;
        }
        // 인벤토리보기, 능력치보기와 같이 디스플레이가 변형되야 하는 경우는 아래로 표시됨.
        public int RefreshDisplay(int displayModifyNum)
        {
            int result = 0;
            switch (displayModifyNum)
            {
                // 특수한 스테이지 표시

                // case 1: 능력치보기창
                case 1:
                    // 콘솔창 지우기
                    Console.Clear();
                    // 콘솔에 스테이지 Title을 표시하기
                    Console.WriteLine("########################################");
                    Console.WriteLine($"          {_currentPlaceData.Name}          ");
                    Console.WriteLine("########################################\n\n");
                    return result;
                case 99:
                    // 선택지 출력
                    for (int i = 0; i < _currentPlaceData.Selections.Length; i++)
                    {
                        Console.WriteLine($"{i + 1} : [{_placeDataDictionary[_currentPlaceData.Selections[i]].Name}](으)로 가기");

                        // 종류가 다른 선택지 사이에는 1줄 공간을 띄워서 보기 쉽게 설정
                        if ((i + 1) < _currentPlaceData.Selections.Length)
                        {
                            if (_currentPlaceData.Selections[i] - _currentPlaceData.Selections[i + 1] > 5000)
                            { Console.WriteLine(); }
                        }
                    }
                    Console.WriteLine("\n\n");
                    int input = IsValidInput(1, _currentPlaceData.Selections.Length);
                    result = _currentPlaceData.Selections[input - 1];
                    return result;
                default:
                    return result;
            }
        }
        public void ChangePlace(PlaceData placeData)
        {
            _currentPlaceData = placeData;
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

    public class ItemData
    {
        int ItemID { get; }
        string Name { get; }
        string Description { get; }


    }
    public struct Item
    {

    }
    public struct Status
    {

    }
    public class CharacterData
    {
        public int CharacterID { get; }
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int MaxHp { get; }
        public int CurHp { get; }
        public int Gold { get; }
        public CharacterData(string id, string name, string job, string level, string atk, string def, string maxhp, string gold)
        {
            CharacterID = int.Parse(id);
            Name = name;
            Job = job;
            Level = int.Parse(level);
            Atk = int.Parse(atk);
            Def = int.Parse(def);
            MaxHp = int.Parse(maxhp);
            CurHp = int.Parse(maxhp);
            Gold = int.Parse(gold);
        }
    }
    public class Character
    {
        public string Name => _characterData.Name;
        public int CharacterID => _characterData.CharacterID;

        CharacterData _characterData;
        // InventoryData _inventoryData;

        public Character(CharacterData characterData)
        {
            _characterData = characterData;
            // _inventoryData = inventoryData;
        }
        public void ShowStatus()
        {
            Console.WriteLine($"Name : {_characterData.Name}");
            Console.WriteLine($"Job : {_characterData.Job}");
            Console.WriteLine($"Level : {_characterData.Level}");
            Console.WriteLine($"Atk : {_characterData.Atk}");
            Console.WriteLine($"Def : {_characterData.Def}");
            Console.WriteLine($"HP : {_characterData.CurHp} / {_characterData.MaxHp}");
            Console.WriteLine($"Gold : {_characterData.Gold}");
        }
    }
}