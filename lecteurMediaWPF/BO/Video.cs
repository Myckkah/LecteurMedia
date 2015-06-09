using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Video
    {
        #region ATTRIBUTS

        private string nom;
        private int duree;

        #endregion

        #region PROPRIETES

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        public int Duree 
        {
            get { return duree; }
            set { duree = value; }
        }
        #endregion
    }
}
