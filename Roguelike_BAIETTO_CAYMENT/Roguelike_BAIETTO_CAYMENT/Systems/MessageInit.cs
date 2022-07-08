using RLNET;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using System.Collections.Generic;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    // Classe qui correspond aux messages affichés à la place de la map (choix du stuff, gameover, fin du jeu, menu)
    public class MessageInit
    {
        // On défine le nombre de lignes qui seront stockées
        private static readonly int _maxLines = 35;

        // On utilie une Queue pour garder les lignes
        // La première ligne ajouté sera donc la première supprimée
        private readonly Queue<string> _lines;

        // Constructeur
        public MessageInit()
        {
            _lines = new Queue<string>();
        }

        //On ajoute une ligne à la Queue
        public void Add(string message)
        {
            _lines.Enqueue(message);

            // Suppression de la ligne la plus ancienne lorsque le nombre de lignes max est atteint
            if (_lines.Count > _maxLines)
            {
                _lines.Dequeue();
            }
        }

        public void Delete()
        {
            _lines.Clear();
        }

        // On dessine chaque messages en blanc
        public void Draw(RLConsole _mapConsole, int _largeurConsole, int _hauteurConsole)
        {
            _mapConsole.SetBackColor(0, 0, _largeurConsole, _hauteurConsole, Palette.DbDark);
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                _mapConsole.Print(2, i * 2 + 1, lines[i], RLColor.White);
            }
        }

    }
}
