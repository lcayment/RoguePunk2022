using RLNET;
using Roguelike_BAIETTO_CAYMENT.Core;
using Roguelike_BAIETTO_CAYMENT.Systems;

namespace Roguelike_BAIETTO_CAYMENT.Interfaces
{
    public interface IBehavior
    {
        // Méthode pour faire agir l'ennemi
        bool Act(Enemy enemy, SystemeDeCommande commandSystem);
    }
}
