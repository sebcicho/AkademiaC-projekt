using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tank
{
    class Obstackle
    {
        private int xPosition;
        private int yPosition;

        public int xGridPosition;
        public int yGridPosition;
        public int health;
        public Obstackle()
        {
        }

        public int XPosition
        {
            get { return xPosition; }
            set { xPosition = value; }
        }

        public int YPosition
        {
            get { return yPosition; }
            set { yPosition = value; }
        }

    }
}
