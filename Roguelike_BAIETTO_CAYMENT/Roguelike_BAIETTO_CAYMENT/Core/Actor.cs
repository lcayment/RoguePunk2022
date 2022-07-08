using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.BigColors;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Classe qui hérite des interfaces Actor, Dessin et Speed
    public class Actor : IActor, IDessin, ISpeed
    {
        // ------------------- IActor -------------------
        private string _name;           // nom
        private int _attack;            // points d'attaque
        private int _attackChance;      // taux de réussite de l'attaque
        private int _fov;               // valeur du fov
        private int _defense;           // points de défense
        private int _defenseChance;     // taux de réussite de la défense
        private int _credit;            // nombre de crédits
        private int _health;            // points de vie actuels
        private int _maxHealth;         // points de vie maximum
        private int _speed;             // vitesse 
        public int _niveau;             // niveau 
        public int _exp;                // experience
        private string _classe;         // equipement choisi (offensif, defensif, furtif)
        private bool _carte;            // carte d'acces (true/false)
        private int _nbKitRepa;         // nombre de kit de reparation
        private bool _canAttack;        // peut attaquer (true/false)
        public bool _isDead;            // est mort (true/false)

        // Accesseur des variables de l'interface Actor
        public int Attack
        {
            get { return _attack; }
            set { _attack = value; }
        }

        public int AttackChance
        {
            get { return _attackChance; }
            set { _attackChance = value; }
        }

        public int Fov
        {
            get { return _fov; }
            set { _fov = value; }
        }

        public int Defense
        {
            get { return _defense; }
            set { _defense = value; }
        }

        public int DefenseChance
        {
            get { return _defenseChance; }
            set { _defenseChance = value; }
        }

        public int Credit
        {
            get { return _credit; }
            set { _credit = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        public int Niveau
        {
            get { return _niveau; }
            set { _niveau = value; }
        }
        public int Exp
        {
            get { return _exp; }
            set { _exp = value; }
        }
        public string Classe
        {
            get { return _classe; }
            set { _classe = value; }
        }

        public bool carte
        {
            get { return _carte; }
            set { _carte = value; }
        }
        public bool CanAttack
        {
            get { return _canAttack; }
            set { _canAttack = value; }
        }
        public int nbKitRepa
        {
            get { return _nbKitRepa; }
            set { _nbKitRepa = value; }
        }
        public bool IsDead
        {
            get { return _isDead; }
            set { _isDead = value; }
        }
        // ------------------- IDessin -------------------
        public RLColor color { get; set; }      // couleur correspondante
        public char Symbol { get; set; }        // symbole correspondant
        public int PosX { get; set; }           // position en X
        public int PosY { get; set; }           // position en Y

        // ------------------- ISpeed -------------------
        public int Time
        {
            get { return Speed; }
        }

        // ------------------- Méthode de dessin de l'acteur -------------------
        public void Draw(RLConsole console, IMap map)
        {
            // si la cellule n'a pas été exploré le joueur ne peux pas être dessiné dedans
            if (!map.GetCell(PosX, PosY).IsExplored)
            {
                return;
            }

            // Dans le Fov, on dessine le joueur
            if (map.IsInFov(PosX, PosY))
            {
                console.Set(PosX, PosY, color, Colors.P1_FloorBackgroundFov, Symbol);
            }
            else
            {
                // Pas dans le Fov donc dessin normal
                console.Set(PosX, PosY, Colors.P1_Floor, Colors.P1_FloorBackground, '.');
            }
        }
    }
}
