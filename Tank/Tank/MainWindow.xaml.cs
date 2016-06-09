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
        public int ticInterval = 2;


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
            if (ticPrescaler>1)
            {
                ticPrescaler = 0;
                
                if (gameParams.reload <150)
                {
                    if (gameParams.reload <= 150- ticInterval)
                        gameParams.reload+= ticInterval;
                    else
                        gameParams.reload = 150;
                }

                

            }
            if (playerTank.shootFired == true)
            {

                if (playerTank.YShootPosition == 60)
                {
                    playerTank.XShootPosition = playerTank.XPosition+28;
                    playerTank.ShootVisible = 100;
                }
                playerTank.YShootPosition += 10;
                if (playerTank.YShootPosition == 500)
                {
                    playerTank.shootFired = false;
                    playerTank.XShootPosition = 0;
                    playerTank.YShootPosition = 60;
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
                if (gameParams.reload >= 150 && playerTank.shootFired == false)
                {
                    gameParams.reload = 0;
                    playerTank.shootFired = true;
                }
            }


        }



    }
}
