using Roguelike_BAIETTO_CAYMENT.Core;
using Roguelike_BAIETTO_CAYMENT.Objets;
using RogueSharpRLNetSamples.Systems;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    public class ItemGenerator
    {
        public Objet CreateItem()
        {
            Pool<Objet> itemPool = new Pool<Objet>(); // On génére un pool d'objet

            itemPool.Add(new KitReparation());  // On ajoute les kits de reparation au pool

            return itemPool.Get(); // On renvoie tout les items du pool  (ici que les kits)
        }
    }
}
