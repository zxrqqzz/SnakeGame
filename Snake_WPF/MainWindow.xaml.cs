using Snake_WPF.Enums;
using Snake_WPF.Models;
using Snake_WPF.Utilities;
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
        /// <summary>
        /// 将网格中的逻辑值（Empty, Snake, Food, Next）映射为实际要显示的图片
        /// </summary>
        private readonly Dictionary<GridValue, ImageSource> gridValToImages = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food },
            {GridValue.Next, Images.Next }
        };

        /// <summary>
        /// 将移动方向映射为蛇头图片需要旋转的角度（0°上，90°右，180°下，270°左）
        /// </summary>
        private readonly Dictionary<Direction, int> diroRotation = new()
        {
            {Direction.Up, 0},
            {Direction.Right,90 },
            {Direction.Down,180 },
            {Direction.Left,270 }
        };

        // 当前游戏网格的行数和列数（初始15x15，升级时会增加）
        private int rows = 15, cols = 15;
        // 存储所有显示格子的Image控件引用，用于快速更新图片
        private Image[,] gridImages;
        // 游戏核心逻辑模型
        private GameState gameState;
        // 标记游戏是否正在运行中（用于避免多个游戏循环同时执行）
        private bool gameRuning;
        // 标记是否正在进行关卡升级（用于抑制GameOver界面的显示）
        private bool levelUpInProgress;

        // 导入Windows API，用于模拟键盘按键
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        // 左Shift键的虚拟码
        private const byte VK_LSHIFT = 0xA0;
        // 按键释放标志
        private const uint KEYEVENTF_KEYUP = 0x0002;


        public MainWindow()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>//窗口加载完成后模拟按下 Shift 键
            {
                SimulateShiftPress();
            }));

            // 创建显示网格（所有格子图片）
            gridImages = SetupGrid();
            // 创建游戏逻辑模型
            gameState = new GameState(rows, cols);
            //gameState.OnNextLevelTriggered += GameState_OnNextLevelTriggered;
        }

        /// <summary>
        /// 运行一局完整的游戏：绘制初始画面 → 倒计时 → 隐藏覆盖层 → 进入游戏循环 → 游戏结束后显示 GameOver
        /// </summary>
        private async Task RunGame()
        {
            Draw();                     // 先绘制一次当前状态
            await ShowCountDown();      // 显示倒计时 3 2 1 Go
            Overlay.Visibility = Visibility.Hidden; // 隐藏提示覆盖层
            await GameLoop();           // 进入主游戏循环，直到游戏结束

            // 如果是因为升级关卡而退出循环，则不显示 GameOver 画面，并且不重置 gameState
            if (!levelUpInProgress)
            {
                await ShowGameOver();                // 显示游戏结束动画和文字
                gameState = new GameState(rows, cols); // 重置游戏状态（但保持网格尺寸）
            }
            levelUpInProgress = false; // 重置升级标志
        }

        /// <summary>
        /// 预览按键事件：处理Shift键忽略、覆盖层可见时阻止穿透、以及启动游戏。
        /// </summary>
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

        /// <summary>
        /// 按键按下事件：仅在游戏进行中且未结束时，根据WASD修改蛇的方向。
        /// </summary>
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

        /// <summary>
        /// 主游戏循环：每隔100毫秒移动一次蛇，并重绘画面，直到游戏结束或外部中断。
        /// </summary>
        private async Task GameLoop()
        {
            while (!gameState.GameOver && gameRuning)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        /// <summary>
        /// 初始化显示网格：创建 rows×cols 个 Image 控件，添加到 UniformGrid 中，并返回二维数组以便快速访问。
        /// </summary>
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            // 设置 UniformGrid 的行列数
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            // 根据行列比例调整 UniformGrid 的宽高，保证每个格子为正方形
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);
            GameGrid.Height = GameGrid.Width * (rows / (double)cols);


            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,                // 初始为空图片
                        RenderTransformOrigin = new Point(0.5, 0.5) // 旋转中心设为图片中心
                    };

                    images[r, c] = image;          // 保存引用
                    GameGrid.Children.Add(image);  // 添加到界面布局中
                }
            }
            return images;
        }

        /// <summary>
        /// 整体绘制
        /// </summary>
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        /// <summary>
        /// 绘制整个网格：根据 gameState 中每个格子的类型，设置对应 Image 的 Source，并重置旋转变换。
        /// </summary>
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    //从游戏逻辑模型（gameState）中获取第r行第c列是什么东西
                    GridValue gridValue = gameState.Grid[r, c];
                    //根据这个枚举值，从字典 gridValToImages 中找到对应的图片
                    gridImages[r, c].Source = gridValToImages[gridValue];
                    //重置该格子的旋转变换（防止别的格子像蛇头一样歪掉，因为蛇头会旋转）
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        /// <summary>
        /// 绘制蛇头：从 gameState 获取蛇头位置，设置头部图片，并根据当前方向旋转对应角度。
        /// </summary>
        private void DrawSnakeHead()
        {
            Position headpos = gameState.HeadPosition();// 蛇头坐标
            Image image = gridImages[headpos.Row, headpos.Col];// 对应的Image控件
            image.Source = Images.Head; // 设为头部图片

            int rotation = diroRotation[gameState.Dir]; // 获取方向对应的旋转角度
            image.RenderTransform = new RotateTransform(rotation); // 应用旋转变换
        }

        /// <summary>
        /// 模拟按下并释放左Shift键。用于绕过WPF某些输入法导致的方向键失灵问题。
        /// </summary>
        private void SimulateShiftPress()
        {
            // 按下左 Shift
            keybd_event(VK_LSHIFT, 0, 0, UIntPtr.Zero);
            // 释放左 Shift
            keybd_event(VK_LSHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        /// <summary>
        /// 游戏结束时显示蛇的死亡动画：依次将蛇身每个格子替换为“死亡”样式的图片，间隔50ms。
        /// </summary>
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

        /// <summary>
        /// 显示开始倒计时
        /// </summary>
        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverLayText.Text = i.ToString();
                await Task.Delay(500);
            }
            OverLayText.Text = "Go!";
            await Task.Delay(500);
        }

        /// <summary>
        /// 显示游戏结束画面
        /// </summary>
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);

            Overlay.Visibility = Visibility.Visible;
            OverLayText.Text = "按空格键重新开始";
        }

        private async void GameState_OnNextLevelTriggered()
        {

        }
    }
}