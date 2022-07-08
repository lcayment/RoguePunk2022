using RLNET;
using Roguelike_BAIETTO_CAYMENT.Core;
using Roguelike_BAIETTO_CAYMENT.Systems;
using RogueSharp.Random;
using System;

namespace Roguelike_BAIETTO_CAYMENT
{
    public class Game
    {
        // création de la console principale
        private static readonly int _largeurMain = 170;
        private static readonly int _hauteurMain = 110;
        public static RLRootConsole _mainConsole;

        // création de la console du titre
        private static readonly int _largeurTitre = 140;
        private static readonly int _hauteurTitre = 10;
        private static RLConsole _titreConsole;

        // création de la console de map
        private static readonly int _largeurMap = 140;
        private static readonly int _hauteurMap = 80;
        private static RLConsole _mapConsole;

        // création de la console de message
        private static readonly int _largeurLog = 140;
        private static readonly int _hauteurLog = 20;
        private static RLConsole _logConsole;

        // création de la console de stats du personnage
        private static readonly int _largeurStats = 30;
        private static readonly int _hauteurStats = 200;
        private static RLConsole _statsConsole;

        private static bool _majEcran = true;     // permet de ne pas redessiner la map si il n'y a pas eu de mouvement

        public static bool _classeChoisie = false;      // le joueur a choisi sa classe
        public static bool _displayMenu = false;        // le menu doit être affiché
        public static bool _displayCombat = false;      // le visuel combat doit être affiché
        public static RLKey _choixCombat = 0;           // correspond au choix du joueur durant le combat (A, D, P ou F)
        public static bool _endTurnCombat = false;      // un tour de combat est fini
        public static bool _gameOver = false;           // le joueur a perdu (0 PV) 
        public static bool _win = false;                // le joueur a gagné (arrivé en haut de la tour)
        public static bool _findujeu = false;           // la partie est terminée (perdu ou gagné)

        // carte : citymap
        public static CityMap citymap1 { get; private set; }

        // joueur 1
        public static Player player1 { get; set; }

        // système de commandes
        public static SystemeDeCommande systDeCommande { get; private set; }

        // messages affichés à la place de la carte
        public static MessageInit messageInit { get; private set; }

        // journal des messages dans la console de log
        public static MessageLog messageLog { get; private set; }

        // interface utilisée pour la génération de nombres aléatoires
        public static IRandom Random { get; private set; }
        public static int randomMapSeed = (int)DateTime.UtcNow.Ticks; // utilisé pour le numéro de "seed" (pour le debug)

        private static int _mapLevel = 1; // niveau initial de la carte

        // système de déplacement et vitesse des acteurs
        public static SchedulingSystem schedulingSystem { get; private set; }



        public static void Main()
        {
            string fontFileName = "terminal8x8.png";    // permet d'utiliser le fichier bitmap de RLNET

            Random = new DotNetRandom(randomMapSeed);       // genère des nombres pseudo-aléatoires pour le numéro de seed (pour le debug)

            string titreConsole = $"RoguePunk 2021 - Level 1";     // titre de la console principale

            // initialisation de la console principale avec chaque carreau faisant 8x8 pixels
            _mainConsole = new RLRootConsole(fontFileName, _largeurMain, _hauteurMain, 8, 8, 1f, titreConsole);

            // initialisation des 4 sous-consoles 
            _titreConsole = new RLConsole(_largeurTitre, _hauteurTitre);
            _mapConsole = new RLConsole(_largeurMap, _hauteurMap);
            _logConsole = new RLConsole(_largeurLog, _hauteurLog);
            _statsConsole = new RLConsole(_largeurStats, _hauteurStats);

            // initialisation du bandeau de titre (on récupère le contenu d'un fichier texte)
            string message = System.IO.File.ReadAllText("Txt/title.txt");
            int k = 2;
            foreach (var line in message.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
            {
                _titreConsole.Print(15, k++, line, BigColors.Palette.P1_Yellow);
            }

            Init();

            // RLNET's Update event : update des rendus graphiques
            _mainConsole.Update += OnRootConsoleUpdate;
            _mainConsole.Render += OnRootConsoleRender;

            // On lance le programme
            _mainConsole.Run();
        }

        private static void CreateMap()
        {
            // création de la nouvelle carte
            MapGenerator mapGenerator = new MapGenerator(_largeurMap, _hauteurMap, 20, 20, 10, _mapLevel);
            citymap1 = mapGenerator.CreateMap();

            // on met à jour le FoV du joueur
            citymap1.UpdatePlayerFieldOfView();
        }
        public static void Init()
        {
            // initialisation du système de scheduling pour les déplacements et la vitesse des acteurs
            schedulingSystem = new SchedulingSystem();

            // création du système de commande (gestion des déplacements, entrée utilisateur, attaque, defense, etc)
            systDeCommande = new SystemeDeCommande();

            // création de notre joueur
            player1 = new Player();

            // création de la carte et mise à jour du FoV
            CreateMap();

            // Création d'une instance de messageLog pour les messages de la console de log
            messageLog = new MessageLog();
            messageLog.Add($"Le niveau a ete cree avec le seed numero '{randomMapSeed}'");

            // Création d'une instance de messageInit pour les messages de la console centrale
            messageInit = new MessageInit();

            // Lecture et affichage du fichier de règles du début de partie
            messageInit.Delete();
            string message = System.IO.File.ReadAllText("Txt/regles.txt");
            foreach (var line in message.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
            {
                messageInit.Add(line);
            }

            // Réinitialisation des booléen (utile lorsqu'une partie est relancée)
            _classeChoisie = false;
            _statsConsole.Clear();
            _displayMenu = false;
            _displayCombat = false;
            player1.carte = false;
            _gameOver = false;
            _win = false;
            _findujeu = false;
        }

        // Event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool _joueurDeplace = false;     // le joueur s'est-il déplacé ?
            RLKeyPress keyPress = _mainConsole.Keyboard.GetKeyPress();      // entrée utilisateur

            // A tout moment, si on appuie sur Echap, la console se ferme
            if (keyPress != null)
            {
                // permet de quitter le jeu
                if (keyPress.Key == RLKey.Escape)
                {
                    _mainConsole.Close();
                }
            }

            // Lorsque nous sommes arrivés au dernier étage de la tour, un message de victoire s'affiche
            if ((_mapLevel == 7) && (!_win) && (!_findujeu))
            {
                messageInit.Delete();
                string message = System.IO.File.ReadAllText("Txt/victoire.txt");
                foreach (var line in message.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None))
                {
                    messageInit.Add(line);
                }
                _win = true;
                _mapLevel = 0;
            }

            // Lorsque le joueur a fini ou perdu la partie, il peut en recommencer une en cliquant sur R (Restart)
            if ((_win) || (_gameOver))
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.R)
                    {
                        systDeCommande.EndGame(keyPress.Key);
                        _majEcran = true;
                    }
                    else
                    {
                        systDeCommande.EndGame(keyPress.Key);
                        _majEcran = true;
                    }
                }

            }

            // Lorsque le visuel combat est affichée, le joueur peut opérer différentes actions
            if (_displayCombat)
            {
                if (keyPress != null)
                {
                    messageInit.Add("            ");
                    if (keyPress.Key == RLKey.Q)             // A pour attaquer (Q car Roguesharp utilise un clavier QWERTY)
                    {
                        messageInit.Add("Vous avez choisi d'attaquer");
                        _choixCombat = systDeCommande.ChoiceCombat(keyPress.Key);
                        _endTurnCombat = true;
                    }
                    else if (keyPress.Key == RLKey.D)            // D pour défendre
                    {
                        messageInit.Add("Vous avez choisi de defendre");
                        _choixCombat = systDeCommande.ChoiceCombat(keyPress.Key);
                        _endTurnCombat = true;
                    }
                    else if (keyPress.Key == RLKey.P)               // P pour guérir
                    {
                        messageInit.Add("Vous avez choisi de prendre une potion");
                        _choixCombat = systDeCommande.ChoiceCombat(keyPress.Key);
                        _endTurnCombat = true;
                    }
                    else if (keyPress.Key == RLKey.F)            // F pour fuir
                    {
                        messageInit.Add("Vous avez choisi de fuir");
                        messageLog.Add($"{player1.Name} a choisi de fuir");
                        _choixCombat = systDeCommande.ChoiceCombat(keyPress.Key);
                        _endTurnCombat = true;
                    }
                    else
                    {
                        _choixCombat = systDeCommande.ChoiceCombat(keyPress.Key);
                    }
                }
            }

            // Si le joueur n'as pas choisi ses équipements (sa classe)
            if (!_classeChoisie)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.O)        // le joueur décide de jouer offensif
                    {
                        messageLog.Add("Vous avez choisi l'equipement offensif");
                        _classeChoisie = systDeCommande.ChoiceClasse(keyPress.Key);
                    }
                    else if (keyPress.Key == RLKey.D)       // le joueur décide de jouer défensif
                    {
                        messageLog.Add("Vous avez choisi l'equipement defensif !");
                        _classeChoisie = systDeCommande.ChoiceClasse(keyPress.Key);
                    }
                    else if (keyPress.Key == RLKey.F)       // le joueur d
                    {
                        messageLog.Add("Vous avez choisi l'equipement furtif !");
                        _classeChoisie = systDeCommande.ChoiceClasse(keyPress.Key);
                    }
                    else        // une autre touche a été pressée
                    {
                        _classeChoisie = systDeCommande.ChoiceClasse(keyPress.Key);
                    }
                    messageLog.Add("QUE LA PARTIE COMMENCE !");
                }
            }

            // Lorsque le joueur a choisi ses equipements, on gère le déplacement du joueur et des ennemis
            if (_classeChoisie)
            {
                if (systDeCommande.IsPlayerTurn)
                {
                    if ((!_displayCombat) && (!_gameOver) && (!_win))
                    {
                        if (keyPress != null)
                        {
                            if (keyPress.Key == RLKey.Semicolon)
                            {
                                messageLog.Add("Affichage du menu");
                                messageInit.Add("Affichage du menu");
                                _displayMenu = systDeCommande.DisplayMenu(keyPress.Key);
                                _majEcran = true;
                            }
                            else if (keyPress.Key == RLKey.X)
                            {
                                messageLog.Add("Sortie du menu");
                                messageInit.Add("Sortie du menu");
                                _displayMenu = systDeCommande.DisplayMenu(keyPress.Key);
                                _majEcran = true;
                            }
                            else if (keyPress.Key == RLKey.Period)
                            {
                                if (citymap1.CanTpToNextLevel())
                                {
                                    player1.carte = false;
                                    MapGenerator mapGenerator = new MapGenerator(_largeurMap, _hauteurMap, 20, 20 - _mapLevel, 10 - _mapLevel, ++_mapLevel);
                                    citymap1 = mapGenerator.CreateMap();
                                    messageLog = new MessageLog();
                                    systDeCommande = new SystemeDeCommande();
                                    _mainConsole.Title = $"Roguepunk 2021- Level {_mapLevel}";
                                    _joueurDeplace = true;
                                }
                                _majEcran = true;
                            }

                            if (!_displayMenu)
                            {
                                if (keyPress.Key == RLKey.Up)
                                {
                                    _joueurDeplace = systDeCommande.MovePlayer(keyPress.Key);
                                }
                                else if (keyPress.Key == RLKey.Down)
                                {
                                    _joueurDeplace = systDeCommande.MovePlayer(keyPress.Key);
                                }
                                else if (keyPress.Key == RLKey.Left)
                                {
                                    _joueurDeplace = systDeCommande.MovePlayer(keyPress.Key);
                                }
                                else if (keyPress.Key == RLKey.Right)
                                {
                                    _joueurDeplace = systDeCommande.MovePlayer(keyPress.Key);
                                }
                                else
                                {
                                    _joueurDeplace = systDeCommande.UtiliserKit(keyPress.Key);
                                    _majEcran = true;
                                }
                            }
                        }
                    }

                    if (_joueurDeplace && _classeChoisie || _displayCombat)
                    {
                        _majEcran = true;
                        systDeCommande.EndPlayerTurn();
                    }
                }
                else        // si ca n'est pas au tour du joueur
                {
                    systDeCommande.ActivateEnemy();
                    _majEcran = true;
                }
            }


        }
        // Event handler for RLNET's Render event
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // permet de ne pas redessiner toutes les consoles à chaque fois
            if ((_majEcran) || (_findujeu))
            {
                // on nettoie les consoles
                _mapConsole.Clear();
                _statsConsole.Clear();
                _logConsole.Clear();

                // permet de séparer en deux parties la sous-console des stats
                _statsConsole.SetBackColor(0, 0, _largeurStats, 21, BigColors.Colors.P1_StatsPersoConsole);
                _statsConsole.SetBackColor(0, 21, _largeurStats, _hauteurStats, BigColors.Colors.P1_statsEnnemiesConsole);

                // console de titre 
                _titreConsole.SetBackColor(0, 0, _largeurTitre, _hauteurTitre, BigColors.Palette.DbDark);

                // dessine la console principale : soit on affiche un message, soit la map
                if (!_classeChoisie || _displayMenu || _displayCombat || _gameOver || _win || _findujeu)
                {
                    messageInit.Draw(_mapConsole, _largeurMap, _hauteurMap);
                }
                else
                {
                    citymap1.DrawMap(_mapConsole);
                }

                // on dessine les autres éléments en plus de la carte
                citymap1.DrawStats(_statsConsole);
                messageLog.Draw(_logConsole, _largeurLog, _hauteurLog);
                player1.Draw(_mapConsole, citymap1);
                player1.DrawStats(_statsConsole);

                // ajoute les sous-consoles aux coordonées renseignées dans la console principale
                RLConsole.Blit(_titreConsole, 0, 0, _largeurTitre, _hauteurTitre, _mainConsole, 0, 0);
                RLConsole.Blit(_mapConsole, 0, 0, _largeurMap, _hauteurMap, _mainConsole, 0, _hauteurMain - _hauteurMap - _hauteurLog);
                RLConsole.Blit(_logConsole, 0, 0, _largeurLog, _hauteurLog, _mainConsole, 0, _hauteurMain - _hauteurLog);
                RLConsole.Blit(_statsConsole, 0, 0, _largeurStats, _hauteurStats, _mainConsole, _largeurMap, 0);

                // on dessine la console principale
                _mainConsole.Draw();

                _majEcran = false;
            }

        }
    }
}

