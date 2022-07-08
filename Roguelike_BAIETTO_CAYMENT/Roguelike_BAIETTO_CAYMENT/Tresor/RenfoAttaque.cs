using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Interfaces;

namespace Roguelike_BAIETTO_CAYMENT.Tresor
{
    public class RenfoAttaque : Core.Tresor
    {
        public RenfoAttaque()
        {
            Symbol = (char)18;
            color = RLColor.White;
        }
    }
}