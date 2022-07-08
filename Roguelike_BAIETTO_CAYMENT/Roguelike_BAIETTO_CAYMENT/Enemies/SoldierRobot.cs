using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Core;
using RogueSharp.DiceNotation;


namespace Roguelike_BAIETTO_CAYMENT.Enemies
{
    // SoldierRobot hérite de Enemy
    public class SoldierRobot : Enemy
    {
        public static SoldierRobot Create(int level)
        {
            int health = 35;
            return new SoldierRobot
            {
                Attack = Dice.Roll("1D5") + level / 3,  // Entre 1 and 5 (+ level (increasing attack))
                AttackChance = Dice.Roll("20D3"),       // Entre 20 and 60
                Fov = 7,
                color = Colors.P1_Droid,
                Defense = Dice.Roll("1D3") + level / 3, // Entre 1 and 5 (+ level (increasing defense))
                DefenseChance = Dice.Roll("15D3"),      // Entre 15 and 45
                Credit = Dice.Roll("10D4"),             // Entre 10 and 40
                Health = health,
                MaxHealth = health,
                Name = "Robot soldat",
                Speed = 12,
                Exp = Dice.Roll("5D3"), // L'ennemi rapporte entre 5 et 15 pts d'exp
                Symbol = (char)17,
                CanAttack = true
            };
        }
    }
}
