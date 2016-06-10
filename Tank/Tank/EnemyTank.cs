using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
namespace Tank
{
    class EnemyTank : Obstackle
    {
        private MainWindow main;


        public EnemyTank(MainWindow win, int hp)
        {
            main = win;
            health = hp;

        }
        public EnemyTank(MainWindow win, int hp, int x, int y)
        {
            main = win;
            health = hp;
            xGridPosition = x;
            yGridPosition = y;
        }
        public void Draw()
        {
            Canvas tankCanvas = new Canvas();
            tankCanvas.Height = 60;
            tankCanvas.Width = 60;

            Rectangle hull = new Rectangle();
            hull.Height = 40;
            hull.Width = 30;
            hull.Fill = new SolidColorBrush(Colors.GreenYellow);

            Rectangle turret = new Rectangle();
            turret.Height = 25;
            turret.Width = 20;
            turret.Fill = new SolidColorBrush(Colors.Green);

            Rectangle gun = new Rectangle();
            gun.Height = 25;
            gun.Width = 4;
            gun.Fill = new SolidColorBrush(Colors.Green);

            Rectangle trackLeft = new Rectangle();
            trackLeft.Height = 40;
            trackLeft.Width = 4;
            trackLeft.Fill = new SolidColorBrush(Colors.Chocolate);

            Rectangle trackRight = new Rectangle();
            trackRight.Height = 40;
            trackRight.Width = 4;
            trackRight.Fill = new SolidColorBrush(Colors.Chocolate);

            tankCanvas.Children.Add(hull);
            Canvas.SetTop(hull, 0);
            Canvas.SetLeft(hull, 15);
            tankCanvas.Children.Add(turret);
            Canvas.SetTop(turret, 5);
            Canvas.SetLeft(turret, 20);
            tankCanvas.Children.Add(gun);
            Canvas.SetTop(gun, 30);
            Canvas.SetLeft(gun, 28);
            tankCanvas.Children.Add(trackLeft);
            Canvas.SetTop(trackLeft, 0);
            Canvas.SetLeft(trackLeft, 8);
            tankCanvas.Children.Add(trackRight);
            Canvas.SetTop(trackRight, 0);
            Canvas.SetLeft(trackRight, 48);


            main.obstacleCanvas.Children.Add(tankCanvas);
            Canvas.SetTop(tankCanvas, YPosition);
            Canvas.SetLeft(tankCanvas, XPosition);

            // GetChildren(main.myGrid,YPosition,XPosition).

        }


    }
}
