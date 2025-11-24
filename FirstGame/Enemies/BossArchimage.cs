namespace TextRPG.Enemies;

class BossArchimage : Mage
{
    public BossArchimage(Random rnd) : base(rnd)
    {
        Name = "Архимаг C++ (босс-маг)";
        HP = (int)Math.Round(25 * 1.8);
        Attack = (int)Math.Round(6 * 1.6);
        Defense = (int)Math.Round(2 * 1.1);
        FreezeChance = 0.20 + 0.10;
    }
}