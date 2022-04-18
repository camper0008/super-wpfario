using System.Windows;
using System;
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

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            new Thread(ctx!.Tick).Start();

            EventManager.RegisterClassHandler(typeof(Window),
                 Keyboard.KeyUpEvent, new KeyEventHandler(HandleKeyUp), true);

            EventManager.RegisterClassHandler(typeof(Window),
                 Keyboard.KeyDownEvent, new KeyEventHandler(HandleKeyDown), true);
        }
        private Sprite[] LevelStaticSprites()
        {
            int blocks = 10;
            var sprites = new Sprite[blocks];
            for (int i = 0; i < blocks; i++)
            {
                var groundImage = Utils.ImageFromPath(@"sprites/ground.png");
                var ground = new Sprite(new Vector2(120 * i, Convert.ToInt32(900 - 120)), new Vector2(120, 120), groundImage, null);
                sprites[i] = ground;
            }
            return sprites;
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
            canvas.Width = 1400;
            canvas.Height = 900;

            this.Content = canvas;
            this.canvas = canvas;
        }

        private void InitializeContext()
        {
            var player = new Mario(new Vector2(0, 0), new Vector2(120, 120));
            var ctx = new Context(LevelStaticSprites(), new DynamicSprite[0], player, canvas!);
            this.ctx = ctx;
        }




    }
}
