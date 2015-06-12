using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outils
{
    public static class Outil
    {
        public static string premiereLettreEnMajuscule(this string texte, char delimiteur)
        {
            Boolean maj = true;
            string nouveauTexte = "";
            for (int i = 0; i < texte.Length; i++)
            {
                if (maj)
                    nouveauTexte = nouveauTexte + char.ToUpper(texte.ElementAt(i));
                else
                    nouveauTexte = nouveauTexte + char.ToLower(texte.ElementAt(i));
                maj = (texte.ElementAt(i) == delimiteur);
            }

            return nouveauTexte;
        }

        public static string replaceCaractere(this string texte)
        {
            string nouveauTexte = texte.Replace('-', ' ');
            nouveauTexte = nouveauTexte.Replace('_', ' ');

            return nouveauTexte;
        }
    }
}
