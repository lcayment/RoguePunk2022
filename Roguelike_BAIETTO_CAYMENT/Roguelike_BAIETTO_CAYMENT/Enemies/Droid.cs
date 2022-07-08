using Roguelike_BAIETTO_CAYMENT.BigColors;
using Roguelike_BAIETTO_CAYMENT.Core;
using RogueSharp.DiceNotation;

namespace Roguelike_BAIETTO_CAYMENT.Enemies
{
    // Droid hérite de Enemy
    public class Droid : Enemy
    {
        public static Droid Create(int level)
        {
            int health = 25;      // Points de vie
            return new Droid
            {
                Attack = Dice.Roll("1D3") + level / 3,  // Entre 1 and 3 (+ level (increasing attack))
                AttackChance = Dice.Roll("25D3"),       // Entre 25 and 75
                Fov = 5,
                color = BigColors.Colors.P1_Droid,
                Defense = Dice.Roll("1D5") + level / 3, // Entre 1 and 5 (+ level (increasing defense))
                DefenseChance = Dice.Roll("10D4"),      // Entre 10 and 40
                Credit = Dice.Roll("5D5"),              // Entre 5 and 25
                Health = health,
                MaxHealth = health,
                Name = "Droid",
                Speed = 14,
                Exp = Dice.Roll("2D3"), // L'ennemi rapporte entre 2 et 6 pts d'exp
                Symbol = (char)16,
                CanAttack = true
            };
        }
    }
}
