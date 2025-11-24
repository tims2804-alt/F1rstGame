namespace TextRPG.Enemies;

class BossPestov : Skeleton
{
    public BossPestov(Random rnd) : base(rnd)
    {
        Name = "Пестов С-- (босс-скелет)";
        HP = (int)Math.Round(35 * 1.3);
        Attack = (int)Math.Round(10 * 1.8);
        Defense = (int)Math.Round(5 * 0.6);
        IgnoresArmor = true;
        FreezeChance = 0.20 + 0.15;
    }
}