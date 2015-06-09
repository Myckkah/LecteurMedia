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
        private string chemin;
        private string extension;

        #endregion

        #region PROPRIETES

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        public string Chemin 
        {
            get { return chemin; }
            set { chemin = value; }
        }

        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        #endregion

        public Video() 
        { 
        }

        public Video(string nom, string chemin, string extension)
        {
            this.Nom = nom;
            this.Chemin = chemin;
            this.Extension = extension;
        }
    }
}
