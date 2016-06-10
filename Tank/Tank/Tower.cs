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
    class Tower : Obstackle
    {
        private MainWindow main;


        public Tower(MainWindow win)
        {
            main = win;
        }

        public void Draw()
        {
            Canvas towerCanvas = new Canvas();
            towerCanvas.Height = 60;
            towerCanvas.Width = 60;

            Rectangle mount = new Rectangle();
            mount.Height = 55;
            mount.Width = 55;
            mount.Fill = new SolidColorBrush(Colors.Gray);

            Rectangle rest = new Rectangle();
            rest.Height = 45;
            rest.Width = 45;
            rest.Fill = new SolidColorBrush(Colors.Ivory);

            Rectangle turret = new Rectangle();
            turret.Height = 20;
            turret.Width = 35;
            turret.Fill = new SolidColorBrush(Colors.Violet);

            Rectangle gun1 = new Rectangle();
            gun1.Height = 20;
            gun1.Width = 5;
            gun1.Fill = new SolidColorBrush(Colors.Violet);

            Rectangle gun2 = new Rectangle();
            gun2.Height = 20;
            gun2.Width = 5;
            gun2.Fill = new SolidColorBrush(Colors.Violet);



            towerCanvas.Children.Add(mount);
            Canvas.SetTop(mount, 0);
            Canvas.SetLeft(mount, 0);
            towerCanvas.Children.Add(rest);
            Canvas.SetTop(rest, 5);
            Canvas.SetLeft(rest, 5);
            towerCanvas.Children.Add(turret);
            Canvas.SetTop(turret, 10);
            Canvas.SetLeft(turret, 10);
            towerCanvas.Children.Add(gun1);
            Canvas.SetTop(gun1, 30);
            Canvas.SetLeft(gun1, 15);
            towerCanvas.Children.Add(gun2);
            Canvas.SetTop(gun2, 30);
            Canvas.SetLeft(gun2, 35);

            main.obstacleCanvas.Children.Add(towerCanvas);
            Canvas.SetTop(towerCanvas, YPosition);
            Canvas.SetLeft(towerCanvas, XPosition + 3);


        }

    }
}