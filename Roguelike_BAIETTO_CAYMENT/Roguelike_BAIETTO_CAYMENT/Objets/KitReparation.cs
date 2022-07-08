using RLNET;
using Roguelike_BAIETTO_CAYMENT.Core;

namespace Roguelike_BAIETTO_CAYMENT.Objets
{
    public class KitReparation : Objet
    {
        public KitReparation()
        {
            Name = "Kit de Reparation";
            UsageRestant = 1;
        }

        protected override bool UseItem()
        {
            int healAmount = 20;

            if (Game.player1.nbKitRepa != 0)
            {
                Game.messageLog.Add($"{Game.player1.Name} consumes a {Name} and recovers {healAmount} health");

                Soin heal = new Soin(healAmount);

                Game.player1.nbKitRepa--;

                return heal.SeSoigner();
            }
            else
            {
                Game.messageLog.Add($"Vous n'avez pas de quoi vous soigner");
                return false;
            }

        }
    }
}
