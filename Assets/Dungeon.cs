using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dungeon
{   
    // TODO figure out how many can be in the dungeon level, for now assume 16 max
    public const int MAX_DUNGEON_MONSTERS = 16;
    public const int MAX_DUNGEON_ROOM_MONSTERS = 16;

    public static DUNGEON[] dungeons = new DUNGEON[(int)DUNGEONS.MAX];

    public static DungeonBlockLevel[,] currentDungeonBlockLevel = new DungeonBlockLevel[8, 8];

    public struct DungeonBlockLevel
    {
        public Tile.TILE[,] dungeonBlockMap;
        public GameObject dungeonGameObject;
        public Dungeon.DUNGEON_TILE dungeonTile;
    }

    // only the upper nibble defines the dungeon tile, the lower nibble is used for active dungeon monsters
    public static Combat.COMBAT_TERRAIN[] convertDungeonTileToCombat =
    {
        Combat.COMBAT_TERRAIN.DNG0, // HALLWAY -> hallway
        Combat.COMBAT_TERRAIN.DNG1, // LADDER_UP -> ladder up
        Combat.COMBAT_TERRAIN.DNG2, // LADDER_DOWN -> ladder down
        Combat.COMBAT_TERRAIN.DNG3, // LADDER_UP_AND_DOWN -> ladder up and down
        Combat.COMBAT_TERRAIN.DNG4, // TREASURE_CHEST -> chest
        Combat.COMBAT_TERRAIN.DNG0, // CEILING_HOLE -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // FLOOR_HOLE -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // MAGIC_ORB -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // TRAP -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // FOUNTAIN -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // FIELD -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // ALTAR -> hallway
        Combat.COMBAT_TERRAIN.DNG5, // DOOR -> doorway 
        Combat.COMBAT_TERRAIN.DNG0, // DUNGEON_ROOM -> hallway
        Combat.COMBAT_TERRAIN.DNG0, // DOOR_SECRECT -> secret doorway  /* DNG6 make secret door just regular hallway as we do the secret door/wall on the ajacent room/hallway */
        Combat.COMBAT_TERRAIN.DNG0  // WALL -> hallway, just in case
    };


    // These are different than the map tiles
    // this is needed to determine which combat map to use for which dungeon tile
    /*
        DNG0 = 16, // hallway
        DNG1 = 17, // ladder up
        DNG2 = 18, // ladder down
        DNG3 = 19, // ladder up and down
        DNG4 = 20, // chest
        DNG5 = 21, // doorway
        DNG6 = 22, // secret doorway
    */
    /*
        HALLWAY = 0x00,
        LADDER_UP = 0x10, 
        LADDER_DOWN = 0x20, 
        LADDER_UP_AND_DOWN = 0x30, 
        TREASURE_CHEST = 0x40, 
        CEILING_HOLE = 0x50,
        FLOOR_HOLE = 0x60,
        MAGIC_ORB = 0x70,
        TRAP = 0x80,
        FOUNTAIN = 0x90,
        FIELD = 0xa0,
        ALTAR = 0xB0, 
        DOOR = 0xC0, 
        DUNGEON_ROOM = 0xd0,
        DOOR_SECRECT = 0xE0, 
        WALL = 0xF0
    */
    public enum DUNGEON_TILE
    {
        HALLWAY = 0x00,
        LADDER_UP = 0x10, // 	Ladder Up
        LADDER_DOWN = 0x20, // 	Ladder Down
        LADDER_UP_AND_DOWN = 0x30, // 	Laddr Up & Down
        TREASURE_CHEST = 0x40, // 	Treasure Chest
        CEILING_HOLE = 0x50, //	Ceiling Hole
        FLOOR_HOLE = 0x60, // 	Floor Hole (unused)
        MAGIC_ORB = 0x70, // 	Magic Orb
        TRAP = 0x80,
        TRAP_WIND_DARKNESS = 0x80, // 	Winds/Darknes Trap
        TRAP_FALLING_ROCKS = 0x81, // 	Falling Rock Trap
        TRAP_PIT = 0x8E, //	Pit Trap
        FOUNTAIN = 0x90, // 	Plain Fountain
        FOUNTAIN_HEALING = 0x91, // 	Healing Fountain
        FOUNTAIN_ACID = 0x92, // 	Acid Fountain
        FOUNTAIN_CURE = 0x93, // 	Cure Fountain
        FOUNTAIN_POISIN = 0x94, // 	Poison Fountain
        FIELD = 0xA0,
        FIELD_POISON = 0xA0, // Poison Field
        FIELD_ENERGY = 0xA1, //  Energy Field
        FIELD_FIRE = 0xA2, // Fire Field
        FIELD_SLEEP = 0xA3, //  Sleep Field
        ALTAR = 0xB0, // Altar
        DOOR = 0xC0, // Door
        DUNGEON_ROOM = 0xD0,
        DUNGEON_ROOM_0 = 0xD0, // D0-DF Dungeon Rooms 0-15
        DUNGEON_ROOM_1 = 0xD1,
        DUNGEON_ROOM_2 = 0xD2,
        DUNGEON_ROOM_3 = 0xD3,
        DUNGEON_ROOM_4 = 0xD4,
        DUNGEON_ROOM_5 = 0xD5,
        DUNGEON_ROOM_6 = 0xD6,
        DUNGEON_ROOM_7 = 0xD7,
        DUNGEON_ROOM_8 = 0xD8,
        DUNGEON_ROOM_9 = 0xD9,
        DUNGEON_ROOM_10 = 0xDA,
        DUNGEON_ROOM_11 = 0xDB,
        DUNGEON_ROOM_12 = 0xDC,
        DUNGEON_ROOM_13 = 0xDD,
        DUNGEON_ROOM_14 = 0xDE,
        DUNGEON_ROOM_15 = 0xDF,
        DOOR_SECRECT = 0xE0, // Secret Door
        WALL = 0xF0 //  Wall
    }

    public enum DUNGEONS
    {
        DECEIT = 0,
        DESPISE = 1,
        DESTARD = 2,
        WRONG = 3,
        COVETOUS = 4,
        SHAME = 5,
        HYTHLOTH = 6,
        ABYSS = 7,
        MAX = 8
    };

    public struct FLOOR_TRIGGER
    {
        public Tile.TILE changeTile;
        public int trigger_x, trigger_y;
        public int changeTile_x1, changeTile_y1;
        public int changeTile_x2, changeTile_y2;
    }

    public struct DUNGEON_MONSTER
    {
        public Tile.TILE monster;
        public int x, y;
    }

    public struct DUNGEON_PARTY_START_LOCATION
    {
        public int x, y;
    }

    public struct DUNGEON_ROOM
    {
        public FLOOR_TRIGGER[] triggers; // (4 bytes each X 4 triggers possible)
        public DUNGEON_MONSTER[] monsters; // 16 of them, (0 means no monster and 0's come FIRST)
        public DUNGEON_PARTY_START_LOCATION[] partyNorthEntry; // 0-7 (north entry)
        public DUNGEON_PARTY_START_LOCATION[] partyEastEntry; // 0-7 (east entry)
        public DUNGEON_PARTY_START_LOCATION[] partySouthEntry; // 0-7 (south entry)
        public DUNGEON_PARTY_START_LOCATION[] partyWestEntry; // 0-7 (west entry)
        public Tile.TILE[,] dungeonRoomMap; // 11x11 map matrix for room
    }

    public struct DUNGEON
    {
        public string name;
        public DUNGEON_TILE[][,] dungeonTILEs; // 8x8x8 map
        public DUNGEON_ROOM[] dungeonRooms; // 16 or 64 rooms
    }

    public static void LoadDungeons()
    {
        for (int index = 0; index < (int)DUNGEONS.MAX; index++)
        {
            int rooms = 0;
            dungeons[index].name = ((DUNGEONS)index).ToString();
            dungeons[index].dungeonTILEs = new DUNGEON_TILE[8][,];
            if (index == (int)DUNGEONS.ABYSS)
            {
                rooms = 64;
            }
            else
            {
                rooms = 16;
            }

            if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG"))
            {
                Debug.Log("Could not find dungeon file " + Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG");
                return;
            }

            // read the file
            byte[] dungeonFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + (DUNGEONS)index + ".DNG");

            if (dungeonFileData.Length != 0x200 + 0x100 * rooms)
            {
                Debug.Log("dungeon file incorrect length " + dungeonFileData.Length);
                return;
            }

            int fileIndex = 0;

            for (int level = 0; level < 8; level++)
            {
                dungeons[index].dungeonTILEs[level] = new DUNGEON_TILE[8, 8];
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        dungeons[index].dungeonTILEs[level][x, 7 - y] = (DUNGEON_TILE)dungeonFileData[fileIndex++];
                    }
                }
            }

            dungeons[index].dungeonRooms = new DUNGEON_ROOM[rooms];

            for (int room = 0; room < rooms; room++)
            {
                dungeons[index].dungeonRooms[room].triggers = new FLOOR_TRIGGER[4];
                dungeons[index].dungeonRooms[room].monsters = new DUNGEON_MONSTER[MAX_DUNGEON_ROOM_MONSTERS];
                dungeons[index].dungeonRooms[room].partyNorthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyEastEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partySouthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyWestEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].dungeonRoomMap = new Tile.TILE[11, 11];

                // get the triggers
                for (int i = 0; i < 4; i++)
                {
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile = (Tile.TILE)dungeonFileData[fileIndex++];
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_x = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_y = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x1 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y1 = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x2 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y2 = dungeonFileData[fileIndex++] & 0xf;
                }

                // get the monsters
                for (int i = 0; i < MAX_DUNGEON_ROOM_MONSTERS; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].monster = (Tile.TILE)dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < MAX_DUNGEON_ROOM_MONSTERS; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < MAX_DUNGEON_ROOM_MONSTERS; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].y = dungeonFileData[fileIndex++];
                }

                // get party start positions for each room entry direction
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyNorthEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyNorthEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyEastEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyEastEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partySouthEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partySouthEntry[i].y = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyWestEntry[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 8; i++)
                {
                    dungeons[index].dungeonRooms[room].partyWestEntry[i].y = dungeonFileData[fileIndex++];
                }

                for (int y = 0; y < 11; y++)
                {
                    for (int x = 0; x < 11; x++)
                    {
                        dungeons[index].dungeonRooms[room].dungeonRoomMap[x, y] = (Tile.TILE)dungeonFileData[fileIndex++];
                    }
                }

                fileIndex += 7; // skip over unused buffer
            }
        }
    }


    public static void AddDungeonRoomMonsters(GameObject dungeonRoomGameObject, ref Dungeon.DUNGEON_ROOM dungeonRoom)
    {
        GameObject monstersGameObject = new GameObject("Monsters");
        monstersGameObject.transform.SetParent(dungeonRoomGameObject.transform);

        // add all the monsters
        for (int i = 0; i < Dungeon.MAX_DUNGEON_ROOM_MONSTERS; i++)
        {
            Tile.TILE monsterTile = dungeonRoom.monsters[i].monster;

            if (monsterTile != 0)
            {
                GameObject monsterGameObject = Primitive.CreateQuad();
                monsterGameObject.name = monsterTile.ToString();

                // get the renderer
                MeshRenderer renderer = monsterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // there is at least one case where the dungeon monster tile refers to an energy field.
                // TODO: see if these are actually monsters or just static objects in the actual game,
                // for now billboard them like actual monsters.
                if ((monsterTile >= Tile.TILE.POISON_FIELD) && (monsterTile <= Tile.TILE.SLEEP_FIELD))
                {
                    renderer.material.mainTexture = Tile.combinedLinearTexture;
                    renderer.material.mainTextureOffset = new Vector2((float)((int)monsterTile * Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                    renderer.material.mainTextureScale = new Vector2((float)Tile.originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);

                    Animate1 animate = monsterGameObject.AddComponent<Animate1>();
                }
                else
                {
                    // add our little animator script and set the tile
                    Animate3 animate = monsterGameObject.AddComponent<Animate3>();
                    animate.npcTile = 0;
                    animate.ObjectRenderer = renderer;

                    animate.SetNPCTile(monsterTile);
                }

                // rotate the monster game object into position after creating
                monsterGameObject.transform.position = new Vector3(dungeonRoom.monsters[i].x, 10 - dungeonRoom.monsters[i].y, 0);
                monsterGameObject.transform.eulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // make it billboard
                //Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                //look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                //monsterGameObject.transform.LookAt(look.transform);
                //Vector3 rot = monsterGameObject.transform.eulerAngles;
                //monsterGameObject.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                monsterGameObject.transform.LookAt(look.transform);
                Vector3 rot = monsterGameObject.transform.eulerAngles;
                monsterGameObject.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

                // set this as a parent of the monsters game object
                monsterGameObject.transform.SetParent(monstersGameObject.transform);
            }
        }
    }

    public static void UpdateExistingBillboardsDungeonRoomMonster(GameObject dungeonRoomGameObject)
    {
        GameObject billboardGameObject;

        // create the billboard child object if it does not exist
        Transform billboardTransform = dungeonRoomGameObject.transform.Find("Monsters");
        if (billboardTransform == null)
        {
            billboardGameObject = new GameObject("Monsters");
            billboardGameObject.transform.SetParent(dungeonRoomGameObject.transform);
            billboardGameObject.transform.localPosition = Vector3.zero;
            billboardGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardGameObject = billboardTransform.gameObject;
        }

        // update any children
        foreach (Transform child in billboardGameObject.transform)
        {
            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            child.transform.LookAt(look.transform);
            Vector3 rot = child.transform.eulerAngles;
            child.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }
    }

    public static GameObject CreateDungeonRoom(ref DUNGEON_ROOM dungeonRoom)
    {
        GameObject mapGameObject = new GameObject();
        Map.CreateMap(mapGameObject, dungeonRoom.dungeonRoomMap, Vector3.zero, Vector3.zero);
        AddDungeonRoomMonsters(mapGameObject, ref dungeonRoom);
        return mapGameObject;
    }

    public static void CreateDungeonRooms(GameObject dungeonsRoomsObject)
    {
        for (int i = 0; i < (int)DUNGEONS.MAX; i++)
        {
            for (int room = 0; room < dungeons[i].dungeonRooms.Length; room++)
            {
                GameObject dungeonRoomObject = CreateDungeonRoom(ref dungeons[i].dungeonRooms[room]);
                dungeonRoomObject.transform.SetParent(dungeonsRoomsObject.transform);
                dungeonRoomObject.name = ((DUNGEONS)i).ToString() + " room #" + room;
                dungeonRoomObject.transform.localPosition = new Vector3((room % 16) * 11, 0, (i + (room / 16)) * 11);
                dungeonRoomObject.transform.eulerAngles = new Vector3(90.0f, 0f, 0f);
            }
        }
    }

    public static void CreateDungeons(GameObject dungeonsGameObject)
    {
        for (int index = 0; index < (int)DUNGEONS.MAX; index++)
        {
            GameObject dungeonGameObject = new GameObject();
            dungeonGameObject.name = ((DUNGEONS)index).ToString();
            dungeonGameObject.transform.SetParent(dungeonsGameObject.transform);

            for (int z = 0; z < 8; z++)
            {
                GameObject dungeonLevelGameObject = new GameObject();
                dungeonLevelGameObject.name = ((DUNGEONS)index).ToString() + " Level #" + z;
                dungeonLevelGameObject.transform.SetParent(dungeonGameObject.transform);
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        GameObject mapTile;
                        Tile.TILE tileIndex;
                        DUNGEON_TILE dungeonTile = dungeons[index].dungeonTILEs[z][x, y];

                        if (dungeonTile == DUNGEON_TILE.WALL)
                        {
                            mapTile = Primitive.CreatePartialCube(true, true, true, true);
                            tileIndex = Tile.TILE.BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.HALLWAY)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.TILED_FLOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.LADDER_UP;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_DOWN)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.LADDER_DOWN;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP_AND_DOWN)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.LADDER_DOWN; // TODO: need to overlap the up and down tiles, but this will do for now
                        }
                        else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.CHEST;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.FOUNTAIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_CURE) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_HEALING) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_POISIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_ACID))
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.SHALLOW_WATER;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.ENERGY_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.FIRE_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.POISON_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.SLEEP_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.DOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR_SECRECT)
                        {
                            mapTile = Primitive.CreatePartialCube(true, true, true, true);
                            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.ALTAR)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.ALTAR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.MISSLE_ATTACK_BLUE;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_PIT))
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.TILED_FLOOR;
                        }
                        else
                        {
                            mapTile = Primitive.CreateQuad();
                            tileIndex = Tile.TILE.TILED_FLOOR;
                        }

                        mapTile.transform.SetParent(dungeonLevelGameObject.transform);
                        mapTile.transform.localPosition = new Vector3(x, y, 7 - z);
                        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
                        renderer.material.mainTexture = Tile.expandedTiles[(int)tileIndex];
                        renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                    }
                }
            }

            // rotate dungeon into place
            dungeonGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }
    }

    public static Tile.TILE[,] CreateDungeonHallway(ref DUNGEON_TILE[,] dungeonTileMap, ref DUNGEON_ROOM[] dungeonRooms, int posx, int posy, int posz,
        Tile.TILE centerTile = Tile.TILE.TILED_FLOOR)
    {
        DUNGEON_TILE leftUncleared = dungeonTileMap[posx, (posy + 1) % 8]; //  0,-1
        DUNGEON_TILE aboveUncleared = dungeonTileMap[(posx + 8 - 1) % 8, posy]; // -1, 0
        DUNGEON_TILE rightUncleared = dungeonTileMap[(posx + 1) % 8, posy];     //  1, 0
        DUNGEON_TILE belowUncleared = dungeonTileMap[posx, (posy + 8 - 1) % 8];     //  0, 1

        DUNGEON_TILE left = (DUNGEON_TILE)((int)leftUncleared & 0xf0); //  0,-1
        DUNGEON_TILE above = (DUNGEON_TILE)((int)aboveUncleared & 0xf0); // -1, 0
        DUNGEON_TILE right = (DUNGEON_TILE)((int)rightUncleared & 0xf0);     //  1, 0
        DUNGEON_TILE below = (DUNGEON_TILE)((int)belowUncleared & 0xf0);     //  0, 1

        DUNGEON_TILE diagonalLeftBelow = (DUNGEON_TILE)((int)dungeonTileMap[(posx + 8 - 1) % 8, (posy + 8 - 1) % 8] & 0xf0);  //  0,-1 ->  -1, 1
        DUNGEON_TILE diagonalRightBelow = (DUNGEON_TILE)((int)dungeonTileMap[(posx + 1) % 8, (posy + 8 - 1) % 8] & 0xf0);  // -1, 0 ->   1, 1
        DUNGEON_TILE diagonalRightAbove = (DUNGEON_TILE)((int)dungeonTileMap[(posx + 1) % 8, (posy + 1) % 8] & 0xf0);      //  1, 0 ->   1,-1
        DUNGEON_TILE diagonalLeftAbove = (DUNGEON_TILE)((int)dungeonTileMap[(posx + 8 - 1) % 8, (posy + 1) % 8] & 0xf0);   //  0, 1 ->  -1,-1

        Tile.TILE tileIndex;

        Tile.TILE[,] map = new Tile.TILE[11, 11];

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                tileIndex = Tile.TILE.TILED_FLOOR;

                // center
                if ((x == 5) && (y == 5))
                {
                    tileIndex = centerTile;
                }

                // walls
                if (((left == DUNGEON_TILE.WALL) || (left == DUNGEON_TILE.DOOR_SECRECT)) && (y == 1))
                {
                    if ((x == 5) && (left == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = Tile.TILE.BRICK_WALL;
                    }
                }
                if (((above == DUNGEON_TILE.WALL) || (above == DUNGEON_TILE.DOOR_SECRECT)) && (x == 1))
                {
                    if ((y == 5) && (above == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = Tile.TILE.BRICK_WALL;
                    }
                }
                if (((right == DUNGEON_TILE.WALL) || (right == DUNGEON_TILE.DOOR_SECRECT)) && (x == 9))
                {
                    if ((y == 5) && (right == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = Tile.TILE.BRICK_WALL;
                    }
                }
                if (((below == DUNGEON_TILE.WALL) || (below == DUNGEON_TILE.DOOR_SECRECT)) && (y == 9))
                {
                    if ((x == 5) && (below == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = Tile.TILE.BRICK_WALL;
                    }
                }

                // Dungeon Rooms
                // NOTE: rooms may not have a visible entrance so put a secret door on the wall in the middle
                // NOTE: not so sure how this works if the passage is on one side of the wall,
                // or do we wait for player to trigger the openning?
                // TODO: maybe need to remember if there is a passage or not before putting in the secret wall
                if ((left == DUNGEON_TILE.DUNGEON_ROOM_0) && (y == 0))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)leftUncleared - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    Tile.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 10];
                    if ((roomTileIndex == Tile.TILE.TILED_FLOOR) ||
                        (roomTileIndex == Tile.TILE.BRICK_FLOOR) ||
                        (roomTileIndex == Tile.TILE.WOOD_FLOOR) ||
                        (roomTileIndex == Tile.TILE.SLEEP_FIELD) ||
                        (roomTileIndex == Tile.TILE.FIRE_FIELD) ||
                        (roomTileIndex == Tile.TILE.POISON_FIELD) ||
                        (roomTileIndex == Tile.TILE.ENERGY_FIELD) ||
                        (roomTileIndex == Tile.TILE.GRASS) ||
                        (roomTileIndex == Tile.TILE.FOREST) ||
                        (roomTileIndex == Tile.TILE.HILLS) ||
                        (roomTileIndex == Tile.TILE.SWAMP) ||
                        (roomTileIndex == Tile.TILE.BRUSH) ||
                        (roomTileIndex == Tile.TILE.BRIDGE) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_BOTTOM) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_TOP))
                    {
                        tileIndex = Tile.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        if (x == 5)
                        {
                            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                        }
                        else
                        {
                            tileIndex = Tile.TILE.BRICK_WALL;
                        }
                    }
                }
                if ((above == DUNGEON_TILE.DUNGEON_ROOM) && (x == 0))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)aboveUncleared - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    Tile.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[10, y];
                    if ((roomTileIndex == Tile.TILE.TILED_FLOOR) ||
                        (roomTileIndex == Tile.TILE.BRICK_FLOOR) ||
                        (roomTileIndex == Tile.TILE.WOOD_FLOOR) ||
                        (roomTileIndex == Tile.TILE.SLEEP_FIELD) ||
                        (roomTileIndex == Tile.TILE.FIRE_FIELD) ||
                        (roomTileIndex == Tile.TILE.POISON_FIELD) ||
                        (roomTileIndex == Tile.TILE.ENERGY_FIELD) ||
                        (roomTileIndex == Tile.TILE.GRASS) ||
                        (roomTileIndex == Tile.TILE.FOREST) ||
                        (roomTileIndex == Tile.TILE.HILLS) ||
                        (roomTileIndex == Tile.TILE.SWAMP) ||
                        (roomTileIndex == Tile.TILE.BRUSH) ||
                        (roomTileIndex == Tile.TILE.BRIDGE) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_BOTTOM) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_TOP))
                    {
                        tileIndex = Tile.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        if (y == 5)
                        {
                            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                        }
                        else
                        {
                            tileIndex = Tile.TILE.BRICK_WALL;
                        }
                    }
                }
                if ((right == DUNGEON_TILE.DUNGEON_ROOM) && (x == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)rightUncleared - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    Tile.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[0, y];
                    if ((roomTileIndex == Tile.TILE.TILED_FLOOR) ||
                        (roomTileIndex == Tile.TILE.BRICK_FLOOR) ||
                        (roomTileIndex == Tile.TILE.WOOD_FLOOR) ||
                        (roomTileIndex == Tile.TILE.SLEEP_FIELD) ||
                        (roomTileIndex == Tile.TILE.FIRE_FIELD) ||
                        (roomTileIndex == Tile.TILE.POISON_FIELD) ||
                        (roomTileIndex == Tile.TILE.ENERGY_FIELD) ||
                        (roomTileIndex == Tile.TILE.GRASS) ||
                        (roomTileIndex == Tile.TILE.FOREST) ||
                        (roomTileIndex == Tile.TILE.HILLS) ||
                        (roomTileIndex == Tile.TILE.SWAMP) ||
                        (roomTileIndex == Tile.TILE.BRUSH) ||
                        (roomTileIndex == Tile.TILE.BRIDGE) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_BOTTOM) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_TOP))
                    {
                        tileIndex = Tile.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        if (y == 5)
                        {
                            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                        }
                        else
                        {
                            tileIndex = Tile.TILE.BRICK_WALL;
                        }
                    }
                }
                if ((below == DUNGEON_TILE.DUNGEON_ROOM) && (y == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)belowUncleared - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    Tile.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 0];
                    if ((roomTileIndex == Tile.TILE.TILED_FLOOR) ||
                        (roomTileIndex == Tile.TILE.BRICK_FLOOR) ||
                        (roomTileIndex == Tile.TILE.WOOD_FLOOR) ||
                        (roomTileIndex == Tile.TILE.SLEEP_FIELD) ||
                        (roomTileIndex == Tile.TILE.FIRE_FIELD) ||
                        (roomTileIndex == Tile.TILE.POISON_FIELD) ||
                        (roomTileIndex == Tile.TILE.ENERGY_FIELD) ||
                        (roomTileIndex == Tile.TILE.GRASS) ||
                        (roomTileIndex == Tile.TILE.FOREST) ||
                        (roomTileIndex == Tile.TILE.HILLS) ||
                        (roomTileIndex == Tile.TILE.SWAMP) ||
                        (roomTileIndex == Tile.TILE.BRUSH) ||
                        (roomTileIndex == Tile.TILE.BRIDGE) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_BOTTOM) ||
                        (roomTileIndex == Tile.TILE.BRIDGE_TOP))
                    {
                        tileIndex = Tile.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        if (x == 5)
                        {
                            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
                        }
                        else
                        {
                            tileIndex = Tile.TILE.BRICK_WALL;
                        }
                    }
                }

                //corners
                if ((x <= 1) && (y <= 1) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y <= 1) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y >= 9) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BRICK_WALL;
                }
                if ((x <= 1) && (y >= 9) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BRICK_WALL;
                }
                if ((x == 0) && (y == 0) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((x == 10) && (y == 0) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((x == 10) && (y == 10) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((x == 0) && (y == 10) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = Tile.TILE.BLANK;
                }

                // override
                if ((left == DUNGEON_TILE.WALL) && (y == 0))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((above == DUNGEON_TILE.WALL) && (x == 0))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((right == DUNGEON_TILE.WALL) && (x == 10))
                {
                    tileIndex = Tile.TILE.BLANK;
                }
                if ((below == DUNGEON_TILE.WALL) && (y == 10))
                {
                    tileIndex = Tile.TILE.BLANK;
                }

                map[x, y] = tileIndex;
            }
        }

        return map;
    }

    public static GameObject CreateDungeonBlock(Tile.TILE tileIndex)
    {
        GameObject dungeonBlockGameObject = new GameObject();
        dungeonBlockGameObject.name = tileIndex.ToString();

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                GameObject mapTile;

                if (tileIndex == Tile.TILE.BRICK_WALL)
                {
                    mapTile = Primitive.CreatePartialCube(true, true, true, true);
                }
                else if (tileIndex == Tile.TILE.TILED_FLOOR)
                {
                    mapTile = Primitive.CreateQuad();
                }
                else
                {
                    mapTile = Primitive.CreateQuad();
                }

                mapTile.transform.SetParent(dungeonBlockGameObject.transform);
                mapTile.transform.localPosition = new Vector3(x, y, 0);
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.mainTexture = Tile.expandedTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
            }
        }

        // rotate dungeon into place
        // do this after creating all the blocks
        //dungeonBlockGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonBlockGameObject;
    }

    public static GameObject CreateDungeonExpandedLevel(DUNGEONS dungeon, int level, Tile.TILE[][,] combatMaps)
    {
        GameObject dungeonLevel = new GameObject();
        dungeonLevel.name = dungeon.ToString() + " Level #" + level;


        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject dungeonBlockGameObject;

                DUNGEON_TILE dungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, y];
                DUNGEON_TILE aboveDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, (y - 1 + 8) % 8];
                DUNGEON_TILE belowDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][x, (y + 1) % 8];
                DUNGEON_TILE leftDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][(x - 1 + 8) % 8, y];
                DUNGEON_TILE rightDungeonTile = dungeons[(int)dungeon].dungeonTILEs[level][(x + 1) % 8, y];

                Tile.TILE[,] map = null;

                if (dungeonTile == DUNGEON_TILE.WALL)
                {
                    // need to set this otherwise it shows hallway
                    currentDungeonBlockLevel[x, y].dungeonTile = dungeonTile;

                    // we won't bother creating anything for walls as hallways already are complete
                    continue;
                }
                else if ((dungeonTile >= DUNGEON_TILE.DUNGEON_ROOM_0) &&
                        (dungeonTile <= DUNGEON_TILE.DUNGEON_ROOM_15))
                {
                    int room;
                    // special case the ABYSS as it has 64 rooms instead of 16 like the others
                    if (dungeon == DUNGEONS.ABYSS)
                    {
                        room = (int)dungeonTile - (int)DUNGEON_TILE.DUNGEON_ROOM_0 + (level >> 1) * 16;
                    }
                    else
                    {
                        room = (int)dungeonTile - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    }
                    dungeonBlockGameObject = CreateDungeonRoom(ref dungeons[(int)dungeon].dungeonRooms[room]);
                    dungeonBlockGameObject.name = "Room #" + room;

                    map = dungeons[(int)dungeon].dungeonRooms[room].dungeonRoomMap;
                }
                else if ((dungeonTile == DUNGEON_TILE.HALLWAY)
                        || (dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS)
                        || (dungeonTile == DUNGEON_TILE.TRAP_PIT)
                        || (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS)
                        )
                {
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.TILED_FLOOR);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if ((dungeonTile == DUNGEON_TILE.FOUNTAIN)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_ACID)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_CURE)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_HEALING)
                        || (dungeonTile == DUNGEON_TILE.FOUNTAIN_POISIN))
                {
                    // TODO make a pretty fountain
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.SHALLOW_WATER);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                {
                    // TODO make some kind of field in the room
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.ENERGY_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                {
                    // TODO make some kind of field in the room
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.FIRE_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                {
                    // TODO make some kind of field in the room
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.POISON_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                {
                    // TODO make a pretty fountain
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.SLEEP_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                {
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.CHEST);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                {
                    // TODO make orb into a billboard and make some kind of structure
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.MISSLE_ATTACK_BLUE);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();

                }
                else if (dungeonTile == DUNGEON_TILE.ALTAR)
                {
                    // TODO make altar into a billboard
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, Tile.TILE.ALTAR);

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else
                {
                    // use a combat map as the dungeon room base on the dungeon tile
                    int combat = (int)convertDungeonTileToCombat[(int)dungeons[(int)dungeon].dungeonTILEs[level][x, y] >> 4];

                    // get a halway that fits
                    map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level);

                    // check if we need to flip the door map
                    if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                        (aboveDungeonTile == DUNGEON_TILE.WALL) &&
                        (belowDungeonTile == DUNGEON_TILE.WALL) &&
                        (leftDungeonTile != DUNGEON_TILE.WALL) &&
                        (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy the center of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x]; // need to flip the door map
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                        (aboveDungeonTile == DUNGEON_TILE.WALL) &&
                        (belowDungeonTile != DUNGEON_TILE.WALL) &&
                        (leftDungeonTile != DUNGEON_TILE.WALL) &&
                        (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 5; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                       (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                       (belowDungeonTile == DUNGEON_TILE.WALL) &&
                       (leftDungeonTile != DUNGEON_TILE.WALL) &&
                       (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 6; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_y, copy_x];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                      (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                      (belowDungeonTile != DUNGEON_TILE.WALL) &&
                      (leftDungeonTile == DUNGEON_TILE.WALL) &&
                      (rightDungeonTile != DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 5; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }
                    else if ((dungeonTile == DUNGEON_TILE.DOOR) &&
                      (aboveDungeonTile != DUNGEON_TILE.WALL) &&
                      (belowDungeonTile != DUNGEON_TILE.WALL) &&
                      (leftDungeonTile != DUNGEON_TILE.WALL) &&
                      (rightDungeonTile == DUNGEON_TILE.WALL))
                    {
                        // copy half of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 6; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }
                    else
                    {
                        // copy the center of the combat map into the hallway
                        for (int copy_y = 2; copy_y < 9; copy_y++)
                        {
                            for (int copy_x = 2; copy_x < 9; copy_x++)
                            {
                                map[copy_x, copy_y] = combatMaps[combat][copy_x, copy_y];
                            }
                        }
                    }

                    dungeonBlockGameObject = new GameObject();
                    Map.CreateMap(dungeonBlockGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));
                    dungeonBlockGameObject.name = "Combat map hallway " + ((Combat.COMBAT_TERRAIN)combat).ToString();

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);
                }

                dungeonBlockGameObject.transform.SetParent(dungeonLevel.transform);
                dungeonBlockGameObject.transform.localPosition = new Vector3(x * 11, y * 11, 0);
                currentDungeonBlockLevel[x, y].dungeonBlockMap = map;
                currentDungeonBlockLevel[x, y].dungeonGameObject = dungeonBlockGameObject;
                currentDungeonBlockLevel[x, y].dungeonTile = dungeonTile;
            }
        }

        // TODO cannot combine yet as the  areas are already combined, need to rebuild them into the dungeon
        //Combine(dungeonLevel);

        dungeonLevel.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonLevel;
    }
}
