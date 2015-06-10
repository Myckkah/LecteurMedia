using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using BO;
using System.ComponentModel;


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
            MgtVideo.getInstance().TrouverFilm(@"\\10.35.41.14\Partage\Videos\");
            ListeVideo = MgtVideo.getInstance().ListeVideo;
        }
    }
}
