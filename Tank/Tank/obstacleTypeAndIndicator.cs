using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tank
{
    class obstacleTypeAndIndicator
    {
        public int type;
        public int y;
        public int x;
        public obstacleTypeAndIndicator(int a1, int a2, int a3)
        {
            type = a1;
            x = a2;
            y = a3;
        }

        public obstacleTypeAndIndicator(int a1)
        {
            type = a1;
        }
    };
}
