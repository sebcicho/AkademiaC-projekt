using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Tank 
{
    class PlayerTank : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int xPosition;
        private int xShootPosition;
        private int yShootPosition;
        public bool shootFired;
        private int shootVisible;
        public PlayerTank(bool shoot )
        {
            shootFired = shoot;
        }

        //private int Position;
       
        public int XPosition
        {
            get { return xPosition; }
            set
            {
                xPosition = value;
                OnPropertyChanged("XPosition");
            }
        }

        public int XShootPosition
        {
            get { return xShootPosition; }
            set
            {
                xShootPosition = value;
                OnPropertyChanged("XShootPosition");
            }
        }

        public int ShootVisible
        {
            get { return shootVisible; }
            set
            {
                shootVisible = value;
                OnPropertyChanged("ShootVisible");
            }
        }

        public int YShootPosition
        {
            get { return yShootPosition; }
            set
            {
                yShootPosition = value;
                OnPropertyChanged("YShootPosition");
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        

    }
}
