using TextRPG.Entities;

namespace TextRPG.Enemies;

public abstract class Enemy
    {
        public string Name { get; protected set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public bool IgnoresArmor { get; protected set; } = false;
        public double CritChance { get; protected set; } = 0.0; // 0..1
        public double CritMultiplier { get; protected set; } = 1.5;
        public double FreezeChance { get; protected set; } = 0.0; // chance to freeze player (skip next turn)

        public bool IsAlive => HP > 0;

        protected Random rnd;

        public Enemy(Random random)
        {
            rnd = random;
        }

        public virtual int ReceiveDamage(int dmg)
        {
            int actual = Math.Max(1, dmg - Defense); // поощряем урон хотя бы 1
            HP -= actual;
            return actual;
        }

        public virtual int AttackPlayer(Player player, bool playerIsDefending)
        {
            double dmg = Attack;

            // крит
            if (CritChance > 0 && rnd.NextDouble() < CritChance)
            {
                dmg *= CritMultiplier;
                Console.WriteLine($"Враг {Name} наносит критический удар!");
            }

            // проверка уклонения при защите игрока
            if (playerIsDefending)
            {
                double dodgeRoll = rnd.NextDouble();
                if (dodgeRoll < 0.40)
                {
                    Console.WriteLine("Игрок успешно уклонился от атаки!");
                    return 0;
                }
            }

            // вычисляем уменьшение урона за счёт брони игрока
            double effectiveArmor = IgnoresArmor ? 0 : player.ArmorValue;
            if (playerIsDefending)
            {
                // блок уменьшает урон на 70–100% от характеристики защиты
                double factor = 0.7 + rnd.NextDouble() * 0.3;
                double reduction = effectiveArmor * factor;
                dmg -= reduction;
                Console.WriteLine($"Защита снизила урон на {Math.Round(reduction, 1)}.");
            }
            else
            {
                // обычный учёт брони
                dmg -= effectiveArmor;
            }

            int final = Math.Max(0, (int)Math.Round(dmg));
            player.HP -= final;
            Console.WriteLine($"Враг {Name} наносит {final} урона игроку.");
            // проверка наложения заморозки
            if (FreezeChance > 0 && rnd.NextDouble() < FreezeChance)
            {
                player.IsFrozen = true;
                Console.WriteLine("Игрок поражён заморозкой и пропустит следующий ход!");
            }

            return final;
        }
        
        public override string ToString()
        {
            return $"{Name} (HP:{HP}, Atk:{Attack}, Def:{Defense})";
        }
        
        
    }