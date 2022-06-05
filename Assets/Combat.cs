using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Combat
{
    public static GameObject[] CombatTerrains;

    public static GameObject CenterOfCombatTerrain;

    // create a temp TILE map array to hold the combat terrains as we load them
    public static Tile.TILE[][,] combatMaps = new Tile.TILE[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][,];
    public static Combat.CombatMonsterStartPositions[][] combatMonsterStartPositions = new Combat.CombatMonsterStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];
    public static Combat.CombatPartyStartPositions[][] combatPartyStartPositions = new Combat.CombatPartyStartPositions[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX][];

    // unfortuantly the game engine never saves this information after loading the combat terrain in function C_7C65()
    // the code is not re-entrant so I cannot just expose and call the function directly so I need to re-implement the 
    // logic here so I can on the fly determine the combat terrain to display by exposing the interal variables used in the
    // original function. The INN or shop or camp case is handled elsewhere (e.g. In the middle of the night, while out for a stroll...)
    public static U4_Decompiled_AVATAR.COMBAT_TERRAIN Convert_Tile_to_Combat_Terrian(Tile.TILE underPartyTile, Tile.TILE partyTile, Tile.TILE monsterTile, Tile.TILE underMonsterTile)
    {
        U4_Decompiled_AVATAR.COMBAT_TERRAIN combat_terrain;

        if (partyTile <= Tile.TILE.SHIP_SOUTH || (Tile.TILE)((byte)underPartyTile & ~3) == Tile.TILE.SHIP_WEST)
        {
            if (monsterTile == Tile.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSHIP;
            }
            else if (underMonsterTile <= Tile.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSEA;
            }
            else
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHIPSHOR;
            }
        }
        else
        {
            if (monsterTile == Tile.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORSHIP;
            }
            else if (underMonsterTile <= Tile.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.SHORE;
            }
            else
            {
                switch (underPartyTile)
                {
                    case Tile.TILE.SWAMP:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.MARSH;
                            break;
                        }
                    case Tile.TILE.BRUSH:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRUSH;
                            break;
                        }
                    case Tile.TILE.FOREST:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.FOREST;
                            break;
                        }
                    case Tile.TILE.HILLS:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.HILL;
                            break;
                        }
                    case Tile.TILE.DUNGEON:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.DUNGEON;
                            break;
                        }
                    case Tile.TILE.BRICK_FLOOR:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRICK;
                            break;
                        }
                    case Tile.TILE.BRIDGE:
                    case Tile.TILE.BRIDGE_TOP:
                    case Tile.TILE.BRIDGE_BOTTOM:
                    case Tile.TILE.WOOD_FLOOR:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.BRIDGE;
                            break;
                        }
                    default:
                        {
                            combat_terrain = U4_Decompiled_AVATAR.COMBAT_TERRAIN.GRASS;
                            break;
                        }
                }
            }
        }

        return combat_terrain;
    }

    public struct CombatMonsterStartPositions
    {
        public int start_x;
        public int start_y;
    }

    public struct CombatPartyStartPositions
    {
        public int start_x;
        public int start_y;
    }
    public static void LoadCombatMap(string combatMapFilepath,
        ref Tile.TILE[,] combatMap,
        ref CombatMonsterStartPositions[] monsterStartPositions,
        ref CombatPartyStartPositions[] partyStartPositions)
    {
        /*
        These files contain the 11x11 battleground maps shown when combat starts. It has the map itself plus starting positions for up to 16 monsters and 8 party members.
        Offset 	Length (in bytes) 	Purpose
        0x0 	16 	start_x for monsters 0-15
        0x10 	16 	start_y for monsters 0-15
        0x20 	8 	start_x for party members 0-7
        0x28 	8 	start_y for party members 0-7
        0x30 	16 	Purpose unknown; seems to be a constant: 08 AD 83 C0 AD 83 C0 AD 83 C0 A0 00 B9 A6 08 F0
        0x40 	121 	11x11 Map Matrix
        0xB9 	7 	Purpose unknown; seems to be a constant: 8D 00 00 00 00 47 09 
        */

        if (!System.IO.File.Exists(Application.persistentDataPath + combatMapFilepath))
        {
            Debug.Log("Could not find combat map file " + Application.persistentDataPath + combatMapFilepath);
            return;
        }

        // read the file
        byte[] combatMapFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + combatMapFilepath);

        if (combatMapFileData.Length != 0xc0)
        {
            Debug.Log("Combat map file incorrect length " + combatMapFileData.Length);
            return;
        }

        int fileIndex = 0;

        // the shrine does not have any start positions and omits those and goes right to the map,
        // the file size appear to be the same though
        if (combatMapFilepath != "/u4/SHRINE.CON")
        {
            for (int i = 0; i < 16; i++)
            {
                monsterStartPositions[i].start_x = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 16; i++)
            {
                monsterStartPositions[i].start_y = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 8; i++)
            {
                partyStartPositions[i].start_x = combatMapFileData[fileIndex++];
            }

            for (int i = 0; i < 8; i++)
            {
                partyStartPositions[i].start_y = combatMapFileData[fileIndex++];
            }

            // skip over
            fileIndex += 16;
        }

        // read in the tile map
        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                combatMap[x, y] = (Tile.TILE)combatMapFileData[fileIndex++];
            }
        }
    }

    public static void CreateCombatTerrains(int offset_z)
    {
        // create a game object to store the combat terrain game objects, this should be at the top with no parent same as the world
        GameObject combatTerrainsObject = new GameObject();
        combatTerrainsObject.name = "Combat Terrains";

        CombatTerrains = new GameObject[(int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX];

        // go through all the combat terrains and load their maps and create a game object to hold them
        // as a child of the above combat terrains game object
        for (int i = 0; i < (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.MAX; i++)
        {
            // allocate space for the individual map
            combatMaps[i] = new Tile.TILE[11, 11];
            // allocate space for the monster and party starting positions
            combatMonsterStartPositions[i] = new CombatMonsterStartPositions[16];
            combatPartyStartPositions[i] = new CombatPartyStartPositions[8];

            if (i == (int)U4_Decompiled_AVATAR.COMBAT_TERRAIN.CAMP_DNG)
            {
                // this one has a different name format
                LoadCombatMap("/u4/CAMP.DNG",
                    ref combatMaps[i],
                    ref combatMonsterStartPositions[i],
                    ref combatPartyStartPositions[i]);
            }
            else
            {
                // load the combat map from the original files
                LoadCombatMap("/u4/" + ((U4_Decompiled_AVATAR.COMBAT_TERRAIN)i).ToString() + ".CON",
                    ref combatMaps[i],
                    ref combatMonsterStartPositions[i],
                    ref combatPartyStartPositions[i]);
            }

            // create a game object to hold it and set it as a child of the combat terrains game object
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(combatTerrainsObject.transform);

            // set it's name to match the combat terrain being created
            gameObject.name = ((U4_Decompiled_AVATAR.COMBAT_TERRAIN)i).ToString();

            // create the combat terrain based on the loaded map
            Map.CreateMap(gameObject, combatMaps[i]);

            // Disable it initially
            gameObject.SetActive(false);

            // Position the combat map in place
            gameObject.transform.position = new Vector3(0, 0, offset_z - combatMaps[i].GetLength(1));

            // rotate map into place
            gameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            // save the game object in the array
            CombatTerrains[i] = gameObject;
        }

        CenterOfCombatTerrain = new GameObject();
        CenterOfCombatTerrain.transform.position = new Vector3(5f, 0, 250f);
        CenterOfCombatTerrain.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        CenterOfCombatTerrain.transform.SetParent(combatTerrainsObject.transform);
    }
}
