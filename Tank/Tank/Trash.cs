using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
namespace Tank
{
    class Trash : Obstackle
    {
        private MainWindow main;

        
        public Trash(MainWindow win, int hp)
        {
            main = win;
            health = hp;
        }

        public Trash(MainWindow win, int hp,int x, int y)
        {
            main = win;
            health = hp;
            xGridPosition = x;
            yGridPosition = y;
        }
        public void Draw()
        {

            Rectangle rect = new Rectangle();
            rect.Height = 50;
            rect.Width = 50;    
            rect.Fill = new SolidColorBrush(Colors.Beige);
            main.obstacleCanvas.Children.Add(rect);
            Canvas.SetTop(rect,YPosition + 5);
            Canvas.SetLeft(rect, XPosition + 5);

        }

    }
}
