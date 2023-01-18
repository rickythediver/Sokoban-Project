using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Project
{
    public class Game
    {
        public int min_x;
        public int max_x;
        public int min_y;
        public int max_y;
        public string boundary;
        public int movecount;
        public int maxmove;
        

        public Game()
        {
            min_x = 1;
            min_y = 1;
            max_x = 40;
            max_y = 20;
            boundary = "#";
            movecount = 0;
            maxmove = 150;
            

        }



        public void InitialSetting()
        {
            // 초기 세팅
            Console.ResetColor(); // 컬러를 초기화 하는 것
            Console.CursorVisible = false; // 커서를 숨기기
            Console.Title = "어코반"; // 타이틀을 설정한다.
            Console.BackgroundColor = ConsoleColor.Black; // 배경색을 설정한다.
            Console.Clear(); // 출력된 내용을 지운다.
            
        }

    }
}
