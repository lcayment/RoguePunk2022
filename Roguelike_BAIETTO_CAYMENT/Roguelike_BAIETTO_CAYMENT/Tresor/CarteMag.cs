using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.BigColors;

namespace Roguelike_BAIETTO_CAYMENT.Tresor
{
    public class CarteMag : Core.Tresor
    {
        public CarteMag()
        {
            Symbol = (char)21;
            color = Colors.ColorCarte;
        }
    }
}