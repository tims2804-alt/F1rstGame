using System;
using System.Collections.Generic;
using System.Threading;

namespace TextRPG
{
    abstract class Item
    {
        public string Name { get; set; }
    }

    class Weapon : Item
    {
        
        public int Damage { get; set; }

        public Weapon(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }
    }

    class Armor : Item
    {
        public int Defense { get; set; }

        public Armor(string name, int defense)
        {
            Name = name;
            Defense = defense;
        }
    }

    class LegendaryItem : Item
    {
        public int Damage { get; set; }
        public int Defense { get; set; }
        public int HpBonus { get; set; }

        public LegendaryItem(string name, int damage, int defense, int hpBonus)
        {
            Name = name;
            Damage = damage;
            Defense = defense;
            HpBonus = hpBonus;
        }
    }


    class Player
    {
        public int MaxHP { get; private set; }
        public int HP { get; set; }
        public Weapon Weapon { get; set; }
        public Armor Armor { get; set; }
        public LegendaryItem LegendaryItem { get; set; }
        public bool IsFrozen { get; set; } = false;

        public Player(int maxHp, Weapon weapon, Armor armor)
        {
            MaxHP = maxHp;
            HP = maxHp;
            Weapon = weapon;
            Armor = armor;
        }

        public int AttackValue
        {
            get
            {
                if (LegendaryItem != null)
                    return LegendaryItem.Damage; // если есть легендарка — она даёт урон
                return Weapon?.Damage ?? 1; // иначе обычное оружие
            }
        }

        public int ArmorValue
        {
            get
            {
                if (LegendaryItem != null)
                    return LegendaryItem.Defense; // если есть легендарка — она даёт защиту
                return Armor?.Defense ?? 0; // иначе обычная броня
            }
        }

// 💖 теперь здоровье учитывает бонус от легендарного предмета
        public int TotalMaxHP
        {
            get
            {
                if (LegendaryItem != null)
                    return MaxHP + LegendaryItem.HpBonus;
                return MaxHP;
            }
        }

        public void HealToFull()
        {
            HP = TotalMaxHP;
        }

        public bool IsAlive => HP > 0;
    }

    // Враги
    abstract class Enemy
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

    // Конкретные враги
    class Goblin : Enemy
    {
        public Goblin(Random rnd) : base(rnd)
        {
            Name = "Гоблин";
            HP = 30;
            Attack = 8;
            Defense = 3;
            CritChance = 0.15;
        }
    }

    class Skeleton : Enemy
    {
        public Skeleton(Random rnd) : base(rnd)
        {
            Name = "Скелет";
            HP = 35;
            Attack = 10;
            Defense = 5;
            IgnoresArmor = true;
        }
    }

    class Mage : Enemy
    {
        public Mage(Random rnd) : base(rnd)
        {
            Name = "Маг";
            HP = 25;
            Attack = 6;
            Defense = 2;
            FreezeChance = 0.20;
        }
    }

    // Боссы
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

    class Game
    {
        private List<(Func<Enemy> createEnemy, double weight)> enemyWeights;

        private void InitializeEnemyWeights()
        {
            enemyWeights = new List<(Func<Enemy>, double)>
            {
                (() => new Goblin(rnd), 0.5),
                (() => new Skeleton(rnd), 0.25),
                (() => new Mage(rnd), 0.25)
            };
        }

        private Random rnd = new Random();
        private Player player;
        private int turn = 0;
        private int currentWave = 0;

        public Game()
        {
            
            var starterWeapon = new Weapon("Деревянный меч", 6);
            var starterArmor = new Armor("Кожаная броня", 3);
            player = new Player(100, starterWeapon, starterArmor);
            InitializeEnemyWeights();
        }
        private AchievementTracker achievements = new AchievementTracker();

        public void Start()
        {
            Console.WriteLine("Добро пожаловать в пошаговую текстовую игру!");
            Console.WriteLine("Игрок начинает с полной жизни и начальным экипировкой.");
            Console.WriteLine("Нажмите Enter для начала.");
            Console.ReadLine();

            while (player.IsAlive)
            {
                turn++;
                currentWave = turn;
                Console.WriteLine($"=== Ход {turn} ===");
                Console.WriteLine(
                    $"Игрок: HP {player.HP}/{player.MaxHP}, Оружие: {player.Weapon.Name} (Atk {player.Weapon.Damage}), Доспехи: {player.Armor.Name} (Def {player.Armor.Defense})");

                Enemy enemy = null;
                bool isBossEncounter = (turn % 10 == 0);

                if (isBossEncounter)
                {
                    if (currentWave > 10)
                    {
                        enemy = GetBossForWave(currentWave);
                        Console.WriteLine($"Встреча! Усиленный босс (волна {currentWave}): {enemy}");
                    }
                    else
                    {
                        enemy = GetRandomBoss();
                        Console.WriteLine($"Встреча! Это первый босс (волна {currentWave}): {enemy}");
                    }
                }
                else
                {
                    if (rnd.NextDouble() < 0.5)
                    {
                        OpenChest();
                        if (!player.IsAlive) break;
                        Console.WriteLine("Ход завершается после сундука.");
                        ContinueOrPause();
                        continue;
                    }

                    enemy = GetEnemyForWave(currentWave);
                    Console.WriteLine($"Враждебная встреча: {enemy}");
                }

                // Бой
                bool battleOver = false;
                bool playerDefendingThisTurn = false; // применимо к текущему вражескому удару
                while (!battleOver && player.IsAlive && enemy.IsAlive)
                {
                    // Проверка, заморожен ли игрок
                    if (player.IsFrozen)
                    {
                        Console.WriteLine("Игрок заморожен и пропускает ход!");
                        player.IsFrozen = false; // пропускает только один ход
                    }
                    else
                    {
                        // Ход игрока: выбор Атака или Защита
                        Console.WriteLine("\nВыберите действие: 1) Атаковать  2) Защищаться");
                        string choice = Console.ReadLine();
                        if (choice == "2")
                        {
                            playerDefendingThisTurn = true;
                            Console.WriteLine(
                                "Игрок занимает оборонительную стойку (40% шанс уклониться, иначе блок 70–100% от брони).");
                        }
                        else
                        {
                            playerDefendingThisTurn = false;
                            // Атака
                            int playerDmg = player.AttackValue;
                            int dealt = enemy.ReceiveDamage(playerDmg);
                            Console.WriteLine(
                                $"Вы атаковали {enemy.Name} и нанесли {dealt} урона. (Оставшееся HP врага: {Math.Max(0, enemy.HP)})");
                            if (!enemy.IsAlive)
                            {
                                Console.WriteLine($"Враг {enemy.Name} повержен!");
                                achievements.AddKill();
                                if (enemy is BossVVG || enemy is BossKovalsky || enemy is BossArchimage || enemy is BossPestov)
                                    achievements.AddBossKill();
                                battleOver = true;
                                break;
                                
                            }
                        }
                    }

                    // Ход врага (если он жив)
                    if (enemy.IsAlive)
                    {
                        enemy.AttackPlayer(player, playerIsDefending: playerDefendingThisTurn);
                        Console.WriteLine($"HP игрока: {Math.Max(0, player.HP)}/{player.MaxHP}");
                        if (!player.IsAlive)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n☠️  Игрок погиб. Игра окончена.");
                            Console.ResetColor();

                            achievements.ShowSummary();

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\nСпасибо за игру!");
                            Console.ResetColor();
                            Console.WriteLine("---------------------------");
                            return;
                        }
                    }

                }

                Console.WriteLine("Бой завершён.");
                ContinueOrPause();
            }
            Console.WriteLine("\nИгра закончена.");
        }

        private Enemy GetRandomWeightedEnemy(List<(Func<Enemy> createEnemy, double weight)> weightedList)
        {
            double totalWeight = 0;
            foreach (var item in weightedList)
                totalWeight += item.weight;

            double roll = rnd.NextDouble() * totalWeight;
            double cumulative = 0;

            foreach (var item in weightedList)
            {
                cumulative += item.weight;
                if (roll <= cumulative)
                    return item.createEnemy();
            }

            return weightedList[weightedList.Count - 1].createEnemy();
        }

        private Enemy GetEnemyForWave(int wave)
        {
            double hpMultiplier = 1 + wave * 0.01;
            double attackMultiplier = 1 + wave * 0.05;
            double defenseMultiplier = 1 + wave * 0.01;

            Enemy enemy = GetRandomWeightedEnemy(enemyWeights);

            enemy.HP = (int)(enemy.HP * hpMultiplier);
            enemy.Attack = (int)(enemy.Attack * attackMultiplier);
            enemy.Defense = (int)(enemy.Defense * defenseMultiplier);

            return enemy;
        }


        private Enemy GetBossForWave(int wave)
        {
            int unused = Math.Max(0, (wave / 10) - 1);
            double hpMultiplier = 1 + unused * 0.15;
            double attackMultiplier = 1 + unused * 0.15;
            double defenseMultiplier = 1 + unused * 0.05;

            Enemy boss = GetRandomBoss();

            boss.HP = (int)(boss.HP * hpMultiplier);
            boss.Attack = (int)(boss.Attack * attackMultiplier);
            boss.Defense = (int)(boss.Defense * defenseMultiplier);

            return boss;
        }

        private void ContinueOrPause()
        {
            achievements.AddWave();
            Console.WriteLine("Нажмите Enter чтобы продолжить...");
            Console.ReadLine();
        }


        private Enemy GetRandomBoss()
        {
            double r = rnd.NextDouble();
            if (r < 0.25) return new BossVVG(rnd);
            if (r < 0.50) return new BossKovalsky(rnd);
            if (r < 0.75) return new BossArchimage(rnd);
            return new BossPestov(rnd);
        }

        private void OpenChest()
        {
            Console.WriteLine("Вы находите сундук...");
            double roll = rnd.NextDouble();
            if (roll < 0.1)
            {
                achievements.AddLegendary();
                LegendaryItem newItem = legendaryItems[rnd.Next(legendaryItems.Count)];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"🌟 Невероятная удача! Вы нашли легендарный артефакт: {newItem.Name}!");
                Console.WriteLine($"Урон: +{newItem.Damage}, Защита: +{newItem.Defense}, HP бонус: +{newItem.HpBonus}");
                Console.ResetColor();

                if (player.LegendaryItem != null)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("У вас уже есть легендарный предмет:");
                    Console.WriteLine(
                        $"{player.LegendaryItem.Name} (Atk {player.LegendaryItem.Damage}, Def {player.LegendaryItem.Defense}, HP +{player.LegendaryItem.HpBonus})");
                    Console.ResetColor();

                    Console.WriteLine("\nХотите заменить текущий легендарный предмет новым?");
                    Console.WriteLine("1) Да, заменить");
                    Console.WriteLine("2) Нет, оставить старый");

                    string input = Console.ReadLine();
                    if (input == "1")
                    {
                        player.LegendaryItem = newItem;
                        player.HP = player.TotalMaxHP;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(
                            $"Вы экипировали новый артефакт: {newItem.Name}! Ваше здоровье увеличено до {player.TotalMaxHP} HP!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Вы решили оставить свой текущий легендарный предмет.");
                    }
                }
                else
                {
                    player.LegendaryItem = newItem;
                    player.HP = player.TotalMaxHP;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"Вы экипировали {newItem.Name}! Ваше здоровье увеличено до {player.TotalMaxHP} HP!");
                    Console.ResetColor();
                }

                return;
            }

            double r = rnd.NextDouble();
            if (r < 0.33)
            {
                Console.WriteLine("В сундуке — лечебное зелье! Вы мгновенно полностью исцеляетесь.");
                player.HealToFull();
                Console.WriteLine($"HP игрока: {player.HP}/{player.TotalMaxHP}");
            }
            else if (r < 0.66)
            {
                Weapon newW = ChooseWeaponFromList();

                if (player.LegendaryItem != null)
                {
                    Console.WriteLine("Ваш легендарный предмет превосходит любое оружие. Он не заменяется.");
                    return;
                }

                int oldW = player.Weapon.Damage;
                Console.WriteLine("\n⚔ Оружие найдено в сундуке:");
                Console.WriteLine($"✨ Новое оружие: {newW.Name} (Atk {newW.Damage})");
                Console.WriteLine($"Текущее оружие: {player.Weapon.Name} (Atk {player.Weapon.Damage})");

                if (newW.Damage > oldW)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Новое оружие сильнее вашего!");
                    Console.ResetColor();

                    Console.WriteLine("Взять новое оружие? 1) Да  2) Нет");
                    if (Console.ReadLine() == "1")
                    {
                        player.Weapon = newW;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Вы экипировали {newW.Name}!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Вы оставили текущее оружие.");
                    }
                }
                else if (newW.Damage == oldW)
                {
                    Console.WriteLine("Оба оружия равны по силе.");
                }
                else
                {
                    Console.WriteLine("У вас оружие лучше!");
                }
            }

            else
            {
                Armor newA = ChooseArmorFromList();

                if (player.LegendaryItem != null)
                {
                    Console.WriteLine("Ваш легендарный предмет превосходит любую броню. Он не заменяется.");
                    return;
                }

                int oldA = player.Armor.Defense;
                Console.WriteLine("\n🛡 Доспех найден в сундуке:");
                Console.WriteLine($"✨ Новая броня: {newA.Name} (Def {newA.Defense})");
                Console.WriteLine($"Текущая броня: {player.Armor.Name} (Def {player.Armor.Defense})");

                if (newA.Defense > oldA)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Новая броня прочнее вашей!");
                    Console.ResetColor();

                    Console.WriteLine("Взять новые доспехи? 1) Да  2) Нет");
                    if (Console.ReadLine() == "1")
                    {
                        player.Armor = newA;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Вы экипировали {newA.Name}!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Вы оставили свою броню.");
                    }
                }
                else if (newA.Defense == oldA)
                {
                    Console.WriteLine("Обе брони равны по защите.");
                }
                else
                {
                    Console.WriteLine("У вас броня лучше!");
                }
            }
        }


        private Weapon ChooseWeaponFromList()
        {
            int index = rnd.Next(weaponList.Count);
            return weaponList[index];
        }

        private Armor ChooseArmorFromList()
        {
            int index = rnd.Next(armorList.Count);
            return armorList[index];
        }


        private List<Weapon> weaponList = new List<Weapon>
        {
            new Weapon("Короткая пиписька", 8),
            new Weapon("Боевой топор", 12),
            new Weapon("Длинный меч", 15),
            new Weapon("Лёгкий кинжал", 7),
            new Weapon("Тяжёлая булава", 18),
            new Weapon("Кровавый Скипетр", 21),
            new Weapon("Клятва Утренней Тьмы", 25),
            new Weapon("Песнь Лезвия", 28),
            new Weapon("Печать Бури", 30),
            new Weapon("Сердце Дракона", 35),
        };

        private List<Armor> armorList = new List<Armor>
        {
            new Armor("Кольчуга", 5),
            new Armor("Железный щит", 9),
            new Armor("Кожаная броня", 3),
            new Armor("Пластинчатая броня", 12),
            new Armor("Тяжёлый шлем", 8),
            new Armor("Доспех Алого Господства", 15),
            new Armor("Одеяние Первого Сумрака", 18),
            new Armor("Пластинчатый Хор Клинков", 21),
            new Armor("Доспех Громового Печати", 24),
            new Armor("Чешуя Драконьего Сердца", 27),
        };

        private List<LegendaryItem> legendaryItems = new List<LegendaryItem>
        {
            new LegendaryItem("Бабаха", 300, 5, 300),
            new LegendaryItem("Маус", 100, 80, 250),
            new LegendaryItem("Е100", 120, 60, 200),
        };
    }
    
    class AchievementTracker
    {
        public int WavesSurvived { get; private set; }
        public int EnemiesKilled { get; private set; }
        public int BossesKilled { get; private set; }
        public int LegendaryFound { get; private set; }

        public void AddWave() => WavesSurvived++;
        public void AddKill() => EnemiesKilled++;
        public void AddBossKill() => BossesKilled++;
        public void AddLegendary() => LegendaryFound++;

        public void ShowSummary()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== 🏆 ДОСТИЖЕНИЯ ИГРОКА ===");
            Console.ResetColor();

            Console.WriteLine($"🌊 Пройдено волн: {WavesSurvived}");
            Console.WriteLine($"⚔️ Убито врагов: {EnemiesKilled}");
            Console.WriteLine($"👑 Побеждено боссов: {BossesKilled}");
            Console.WriteLine($"💎 Найдено легендарок: {LegendaryFound}");

            Console.WriteLine("\n--- Особые достижения ---");

            if (WavesSurvived >= 20) Console.WriteLine("🔥 Выживший: Пережил 20 волн!");
            if (WavesSurvived >= 50) Console.WriteLine("🔥 Выживший: Пережил 50 волн!");
            if (WavesSurvived >= 100) Console.WriteLine("🔥 Выживший: Пережил 100 волн!");
            if (WavesSurvived >= 200) Console.WriteLine("🔥 Выживший: Пережил 200 волн!");

            if (BossesKilled >= 3) Console.WriteLine("👑 Охотник на титанов: Победил 3 боссов!");
            if (BossesKilled >= 10) Console.WriteLine("👑 Охотник на титанов: Победил 10 боссов!");
            if (BossesKilled >= 20) Console.WriteLine("👑 Охотник на титанов: Победил 20 боссов!");
            if (BossesKilled >= 50) Console.WriteLine("👑 Охотник на титанов: Победил 50 боссов!");
            
            if (LegendaryFound >= 1) Console.WriteLine("💫 Избранный: Нашёл легендарный артефакт!");
            if (LegendaryFound >= 5) Console.WriteLine("💫 Избранный: Нашёл 5 легендарных артефакта!");
            if (LegendaryFound >= 25) Console.WriteLine("💫 Избранный: Нашёл 25 легендарных артефактов!");
            if (LegendaryFound >= 50) Console.WriteLine("💫 Избранный: Нашёл 50 легендарных артефактов!");
            
            if (EnemiesKilled >= 50) Console.WriteLine("☠️ Мясорубка: Убил 50 врагов!");
            if (EnemiesKilled >= 100) Console.WriteLine("☠️ Мясорубка: Убил 100 врагов!");
            if (EnemiesKilled >= 200) Console.WriteLine("☠️ Мясорубка: Убил 200 врагов!");
            if (EnemiesKilled >= 500) Console.WriteLine("☠️ Мясорубка: Убил 500 врагов!");
            
            if (WavesSurvived >= 50) Console.WriteLine("🌌 Бессмертный: Достиг 50-й волны!");
            if (WavesSurvived >= 100) Console.WriteLine("🌌 Бессмертный: Достиг 100-й волны!");
            if (WavesSurvived >= 200) Console.WriteLine("🌌 Бессмертный: Достиг 200-й волны!");
            if (WavesSurvived >= 500) Console.WriteLine("🌌 Бессмертный: Достиг 500-й волны!");
        }
    }

    class Program
    {
        static void Main(string[] args)

        {
            var game = new Game();
            game.Start();
        }
    }
}
