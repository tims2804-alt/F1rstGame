namespace TextRPG.Enemies;

class BossVVG : Goblin
{
    public BossVVG(Random rnd) : base(rnd)
    {
        Name = "ВВГ (босс-гоблин)";
        HP = (int)Math.Round(30 * 2.0);
        Attack = (int)Math.Round(8 * 1.5);
        Defense = (int)Math.Round(3 * 1.2);
        CritChance = 0.15 + 0.10; // +10%
    }
}