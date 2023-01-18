﻿using Sokoban_Project;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Sokoban
{



    class Eohkoban
    {



        static void Main()
        {
            Random random = new(); // Random 클라스 인스턴스화
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

            Player player = new Player(); // player 클라스 인스턴스화

            Debuffs reverse = new Debuffs(); // debuff 클라스 리버스아이템 인스턴스화
            Debuffs[] hpitem =
            {
                new Debuffs{X = 23, Y = 8 },
                new Debuffs{X = 17, Y = 3},
                new Debuffs{X = 3, Y = 9 }
            };
            int hpcount = hpitem.Length;
            // debuff 클라스 체력아이템 인스턴스화

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




            #region GameLoop
            // 게임 루프 구성
            while (true)
            {
                // Render
                Render();

                // Input
                ConsoleKey key = Console.ReadKey().Key;

                // Update

                MovePlayer(key, ref player.X, ref player.Y, ref player.MoveDirection); // 플레이어 이동 구현

                Reverse_item_obtained(); // 리버스 아이템 먹으면 이동방향 반대로 가게하기

                Pushbox(); // 박스 밀기

                Wall_blocks_player(); // 벽이 플레이어 막기

                Wall_blocks_box(); // 벽이 박스 막기

                Box_blocks_anotherbox(); // 박스가 박스 막기

                Jump(key, ref player.X, ref player.Y); // 점프하기

                Destroy_wall(key); // 벽 부수기

                Countmovement(key); // 이동횟수 세주기

                Trap_activated(); // 함정 밟으면 피 깎이기

                Countboxongoal(boxes, goals); // 골 위 박스 몇개인지 세주기

                Hp_item_obtained(); // hp item 먹으면 랜덤으로 피 회복 or 차감

                // 모든 골 지점에 박스가 올라와 있다면?
                if (Countboxongoal(boxes, goals) == GOAL_COUNT)
                {
                    Console.Clear();
                    Console.WriteLine("축하합니다. 클리어 하셨습니다.");

                    break;
                }
                else if (game.movecount == game.maxmove || player.HP < 1)
                {
                    Console.Clear();
                    Console.WriteLine("실패하셨습니당");

                    break;
                }


            }
            #endregion










            #region 함수 세트


            #region 렌더링
            // 프레임을 그립니다.
            void Render()
            {
                // 이전 프레임을 지운다.
                Console.Clear();

                // 플레이어 HP 그리기
                for (int i = 0; i < player.HP; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 3, "player HP: ");
                    //RenderObject(game.max_x+5, game.min_y+3, "player HP: ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Update_set.RenderObject(game.max_x + 16 + (i * 2), game.min_y + 3, "■");
                }

                // 플레이어 MP 그리기
                for (int i = 0; i < player.MP; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 4, "player MP: ");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Update_set.RenderObject(game.max_x + 16 + (i * 2), game.min_y + 4, "■");
                }

                // 게임 옆에 이동횟수 텍스트 그리기
                Console.ForegroundColor = ConsoleColor.White;
                Update_set.RenderObject(game.max_x + 5, game.min_y + 5, $"현재 이동횟수 : {game.movecount} / {game.maxmove}");

                // 게임 옆에 reverse 유효시간 텍스트 그리기
                if (0 < reverse.lasttime)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 7, $"Reverse 유효시간: {reverse.lasttime}");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                // 게임 옆에 destory 잔여횟수 텍스트 그리기
                if (0 < destroylimit)
                {
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 6, $"destroy 잔여횟수: {destroylimit}");
                }

                Update_set.RenderObject(game.max_x + 5, game.min_y + 9, $"⊙ = 덫 (--HP)");

                // reverse item 설명 그리기
                if (reverse.X != 0 || reverse.Y != 0)
                {
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 10, $"◐ =  Reverse item");
                }

                // hp item 설명 그리기
                if (hpcount != 0)
                {
                    Update_set.RenderObject(game.max_x + 5, game.min_y + 11, $"? = Hp item (회복 or 차감)");
                }


                // 게임 밑에 사용키 설명 텍스트 그리기
                Update_set.RenderObject(game.min_x + 3, game.max_y + 3, "Space = 대쉬(3칸)");
                Update_set.RenderObject(game.min_x + 3, game.max_y + 4, "D = 벽 부수기(벽만 가능)");

                // 리버스 그리기
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Update_set.RenderObject(reverse.X, reverse.Y, "◐");

                // trap 그리기
                for (int i = 0; i < trap.Length; ++i)
                {

                    Update_set.RenderObject(trap[i].X, trap[i].Y, "⊙");
                }

                // 골을 그린다.
                for (int i = 0; i < GOAL_COUNT; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Update_set.RenderObject(goals[i].X, goals[i].Y, "G");
                }

                // 체력아이템을 그린다
                Console.ForegroundColor = ConsoleColor.Gray;
                for (int i = 0; i < hpitem.Length ; ++i)
                {
                    Update_set.RenderObject(hpitem[i].X, hpitem[i].Y, "?");
                }
                


                // 플레이어를 그린다.
                Console.ForegroundColor = ConsoleColor.White;
                Update_set.RenderObject(player.X, player.Y, "P");


                // 박스를 그린다.
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    string boxShape = boxes[boxId].IsOnGoal ? "★" : "O";
                    Update_set.RenderObject(boxes[boxId].X, boxes[boxId].Y, boxShape);
                }


                // 벽을 그린다.
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Update_set.RenderObject(walls[wallId].X, walls[wallId].Y, "W");

                }

                // 바운더리 그리기
                for (int i = 0; i < game.max_x + 2; ++i)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Update_set.RenderObject(i, game.min_y - 1, game.boundary);
                    Update_set.RenderObject(i, game.max_y + 1, game.boundary);
                }
                for (int i = 0; i < game.max_y + 1; ++i)
                {
                    Update_set.RenderObject(game.min_x - 1, i, game.boundary);
                    Update_set.RenderObject(game.max_x + 1, i, game.boundary);
                }


            }

            #endregion 


            void Latter_blocks_move(Action action) => action();
            
            
            // hp item 먹으면 피 랜덤으로 까이기
            void Hp_item_obtained()
            {
                for (int i = 0; i < hpitem.Length; ++i)
                {
                    if (IsCollided(player.X, player.Y, hpitem[i].X, hpitem[i].Y))
                    {
                        hpitem[i].X = 0;
                        hpitem[i].Y = 0;

                        hpcount--;
                        int hpchange = random.Next(-3, 5);
                        player.HP += hpchange;
                        break;
                    }
                }
            }


            // 박스 이동 처리
            // 플레이어가 박스를 밀었을 때라는 게 무엇을 의미하는가? => 플레이어가 이동했는데 플레이어의 위치와 박스 위치가 겹쳤다.
            void Pushbox()
            {
                for (int i = 0; i < BOX_COUNT; ++i)
                {
                    if (IsCollided(player.X, player.Y, boxes[i].X, boxes[i].Y) == false)
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            Moveleft(ref boxes[i].X);
                            Latter_blocks_leftmove(ref player.X, boxes[i].X);
                            break;

                        case Direction.Right:
                            Moveright(ref boxes[i].X);
                            Latter_blocks_rightmove(ref player.X, boxes[i].X);
                            break;

                        case Direction.Up:
                            Moveup(ref boxes[i].Y);
                            Latter_blocks_upmove(ref player.Y, boxes[i].Y);
                            break;

                        case Direction.Down:
                            Movedown(ref boxes[i].Y);
                            Latter_blocks_downmove(ref player.Y, boxes[i].Y);
                            break;

                        default:
                            Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                            return;
                    }

                    player.pushedBoxId = i;
                }
            }


            // 플레이어와 벽의 충돌 처리
            void Wall_blocks_player()
            {

                for (int i = 0; i < WALL_COUNT; ++i)
                {
                    if (IsCollided(player.X, player.Y, walls[i].X, walls[i].Y) == false)
                    {
                        continue;
                    }

                    switch (player.MoveDirection)
                    {
                        case Direction.Left:
                            Latter_blocks_leftmove(ref player.X, walls[i].X);
                            break;

                        case Direction.Right:
                            Latter_blocks_rightmove(ref player.X, walls[i].X);
                            break;

                        case Direction.Up:
                            Latter_blocks_upmove(ref player.Y, walls[i].Y);
                            break;

                        case Direction.Down:
                            Latter_blocks_downmove(ref player.Y, walls[i].Y);
                            break;

                        default:
                            Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                            return;
                    }

                }
            }


            void Wall_blocks_box()
            {
                // 박스와 벽의 충돌 처리
                for (int i = 0; i < WALL_COUNT; ++i)
                {
                    if (IsCollided(boxes[player.pushedBoxId].X, boxes[player.pushedBoxId].Y, walls[i].X, walls[i].Y))
                    {
                        switch (player.MoveDirection)
                        {
                            case Direction.Left:
                                Latter_blocks_leftmove(ref boxes[player.pushedBoxId].X, walls[i].X);
                                Latter_blocks_leftmove(ref player.X, boxes[player.pushedBoxId].X);
                                break;

                            case Direction.Right:
                                Latter_blocks_rightmove(ref boxes[player.pushedBoxId].X, walls[i].X);
                                Latter_blocks_rightmove(ref player.X, boxes[player.pushedBoxId].X);
                                break;
                            case Direction.Up:
                                Latter_blocks_upmove(ref boxes[player.pushedBoxId].Y, walls[i].Y);
                                Latter_blocks_upmove(ref player.Y, boxes[player.pushedBoxId].Y);
                                break;
                            case Direction.Down:
                                Latter_blocks_downmove(ref boxes[player.pushedBoxId].Y, walls[i].Y);
                                Latter_blocks_downmove(ref player.Y, boxes[player.pushedBoxId].Y);
                                break;
                            default:
                                Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                                return;
                        }
                    }

                }
            }

            void Box_blocks_anotherbox()
            {
                // 박스끼리 충돌 처리
                for (int collidedBoxId = 0; collidedBoxId < BOX_COUNT; ++collidedBoxId)
                {
                    // 같은 박스라면 처리할 필요가 X
                    if (player.pushedBoxId == collidedBoxId)
                    {
                        continue;
                    }

                    if (IsCollided(boxes[player.pushedBoxId].X, boxes[player.pushedBoxId].Y, boxes[collidedBoxId].X, boxes[collidedBoxId].Y))
                    {


                        switch (player.MoveDirection)
                        {
                            case Direction.Left:

                                Action action = () =>
                                {
                                    Latter_blocks_leftmove(ref boxes[player.pushedBoxId].X, boxes[collidedBoxId].X);
                                    Latter_blocks_leftmove(ref player.X, boxes[player.pushedBoxId].X);
                                };
                                Latter_blocks_move(action);
                                //target_blocks_leftmove(ref boxes[player.pushedBoxId].X, boxes[collidedBoxId].X);
                                //target_blocks_leftmove(ref player.X, boxes[player.pushedBoxId].X);

                                break;
                            case Direction.Right:

                                action = () =>
                                {
                                    Latter_blocks_rightmove(ref boxes[player.pushedBoxId].X, boxes[collidedBoxId].X);
                                    Latter_blocks_rightmove(ref player.X, boxes[player.pushedBoxId].X);
                                };
                                Latter_blocks_move(action);
                                //target_blocks_rightmove(ref boxes[player.pushedBoxId].X, boxes[collidedBoxId].X);
                                //target_blocks_rightmove(ref player.X, boxes[player.pushedBoxId].X);

                                break;
                            case Direction.Up:

                                action = () =>
                                {
                                    Latter_blocks_upmove(ref boxes[player.pushedBoxId].Y, boxes[collidedBoxId].Y);
                                    Latter_blocks_upmove(ref player.Y, boxes[player.pushedBoxId].Y);
                                };
                                Latter_blocks_move(action);
                                //target_blocks_upmove(ref boxes[player.pushedBoxId].Y, boxes[collidedBoxId].Y);
                                //target_blocks_upmove(ref player.Y, boxes[player.pushedBoxId].Y);

                                break;
                            case Direction.Down:

                                action = () =>
                                {
                                    Latter_blocks_downmove(ref boxes[player.pushedBoxId].Y, boxes[collidedBoxId].Y);
                                    Latter_blocks_downmove(ref player.Y, boxes[player.pushedBoxId].Y);
                                };
                                Latter_blocks_move(action);
                                //target_blocks_downmove(ref boxes[player.pushedBoxId].Y, boxes[collidedBoxId].Y);
                                //target_blocks_downmove(ref player.Y, boxes[player.pushedBoxId].Y);

                                break;
                            default:
                                Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                                return;
                        }
                    }

                }
            }

            // 골위에 박스 몇개인지 세주는 함수
            int Countboxongoal(Box[] boxes, Goal[] goals)
            {
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

                return boxOnGoalCount;
            }

            // trap 밟으면 피 하나 까이기
            void Trap_activated()
            {

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
            }

            // 아이템 먹으면 특정 프레임동안 플레이어의 이동 반대로 만들기
            void Reverse_item_obtained()
            {
                if (IsCollided(player.X, player.Y, reverse.X, reverse.Y)) // 플레이어가 아이템을 먹는다면
                {
                    reverse.X = 0;
                    reverse.Y = 0; // 아이템은 치워버리고
                    reverse.lasttime = 10; // 디버프의 유효시간을 정해준다
                }

                if (0 < reverse.lasttime) // 만약 디버프 시간이 0보다 크다면
                {
                    switch (player.MoveDirection) // 이동을 반대로 구현해준다(대신 +2 또는 -2로) 왜? 위에 MovePlayer()에서 이미 한칸 가서 2칸 빼주는거임. 그럼 1칸 빠지는거니까
                    {
                        case Direction.Right:
                            Reverse_Moveright(ref player.X);
                            break;
                        case Direction.Left:
                            Reverse_Moveleft(ref player.X);
                            break;
                        case Direction.Up:
                            Reverse_Moveup(ref player.Y);
                            break;
                        case Direction.Down:
                            Reverse_Movedown(ref player.Y);
                            break;
                        default:
                            Exiterrormsg($"you have entered something wrong : {player.MoveDirection}");
                            break;
                    }
                    --reverse.lasttime; // 이동을 했으면 디버프시간을 1초 줄여준다
                }
            }


            // D키를 눌러 벽 부수기
            void Destroy_wall(ConsoleKey key)
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
                                    if (IsCollided(player.X, player.Y, walls[i].X - 1, walls[i].Y))
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
                                    if (IsCollided(player.X, player.Y, walls[i].X + 1, walls[i].Y))
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
                                    if (IsCollided(player.X, player.Y, walls[i].X, walls[i].Y + 1))
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
                                    if (IsCollided(player.X, player.Y, walls[i].X, walls[i].Y - 1))
                                    {
                                        walls[i].X = 0;
                                        walls[i].Y = 0;
                                        --destroylimit;
                                        --player.MP;
                                    }
                                }
                                break;

                            default:
                                Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                                return;
                        }

                    }
                }
            }








            // space를 눌러 점프하는거임
            void Jump(ConsoleKey key, ref int x, ref int y)
            {
                if (key == ConsoleKey.Spacebar)
                {
                    if (0 < player.MP)
                    {
                        switch (player.MoveDirection)
                        {
                            case Direction.Right:
                                if (reverse.lasttime > 0) // 만약 리버스 시간이 남아있다면
                                {
                                    player.X = Math.Clamp(player.X - 2, game.min_x, game.max_x); // 반대방향으로 점프
                                }
                                else // 리버스 시간이 종료되었다면
                                {
                                    player.X = Math.Clamp(player.X + 2, game.min_x, game.max_x); // 정방향으로 점프
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
                                    player.X = Math.Clamp(player.X - 2, game.min_x, game.max_x);
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
                                    player.Y = Math.Clamp(player.Y - 2, game.min_y, game.max_y);
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
                                    player.Y = Math.Clamp(player.Y + 2, game.min_y, game.max_y);
                                }
                                --player.MP;
                                break;

                            default:
                                Exiterrormsg($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {player.MoveDirection}");
                                return;
                        }
                    }
                }
            }



            // 플레이어를 이동시킨다.
            void MovePlayer(ConsoleKey key, ref int x, ref int y, ref Direction moveDirection)
            {
                if (key == ConsoleKey.LeftArrow)
                {
                    Moveleft(ref x);
                    moveDirection = Direction.Left;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    Moveright(ref x);
                    moveDirection = Direction.Right;
                }

                if (key == ConsoleKey.UpArrow)
                {
                    Moveup(ref y);
                    moveDirection = Direction.Up;
                }

                if (key == ConsoleKey.DownArrow)
                {
                    Movedown(ref y);
                    moveDirection = Direction.Down;
                }
            }

            // 정방향 이동 함수
            void Moveleft(ref int x) => x = Math.Clamp(x - 1, game.min_x, game.max_x);
            void Moveright(ref int x) => x = Math.Clamp(x + 1, game.min_x, game.max_x);
            void Moveup(ref int y) => y = Math.Clamp(y - 1, game.min_y, game.max_y);
            void Movedown(ref int y) => y = Math.Clamp(y + 1, game.min_y, game.max_y);

            // 반대방향 이동함수
            void Reverse_Moveright(ref int x) => x = Math.Clamp(x - 2, game.min_x, game.max_x);
            void Reverse_Moveleft(ref int x) => x = Math.Clamp(x + 2, game.min_x, game.max_x);
            void Reverse_Moveup(ref int y) => y = Math.Clamp(y + 2, game.min_y, game.max_y);
            void Reverse_Movedown(ref int y) => y = Math.Clamp(y - 2, game.min_y, game.max_y);

            // 두 물체가 충돌했는지 판별함
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

            // 이동횟수 카운트 해주는 함수
            void Countmovement(ConsoleKey key)
            {
                if (key == ConsoleKey.RightArrow || key == ConsoleKey.LeftArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow || key == ConsoleKey.Spacebar)
                {
                    ++game.movecount;
                }
            }

            // 에러메세지 함수
            void Exiterrormsg(string message)
            {
                Console.Clear();
                Console.WriteLine(message);
            }

            // 뒤에놈이 앞에놈의 이동을 막는 함수
            void Latter_blocks_leftmove(ref int x, int target) => x = target + 1;
            void Latter_blocks_rightmove(ref int x, int target) => x = target - 1;
            void Latter_blocks_upmove(ref int y, int target) => y = target + 1;
            void Latter_blocks_downmove(ref int y, int target) => y = target - 1;


            #endregion

        }




    }
}