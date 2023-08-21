using System;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RPG_Game
{
    public enum GAME_STATUS { }
    public enum ITEM_TYPE { Weapon= 0, SubWeapon, Halmet, Armor, Gloves, Boots, Ring, Amulet, Potion, Food, }
    public enum SELECT_TYPE { Place= 0, Status, Inventory, Item, Quest,}
    internal class Program
    {
        static void Main(string[] args)
        {
            Displayer displayer = new Displayer();
            InventoryData inventory;
            Dictionary<String, ISelectable> myData = new Dictionary<string, ISelectable>();
            
            SetSenario(ref myData);

            //while (true)
            //{
            //}

            
        }
        static void SetSenario(ref Dictionary<String, ISelectable> data)
        {
            string filePath = "C:\\TeamSparta\\PersonalProject\\RPG_Game\\StageData.xlsx";
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb;
            Worksheet ws;

            wb = excel.Workbooks.Open(filePath);
            ws = wb.Worksheets[1];

            Console.WriteLine(Convert.ToString(ws.Cells[1,2].Value));

            Range cell = ws.Cells[1,2];
            Range cell2 = ws.Cells[1,3];
            string value = cell.Value;

            cell2.Value = "Wow!";
            // data.Add();

            // 작업후 리소스 닫기
            wb.SaveAs(filePath,false);
            wb.Close();
            excel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
        }

        static int GetValidInput(int min, int max)
        {
            while (true)
            {
                Console.Write($"원하시는 행동의 번호를 입력해 주세요. : ");
                string answer = Console.ReadLine();
                bool isParseSuccess = int.TryParse(answer, out int result);
                if (isParseSuccess)
                {
                    if(result >= min && result <= max) return result;
                }
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
            for(int i =0; i< placeData.Selections.Count; i++)
            {
                _selections.Append($"{i + 1} : ");
                switch (placeData.Selections[i].SelectType)
                {
                    case SELECT_TYPE.Place :
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
        public string Name { get; }
        public string Description { get; }
        public SELECT_TYPE SelectType { get; } = SELECT_TYPE.Place;
        public List<ISelectable> Selections { get; }
        public PlaceData(string name, string description)
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
}