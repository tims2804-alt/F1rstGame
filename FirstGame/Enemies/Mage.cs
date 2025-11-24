namespace TextRPG.Enemies;

class Mage : Enemy
{
    public Mage(Random rnd) : base(rnd)
    {
        Name = "Маг";
        HP = 25;
        Attack = 6;
        Defense = 2;
        FreezeChance = 0.20;
    }
}