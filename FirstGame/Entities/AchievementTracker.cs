namespace TextRPG.Entities;

public class AchievementTracker
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
            Console.WriteLine("\n=== ДОСТИЖЕНИЯ ИГРОКА ===");
            Console.ResetColor();

            Console.WriteLine($"Пройдено волн: {WavesSurvived}");
            Console.WriteLine($"Убито врагов: {EnemiesKilled}");
            Console.WriteLine($"Побеждено боссов: {BossesKilled}");
            Console.WriteLine($"Найдено легендарок: {LegendaryFound}");

            Console.WriteLine("\n--- Особые достижения ---");

            if (WavesSurvived >= 20) Console.WriteLine("Выживший: Пережил 20 волн!");
            if (WavesSurvived >= 50) Console.WriteLine("Выживший: Пережил 50 волн!");
            if (WavesSurvived >= 100) Console.WriteLine("Выживший: Пережил 100 волн!");
            if (WavesSurvived >= 200) Console.WriteLine("Выживший: Пережил 200 волн!");

            if (BossesKilled >= 3) Console.WriteLine("Охотник на титанов: Победил 3 боссов!");
            if (BossesKilled >= 10) Console.WriteLine("Охотник на титанов: Победил 10 боссов!");
            if (BossesKilled >= 20) Console.WriteLine("Охотник на титанов: Победил 20 боссов!");
            if (BossesKilled >= 50) Console.WriteLine("Охотник на титанов: Победил 50 боссов!");

            if (LegendaryFound >= 1) Console.WriteLine("Избранный: Нашёл легендарный артефакт!");
            if (LegendaryFound >= 5) Console.WriteLine("Избранный: Нашёл 5 легендарных артефактов!");
            if (LegendaryFound >= 25) Console.WriteLine("Избранный: Нашёл 25 легендарных артефактов!");
            if (LegendaryFound >= 50) Console.WriteLine("Избранный: Нашёл 50 легендарных артефактов!");

            if (EnemiesKilled >= 50) Console.WriteLine("Мясорубка: Убил 50 врагов!");
            if (EnemiesKilled >= 100) Console.WriteLine("Мясорубка: Убил 100 врагов!");
            if (EnemiesKilled >= 200) Console.WriteLine("Мясорубка: Убил 200 врагов!");
            if (EnemiesKilled >= 500) Console.WriteLine("Мясорубка: Убил 500 врагов!");

            if (WavesSurvived >= 50) Console.WriteLine("Бессмертный: Достиг 50-й волны!");
            if (WavesSurvived >= 100) Console.WriteLine("Бессмертный: Достиг 100-й волны!");
            if (WavesSurvived >= 200) Console.WriteLine("Бессмертный: Достиг 200-й волны!");
            if (WavesSurvived >= 500) Console.WriteLine("Бессмертный: Достиг 500-й волны!");
        }
}