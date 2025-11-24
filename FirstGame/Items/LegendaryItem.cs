namespace TextRPG.Items;

public class LegendaryItem : Item
{
    public int Damage { get; set; }
    public int Defense { get; set; }
    public int HpBonus { get; set; }

    public LegendaryItem(string name, int damage, int defense, int hpBonus)
        : base(name, damage)
    {
        Damage = damage;
        Defense = defense;
        HpBonus = hpBonus;
    }
}