using RLNET;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Enemies;
using Roguelike_BAIETTO_CAYMENT.Systems;
using System;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    // Enemy hérite de Actor
    public class Enemy : Actor
    {
        public int? TurnsAlerted { get; set; }              // le type int? signifie que la valeur peut être sous forme de int ou null

        // L'enemy fait une action (bouge, attaque)
        public virtual void PerformAction(SystemeDeCommande commandSystem)
        {
            var behavior = new StandardMove();
            behavior.Act(this, commandSystem);
        }

        // Dessine les stats de l'ennemi
        public void DrawStats(RLConsole _statsEnnemiesConsole, int position)
        {

            // On commence a Y=19 (sous les stats du joueur)
            // On multiplie par 2 pour laisser de l'espace entre les stats des ennemis
            int yPosition = 23 + (position * 2);

            // On écrit le symbole correspondant à l'ennemi
            _statsEnnemiesConsole.Print(1, yPosition, Symbol.ToString(), color);

            // Le niveau de vie restant est affichée
            int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * 16.0);
            int remainingWidth = 16 - width;

            // On différencie le nombre de pv restants et le nombre de vie max
            _statsEnnemiesConsole.SetBackColor(3, yPosition, width, 1, Palette.P1_Orange);
            _statsEnnemiesConsole.SetBackColor(3 + width, yPosition, remainingWidth, 1, Palette.P1_Yellow);

            // Le nom de chaque ennemi est inscrit sur la barre de vie
            _statsEnnemiesConsole.Print(2, yPosition, $": {Name}", Palette.DbLight);
        }

        // L'ennemi attaque
        public void AttackEnemy()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 2);
            int damage;

            // on choisit aléatoirement l'attaque ou la défense (50/50)
            if (randomNumber == 1)
            {
                Game.systDeCommande.HitsEnemy = Game.systDeCommande.Attack(this);
            }
            else
            {
                Game.systDeCommande.BlockEnemy = Game.systDeCommande.Defense(this);
            }

            // si le joueur a + de 0 hits, le block sert, sinon la santé ne change pas
            damage = Game.systDeCommande.HitsPlayer - Game.systDeCommande.BlockEnemy;
            if (damage >= 0)
            {
                this.Health -= damage;
            }
            Game.messageInit.Add($"Nombre de pv restants de l'ennemi : {this.Health}");

            if (this.Health <= 0)       // Si l'ennemi est mort
            {
                Game.systDeCommande.Mort(this);
                Game._displayCombat = false;
                this.IsDead = true;
            }
            else
            {
                this.IsDead = false;
            }

            Game.systDeCommande.EndTurnCombat();        // On finit le tour du combat

            // on réinitialise les valeurs
            Game.systDeCommande.HitsEnemy = 0;
            Game.systDeCommande.BlockEnemy = 0;
            Game.systDeCommande.HitsPlayer = 0;
            Game.systDeCommande.BlockPlayer = 0;
        }

    }
}
