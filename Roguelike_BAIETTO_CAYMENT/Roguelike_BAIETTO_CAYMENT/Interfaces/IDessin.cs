using RLNET;
using RogueSharp;

namespace Roguelike_BAIETTO_CAYMENT.Interfaces
{
    public interface IDessin
    {
        RLColor color { get; set; }     // couleur correspondante
        char Symbol { get; set; }       // symbole correspondant
        int PosX { get; set; }          // position en X
        int PosY { get; set; }          // position en X

        // Méthode pour dessin sur une console donnée
        void Draw(RLConsole console, IMap map);
    }
}
