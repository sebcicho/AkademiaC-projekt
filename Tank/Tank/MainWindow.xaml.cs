using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.ComponentModel;

namespace Tank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        PlayerTank playerTank = new PlayerTank(false) { XPosition = 270, XShootPosition = 270, YShootPosition =60, ShootVisible=0};
        Params gameParams = new Params() { level = 1, points = 0, health=100, reload=0};

        private int tankSpeed = 10;
        public int ticPrescaler =0;
        public int ticMultiplier = 2;

        public static int ticPrescalerMax = 1;
        static public int barsLength = 150; //health and reloda bar length in pixels
        static public int cannonOffset = 28; //offset of cannon position on canvas in pixels 
        static public int playerShotRange = 500; 
        static public int playerTankEndPosition = 60; 
        static public int ammoVelocity = 10; //added with every tick to y shoot position

        public MainWindow()
        {
            InitializeComponent();

            PlayerTankObject.DataContext = playerTank;
            Level.DataContext = gameParams;
            Points.DataContext = gameParams;
            ReloadRectangle.DataContext = gameParams;
            HealthRectangle.DataContext = gameParams;

            PlayerAmmo.DataContext = playerTank;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            timer.Tick += timerTick;

           
        }
        void timerTick(object sender, EventArgs e)
        {

            ticPrescaler++;
            if (ticPrescaler> ticPrescalerMax)
            {
                ticPrescaler = 0;
                
                if (gameParams.reload < barsLength)
                {
                    if (gameParams.reload <= barsLength - ticMultiplier)
                        gameParams.reload+= ticMultiplier;
                    else
                        gameParams.reload = barsLength;
                }
            }

            if (playerTank.shootFired == true)
            {

                if (playerTank.YShootPosition == playerTankEndPosition)
                {
                    playerTank.XShootPosition = playerTank.XPosition+cannonOffset;
                    playerTank.ShootVisible = 100;
                }
                playerTank.YShootPosition += ammoVelocity;
                if (playerTank.YShootPosition == playerShotRange)
                {
                    playerTank.shootFired = false;
                    playerTank.XShootPosition = 0;
                    playerTank.YShootPosition = playerTankEndPosition;
                    playerTank.ShootVisible = 0;
                }
            }
  
        }

        private void MainWindow_OnKeyDown(object sender, KeyboardEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.A))
                playerTank.XPosition -= tankSpeed;
            if (Keyboard.IsKeyDown(Key.D))
                playerTank.XPosition += tankSpeed;
            if (Keyboard.IsKeyDown(Key.W))
                gameParams.level++;
            if (Keyboard.IsKeyDown(Key.Space))
            {
                if (gameParams.reload >= barsLength && playerTank.shootFired == false)
                {
                    gameParams.reload = 0;
                    playerTank.shootFired = true;
                }
            }
        }

    }
}
