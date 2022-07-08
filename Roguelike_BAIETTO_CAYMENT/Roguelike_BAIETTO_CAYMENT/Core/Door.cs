using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.BigColors;


namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Door hérite de l'interface Dessin
    public class Door : IDessin
    {
        // Constructeur
        public Door()
        {
            Symbol = (char)10;
            color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }

        public bool IsOpen { get; set; }                // porte ouverte (true/false)
        public RLColor color { get; set; }              // couleur de la porte
        public RLColor BackgroundColor { get; set; }    // couleur du fond de la porte
        public char Symbol { get; set; }                // symbole de la porte
        public int PosX { get; set; }                   // position en X
        public int PosY { get; set; }                   // position en Y

        // On dessin la porte
        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(PosX, PosY).IsExplored)
            {
                return;
            }

            // En fonction de si elle est ouverte ou fermée on dessine des symboles différents
            Symbol = IsOpen ? (char)9 : (char)10;

            // Selon si elle est dans le FoV du joueur ou pas, on lui attribue des couleurs différentes
            if (map.IsInFov(PosX, PosY))
            {
                color = Colors.DoorFov;
                BackgroundColor = Colors.DoorBackgroundFov;
            }
            else
            {
                color = Colors.Door;
                BackgroundColor = Colors.DoorBackground;
            }

            console.Set(PosX, PosY, color, BackgroundColor, Symbol);
        }
    }
}