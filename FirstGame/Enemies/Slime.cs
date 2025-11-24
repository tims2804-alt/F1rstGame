namespace TextRPG.Enemies;

class Slime : Enemy
{
    public Slime(Random rnd) : base(rnd)
    {
        Name = "Слизень";
        HP = 40;
        Attack = 5;
        Defense = 1;
    }
    public override int ReceiveDamage(int dmg)
    {
        int reduced = dmg - 2;

        int actual = Math.Max(1, reduced - Defense);
        HP -= actual;
        return actual;
    }
    
}