using Snake_WPF.Enums;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Media.Animation;

namespace Snake_WPF.Models
{
    public class GameState
    {
        /// <summary>网格行数</summary>
        public int Rows { get; set; }
        /// <summary>网格列数</summary>
        public int Cols { get; set; }
        /// <summary>网格二维数组，存储每个格子是 Empty/Snake/Food/Outside</summary>
        public GridValue[,] Grid { get; set; }
        /// <summary>当前蛇移动方向</summary>
        public Direction Dir { get; set; }
        /// <summary>当前得分</summary>
        public int Score { get; set; }
        /// <summary>游戏是否结束</summary>
        public bool GameOver { get; set; }
        /// <summary>蛇长度达到多少时出现道具</summary>
        public int nextThreshold = 5;
        /// <summary>当前是否已生成过关道具</summary>
        public bool hasNextItem = false;
        /// <summary>当前蛇身长度</summary>
        public int SnakeLength => snakePositions.Count;
        /// <summary>开启下一关/summary>
        public event Action? OnNextLevelTriggered;

        /// <summary>
        /// 存储玩家输入的方向改变
        /// </summary>
        private readonly LinkedList<Direction> dirChanges=new LinkedList<Direction>();
        /// <summary>
        /// 蛇的位置
        /// </summary>
        private readonly LinkedList<Position> snakePositions=new LinkedList<Position>();
        private readonly Random random = new Random();//随机食物位置

        /// <summary>
        /// 初始化游戏状态，创建指定大小的网格，生成初始蛇和第一个食物。
        /// </summary>
        /// <param name="rows">网格行数</param>
        /// <param name="cols">网格列数</param>
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];//初始化为默认值Empty
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        /// <summary>
        /// 在网格中央生成初始蛇身（长度为3，水平放置，头部向右）。
        /// </summary>
        public void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        /// <summary>
        /// 遍历整个网格，返回所有值为 Empty 的位置
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
        /// 随机选择一个空位放置食物。如果没有空位则不做任何操作。
        /// </summary>
        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0) return;//没有空位了

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        /// <summary>
        /// 生成过关道具
        /// </summary>
        private void AddNextItem()
        {
            if (hasNextItem) return;
            if (SnakeLength < nextThreshold) return;

            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0) return;

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Next;
            hasNextItem = true;
        }
        /// <summary>
        /// 获取蛇头位置
        /// </summary>
        /// <returns>蛇头坐标</returns>
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        /// <summary>
        /// 获取蛇尾位置
        /// </summary>
        /// <returns>蛇尾坐标</returns>
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        /// <summary>
        /// 获取蛇身所有位置（从头到尾）
        /// </summary>
        /// <returns>蛇位置的可枚举集合</returns>
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        /// <summary>
        /// 在蛇头位置添加新块
        /// </summary>
        /// <param name="pos">新头部位置</param>
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        /// <summary>
        /// 移除蛇尾块
        /// </summary>
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }
        /// <summary>
        /// 获取队列中最后待处理的方向
        /// </summary>
        /// <returns>最后的方向</returns>
        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
                return Dir;
            return dirChanges.Last.Value;
        }
        /// <summary>
        /// 判断是否可以改变方向（不能与当前方向相反，且未处理队列最多容纳2个）
        /// </summary>
        /// <param name="newDir">新方向</param>
        /// <returns>是否允许改变</returns>
        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;//已经有两次未处理的方向改变了
            }
            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();//不能改变为当前方向或相反方向

        }
        /// <summary>
        /// 外部接口：请求改变方向。仅当方向有效时加入队列。
        /// </summary>
        /// <param name="Dir">请求的新方向</param>
        public void ChangeDirection(Direction Dir)
        {
            if (CanChangeDirection(Dir))
                dirChanges.AddLast(Dir);
        }

        /// <summary>
        /// 判断一个位置是否超出网格边界。
        /// </summary>
        /// <param name="pos">要检查的位置</param>
        /// <returns>是否超出边界</returns>
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        /// <summary>
        /// 预测新头部位置会碰到什么
        /// 特殊处理：如果新头部位置是蛇尾，视为 Empty。
        /// </summary>
        /// <param name="newHeadPos">新头部位置</param>
        /// <returns>碰到的物体</returns>
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
        /// <summary>
        /// 核心移动逻辑：处理队列中的方向，计算新头部，根据碰撞结果执行相应动作。
        /// - 撞墙或撞身：游戏结束
        /// - 吃到食物：头部增加，分数+1，生成新食物
        /// - 普通移动：头部增加，尾部移除
        /// </summary>
        public void Move()
        {
            if(dirChanges.Count > 0)//处理队列中的方向，计算新头部，根据碰撞结果执行相应动作。
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
                AddNextItem();
            }
            else if(hit == GridValue.Next)
            {
                hasNextItem = false;
                OnNextLevelTriggered?.Invoke();
            }
        }
    }
}
