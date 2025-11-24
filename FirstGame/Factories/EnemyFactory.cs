using System;
using System.Collections.Generic;
using TextRPG.Enemies;

namespace TextRPG.Factories;

    public class EnemyFactory
    {
        private readonly Random rnd;

        private List<(Func<Enemy> createEnemy, double weight)> enemyWeights;

        public EnemyFactory(Random random)
        {
            rnd = random;

            enemyWeights = new List<(Func<Enemy>, double)>
            {
                (() => new Goblin(rnd), 0.5),
                (() => new Skeleton(rnd), 0.25),
                (() => new Mage(rnd), 0.25),
                (() => new Slime(rnd), 0.35),
            };
        }

        public Enemy CreateRandomEnemy(int wave)
        {
            Enemy enemy = GetRandomWeightedEnemy();
            ScaleEnemy(enemy, wave);
            return enemy;
        }

        public Enemy CreateBoss(int wave)
        {
            Enemy boss = CreateRandomBoss();
            ScaleBoss(boss, wave);
            return boss;
        }

        private Enemy GetRandomWeightedEnemy()
        {
            double totalWeight = 0;
            foreach (var item in enemyWeights)
                totalWeight += item.weight;

            double roll = rnd.NextDouble() * totalWeight;
            double cumulative = 0;

            foreach (var item in enemyWeights)
            {
                cumulative += item.weight;
                if (roll <= cumulative)
                    return item.createEnemy();
            }

            return enemyWeights[^1].createEnemy();
        }

        private Enemy CreateRandomBoss()
        {
            double r = rnd.NextDouble();
            if (r < 0.25) return new BossVVG(rnd);
            if (r < 0.50) return new BossKovalsky(rnd);
            if (r < 0.75) return new BossArchimage(rnd);
            return new BossPestov(rnd);
        }

        private void ScaleEnemy(Enemy enemy, int wave)
        {
            enemy.HP = (int)(enemy.HP * (1 + wave * 0.01));
            enemy.Attack = (int)(enemy.Attack * (1 + wave * 0.05));
            enemy.Defense = (int)(enemy.Defense * (1 + wave * 0.01));
        }

        private void ScaleBoss(Enemy boss, int wave)
        {
            int unused = Math.Max(0, wave / 10 - 1);

            boss.HP = (int)(boss.HP * (1 + unused * 0.15));
            boss.Attack = (int)(boss.Attack * (1 + unused * 0.15));
            boss.Defense = (int)(boss.Defense * (1 + unused * 0.05));
        }
    }

