using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace Snake_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImages = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food }

        };

        private readonly Dictionary<Direction, int> diroRotation = new()
        {
            {Direction.Up, 0},
            {Direction.Right,90 },
            {Direction.Down,180 },
            {Direction.Left,270 }
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRuning;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const byte VK_LSHIFT = 0xA0;
        private const uint KEYEVENTF_KEYUP = 0x0002;


        public MainWindow()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>//窗口加载完成后模拟按下 Shift 键
            {
                SimulateShiftPress();
            }));

            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                return;
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRuning)
            {
                gameRuning = true;
                await RunGame();
                gameRuning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.A:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.D:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.W:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.S:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }


        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width=GameGrid.Height*(cols / (double)rows);
            GameGrid.Height = GameGrid.Width * (rows / (double)cols);


            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImages[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headpos = gameState.HeadPosition();
            Image image = gridImages[headpos.Row, headpos.Col];
            image.Source = Images.Head;

            int rotation = diroRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }


        private void SimulateShiftPress()
        {
            // 按下左 Shift
            keybd_event(VK_LSHIFT, 0, 0, UIntPtr.Zero);
            // 释放左 Shift
            keybd_event(VK_LSHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverLayText.Text = i.ToString();
                await Task.Delay(500);
            }
            OverLayText.Text = "Go!";
            await Task.Delay(1000);
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);

            Overlay.Visibility = Visibility.Visible;
            OverLayText.Text = "按空格键重新开始";
        }
    }
}