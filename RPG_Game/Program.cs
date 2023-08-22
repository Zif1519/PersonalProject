using System;
using System.IO;
using System.Text;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using RPG_Game;
using DocumentFormat.OpenXml.Drawing;

// 아래는 excel app을 작동시켜서 조작하는 방식, 엑셀의 양식을 더 많이 사용할 수는 있지만 속도나 메모리적으로 불편함이 있다.
// using Microsoft.Office.Interop.Excel;
// using Range = Microsoft.Office.Interop.Excel.Range;

namespace RPG_Game
{
    public enum GAME_STATUS { }
    public enum ITEM_TYPE { Weapon = 0, SubWeapon, Halmet, Armor, Gloves, Boots, Ring, Amulet, Potion, Food, }
    public enum SELECT_TYPE { Place = 0, Status, Inventory, Item, Quest, }
    internal class Program
    {
        static void Main(string[] args)
        {
            Displayer displayer = new Displayer();
            InventoryData inventory;
            Dictionary<int, ISelectable> myData = new Dictionary<int, ISelectable>();

            SetSenario(ref myData);

            //while (true)
            //{
            //}


        }
        static void SetSenario(ref Dictionary<int, ISelectable> data)
        {

            string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "PlaceData.xlsx";
            string filePath = projectFolder + "..\\"+"..\\"+"..\\"+fileName;

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();

                if (worksheetPart == null)
                {
                    return; 
                }

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                foreach (Row row in sheetData.Elements<Row>())
                {
                    //foreach (Cell cell in row.Elements<Cell>())
                    //{
                    //    Console.Write(GetCellValue(cell, workbookPart) + "\t");
                    //}
                    //Console.WriteLine();
                    int num_id; 
                    int.TryParse(row.ElementAt(0).ToString(), out num_id);
                    if (num_id < 2000)
                    {
                        PlaceData inputdata = new PlaceData(row.ElementAt(0).ToString(), row.ElementAt(1).ToString(), row.ElementAt(2).ToString());
                        data.Add(inputdata.PlaceID, inputdata);
                    } else if (num_id < 3000)
                    {
                        //
                    }


                }
                document.Dispose();
            }

            return;
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





    class WorksheetWriter
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




}

public interface ISelectable
{
    string Name { get; }
    string Description { get; }
    public SELECT_TYPE SelectType { get; }
    public List<ISelectable> Selections { get; }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
}

public class Displayer
{
    StringBuilder _title;
    StringBuilder _description;
    StringBuilder _selections;

    public void RefreshDisplay()
    {
        // 콘솔창 지우기
        Console.Clear();

        // 콘솔에 스테이지 Title을 표시하기
        Console.WriteLine("########################################");
        Console.WriteLine($"          {_title}          ");
        Console.WriteLine("########################################\n\n");

        // 스테이지 설명이 있는 경우 출력.
        if (_description != null)
        {
            Console.WriteLine(_description);
        }
        else { Console.WriteLine("\n\t\t  ...\t\t\n"); }

        // 선택지 출력
        Console.WriteLine(_selections);
        Console.WriteLine("\n\n");
    }
    public void UpdatePlaceData(PlaceData placeData)
    {
        // PlaceData가 들어오면 모든 정보 초기화 
        _title.Clear();
        _description.Clear();
        _selections.Clear();

        // 재설정
        _title.Append(placeData.Name);
        _description.Append(placeData.Description);
        if (placeData.Selections.Count <= 0) return;
        for (int i = 0; i < placeData.Selections.Count; i++)
        {
            _selections.Append($"{i + 1} : ");
            switch (placeData.Selections[i].SelectType)
            {
                case SELECT_TYPE.Place:
                    _selections.Append($"[{placeData.Selections[i].Name}](으)로 이동.");
                    break;

                // 퀘스트나 아이템의 경우 추가로 구현

                default:
                    break;
            }
        }
        RefreshDisplay();
    }
    public void UpdateQuestData(QuestData QuestData)
    {
        RefreshDisplay();
    }
}
public class PlaceData : ISelectable
{
    public int PlaceID { get; }
    public string Name { get; }
    public string Description { get; }
    public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Place;
    public List<ISelectable> Selections { get; }
    public PlaceData(string id, string name, string description)
    {
        PlaceID = int.Parse(id);
        Name = name;
        Description = description;
        Selections = new List<ISelectable>();
    }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
}
public class QuestData
{
    public string Name { get; }
    public string Description { get; }
    public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Quest;
    public List<ISelectable> Selections { get; }
    public QuestData(String name, string description)
    {
        Name = name;
        Description = description;
        Selections = new List<ISelectable>();
    }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
}
public class InventoryData : ISelectable
{
    public string Name { get; }
    public string Description { get; }
    public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Inventory;
    public List<ISelectable> Selections { get; }
    public InventoryData(String name, string description)
    {
        Name = name;
        Description = description;
        Selections = new List<ISelectable>();
    }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
}
public class StatusData
{
    public string Name { get; }
    public string Description { get; }
    public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Status;
    public List<ISelectable> Selections { get; }
    public StatusData(String name, string description)
    {
        Name = name;
        Description = description;
        Selections = new List<ISelectable>();
    }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
}
public class ItemData : ISelectable
{
    public string Name { get; }
    public string Description { get; }
    public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Item;
    public List<ISelectable> Selections { get; }
    public ItemData(String name, string description)
    {
        Name = name;
        Description = description;
        Selections = new List<ISelectable>();
    }
    public void AddSelection(ISelectable selection)
    {
        Selections.Add(selection);
    }
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