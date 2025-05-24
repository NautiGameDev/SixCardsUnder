using System.Numerics;

namespace PixelArtGameJam.Game.MapGenerator
{
    public class LevelGenerator
    {
        Random random = new Random();
        BaseRooms baseRooms { get; set; }

        const int NUMROWS = 125;
        const int NUMCOLS = 125;
        const int XCHUNKS = 5;
        const int YCHUNKS = 5;
        const int CHUNKSIZE = 25;

        int EnemyCount = 50;
        int collectablesCount = 10;

        Queue<int[]> westDoors = new Queue<int[]>();
        Queue<int[]> eastDoors = new Queue<int[]>();
        Queue<int[]> northDoors = new Queue<int[]>();
        Queue<int[]> southDoors = new Queue<int[]>();

        int[] playerCoords = new int[2];

        public float startRotation { get; set; }

        List<int[]> GoalDoors = new List<int[]>();
        List<int[]> EnemyPositions = new List<int[]>();
        List<int[]> Collectables = new List<int[]>();
                

        public async Task SetEnemyNumber(int floor)
        {
            EnemyCount = 50 + ((floor - 1) * 10);

            if (EnemyCount > 125)
            {
                EnemyCount = 125;
            }
        }

        public async Task LoadRoomData()
        {
            baseRooms = new BaseRooms();
            await baseRooms.LoadRoomData();
        }

        public async void RunGenerator()
        {
            string[][] mapData = await GenerateMap();
            PrintMapToConsole(mapData);
        }

        private async Task RefreshMap()
        {
            Console.Clear();
            westDoors.Clear();
            eastDoors.Clear();
            northDoors.Clear();
            southDoors.Clear();
            GoalDoors.Clear();
            EnemyPositions.Clear();
        }

        public async Task<string[][]> GenerateMap()
        {
            await RefreshMap();
            Console.WriteLine("\nGenerating map...\n");

            string[][] mapData = new string[NUMCOLS][];

            //Instantiate Data structure
            mapData = await BuildMapDataBase(mapData);
            mapData = await GenerateRooms(mapData);

            //Hallways
            mapData = await GenerateEastToWestHalls(mapData);
            mapData = await GenerateSouthToNorthHalls(mapData);

            //Generate key items
            mapData = await ChooseStartPosition(mapData);
            mapData = await ChooseEndPosition(mapData);
            mapData = await GenerateEnemies(mapData);

            return mapData;
        }

        private async Task<string[][]> BuildMapDataBase(string[][] mapData)
        {
            mapData = new string[NUMROWS][];

            for (int i = 0; i < NUMROWS; i++)
            {
                string[] newRow = new string[NUMCOLS];
                mapData[i] = newRow;
            }

            for (int y = 0; y < mapData.Length; y++)
            {
                for (int x = 0; x < mapData[y].Length; x++)
                {
                    mapData[y][x] = ".";
                }
            }

            return mapData;
        }

        private async Task<string[][]> GenerateRooms(string[][] mapData)
        {
            for (int yChunk = 0; yChunk < YCHUNKS; yChunk++)
            {
                for (int xChunk = 0; xChunk < XCHUNKS; xChunk++)
                {
                    string[][] newRoom;

                    if (yChunk == 0 && xChunk == 0) //Top left corner
                    {
                        newRoom = baseRooms.GetSERoom();
                    }
                    else if (yChunk == 0 && xChunk > 0 && xChunk < XCHUNKS - 1) //Top rooms
                    {
                        newRoom = baseRooms.GetSEWRoom();
                    }
                    else if (yChunk == 0 && xChunk == XCHUNKS - 1) //Top right corner
                    {
                        newRoom = baseRooms.GetSWRoom();
                    }

                    else if (xChunk == 0 && yChunk > 0 && yChunk < YCHUNKS - 1) //Left Side Rooms
                    {
                        newRoom = baseRooms.GetNESRoom();
                    }

                    else if (xChunk == XCHUNKS - 1 && yChunk > 0 && yChunk < YCHUNKS - 1)
                    {
                        newRoom = baseRooms.GetNSWRoom();
                    }

                    else if (xChunk == 0 && yChunk == YCHUNKS - 1) //Bottom left corner
                    {
                        newRoom = baseRooms.GetNERoom();
                    }

                    else if (xChunk > 0 && xChunk < XCHUNKS - 1 && yChunk == YCHUNKS - 1) //Bottom Rooms
                    {
                        newRoom = baseRooms.GetNEWRoom();
                    }

                    else if (xChunk == XCHUNKS - 1 && yChunk == YCHUNKS - 1) //Bottom right corner
                    {
                        newRoom = baseRooms.GetNWRoom();
                    }

                    else
                    {
                        newRoom = baseRooms.GetNWSERoom();
                    }

                    for (int y = 0; y < newRoom.Length; y++)
                    {
                        for (int x = 0; x < newRoom[y].Length; x++)
                        {
                            int startX = xChunk * CHUNKSIZE;
                            int startY = yChunk * CHUNKSIZE;

                            mapData[y + startY][x + startX] = newRoom[y][x];


                            //Add coordinates of special tiles to lists (doors, enemies, chests, etc.)

                            int[] coords = new int[2] { (x + startX), (y + startY) };

                            if (newRoom[y][x] == "n")
                            {
                                northDoors.Enqueue(coords);
                            }
                            else if (newRoom[y][x] == "e")
                            {
                                eastDoors.Enqueue(coords);
                            }
                            else if (newRoom[y][x] == "s")
                            {
                                southDoors.Enqueue(coords);
                            }
                            else if (newRoom[y][x] == "w")
                            {
                                westDoors.Enqueue(coords);
                            }
                            else if (newRoom[y][x] == "D")
                            {
                                GoalDoors.Add(coords);
                                mapData[y + startY][x + startX] = "X";
                            }
                            else if (newRoom[y][x] == "E")
                            {
                                EnemyPositions.Add(coords);
                                mapData[y + startY][x + startX] = " ";
                            }
                            else if (newRoom[y][x] == "C")
                            {
                                Collectables.Add(coords);
                                mapData[y + startY][x + startX] = " ";
                            }
                        }
                    }
                }
            }

            return mapData;
        }

        private async Task<string[][]> GenerateEastToWestHalls(string[][] mapData)
        {
            foreach (int[] eastDoor in eastDoors)
            {
                int[] westDoorCoords = westDoors.Peek();


                int halfWayLength = (eastDoor[0] + westDoorCoords[0]) / 2 - eastDoor[0];
                int vertLength = westDoorCoords[1] - eastDoor[1];
                int finalHall = westDoorCoords[0] - halfWayLength;

                for (int i = 0; i < halfWayLength; i++)
                {
                    mapData[eastDoor[1]][eastDoor[0] + i] = " ";

                    mapData = BuildWallOnNeighbors(eastDoor[0] + i, eastDoor[1], mapData);
                }

                for (int i = 0; i <= halfWayLength + 1; i++)
                {
                    mapData[eastDoor[1] + vertLength][eastDoor[0] + halfWayLength + i] = " ";

                    mapData = BuildWallOnNeighbors(eastDoor[0] + halfWayLength + i, eastDoor[1] + vertLength, mapData);
                }

                if (vertLength > 0)
                {
                    for (int i = 0; i < vertLength; i++)
                    {
                        mapData[eastDoor[1] + i][eastDoor[0] + halfWayLength] = " ";

                        mapData = BuildWallOnNeighbors(eastDoor[0] + halfWayLength, eastDoor[1] + i, mapData);
                    }
                }
                else
                {
                    for (int i = 0; i > vertLength; i--)
                    {
                        mapData[eastDoor[1] + i][eastDoor[0] + halfWayLength] = " ";
                        mapData = BuildWallOnNeighbors(eastDoor[0] + halfWayLength, eastDoor[1] + i, mapData);
                    }
                }

                westDoors.Dequeue();
            }

            return mapData;
        }

        private async Task<string[][]> GenerateSouthToNorthHalls(string[][] mapData)
        {
            foreach (int[] southDoor in southDoors)
            {
                int[] northDoorCoords = northDoors.Peek();


                int halfWayLength = (southDoor[1] + northDoorCoords[1]) / 2 - southDoor[1];
                int horzWidth = northDoorCoords[0] - southDoor[0];
                int finalHall = northDoorCoords[1] - halfWayLength;

                for (int i = 0; i < halfWayLength; i++)
                {
                    mapData[southDoor[1] + i][southDoor[0]] = " ";

                    mapData = BuildWallOnNeighbors(southDoor[0], southDoor[1] + i, mapData);
                }



                for (int i = 0; i <= halfWayLength + 1; i++)
                {

                    mapData[southDoor[1] + halfWayLength + i][southDoor[0] + horzWidth] = " ";

                    mapData = BuildWallOnNeighbors(southDoor[0] + horzWidth, southDoor[1] + halfWayLength + i, mapData);
                }

                if (horzWidth > 0)
                {
                    for (int i = 0; i < horzWidth; i++)
                    {

                        mapData[southDoor[1] + halfWayLength][southDoor[0] + i] = " ";

                        mapData = BuildWallOnNeighbors(southDoor[0] + i, southDoor[1] + halfWayLength, mapData);
                    }
                }
                else
                {
                    for (int i = 0; i > horzWidth; i--)
                    {
                        mapData[southDoor[1] + halfWayLength][southDoor[0] + i] = " ";

                        mapData = BuildWallOnNeighbors(southDoor[0] + i, southDoor[1] + halfWayLength, mapData);
                    }
                }

                northDoors.Dequeue();
            }

            return mapData;
        }

        private string[][] BuildWallOnNeighbors(int x, int y, string[][] mapData)
        {
            if (mapData[y - 1][x] == ".")
            {
                mapData[y - 1][x] = "#";
            }

            if (mapData[y + 1][x] == ".")
            {
                mapData[y + 1][x] = "#";
            }

            if (mapData[y][x - 1] == ".")
            {
                mapData[y][x - 1] = "#";
            }

            if (mapData[y][x + 1] == ".")
            {
                mapData[y][x + 1] = "#";
            }

            if (mapData[y + 1][x + 1] == ".")
            {
                mapData[y + 1][x + 1] = "#";
            }

            if (mapData[y - 1][x + 1] == ".")
            {
                mapData[y - 1][x + 1] = "#";
            }

            if (mapData[y + 1][x - 1] == ".")
            {
                mapData[y + 1][x - 1] = "#";
            }

            if (mapData[y - 1][x - 1] == ".")
            {
                mapData[y - 1][x - 1] = "#";
            }

            return mapData;
        }

        private async Task<string[][]> ChooseStartPosition(string[][] mapData)
        {
            int randomInt = random.Next(GoalDoors.Count);

            int[] doorCoordinates = GoalDoors[randomInt];
            GoalDoors.Remove(doorCoordinates);

            mapData[doorCoordinates[1]][doorCoordinates[0]] = "S";

            //Find empty spot for player to spawn and set player coordinates for distance testing in future methods
            if (mapData[doorCoordinates[1] + 1][doorCoordinates[0]] == " ")
            {
                mapData[doorCoordinates[1] + 1][doorCoordinates[0]] = "P";
                playerCoords[0] = doorCoordinates[0];
                playerCoords[1] = doorCoordinates[1] + 1;
            }
            else if (mapData[doorCoordinates[1] - 1][doorCoordinates[0]] == " ")
            {
                mapData[doorCoordinates[1] - 1][doorCoordinates[0]] = "P";
                playerCoords[0] = doorCoordinates[0];
                playerCoords[1] = doorCoordinates[1] - 1;
            }
            else if (mapData[doorCoordinates[1]][doorCoordinates[0] + 1] == " ")
            {
                mapData[doorCoordinates[1]][doorCoordinates[0] + 1] = "P";
                playerCoords[0] = doorCoordinates[0] + 1;
                playerCoords[1] = doorCoordinates[1];
            }
            else if (mapData[doorCoordinates[1]][doorCoordinates[0] - 1] == " ")
            {
                mapData[doorCoordinates[1]][doorCoordinates[0] - 1] = "P";
                playerCoords[0] = doorCoordinates[0] - 1;
                playerCoords[1] = doorCoordinates[1];
            }

            double angleRadians = Math.Atan2((doorCoordinates[1] - playerCoords[1]), (doorCoordinates[0] - playerCoords[0]));
            float angleDegrees = -(float)(angleRadians * 180 / Math.PI);
                        
            if (angleDegrees > 180)
            {
                angleDegrees = -90;
            }

            if (angleDegrees == 90 || angleDegrees == -90)
            {
                angleDegrees = -angleDegrees;
            }

            startRotation = angleDegrees;

            return mapData;
        }

        private async Task<string[][]> ChooseEndPosition(string[][] mapData)
        {
            await CleanListFromPlayerRadius(GoalDoors);

            int randomInt = random.Next(GoalDoors.Count);

            int[] doorCoordinates = GoalDoors[randomInt];
            GoalDoors.Remove(doorCoordinates);

            mapData[doorCoordinates[1]][doorCoordinates[0]] = "F";

            return mapData;
        }

        private async Task<string[][]> GenerateEnemies(string[][] mapData)
        {
            CleanListFromPlayerRadius(EnemyPositions);
                       

            for (int i = 0; i < EnemyCount; i++)
            {
                int randomInt = random.Next(EnemyPositions.Count);
                int[] enemyCoords = EnemyPositions[randomInt];
                EnemyPositions.Remove(enemyCoords);

                mapData[enemyCoords[1]][enemyCoords[0]] = "E";
            }

            for (int i = 0; i < collectablesCount; i++)
            {
                int randomInt = random.Next(EnemyPositions.Count);
                int[] collectableCoords = EnemyPositions[randomInt];
                EnemyPositions.Remove(collectableCoords);

                mapData[collectableCoords[1]][collectableCoords[0]] = "H";
            }

            for (int i = 0; i < collectablesCount; i++)
            {
                int randomInt = random.Next(EnemyPositions.Count);
                int[] collectableCoords = EnemyPositions[randomInt];
                EnemyPositions.Remove(collectableCoords);

                mapData[collectableCoords[1]][collectableCoords[0]] = "C";
            }

            return mapData;
        }

        private async Task CleanListFromPlayerRadius(List<int[]> list)
        {
            Vector2 playerPos = new Vector2(playerCoords[0], playerCoords[1]);

            List<int[]> tempList = list.ToList();

            foreach (int[] coord in tempList)
            {
                Vector2 listPosition = new Vector2(coord[0], coord[1]);

                float distance = Vector2.Distance(playerPos, listPosition);

                if (distance < CHUNKSIZE)
                {
                    list.Remove(coord);
                }
            }
        }

        private void PrintMapToConsole(string[][] mapData) //For debugging in console application
        {
            foreach (string[] row in mapData)
            {
                foreach (string cell in row)
                {
                    if (cell == "n" || cell == "e" || cell == "w" || cell == "s")
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write($"{cell}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == "D")
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{cell}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == "P")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{cell}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == "F")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($"{cell}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == "E")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{cell}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write($"{cell}");
                    }

                }

                Console.Write("\n");
            }
        }
    }
}