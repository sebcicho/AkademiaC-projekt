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
    class Bunker : Obstackle
    {
        private MainWindow main;


        public Bunker(MainWindow win)
        {
            main = win;
        }

        public void Draw()
        {
            Canvas bunkerCanvas = new Canvas();
            bunkerCanvas.Height = 60;
            bunkerCanvas.Width = 60;

            Rectangle mount = new Rectangle();
            mount.Height = 55;
            mount.Width = 55;
            mount.Fill = new SolidColorBrush(Colors.Gray);

            Rectangle rest = new Rectangle();
            rest.Height = 35;
            rest.Width = 35;
            rest.Fill = new SolidColorBrush(Colors.Ivory);

            bunkerCanvas.Children.Add(mount);
            Canvas.SetTop(mount, 0);
            Canvas.SetLeft(mount, 0);
            bunkerCanvas.Children.Add(rest);
            Canvas.SetTop(rest, 10);
            Canvas.SetLeft(rest, 10);

            main.obstacleCanvas.Children.Add(bunkerCanvas);
            Canvas.SetTop(bunkerCanvas, YPosition);
            Canvas.SetLeft(bunkerCanvas, XPosition+3);
            // GetChildren(main.myGrid,YPosition,XPosition).

        }

    }
}