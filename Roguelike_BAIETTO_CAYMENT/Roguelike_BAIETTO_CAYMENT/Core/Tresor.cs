using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.Tresor;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Tresor hérite des interfaces ITresor et IDessin
    public class Tresor : ITresor, IDessin
    {
        // Constructeur
        public Tresor()
        {
            Symbol = '.';
            color = RLColor.Yellow;
        }

        // Les actions diffèrent selon le type de trésor
        public bool PickUp(IActor actor)
        {
            Player player = actor as Player;
            if (player != null)
            {
                if (this is RenfoArmure)
                {
                    player.Defense += 1;
                    Game.messageLog.Add($"Blindage renforce !! Def +1");
                    return true;
                }
                if (this is RenfoAttaque)
                {
                    player.Attack += 1;
                    Game.messageLog.Add($"Attaque renforcee !! Atk +1");
                    return true;
                }
                if (this is CarteMag)
                {
                    player.carte = true;
                    Game.messageLog.Add($"Teleporteur deverouille !!");
                    return true;
                }
                return false;
            }
            return false;

        }

        // ------------------- IDessin -------------------
        public RLColor color { get; set; }
        public char Symbol { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }

        // On dessine le trésor en fonction de son symbole
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
                console.Set(PosX, PosY, Colors.P1_Floor, Colors.P1_FloorBackground, '.');
            }
        }
    }
}
