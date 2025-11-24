using TextRPG.Enemies;
using TextRPG.Entities;
using TextRPG.Factories;
using TextRPG.Items;

namespace TextRPG;

class Game
{

    private EnemyFactory enemyFactory;


    private Random rnd = new Random();
    private Player player;
    private int turn = 0;
    private int currentWave = 0;

    public Game()
    {
        enemyFactory = new EnemyFactory(rnd);
        var starterWeapon = new Weapon("Деревянный меч", 6);
        var starterArmor = new Armor("Кожаная броня", 3);
        player = new Player(100, starterWeapon, starterArmor);

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
                    enemy = enemyFactory.CreateBoss(currentWave);
                    Console.WriteLine($"Встреча! Усиленный босс (волна {currentWave}): {enemy}");
                }
                else
                {
                    enemy = enemyFactory.CreateBoss(currentWave);
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

                enemy = enemyFactory.CreateRandomEnemy(currentWave);
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
                            if (enemy is BossVVG || enemy is BossKovalsky || enemy is BossArchimage ||
                                enemy is BossPestov)
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





    private void ContinueOrPause()
    {
        achievements.AddWave();
        Console.WriteLine("Нажмите Enter чтобы продолжить...");
        Console.ReadLine();
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