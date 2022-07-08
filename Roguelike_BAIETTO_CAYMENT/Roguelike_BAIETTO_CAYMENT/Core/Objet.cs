using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Objets;


namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Objet hérite de 3 interfaces : IOBjet, ITresor et IDessin
    public class Objet : IObjet, ITresor, IDessin
    {
        // Constructeur
        public Objet()
        {
            Symbol = (char)20;
            color = RLColor.Yellow;
        }

        // ------------------- IObjet -------------------
        public string Name { get; protected set; }              // Nom de l'objet
        public int UsageRestant { get; protected set; }         // Nombre d'usage restant

        // Utilisation de l'item
        public bool Use()
        {
            return UseItem();
        }

        // Utilisation de l'item
        protected virtual bool UseItem()
        {
            return false;
        }

        // Ramassage de l'item
        public bool PickUp(IActor actor)
        {
            Player player = actor as Player;

            if (player != null)
            {
                if (this is KitReparation)
                {
                    Game.messageLog.Add($"{actor.Name} a trouve un kit de reparation !!");
                    player.nbKitRepa++;
                    return true;
                }
                return false;
            }

            return false;
        }

        // ------------------- ITresor -------------------
        public RLColor color { get; set; }      // Couleur du trésor
        public char Symbol { get; set; }        // Symbole du trésor
        public int PosX { get; set; }           // Position en X
        public int PosY { get; set; }           // Position en Y

        // On dessine l'objet
        public void Draw(RLConsole console, IMap map)
        {
            if (!map.IsExplored(PosX, PosY))
            {
                return;
            }

            if (map.IsInFov(PosX, PosY))
            {
                console.Set(PosX, PosY, color, Colors.P1_FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(PosX, PosY, Colors.P1_Floor, Colors.P2_FloorBackground, '.');
            }
        }
    }
}