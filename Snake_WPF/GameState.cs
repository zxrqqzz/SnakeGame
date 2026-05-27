using System.DirectoryServices.ActiveDirectory;
using System.Windows.Media.Animation;

namespace Snake_WPF
{
    public class GameState
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public GridValue[,] Grid { get; set; }
        public Direction Dir {  get; set; }
        public int Score { get; set; }
        public bool GameOver { get; set; }

        private readonly LinkedList<Direction> dirChanges=new LinkedList<Direction>();//存储玩家输入的方向改变
        private readonly LinkedList<Position> snakePositions=new LinkedList<Position>();//蛇的位置
        private readonly Random random = new Random();//食物位置

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];//初始化为默认值Empty
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        public void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
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
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0) return;//没有空位了

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }
        /// <summary>
        /// 获取蛇头位置
        /// </summary>
        /// <returns></returns>
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        /// <summary>
        /// 获取蛇尾位置
        /// </summary>
        /// <returns></returns>
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }
        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
                return Dir;
            return dirChanges.Last.Value;
        }
        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;//已经有两次未处理的方向改变了
            }
            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();//不能改变为当前方向或相反方向

        }
        public void ChangeDirection(Direction Dir)
        {
            if (CanChangeDirection(Dir))
                dirChanges.AddLast(Dir);
        }
        
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos==TailPosition())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }
        public void Move()
        {
            if(dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (hit==GridValue.Outside||hit==GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
