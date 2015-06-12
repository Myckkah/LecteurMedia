using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using BO;
using System.ComponentModel;
using System.Configuration;


namespace IHM
{
    public class MainViewModel
    {
        private BindingList<Video> listeVideo;

        public BindingList<Video> ListeVideo
        {
            get { return listeVideo; }
            set { listeVideo = value; }
        }

        public MainViewModel()
        {
            MgtVideo.getInstance().TrouverFilm(ConfigurationManager.AppSettings["Dossier"]);
            ListeVideo = MgtVideo.getInstance().ListeVideo;
        }
    }
}
