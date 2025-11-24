namespace TextRPG.Items;

public class Weapon : Item
{
    public int Damage { get; set; }

    public Weapon(string name, int damage)
        : base(name, damage)
    {
        Damage = damage;
    }
}