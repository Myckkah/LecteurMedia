using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace BO
{
    public class Video
    {
        #region ATTRIBUTS

        private string nom;
        private string chemin;
        private string extension;
        private Bitmap image;
        private BitmapSource imageSource;

        #endregion

        #region PROPRIETES

        public string Nom
        {
            get { return Outils.Outil.replaceCaractere(nom); }
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

        public Bitmap ImageVideo
        {
            get 
            {
                return image; 
            }
            set { image = value; }
        }

        public BitmapSource ImageVideoSource
        {
            get
            {
                imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(image.Width, ImageVideo.Height));
                
                return imageSource;
            }
            set { imageSource = value; }
        }

        public String Color
        {
            get
            {
                int sum = 0;
                foreach (char caractere in Nom)
                {
                    sum += (int)caractere;
                }
                return "#" + sum.ToString("X").Substring(0, 3);
            }
        }

        #endregion

        public Video() 
        { 
        }

        public Video(string nom, string chemin, string extension, Bitmap image)
        {
            this.Nom = nom;
            this.Chemin = chemin;
            this.Extension = extension;
            this.ImageVideo = image;
        }

    }
}
