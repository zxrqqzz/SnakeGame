using System;
using System.Collections.Generic;
using System.Text;
using Tetris.Enums;

namespace Tetris.Models
{
    public class GameState
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public GridValue[,] Grid { get; set; }
        public Direction Dir { get; set; }
        public int Score { get; set; }
        public bool GameOver { get; set; }

        //当前方块
        public Tetromino CurrentTetromino { get; set; }
        public int CurrentRow {  get; set; }
        public int CurrentCol { get; set; }
        public int CurrentRotation { get; set; }
        //下一个方块
        public Tetromino NextTetromino { get; set; }

        /// <summary>
        /// 方块的位置
        /// </summary>
        private readonly LinkedList<Position> squarePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];

            NextTetromino = GetTetromino();
            SpawnTetromino();

        }

        /// <summary>
        /// 检查是否有空位可以放置新的方块
        /// </summary>
        /// <returns>空位集合</returns>
        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        /// <summary>
        /// 从预定义的形状库中随机选取一个形状（Tetromino）
        /// </summary>
        private Tetromino GetTetromino()
        {
            var all = TetrominoData.All;
            return all[random.Next(all.Length)];
        }

        /// <summary>
        /// 生成新的当前活动方块
        /// </summary>
        private void SpawnTetromino()
        {
            CurrentTetromino=NextTetromino ?? GetTetromino();
            NextTetromino=GetTetromino();

            // 初始位置：行从0开始，列居中（根据当前方块的矩阵尺寸计算偏移）
            CurrentRow = 0;
            int size = CurrentTetromino.Size;
            CurrentCol = Cols / 2 - size / 2;
            CurrentRotation = 0;

            if (Collision())
                GameOver = true;

        }
        /// <summary>
        /// 检测当前活动方块是否与边界或网格中的固定方块发生碰撞
        /// </summary>
        private bool Collision()
        {
            bool[,] matrix = CurrentTetromino.Shapes[CurrentRotation];
            int size = CurrentTetromino.Size;
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (matrix[r, c])
                    {
                        int boardR = CurrentRow + r;
                        int boardC = CurrentCol + c;
                        // 超出边界或与已有方块重叠
                        if (boardR >= Rows || boardC < 0 || boardC >= Cols || boardC < 0)
                            return true;
                        if (boardR>=0 && Grid[boardR, boardC] != GridValue.Empty)
                            return true;
                    }
                }
            }
            return false;
        }

        private void PlacePiece()
        {
            bool[,] matrix = CurrentTetromino.Shapes[CurrentRotation];
            int size = CurrentTetromino.Size;
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (matrix[r, c])
                    {
                        int boardR = CurrentRow + r;
                        int boardC = CurrentCol + c;
                        if (boardR >= 0 && boardR < Rows && boardC >= 0 && boardC < Cols)
                            Grid[boardR, boardC] = GridValue.Square;
                    }
                }
            }
        }

        public void MoveDown()
        {
            if (GameOver) return;

            // 尝试向下移动
            CurrentRow++;
            if (Collision())   // 移动后发生碰撞
            {
                CurrentRow--;          // 回退到原位置
                PlacePiece();          // 固定到网格
                SpawnTetromino();      // 生成下一个方块
            }
        }

    }
}
