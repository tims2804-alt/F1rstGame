namespace TextRPG.Enemies;

class Goblin : Enemy
{
    public Goblin(Random rnd) : base(rnd)
    {
        Name = "Гоблин";
        HP = 30;
        Attack = 8;
        Defense = 3;
        CritChance = 0.15;
    }
}