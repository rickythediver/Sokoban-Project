using System;
using System.Xml.Linq;

namespace Sokoban
{
    enum Direction // 방향을 저장하는 타입
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    class Sokoban
    {




        static void Main()
        {

            #region 함수 모음집

            // a와 b중에 최댓값을 구하는 함수
            int Max(int a, int b)
            {
                return (a > b) ? a : b; // 이건 삼향연산자 쓴거임
            }

            // a 와 b중 최소값을 구하는 함수
            int Min(int a, int b) => (a > b) ? b : a;


            // 좌표가 주어지고 거기다 그리는 함수
            void rendering(int a, int b, string text)
            {
                Console.SetCursorPosition(a, b);
                Console.WriteLine(text);
            }

            // 끝나면 실행되는 함수
            void finished()
            {
                Console.Clear();
                Console.WriteLine("ㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋ");
            }

            // 플레이어의 이동과 방향을 저장하는 함수


            #endregion

            #region 초기세팅

            Console.ResetColor();                   // 컬러를 초기화 하는 것
            Console.CursorVisible = false;          // 커서를 숨기기
            Console.Title = "어우석의 소코반";    // 타이틀을 설정한다.
            Console.BackgroundColor = ConsoleColor.White;   // 배경색을 설정한다.
            Console.ForegroundColor = ConsoleColor.DarkYellow;      // 글꼴색을 설정한다.
            Console.Clear();                                    // 출력된 내용을 지운다.
            #endregion

            #region 변수 정의

            // 기호 상수 정의
            const int GOAL_COUNT = 3;
            const int BOX_COUNT = GOAL_COUNT;
            const int WALL_COUNT = 2;

            // 플레이어 위치를 저장하기 위한 변수
            int playerX = 1;
            int playerY = 1;

            // 플레이어의 이동 방향을 저장하기 위한 변수
            Direction playerMoveDirection = Direction.None;

            // 플레이어가 무슨 박스를 밀고 있는지 저장하기 위한 변수
            int pushedBoxId = 0; // 1이면 박스1, 2면 박스2

            // 박스 위치를 저장하기 위한 변수
            int[] boxPositionsX = { 5, 7, 4 };
            int[] boxPositionsY = { 5, 3, 4 };

            // 벽 위치를 저장하기 위한 변수
            int[] wallPositionsX = { 7, 8 };
            int[] wallPositionsY = { 7, 5 };

            // 골 위치를 저장하기 위한 변수
            int[] goalPositionsX = { 9, 1, 3 };
            int[] goalPositionsY = { 9, 2, 3 };

            // 박스가 골 위에 있는지를 저장하기 위한 변수
            bool[] isBoxOnGoal = new bool[BOX_COUNT];

            #endregion


            // 게임 루프 구성
            while (true)
            {

                #region 렌더링
                Console.Clear();// 이전 프레임을 지운다.

                // 골을 그린다.
                for (int i = 0; i < GOAL_COUNT; ++i)
                {
                    int goalX = goalPositionsX[i];
                    int goalY = goalPositionsY[i];

                    rendering(goalX, goalY, "G");

                    //Console.SetCursorPosition(goalX, goalY);
                    //Console.Write("G");
                }


                // 플레이어를 그린다.

                rendering(playerX, playerY, "P");

                //Console.SetCursorPosition(playerX, playerY);
                //Console.Write("P");


                // 박스를 그린다.
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    int boxX = boxPositionsX[boxId];
                    int boxY = boxPositionsY[boxId];

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.SetCursorPosition(boxX, boxY);

                    // 그려줘야함.
                    if (isBoxOnGoal[boxId])
                    {

                        Console.ForegroundColor = ConsoleColor.Red;
                        rendering(boxX, boxY, "☆");
                        // Console.Write("☆");
                    }
                    else
                    {
                        rendering(boxX, boxY, "o");
                        // Console.Write("o");
                    }
                }

                // 벽을 그린다.
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    int wallX = wallPositionsX[wallId];
                    int wallY = wallPositionsY[wallId];

                    //Console.SetCursorPosition(wallX, wallY);
                    //Console.Write("W");

                    rendering(wallX, wallY, "W");
                }


                // 바운더리를 그린다.
                for (int i = 0; i < 21; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    //Console.SetCursorPosition(i ,0); // x축 바운더리 그려주기
                    //Console.Write("#");

                    rendering(i, 0, "#"); // x축 바운더리 그려주기

                    //Console.SetCursorPosition(i,11);
                    //Console.Write("#");
                    rendering(i, 11, "#");
                }
                for (int i = 0; i < 12; ++i)
                {
                    //Console.SetCursorPosition(0, i); // y축 바운더리 그려주기
                    //Console.Write("#");
                    rendering(0, i, "#");// y축 바운더리 그려주기

                    //Console.SetCursorPosition(21, i);
                    //Console.Write("#");
                    rendering(21, i, "#");
                }
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                #endregion

                #region 인풋

                ConsoleKey key = Console.ReadKey().Key;

                #endregion

                #region 업데이트


                // 플레이어 이동 처리
                //if (key == ConsoleKey.LeftArrow)
                //{
                //    playerX = Max(1, playerX - 1);
                //    playerMoveDirection = Direction.Left;
                //}

                //if (key == ConsoleKey.RightArrow)
                //{
                //    playerX = Min(playerX + 1, 20);
                //    playerMoveDirection = Direction.Right;
                //}

                //if (key == ConsoleKey.UpArrow)
                //{
                //    playerY = Max(1, playerY - 1);
                //    playerMoveDirection = Direction.Up;
                //}

                //if (key == ConsoleKey.DownArrow)
                //{
                //    playerY = Min(playerY + 1, 10);
                //    playerMoveDirection = Direction.Down;
                //}

                playercordirec(key, ref playerX, ref playerY, ref playerMoveDirection);

                void playercordirec(ConsoleKey a, ref int b, ref int c, ref Direction d)
                {
                    if (a == ConsoleKey.LeftArrow)
                    {
                        b = Max(1, b - 1);
                        d = Direction.Left;
                    }

                    if (a == ConsoleKey.RightArrow)
                    {
                        b = Min(b + 1, 20);
                        d = Direction.Right;
                    }

                    if (a == ConsoleKey.UpArrow)
                    {
                        c = Max(1, c - 1);
                        d = Direction.Up;
                    }

                    if (a == ConsoleKey.DownArrow)
                    {
                        c = Min(c + 1, 10);
                        d = Direction.Down;
                    }
                }




                // 플레이어와 벽의 충돌 처리
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    int wallX = wallPositionsX[wallId];
                    int wallY = wallPositionsY[wallId];

                    if (playerX == wallX && playerY == wallY)
                    {
                        switch (playerMoveDirection)
                        {
                            case Direction.Left:
                                playerX = wallX + 1;
                                break;
                            case Direction.Right:
                                playerX = wallX - 1;
                                break;
                            case Direction.Up:
                                playerY = wallY + 1;
                                break;
                            case Direction.Down:
                                playerY = wallY - 1;
                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {playerMoveDirection}");

                                return;
                        }

                        break;
                    }
                }

                // 박스 이동 처리
                // 플레이어가 박스를 밀었을 때라는 게 무엇을 의미하는가? => 플레이어가 이동했는데 플레이어의 위치와 박스 위치가 겹쳤다.
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    int boxX = boxPositionsX[i];
                    int boxY = boxPositionsY[i];

                    if (playerX == boxX && playerY == boxY)
                    {
                        // 박스를 민다. => 박스의 좌표를 바꾼다.
                        switch (playerMoveDirection)
                        {
                            case Direction.Left:
                                boxX = Math.Max(1, boxX - 1);
                                playerX = boxX + 1;
                                break;
                            case Direction.Right:
                                boxX = Math.Min(boxX + 1, 20);
                                playerX = boxX - 1;
                                break;
                            case Direction.Up:
                                boxY = Math.Max(1, boxY - 1);
                                playerY = boxY + 1;
                                break;
                            case Direction.Down:
                                boxY = Math.Min(boxY + 1, 10);
                                playerY = boxY - 1;
                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {playerMoveDirection}");

                                return;
                        }

                        pushedBoxId = i;
                    }

                    boxPositionsX[i] = boxX;
                    boxPositionsY[i] = boxY;
                }

                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    int boxX = boxPositionsX[boxId];
                    int boxY = boxPositionsY[boxId];

                    // 박스와 벽의 충돌 처리
                    for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                    {
                        int wallX = wallPositionsX[wallId];
                        int wallY = wallPositionsY[wallId];

                        if (boxX == wallX && boxY == wallY)
                        {
                            switch (playerMoveDirection)
                            {
                                case Direction.Left:
                                    boxX = wallX + 1;
                                    playerX = boxX + 1;
                                    break;
                                case Direction.Right:
                                    boxX = wallX - 1;
                                    playerX = boxX - 1;
                                    break;
                                case Direction.Up:
                                    boxY = wallY + 1;
                                    playerY = boxY + 1;
                                    break;
                                case Direction.Down:
                                    boxY = wallY - 1;
                                    playerY = boxY - 1;
                                    break;
                                default:
                                    Console.Clear();
                                    Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {playerMoveDirection}");

                                    return;
                            }

                            boxPositionsX[boxId] = boxX;
                            boxPositionsY[boxId] = boxY;

                            break;
                        }
                    }

                }

                // 박스끼리 충돌 처리
                for (int collidedBoxId = 0; collidedBoxId < BOX_COUNT; ++collidedBoxId)
                {
                    // 같은 박스라면 처리할 필요가 X
                    if (pushedBoxId == collidedBoxId)
                    {
                        continue;
                    }

                    // 두 개의 박스가 부딪혔을 때
                    if (boxPositionsX[pushedBoxId] == boxPositionsX[collidedBoxId] && boxPositionsY[pushedBoxId] == boxPositionsY[collidedBoxId])
                    {
                        switch (playerMoveDirection)
                        {
                            case Direction.Left:
                                boxPositionsX[pushedBoxId] = boxPositionsX[collidedBoxId] + 1;
                                playerX = boxPositionsX[pushedBoxId] + 1;

                                break;
                            case Direction.Right:
                                boxPositionsX[pushedBoxId] = boxPositionsX[collidedBoxId] - 1;
                                playerX = boxPositionsX[pushedBoxId] - 1;

                                break;
                            case Direction.Up:
                                boxPositionsY[pushedBoxId] = boxPositionsY[collidedBoxId] + 1;
                                playerY = boxPositionsY[pushedBoxId] + 1;

                                break;
                            case Direction.Down:
                                boxPositionsY[pushedBoxId] = boxPositionsY[collidedBoxId] - 1;
                                playerY = boxPositionsY[pushedBoxId] - 1;

                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {playerMoveDirection}");

                                return;
                        }

                        break;
                    }
                }

                // 박스와 골의 처리
                int boxOnGoalCount = 0;

                // 골 지점에 박스에 존재하냐?
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId) // 모든 골 지점에 대해서
                {
                    // 현재 박스가 골 위에 올라와 있는지 체크한다.
                    isBoxOnGoal[boxId] = false; // 없을 가능성이 높기 때문에 false로 초기화 한다.

                    for (int goalId = 0; goalId < GOAL_COUNT; ++goalId) // 모든 박스에 대해서
                    {
                        // 박스가 골 지점 위에 있는지 확인한다.
                        if (boxPositionsX[goalId] == goalPositionsX[boxId] && boxPositionsY[goalId] == goalPositionsY[boxId])
                        {
                            ++boxOnGoalCount;

                            isBoxOnGoal[goalId] = true; // 박스가 골 위에 있다는 사실을 저장해둔다.

                            break;
                        }
                    }
                }

                // 모든 골 지점에 박스가 올라와 있다면?
                if (boxOnGoalCount == GOAL_COUNT)
                {
                    //Console.Clear();
                    //Console.WriteLine("ㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊㅋㅊ.");
                    finished();

                    break;
                }



                #endregion
            }
        }


    }
}