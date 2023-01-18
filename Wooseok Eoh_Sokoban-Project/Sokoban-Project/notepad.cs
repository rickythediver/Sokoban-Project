using Sokoban_Project;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Sokoban
{



    class Eohkoban
    {



        static void Main()
        {

            Game game = new Game(); // game 클라스 인스턴스화
            game.InitialSetting();

            // 기호 상수 정의
            const int GOAL_COUNT = 3;
            const int BOX_COUNT = GOAL_COUNT;
            const int WALL_COUNT = 20;
            int destroylimit = 10;

            // Trap 클라스 인스턴스화
            Trap[] trap = new Trap[]
            {
                new Trap{ X = 3, Y = 5 },
                new Trap{ X = 3, Y = 8 },
                new Trap{ X = 13, Y = 11 },
                new Trap{ X = 19, Y = 13 },
                new Trap{ X = 23, Y = 5 }
            };





            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // 시간 측정을 시작함




            Player player = new Player(); // player 클라스 인스턴스화

            Debuffs reverse = new Debuffs(); // debuff 클라스 인스턴스화


            Box[] boxes = new Box[3] // box 클라스 인스턴스화
            {
                new Box{ X = 5, Y = 7, IsOnGoal = false },
                new Box{ X = 13, Y = 10, IsOnGoal = false },
                new Box{ X = 21, Y = 13, IsOnGoal = false }
            };


            Wall[] walls = new Wall[WALL_COUNT]; // wall 클라스 인스턴스화
            for (int i = 0; i < 5; ++i)
            {
                walls[i] = new Wall();
                walls[i].X = 6;
                walls[i].Y = i + 1;

            }
            for (int i = 5; i < 10; ++i)
            {
                walls[i] = new Wall();
                walls[i].X = 11;
                walls[i].Y = game.max_y - i;

            }
            for (int i = 10; i < 15; ++i)
            {
                walls[i] = new Wall();
                walls[i].X = 16;
                walls[i].Y = i + 1;

            }
            for (int i = 15; i < WALL_COUNT; ++i)
            {
                walls[i] = new Wall();
                walls[i].X = 27;
                walls[i].Y = game.max_y - (i - 10);

            }



            Goal[] goals = new Goal[] // goal 클라스 인스턴스화
            {
                new Goal{ X = 9, Y = 9 },
                new Goal{ X = 18, Y = 14 },
                new Goal{ X = 23, Y = 3 },
            };


            Action<int, int, string> action = RenderObject;

            #region GameLoop
            // 게임 루프 구성
            while (true)
            {

                Render();

                ConsoleKey key = Console.ReadKey().Key;

                Update(key);

                // 박스와 골의 처리
                int boxOnGoalCount = 0;

                // 골 지점에 박스에 존재하냐?
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId) // 모든 골 지점에 대해서
                {
                    // 현재 박스가 골 위에 올라와 있는지 체크한다.
                    boxes[boxId].IsOnGoal = false; // 없을 가능성이 높기 때문에 false로 초기화 한다.

                    for (int goalId = 0; goalId < GOAL_COUNT; ++goalId) // 모든 박스에 대해서
                    {
                        // 박스가 골 지점 위에 있는지 확인한다.
                        if (IsCollided(boxes[boxId].X, boxes[boxId].Y, goals[goalId].X, goals[goalId].Y))
                        {
                            ++boxOnGoalCount;

                            boxes[boxId].IsOnGoal = true; // 박스가 골 위에 있다는 사실을 저장해둔다.

                            break;
                        }
                    }
                }



                // 모든 골 지점에 박스가 올라와 있다면?
                if (boxOnGoalCount == GOAL_COUNT)
                {
                    Console.Clear();
                    Console.WriteLine("축하합니다. 클리어 하셨습니다.");

                    break;
                }
                else if (game.movecount == 150 || player.HP < 1)
                {
                    Console.Clear();
                    Console.WriteLine("응 실패~~~~");

                    break;
                }


            }
            #endregion


            // 구현한 것
            // 1. 리버스 아이템
            // 2. 대쉬(3칸) - 벽 통과함
            // 3. 벽 부수기 

            // 구현할 것
            // 1. 블리츠크랭크?
            // 2. 이스터 에그



            #region 함수 세트

            // 프레임을 그립니다.
            void Render()
            {
                // 이전 프레임을 지운다.
                Console.Clear();

                // 플레이어 HP 그리기
                for (int i = 0; i < player.HP; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    action(game.max_x + 5, game.min_y + 3, "player HP: ");
                    //RenderObject(game.max_x+5, game.min_y+3, "player HP: ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    RenderObject(game.max_x + 16 + (i * 2), game.min_y + 3, "■");
                }

                // 플레이어 MP 그리기
                for (int i = 0; i < player.MP; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    RenderObject(game.max_x + 5, game.min_y + 4, "player MP: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    RenderObject(game.max_x + 16 + (i * 2), game.min_y + 4, "■");
                }

                // 게임 옆에 이동횟수 텍스트 그리기
                Console.ForegroundColor = ConsoleColor.White;
                RenderObject(game.max_x + 5, game.min_y + 5, $"현재 이동횟수 : {game.movecount}");

                // 게임 옆에 reverse 유효시간 텍스트 그리기
                if (0 < reverse.lasttime)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    RenderObject(game.max_x + 5, game.min_y + 7, $"Reverse 유효시간: {reverse.lasttime}");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                // 게임 옆에 destory 잔여횟수 텍스트 그리기
                if (0 < destroylimit)
                {
                    RenderObject(game.max_x + 5, game.min_y + 6, $"destroy 잔여횟수: {destroylimit}");
                }


                // 게임 밑에 사용키 설명 텍스트 그리기
                RenderObject(game.min_x + 3, game.max_y + 3, "Space = 대쉬(3칸)");
                RenderObject(game.min_x + 3, game.max_y + 4, "D = 벽 부수기(벽만 가능)");

                // 디버프 그리기
                Console.ForegroundColor = ConsoleColor.Cyan;
                RenderObject(reverse.X, reverse.Y, "◐");

                // trap 그리기
                for (int i = 0; i < trap.Length; ++i)
                {

                    RenderObject(trap[i].X, trap[i].Y, "?");
                }

                // 골을 그린다.
                for (int i = 0; i < GOAL_COUNT; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    RenderObject(goals[i].X, goals[i].Y, "G");
                }


                // 플레이어를 그린다.
                Console.ForegroundColor = ConsoleColor.White;
                RenderObject(player.X, player.Y, "P");


                // 박스를 그린다.
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string boxShape = boxes[boxId].IsOnGoal ? "★" : "O";
                    RenderObject(boxes[boxId].X, boxes[boxId].Y, boxShape);
                }


                // 벽을 그린다.
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    RenderObject(walls[wallId].X, walls[wallId].Y, "W");

                }

                // 바운더리 그리기
                for (int i = 0; i < game.max_x + 2; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    RenderObject(i, game.min_y - 1, game.boundary);
                    RenderObject(i, game.max_y + 1, game.boundary);
                }
                for (int i = 0; i < game.max_y + 1; ++i)
                {
                    RenderObject(game.min_x - 1, i, game.boundary);
                    RenderObject(game.max_x + 1, i, game.boundary);
                }


            }



            // 오브젝트를 그린다
            void RenderObject(int x, int y, string obj)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(obj);
            }



            void Update(ConsoleKey key)
            {

                MovePlayer(key, ref player.X, ref player.Y, ref player.MoveDirection); // 플레이어 이동 구현

                Dash(key, ref player.X, ref player.Y);

                Destroy(key);

                // 이동횟수 구하기
                if (key == ConsoleKey.RightArrow || key == ConsoleKey.LeftArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow || key == ConsoleKey.Spacebar)
                {
                    ++game.movecount;
                }


                // 아이템 먹으면 특정 프레임동안 플레이어의 이동 반대로 만들기

                if (IsCollided(player.X, player.Y, reverse.X, reverse.Y) == true) // 플레이어가 아이템을 먹는다면
                {
                    reverse.X = 0;
                    reverse.Y = 0; // 아이템은 치워버리고
                    reverse.lasttime = 10; // 디버프의 유효시간을 정해준다
                }

                if (0 < reverse.lasttime) // 만약 디버프 시간이 0보다 크다면
                {
                    switch (player.MoveDirection) // 이동을 반대로 구현해준다(대신 +2 또는 -2로)
                    {
                        case Direction.Right:
                            player.X = Math.Clamp(player.X - 2, game.min_x, game.max_x);
                            // 위에 MovePlayer()에서 이미 한칸 가서 2칸 빼주는거임. 그럼 1칸 빠지는거니까
                            break;
                        case Direction.Left:
                            player.X = Math.Clamp(player.X + 2, game.min_x, game.max_x);
                            break;
                        case Direction.Up:
                            player.Y = Math.Clamp(player.Y + 2, game.min_y, game.max_y);
                            break;
                        case Direction.Down:
                            player.Y = Math.Clamp(player.Y - 2, game.min_y, game.max_y);
                            break;
                    }
                    --reverse.lasttime; // 이동을 했으면 디버프시간을 1초 줄여준다
                }



                // trap 밟으면 피 하나 까이기
                for (int i = 0; i < trap.Length; ++i)
                {
                    if (IsCollided(player.X, player.Y, trap[i].X, trap[i].Y))
                    {
                        --player.HP;
                        trap[i].X = 0;
                        trap[i].Y = 0;
                        break;
                    }
                }





                // 플레이어와 벽의 충돌 처리
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    if (false == IsCollided(player.X, player.Y, walls[wallId].X, walls[wallId].Y))
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            player.X = walls[wallId].X + 1;
                            break;
                        case Direction.Right:
                            player.X = walls[wallId].X - 1;
                            break;
                        case Direction.Up:
                            player.Y = walls[wallId].Y + 1;
                            break;
                        case Direction.Down:
                            player.Y = walls[wallId].Y - 1;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");

                            return;
                    }

                    break;
                }


                // 박스 이동 처리
                // 플레이어가 박스를 밀었을 때라는 게 무엇을 의미하는가? => 플레이어가 이동했는데 플레이어의 위치와 박스 위치가 겹쳤다.
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    if (false == IsCollided(player.X, player.Y, boxes[i].X, boxes[i].Y))
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            boxes[i].X = Math.Clamp(boxes[i].X - 1, game.min_x, game.max_x);
                            player.X = boxes[i].X + 1;
                            break;
                        case Direction.Right:
                            boxes[i].X = Math.Clamp(boxes[i].X + 1, game.min_x, game.max_x);
                            player.X = boxes[i].X - 1;
                            break;
                        case Direction.Up:
                            boxes[i].Y = Math.Clamp(boxes[i].Y - 1, game.min_y, game.max_y);
                            player.Y = boxes[i].Y + 1;
                            break;
                        case Direction.Down:
                            boxes[i].Y = Math.Clamp(boxes[i].Y + 1, game.min_y, game.max_y);
                            player.Y = boxes[i].Y - 1;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");

                            return;
                    }

                    player.pushedBoxId = i;

                    break;
                }

                // 박스와 벽의 충돌 처리
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    if (false == IsCollided(boxes[player.pushedBoxId].X, boxes[player.pushedBoxId].Y, walls[wallId].X, walls[wallId].Y))
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            boxes[player.pushedBoxId].X = walls[wallId].X + 1;
                            player.X = boxes[player.pushedBoxId].X + 1;
                            break;
                        case Direction.Right:
                            boxes[player.pushedBoxId].X = walls[wallId].X - 1;
                            player.X = boxes[player.pushedBoxId].X - 1;
                            break;
                        case Direction.Up:
                            boxes[player.pushedBoxId].Y = walls[wallId].Y + 1;
                            player.Y = boxes[player.pushedBoxId].Y + 1;
                            break;
                        case Direction.Down:
                            boxes[player.pushedBoxId].Y = walls[wallId].Y - 1;
                            player.Y = boxes[player.pushedBoxId].Y - 1;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");

                            return;
                    }

                    break;
                }

                // 박스끼리 충돌 처리
                for (int collidedBoxId = 0; collidedBoxId < BOX_COUNT; ++collidedBoxId)
                {
                    // 같은 박스라면 처리할 필요가 X
                    if (player.pushedBoxId == collidedBoxId)
                    {
                        continue;
                    }

                    if (false == IsCollided(boxes[player.pushedBoxId].X, boxes[player.pushedBoxId].Y, boxes[collidedBoxId].X, boxes[collidedBoxId].Y))
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            boxes[player.pushedBoxId].X = boxes[collidedBoxId].X + 1;
                            player.X = boxes[player.pushedBoxId].X + 1;

                            break;
                        case Direction.Right:
                            boxes[player.pushedBoxId].X = boxes[collidedBoxId].X - 1;
                            player.X = boxes[player.pushedBoxId].X - 1;

                            break;
                        case Direction.Up:
                            boxes[player.pushedBoxId].Y = boxes[collidedBoxId].Y + 1;
                            player.Y = boxes[player.pushedBoxId].Y + 1;

                            break;
                        case Direction.Down:
                            boxes[player.pushedBoxId].Y = boxes[collidedBoxId].Y - 1;
                            player.Y = boxes[player.pushedBoxId].Y - 1;

                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");

                            return;
                    }

                    break;
                }
            }


            // D키를 눌러 벽 부수기
            void Destroy(ConsoleKey key)
            {
                if (key == ConsoleKey.D)
                {
                    if (0 < destroylimit && 0 < player.MP)
                    {
                        switch (player.MoveDirection)
                        {
                            case Direction.Right:
                                for (int i = 0; i < WALL_COUNT; ++i)
                                {
                                    if (player.X == walls[i].X - 1 && player.Y == walls[i].Y)
                                    {
                                        walls[i].X = 0;
                                        walls[i].Y = 0;
                                        --destroylimit;
                                        --player.MP;
                                    }
                                }
                                break;
                            case Direction.Left:
                                for (int i = 0; i < WALL_COUNT; ++i)
                                {
                                    if (player.X == walls[i].X + 1 && player.Y == walls[i].Y)
                                    {
                                        walls[i].X = 0;
                                        walls[i].Y = 0;
                                        --destroylimit;
                                        --player.MP;
                                    }
                                }
                                break;
                            case Direction.Up:
                                for (int i = 0; i < WALL_COUNT; ++i)
                                {
                                    if (player.X == walls[i].X && player.Y == walls[i].Y + 1)
                                    {
                                        walls[i].X = 0;
                                        walls[i].Y = 0;
                                        --destroylimit;
                                        --player.MP;
                                    }
                                }
                                break;
                            case Direction.Down:
                                for (int i = 0; i < WALL_COUNT; ++i)
                                {
                                    if (player.X == walls[i].X && player.Y == walls[i].Y - 1)
                                    {
                                        walls[i].X = 0;
                                        walls[i].Y = 0;
                                        --destroylimit;
                                        --player.MP;
                                    }
                                }
                                break;
                        }

                    }
                }
            }








            // space를 눌러 대쉬하는거임
            void Dash(ConsoleKey key, ref int x, ref int y)
            {
                if (key == ConsoleKey.Spacebar)
                {
                    if (0 < player.MP)
                    {
                        switch (player.MoveDirection)
                        {
                            case Direction.Right:
                                if (reverse.lasttime > 0)
                                {
                                    player.X = Math.Clamp(player.X - 2, game.min_x, game.max_x);

                                }
                                else
                                {
                                    player.X = Math.Clamp(player.X + 3, game.min_x, game.max_x);
                                }
                                --player.MP;
                                break;

                            case Direction.Left:
                                if (reverse.lasttime > 0)
                                {
                                    player.X = Math.Clamp(player.X + 2, game.min_x, game.max_x);
                                }
                                else
                                {
                                    player.X = Math.Clamp(player.X - 3, game.min_x, game.max_x);
                                }
                                --player.MP;
                                break;

                            case Direction.Up:
                                if (reverse.lasttime > 0)
                                {
                                    player.Y = Math.Clamp(player.Y + 2, game.min_y, game.max_y);
                                }
                                else
                                {
                                    player.Y = Math.Clamp(player.Y - 3, game.min_y, game.max_y);
                                }
                                --player.MP;
                                break;

                            case Direction.Down:
                                if (reverse.lasttime > 0)
                                {
                                    player.Y = Math.Clamp(player.Y - 2, game.min_y, game.max_y);
                                }
                                else
                                {
                                    player.Y = Math.Clamp(player.Y + 3, game.min_y, game.max_y);
                                }
                                --player.MP;
                                break;
                        }
                    }
                }
            }



            // 플레이어를 이동시킨다.
            void MovePlayer(ConsoleKey key, ref int x, ref int y, ref Direction moveDirection)
            {
                if (key == ConsoleKey.LeftArrow)
                {
                    x = Math.Max(game.min_x, x - 1);
                    moveDirection = Direction.Left;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    x = Math.Min(x + 1, game.max_x);
                    moveDirection = Direction.Right;
                }

                if (key == ConsoleKey.UpArrow)
                {
                    y = Math.Max(game.min_y, y - 1);
                    moveDirection = Direction.Up;
                }

                if (key == ConsoleKey.DownArrow)
                {
                    y = Math.Min(y + 1, game.max_y);
                    moveDirection = Direction.Down;
                }
            }

            // 두 물체가 충돌했는지 판별합니다.
            bool IsCollided(int x1, int y1, int x2, int y2)
            {
                if (x1 == x2 && y1 == y2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

        }




    }
}