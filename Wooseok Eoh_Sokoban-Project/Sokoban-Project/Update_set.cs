using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Project
{
    static class Update_set
    {

        // 오브젝트를 그린다
        public static void RenderObject(int x, int y, string obj)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(obj);
        }




    }
}
