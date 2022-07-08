using RLNET;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.Objets;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Player hérite de la classe Actor
    public class Player : Actor
    {
        public IObjet Carte { get; set; }       // Carte d'accès
        public IObjet KitRepa { get; set; }     // Kit de réparation

        // Constructeur
        public Player()
        {
            Name = "Rogue";
            Attack = 5;
            AttackChance = 50;
            Fov = 10;
            Defense = 5;
            DefenseChance = 40;
            Credit = 0;
            Health = 50;
            MaxHealth = 50;
            Speed = 10;
            Niveau = 1;
            Exp = 0;
            color = Colors.Player;
            Symbol = (char)22;
            PosX = 10;
            PosY = 10;
            Classe = " ";
            carte = false;
            nbKitRepa = 1;
            KitRepa = new KitReparation();
        }

        // Dessine les statistiques
        public void DrawStats(RLConsole _statsConsole)
        {

            _statsConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            _statsConsole.Print(1, 3, $"Niveau:  {Niveau}", Colors.Text);
            _statsConsole.Print(1, 5, $"Exp :    {Exp}/25", Colors.Text);
            _statsConsole.Print(1, 7, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            _statsConsole.Print(1, 9, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            _statsConsole.Print(1, 11, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            _statsConsole.Print(1, 13, $"Credit:  {Credit}", Palette.P1_Yellow);
            _statsConsole.Print(1, 15, $"Classe:  {Classe}", Colors.Text);
            _statsConsole.Print(1, 17, $"Carte :  {carte}", Palette.P1_Orange);
            _statsConsole.Print(1, 19, $"(P)Kit de Reparation : {nbKitRepa}", Colors.Text);
        }

    }
}
