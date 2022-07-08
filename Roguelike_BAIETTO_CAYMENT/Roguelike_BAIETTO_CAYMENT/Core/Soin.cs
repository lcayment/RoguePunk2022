
using System;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Classe utilisée lors de l'utilisation d'un kit de réparation
    public class Soin
    {
        private readonly int _amountToHeal;     // quantité de guérison

        // Constructeur
        public Soin(int amountToHeal)
        {
            _amountToHeal = amountToHeal;
        }

        // Méthode pour se soigner
        public bool SeSoigner()
        {
            Player player = Game.player1;
            player.Health = Math.Min(player.MaxHealth, player.Health + _amountToHeal);

            return true;
        }
    }
}
