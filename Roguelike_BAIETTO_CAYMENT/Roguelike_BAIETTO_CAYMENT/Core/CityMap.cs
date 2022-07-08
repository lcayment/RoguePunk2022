using RLNET;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.BigColors;
using System.Collections.Generic;
using System.Linq;
using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.Tresor;

namespace Roguelike_BAIETTO_CAYMENT.Core
{
    public class CityMap : Map
    {
        public List<Rectangle> _pieces;         // pieces qui composent la map
        public readonly List<Enemy> _enemy;     // ennemies qui remplissent la map
        public List<Door> _doors { get; set; }  // portes
        private readonly List<TresorRamassable> _tresorRamassable;  // trésor ramassable (Attaque, Defense, Kit, Carte)
        public Teleporteur TpUp { get; set; }       // Téléporteur vers l'étage supérieur
        public Teleporteur TpDown { get; set; }     // Téléporteur vers l'étage inférieur

        // Constructeur
        public CityMap()
        {
            _pieces = new List<Rectangle>();
            _enemy = new List<Enemy>();
            _doors = new List<Door>();
            _tresorRamassable = new List<TresorRamassable>();
        }

        // ####################################################################################################################
        //                PORTES
        // ####################################################################################################################
        // Méthode qui indique si il y a une porte ou non
        public Door GetDoor(int x, int y)
        {
            return _doors.SingleOrDefault(d => d.PosX == x && d.PosY == y);
        }

        // Le joueur ouvre la porte en position (x,y)
        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)       // si il y a une porte et qu'elle n'a pas été ouverte
            {
                door.IsOpen = true;     // on l'ouvre
                var cell = GetCell(x, y);
                // Une fois que la porte est ouverte, elle ne bloque plus le FoV
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);

                Game.messageLog.Add($"{actor.Name} a ouvert une porte");
            }
        }

        // ####################################################################################################################
        //                JOUEUR
        // ####################################################################################################################
        // Ajoute le joueur sur la carte à une position donnée
        public void AddPlayer(Player player)
        {
            Game.player1 = player;
            SetIsWalkable(player.PosX, player.PosY, false);
            UpdatePlayerFieldOfView();
            Game.schedulingSystem.Add(player);
        }

        // ####################################################################################################################
        //                ENNEMIS
        // ####################################################################################################################
        // Ajoute les ennemies sur la carte à une position donnée
        public void AddEnemy(Enemy enemy)
        {
            _enemy.Add(enemy);
            // La cellule ne doit pas être walkable
            SetIsWalkable(enemy.PosX, enemy.PosY, false);
            Game.schedulingSystem.Add(enemy);
        }

        // Supprime les ennemis de la carte et rend la cellule walkable de nouveau
        public void RemoveEnemy(Enemy enemy)
        {
            _enemy.Remove(enemy);

            // After removing the enemy from the map, make sure the cell is walkable again
            SetIsWalkable(enemy.PosX, enemy.PosY, true);
            Game.schedulingSystem.Remove(enemy);        // on supprime aussi l'ennemi du scheduler
        }

        // Retourne une valeur par défaut s'il n'y a pas d'ennemis ou un élement de la liste Enemy si il y a un ennemi dans la cellule où on souhaite aller
        public Enemy IsEnemy(int x, int y)
        {
            return _enemy.FirstOrDefault(m => m.PosX == x && m.PosY == y);
        }

        // ####################################################################################################################
        //                TRESOR RAMASSABLES
        // ####################################################################################################################
        // Ajoute un trésor en position (x,y)
        public void AddTresor(int x, int y, ITresor treasure)
        {
            _tresorRamassable.Add(new TresorRamassable(x, y, treasure));
        }

        // Création de la carte pour accèder au téléporteur et ajout dans la liste
        public void CreateCarteMag(int x, int y)
        {
            AddTresor(x, y, new CarteMag());
        }

        // Création du point de défense ramassable et ajout dans la liste
        public void AddArmure(int x, int y)
        {
            AddTresor(x, y, new RenfoArmure());
        }

        // Création du point de attaque ramassable et ajout dans la liste
        public void AddAttaque(int x, int y)
        {
            AddTresor(x, y, new RenfoAttaque());
        }

        // Permet de ramasser le trésor en marchant dessus
        private void RecupTresor(Actor actor, int x, int y)
        {
            List<TresorRamassable> treasureAtLocation = _tresorRamassable.Where(g => g.PosX == x && g.PosY == y).ToList();
            foreach (TresorRamassable treasurePile in treasureAtLocation)
            {
                if (treasurePile.Treasure.PickUp(actor))
                {
                    _tresorRamassable.Remove(treasurePile);
                }
            }
        }

        // ####################################################################################################################
        //                UTILITAIRES
        // ####################################################################################################################
        // On cherche pour une position aléatoire dans une piece qui est walkable (pour placer les ennemis et les trésors)
        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            // Retourne null si aucun point n'est trouvé
            return default(Point);
        }

        // On itère dans les cellules pour voir si elles sont walkable
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // Permet de choisir les symboles visibles sur la console
        private void SetSymboleCellule(RLConsole console, Cell cell)
        {
            // Deux cas : la cellule a deja été visité ou la cellule n'a pas été visitée
            // Cas 1 : la cellule n'a pas été visitée, on ne dessine rien
            if (!cell.IsExplored)
            {
                return; // on ne retourne rien
            }
            // Cas 2 : la cellule a été visitée
            if (IsInFov(cell.X, cell.Y))        // dans le FoV du joueur
            {
                if (cell.IsWalkable)  // aka pas un mur
                {
                    console.Set(cell.X, cell.Y, Colors.P1_FloorFov, Colors.P1_FloorBackgroundFov, '.');
                }
                else    // un mur
                {
                    console.Set(cell.X, cell.Y, Colors.P1_WallFov, Colors.P1_WallBackgroundFov, (char)176);
                }
            }
            else        // pas dans le gov du joueur
            {
                if (cell.IsWalkable)  // aka pas un mur
                {
                    console.Set(cell.X, cell.Y, Colors.P1_Floor, Colors.P1_FloorBackground, '.');
                }
                else    // un mur
                {
                    console.Set(cell.X, cell.Y, Colors.P1_Wall, Colors.P1_WallBackground, (char)176);
                }
            }
        }

        // Méthode appelée lorsque le joueur est déplacé
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.player1;

            // Génère le Fov en fonction de la position du joueur et la portée de son fov
            ComputeFov(player.PosX, player.PosY, player.Fov, true);

            // Marque les cellules qui ont été explorées
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        // Si l'acteur peut etre placé dans une cellule, retourne true, sinon false
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Seulement si la cellule est walkable
            if (GetCell(x, y).IsWalkable)
            {
                // Recupère l'objet si il est dessus
                RecupTresor(actor, x, y);

                // Les cellules sur lesquelles le joueur est déjà passé sont disponibles
                SetIsWalkable(actor.PosX, actor.PosY, true);

                // On ouvre la porte s'il y en a une
                OpenDoor(actor, x, y);

                // MAJ de la position de l'acteur
                actor.PosX = x;
                actor.PosY = y;
                // La nouvelle cellule, sur laquelle le joueur est, est disponible
                SetIsWalkable(actor.PosX, actor.PosY, false);

                // MAJ Fov
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        // Méthode qui permet de paramétrer la cellule comme étant walkable
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        // Determine si le joueur peut se téléporter au niveau supérieur
        public bool CanTpToNextLevel()
        {
            Player player = Game.player1;
            if (TpDown.PosX == player.PosX && TpDown.PosY == player.PosY)
            {
                if (player.carte)
                    return true;
                else
                {
                    Game.messageLog.Add("Vous n'avez pas la carte !! Vous ne pouvez pas utiliser ce teleporteur");
                    return false;
                }
            }
            else
            {
                Game.messageLog.Add("Vous n'etes pas sur le teleporteur !!");
                return false;
            }
        }

        // a chaque fois que la map est mise à jour, la méthode Draw est appelée et on redessine les éléments
        public void DrawMap(RLConsole mapConsole)
        {

            foreach (Cell cell in GetAllCells())
            {
                SetSymboleCellule(mapConsole, cell);
            }

            foreach (Enemy enemy in _enemy.ToList())
            {
                if (!enemy.IsDead)
                {
                    enemy.Draw(mapConsole, this);
                }
            }

            foreach (Door door in _doors)
            {
                door.Draw(mapConsole, this);
            }
            TpUp.Draw(mapConsole, this);
            TpDown.Draw(mapConsole, this);

            foreach (TresorRamassable treRama in _tresorRamassable)
            {
                IDessin drawableTreasure = treRama.Treasure as IDessin;
                drawableTreasure?.Draw(mapConsole, this);
            }

            foreach (Door door in _doors)
            {
                door.Draw(mapConsole, this);
            }

            TpUp.Draw(mapConsole, this);
            TpDown.Draw(mapConsole, this);
        }

        // On dessine les stats des ennemis
        public void DrawStats(RLConsole _statsConsole)
        {
            // Index pour dessiner les stats de l'ennemi
            int i = 0;

            // Après avoir dessiner les cellules, on dessine les ennemis
            foreach (Enemy enemy in _enemy.ToList())
            {
                // Si les ennemis sont dans le FoV du joueur, on dessine ses stats
                if (IsInFov(enemy.PosX, enemy.PosY))
                {
                    // i est utilisé pour donné l'emplacement en Y pour décaler les stats de chaque monstre
                    enemy.DrawStats(_statsConsole, i);
                    if (Game._endTurnCombat)
                    {
                        enemy.AttackEnemy();
                        Game._endTurnCombat = false;
                    }
                    i++;
                }
            }

        }

    }
}
