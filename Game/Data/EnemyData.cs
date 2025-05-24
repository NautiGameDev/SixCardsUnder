using PixelArtGameJam.Game.Abilities;
using PixelArtGameJam.Game.Components;

namespace PixelArtGameJam.Game.Data
{
    public class EnemyData
    {
        private Dictionary<string, Dictionary<string, string>> enemies = new Dictionary<string, Dictionary<string, string>>()
        {
            {"DarkElf Alchemist", new Dictionary<string, string>()
			{
				{"Health", "10"},
				{"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Specter" },
				{"Image", "Assets/Enemies/DarkElf_Alchemist.png"},
				{"Shadow", "Assets/Enemies/DarkElf_Alchemist_Shadow.png"}
			}
			},
			{"DarkElf Necromancer", new Dictionary<string, string>()
			{
                {"Health", "10"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Specter" },
                {"Image", "Assets/Enemies/DarkElf_Necromancer.png"},
                {"Shadow", "Assets/Enemies/DarkElf_Necromancer_Shadow.png"}
            }
			},
			{"Demon Druid", new Dictionary<string, string>()
			{
                {"Health", "15"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Fireball" },
                {"Image", "Assets/Enemies/Demon_Druid.png"},
                {"Shadow", "Assets/Enemies/Demon_Druid_Shadow.png"}
            }
			},
			{"Demon Mage", new Dictionary<string,string>()
			{
                {"Health", "15"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Fireball" },
                {"Image", "Assets/Enemies/Demon_Mage.png"},
                {"Shadow", "Assets/Enemies/Demon_Mage_Shadow.png"}
            }
			},
			{"Undead Archer", new Dictionary<string, string>()
			{
                {"Health", "5"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Burst" },
                {"Image", "Assets/Enemies/Undead_Archer.png"},
                {"Shadow", "Assets/Enemies/Undead_Archer_Shadow.png"}
            }
			},
			{"Undead Archer 2", new Dictionary<string, string>()
			{
                {"Health", "5"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Burst" },
                {"Image", "Assets/Enemies/Undead_Archer2.png"},
                {"Shadow", "Assets/Enemies/Undead_Archer2_Shadow.png"}
            }
			},
            {"Undead Knight", new Dictionary<string, string>()
            {
                {"Health", "20"},
                {"Move Speed", "1"},
                {"Attack Range", "3"},
                {"Ability", "Spikes" },
                {"Image", "Assets/Enemies/Undead_Knight.png"},
                {"Shadow", "Assets/Enemies/Undead_Knight_Shadow.png"}
            }
            },
            {"Undead Sage", new Dictionary<string, string>()
            {
                {"Health", "12"},
                {"Move Speed", "1"},
                {"Attack Range", "10"},
                {"Ability", "Spark" },
                {"Image", "Assets/Enemies/Undead_Sage.png"},
                {"Shadow", "Assets/Enemies/Undead_Sage_Shadow.png"}
            }
            },
            {"Undead Warrior", new Dictionary<string, string>()
            {
                {"Health", "18"},
                {"Move Speed", "1"},
                {"Attack Range", "3"},
                {"Ability", "Firewall" },
                {"Image", "Assets/Enemies/Undead_Warrior.png"},
                {"Shadow", "Assets/Enemies/Undead_Warrior_Shadow.png"}
            }
            }
        };


        public Dictionary<string, string> GetRandomEnemy()
        {
            Random random = new Random();

            List<string> enemyList = enemies.Keys.ToList();

            int randomIndex = random.Next(enemyList.Count);
            string enemyName = enemyList[randomIndex];

            return enemies[enemyName];
        }

        public Ability GetEnemyAbility(string abilityName)
        {
            switch (abilityName) 
            {
                case "Specter":
                    Specter specterAbility = new Specter(WorldObjects.Projectile.ProjSource.ENEMY);
                    return specterAbility;
                case "Burst":
                    Burst burstAbility = new Burst(WorldObjects.Projectile.ProjSource.ENEMY);
                    return burstAbility;
                case "Spark":
                    Spark sparkAbility = new Spark(WorldObjects.Projectile.ProjSource.ENEMY);
                    return sparkAbility;
                case "Fireball":
                    Fireball fireballAbility = new Fireball(WorldObjects.Projectile.ProjSource.ENEMY);
                    return fireballAbility;
                case "Spikes":
                    Spikes spikesAbility = new Spikes(WorldObjects.Projectile.ProjSource.ENEMY);
                    return spikesAbility;
                case "Firewall":
                    Firewall firewallAbility = new Firewall(WorldObjects.Projectile.ProjSource.ENEMY);
                    return firewallAbility;
                default:
                    Burst defaultBurst = new Burst(WorldObjects.Projectile.ProjSource.ENEMY);
                    return defaultBurst;
            }
        }
    }
}
