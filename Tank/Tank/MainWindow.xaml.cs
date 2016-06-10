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


        PlayerTank playerTank = new PlayerTank(false) { XPosition = 270, XShootPosition = 270, YShootPosition = 60, ShootVisible = 0 };
        Params gameParams = new Params() { level = 1, points = 0, health = 100, reload = 0 };

        private int tankSpeed = 10;
        private int ticPrescaler = 0;
        private int ticMultiplier = 2;
        private obstacleTypeAndIndicator obs;

        private List<List<List<obstacleTypeAndIndicator>>> obstacles = new List<List<List<obstacleTypeAndIndicator>>>();
        private List<List<List<Trash>>> trashes = new List<List<List<Trash>>>();
        private List<List<List<EnemyTank>>> tanks = new List<List<List<EnemyTank>>>();
        private List<List<List<Bunker>>> bunkers = new List<List<List<Bunker>>>();
        private List<List<List<Tower>>> towers = new List<List<List<Tower>>>();

        public static int ticPrescalerMax = 1;
        public static int barsLength = 150; //health and reloda bar length in pixels
        public static int cannonOffset = 28; //offset of cannon position on canvas in pixels 
        public static int playerShotRange = 500;
        public static int playerTankEndPosition = 60;
        public static int ammoVelocity = 10; //added with every tick to y shoot position
        public static int gridHeight = 3;
        public static int gridWidth = 9;
        public static int levelNumber = 5;


        private enum obstaclesTypes
        {
            noObstacle,
            trash,
            tank,
            bunker,
            tower
        }

        private enum obstackleHealthPoints
        {
            noObstackle,
            trash,
            tankAndTower,
            bunker
        }
        public MainWindow()
        {
            InitializeComponent();
            LevelInit(levelNumber);

            LevelDraw(3);
            //------------------------------------------------------
            PlayerTankObject.DataContext = playerTank;
            Level.DataContext = gameParams;
            Points.DataContext = gameParams;
            ReloadRectangle.DataContext = gameParams;
            HealthRectangle.DataContext = gameParams;
            PlayerAmmo.DataContext = playerTank;
            //------------------------------------------------------

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            timer.Tick += timerTick;

        }
        void timerTick(object sender, EventArgs e)
        {

            ticPrescaler++;
            if (ticPrescaler > ticPrescalerMax)
            {
                ticPrescaler = 0;

                if (gameParams.reload < barsLength)
                {
                    if (gameParams.reload <= barsLength - ticMultiplier)
                        gameParams.reload += ticMultiplier;
                    else
                        gameParams.reload = barsLength;
                }
            }

            if (playerTank.shootFired == true)
            {

                if (playerTank.YShootPosition == playerTankEndPosition)
                {
                    playerTank.XShootPosition = playerTank.XPosition + cannonOffset;
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
        private void LevelInit(int levels)
        {
            int checkSum = 0;
            Random rnd = new Random();
            for (int i = 0; i < levels; i++)
            {

                trashes.Add(new List<List<Trash>>());
                tanks.Add(new List<List<EnemyTank>>());
                obstacles.Add(new List<List<obstacleTypeAndIndicator>>());
                bunkers.Add(new List<List<Bunker>>());
                towers.Add(new List<List<Tower>>());
                checkSum = 0;

                for (int j = 0; j < gridWidth; j++)
                {
                    trashes[i].Add(new List<Trash>());
                    tanks[i].Add(new List<EnemyTank>());
                    obstacles[i].Add(new List<obstacleTypeAndIndicator>());
                    bunkers[i].Add(new List<Bunker>());
                    towers[i].Add(new List<Tower>());

                    for (int k = 0; k < gridHeight; k++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(rnd.Next(0, 2), 0));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1].type != 0)
                                    {
                                        checkSum++;
                                        obstacles[i][j][obstacles[i][j].Count - 1].sum = checkSum;
                                    }
                                }
                                else
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(0, 0));
                                break;
                            case 1:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(rnd.Next(0, 3), 0));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1].type != 0)
                                    {
                                        checkSum++;
                                        obstacles[i][j][obstacles[i][j].Count - 1].sum = checkSum;
                                    }
                                }
                                else
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(0, 0));
                                break;
                            case 2:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(rnd.Next(0, 4), 0));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1].type != 0)
                                    {
                                        checkSum++;
                                        obstacles[i][j][obstacles[i][j].Count - 1].sum = checkSum;
                                    }
                                }
                                else
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(0, 0));
                                break;
                            case 3:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(rnd.Next(0, 5), 0));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1].type != 0)
                                    {
                                        checkSum++;
                                        obstacles[i][j][obstacles[i][j].Count - 1].sum = checkSum;
                                    }
                                }
                                else
                                    obstacles[i][j].Add(new obstacleTypeAndIndicator(0, 0));
                                break;
                            default:
                                obstacles[i][j].Add(new obstacleTypeAndIndicator(rnd.Next(0, 5), 0));
                                if (obstacles[i][j][obstacles[i][j].Count - 1].type != 0)
                                {
                                    checkSum++;
                                    obstacles[i][j][obstacles[i][j].Count - 1].sum = checkSum;
                                }
                                break;
                        }
                    }
                    if (checkSum == 0)
                        obstacles[i][j][0].type = 1;
                }
            }
        }
        private void LevelDraw(int level)
        {

            for (int j = 0; j < obstacles[level].Count; j++)
            {
                for (int k = 0; k < obstacles[level][j].Count; k++)
                {


                    switch (obstacles[level][j][k].type)
                    {
                        case (int)obstaclesTypes.noObstacle:
                            //do nothing
                            break;
                        case (int)obstaclesTypes.trash:
                            //draw a trash
                            trashes[level][j].Add(new Trash(this, (int)obstackleHealthPoints.trash));
                            trashes[level][j][trashes[level][j].Count - 1].XPosition = (j * 60) + 10;
                            trashes[level][j][trashes[level][j].Count - 1].YPosition = (k * 60) + 10;
                            trashes[level][j][trashes[level][j].Count - 1].Draw();
                            break;
                        case (int)obstaclesTypes.tank:
                            //draw a tank
                            tanks[level][j].Add(new EnemyTank(this, (int)obstackleHealthPoints.tankAndTower));
                            tanks[level][j][tanks[level][j].Count - 1].XPosition = (j * 60) + 10;
                            tanks[level][j][tanks[level][j].Count - 1].YPosition = (k * 60) + 10;
                            tanks[level][j][tanks[level][j].Count - 1].Draw();
                            break;
                        case (int)obstaclesTypes.bunker:
                            //draw a bunker
                            bunkers[level][j].Add(new Bunker(this,  (int)obstackleHealthPoints.bunker));
                            bunkers[level][j][bunkers[level][j].Count - 1].XPosition = (j * 60) + 10;
                            bunkers[level][j][bunkers[level][j].Count - 1].YPosition = (k * 60) + 10;
                            bunkers[level][j][bunkers[level][j].Count - 1].Draw();
                            break;
                        case (int)obstaclesTypes.tower:
                            //draw a attackBuilding
                            towers[level][j].Add(new Tower(this, (int)obstackleHealthPoints.tankAndTower));
                            towers[level][j][towers[level][j].Count - 1].XPosition = (j * 60) + 10;
                            towers[level][j][towers[level][j].Count - 1].YPosition = (k * 60) + 10;
                            towers[level][j][towers[level][j].Count - 1].Draw();
                            break;

                    }
                }
            }

        }


    }
}
