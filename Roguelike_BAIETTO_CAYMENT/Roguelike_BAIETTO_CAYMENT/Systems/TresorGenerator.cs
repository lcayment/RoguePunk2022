using System;
using System.Collections.Generic;
using RogueSharpRLNetSamples.Systems;
using Roguelike_BAIETTO_CAYMENT.Tresor;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    class TresorGenerator // Fonctionne comme ItemGenerator mais pour la génération des trésors
    {
        private readonly Pool<Core.Tresor> _tresorPool; // On crée un pool de trésor (la classe)

        public TresorGenerator()
        {
            _tresorPool = new Pool<Core.Tresor>();

            _tresorPool.Add(new RenfoArmure());
            _tresorPool.Add(new RenfoAttaque());
            _tresorPool.Add(new CarteMag());
        }

        public Core.Tresor CreateTresor()
        {
            return _tresorPool.Get(); // On recup les élements (cad Renfoarmure, renfoattaque et la carte)
        }
    }
}
