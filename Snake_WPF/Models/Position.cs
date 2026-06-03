namespace Snake_WPF.Models
{
    /// <summary>
    /// 表示网格上的一个位置，由行（Row）和列（Col）组成。
    /// 提供位置平移、相等性比较和哈希码计算功能。
    /// </summary>
    public class Position
    {
        /// <summary>获取或设置行索引（从0开始）</summary>
        public int Row { get; set; }
        /// <summary>获取或设置列索引（从0开始）</summary>
        public int Col { get; set; }

        /// <summary>
        /// 使用指定行列初始化 Position 实例。
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="col">列索引</param>
        public Position(int row, int col)
        {
            Row = row; Col = col;
        }

        /// <summary>
        /// 根据给定方向平移位置，返回新位置。
        /// </summary>
        /// <param name="direction">移动方向（包含行、列偏移量）</param>
        /// <returns>平移后的新 Position 对象</returns>
        public Position Translate(Direction direction)
        {
            return new Position(Row + direction.RowOffset, Col + direction.ColOffset);
        }

        /// <summary>
        /// 判断当前位置是否与另一对象相等（基于行列值）。
        /// </summary>
        /// <param name="obj">待比较的对象</param>
        /// <returns>若 obj 是 Position 且行列相同则返回 true</returns>
        public override bool Equals(object? obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        /// <summary>
        /// 基于行列值生成哈希码。
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        /// <summary>相等运算符重载（比较行列是否相同）</summary>
        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        /// <summary>不等运算符重载</summary>
        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}