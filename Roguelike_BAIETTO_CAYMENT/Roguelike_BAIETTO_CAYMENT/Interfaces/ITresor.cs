using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Roguelike_BAIETTO_CAYMENT.Interfaces
{
    public interface ITresor
    {
        bool PickUp(IActor actor);      // trésor ramassé (true/false)
    }
}

