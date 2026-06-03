using System.CodeDom.Compiler;
using System.Windows.Media;

namespace Snake_WPF.Models
{
    /// <summary>
    /// 表示蛇的移动方向，封装行、列的偏移量。
    /// 提供四个预定义方向（Left, Right, Up, Down）及相反方向计算。
    /// </summary>
    public class Direction
    {
        /// 预定义方向实例：左 (行不变, 列-1)
        public readonly static Direction Left = new Direction(0, -1);
        // 预定义方向实例：右 (行不变, 列+1)
        public readonly static Direction Right = new Direction(0, 1);
        // 预定义方向实例：上 (行-1, 列不变)
        public readonly static Direction Up = new Direction(-1, 0);
        // 预定义方向实例：下 (行+1, 列不变)
        public readonly static Direction Down = new Direction(1, 0);

        /// <summary>行的偏移量</summary>
        public int RowOffset { get; set; }
        /// <summary>列的偏移量</summary>
        public int ColOffset { get; set; }

        /// <summary>
        /// 私有构造函数，确保只能通过预定义字段创建方向实例。
        /// </summary>
        /// <param name="rowOffset">行偏移量</param>
        /// <param name="colOffset">列偏移量</param>
        private Direction(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        /// <summary>
        /// 返回当前方向的反方向。
        /// </summary>
        /// <returns>相反方向的新 Direction 实例</returns>
        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColOffset);
        }

        /// <summary>
        /// 判断当前方向是否与另一对象相等。
        /// </summary>
        /// <param name="obj">待比较的对象</param>
        /// <returns>如果 obj 是 Direction 且偏移量相同则为 true</returns>
        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        /// <summary>
        /// 基于行、列偏移量生成哈希码。
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        /// <summary>相等运算符重载</summary>
        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        /// <summary>不等运算符重载</summary>
        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}