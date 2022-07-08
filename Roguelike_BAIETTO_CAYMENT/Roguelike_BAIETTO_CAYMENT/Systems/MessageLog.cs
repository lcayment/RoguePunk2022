using RLNET;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using System.Collections.Generic;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    // Classe qui correspond aux messages affichés dans la sous-console de log (choix du stuff, gameover, fin du jeu, menu)
    public class MessageLog
    {
        // On défine le nombre de lignes qui seront stockées
        private static readonly int _maxLines = 9;

        // On utilie une Queue pour garder les lignes
        // La première ligne ajouté sera donc la première supprimée
        private readonly Queue<string> _lines;

        // Constructeur
        public MessageLog()
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

        // On dessine chaque messages en blanc
        public void Draw(RLConsole _logConsole, int _largeurConsole, int _hauteurConsole)
        {
            _logConsole.SetBackColor(0, 0, _largeurConsole, _hauteurConsole, Palette.P1_DarkBlue);
            string[] lines = _lines.ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                _logConsole.Print(2, i * 2 + 1, lines[i], RLColor.White);
            }
        }

    }
}
