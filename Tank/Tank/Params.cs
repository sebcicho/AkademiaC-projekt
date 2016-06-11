using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Tank
{
    public class Params : INotifyPropertyChanged
    {
        private int gameLevel;
        private int gamePoints;
        private float gameHealth;
        private float reloadTime;
        public event PropertyChangedEventHandler PropertyChanged;
        public Params()
        { }

        public int level
        {
            get { return gameLevel; }
            set
            {
                gameLevel = value;
                OnPropertyChanged("level");
            }
        }

        public int points
        {
            get { return gamePoints; }
            set
            {
                gamePoints = value;
                OnPropertyChanged("points");
            }
        }

        public float health
        {
            get { return gameHealth; }
            set
            {
                gameHealth = value;
                OnPropertyChanged("health");
            }
        }

        public float reload
        {
            get { return reloadTime; }
            set
            {
                reloadTime = value;
                OnPropertyChanged("reload");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
