using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.ComponentModel;
using System.IO;

namespace BLL
{
    public class MgtVideo
    {
        #region ATTRIBUTS

        private BindingList<Video> listeVideo;
        static private MgtVideo instance;

        #endregion

        #region PROPRIETES

        public BindingList<Video> ListeVideo 
        {
            get { return listeVideo; }
            set { listeVideo = value; }
        }

        #endregion

        #region SINGLETON

        static public MgtVideo getInstance()
        {
            if (instance == null)
                instance = new MgtVideo();

            return instance;
        }

        #endregion

        #region CONSTRUCTEUR
        private MgtVideo()
        {
            ListeVideo = new BindingList<Video>();
        }
        #endregion

        #region FONCTIONS

        public void AjouterVideo(string nom, string chemin, string extension)
        {
            Video video = new Video(nom, chemin, extension);
            ListeVideo.Add(video);
        }

        #region OBTENIR
        public Video ObtenirVideo(int position) 
        {
            if (position >= 0 && position < ListeVideo.Count())
            {
                return ListeVideo.ElementAt(position);
            }
            else
            {
                throw new ApplicationException("la position est incorrecte !");
            }
        }

        public Video ObtenirVideo(string nom)
        {
            Video retourVideo = null;
            if (string.IsNullOrEmpty(nom))
            {
                throw new ApplicationException("le nom est incorrect !");
            }
            foreach (Video item in ListeVideo)
            {
                if (item.Nom == nom)
                {
                    retourVideo = item;
                }
            }
            return retourVideo;
        }

        public void TrouverFilm(string path)
        {
           List<string> extensions = new List<string>(){".mp4", ".avi", ".mkv"};
           foreach (string item in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
           {
               if (extensions.Contains(Path.GetExtension(item)))
               {
                   AjouterVideo(Path.GetFileName(item).Substring(0, Path.GetFileName(item).LastIndexOf('.')), item,
                       Path.GetExtension(item));
               }
           }
        }
        #endregion

        #region SUPPRIMER

        public void SupprimerVideo(Video video)
        {
            if (ListeVideo.Contains(video))
            {
                ListeVideo.Remove(video);
            }
            else
            {
                throw new ApplicationException("La vidéo n'existe pas");
            }
        }

        public void SupprimerVideo(int position)
        {
            if (position >=0 && position < ListeVideo.Count())
            {
                ListeVideo.RemoveAt(position);
            }
            else
            {
                throw new ApplicationException("La vidéo n'existe pas");
            }
        }

        public void SupprimerVideo(string nom)
        {
            Video video = this.ObtenirVideo(nom);
            if (video != null)
            {
                this.SupprimerVideo(video);
            }
            else
            {
                throw new ApplicationException("La vidéo n'existe pas");
            }
        }

        #endregion

        #endregion
    }
}
