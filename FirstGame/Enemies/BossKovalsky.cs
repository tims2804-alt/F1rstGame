namespace TextRPG.Enemies;

class BossKovalsky : Skeleton
{
    public BossKovalsky(Random rnd) : base(rnd)
    {
        Name = "Ковальский (босс-скелет)";
        HP = (int)Math.Round(35 * 2.5);
        Attack = (int)Math.Round(10 * 1.3);
        Defense = (int)Math.Round(5 * 1.4);
        IgnoresArmor = true;
    }
}