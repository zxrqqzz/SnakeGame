using System;
using System.Collections.Generic;
using System.Text;
using Tetris.Enums;

namespace Tetris.Models
{
    /// <summary>
    /// 表示一种俄罗斯方块形状（含所有旋转形态）
    /// </summary>
    public class Tetromino
    {
        /// <summary>形状类型</summary>
        public TetrominoType Type { get; }
        /// <summary>所有旋转形态的矩阵（最多4种）</summary>
        public bool[][,] Shapes { get; }

        public Tetromino(TetrominoType type, bool[][,] shapes)
        {
            Type = type;
            Shapes = shapes;
        }

        /// <summary>
        /// 获取默认旋转形态（索引0）的矩阵尺寸
        /// </summary>
        public int Size => Shapes[0].GetLength(0);//方块矩阵的大小（宽高相等）
    }


    /// <summary>
    /// 存储所有Tetromino实例的形状数据类，提供静态访问接口
    /// </summary>
    public static class TetrominoData
    {
        public static readonly Tetromino I = new Tetromino(
            TetrominoType.I,
            new bool[][,]
            {
                // 旋转0
                new bool[,] {
                    { false, false, false, false },
                    { true,  true,  true,  true },
                    { false, false, false, false },
                    { false, false, false, false }
                },
                // 旋转90°
                new bool[,] {
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false }
                },
                // 旋转180°（同旋转0）
                new bool[,] {
                    { false, false, false, false },
                    { true,  true,  true,  true },
                    { false, false, false, false },
                    { false, false, false, false }
                },
                // 旋转270°（同旋转90°）
                new bool[,] {
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false }
                }
            });

        /// <summary>
        /// O 
        /// </summary>
        public static readonly Tetromino O = new Tetromino(
            TetrominoType.O,
            new bool[][,]
            {
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  true,  false },
                    { false, true,  true,  false },
                    { false, false, false, false }
                },
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  true,  false },
                    { false, true,  true,  false },
                    { false, false, false, false }
                },
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  true,  false },
                    { false, true,  true,  false },
                    { false, false, false, false }
                },
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  true,  false },
                    { false, true,  true,  false },
                    { false, false, false, false }
                }
            }
        );

        /// <summary>
        /// T 
        /// </summary>
        public static readonly Tetromino T = new Tetromino(
            TetrominoType.T,
            new bool[][,]
            {
                // 旋转0
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  false, false },
                    { true,  true,  true,  false },
                    { false, false, false, false }
                },
                // 旋转90°
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  false, false },
                    { false, true,  true,  false },
                    { false, true,  false, false }
                },
                // 旋转180°
                new bool[,] {
                    { false, false, false, false },
                    { false, false, false, false },
                    { true,  true,  true,  false },
                    { false, true,  false, false }
                },
                // 旋转270°
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  false, false },
                    { true,  true,  false, false },
                    { false, true,  false, false }
                }
            }
        );

        /// <summary>
        /// L 
        /// </summary>
        public static readonly Tetromino L = new Tetromino(
            TetrominoType.L,
            new bool[][,]
            {
                // 0°
                new bool[,] {
                    { false, false, false, false },
                    { false, false, true, false  },
                    { true,  true,  true, false  },
                    { false, false, false, false }
                },
                // 90°
                new bool[,] {
                    { false, false, false, false },
                    { false, true,  false, false },
                    { false, true,  false, false },
                    { false, true,  true,  false }
                },
                // 180°
                new bool[,] {
                    { false, false, false, false },
                    { false, false, false, false },
                    { true,  true,  true,  false },
                    { true,  false, false, false }
                },
                // 270°
                new bool[,] {
                    { false, false, false, false },
                    { true,  true,  false, false },
                    { false, true,  false, false },
                    { false, true,  false, false }
                }
            }
        );

        /// <summary>
        /// Z 
        /// </summary>
        public static readonly Tetromino Z = new Tetromino(
            TetrominoType.Z,
            new bool[][,]
            {
                // 0°
                new bool[,] {
                    { false, false, false, false },
                    { true,  true,  false, false },
                    { false, true,  true, false },
                    { false, false, false, false }
                },
                // 90°
                new bool[,] {
                    { false, false, false, false },
                    { false, false, true, false },
                    { false, true,  true, false },
                    { false, true,  false, false }
                },
                // 180° (同0°)
                new bool[,] {
                    { false, false, false, false },
                    { true,  true,  false, false },
                    { false, true,  true, false },
                    { false, false, false, false }
                },
                // 270° (同90°)
                new bool[,] {
                    { false, false, false, false },
                    { false, false, true, false },
                    { false, true,  true, false },
                    { false, true,  false, false }
                }
            }
        );

        public static readonly Tetromino[] All = new Tetromino[]
        {
            I, O, T, L, Z
        };
    }    
}
