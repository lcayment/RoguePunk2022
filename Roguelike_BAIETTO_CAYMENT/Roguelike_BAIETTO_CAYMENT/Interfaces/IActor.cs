namespace Roguelike_BAIETTO_CAYMENT.Interfaces
{
    public interface IActor
    {
        string Name { get; set; }           // nom de l'acteur
        int Attack { get; set; }            // nombre de dégats
        int AttackChance { get; set; }      // probabilité que l'attaque soit un succès
        int Fov { get; set; }               // correspond au champ dans lequel l'acteur peut detecter d'autres acteurs
        int Defense { get; set; }           // nombre de points de défense
        int DefenseChance { get; set; }     // probabilité que la défense soit un succès
        int Credit { get; set; }            // nombre de credits que l'acteur a
        int Health { get; set; }            // nombre de points de vie actuel
        int MaxHealth { get; set; }         // nombre de points de vie max
        int Speed { get; set; }             // vitesse de l'acteur (voir si on fait comme dans le tuto ou pas)
        int Niveau { get; set; }            // niveau de l'acteur
        int Exp { get; set; }               // experience
        string Classe { get; set; }         // equipement choisi (offensif, defensif, furtif)
        bool carte { get; set; }            // carte d'acces (true/false)
        int nbKitRepa { get; set; }         // nombre de kit de réparation
        bool CanAttack { get; set; }        // peut attaquer (true/false)
        bool IsDead { get; set; }           // est mort (true/false)
    }
}
