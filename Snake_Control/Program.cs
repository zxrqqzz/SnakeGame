using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleSnake
{
    class Program
    {
        static int width = 30;
        static int height = 30;
        static int x = width / 2;
        static int y = height / 2;
        static int pos = 0;//当前位置
        static bool goRight = true;//是否向右移动
        static int dy = 0, dx = 0;//方向移动
        static int nextX = 1, nextY = 0;//下一个位置
        static bool running = true;//是否运行
        static int StepDelayMs = 200;//每一步的延迟时间
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height + 3);
            Console.SetBufferSize(width + 2, height + 3);
            DateTime lastMove = DateTime.Now;

            while (true)
            {
                HandleInput();
                if ((DateTime.Now - lastMove).TotalMilliseconds >= StepDelayMs)
                {
                    lastMove = DateTime.Now;
                    Update();
                    Draw();
                }
                Thread.Sleep(10); // 等待一段时间
                
            }

            void Draw()
            {
                // 绘制：清空行，然后在 pos 处画 O
                Console.SetCursorPosition(0, 1);
                Console.Write(new string(' ', width)); // 清空行
                Console.SetCursorPosition(pos, 1); // 在 pos 处画 O
                Console.Write("o");
            }

            static void UpdateBack()
            {
                if (goRight)
                {
                    pos++;
                    if (pos >= width)
                    {
                        goRight = false;
                        pos = width - 1; // 确保 pos 不超过边界
                    }
                }
                else
                {
                    pos--;
                    if (pos < 0)
                    {
                        goRight = true;
                        pos = 0; // 确保 pos 不超过边界
                    }
                }
            }
            static void HandleInput()
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W: if (dy != 1) { nextX = 0; nextY = -1; } break;
                    case ConsoleKey.S: if (dy != -1) { nextX = 0; nextY = 1; } break;
                    case ConsoleKey.A: if (dx != 1) { nextX = -1; nextY = 0; } break;
                    case ConsoleKey.D: if (dx != -1) { nextX = 1; nextY = 0; } break;
                    case ConsoleKey.Escape: running = false; break;
                }
            }
            static void Update()
            {
                dx = nextX;
                dy = nextY;

                //计算新位置
                int newX = x + dx;
                int newY = y + dy;

                if (newX < 0 || newY < 0 || newX > width || newY > height)
                {
                    running = false;
                    return;
                }
                x = newX;
                y = newY;

            }
            static void DrawGame()
            {

            }
        }
    }
}