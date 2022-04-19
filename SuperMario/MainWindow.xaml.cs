using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;

namespace SuperMario
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Context ctx;
        Canvas? canvas;

        public MainWindow()
        {
            InitializeComponent();
            this.InitializeCanvas();
            this.InitializeContext();

            this.Width = LevelUtils.CANVAS_WIDTH + 60;
            this.Height = LevelUtils.CANVAS_HEIGHT + 60;

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            new Thread(ctx!.Tick).Start();

            EventManager.RegisterClassHandler(typeof(Window),
                 Keyboard.KeyUpEvent, new KeyEventHandler(HandleKeyUp), true);

            EventManager.RegisterClassHandler(typeof(Window),
                 Keyboard.KeyDownEvent, new KeyEventHandler(HandleKeyDown), true);
        }
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            this.ctx.SetKeyUp(e.Key);
        }
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            this.ctx.SetKeyDown(e.Key);
        }

        private void InitializeCanvas()
        {
            var canvas = new Canvas();
            canvas.Background = Brushes.Blue;
            canvas.Width = LevelUtils.CANVAS_WIDTH;
            canvas.Height = LevelUtils.CANVAS_HEIGHT;
            canvas.ClipToBounds = true;

            this.Content = canvas;
            this.canvas = canvas;
        }

        private void InitializeContext()
        {
            var level = LevelUtils.LevelFromTxt("levels/01.level");
            var ctx = new Context(level.statics, level.enemies, level.mario, canvas!);
            this.ctx = ctx;
        }
    }
}
