
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Project
{

    enum Direction // 방향을 저장하는 타입
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
    internal class Player
    {
        
        
        public int X;
        public int Y;
        public Direction MoveDirection;
        public int pushedBoxId;
        public int HP;
        public int MP;



        public Player()
        {
            X = 1;
            Y = 1;
            MoveDirection = Direction.Right;
            pushedBoxId= 0;
            HP = 5;
            MP = 10;
            
        }
        



    }
}
