using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEST2
{
    class Program
    {
        static void Main(string[] args)
        {
            MgtVideo mgt = MgtVideo.getInstance();
            mgt.TrouverFilm(@"\\10.35.41.14\Partage\Videos\");
        }
    }
}
