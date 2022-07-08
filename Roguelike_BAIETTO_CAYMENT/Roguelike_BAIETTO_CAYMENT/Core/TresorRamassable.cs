using Roguelike_BAIETTO_CAYMENT.Interfaces;


namespace Roguelike_BAIETTO_CAYMENT.Core
{
    public class TresorRamassable
    {
        public int PosX { get; set; }           // Position en X
        public int PosY { get; set; }           // Position en Y
        public ITresor Treasure { get; set; }   // Type de trésor

        // Constructeur
        public TresorRamassable(int x, int y, ITresor treasure)
        {
            PosX = x;
            PosY = y;
            Treasure = treasure;

            IDessin drawableTreasure = treasure as IDessin;
            if (drawableTreasure != null)
            {
                drawableTreasure.PosX = x;
                drawableTreasure.PosY = y;
            }
        }
    }
}