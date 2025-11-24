using TextRPG.Items;

namespace TextRPG.Entities;

public class Player
{
    public int MaxHP { get; private set; }
    public int HP { get; set; }
    public Weapon Weapon { get; set; }
    public Armor Armor { get; set; }
    public LegendaryItem LegendaryItem { get; set; }
    public bool IsFrozen { get; set; } 

    public Player(int maxHp, Weapon weapon, Armor armor)
    {
        MaxHP = maxHp;
        HP = maxHp;
        Weapon = weapon;
        Armor = armor;
    }

    public int AttackValue =>
        LegendaryItem != null ? LegendaryItem.Damage : (Weapon?.Damage ?? 1);

    public int ArmorValue =>
        LegendaryItem != null ? LegendaryItem.Defense : (Armor?.Defense ?? 0);

    public int TotalMaxHP =>
        LegendaryItem != null ? MaxHP + LegendaryItem.HpBonus : MaxHP;

    public void HealToFull()
    {
        HP = TotalMaxHP;
    }

    public bool IsAlive => HP > 0;
}