using System.Drawing.Imaging.Effects;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.Enums;
using Tetris.Models;
using Tetris.Utilities;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImages = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Square, Images.Square },
        };

        // 当前游戏网格的行数和列数（初始15x15，升级时会增加）
        private int rows = 15, cols = 15;
        private GameState gameState;
        private Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();

            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);

            DrawGrid();
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);
            GameGrid.Height = GameGrid.Width * (rows / (double)cols);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image img = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5),

                    };
                    images[r, c] = img;
                    GameGrid.Children.Add(img);
                }
            }
            return images;
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridvalue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImages[gridvalue];
                }
            }
        }

        /// <summary>
        /// 整体绘制
        /// </summary>
        private void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"Score：{gameState.Score}";
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.MoveDown();
            }
        }
    }
}