using System;
using System.Text;

namespace RPG_Game
{
    public enum GAME_STATUS { }
    public enum ITEM_TYPE { Weapon = 0, SubWeapon, Halmet, Armor, Gloves, Boots, Ring, Amulet, Potion, Food, }
    public enum SELECT_TYPE { Place = 0, Status, Inventory, Item, Quest,}
    internal class Program
    {
        static void Main(string[] args)
        {
            Displayer displayer = new Displayer();
            Inventory inventory = new Inventory();
            Dictionary<String, ISelectable> myData = new Dictionary<string, ISelectable>();
            
            SetSenario(ref myData);
            if (true)
            {
                //1.게임 시작 화면
                //    -게임 시작시 간단한 소개 말과 마을에서 할 수 있는 행동을 알려줍니다.
                //    -원하는 행동의 숫자를 타이핑하면 실행합니다. 
                //    1 ~2 이외 입력시 -**잘못된 입력입니다** 출력

                //    ```csharp
                //    스파르타 마을에 오신 여러분 환영합니다.
                //    이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.


                //    1.상태 보기
                //    2.인벤토리


                //    원하시는 행동을 입력해주세요.
                //    >>
                //    ```

                //-2.상태보기
                //    - 캐릭터의 정보를 표시합니다.
                //    -7개의 속성을 가지고 있습니다.
                //    레벨 / 이름 / 직업 / 공격력 / 방어력 / 체력 / Gold
                //    - 처음 기본값은 이름을 제외하고는 아래와 동일하게 만들어주세요
                //    -이후 장착한 아이템에 따라 수치가 변경 될 수 있습니다.

                //    ```csharp
                //    ** 상태 보기**
                //    캐릭터의 정보가 표시됩니다.


                //    Lv. 01
                //    Chad(전사)
                //    공격력: 10
                //    방어력: 5
                //    체 력 : 100
                //    Gold: 1500 G


                //    0.나가기


                //    원하시는 행동을 입력해주세요.
                //    >>
                //    ```

                //-3.인벤토리
                //    - 보유 중인 아이템을 전부 보여줍니다.
                //    이때 장착중인 아이템 앞에는[E] 표시를 붙여 줍니다.
                //    - 처음 시작시에는 2가지 아이템이 있습니다.

                //    ```csharp
                //    ** 인벤토리**
                //    보유 중인 아이템을 관리할 수 있습니다.

                //    [아이템 목록]
                //    - [E]무쇠갑옷 | 방어력 + 5 | 무쇠로 만들어져 튼튼한 갑옷입니다.
                //    - 낡은 검 | 공격력 + 2 | 쉽게 볼 수 있는 낡은 검 입니다.

                //    1.장착 관리
                //    0.나가기

                //    원하시는 행동을 입력해주세요.
                //    >>
                //    ```

                //    3 - 1.장착 관리

                //    - 장착관리가 시작되면 아이템 목록 앞에 숫자가 표시됩니다.
                //    -일치하는 아이템을 선택했다면(예제에서 1~2선택시)
                //        -장착중이지 않다면 → 장착
                //        [E] 표시 추가
                //        -이미 장착중이라면 → 장착 해제
                //        [E] 표시 없애기
                //    -일치하는 아이템을 선택했지 않았다면(예제에서 1~3이외 선택시)
                //        - **잘못된 입력입니다** 출력
                //    -아이템의 중복 장착을 허용합니다.
                //        - 창과 검을 동시에 장착가능
                //        - 갑옷도 동시에 착용가능
                //        -장착 갯수 제한 X

                //    ```csharp
                //    ** 인벤토리 -장착 관리**
                //    보유 중인 아이템을 관리할 수 있습니다.

                //    [아이템 목록]
                //    - 1[E]무쇠갑옷 | 방어력 + 5 | 무쇠로 만들어져 튼튼한 갑옷입니다.
                //    - 2 낡은 검         | 공격력 + 2 | 쉽게 볼 수 있는 낡은 검입니다.

                //    0.나가기

                //    원하시는 행동을 입력해주세요.
                //    >>
                //    ```

                //    -아이템이 장착되었다면 1.상태보기 에 정보가 반영되어야 합니다.
                //        -정보 반영 예제
            }

            while (true)
            {

            }

            
        }
        static void SetSenario(ref Dictionary<String, ISelectable> data)
        {
            data.Add() 
        }

        static int GetValidInput(int min, int max)
        {
            while (true)
            {
                Console.Write($"원하시는 행동의 번호를 입력해 주세요. : ");
                string answer = Console.ReadLine();
                bool isParseSuccess = int.TryParse(answer, out int result)
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
            if (stageData.Description != null && stageData.Description != "")
            {
                Console.WriteLine(description);
            }
            else { Console.WriteLine("\n\t\t  ...\t\t\n"); }

            // 선택지 출력
            Console.WriteLine(_Selection);
            Console.WriteLine("\n\n");
        }
        public void UpdatePlaceData(PlaceData placeData)
        {
            // PlaceData가 들어오면 모든 정보 초기화 
            _title.Clear();
            _description.Clear();
            _selections.Clear();
            _selectCount = 0;

            // 재설정
            _title.Append(placeData.Name);
            _description.Append(placeData.Description);
            if{ (placeData.Selections.Count <= 0) return; }
            else { _selectCount = placeData.Selections.Count; }
            for(int i =0; i< placeData.Selections.Count; i++)
            {
                _selections.Append($"{i + 1} : ");
                switch (placeData.Selections[i].SelectType)
                {
                    case : SELECT_TYPE.Place
                        _selections.Append($"[{placeData.Selections[i].Name}](으)로 이동.");
                        break;
                        
                        // 퀘스트나 아이템의 경우 추가로 구현

                    default:
                        break;
                }
            }

            // 플레이어의 선택지인 Selections에 번호를 붙여서 표시하기
            for (int i = 0; i < selections.Length; i++)
            {
                Console.WriteLine($" {i + 1} : [{selections[i].Name}](으)로 가기");
            }
            Console.WriteLine("\n\n");

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
        public Inventory(String name, string description)
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
        public Inventory(String name, string description)
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
        public Inventory(String name, string description)
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
        public Inventory(String name, string description)
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