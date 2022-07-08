using RLNET;
using Roguelike_BAIETTO_CAYMENT.Core;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Text;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    // Gère les entrées utilisateurs (déplacement, actions pendant le combat, choix du stuff, etc.)
    public class SystemeDeCommande
    {
        public bool IsPlayerTurn { get; set; }      // est ce que c'est au joueur de jouer ?
        public int HitsPlayer;      // correspond au nombre de dégat que le joueur inflige
        public int BlockPlayer;     // correspond au nombre de dégat que le joueur peut encaisser
        public int HitsEnemy;       // correspond au nombre de dégat que l'ennemi inflige
        public int BlockEnemy;      // correspond au nombre de dégat que l'ennemi peut encaisser


        // ####################################################################################################################
        //                CHOIX AVEC ENTREES UTILISATEUR
        // ####################################################################################################################
        // -------------- Choix de l'action pendant le combat --------------
        public RLKey ChoiceCombat(RLKey key)
        {
            if (key == RLKey.Q)         // Attaque
            {
                int crit = 1;
                if ((Game.player1.Classe == "Cyborg offensif") && (Dice.Roll("1D10") < 3))
                {
                    crit = 2;
                    Game.messageInit.Add("Vous avez mis un coup critique !! Degats x2");
                    Game.messageInit.Add("");
                }
                HitsPlayer = Game.systDeCommande.Attack(Game.player1) * crit;
                return key;
            }
            else if (key == RLKey.D)    // Défense
            {
                BlockPlayer = Game.systDeCommande.Defense(Game.player1);
                return key;
            }
            else if (key == RLKey.P)    // Potion
            {
                if (Game.player1.nbKitRepa != 0)
                {
                    Game.player1.KitRepa.Use();
                }
                else
                {
                    Game.messageInit.Add("Vous n'avez rien pour vous reparer !!");
                    Game.messageInit.Add("");
                }
                // méthode pour se soigner
                return key;
            }
            else if (key == RLKey.F)    // Fuite
            {
                if ((Game.player1.Classe != "Cyborg furtif") && (Dice.Roll("1D10") < 6))
                {
                    Game.messageInit.Add("Vous n'avez pas pu fuir !!");
                    Game.messageInit.Add("");
                }
                else
                {
                    Game._displayCombat = false;
                }
                return key;
            }
            else
            {
                return 0;
            }
        }

        // -------------- Affichage de la fenêtre de menu avec le but du jeu et les contrôles --------------
        public bool DisplayMenu(RLKey key)
        {
            if (!Game._displayCombat)
            {
                if (key == RLKey.Semicolon)     // Lorsque l'on appuie sur la touche M
                {
                    Game.messageInit.Delete();

                    // Lecture et affichage du fichier texte
                    string message = System.IO.File.ReadAllText("Txt/butdujeu.txt");
                    foreach (var line in message.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
                    {
                        Game.messageInit.Add(line);
                    }

                    return true;
                }
                else if (key == RLKey.X)        // Lorsque l'on appuie sur la touche X
                {
                    return false;
                }
                else        // Lorsque l'on appuie sur un autre touche
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        // -------------- Choix de l'équipement par le joueur --------------
        public bool ChoiceClasse(RLKey key)
        {
            if (key == RLKey.D)     // Choix défensif
            {
                Game.player1.Classe = "Cyborg defensif";
                Game.player1.Defense += 3;
                Game.player1.DefenseChance = 100;
                Game._findujeu = false;     // utilisée lorsque l'on relance une partie pour signaler que ca en est un nouvelle
                return true;
            }
            else if (key == RLKey.O)    // Choix offensif
            {
                Game.player1.Classe = "Cyborg offensif";
                Game.player1.Attack += 2;
                Game.player1.AttackChance = 70;
                Game._findujeu = false;      // utilisée lorsque l'on relance une partie pour signaler que ca en est un nouvelle
                return true;
            }
            else if (key == RLKey.F)    // Chois furtif
            {
                Game.player1.Classe = "Cyborg furtif";
                Game.player1.Speed = 7;
                Game._findujeu = false;      // utilisée lorsque l'on relance une partie pour signaler que ca en est un nouvelle
                return true;
            }
            else
            {
                return false;
            }
        }

        // -------------- Mouvement du joueur --------------
        // Retourne false si le joueur ne peux pas bouger, true sinon
        public bool MovePlayer(RLKey keyPress)
        {
            int x = Game.player1.PosX;
            int y = Game.player1.PosY;

            // Quelles flèches est pressée
            if (keyPress == RLKey.Up)       // le joueur se déplace vers le haut
            {
                y = Game.player1.PosY - 1;
            }
            else if (keyPress == RLKey.Down)      // le joueur se déplace vers le bas
            {
                y = Game.player1.PosY + 1;
            }
            else if (keyPress == RLKey.Left)      // le joueur se déplace vers la gauche
            {
                x = Game.player1.PosX - 1;
            }
            else if (keyPress == RLKey.Right)      // le joueur se déplace vers la gauche
            {
                x = Game.player1.PosX + 1;
            }
            else
            {
                return false;
            }

            // On place le joueur sur la bonne case
            if (Game.citymap1.SetActorPosition(Game.player1, x, y))
            {
                return true;
            }

            // Utilise cette mathode pour déterminer si il y a un ennemi dans la cellule
            Enemy enemy = Game.citymap1.IsEnemy(x, y);

            // Si il y a un ennemi, le combat se lance
            if (enemy != null)
            {
                Game._displayCombat = Game.systDeCommande.LaunchCombat(enemy);
                return true;
            }

            return false;
        }

        // -------------- Utilisation du kit de réparation --------------
        public bool UtiliserKit(RLKey key)
        {
            if (key == RLKey.P)     // Si on presse P
            {
                return Game.player1.KitRepa.Use();
            }
            return false;
        }

        // ####################################################################################################################
        //                          FIN DE TOUR OU DE PARTIE
        // ####################################################################################################################
        // -------------- Le tour est fini --------------
        public void EndTurnCombat()
        {
            int damage;
            damage = HitsEnemy - BlockPlayer;       // calcul des dégats infligés au joueur

            if (damage >= 0)
            {
                Game.player1.Health -= damage;      // application des dégats
            }

            if (Game.player1.Health <= 0)   // si le joueur est mort
            {
                Mort(Game.player1);
                Game.player1.Health = 0;
            }

        }

        // -------------- Le tour du joueur est fini --------------
        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        // -------------- Partie finie ou perdue --------------
        public void EndGame(RLKey key)
        {
            if (key == RLKey.R)  // Relance une partie
            {
                Game._findujeu = true;
                Game.Init();
            }
        }

        // ####################################################################################################################
        //                          VISUEL DE COMBAT
        // ####################################################################################################################
        // -------------- Affichage de la fenêtre de combat --------------
        public bool LaunchCombat(Enemy enemy)
        {
            Game.messageInit.Delete();

            // Messages du début de la partie
            Game.messageInit.Add($"{enemy.Name} et {Game.player1.Name} vont combattre !");
            Game.messageInit.Add("COMBAT");
            Game.messageLog.Add("Le combat se lance !");
            Game.messageInit.Add("Vous devez choisir votre action ! A pour attaquer, D pour defendre, F pour fuir et P pour potion !");
            Game.messageInit.Add("Une fois que vous avez choisi vous devez cliquez sur la touche \"Entree\"");

            // Ici cela signifie que l'ennemi ne peux plus attaque le joueur depuis la map
            // Cette variable est remise a true lorsque le joueur sort du FoV de l'ennemi
            // En réalité cette variable est seulement utile lorsque le joueur choisi de fuir le combat
            enemy.CanAttack = false;
            return true;
        }


        // ####################################################################################################################
        //                        MOUVEMENTS DES ACTEURS
        // ####################################################################################################################
        // -------------- Mouvement de l'ennemi --------------
        public void MoveEnemy(Enemy enemy, ICell cell)
        {
            if (!Game.citymap1.SetActorPosition(enemy, cell.X, cell.Y))     // si l'ennemi s'est déplacé
            {
                if (Game.player1.PosX == cell.X && Game.player1.PosY == cell.Y)     // si l'ennemi s'est déplacé là où le joueur est
                {
                    if (enemy.CanAttack)        // si l'ennemi peut attaquer (il peut ne pas pouvoir attaquer si le joueur a choisi la fuite)
                    {
                        Game._displayCombat = Game.systDeCommande.LaunchCombat(enemy);      // on lance le combat
                    }
                }
            }
        }

        // -------------- Activation du mouvement de l'ennemi --------------
        public void ActivateEnemy()
        {
            ISpeed scheduleable = Game.schedulingSystem.Get();
            if (scheduleable is Player)     // si l'acteur est un joueur
            {
                IsPlayerTurn = true;
                Game.schedulingSystem.Add(Game.player1);    // on l'ajoute au système de scheduler
            }
            else        // si l'acteur est un ennemi
            {
                Enemy enemy = scheduleable as Enemy;
                if (enemy != null)
                {
                    enemy.PerformAction(this);
                    Game.schedulingSystem.Add(enemy);
                }
                ActivateEnemy();
            }
        }


        // ####################################################################################################################
        //                       ATTAQUE, DEFENSE, MORT
        // ####################################################################################################################
        // -------------- Méthode pour l'attaque des acteurs --------------
        public int Attack(Actor attacker)
        {
            // génère un nombre aléatoire entre 0 et 100
            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            int hits = 0; ;
            int attackChance = 0;

            // on determine si l'acteur est un ennemi ou un joueur pour selectionner le taux d'attaque
            if (attacker is Player)
            {
                attackChance = Game.player1.AttackChance;
            }
            else if (attacker is Enemy)
            {
                attackChance = attacker.AttackChance;
            }

            // le nombre aléatoire généré avant détermine si l'attaque est une réussite ou non
            if (randomNumber >= attackChance)       // l'attaque a réussi
            {
                if (attacker is Player)
                {
                    hits = Game.player1.Attack;
                    Game.messageInit.Add($"Nombre de degats infliges par {Game.player1.Name} : {hits}");
                }
                else if (attacker is Enemy)
                {
                    hits = attacker.Attack;
                    Game.messageInit.Add($"Nombre de degats infliges par l'ennemi : {hits}");

                }
            }
            else        // l'attaque a échoué
            {
                Game.messageInit.Add($"L'attaque a echoue");
                hits = 0;
            }
            return hits;

        }

        // -------------- Méthode pour la defense des acteurs --------------
        public int Defense(Actor defenser)
        {
            // génère un nombre aléatoire entre 0 et 100
            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            int block = 0; ;
            int defenseChance = 0;

            // on determine si l'acteur est un ennemi ou un joueur pour selectionner le taux d'attaque
            if (defenser is Player)
            {
                defenseChance = Game.player1.DefenseChance;
            }
            else if (defenser is Enemy)
            {
                defenseChance = defenser.DefenseChance;
            }

            // le nombre aléatoire généré avant détermine si la défense est une réussite ou non
            if (randomNumber >= defenseChance)      // la défense a réussi
            {
                if (defenser is Player)
                {
                    block = Game.player1.Defense;
                    Game.messageInit.Add($"Nombre de degats bloques par {Game.player1.Name} : {block}");

                }
                else if (defenser is Enemy)
                {
                    block = defenser.Defense;
                    Game.messageInit.Add($"Nombre de degats bloques par l'ennemi : {block}");

                }
            }
            else        // la defense a échoué
            {
                Game.messageInit.Add($"La defense a echoue");
                block = 0;
            }
            return block;

        }

        // -------------- Méthode pour la mort des acteurs --------------
        public void Mort(Actor defender)
        {
            if (defender is Player)     // si le joueur est mort
            {
                Game.messageLog.Add($"  {defender.Name} est mort ...");
                Game._displayCombat = false;

                Game.messageInit.Delete();
                // Lecture et affichage du fichier texte gameover.txt
                string message = System.IO.File.ReadAllText("Txt/gameover.txt");
                foreach (var line in message.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
                {
                    Game.messageInit.Add(line);
                }

                Game._gameOver = true;

            }
            else if (defender is Enemy)     // si l'ennemi est mort
            {
                Game.citymap1.RemoveEnemy((Enemy)defender);     // on l'enleve de la map

                // le joueur récupère des crédits
                Game.messageLog.Add($"  {defender.Name} est mort et a drop {defender.Credit} credits");
                Game.player1.Credit += defender.Credit;
                Game.player1.Exp += defender.Exp;
                if (Game.player1.Exp >= 25)
                {
                    Game.messageLog.Add("Vous avez gagné un niveau !! Vous etes plus fort");
                    LevelUpPlayer(Game.player1);
                }
                Game._displayCombat = false;        // reaffichage de la map après la fin du combat
            }
        }
        // --------------------Méthode pour augmenter le niveau du joueur---------------------------------
        public void LevelUpPlayer(Player player)
        {
            if (player.Classe == "Cyborg offensif")
            {
                player.Health += 5;
                player.MaxHealth += 5;
                Game.messageLog.Add("Niveau superieur : Vie max +5 !!");

                player.Attack += 1;
                player.AttackChance += 2;
                Game.messageLog.Add("Niveau superieur : Atk +1 !!");
                Game.messageLog.Add("Niveau superieur : Atk Chance +2 !!");

                if (player.Niveau % 2 == 1)
                {
                    player.Defense += 1;
                    player.DefenseChance += 2;
                    player.Speed -= 1;
                    if (player.Speed - 1 < 1)
                        player.Speed = 1;
                    else
                        player.Speed -= 1;
                    Game.messageLog.Add("Niveau superieur : Def +1 !!");
                    Game.messageLog.Add("Niveau superieur : Def Chance +1 !!");
                    Game.messageLog.Add("Niveau superieur : Vous etes plus rapide !!");

                }
            }
            else if (player.Classe == "Cyborg defensif")
            {
                player.Health += 5;
                player.MaxHealth += 5;
                Game.messageLog.Add("Niveau superieur : Vie max +5 !!");

                player.Defense += 1;
                player.DefenseChance += 2;
                Game.messageLog.Add("Niveau superieur : Def +1 !!");
                Game.messageLog.Add("Niveau superieur : Def Chance +1 !!");

                if (player.Niveau % 2 == 1)
                {
                    player.Attack += 1;
                    player.AttackChance += 2;
                    if (player.Speed - 1 < 1)
                        player.Speed = 1;
                    else
                        player.Speed -= 1;
                    Game.messageLog.Add("Niveau superieur : Atk +1 !!");
                    Game.messageLog.Add("Niveau superieur : Atk Chance +2 !!");
                    Game.messageLog.Add("Niveau superieur : Vous etes plus rapide !!");

                }
            }
            else if (player.Classe == "Cyborg furtif")
            {
                player.Health += 5;
                player.MaxHealth += 5;
                Game.messageLog.Add("Niveau superieur : Vie max +5 !!");

                if (player.Speed - 2 < 1)
                    player.Speed = 1;
                else
                    player.Speed -= 2;
                Game.messageLog.Add("Niveau superieur : Vous etes beaucoup plus rapide !!");

                if (player.Niveau % 2 == 1)
                {
                    player.Defense += 1;
                    player.DefenseChance += 2;
                    Game.messageLog.Add("Niveau superieur : Def +1 !!");
                    Game.messageLog.Add("Niveau superieur : Def Chance +1 !!");

                    player.Attack += 1;
                    player.AttackChance += 2;
                    Game.messageLog.Add("Niveau superieur : Atk +1 !!");
                    Game.messageLog.Add("Niveau superieur : Atk Chance +2 !!");
                }
            }
            player.Niveau += 1;
            player.Exp -= 25;
        }
    }
}
