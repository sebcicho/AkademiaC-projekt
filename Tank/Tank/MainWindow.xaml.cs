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
using System.Diagnostics;

namespace Tank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlayerTank playerTank = new PlayerTank(false) { XPosition = 270, XShootPosition = 270, YShootPosition = 60, ShootVisible = 0 };
        public Params gameParams = new Params() { level = 1, points = 0, health = 150, reload = 0 };

        private int tankSpeed = 10;
        private int ticPrescaler = 0;
        private int ticMultiplier = 2;
        private int bunkerHealingIterator = 0;
        private int towerSalvoIterator = 0;
        private int tankShootIterator = 0;
        private int enemyShootStateHolder = 1;
        private int enemyShootCount = 0;
        private int enemyShootX = 0;
        private int enemyShootY = 0;

        private int tanksAliveCounter = 0;
        private int bunkersAliveCounter = 0;
        private int towersAliveCounter = 0;
        private int trashesAliveCounter = 0;

        private bool defeat = false;

        private List<List<List<int>>> obstacles = new List<List<List<int>>>();
        private List<List<List<Trash>>> trashes = new List<List<List<Trash>>>();
        private List<List<List<EnemyTank>>> tanks = new List<List<List<EnemyTank>>>();
        private List<List<List<Bunker>>> bunkers = new List<List<List<Bunker>>>();
        private List<List<List<Tower>>> towers = new List<List<List<Tower>>>();
        private List<Rectangle> enemyShootsList = new List<Rectangle>();

        public static int ticPrescalerMax = 1;
        public static int barsLength = 150; //health and reloda bar length in pixels
        public static int cannonOffset = 28; //offset of cannon position on canvas in pixels 
        public static int playerShotRange = 500;
        public static int playerTankEndPosition = 60;
        public static int ammoVelocity = 10; //added with every tick to y shoot position
        public static int gridHeight = 3;
        public static int gridWidth = 9;
        public static int levelNumber = 10;
        public static int bunkerHealingTime = 100;
        public static int towerSalvoTime = 100;
        public static int tankShootTime = 100;
        public static int enemyTankDamage = 25;
        public static int enemyShootRange = 470;
        public static int obstaclesFirstHeighOffset = 455;
        public static int obstaclesWidthOffset = 10;

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

        private enum enemyTankShellState
        {
            noShoot,
            initState,
            shootState,
            hitState
        }
        public MainWindow()
        {
            InitializeComponent();
            LevelInit(levelNumber);


            LevelDraw(0);
            updateAlive();

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

            var timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(10);
            timer2.Start();
            timer2.Tick += timerTick2;



        }
        void timerTick2(object sender, EventArgs e)
        {
            if (tanksAliveCounter != 0)
            {
                tankShootIterator++;
                EnemyTank tempTank = new EnemyTank(this, 0);

                if (tankShootIterator == tankShootTime)
                {
                    tankShootIterator = 0;
                    tempTank = enemyTanksToShoot();
                    // Debug.WriteLine("x = " + temp.xGridPosition.ToString() + " y = " + temp.yGridPosition.ToString());
                    //Debug.WriteLine("------------------------------------------");

                    if (enemyShootStateHolder == (int)enemyTankShellState.initState)
                    {
                        enemyShootsList.Add(new Rectangle());

                        enemyShootsList[enemyShootCount].Height = 5;
                        enemyShootsList[enemyShootCount].Width = 4;
                        enemyShootsList[enemyShootCount].Fill = new SolidColorBrush(Colors.Red);
                        enemyShootStateHolder = (int)enemyTankShellState.shootState;
                        enemyShootX = tempTank.xGridPosition * 60 + 38;
                        enemyShootY = tempTank.yGridPosition * 60 + 60;
                        this.obstacleCanvas.Children.Add(enemyShootsList[enemyShootCount]);
                        Canvas.SetTop(enemyShootsList[enemyShootCount], enemyShootY);
                        Canvas.SetLeft(enemyShootsList[enemyShootCount], enemyShootX);

                        enemyShootCount++;
                    }
                }

                if (enemyShootStateHolder == (int)enemyTankShellState.shootState)
                {
                    enemyShootY += ammoVelocity;
                    this.obstacleCanvas.Children.Remove(enemyShootsList[enemyShootCount - 1]);
                    this.obstacleCanvas.Children.Add(enemyShootsList[enemyShootCount - 1]);
                    Canvas.SetTop(enemyShootsList[enemyShootCount - 1], enemyShootY);
                    Canvas.SetLeft(enemyShootsList[enemyShootCount - 1], enemyShootX);
                    if (enemyShootY == enemyShootRange)
                    {
                        enemyShootStateHolder = (int)enemyTankShellState.hitState;
                    }
                }
                if (enemyShootStateHolder == (int)enemyTankShellState.hitState)
                {
                    if (isHerePlayerTank(enemyShootX))
                    {
                        if (gameParams.health - enemyTankDamage > 0)
                            gameParams.health -= enemyTankDamage;
                        else
                        {
                            gameParams.health = 0;
                            if (defeat == false)
                            {
                                defeat = true;
                                Defeat wind = new Defeat(this);
                                this.IsEnabled = false;
                                wind.Show();
                            }
                        }
                    }
                    enemyShootStateHolder = (int)enemyTankShellState.initState;
                    this.obstacleCanvas.Children.Remove(enemyShootsList[enemyShootCount - 1]);
                }
            }




        }
        void timerTick(object sender, EventArgs e)
        {
            bunkerHealingIterator++;
            towerSalvoIterator++;
            ticPrescaler++;

            Tower tempTower = new Tower(this, 0);
            Bunker tempBunker = new Bunker(this, 0);
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
            if (bunkersAliveCounter != 0)
            {
                if (bunkerHealingIterator == bunkerHealingTime)
                {
                    bunkerHealingIterator = 0;
                    tempBunker = enemyBunkerToHeal();
                    bunkerHealing(tempBunker);
                }
            }
            if (towersAliveCounter != 0)
            {
                if (towerSalvoIterator == towerSalvoTime)
                {
                    towerSalvoIterator = 0;
                    tempTower = enemyTowerToShoot();
                    towerSalvo(tempTower);
                }
            }

            playerShoot();
        }

        private void MainWindow_OnKeyDown(object sender, KeyboardEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.A))
                playerTank.XPosition -= tankSpeed;
            if (Keyboard.IsKeyDown(Key.D))
                playerTank.XPosition += tankSpeed;
            if (Keyboard.IsKeyDown(Key.W))
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
                obstacles.Add(new List<List<int>>());
                bunkers.Add(new List<List<Bunker>>());
                towers.Add(new List<List<Tower>>());
                checkSum = 0;

                for (int j = 0; j < gridWidth; j++)
                {
                    trashes[i].Add(new List<Trash>());
                    tanks[i].Add(new List<EnemyTank>());
                    obstacles[i].Add(new List<int>());
                    bunkers[i].Add(new List<Bunker>());
                    towers[i].Add(new List<Tower>());

                    for (int k = 0; k < gridHeight; k++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(rnd.Next(0, 2));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1] != (int)obstaclesTypes.noObstacle)
                                        checkSum++;
                                }
                                else
                                    obstacles[i][j].Add(0);
                                break;
                            case 1:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(rnd.Next(0, 3));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1] != (int)obstaclesTypes.noObstacle)
                                        checkSum++;
                                }
                                else
                                    obstacles[i][j].Add(0);
                                break;
                            case 2:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(rnd.Next(0, 4));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1] != (int)obstaclesTypes.noObstacle)
                                        checkSum++;
                                }
                                else
                                    obstacles[i][j].Add(0);
                                break;
                            case 3:
                                if (j % 2 == 0)
                                {
                                    obstacles[i][j].Add(rnd.Next(0, 5));
                                    if (obstacles[i][j][obstacles[i][j].Count - 1] != (int)obstaclesTypes.noObstacle)
                                        checkSum++;
                                }
                                else
                                    obstacles[i][j].Add(0);
                                break;
                            default:
                                obstacles[i][j].Add(rnd.Next(0, 5));
                                if (obstacles[i][j][obstacles[i][j].Count - 1] != (int)obstaclesTypes.noObstacle)
                                    checkSum++;
                                break;
                        }
                    }
                    if (checkSum == 0)
                        obstacles[i][j][0] = 1;
                }
            }
        }
        private void LevelDraw(int level)
        {
            Rectangle blank = new Rectangle();
            blank.Height = 200;
            blank.Width = 600;
            blank.Fill = new SolidColorBrush(Colors.Black);

            this.obstacleCanvas.Children.Add(blank);
            Canvas.SetTop(blank, 0);
            Canvas.SetLeft(blank, 0);
            for (int j = 0; j < obstacles[level].Count; j++)
            {
                for (int k = 0; k < obstacles[level][j].Count; k++)
                {
                    switch (obstacles[level][j][k])
                    {
                        case (int)obstaclesTypes.noObstacle:
                            //do nothing
                            tanks[level][j].Add(new EnemyTank(this, 0, j, k));
                            bunkers[level][j].Add(new Bunker(this, 0, j, k));
                            towers[level][j].Add(new Tower(this, 0, j, k));
                            trashes[level][j].Add(new Trash(this, 0, j, k));
                            break;
                        case (int)obstaclesTypes.trash:
                            //draw a trash
                            trashes[level][j].Add(new Trash(this, (int)obstackleHealthPoints.trash, j, k));
                            trashes[level][j][trashes[level][j].Count - 1].XPosition = (j * 60 + 10);
                            trashes[level][j][trashes[level][j].Count - 1].YPosition = (k * 60 + 10);
                            trashes[level][j][trashes[level][j].Count - 1].Draw();

                            tanks[level][j].Add(new EnemyTank(this, 0, j, k));
                            bunkers[level][j].Add(new Bunker(this, 0, j, k));
                            towers[level][j].Add(new Tower(this, 0, j, k));
                            break;
                        case (int)obstaclesTypes.tank:
                            //draw a tank
                            tanks[level][j].Add(new EnemyTank(this, (int)obstackleHealthPoints.tankAndTower, j, k));
                            tanks[level][j][tanks[level][j].Count - 1].XPosition = (j * 60 + 10);
                            tanks[level][j][tanks[level][j].Count - 1].YPosition = (k * 60 + 10);
                            tanks[level][j][tanks[level][j].Count - 1].Draw();

                            trashes[level][j].Add(new Trash(this, 0, j, k));
                            bunkers[level][j].Add(new Bunker(this, 0, j, k));
                            towers[level][j].Add(new Tower(this, 0, j, k));
                            break;
                        case (int)obstaclesTypes.bunker:
                            //draw a bunker
                            bunkers[level][j].Add(new Bunker(this, (int)obstackleHealthPoints.bunker, j, k));
                            bunkers[level][j][bunkers[level][j].Count - 1].XPosition = (j * 60 + 10);
                            bunkers[level][j][bunkers[level][j].Count - 1].YPosition = (k * 60 + 10);
                            bunkers[level][j][bunkers[level][j].Count - 1].Draw();

                            trashes[level][j].Add(new Trash(this, 0, j, k));
                            tanks[level][j].Add(new EnemyTank(this, 0, j, k));
                            towers[level][j].Add(new Tower(this, 0, j, k));
                            break;
                        case (int)obstaclesTypes.tower:
                            //draw a attackBuilding
                            towers[level][j].Add(new Tower(this, (int)obstackleHealthPoints.tankAndTower, j, k));
                            towers[level][j][towers[level][j].Count - 1].XPosition = (j * 60 + 10);
                            towers[level][j][towers[level][j].Count - 1].YPosition = (k * 60 + 10);
                            towers[level][j][towers[level][j].Count - 1].Draw();

                            trashes[level][j].Add(new Trash(this, 0, j, k));
                            tanks[level][j].Add(new EnemyTank(this, 0, j, k));
                            bunkers[level][j].Add(new Bunker(this, 0, j, k));
                            break;
                    }
                }
            }

        }

        private void playerShoot()
        {
            int yBoom = 0;

            Rectangle blank = new Rectangle();
            blank.Height = 60;
            blank.Width = 60;
            blank.Fill = new SolidColorBrush(Colors.Black);

            if (playerTank.shootFired == true)
            {

                if (playerTank.YShootPosition == playerTankEndPosition)
                {
                    playerTank.XShootPosition = playerTank.XPosition + cannonOffset;
                    playerTank.ShootVisible = 100;
                }
                playerTank.YShootPosition += ammoVelocity;


                if (isHitOnX())
                {
                    yBoom = obstaclesFirstHeighOffset - (YGridPosition() * 60);
                    //     Debug.WriteLine((YGridPosition() * 60 - 50).ToString());
                    if (playerTank.YShootPosition >= yBoom)
                    {

                        switch (obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()])
                        {
                            case (int)obstaclesTypes.trash:
                                trashes[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health--;
                                if (trashes[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health == 0)
                                {
                                    this.obstacleCanvas.Children.Add(blank);
                                    Canvas.SetTop(blank, YGridPosition() * 60 + 9);
                                    Canvas.SetLeft(blank, mapXPositionToGrid(playerTank.XShootPosition) * 60 + 10);
                                    obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()] = 0;
                                    gameParams.points += (int)obstaclesTypes.trash;
                                    trashesAliveCounter--;
                                    if (checkWin())
                                        increaseLevel();
                                }
                                break;
                            case (int)obstaclesTypes.tank:
                                tanks[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health--;
                                if (tanks[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health == 0)
                                {
                                    this.obstacleCanvas.Children.Add(blank);
                                    Canvas.SetTop(blank, YGridPosition() * 60 + 9);
                                    Canvas.SetLeft(blank, mapXPositionToGrid(playerTank.XShootPosition) * 60 + 10);
                                    obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()] = 0;
                                    tanksAliveCounter--;
                                    gameParams.points += (int)obstaclesTypes.tank;
                                    if (checkWin())
                                        increaseLevel();
                                }
                                break;
                            case (int)obstaclesTypes.bunker:
                                bunkers[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health--;
                                if (bunkers[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health == 0)
                                {
                                    this.obstacleCanvas.Children.Add(blank);
                                    Canvas.SetTop(blank, YGridPosition() * 60 + 9);
                                    Canvas.SetLeft(blank, mapXPositionToGrid(playerTank.XShootPosition) * 60 + 10);
                                    obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()] = 0;
                                    bunkersAliveCounter--;
                                    gameParams.points += (int)obstaclesTypes.bunker;
                                    if (checkWin())
                                        increaseLevel();
                                }
                                break;
                            case (int)obstaclesTypes.tower:
                                towers[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health--;
                                if (towers[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()].health == 0)
                                {
                                    this.obstacleCanvas.Children.Add(blank);
                                    Canvas.SetTop(blank, YGridPosition() * 60 + 9);
                                    Canvas.SetLeft(blank, mapXPositionToGrid(playerTank.XShootPosition) * 60 + 10);
                                    obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][YGridPosition()] = 0;
                                    towersAliveCounter--;
                                    gameParams.points += (int)obstaclesTypes.tower;
                                    if (checkWin())
                                        increaseLevel();
                                }
                                break;
                        }
                        playerTank.shootFired = false;
                        playerTank.XShootPosition = 0;
                        playerTank.YShootPosition = playerTankEndPosition;
                        playerTank.ShootVisible = 0;
                    }
                }

                if (playerTank.YShootPosition == playerShotRange)
                {
                    playerTank.shootFired = false;
                    playerTank.XShootPosition = 0;
                    playerTank.YShootPosition = playerTankEndPosition;
                    playerTank.ShootVisible = 0;
                }
            }
        }


        private int mapXPositionToGrid(int xpos)
        {
            int xgrid = 0;
            for (int i = 0; i < gridWidth; i++)
            {
                if (xpos >= 0 + (i * 60 + 10) && xpos < 60 + (i * 60 + 10))
                {
                    xgrid = i;
                    break;
                }
                else xgrid = gridWidth;
            }
            return xgrid;
        }


        private bool isHitOnX()
        {

            for (int i = 0; i < gridHeight; i++)
            {
                if (mapXPositionToGrid(playerTank.XShootPosition) != gridWidth)
                {
                    if (obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][i] != 0)
                        return true;
                }
            }
            return false;
        }

        private int YGridPosition()
        {
            int intToReturn = 0;
            for (int i = 0; i < gridHeight; i++)
            {
                if (obstacles[gameParams.level - 1][mapXPositionToGrid(playerTank.XShootPosition)][i] != 0)
                    intToReturn = i;
            }
            return intToReturn;
        }

        private void bunkerHealing(Bunker bunker)
        {

            if (bunker.health != 0 && bunker.health != 3)
            {
                bunker.health++;
            }

        }

        private void towerSalvo(Tower tower)
        {
            if (tower.health != 0 && gameParams.health != 0)
            {
                gameParams.health = gameParams.health - 10;
                if (gameParams.health <= 0)
                {
                    gameParams.health = 0;
                    if (defeat == false)
                    {
                        Defeat wind = new Defeat(this);
                        defeat = true;
                        this.IsEnabled = false;
                        wind.Show();
                    }
                }
            }
        }

        EnemyTank enemyTanksToShoot()
        {

            Random rdn = new Random();

            int tankChosen = 0;
            int tankAliveIterator = 0;
            EnemyTank temp = new EnemyTank(this, 0, 20, 20);

            tankChosen = rdn.Next(0, tanksAliveCounter);
          //  Debug.WriteLine("tanksAlive: " + tanksAliveCounter.ToString());
            foreach (var subcollection in tanks[gameParams.level - 1])
            {
                foreach (var tank in subcollection)
                {
                    if (tankAliveIterator == tankChosen)
                    {
                        temp = tank;
                    }
                    if (tank.health != 0)
                        tankAliveIterator++;
                }

            }

            return temp;
        }

        Tower enemyTowerToShoot()
        {
            Random rdn = new Random();
            int towersChosen = 0;
            int towersAliveIterator = 0;
            Tower temp = new Tower(this, 0, 20, 20);


            towersChosen = rdn.Next(0, towersAliveCounter);

            foreach (var subcollection in towers[gameParams.level - 1])
            {
                foreach (var tower in subcollection)
                {
                    if (towersAliveIterator == towersChosen)
                    {
                        temp = tower;
                    }
                    if (tower.health != 0)
                        towersAliveIterator++;
                }
            }
            return temp;
        }

        Bunker enemyBunkerToHeal()
        {
            Random rdn = new Random();
            int bunkersChosen = 0;
            int bunkersAliveIterator = 0;
            Bunker temp = new Bunker(this, 0, 20, 20);


            bunkersChosen = rdn.Next(0, bunkersAliveCounter);

            foreach (var subcollection in bunkers[gameParams.level - 1])
            {
                foreach (var bunker in subcollection)
                {
                    if (bunkersAliveIterator == bunkersChosen)
                    {
                        temp = bunker;
                    }
                    if (bunker.health != 0)
                        bunkersAliveIterator++;
                }
            }
            return temp;
        }

        bool isHerePlayerTank(int xPos)
        {
            int freeSpaceOffset = 3;
            int canvasWidth = 60;
            if (playerTank.XPosition + freeSpaceOffset <= xPos && playerTank.XPosition + canvasWidth - freeSpaceOffset >= xPos)
            {
                return true;
            }
            else
                return false;
        }

        void updateAlive()
        {
            foreach (var subcollection in trashes[gameParams.level - 1])
            {
                foreach (var trash in subcollection)
                {
                    if (trash.health != 0)
                    {
                        trashesAliveCounter++;
                    }
                }
            }

            foreach (var subcollection in tanks[gameParams.level - 1])
            {
                foreach (var tank in subcollection)
                {
                    if (tank.health != 0)
                    {
                        tanksAliveCounter++;
                    }
                }
            }

            foreach (var subcollection in bunkers[gameParams.level - 1])
            {
                foreach (var bunker in subcollection)
                {
                    if (bunker.health != 0)
                        bunkersAliveCounter++;
                }
            }

            foreach (var subcollection in towers[gameParams.level - 1])
            {
                foreach (var tower in subcollection)
                {
                    if (tower.health != 0)
                        towersAliveCounter++;
                }
            }
        }

        void increaseLevel()
        {
            if (gameParams.level != levelNumber)
            {
                LevelDraw(gameParams.level);
                gameParams.level++;
                gameParams.health = 150;
                updateAlive();
            }
            else
            {
                WinWindow wWindow = new WinWindow(this);
                wWindow.Show();

            }
        }

        bool checkWin()
        {
            int sum = 0;
            sum = towersAliveCounter + tanksAliveCounter + trashesAliveCounter + bunkersAliveCounter;
            if (sum == 0)
                return true;
            else
                return false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Show();
        }
    }
}
