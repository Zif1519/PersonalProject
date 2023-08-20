using System;
using System.Text;

namespace RPG_Game
{
    internal class Program
    {
        static void Main(string[] args)
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

        static void LoadStage(Stage stage)
        {
            StringBuilder title = stage.Name;
            StringBuilder description = stage.Description;
            Stage[] selections = stage.Selections;


            // 콘솔창 지우기
            Console.Clear();

            // 콘솔에 스테이지 Title을 표시하기
            Console.WriteLine("########################################");
            Console.WriteLine($"          {title}          ");
            Console.WriteLine("########################################\n\n\n");

            // 스테이지 설명이 있는 경우 출력.
            if (description != null)
            {
                Console.WriteLine(description);
            }

            // 플레이어의 선택지인 Selections에 번호를 붙여서 표시하기
            for (int i = 0; i < selections.Length; i++)
            {
                Console.WriteLine($" {i + 1} : [{selections[i].Name}](으)로 가기");
            }

            Console.WriteLine("\n\n");

            // 플레이어의 행동을 입력받는다. 행동의 번호 외의 숫자가 입력된 경우에는 다시 입력을 받는다.
            bool isCorrectInput = false;

            while (!isCorrectInput)
            {
                try
                {
                    Console.Write($"원하시는 행동의 번호를 입력해 주세요. (1~{selections.Length}) : ");
                    int answer = int.Parse(Console.ReadLine());
                    if (answer < 1 || answer > selections.Length)
                    {
                        LoadStage(stage.Selections[answer - 1]);
                    }
                }
                catch (Exception e) 
                {
                    Console.WriteLine($"입력이 올바르지 않습니다. Exception : {e}");
                }
            }


        }

    }

    public class Stage
    {
        public StringBuilder Name { get; set; }
        public StringBuilder Description { get; set; }
        public Stage[] Selections { get; set; }
        public Stage(StringBuilder name, StringBuilder description, Stage[] selections)
        {
            Name = name;
            Description = description;
            Selections = selections;
        }
    }

    public class GameSenario
    {

    }
}