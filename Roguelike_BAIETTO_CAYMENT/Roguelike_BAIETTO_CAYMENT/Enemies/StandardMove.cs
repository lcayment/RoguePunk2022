using Roguelike_BAIETTO_CAYMENT.Interfaces;
using Roguelike_BAIETTO_CAYMENT.Systems;
using Roguelike_BAIETTO_CAYMENT.Core;
using RogueSharp;
using RLNET;

namespace Roguelike_BAIETTO_CAYMENT.Enemies
{
    // StandardMove hérite de l'interface IBehavior
    public class StandardMove : IBehavior
    {
        public bool Act(Enemy enemy, SystemeDeCommande commandSystem)
        {
            CityMap cityMap = Game.citymap1;
            Player player = Game.player1;
            FieldOfView enemyFov = new FieldOfView(cityMap);

            // Si l'ennemi a été alerté, on utlise sa valeur de FoV pour savoir s'il peut se battre avec le joueur
            // On ajoute aussi un message au log
            if (!enemy.TurnsAlerted.HasValue)
            {
                enemyFov.ComputeFov(enemy.PosX, enemy.PosY, enemy.Fov, true);
                if (enemyFov.IsInFov(player.PosX, player.PosY) && !Game._displayCombat)
                {
                    //Game.messageLog.Add($"{enemy.Name} veut se battre avec {player.Name}");
                    enemy.TurnsAlerted = 1;
                }
                else if (!enemyFov.IsInFov(player.PosX, player.PosY) && !Game._displayCombat)
                {
                    enemy.CanAttack = true;     // lorsque le joueur quitte le FoV de l'ennemi, ce dernier peut réattaquer
                }
            }

            // Une fois que l'ennemi a été alerté
            if (enemy.TurnsAlerted.HasValue)
            {
                // On vérifie que les cellules sont walkable
                cityMap.SetIsWalkable(enemy.PosX, enemy.PosY, true);
                cityMap.SetIsWalkable(player.PosX, player.PosY, true);

                PathFinder pathFinder = new PathFinder(cityMap);
                Path path = null;

                try
                {
                    // On cherche le chemin le plus court
                    path = pathFinder.ShortestPath(
                    cityMap.GetCell(enemy.PosX, enemy.PosY),
                    cityMap.GetCell(player.PosX, player.PosY));
                }
                catch (PathNotFoundException)
                {
                    // L'ennemi a vu le joueur mais n'as pas réussi à l'atteindre
                    // A cause d'un obstacle, il attend donc son tour
                    Game.messageLog.Add($"{enemy.Name} attend son tour");
                }

                // Les cellules sur lesquelles sont l'ennemi et le joueur ne sont pas walkable
                cityMap.SetIsWalkable(enemy.PosX, enemy.PosY, false);
                cityMap.SetIsWalkable(player.PosX, player.PosY, false);

                // Si il y a un chemin, on bouge le enemi
                if (path != null)
                {
                    try
                    {
                        commandSystem.MoveEnemy(enemy, path.StepForward());
                    }
                    catch (NoMoreStepsException)
                    {
                        Game.messageLog.Add($"{enemy.Name} bip de frustration");
                    }
                }

                enemy.TurnsAlerted++;

                // Tous les 15 tours, l'ennemi perd son alerte
                // Sauf si le joueur est resté dans son FoV
                if (enemy.TurnsAlerted > 15)
                {
                    enemy.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}

