using System;
using System.Collections.Generic;
using RogueSharp.Random;

namespace RogueSharpRLNetSamples.Systems
{
    public class Pool<T>
    {
        private readonly List<PoolItem<T>> _poolItems; // On crée une liste qui va contenir tout les objets à déposer sur la carte

        public Pool()
        {
            _poolItems = new List<PoolItem<T>>(); // Constructeur
        }

        public T Get() // On récupère dans le pool les objets à l'interieur
        {
            foreach (var poolItem in _poolItems) // On parcours tout les éléments
            {

                Remove(poolItem); // On retire  l'item
                return poolItem.Item; // On récupère l'item en quetion

            }

            throw new InvalidOperationException("Could not get an item from the pool"); // PLus d'objet dans la liste
        }

        public void Add(T item)
        {
            _poolItems.Add(new PoolItem<T> { Item = item });   // ON ajoute un élément au pool        
        }

        public void Remove(PoolItem<T> poolItem)
        {
            _poolItems.Remove(poolItem); // ON enlève l'élément 
        }
    }

    public class PoolItem<T>
    {
        public T Item { get; set; } // On rends accesible les éléments du pool
    }
}
