using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike_BAIETTO_CAYMENT.Interfaces
{

    public interface IObjet
    {
        string Name { get; }            // dénomination de l'objet
        int UsageRestant { get; }       // nombre d'usage restant 

        // Méthode pour utiliser l'objet
        bool Use();
    }
}