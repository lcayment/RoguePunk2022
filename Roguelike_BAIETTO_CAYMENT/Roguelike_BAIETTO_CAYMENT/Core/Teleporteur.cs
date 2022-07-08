using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.BigColors;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Téléporteur hérite de l'interface IDessin
    public class Teleporteur : IDessin
    {
        public RLColor color { get; set; }      // couleur du téléporteur
        public char Symbol { get; set; }        // symbol du téléporteur
        public int PosX { get; set; }           // position en X
        public int PosY { get; set; }           // position en Y
        public bool IsDown { get; set; }        // Mène vers le bas ou vers le haut (true/false)

        // On dessine le téléporteur
        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(PosX, PosY).IsExplored)
            {
                return;
            }

            Symbol = IsDown ? (char)31 : (char)30;      // Selon si il mène vers l'étage sup ou inf, le symbole diffère

            // Si il est das le FoV ou non 
            if (map.IsInFov(PosX, PosY))
            {
                color = Colors.Player;
            }
            else
            {
                Symbol = '.';
                color = Colors.P1_Floor;
            }

            console.Set(PosX, PosY, color, null, Symbol);
        }
    }
}