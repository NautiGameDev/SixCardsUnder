using System.Numerics;
using PixelArtGameJam.Game.Abilities;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class Dungeon : Scene
    {
        public DungeonCrawler dgCrawlerRef { get; private set; }
        public Player player { get; private set; }
        public Map map { get; private set; }
        public Camera camera { get; private set; }

        public int floor { get; private set; }

        public WorldObject[][] worldObjects { get; private set; }
        public WorldObject[][] woLayerTwo { get; set; }
        protected List<Projectile> projectiles { get; set; }
        protected List<Enemy> enemies { get; set; }
        protected List<Collectable> collectables { get; set; }


        private bool hasLoaded { get; set; } = false;
               


        public Dungeon(DungeonCrawler dgCrawlerRef)
        {
            floor = 0;
            this.dgCrawlerRef = dgCrawlerRef;

            StartNewFloor();            
        }

        public async void ReturnToMainMenu()
        {
            MainMenu newScene = new MainMenu(dgCrawlerRef);
            dgCrawlerRef.LoadNewScene(newScene);

            await AudioController.StopSound("Assets/Audio/GameplayMusic.ogg");
            await AudioController.PlaySound("Assets/Audio/MainMenuMusic.ogg", AudioController.MusicVolume, true);
        }

        public async void StartNewFloor()
        {
            floor++;

            camera = new Camera(Vector2.Zero);
            DateTime instancedTime = DateTime.Now;

            projectiles = new List<Projectile>();
            enemies = new List<Enemy>();
            collectables = new List<Collectable>();

            LoadDungeon(instancedTime);
        }

        private async void LoadDungeon(DateTime instancedTime)
        {
            await camera.CreatePlayerUI();
            await GenerateMap();
            await camera.CreateFloorAndCeiling(map.floorPath, map.ceilingPath);
            await camera.InstantiateRayCasts(worldObjects, woLayerTwo, map.gridSize);

            hasLoaded = true;
            camera.CreateFadeInEffect();
            float loadTime = (float)(DateTime.Now - instancedTime).TotalSeconds;
            Console.WriteLine($"Dungeon loaded in {loadTime} seconds");
        }

        private async Task GenerateMap()
        {
            //Get map
            map = new Map();
            await map.LoadLevelGenerator(floor);
            await map.GenerateMapData();
            string[][] mapData = map.GetMapData();

            //Get map data inforation
            int rowCount = mapData.Length;
            int columnCount = (rowCount > 0) ? mapData[0].Length : 0;

            //Set-up world objects array
            worldObjects = new WorldObject[rowCount][];
            woLayerTwo = new WorldObject[rowCount][];
            for (int i=0; i< rowCount; i++)
            {
                WorldObject[] newRow = new WorldObject[columnCount];
                worldObjects[i] = newRow;

                WorldObject[] newRowTwo = new WorldObject[columnCount];
                woLayerTwo[i] = newRowTwo;
            }

            await PopulateWorldObjectsFromMapData(mapData);
        }

        private async Task PopulateWorldObjectsFromMapData(string[][] mapData)
        {
            EnemyData enemyData = new EnemyData();

            for (int y = 0; y < mapData.Length; y++)
            {
                for (int x = 0; x < mapData[y].Length; x++)
                {
                    float xPos = x * (float)(map.gridSize) + map.gridSize / 2;
                    float yPos = y * (float)(map.gridSize) + map.gridSize / 2;
                    Vector2 position = new Vector2(xPos, yPos);


                    switch (mapData[y][x])
                    {
                        case "P":
                            player = new Player(position, camera, this, map.levelGen.startRotation);
                            worldObjects[y][x] = player;
                            break;

                        //Walls
                        case "x":
                            Wall interiorWall = new Wall(position);
                            string wallTexturePath = map.GetTexturePath("MudBrick");
                            interiorWall.sprite.SetImage(wallTexturePath);
                            interiorWall.sprite.SetDimensions(new Vector2(256, 256));
                            interiorWall.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = interiorWall;
                            break;
                        case "X":
                            Wall outerWall = new Wall(position);
                            string wallTexturePathX = map.GetTexturePath("TwoToneBrick");
                            outerWall.sprite.SetImage(wallTexturePathX);
                            outerWall.sprite.SetDimensions(new Vector2(256, 256));
                            outerWall.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = outerWall;
                            break;
                        case "#":
                            Wall  hallwayWall = new Wall(position);
                            string wallTexturePathHallway = map.GetTexturePath("CrackedBricks");
                            hallwayWall.sprite.SetImage(wallTexturePathHallway);
                            hallwayWall.sprite.SetDimensions(new Vector2(256, 256));
                            hallwayWall.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = hallwayWall;
                            break;
                        case "@":
                            Wall column = new Wall(position);
                            string wallTexturePathColumn = map.GetTexturePath("PolishedFlag");
                            column.sprite.SetImage(wallTexturePathColumn);
                            column.sprite.SetDimensions(new Vector2(256, 256));
                            column.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = column;
                            break;
                        case "S":
                            Wall startDoor = new Wall(position);
                            startDoor.SetSpecialWallType(WorldObject.ObjectType.WALLSTART);
                            string startTexturePath = map.GetTexturePath("TwoToneStart");
                            startDoor.sprite.SetImage(startTexturePath);
                            startDoor.sprite.SetDimensions(new Vector2(256, 256));
                            startDoor.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = startDoor;
                            break;
                        case "F":
                            Wall finishDoor = new Wall(position);
                            finishDoor.SetSpecialWallType(WorldObject.ObjectType.WALLFINISH);
                            string finishTexturePath = map.GetTexturePath("TwoToneFinish");
                            finishDoor.sprite.SetImage(finishTexturePath);
                            finishDoor.sprite.SetDimensions(new Vector2(256, 256));
                            finishDoor.sprite.SetScale(new Vector2(1, 1));
                            worldObjects[y][x] = finishDoor;
                            break;

                        //World Entities
                        case "E":
                            Dictionary<string, string> enemy = enemyData.GetRandomEnemy();
                            int maxHealth = int.Parse(enemy["Health"]) + ((floor - 1) * 5);
                            int attackRange = int.Parse(enemy["Attack Range"]);
                            int moveSpeed = int.Parse(enemy["Move Speed"]);
                            string enemyAbility = enemy["Ability"];
                            Ability ability = enemyData.GetEnemyAbility(enemyAbility);
                            string spritePath = enemy["Image"];
                            string shadowPath = enemy["Shadow"];

                            Enemy newEnemy = new Enemy(position, WorldObject.ObjectType.ENEMY, this, maxHealth, attackRange, moveSpeed, ability);
                                                        
                            newEnemy.sprite.SetImage(spritePath);
                            newEnemy.sprite.SetShadow(shadowPath);
                            newEnemy.sprite.SetDimensions(new Vector2(136, 289));
                            newEnemy.sprite.SetScale(new Vector2(1, 1));
                            newEnemy.SetVerticalOffset(50);
                            worldObjects[y][x] = newEnemy;
                            enemies.Add(newEnemy);

                            break;
                        case "H":
                            Heal healAbility = new Heal(Projectile.ProjSource.PLAYER);
                            Collectable newCollectable = new Collectable(position, WorldObject.ObjectType.COLLECTABLE, healAbility);

                            newCollectable.sprite.SetImage("Assets/Objects/Heart.png");
                            newCollectable.sprite.SetShadow("Assets/Objects/Heart_Shadow.png");
                            newCollectable.sprite.SetDimensions(new Vector2(256, 256));
                            newCollectable.sprite.SetScale(new Vector2(1, 1));
                            newCollectable.SetVerticalOffset(25);
                            worldObjects[y][x] = newCollectable;
                            collectables.Add(newCollectable);
                            break;

                        case "C":
                            Block shieldAbility = new Block(Projectile.ProjSource.PLAYER);
                            Collectable newCollectableShield = new Collectable(position, WorldObject.ObjectType.COLLECTABLE, shieldAbility);

                            newCollectableShield.sprite.SetImage("Assets/Objects/Crystal.png");
                            newCollectableShield.sprite.SetShadow("Assets/Objects/Crystal_Shadow.png");
                            newCollectableShield.sprite.SetDimensions(new Vector2(256, 256));
                            newCollectableShield.sprite.SetScale(new Vector2(1, 1));
                            newCollectableShield.SetVerticalOffset(25);
                            worldObjects[y][x] = newCollectableShield;
                            collectables.Add(newCollectableShield);
                            break;
                    }
                }
            }
        }

        public Vector2 MoveWorldObject(WorldObject wo, Vector2 currentPosition, Vector2 direction)
        {
            int currentGridX = (int)Math.Floor(currentPosition.X / map.gridSize);
            int currentGridY = (int)Math.Floor(currentPosition.Y /  map.gridSize);

            int targetGridX = (int)(currentGridX + direction.X);
            int targetGridY = (int)(currentGridY + direction.Y);

            if (worldObjects[targetGridY][targetGridX] == null)
            {
                worldObjects[targetGridY][targetGridX] = wo;
                worldObjects[currentGridY][currentGridX] = null;

                Vector2 convertedNewPosition = new Vector2(targetGridX * map.gridSize + (map.gridSize/2), targetGridY * map.gridSize + (map.gridSize / 2));
                               

                return convertedNewPosition;
            }
            else
            {                

                return currentPosition;
            }                
        }

        public Vector2 MoveProjectileLayerTwo(Projectile projectile, Vector2 currentPosition, Vector2 direction)
        {
            int currentGridX = (int)Math.Floor(currentPosition.X / map.gridSize);
            int currentGridY = (int)Math.Floor(currentPosition.Y / map.gridSize);

            int targetGridX = (int)(currentGridX + direction.X);
            int targetGridY = (int)(currentGridY + direction.Y);

            if (woLayerTwo[targetGridY][targetGridX] == null)
            {
                woLayerTwo[targetGridY][targetGridX] = projectile;
                worldObjects[currentGridY][currentGridX] = null;
                

                Vector2 convertedNewPosition = new Vector2(targetGridX * map.gridSize + (map.gridSize / 2), targetGridY * map.gridSize + (map.gridSize / 2));

                return convertedNewPosition;
            }
            else
            {
                return currentPosition;
            }
        }

        public void RemoveProjectileFromLayerTwo(Projectile projectile, Vector2 currentPosition)
        {
            int currentGridX = (int)Math.Floor(currentPosition.X / map.gridSize);
            int currentGridY = (int)Math.Floor(currentPosition.Y / map.gridSize);

            if (woLayerTwo[currentGridY][currentGridX] == projectile)
            {
                woLayerTwo[currentGridY][currentGridX] = null;
                
            }
        }

        public bool TestGridSpaceEmpty(Vector2 gridPosition)
        {
            int x = (int)(gridPosition.X);
            int y = (int)(gridPosition.Y);

            if (worldObjects[y][x] == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public WorldObject GetObjectAtGridPos(Vector2 currentPosition, Vector2 direction)
        {
            int currentGridX = (int)Math.Floor(currentPosition.X / map.gridSize);
            int currentGridY = (int)Math.Floor(currentPosition.Y / map.gridSize);

            int targetGridX = (int)(currentGridX + direction.X);
            int targetGridY = (int)(currentGridY + direction.Y);

            return worldObjects[targetGridY][targetGridX];
        }

        public Projectile GetProjectileAtGridPos(Vector2 gridPosition)
        {
            int x = (int)(gridPosition.X);
            int y = (int)(gridPosition.Y);

            if (worldObjects[y][x].objType == WorldObject.ObjectType.PROJECTILE)
            {
                return (Projectile)worldObjects[y][x];
            }

            return null;
        }

        public void AddProjectileToDungeon(Projectile projectile, Vector2 gridPosition)
        {
            int x = (int)(gridPosition.X);
            int y = (int)(gridPosition.Y);

            projectiles.Add(projectile);
            worldObjects[y][x] = projectile;
        }

        public void RemoveProjectileFromDungeon(Projectile projectile, Vector2 gridPosition)
        {
            int x = (int)(gridPosition.X);
            int y = (int)(gridPosition.Y);

            projectiles.Remove(projectile);

            if (worldObjects[y][x] == projectile)
            {
                worldObjects[y][x] = null;
            }

            if (woLayerTwo[y][x] == projectile)
            {
                woLayerTwo[y][x] = null;
            }        
        }

        public void RemoveObjectFromDungeon(WorldObject wo, Vector2 worldposition)
        {
            int gridX = (int)(worldposition.X / map.gridSize);
            int gridY = (int)(worldposition.Y / map.gridSize);

            if(worldObjects[gridY][gridX] == wo)
            {
                worldObjects[gridY][gridX] = null;
            }
        }

        public void RemoveEnemyFromEnemies(Enemy enemy)
        {
            enemies.Remove(enemy);
        }

        public void RemoveCollectableFromCollectables(Collectable collectable)
        {
            collectables.Remove(collectable);
        }

        public override async Task Update(float deltaTime)
        {
            if (!hasLoaded) return;

            if (player != null)
            {
                player.Update(deltaTime);
            }

            if (camera != null)
            {
                await camera.RenderWorld(deltaTime);
            }

            List<Projectile> tempProjectiles = projectiles.ToList();

            foreach (Projectile projectile in tempProjectiles)
            {
                projectile.Update(deltaTime);
            }

            List<Collectable> tempCollectables = collectables.ToList();

            foreach (Collectable collectable in tempCollectables)
            {
                collectable.Update(deltaTime);
            }
        }

        public void UpdateEnvironment()
        {
            List<Enemy> tempEnemies = enemies.ToList();
            foreach (Enemy enemy in tempEnemies)
            {
                enemy.EnvironmentUpdate();
            }

            List<Projectile> tempProjectiles = projectiles.ToList();

            foreach (Projectile projectile in tempProjectiles)
            {
                projectile.EnvironmentUpdate();
            }
        }
    }
}
