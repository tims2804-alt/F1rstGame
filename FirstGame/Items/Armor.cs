namespace TextRPG.Items;

public class Armor : Item
{
    public int Defense { get; set; }

    public Armor(string name, int defense)
        : base(name, defense)
    {
        Defense = defense;
    }
}