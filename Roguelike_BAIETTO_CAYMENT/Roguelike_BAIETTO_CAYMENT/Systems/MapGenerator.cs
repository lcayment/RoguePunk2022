using System.Collections.Generic;
using System.Linq;
using System;
using RogueSharp;
using Roguelike_BAIETTO_CAYMENT.Core;
using RogueSharp.DiceNotation;
using Roguelike_BAIETTO_CAYMENT.Enemies;
using Roguelike_BAIETTO_CAYMENT.Tresor;

namespace Roguelike_BAIETTO_CAYMENT.Systems
{
    // Permet de générer différentes salles de manière aléatoire en prenant en compte des paramètres de tailles
    class MapGenerator
    {
        private readonly int _widthMap;         // largeur de la map
        private readonly int _heightMap;        // hauteur de la map
        private readonly int _nbRoomMax;        // nombre de pièces maximum sur la map
        private readonly int _sizeRoomMax;      // taille maximum des salles
        private readonly int _sizeRoomMin;      // taille minimum des salles
        private readonly int _mapLevel;         // niveau de la map 

        private readonly TresorGenerator _tresorGenerator;
        private readonly ItemGenerator _itemGenerator;

        private readonly CityMap _map;

        // constructeur
        public MapGenerator(int width, int height, int nbRoomMax, int sizeRoomMax, int sizeRoomMin, int mapLevel)
        {
            _widthMap = width;
            _heightMap = height;
            _nbRoomMax = nbRoomMax;
            _sizeRoomMax = sizeRoomMax;
            _sizeRoomMin = sizeRoomMin;
            _mapLevel = mapLevel;
            _map = new CityMap();
            _tresorGenerator = new TresorGenerator();
            _itemGenerator = new ItemGenerator();
        }

        // On ajoute les ennemies de manière aléatoire
        // Dans chaque salle on lance un dès pour savoir si il y aura des ennemies
        // Si il y a des ennemies, on lance un dès pour savoir combien (entre 1 et 3)
        private void PlaceEnemies()
        {
            int nbpieces = _map._pieces.Count();
            for (int i = 1; i < nbpieces - 1; i++) // Aucun ennemi dans la 1ere et dernier salle
            {
                // 60% de chance d'avoir des Ennemies
                if (Dice.Roll("1D10") < 7)
                {
                    // Entre 1 et 3 Ennemies
                    var numberOfEnemies = Dice.Roll("1D3");
                    for (int j = 0; j < numberOfEnemies; j++)
                    {
                        // On récupère une position walkable pour placer les ennemis
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(_map._pieces[i]);

                        // Si il n'y a pas de place, on ne crée pas l'ennemi
                        if ((randomRoomLocation != null))
                        {
                            if (j % 2 == 0)
                            {
                                // le niveau des droides est dépendant du niveau de la map, leur vitesse aussi
                                var enemy = Droid.Create(_mapLevel);
                                Point location = _map.GetRandomWalkableLocationInRoom(_map._pieces[i]);
                                if ((enemy.Speed - _mapLevel / 2) < 1) // Permet d'augmenter progressivement la vitesse des ennemis sans passer sous 0
                                {
                                    enemy.Speed = 1;
                                }
                                enemy.PosX = location.X;
                                enemy.PosY = location.Y;
                                _map.AddEnemy(enemy);
                            }
                            else
                            {
                                // le niveau des soldierRobot est dépendant du niveau de la map, leur vitesse aussi
                                var enemy2 = SoldierRobot.Create(_mapLevel);
                                Point location2 = _map.GetRandomWalkableLocationInRoom(_map._pieces[i]);
                                if ((enemy2.Speed - _mapLevel / 2) < 1) // Permet d'augmenter progressivement la vitesse des ennemis sans passer sous 0
                                {
                                    enemy2.Speed = 1;
                                }
                                enemy2.PosX = location2.X;
                                enemy2.PosY = location2.Y;
                                _map.AddEnemy(enemy2);
                            }
                        }
                    }
                }
            }
        }

        // On place aléatoirement les kit de réparation
        private void PlaceKit()
        {
            foreach (var room in _map._pieces)
            {
                // 15% de chance d'avoir un kit de repa dans la salle
                if (Dice.Roll("1D100") <= 15)
                {
                    if (_map.DoesRoomHaveWalkableSpace(room))
                    {
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                        if ((randomRoomLocation != null))
                        {
                            Objet Kit;
                            try
                            {
                                Kit = _itemGenerator.CreateItem();
                            }
                            catch (InvalidOperationException)
                            {
                                // Plus de kit à ajouter
                                return;
                            }
                            Point location = _map.GetRandomWalkableLocationInRoom(room);
                            _map.AddTresor(location.X, location.Y, Kit);
                        }
                    }
                }
            }
        }

        // On place aléatoirement les trèsors
        private void PlaceTresor()
        {
            int nbpieces = _map._pieces.Count();
            for (int i = 1; i < nbpieces; i++)
            //foreach (var room in _map._pieces)
            {
                if (_map.DoesRoomHaveWalkableSpace(_map._pieces[i]))
                {
                    Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(_map._pieces[i]);
                    if (randomRoomLocation != null)
                    {
                        Core.Tresor tresor;
                        try
                        {
                            tresor = _tresorGenerator.CreateTresor();
                        }
                        catch (InvalidOperationException)
                        {
                            // plus d'équipement à placer donc on arrête d'en ajouter pour ce niveau
                            return;
                        }
                        Point location = _map.GetRandomWalkableLocationInRoom(_map._pieces[i]);
                        _map.AddTresor(location.X, location.Y, tresor);
                    }
                }
            }
        }

        // On trouve le centre de la premiere salle créée et on place le joueur dedans
        private void PlacePlayer()
        {
            Player player = Game.player1;
            if (player == null)
            {
                player = new Player();
            }

            player.PosX = _map._pieces[0].Center.X;
            player.PosY = _map._pieces[0].Center.Y;

            _map.AddPlayer(player);
        }

        // création d'une map qui remplit l'intégralité de la sous-console map
        public CityMap CreateMap()
        {
            // chaque cellule est initialisée comme walkable, transparent et explored
            _map.Initialize(_widthMap, _heightMap);

            // On tente de créer autant de pièce que le nombre max spécifié
            for (int r = _nbRoomMax; r > 0; r--)
            {
                // Determine la taille et la position de la salle de manière aléatoire
                int roomWidth = Game.Random.Next(_sizeRoomMin, _sizeRoomMax);
                int roomHeight = Game.Random.Next(_sizeRoomMin, _sizeRoomMax);
                int roomXPosition = Game.Random.Next(0, _widthMap - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _heightMap - roomHeight - 1);

                // Chaque pièce est représentée comme un rectangle
                var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

                // On vérifie si des pièces s'entrecoupent
                bool newRoomIntersects = _map._pieces.Any(room => newRoom.Intersects(room));

                // Si aucune ne s'entrecoupent on continue d'en ajouter
                if (!newRoomIntersects)
                {
                    _map._pieces.Add(newRoom);
                }
            }

            // Don't do anything with the first room, so start at r = 1 instead of r = 0
            for (int r = 1; r < _map._pieces.Count; r++)
            {
                // For all remaing rooms get the center of the room and the previous room
                int previousRoomCenterX = _map._pieces[r - 1].Center.X;
                int previousRoomCenterY = _map._pieces[r - 1].Center.Y;
                int currentRoomCenterX = _map._pieces[r].Center.X;
                int currentRoomCenterY = _map._pieces[r].Center.Y;

                // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            // On appelle CreateRoom et CreateDoors pour chaque rectangle (pièces) qu'on veut placer
            foreach (Rectangle room in _map._pieces)
            {
                CreateRoom(room);
                CreateDoors(room);

            }

            // On appelle les méthodes pour placer les éléments
            CreateTeleporteur();
            PlacePlayer();
            PlaceEnemies();
            PlaceTresor();
            PlaceKit();

            return _map;
        }

        // Permet de créer une pièce avec les propriétés suvantes : transparent, walkable et explored
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, true);
                }
            }

        }

        // On crée un tunnel parallèle à l'axe X (horizontalement)
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        // On crée un tunnel parallèle à l'axe Y (verticalement)
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }

        private void CreateDoors(Rectangle room)
        {
            // The the boundries of the room
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            // Put the rooms border cells into a list
            List<ICell> borderCells = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            // Go through each of the rooms border cells and look for locations to place doors.
            foreach (Cell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    // A door must block field-of-view when it is closed.
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map._doors.Add(new Door
                    {
                        PosX = cell.X,
                        PosY = cell.Y,
                        IsOpen = false
                    });
                }
            }
        }

        // Checks to see if a cell is a good candidate for placement of a door
        private bool IsPotentialDoor(Cell cell)
        {
            // If the cell is not walkable
            // then it is a wall and not a good place for a door
            if (!cell.IsWalkable)
            {
                return false;
            }

            // Store references to all of the neighboring cells 
            ICell right = _map.GetCell(cell.X + 1, cell.Y);
            ICell left = _map.GetCell(cell.X - 1, cell.Y);
            ICell top = _map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = _map.GetCell(cell.X, cell.Y + 1);

            // Make sure there is not already a door here
            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            // This is a good place for a door on the left or right side of the room
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            // This is a good place for a door on the top or bottom of the room
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }
            return false;
        }

        // Création des téléporteurs au centre des pièces
        private void CreateTeleporteur()
        {
            _map.TpUp = new Teleporteur
            {
                PosX = _map._pieces.First().Center.X + 1,
                PosY = _map._pieces.First().Center.Y,
                IsDown = true
            };
            _map.TpDown = new Teleporteur
            {
                PosX = _map._pieces.Last().Center.X,
                PosY = _map._pieces.Last().Center.Y,
                IsDown = false
            };
        }

    }
}
