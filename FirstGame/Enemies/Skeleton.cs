namespace TextRPG.Enemies;

class Skeleton : Enemy
{
    public Skeleton(Random rnd) : base(rnd)
    {
        Name = "Скелет";
        HP = 35;
        Attack = 10;
        Defense = 5;
        IgnoresArmor = true;
    }
}