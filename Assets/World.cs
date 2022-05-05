//#define CREATE_DUMMY_FONT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    // used for automatic klimb and decsend ladders
    public U4_Decompiled.TILE lastCurrentTile;

    // color pallettes and 2D tile textures
    public Color[] CGAColorPalette;
    public Color[] EGAColorPalette;
    public Texture2D[] originalTiles;
    public Texture2D[] expandedTiles;

    // mainTerrain holds the terrain, animatedTerrrain, billboardTerrrain
    public GameObject mainTerrain;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain;

    // these are fixed in space 
    public GameObject npcs;
    //public GameObject bubblePrefab;
    public GameObject party;
    public GameObject fighters;
    public GameObject characters;
    public GameObject activeCharacter;
    public GameObject hits;
    public GameObject moongate;
    public GameObject dungeon;

    public GameObject dungeonMonsters;
    public GameObject partyGameObject;

    //public GameObject[] Settlements;
    public GameObject[] CombatTerrains;

    public List<string> talkWordList = new List<string>();

    public string tileApple2Filepath1 = "/u4/SHP0.B"; 
    public string tileApple2Filepath2 = "/u4/SHP1.B";
    public string tileEGAFilepath = "/u4/SHAPES.EGA";
    public string tileCGAFilepath = "/u4/SHAPES.CGA";
    public string worldMapFilepath = "/u4/WORLD.MAP";

    public Text keyword1ButtonText;
    public Text keyword2ButtonText;
    public GameObject keyword1Button;
    public GameObject keyword2Button;
    public GameObject InputPanel;
    public GameObject Talk;
    public GameObject Action;
    public GameObject ActionMainLoop;
    public GameObject ActionDungeonLoop;
    public GameObject ActionCombatLoop;
    public GameObject TalkCitizen;
    public GameObject TalkHealer;
    public GameObject TalkContinue;
    public GameObject TalkYN;
    public GameObject TalkYesNo;
    public GameObject TalkBuySell;
    public GameObject TalkFoodAle;
    public GameObject TalkHawWind;
    public GameObject TalkPartyCharacter;
    public GameObject TalkArmor;
    public GameObject TalkWeapon;
    public GameObject TalkGuild;
    public GameObject Talk2DigitInput;
    public GameObject Talk3DigitInput;
    public GameObject TalkLordBritish;

    public GameObject TalkMantras;
    public GameObject Talk1DigitInput;
    public GameObject TalkColors;
    public GameObject TalkUseItem;
    public GameObject TalkSpells;
    public GameObject TalkEnergy;
    public GameObject TalkReagents;
    public GameObject TalkTelescope;
    public GameObject TalkPhase;

    public GameObject TalkPubWord;
    public GameObject TalkDigit0123;
    public GameObject TalkInfinity;
    public GameObject TalkVeramocor;
    public GameObject TalkVirtue;
    public GameObject TalkDirection;


    // reference to game engine
    public U4_Decompiled u4;

    // this array size can be adjusted to display more or less of the map at runtime
    U4_Decompiled.TILE[,] raycastSettlementMap = new U4_Decompiled.TILE[64, 64];

    // this array size can be adjusted to display more or less of the map at runtime
    U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[128, 128];
 //   U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[256, 256];

    GameObject CreatePartialCube(bool leftside = true, bool rightside = true, bool back = true, bool front = true)
    {
        // create a game object to hold the cube and give it a proper name
        GameObject partialCube = new GameObject("Partial Cube");
        partialCube.name = "Partial Cube";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = partialCube.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = partialCube.AddComponent<MeshRenderer>();

        // always have the top
        int sides = 1;
        if (leftside)
        {
            sides++;
        }
        if (rightside)
        {
            sides++;
        }
        if (back)
        {
            sides++;
        }
        if (front)
        {
            sides++;
        }

        // allocate just enough vertices
        Vector3[] verts = new Vector3[sides * 4];

        // variable index to walk through arrays depending on number of sides enabled
        int index = 0;

        // top 
        verts[index++] = new Vector3(-0.5f, -0.5f, -0.5f);
        verts[index++] = new Vector3(0.5f, -0.5f, -0.5f);
        verts[index++] = new Vector3(-0.5f, 0.5f, -0.5f);
        verts[index++] = new Vector3(0.5f, 0.5f, -0.5f);

        if (leftside)
        {
            verts[index++] = new Vector3(-0.5f, 0.5f, 0.5f);
            verts[index++] = new Vector3(-0.5f, -0.5f, 0.5f);
            verts[index++] = new Vector3(-0.5f, 0.5f, -0.5f);
            verts[index++] = new Vector3(-0.5f, -0.5f, -0.5f);
        }

        if (rightside)
        {
            verts[index++] = new Vector3(0.5f, -0.5f, 0.5f);
            verts[index++] = new Vector3(0.5f, 0.5f, 0.5f);
            verts[index++] = new Vector3(0.5f, -0.5f, -0.5f);
            verts[index++] = new Vector3(0.5f, 0.5f, -0.5f);
        }

        if (front)
        {
            verts[index++] = new Vector3(-0.5f, -0.5f, 0.5f);
            verts[index++] = new Vector3(0.5f, -0.5f, 0.5f);
            verts[index++] = new Vector3(-0.5f, -0.5f, -0.5f);
            verts[index++] = new Vector3(0.5f, -0.5f, -0.5f);
        }

        if (back)
        {
            verts[index++] = new Vector3(0.5f, 0.5f, 0.5f);
            verts[index++] = new Vector3(-0.5f, 0.5f, 0.5f);
            verts[index++] = new Vector3(0.5f, 0.5f, -0.5f);
            verts[index++] = new Vector3(-0.5f, 0.5f, -0.5f);
        }

        // get the current mesh
        Mesh mesh = meshFilter.sharedMesh;

        // if it is null create it
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }

        // make sure it is empty if we did not just create it above
        mesh.Clear();

        // save the vertices
        mesh.vertices = verts;

        int[] tris = new int[3 * 2 * sides]; // 3 verticies per triangle & 2 triangles per side

        // reset the array index
        index = 0;

        // dynamic offset adjusted depending on sides enabled as we add triangles
        int offset = 0;

        // top
        tris[index++] = 0; tris[index++] = 3; tris[index++] = 1;
        tris[index++] = 3; tris[index++] = 0; tris[index++] = 2;

        if (leftside)
        {
            tris[index++] = 4; tris[index++] = 7; tris[index++] = 5;
            tris[index++] = 7; tris[index++] = 4; tris[index++] = 6;
        }
        else
        {
            offset = -4;
        }

        if (rightside)
        {
            tris[index++] = 8 + offset; tris[index++] = 11 + offset; tris[index++] = 9 + offset;
            tris[index++] = 11 + offset; tris[index++] = 8 + offset; tris[index++] = 10 + offset;
        }
        else
        {
            offset = offset - 4;
        }

        if (front)
        {
            tris[index++] = 12 + offset; tris[index++] = 15 + offset; tris[index++] = 13 + offset;
            tris[index++] = 15 + offset; tris[index++] = 12 + offset; tris[index++] = 14 + offset;
        }
        else
        {
            offset = offset - 4;
        }

        if (back)
        {
            tris[index++] = 16 + offset; tris[index++] = 19 + offset; tris[index++] = 17 + offset;
            tris[index++] = 19 + offset; tris[index++] = 16 + offset; tris[index++] = 18 + offset;
        }

        // save the triangles
        mesh.triangles = tris;

        // create some uvs base on the number of vertices above
        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        // reset the array index
        index = 0;

        // top
        uvs[index++] = new Vector2(0, 0);
        uvs[index++] = new Vector2(1, 0);
        uvs[index++] = new Vector2(0, 1);
        uvs[index++] = new Vector2(1, 1);

        if (leftside)
        {
            uvs[index++] = new Vector2(0, 0);
            uvs[index++] = new Vector2(1, 0);
            uvs[index++] = new Vector2(0, 1);
            uvs[index++] = new Vector2(1, 1);
        }

        if (rightside)
        {
            uvs[index++] = new Vector2(0, 0);
            uvs[index++] = new Vector2(1, 0);
            uvs[index++] = new Vector2(0, 1);
            uvs[index++] = new Vector2(1, 1);
        }

        if (front)
        {
            uvs[index++] = new Vector2(0, 0);
            uvs[index++] = new Vector2(1, 0);
            uvs[index++] = new Vector2(0, 1);
            uvs[index++] = new Vector2(1, 1);
        }

        if (back)
        {
            uvs[index++] = new Vector2(0, 0);
            uvs[index++] = new Vector2(1, 0);
            uvs[index++] = new Vector2(0, 1);
            uvs[index++] = new Vector2(1, 1);
        }

        // TODO add bottom option or create a full cube so I can avoid using the unity cube for the active player

        // save the uvs
        mesh.uv = uvs;

        // finish up the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        // save the mesh
        meshFilter.mesh = mesh;

        // return the game object just like the built in primitive shape creators work
        return partialCube;
    }

    GameObject CreateBridgeUpper()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject bridge = new GameObject("Bridge Upper");
        bridge.name = "Bridge Upper";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = bridge.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = bridge.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.1875f, 0.5f),
            new Vector3(0.5f, -0.1875f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.1875f, 0.5f),
            new Vector3(-0.5f, -0.1875f, 0.5f)
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 7, 5,
            7, 4, 6,
            8, 11, 9,
            11, 8, 10,
            12, 15, 13,
            15, 12, 14
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0.6875f),
            new Vector2(1f, 0.6875f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0.375f),
            new Vector2(1f, 0.375f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0.375f),
            new Vector2(1f, 0.375f),
            new Vector2(0f, 0.6875f),
            new Vector2(1f, 0.6875f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };

        return bridge;
    }

    GameObject CreateBridgeLower()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject bridge = new GameObject("Bridge Lower");
        bridge.name = "Bridge Lower";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = bridge.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = bridge.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.1875f, -0.5f),
            new Vector3(0.5f, -0.1875f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.1875f, -0.5f),
            new Vector3(-0.5f, -0.1875f, -0.5f)
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 7, 5,
            7, 4, 6,
            8, 11, 9,
            11, 8, 10,
            12, 15, 13,
            15, 12, 14
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0.43f),
            new Vector2(1f, 0.43f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.875f),
            new Vector2(1f, 0.875f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.875f),
            new Vector2(1f, 0.875f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0.43f),
            new Vector2(1f, 0.43f),
        };

        return bridge;
    }

    GameObject CreatePillar()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject pillar = new GameObject("Pillar");
        pillar.name = "Pillar";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = pillar.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pillar.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.3566464f, -0.5f, -0.1268815f),
            new Vector3(-0.3033816f, -0.5f, -0.2314196f),
            new Vector3(-0.2204196f, -0.5f, -0.3143816f),
            new Vector3(-0.1158814f, -0.5f, -0.3676464f),
            new Vector3(0f, -0.5f, -0.3860002f),
            new Vector3(0.1158814f, -0.5f, -0.3676464f),
            new Vector3(0.2204196f, -0.5f, -0.3143815f),
            new Vector3(0.3033815f, -0.5f, -0.2314195f),
            new Vector3(0.3566463f, -0.5f, -0.1268814f),
            new Vector3(0.3750001f, -0.5f, -0.011f),
            new Vector3(0.3566463f, -0.5f, 0.1048814f),
            new Vector3(0.3033814f, -0.5f, 0.2094195f),
            new Vector3(0.2204195f, -0.5f, 0.2923814f),
            new Vector3(0.1158814f, -0.5f, 0.3456462f),
            new Vector3(1.117587E-08f, -0.5f, 0.3640001f),
            new Vector3(-0.1158814f, -0.5f, 0.3456462f),
            new Vector3(-0.2204195f, -0.5f, 0.2923814f),
            new Vector3(-0.3033814f, -0.5f, 0.2094195f),
            new Vector3(-0.3566462f, -0.5f, 0.1048814f),
            new Vector3(-0.375f, -0.5f, -0.011f),
            new Vector3(-0.3566464f, -0.4375f, -0.1268815f),
            new Vector3(-0.3033816f, -0.4375f, -0.2314196f),
            new Vector3(-0.2204196f, -0.4375f, -0.3143816f),
            new Vector3(-0.1158814f, -0.4375f, -0.3676464f),
            new Vector3(0f, -0.4375f, -0.3860002f),
            new Vector3(0.1158814f, -0.4375f, -0.3676464f),
            new Vector3(0.2204196f, -0.4375f, -0.3143815f),
            new Vector3(0.3033815f, -0.4375f, -0.2314195f),
            new Vector3(0.3566463f, -0.4375f, -0.1268814f),
            new Vector3(0.3750001f, -0.4375f, -0.011f),
            new Vector3(0.3566463f, -0.4375f, 0.1048814f),
            new Vector3(0.3033814f, -0.4375f, 0.2094195f),
            new Vector3(0.2204195f, -0.4375f, 0.2923814f),
            new Vector3(0.1158814f, -0.4375f, 0.3456462f),
            new Vector3(1.117587E-08f, -0.4375f, 0.3640001f),
            new Vector3(-0.1158814f, -0.4375f, 0.3456462f),
            new Vector3(-0.2204195f, -0.4375f, 0.2923814f),
            new Vector3(-0.3033814f, -0.4375f, 0.2094195f),
            new Vector3(-0.3566462f, -0.4375f, 0.1048814f),
            new Vector3(-0.375f, -0.4375f, -0.011f),
            new Vector3(0f, -0.5f, -0.011f),
            new Vector3(0f, -0.4375f, -0.011f),
            new Vector3(0.3750001f, -0.5f, -0.011f),
            new Vector3(0.3750001f, -0.4375f, -0.011f),
            new Vector3(-0.375f, -0.5f, -0.011f),
            new Vector3(-0.375f, -0.4375f, -0.011f),
            new Vector3(-0.3566464f, -0.4375f, -0.1268815f),
            new Vector3(-0.3566464f, -0.5f, -0.1268815f),
            new Vector3(-0.3033816f, -0.5f, -0.2314196f),
            new Vector3(-0.3566464f, -0.5f, -0.1268815f),
            new Vector3(-0.2204196f, -0.5f, -0.3143816f),
            new Vector3(-0.1158814f, -0.5f, -0.3676464f),
            new Vector3(0f, -0.5f, -0.3860002f),
            new Vector3(0.1158814f, -0.5f, -0.3676464f),
            new Vector3(0.2204196f, -0.5f, -0.3143815f),
            new Vector3(0.3033815f, -0.5f, -0.2314195f),
            new Vector3(0.3566463f, -0.5f, -0.1268814f),
            new Vector3(0.3750001f, -0.5f, -0.011f),
            new Vector3(0.3566463f, -0.5f, 0.1048814f),
            new Vector3(0.3033814f, -0.5f, 0.2094195f),
            new Vector3(0.2204195f, -0.5f, 0.2923814f),
            new Vector3(0.1158814f, -0.5f, 0.3456462f),
            new Vector3(1.117587E-08f, -0.5f, 0.3640001f),
            new Vector3(-0.1158814f, -0.5f, 0.3456462f),
            new Vector3(-0.2204195f, -0.5f, 0.2923814f),
            new Vector3(-0.3033814f, -0.5f, 0.2094195f),
            new Vector3(-0.3566462f, -0.5f, 0.1048814f),
            new Vector3(-0.375f, -0.5f, -0.011f),
            new Vector3(-0.3566464f, -0.4375f, -0.1268815f),
            new Vector3(-0.3033816f, -0.4375f, -0.2314196f),
            new Vector3(-0.2204196f, -0.4375f, -0.3143816f),
            new Vector3(-0.1158814f, -0.4375f, -0.3676464f),
            new Vector3(0f, -0.4375f, -0.3860002f),
            new Vector3(0.1158814f, -0.4375f, -0.3676464f),
            new Vector3(0.2204196f, -0.4375f, -0.3143815f),
            new Vector3(0.3033815f, -0.4375f, -0.2314195f),
            new Vector3(0.3566463f, -0.4375f, -0.1268814f),
            new Vector3(0.3750001f, -0.4375f, -0.011f),
            new Vector3(0.3566463f, -0.4375f, 0.1048814f),
            new Vector3(0.3033814f, -0.4375f, 0.2094195f),
            new Vector3(0.2204195f, -0.4375f, 0.2923814f),
            new Vector3(0.1158814f, -0.4375f, 0.3456462f),
            new Vector3(1.117587E-08f, -0.4375f, 0.3640001f),
            new Vector3(-0.1158814f, -0.4375f, 0.3456462f),
            new Vector3(-0.2204195f, -0.4375f, 0.2923814f),
            new Vector3(-0.3033814f, -0.4375f, 0.2094195f),
            new Vector3(-0.3566462f, -0.4375f, 0.1048814f),
            new Vector3(-0.375f, -0.4375f, -0.011f),
            new Vector3(-0.5f, -0.4999999f, -0.5000001f),
            new Vector3(0.5f, -0.4999999f, -0.5000001f),
            new Vector3(-0.5f, -0.5000001f, 0.5000001f),
            new Vector3(0.5f, -0.5000001f, 0.5000001f),
            new Vector3(-0.2972054f, -0.4375f, -0.09656787f),
            new Vector3(-0.252818f, -0.4375f, -0.183683f),
            new Vector3(-0.183683f, -0.4375f, -0.252818f),
            new Vector3(-0.09656786f, -0.4375f, -0.2972053f),
            new Vector3(0f, -0.4375f, -0.3125001f),
            new Vector3(0.09656786f, -0.4375f, -0.2972053f),
            new Vector3(0.183683f, -0.4375f, -0.2528179f),
            new Vector3(0.2528179f, -0.4375f, -0.1836829f),
            new Vector3(0.2972052f, -0.4375f, -0.09656783f),
            new Vector3(0.3125001f, -0.4375f, 0f),
            new Vector3(0.2972052f, -0.4375f, 0.09656783f),
            new Vector3(0.2528179f, -0.4375f, 0.1836829f),
            new Vector3(0.1836829f, -0.4375f, 0.2528178f),
            new Vector3(0.09656783f, -0.4375f, 0.2972052f),
            new Vector3(9.313226E-09f, -0.4375f, 0.3125f),
            new Vector3(-0.0965678f, -0.4375f, 0.2972052f),
            new Vector3(-0.1836829f, -0.4375f, 0.2528178f),
            new Vector3(-0.2528178f, -0.4375f, 0.1836829f),
            new Vector3(-0.2972052f, -0.4375f, 0.09656781f),
            new Vector3(-0.3125f, -0.4375f, 0f),
            new Vector3(-0.2972054f, -0.375f, -0.09656787f),
            new Vector3(-0.252818f, -0.375f, -0.183683f),
            new Vector3(-0.183683f, -0.375f, -0.252818f),
            new Vector3(-0.09656786f, -0.375f, -0.2972053f),
            new Vector3(0f, -0.375f, -0.3125001f),
            new Vector3(0.09656786f, -0.375f, -0.2972053f),
            new Vector3(0.183683f, -0.375f, -0.2528179f),
            new Vector3(0.2528179f, -0.375f, -0.1836829f),
            new Vector3(0.2972052f, -0.375f, -0.09656783f),
            new Vector3(0.3125001f, -0.375f, 0f),
            new Vector3(0.2972052f, -0.375f, 0.09656783f),
            new Vector3(0.2528179f, -0.375f, 0.1836829f),
            new Vector3(0.1836829f, -0.375f, 0.2528178f),
            new Vector3(0.09656783f, -0.375f, 0.2972052f),
            new Vector3(9.313226E-09f, -0.375f, 0.3125f),
            new Vector3(-0.0965678f, -0.375f, 0.2972052f),
            new Vector3(-0.1836829f, -0.375f, 0.2528178f),
            new Vector3(-0.2528178f, -0.375f, 0.1836829f),
            new Vector3(-0.2972052f, -0.375f, 0.09656781f),
            new Vector3(-0.3125f, -0.375f, 0f),
            new Vector3(0f, -0.4375f, 0f),
            new Vector3(0f, -0.375f, 0f),
            new Vector3(0.3125001f, -0.4375f, 0f),
            new Vector3(0.3125001f, -0.375f, 0f),
            new Vector3(-0.3125f, -0.4375f, 0f),
            new Vector3(-0.3125f, -0.375f, 0f),
            new Vector3(-0.2972054f, -0.375f, -0.09656787f),
            new Vector3(-0.2972054f, -0.4375f, -0.09656787f),
            new Vector3(-0.252818f, -0.4375f, -0.183683f),
            new Vector3(-0.2972054f, -0.4375f, -0.09656787f),
            new Vector3(-0.183683f, -0.4375f, -0.252818f),
            new Vector3(-0.09656786f, -0.4375f, -0.2972053f),
            new Vector3(0f, -0.4375f, -0.3125001f),
            new Vector3(0.09656786f, -0.4375f, -0.2972053f),
            new Vector3(0.183683f, -0.4375f, -0.2528179f),
            new Vector3(0.2528179f, -0.4375f, -0.1836829f),
            new Vector3(0.2972052f, -0.4375f, -0.09656783f),
            new Vector3(0.3125001f, -0.4375f, 0f),
            new Vector3(0.2972052f, -0.4375f, 0.09656783f),
            new Vector3(0.2528179f, -0.4375f, 0.1836829f),
            new Vector3(0.1836829f, -0.4375f, 0.2528178f),
            new Vector3(0.09656783f, -0.4375f, 0.2972052f),
            new Vector3(9.313226E-09f, -0.4375f, 0.3125f),
            new Vector3(-0.0965678f, -0.4375f, 0.2972052f),
            new Vector3(-0.1836829f, -0.4375f, 0.2528178f),
            new Vector3(-0.2528178f, -0.4375f, 0.1836829f),
            new Vector3(-0.2972052f, -0.4375f, 0.09656781f),
            new Vector3(-0.3125f, -0.4375f, 0f),
            new Vector3(-0.2972054f, -0.375f, -0.09656787f),
            new Vector3(-0.252818f, -0.375f, -0.183683f),
            new Vector3(-0.183683f, -0.375f, -0.252818f),
            new Vector3(-0.09656786f, -0.375f, -0.2972053f),
            new Vector3(0f, -0.375f, -0.3125001f),
            new Vector3(0.09656786f, -0.375f, -0.2972053f),
            new Vector3(0.183683f, -0.375f, -0.2528179f),
            new Vector3(0.2528179f, -0.375f, -0.1836829f),
            new Vector3(0.2972052f, -0.375f, -0.09656783f),
            new Vector3(0.3125001f, -0.375f, 0f),
            new Vector3(0.2972052f, -0.375f, 0.09656783f),
            new Vector3(0.2528179f, -0.375f, 0.1836829f),
            new Vector3(0.1836829f, -0.375f, 0.2528178f),
            new Vector3(0.09656783f, -0.375f, 0.2972052f),
            new Vector3(9.313226E-09f, -0.375f, 0.3125f),
            new Vector3(-0.0965678f, -0.375f, 0.2972052f),
            new Vector3(-0.1836829f, -0.375f, 0.2528178f),
            new Vector3(-0.2528178f, -0.375f, 0.1836829f),
            new Vector3(-0.2972052f, -0.375f, 0.09656781f),
            new Vector3(-0.3125f, -0.375f, 0f),
            new Vector3(-0.2377643f, -0.375f, -0.0772543f),
            new Vector3(-0.2022544f, -0.375f, -0.1469464f),
            new Vector3(-0.1469464f, -0.375f, -0.2022544f),
            new Vector3(-0.07725429f, -0.375f, -0.2377643f),
            new Vector3(0f, -0.375f, -0.2500001f),
            new Vector3(0.07725429f, -0.375f, -0.2377642f),
            new Vector3(0.1469464f, -0.375f, -0.2022543f),
            new Vector3(0.2022543f, -0.375f, -0.1469464f),
            new Vector3(0.2377642f, -0.375f, -0.07725427f),
            new Vector3(0.2500001f, -0.375f, 0f),
            new Vector3(0.2377642f, -0.375f, 0.07725427f),
            new Vector3(0.2022543f, -0.375f, 0.1469463f),
            new Vector3(0.1469463f, -0.375f, 0.2022543f),
            new Vector3(0.07725427f, -0.375f, 0.2377642f),
            new Vector3(7.450581E-09f, -0.375f, 0.25f),
            new Vector3(-0.07725424f, -0.375f, 0.2377641f),
            new Vector3(-0.1469463f, -0.375f, 0.2022543f),
            new Vector3(-0.2022543f, -0.375f, 0.1469463f),
            new Vector3(-0.2377641f, -0.375f, 0.07725425f),
            new Vector3(-0.25f, -0.375f, 0f),
            new Vector3(-0.2377643f, 0.5f, -0.0772543f),
            new Vector3(-0.2022544f, 0.5f, -0.1469464f),
            new Vector3(-0.1469464f, 0.5f, -0.2022544f),
            new Vector3(-0.07725429f, 0.5f, -0.2377643f),
            new Vector3(0f, 0.5f, -0.2500001f),
            new Vector3(0.07725429f, 0.5f, -0.2377642f),
            new Vector3(0.1469464f, 0.5f, -0.2022543f),
            new Vector3(0.2022543f, 0.5f, -0.1469464f),
            new Vector3(0.2377642f, 0.5f, -0.07725427f),
            new Vector3(0.2500001f, 0.5f, 0f),
            new Vector3(0.2377642f, 0.5f, 0.07725427f),
            new Vector3(0.2022543f, 0.5f, 0.1469463f),
            new Vector3(0.1469463f, 0.5f, 0.2022543f),
            new Vector3(0.07725427f, 0.5f, 0.2377642f),
            new Vector3(7.450581E-09f, 0.5f, 0.25f),
            new Vector3(-0.07725424f, 0.5f, 0.2377641f),
            new Vector3(-0.1469463f, 0.5f, 0.2022543f),
            new Vector3(-0.2022543f, 0.5f, 0.1469463f),
            new Vector3(-0.2377641f, 0.5f, 0.07725425f),
            new Vector3(-0.25f, 0.5f, 0f),
            new Vector3(0f, -0.375f, 0f),
            new Vector3(0f, 0.5f, 0f),
            new Vector3(0.2500001f, -0.375f, 0f),
            new Vector3(0.2500001f, 0.5f, 0f),
            new Vector3(-0.25f, -0.375f, 0f),
            new Vector3(-0.25f, 0.5f, 0f),
            new Vector3(-0.2377643f, 0.5f, -0.0772543f),
            new Vector3(-0.2377643f, -0.375f, -0.0772543f),
            new Vector3(-0.2022544f, -0.375f, -0.1469464f),
            new Vector3(-0.2377643f, -0.375f, -0.0772543f),
            new Vector3(-0.1469464f, -0.375f, -0.2022544f),
            new Vector3(-0.07725429f, -0.375f, -0.2377643f),
            new Vector3(0f, -0.375f, -0.2500001f),
            new Vector3(0.07725429f, -0.375f, -0.2377642f),
            new Vector3(0.1469464f, -0.375f, -0.2022543f),
            new Vector3(0.2022543f, -0.375f, -0.1469464f),
            new Vector3(0.2377642f, -0.375f, -0.07725427f),
            new Vector3(0.2500001f, -0.375f, 0f),
            new Vector3(0.2377642f, -0.375f, 0.07725427f),
            new Vector3(0.2022543f, -0.375f, 0.1469463f),
            new Vector3(0.1469463f, -0.375f, 0.2022543f),
            new Vector3(0.07725427f, -0.375f, 0.2377642f),
            new Vector3(7.450581E-09f, -0.375f, 0.25f),
            new Vector3(-0.07725424f, -0.375f, 0.2377641f),
            new Vector3(-0.1469463f, -0.375f, 0.2022543f),
            new Vector3(-0.2022543f, -0.375f, 0.1469463f),
            new Vector3(-0.2377641f, -0.375f, 0.07725425f),
            new Vector3(-0.25f, -0.375f, 0f),
            new Vector3(-0.2377643f, 0.5f, -0.0772543f),
            new Vector3(-0.2022544f, 0.5f, -0.1469464f),
            new Vector3(-0.1469464f, 0.5f, -0.2022544f),
            new Vector3(-0.07725429f, 0.5f, -0.2377643f),
            new Vector3(0f, 0.5f, -0.2500001f),
            new Vector3(0.07725429f, 0.5f, -0.2377642f),
            new Vector3(0.1469464f, 0.5f, -0.2022543f),
            new Vector3(0.2022543f, 0.5f, -0.1469464f),
            new Vector3(0.2377642f, 0.5f, -0.07725427f),
            new Vector3(0.2500001f, 0.5f, 0f),
            new Vector3(0.2377642f, 0.5f, 0.07725427f),
            new Vector3(0.2022543f, 0.5f, 0.1469463f),
            new Vector3(0.1469463f, 0.5f, 0.2022543f),
            new Vector3(0.07725427f, 0.5f, 0.2377642f),
            new Vector3(7.450581E-09f, 0.5f, 0.25f),
            new Vector3(-0.07725424f, 0.5f, 0.2377641f),
            new Vector3(-0.1469463f, 0.5f, 0.2022543f),
            new Vector3(-0.2022543f, 0.5f, 0.1469463f),
            new Vector3(-0.2377641f, 0.5f, 0.07725425f),
            new Vector3(-0.25f, 0.5f, 0f)
        };

        meshFilter.mesh.triangles = new int[]
        {
           0, 20, 21,
            0, 21, 1,
            1, 21, 22,
            1, 22, 2,
            2, 22, 23,
            2, 23, 3,
            3, 23, 24,
            3, 24, 4,
            4, 24, 25,
            4, 25, 5,
            5, 25, 26,
            5, 26, 6,
            6, 26, 27,
            6, 27, 7,
            7, 27, 28,
            7, 28, 8,
            8, 28, 29,
            8, 29, 9,
            42, 43, 30,
            42, 30, 10,
            10, 30, 31,
            10, 31, 11,
            11, 31, 32,
            11, 32, 12,
            12, 32, 33,
            12, 33, 13,
            13, 33, 34,
            13, 34, 14,
            14, 34, 35,
            14, 35, 15,
            15, 35, 36,
            15, 36, 16,
            16, 36, 37,
            16, 37, 17,
            17, 37, 38,
            17, 38, 18,
            18, 38, 39,
            18, 39, 19,
            44, 45, 46,
            44, 46, 47,
            48, 40, 49,
            50, 40, 48,
            49, 40, 67,
            51, 40, 50,
            67, 40, 66,
            52, 40, 51,
            66, 40, 65,
            53, 40, 52,
            65, 40, 64,
            54, 40, 53,
            64, 40, 63,
            55, 40, 54,
            63, 40, 62,
            56, 40, 55,
            62, 40, 61,
            57, 40, 56,
            61, 40, 60,
            58, 40, 57,
            60, 40, 59,
            59, 40, 58,
            68, 41, 69,
            87, 41, 68,
            69, 41, 70,
            86, 41, 87,
            70, 41, 71,
            85, 41, 86,
            71, 41, 72,
            84, 41, 85,
            72, 41, 73,
            83, 41, 84,
            73, 41, 74,
            82, 41, 83,
            74, 41, 75,
            81, 41, 82,
            75, 41, 76,
            80, 41, 81,
            76, 41, 77,
            79, 41, 80,
            77, 41, 78,
            78, 41, 79,
            88, 91, 89,
            91, 88, 90,
            92, 112, 113,
            92, 113, 93,
            93, 113, 114,
            93, 114, 94,
            94, 114, 115,
            94, 115, 95,
            95, 115, 116,
            95, 116, 96,
            96, 116, 117,
            96, 117, 97,
            97, 117, 118,
            97, 118, 98,
            98, 118, 119,
            98, 119, 99,
            99, 119, 120,
            99, 120, 100,
            100, 120, 121,
            100, 121, 101,
            134, 135, 122,
            134, 122, 102,
            102, 122, 123,
            102, 123, 103,
            103, 123, 124,
            103, 124, 104,
            104, 124, 125,
            104, 125, 105,
            105, 125, 126,
            105, 126, 106,
            106, 126, 127,
            106, 127, 107,
            107, 127, 128,
            107, 128, 108,
            108, 128, 129,
            108, 129, 109,
            109, 129, 130,
            109, 130, 110,
            110, 130, 131,
            110, 131, 111,
            136, 137, 138,
            136, 138, 139,
            140, 132, 141,
            142, 132, 140,
            141, 132, 159,
            143, 132, 142,
            159, 132, 158,
            144, 132, 143,
            158, 132, 157,
            145, 132, 144,
            157, 132, 156,
            146, 132, 145,
            156, 132, 155,
            147, 132, 146,
            155, 132, 154,
            148, 132, 147,
            154, 132, 153,
            149, 132, 148,
            153, 132, 152,
            150, 132, 149,
            152, 132, 151,
            151, 132, 150,
            160, 133, 161,
            179, 133, 160,
            161, 133, 162,
            178, 133, 179,
            162, 133, 163,
            177, 133, 178,
            163, 133, 164,
            176, 133, 177,
            164, 133, 165,
            175, 133, 176,
            165, 133, 166,
            174, 133, 175,
            166, 133, 167,
            173, 133, 174,
            167, 133, 168,
            172, 133, 173,
            168, 133, 169,
            171, 133, 172,
            169, 133, 170,
            170, 133, 171,
            180, 200, 201,
            180, 201, 181,
            181, 201, 202,
            181, 202, 182,
            182, 202, 203,
            182, 203, 183,
            183, 203, 204,
            183, 204, 184,
            184, 204, 205,
            184, 205, 185,
            185, 205, 206,
            185, 206, 186,
            186, 206, 207,
            186, 207, 187,
            187, 207, 208,
            187, 208, 188,
            188, 208, 209,
            188, 209, 189,
            222, 223, 210,
            222, 210, 190,
            190, 210, 211,
            190, 211, 191,
            191, 211, 212,
            191, 212, 192,
            192, 212, 213,
            192, 213, 193,
            193, 213, 214,
            193, 214, 194,
            194, 214, 215,
            194, 215, 195,
            195, 215, 216,
            195, 216, 196,
            196, 216, 217,
            196, 217, 197,
            197, 217, 218,
            197, 218, 198,
            198, 218, 219,
            198, 219, 199,
            224, 225, 226,
            224, 226, 227,
            228, 220, 229,
            230, 220, 228,
            229, 220, 247,
            231, 220, 230,
            247, 220, 246,
            232, 220, 231,
            246, 220, 245,
            233, 220, 232,
            245, 220, 244,
            234, 220, 233,
            244, 220, 243,
            235, 220, 234,
            243, 220, 242,
            236, 220, 235,
            242, 220, 241,
            237, 220, 236,
            241, 220, 240,
            238, 220, 237,
            240, 220, 239,
            239, 220, 238,
            248, 221, 249,
            267, 221, 248,
            249, 221, 250,
            266, 221, 267,
            250, 221, 251,
            265, 221, 266,
            251, 221, 252,
            264, 221, 265,
            252, 221, 253,
            263, 221, 264,
            253, 221, 254,
            262, 221, 263,
            254, 221, 255,
            261, 221, 262,
            255, 221, 256,
            260, 221, 261,
            256, 221, 257,
            259, 221, 260,
            257, 221, 258,
            258, 221, 259,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0.422132f, 0.1300181f),
            new Vector2(0.4441112f, 0.1300181f),
            new Vector2(0.4660904f, 0.1300181f),
            new Vector2(0.4880697f, 0.1300181f),
            new Vector2(0.5100489f, 0.1300181f),
            new Vector2(0.532028f, 0.1300181f),
            new Vector2(0.5540072f, 0.1300181f),
            new Vector2(0.5759864f, 0.1300181f),
            new Vector2(0.5979655f, 0.1300181f),
            new Vector2(0.6199448f, 0.1300181f),
            new Vector2(0.4221319f, 0.1300181f),
            new Vector2(0.4441111f, 0.1300181f),
            new Vector2(0.4660903f, 0.1300181f),
            new Vector2(0.4880695f, 0.1300181f),
            new Vector2(0.5100487f, 0.1300181f),
            new Vector2(0.5320279f, 0.1300181f),
            new Vector2(0.5540071f, 0.1300181f),
            new Vector2(0.5759863f, 0.1300181f),
            new Vector2(0.5979654f, 0.1300181f),
            new Vector2(0.6199446f, 0.1300181f),
            new Vector2(0.4221319f, 0.1699772f),
            new Vector2(0.4441112f, 0.1699772f),
            new Vector2(0.4660904f, 0.1699772f),
            new Vector2(0.4880697f, 0.1699772f),
            new Vector2(0.5100489f, 0.1699772f),
            new Vector2(0.532028f, 0.1699772f),
            new Vector2(0.5540072f, 0.1699772f),
            new Vector2(0.5759864f, 0.1699772f),
            new Vector2(0.5979656f, 0.1699772f),
            new Vector2(0.6199448f, 0.1699772f),
            new Vector2(0.4221319f, 0.1699772f),
            new Vector2(0.4441111f, 0.1699772f),
            new Vector2(0.4660903f, 0.1699772f),
            new Vector2(0.4880695f, 0.1699772f),
            new Vector2(0.5100487f, 0.1699772f),
            new Vector2(0.5320279f, 0.1699772f),
            new Vector2(0.5540071f, 0.1699772f),
            new Vector2(0.5759861f, 0.1699772f),
            new Vector2(0.5979654f, 0.1699772f),
            new Vector2(0.6199445f, 0.1699772f),
            new Vector2(0.5100493f, 0.1499978f),
            new Vector2(0.5100494f, 0.1499978f),
            new Vector2(0.4001526f, 0.1300181f),
            new Vector2(0.4001527f, 0.1699772f),
            new Vector2(0.4001527f, 0.1300181f),
            new Vector2(0.4001527f, 0.1699772f),
            new Vector2(0.4221319f, 0.1699772f),
            new Vector2(0.422132f, 0.1300181f),
            new Vector2(0.4217485f, 0.1616621f),
            new Vector2(0.4062455f, 0.1561301f),
            new Vector2(0.445895f, 0.1660524f),
            new Vector2(0.4763214f, 0.1688712f),
            new Vector2(0.5100493f, 0.1698424f),
            new Vector2(0.5437772f, 0.1688712f),
            new Vector2(0.5742036f, 0.1660524f),
            new Vector2(0.59835f, 0.1616621f),
            new Vector2(0.613853f, 0.1561301f),
            new Vector2(0.6191949f, 0.1499977f),
            new Vector2(0.6138529f, 0.1438654f),
            new Vector2(0.5983499f, 0.1383334f),
            new Vector2(0.5742034f, 0.1339431f),
            new Vector2(0.543777f, 0.1311244f),
            new Vector2(0.5100492f, 0.1301531f),
            new Vector2(0.4763213f, 0.1311244f),
            new Vector2(0.445895f, 0.1339431f),
            new Vector2(0.4217486f, 0.1383334f),
            new Vector2(0.4062456f, 0.1438654f),
            new Vector2(0.4009036f, 0.1499978f),
            new Vector2(0.6138531f, 0.1561301f),
            new Vector2(0.5983502f, 0.1616621f),
            new Vector2(0.5742036f, 0.1660524f),
            new Vector2(0.5437772f, 0.1688712f),
            new Vector2(0.5100493f, 0.1698424f),
            new Vector2(0.4763215f, 0.1688712f),
            new Vector2(0.4458951f, 0.1660524f),
            new Vector2(0.4217487f, 0.1616621f),
            new Vector2(0.4062457f, 0.1561301f),
            new Vector2(0.4009038f, 0.1499977f),
            new Vector2(0.4062457f, 0.1438654f),
            new Vector2(0.4217487f, 0.1383334f),
            new Vector2(0.4458952f, 0.1339431f),
            new Vector2(0.4763215f, 0.1311244f),
            new Vector2(0.5100493f, 0.1301531f),
            new Vector2(0.5437772f, 0.1311244f),
            new Vector2(0.5742036f, 0.1339431f),
            new Vector2(0.59835f, 0.1383334f),
            new Vector2(0.613853f, 0.1438654f),
            new Vector2(0.619195f, 0.1499978f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5240101f, 0.1875272f),
            new Vector2(0.5260082f, 0.1875272f),
            new Vector2(0.5280063f, 0.1875272f),
            new Vector2(0.5300044f, 0.1875272f),
            new Vector2(0.5320025f, 0.1875272f),
            new Vector2(0.5340006f, 0.1875272f),
            new Vector2(0.5359988f, 0.1875272f),
            new Vector2(0.5379968f, 0.1875272f),
            new Vector2(0.539995f, 0.1875272f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5240101f, 0.1875272f),
            new Vector2(0.5260082f, 0.1875272f),
            new Vector2(0.5280063f, 0.1875272f),
            new Vector2(0.5300044f, 0.1875272f),
            new Vector2(0.5320025f, 0.1875272f),
            new Vector2(0.5340006f, 0.1875272f),
            new Vector2(0.5359987f, 0.1875272f),
            new Vector2(0.5379968f, 0.1875272f),
            new Vector2(0.539995f, 0.1875272f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.5240101f, 0.2474658f),
            new Vector2(0.5260082f, 0.2474658f),
            new Vector2(0.5280063f, 0.2474658f),
            new Vector2(0.5300044f, 0.2474658f),
            new Vector2(0.5320025f, 0.2474658f),
            new Vector2(0.5340006f, 0.2474658f),
            new Vector2(0.5359988f, 0.2474658f),
            new Vector2(0.5379968f, 0.2474658f),
            new Vector2(0.539995f, 0.2474658f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.5240101f, 0.2474658f),
            new Vector2(0.5260082f, 0.2474658f),
            new Vector2(0.5280063f, 0.2474658f),
            new Vector2(0.5300044f, 0.2474658f),
            new Vector2(0.5320025f, 0.2474658f),
            new Vector2(0.5340006f, 0.2474658f),
            new Vector2(0.5359987f, 0.2474658f),
            new Vector2(0.5379968f, 0.2474658f),
            new Vector2(0.539995f, 0.2474658f),
            new Vector2(0.5300044f, 0.2174967f),
            new Vector2(0.5300044f, 0.2174966f),
            new Vector2(0.5200139f, 0.1875272f),
            new Vector2(0.5200139f, 0.2474658f),
            new Vector2(0.5200139f, 0.1875272f),
            new Vector2(0.5200139f, 0.2474658f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5219771f, 0.2349932f),
            new Vector2(0.5205678f, 0.2266951f),
            new Vector2(0.5241722f, 0.2415786f),
            new Vector2(0.5269383f, 0.2458067f),
            new Vector2(0.5300044f, 0.2472637f),
            new Vector2(0.5330706f, 0.2458067f),
            new Vector2(0.5358367f, 0.2415786f),
            new Vector2(0.5380318f, 0.2349932f),
            new Vector2(0.5394412f, 0.2266951f),
            new Vector2(0.5399268f, 0.2174966f),
            new Vector2(0.5394412f, 0.2082981f),
            new Vector2(0.5380318f, 0.2f),
            new Vector2(0.5358366f, 0.1934147f),
            new Vector2(0.5330706f, 0.1891866f),
            new Vector2(0.5300044f, 0.1877297f),
            new Vector2(0.5269383f, 0.1891866f),
            new Vector2(0.5241722f, 0.1934147f),
            new Vector2(0.5219771f, 0.2000001f),
            new Vector2(0.5205678f, 0.2082982f),
            new Vector2(0.5200821f, 0.2174966f),
            new Vector2(0.5394412f, 0.2266951f),
            new Vector2(0.5380318f, 0.2349932f),
            new Vector2(0.5358367f, 0.2415786f),
            new Vector2(0.5330706f, 0.2458067f),
            new Vector2(0.5300044f, 0.2472637f),
            new Vector2(0.5269383f, 0.2458067f),
            new Vector2(0.5241722f, 0.2415786f),
            new Vector2(0.5219771f, 0.2349932f),
            new Vector2(0.5205678f, 0.2266951f),
            new Vector2(0.5200821f, 0.2174966f),
            new Vector2(0.5205678f, 0.2082981f),
            new Vector2(0.5219771f, 0.2f),
            new Vector2(0.5241722f, 0.1934147f),
            new Vector2(0.5269383f, 0.1891866f),
            new Vector2(0.5300044f, 0.1877297f),
            new Vector2(0.5330706f, 0.1891866f),
            new Vector2(0.5358367f, 0.1934147f),
            new Vector2(0.5380318f, 0.2f),
            new Vector2(0.5394412f, 0.2082981f),
            new Vector2(0.5399268f, 0.2174966f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5200505f, 0.5000454f),
            new Vector2(0.5300411f, 0.5000454f),
            new Vector2(0.5400317f, 0.5000454f),
            new Vector2(0.5500222f, 0.5000454f),
            new Vector2(0.5600127f, 0.5000454f),
            new Vector2(0.5700033f, 0.5000454f),
            new Vector2(0.5799938f, 0.5000454f),
            new Vector2(0.5899843f, 0.5000454f),
            new Vector2(0.5999749f, 0.5000454f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5200505f, 0.5000454f),
            new Vector2(0.530041f, 0.5000454f),
            new Vector2(0.5400316f, 0.5000454f),
            new Vector2(0.5500221f, 0.5000454f),
            new Vector2(0.5600127f, 0.5000454f),
            new Vector2(0.5700032f, 0.5000454f),
            new Vector2(0.5799937f, 0.5000454f),
            new Vector2(0.5899843f, 0.5000454f),
            new Vector2(0.5999748f, 0.5000454f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.5200505f, 0.5999431f),
            new Vector2(0.5300411f, 0.5999431f),
            new Vector2(0.5400317f, 0.5999431f),
            new Vector2(0.5500222f, 0.5999431f),
            new Vector2(0.5600127f, 0.5999431f),
            new Vector2(0.5700033f, 0.5999431f),
            new Vector2(0.5799938f, 0.5999431f),
            new Vector2(0.5899844f, 0.5999431f),
            new Vector2(0.5999749f, 0.5999431f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.5200505f, 0.5999431f),
            new Vector2(0.530041f, 0.5999431f),
            new Vector2(0.5400316f, 0.5999431f),
            new Vector2(0.5500221f, 0.5999431f),
            new Vector2(0.5600127f, 0.5999431f),
            new Vector2(0.5700032f, 0.5999431f),
            new Vector2(0.5799937f, 0.5999431f),
            new Vector2(0.5899842f, 0.5999431f),
            new Vector2(0.5999748f, 0.5999431f),
            new Vector2(0.5500224f, 0.5499944f),
            new Vector2(0.5500224f, 0.5499944f),
            new Vector2(0.5000694f, 0.5000454f),
            new Vector2(0.5000694f, 0.5999431f),
            new Vector2(0.5000694f, 0.5000454f),
            new Vector2(0.5000694f, 0.5999431f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5098857f, 0.5791554f),
            new Vector2(0.5028389f, 0.5653253f),
            new Vector2(0.5208614f, 0.590131f),
            new Vector2(0.5346915f, 0.5971779f),
            new Vector2(0.5500224f, 0.5996061f),
            new Vector2(0.5653533f, 0.5971779f),
            new Vector2(0.5791835f, 0.590131f),
            new Vector2(0.5901591f, 0.5791553f),
            new Vector2(0.5972059f, 0.5653252f),
            new Vector2(0.5996341f, 0.5499943f),
            new Vector2(0.5972059f, 0.5346636f),
            new Vector2(0.5901591f, 0.5208334f),
            new Vector2(0.5791833f, 0.5098578f),
            new Vector2(0.5653532f, 0.502811f),
            new Vector2(0.5500224f, 0.5003828f),
            new Vector2(0.5346915f, 0.502811f),
            new Vector2(0.5208614f, 0.5098578f),
            new Vector2(0.5098857f, 0.5208335f),
            new Vector2(0.5028389f, 0.5346636f),
            new Vector2(0.5004107f, 0.5499944f),
            new Vector2(0.5972059f, 0.5653253f),
            new Vector2(0.5901592f, 0.5791554f),
            new Vector2(0.5791835f, 0.590131f),
            new Vector2(0.5653533f, 0.5971779f),
            new Vector2(0.5500224f, 0.5996061f),
            new Vector2(0.5346916f, 0.5971779f),
            new Vector2(0.5208614f, 0.590131f),
            new Vector2(0.5098858f, 0.5791554f),
            new Vector2(0.502839f, 0.5653252f),
            new Vector2(0.5004108f, 0.5499944f),
            new Vector2(0.502839f, 0.5346636f),
            new Vector2(0.5098858f, 0.5208334f),
            new Vector2(0.5208614f, 0.5098578f),
            new Vector2(0.5346916f, 0.502811f),
            new Vector2(0.5500224f, 0.5003828f),
            new Vector2(0.5653533f, 0.502811f),
            new Vector2(0.5791835f, 0.5098578f),
            new Vector2(0.5901591f, 0.5208334f),
            new Vector2(0.5972059f, 0.5346636f),
            new Vector2(0.5996341f, 0.5499944f),
        };

        return pillar;
    }

    GameObject CreateMast()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject mast = new GameObject("Mast");
        mast.name = "Mast";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = mast.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = mast.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.4999999f, -0.5000001f),
            new Vector3(0.5f, -0.4999999f, -0.5000001f),
            new Vector3(-0.5f, -0.5000001f, 0.5000001f),
            new Vector3(0.5f, -0.5000001f, 0.5000001f),
            new Vector3(-0.1931835f, -0.5f, -0.06276911f),
            new Vector3(-0.1643317f, -0.5f, -0.119394f),
            new Vector3(-0.119394f, -0.5f, -0.1643317f),
            new Vector3(-0.06276911f, -0.5f, -0.1931835f),
            new Vector3(0f, -0.5f, -0.2031251f),
            new Vector3(0.06276911f, -0.5f, -0.1931835f),
            new Vector3(0.1193939f, -0.5f, -0.1643316f),
            new Vector3(0.1643316f, -0.5f, -0.1193939f),
            new Vector3(0.1931834f, -0.5f, -0.06276909f),
            new Vector3(0.203125f, -0.5f, 0f),
            new Vector3(0.1931834f, -0.5f, 0.06276909f),
            new Vector3(0.1643316f, -0.5f, 0.1193939f),
            new Vector3(0.1193939f, -0.5f, 0.1643316f),
            new Vector3(0.06276909f, -0.5f, 0.1931834f),
            new Vector3(6.053597E-09f, -0.5f, 0.203125f),
            new Vector3(-0.06276907f, -0.5f, 0.1931834f),
            new Vector3(-0.1193939f, -0.5f, 0.1643316f),
            new Vector3(-0.1643316f, -0.5f, 0.1193939f),
            new Vector3(-0.1931834f, -0.5f, 0.06276908f),
            new Vector3(-0.203125f, -0.5f, 0f),
            new Vector3(-0.1931835f, -0.4375f, -0.06276911f),
            new Vector3(-0.1643317f, -0.4375f, -0.119394f),
            new Vector3(-0.119394f, -0.4375f, -0.1643317f),
            new Vector3(-0.06276911f, -0.4375f, -0.1931835f),
            new Vector3(0f, -0.4375f, -0.2031251f),
            new Vector3(0.06276911f, -0.4375f, -0.1931835f),
            new Vector3(0.1193939f, -0.4375f, -0.1643316f),
            new Vector3(0.1643316f, -0.4375f, -0.1193939f),
            new Vector3(0.1931834f, -0.4375f, -0.06276909f),
            new Vector3(0.203125f, -0.4375f, 0f),
            new Vector3(0.1931834f, -0.4375f, 0.06276909f),
            new Vector3(0.1643316f, -0.4375f, 0.1193939f),
            new Vector3(0.1193939f, -0.4375f, 0.1643316f),
            new Vector3(0.06276909f, -0.4375f, 0.1931834f),
            new Vector3(6.053597E-09f, -0.4375f, 0.203125f),
            new Vector3(-0.06276907f, -0.4375f, 0.1931834f),
            new Vector3(-0.1193939f, -0.4375f, 0.1643316f),
            new Vector3(-0.1643316f, -0.4375f, 0.1193939f),
            new Vector3(-0.1931834f, -0.4375f, 0.06276908f),
            new Vector3(-0.203125f, -0.4375f, 0f),
            new Vector3(0f, -0.5f, 0f),
            new Vector3(0f, -0.4375f, 0f),
            new Vector3(0.203125f, -0.5f, 0f),
            new Vector3(0.203125f, -0.4375f, 0f),
            new Vector3(-0.203125f, -0.5f, 0f),
            new Vector3(-0.203125f, -0.4375f, 0f),
            new Vector3(-0.1931835f, -0.4375f, -0.06276911f),
            new Vector3(-0.1931835f, -0.5f, -0.06276911f),
            new Vector3(-0.1643317f, -0.5f, -0.119394f),
            new Vector3(-0.1931835f, -0.5f, -0.06276911f),
            new Vector3(-0.119394f, -0.5f, -0.1643317f),
            new Vector3(-0.06276911f, -0.5f, -0.1931835f),
            new Vector3(0f, -0.5f, -0.2031251f),
            new Vector3(0.06276911f, -0.5f, -0.1931835f),
            new Vector3(0.1193939f, -0.5f, -0.1643316f),
            new Vector3(0.1643316f, -0.5f, -0.1193939f),
            new Vector3(0.1931834f, -0.5f, -0.06276909f),
            new Vector3(0.203125f, -0.5f, 0f),
            new Vector3(0.1931834f, -0.5f, 0.06276909f),
            new Vector3(0.1643316f, -0.5f, 0.1193939f),
            new Vector3(0.1193939f, -0.5f, 0.1643316f),
            new Vector3(0.06276909f, -0.5f, 0.1931834f),
            new Vector3(6.053597E-09f, -0.5f, 0.203125f),
            new Vector3(-0.06276907f, -0.5f, 0.1931834f),
            new Vector3(-0.1193939f, -0.5f, 0.1643316f),
            new Vector3(-0.1643316f, -0.5f, 0.1193939f),
            new Vector3(-0.1931834f, -0.5f, 0.06276908f),
            new Vector3(-0.203125f, -0.5f, 0f),
            new Vector3(-0.1931835f, -0.4375f, -0.06276911f),
            new Vector3(-0.1643317f, -0.4375f, -0.119394f),
            new Vector3(-0.119394f, -0.4375f, -0.1643317f),
            new Vector3(-0.06276911f, -0.4375f, -0.1931835f),
            new Vector3(0f, -0.4375f, -0.2031251f),
            new Vector3(0.06276911f, -0.4375f, -0.1931835f),
            new Vector3(0.1193939f, -0.4375f, -0.1643316f),
            new Vector3(0.1643316f, -0.4375f, -0.1193939f),
            new Vector3(0.1931834f, -0.4375f, -0.06276909f),
            new Vector3(0.203125f, -0.4375f, 0f),
            new Vector3(0.1931834f, -0.4375f, 0.06276909f),
            new Vector3(0.1643316f, -0.4375f, 0.1193939f),
            new Vector3(0.1193939f, -0.4375f, 0.1643316f),
            new Vector3(0.06276909f, -0.4375f, 0.1931834f),
            new Vector3(6.053597E-09f, -0.4375f, 0.203125f),
            new Vector3(-0.06276907f, -0.4375f, 0.1931834f),
            new Vector3(-0.1193939f, -0.4375f, 0.1643316f),
            new Vector3(-0.1643316f, -0.4375f, 0.1193939f),
            new Vector3(-0.1931834f, -0.4375f, 0.06276908f),
            new Vector3(-0.203125f, -0.4375f, 0f),
            new Vector3(-0.1337424f, -0.4375f, -0.04345554f),
            new Vector3(-0.1137681f, -0.4375f, -0.08265735f),
            new Vector3(-0.08265735f, -0.4375f, -0.1137681f),
            new Vector3(-0.04345554f, -0.4375f, -0.1337424f),
            new Vector3(0f, -0.4375f, -0.1406251f),
            new Vector3(0.04345554f, -0.4375f, -0.1337424f),
            new Vector3(0.08265734f, -0.4375f, -0.1137681f),
            new Vector3(0.113768f, -0.4375f, -0.08265732f),
            new Vector3(0.1337424f, -0.4375f, -0.04345553f),
            new Vector3(0.140625f, -0.4375f, 0f),
            new Vector3(0.1337424f, -0.4375f, 0.04345553f),
            new Vector3(0.113768f, -0.4375f, 0.08265731f),
            new Vector3(0.08265731f, -0.4375f, 0.113768f),
            new Vector3(0.04345553f, -0.4375f, 0.1337423f),
            new Vector3(4.190952E-09f, -0.4375f, 0.140625f),
            new Vector3(-0.04345551f, -0.4375f, 0.1337423f),
            new Vector3(-0.0826573f, -0.4375f, 0.113768f),
            new Vector3(-0.113768f, -0.4375f, 0.08265731f),
            new Vector3(-0.1337423f, -0.4375f, 0.04345552f),
            new Vector3(-0.140625f, -0.4375f, 0f),
            new Vector3(-0.1337424f, 0.5f, -0.04345554f),
            new Vector3(-0.1137681f, 0.5f, -0.08265735f),
            new Vector3(-0.08265735f, 0.5f, -0.1137681f),
            new Vector3(-0.04345554f, 0.5f, -0.1337424f),
            new Vector3(0f, 0.5f, -0.1406251f),
            new Vector3(0.04345554f, 0.5f, -0.1337424f),
            new Vector3(0.08265734f, 0.5f, -0.1137681f),
            new Vector3(0.113768f, 0.5f, -0.08265732f),
            new Vector3(0.1337424f, 0.5f, -0.04345553f),
            new Vector3(0.140625f, 0.5f, 0f),
            new Vector3(0.1337424f, 0.5f, 0.04345553f),
            new Vector3(0.113768f, 0.5f, 0.08265731f),
            new Vector3(0.08265731f, 0.5f, 0.113768f),
            new Vector3(0.04345553f, 0.5f, 0.1337423f),
            new Vector3(4.190952E-09f, 0.5f, 0.140625f),
            new Vector3(-0.04345551f, 0.5f, 0.1337423f),
            new Vector3(-0.0826573f, 0.5f, 0.113768f),
            new Vector3(-0.113768f, 0.5f, 0.08265731f),
            new Vector3(-0.1337423f, 0.5f, 0.04345552f),
            new Vector3(-0.140625f, 0.5f, 0f),
            new Vector3(0f, -0.4375f, 0f),
            new Vector3(0f, 0.5f, 0f),
            new Vector3(0.140625f, -0.4375f, 0f),
            new Vector3(0.140625f, 0.5f, 0f),
            new Vector3(-0.140625f, -0.4375f, 0f),
            new Vector3(-0.140625f, 0.5f, 0f),
            new Vector3(-0.1337424f, 0.5f, -0.04345554f),
            new Vector3(-0.1337424f, -0.4375f, -0.04345554f),
            new Vector3(-0.1137681f, -0.4375f, -0.08265735f),
            new Vector3(-0.1337424f, -0.4375f, -0.04345554f),
            new Vector3(-0.08265735f, -0.4375f, -0.1137681f),
            new Vector3(-0.04345554f, -0.4375f, -0.1337424f),
            new Vector3(0f, -0.4375f, -0.1406251f),
            new Vector3(0.04345554f, -0.4375f, -0.1337424f),
            new Vector3(0.08265734f, -0.4375f, -0.1137681f),
            new Vector3(0.113768f, -0.4375f, -0.08265732f),
            new Vector3(0.1337424f, -0.4375f, -0.04345553f),
            new Vector3(0.140625f, -0.4375f, 0f),
            new Vector3(0.1337424f, -0.4375f, 0.04345553f),
            new Vector3(0.113768f, -0.4375f, 0.08265731f),
            new Vector3(0.08265731f, -0.4375f, 0.113768f),
            new Vector3(0.04345553f, -0.4375f, 0.1337423f),
            new Vector3(4.190952E-09f, -0.4375f, 0.140625f),
            new Vector3(-0.04345551f, -0.4375f, 0.1337423f),
            new Vector3(-0.0826573f, -0.4375f, 0.113768f),
            new Vector3(-0.113768f, -0.4375f, 0.08265731f),
            new Vector3(-0.1337423f, -0.4375f, 0.04345552f),
            new Vector3(-0.140625f, -0.4375f, 0f),
            new Vector3(-0.1337424f, 0.5f, -0.04345554f),
            new Vector3(-0.1137681f, 0.5f, -0.08265735f),
            new Vector3(-0.08265735f, 0.5f, -0.1137681f),
            new Vector3(-0.04345554f, 0.5f, -0.1337424f),
            new Vector3(0f, 0.5f, -0.1406251f),
            new Vector3(0.04345554f, 0.5f, -0.1337424f),
            new Vector3(0.08265734f, 0.5f, -0.1137681f),
            new Vector3(0.113768f, 0.5f, -0.08265732f),
            new Vector3(0.1337424f, 0.5f, -0.04345553f),
            new Vector3(0.140625f, 0.5f, 0f),
            new Vector3(0.1337424f, 0.5f, 0.04345553f),
            new Vector3(0.113768f, 0.5f, 0.08265731f),
            new Vector3(0.08265731f, 0.5f, 0.113768f),
            new Vector3(0.04345553f, 0.5f, 0.1337423f),
            new Vector3(4.190952E-09f, 0.5f, 0.140625f),
            new Vector3(-0.04345551f, 0.5f, 0.1337423f),
            new Vector3(-0.0826573f, 0.5f, 0.113768f),
            new Vector3(-0.113768f, 0.5f, 0.08265731f),
            new Vector3(-0.1337423f, 0.5f, 0.04345552f),
            new Vector3(-0.140625f, 0.5f, 0f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 24, 25,
            4, 25, 5,
            5, 25, 26,
            5, 26, 6,
            6, 26, 27,
            6, 27, 7,
            7, 27, 28,
            7, 28, 8,
            8, 28, 29,
            8, 29, 9,
            9, 29, 30,
            9, 30, 10,
            10, 30, 31,
            10, 31, 11,
            11, 31, 32,
            11, 32, 12,
            12, 32, 33,
            12, 33, 13,
            46, 47, 34,
            46, 34, 14,
            14, 34, 35,
            14, 35, 15,
            15, 35, 36,
            15, 36, 16,
            16, 36, 37,
            16, 37, 17,
            17, 37, 38,
            17, 38, 18,
            18, 38, 39,
            18, 39, 19,
            19, 39, 40,
            19, 40, 20,
            20, 40, 41,
            20, 41, 21,
            21, 41, 42,
            21, 42, 22,
            22, 42, 43,
            22, 43, 23,
            48, 49, 50,
            48, 50, 51,
            52, 44, 53,
            54, 44, 52,
            53, 44, 71,
            55, 44, 54,
            71, 44, 70,
            56, 44, 55,
            70, 44, 69,
            57, 44, 56,
            69, 44, 68,
            58, 44, 57,
            68, 44, 67,
            59, 44, 58,
            67, 44, 66,
            60, 44, 59,
            66, 44, 65,
            61, 44, 60,
            65, 44, 64,
            62, 44, 61,
            64, 44, 63,
            63, 44, 62,
            72, 45, 73,
            91, 45, 72,
            73, 45, 74,
            90, 45, 91,
            74, 45, 75,
            89, 45, 90,
            75, 45, 76,
            88, 45, 89,
            76, 45, 77,
            87, 45, 88,
            77, 45, 78,
            86, 45, 87,
            78, 45, 79,
            85, 45, 86,
            79, 45, 80,
            84, 45, 85,
            80, 45, 81,
            83, 45, 84,
            81, 45, 82,
            82, 45, 83,
            92, 112, 113,
            92, 113, 93,
            93, 113, 114,
            93, 114, 94,
            94, 114, 115,
            94, 115, 95,
            95, 115, 116,
            95, 116, 96,
            96, 116, 117,
            96, 117, 97,
            97, 117, 118,
            97, 118, 98,
            98, 118, 119,
            98, 119, 99,
            99, 119, 120,
            99, 120, 100,
            100, 120, 121,
            100, 121, 101,
            134, 135, 122,
            134, 122, 102,
            102, 122, 123,
            102, 123, 103,
            103, 123, 124,
            103, 124, 104,
            104, 124, 125,
            104, 125, 105,
            105, 125, 126,
            105, 126, 106,
            106, 126, 127,
            106, 127, 107,
            107, 127, 128,
            107, 128, 108,
            108, 128, 129,
            108, 129, 109,
            109, 129, 130,
            109, 130, 110,
            110, 130, 131,
            110, 131, 111,
            136, 137, 138,
            136, 138, 139,
            140, 132, 141,
            142, 132, 140,
            141, 132, 159,
            143, 132, 142,
            159, 132, 158,
            144, 132, 143,
            158, 132, 157,
            145, 132, 144,
            157, 132, 156,
            146, 132, 145,
            156, 132, 155,
            147, 132, 146,
            155, 132, 154,
            148, 132, 147,
            154, 132, 153,
            149, 132, 148,
            153, 132, 152,
            150, 132, 149,
            152, 132, 151,
            151, 132, 150,
            160, 133, 161,
            179, 133, 160,
            161, 133, 162,
            178, 133, 179,
            162, 133, 163,
            177, 133, 178,
            163, 133, 164,
            176, 133, 177,
            164, 133, 165,
            175, 133, 176,
            165, 133, 166,
            174, 133, 175,
            166, 133, 167,
            173, 133, 174,
            167, 133, 168,
            172, 133, 173,
            168, 133, 169,
            171, 133, 172,
            169, 133, 170,
            170, 133, 171,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5240101f, 0.1875272f),
            new Vector2(0.5260082f, 0.1875272f),
            new Vector2(0.5280063f, 0.1875272f),
            new Vector2(0.5300044f, 0.1875272f),
            new Vector2(0.5320025f, 0.1875272f),
            new Vector2(0.5340006f, 0.1875272f),
            new Vector2(0.5359988f, 0.1875272f),
            new Vector2(0.5379968f, 0.1875272f),
            new Vector2(0.539995f, 0.1875272f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5240101f, 0.1875272f),
            new Vector2(0.5260082f, 0.1875272f),
            new Vector2(0.5280063f, 0.1875272f),
            new Vector2(0.5300044f, 0.1875272f),
            new Vector2(0.5320025f, 0.1875272f),
            new Vector2(0.5340006f, 0.1875272f),
            new Vector2(0.5359987f, 0.1875272f),
            new Vector2(0.5379968f, 0.1875272f),
            new Vector2(0.539995f, 0.1875272f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.5240101f, 0.2474658f),
            new Vector2(0.5260082f, 0.2474658f),
            new Vector2(0.5280063f, 0.2474658f),
            new Vector2(0.5300044f, 0.2474658f),
            new Vector2(0.5320025f, 0.2474658f),
            new Vector2(0.5340006f, 0.2474658f),
            new Vector2(0.5359988f, 0.2474658f),
            new Vector2(0.5379968f, 0.2474658f),
            new Vector2(0.539995f, 0.2474658f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.5240101f, 0.2474658f),
            new Vector2(0.5260082f, 0.2474658f),
            new Vector2(0.5280063f, 0.2474658f),
            new Vector2(0.5300044f, 0.2474658f),
            new Vector2(0.5320025f, 0.2474658f),
            new Vector2(0.5340006f, 0.2474658f),
            new Vector2(0.5359987f, 0.2474658f),
            new Vector2(0.5379968f, 0.2474658f),
            new Vector2(0.539995f, 0.2474658f),
            new Vector2(0.5300044f, 0.2174967f),
            new Vector2(0.5300044f, 0.2174966f),
            new Vector2(0.5200139f, 0.1875272f),
            new Vector2(0.5200139f, 0.2474658f),
            new Vector2(0.5200139f, 0.1875272f),
            new Vector2(0.5200139f, 0.2474658f),
            new Vector2(0.522012f, 0.2474658f),
            new Vector2(0.522012f, 0.1875272f),
            new Vector2(0.5219771f, 0.2349932f),
            new Vector2(0.5205678f, 0.2266951f),
            new Vector2(0.5241722f, 0.2415786f),
            new Vector2(0.5269383f, 0.2458067f),
            new Vector2(0.5300044f, 0.2472637f),
            new Vector2(0.5330706f, 0.2458067f),
            new Vector2(0.5358367f, 0.2415786f),
            new Vector2(0.5380318f, 0.2349932f),
            new Vector2(0.5394412f, 0.2266951f),
            new Vector2(0.5399268f, 0.2174966f),
            new Vector2(0.5394412f, 0.2082981f),
            new Vector2(0.5380318f, 0.2f),
            new Vector2(0.5358366f, 0.1934147f),
            new Vector2(0.5330706f, 0.1891866f),
            new Vector2(0.5300044f, 0.1877297f),
            new Vector2(0.5269383f, 0.1891866f),
            new Vector2(0.5241722f, 0.1934147f),
            new Vector2(0.5219771f, 0.2000001f),
            new Vector2(0.5205678f, 0.2082982f),
            new Vector2(0.5200821f, 0.2174966f),
            new Vector2(0.5394412f, 0.2266951f),
            new Vector2(0.5380318f, 0.2349932f),
            new Vector2(0.5358367f, 0.2415786f),
            new Vector2(0.5330706f, 0.2458067f),
            new Vector2(0.5300044f, 0.2472637f),
            new Vector2(0.5269383f, 0.2458067f),
            new Vector2(0.5241722f, 0.2415786f),
            new Vector2(0.5219771f, 0.2349932f),
            new Vector2(0.5205678f, 0.2266951f),
            new Vector2(0.5200821f, 0.2174966f),
            new Vector2(0.5205678f, 0.2082981f),
            new Vector2(0.5219771f, 0.2f),
            new Vector2(0.5241722f, 0.1934147f),
            new Vector2(0.5269383f, 0.1891866f),
            new Vector2(0.5300044f, 0.1877297f),
            new Vector2(0.5330706f, 0.1891866f),
            new Vector2(0.5358367f, 0.1934147f),
            new Vector2(0.5380318f, 0.2f),
            new Vector2(0.5394412f, 0.2082981f),
            new Vector2(0.5399268f, 0.2174966f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5200505f, 0.5000454f),
            new Vector2(0.5300411f, 0.5000454f),
            new Vector2(0.5400317f, 0.5000454f),
            new Vector2(0.5500222f, 0.5000454f),
            new Vector2(0.5600127f, 0.5000454f),
            new Vector2(0.5700033f, 0.5000454f),
            new Vector2(0.5799938f, 0.5000454f),
            new Vector2(0.5899843f, 0.5000454f),
            new Vector2(0.5999749f, 0.5000454f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5200505f, 0.5000454f),
            new Vector2(0.530041f, 0.5000454f),
            new Vector2(0.5400316f, 0.5000454f),
            new Vector2(0.5500221f, 0.5000454f),
            new Vector2(0.5600127f, 0.5000454f),
            new Vector2(0.5700032f, 0.5000454f),
            new Vector2(0.5799937f, 0.5000454f),
            new Vector2(0.5899843f, 0.5000454f),
            new Vector2(0.5999748f, 0.5000454f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.5200505f, 0.5999431f),
            new Vector2(0.5300411f, 0.5999431f),
            new Vector2(0.5400317f, 0.5999431f),
            new Vector2(0.5500222f, 0.5999431f),
            new Vector2(0.5600127f, 0.5999431f),
            new Vector2(0.5700033f, 0.5999431f),
            new Vector2(0.5799938f, 0.5999431f),
            new Vector2(0.5899844f, 0.5999431f),
            new Vector2(0.5999749f, 0.5999431f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.5200505f, 0.5999431f),
            new Vector2(0.530041f, 0.5999431f),
            new Vector2(0.5400316f, 0.5999431f),
            new Vector2(0.5500221f, 0.5999431f),
            new Vector2(0.5600127f, 0.5999431f),
            new Vector2(0.5700032f, 0.5999431f),
            new Vector2(0.5799937f, 0.5999431f),
            new Vector2(0.5899842f, 0.5999431f),
            new Vector2(0.5999748f, 0.5999431f),
            new Vector2(0.5500224f, 0.5499944f),
            new Vector2(0.5500224f, 0.5499944f),
            new Vector2(0.5000694f, 0.5000454f),
            new Vector2(0.5000694f, 0.5999431f),
            new Vector2(0.5000694f, 0.5000454f),
            new Vector2(0.5000694f, 0.5999431f),
            new Vector2(0.51006f, 0.5999431f),
            new Vector2(0.51006f, 0.5000454f),
            new Vector2(0.5098857f, 0.5791554f),
            new Vector2(0.5028389f, 0.5653253f),
            new Vector2(0.5208614f, 0.590131f),
            new Vector2(0.5346915f, 0.5971779f),
            new Vector2(0.5500224f, 0.5996061f),
            new Vector2(0.5653533f, 0.5971779f),
            new Vector2(0.5791835f, 0.590131f),
            new Vector2(0.5901591f, 0.5791553f),
            new Vector2(0.5972059f, 0.5653252f),
            new Vector2(0.5996341f, 0.5499943f),
            new Vector2(0.5972059f, 0.5346636f),
            new Vector2(0.5901591f, 0.5208334f),
            new Vector2(0.5791833f, 0.5098578f),
            new Vector2(0.5653532f, 0.502811f),
            new Vector2(0.5500224f, 0.5003828f),
            new Vector2(0.5346915f, 0.502811f),
            new Vector2(0.5208614f, 0.5098578f),
            new Vector2(0.5098857f, 0.5208335f),
            new Vector2(0.5028389f, 0.5346636f),
            new Vector2(0.5004107f, 0.5499944f),
            new Vector2(0.5972059f, 0.5653253f),
            new Vector2(0.5901592f, 0.5791554f),
            new Vector2(0.5791835f, 0.590131f),
            new Vector2(0.5653533f, 0.5971779f),
            new Vector2(0.5500224f, 0.5996061f),
            new Vector2(0.5346916f, 0.5971779f),
            new Vector2(0.5208614f, 0.590131f),
            new Vector2(0.5098858f, 0.5791554f),
            new Vector2(0.502839f, 0.5653252f),
            new Vector2(0.5004108f, 0.5499944f),
            new Vector2(0.502839f, 0.5346636f),
            new Vector2(0.5098858f, 0.5208334f),
            new Vector2(0.5208614f, 0.5098578f),
            new Vector2(0.5346916f, 0.502811f),
            new Vector2(0.5500224f, 0.5003828f),
            new Vector2(0.5653533f, 0.502811f),
            new Vector2(0.5791835f, 0.5098578f),
            new Vector2(0.5901591f, 0.5208334f),
            new Vector2(0.5972059f, 0.5346636f),
            new Vector2(0.5996341f, 0.5499944f),
        };

        return mast;
    }

    GameObject CreateWheel()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject wheel = new GameObject("Wheel");
        wheel.name = "Wheel";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = wheel.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = wheel.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.4999999f, -0.5000001f),
            new Vector3(0f, -0.4999999f, -0.5000001f),
            new Vector3(-0.5f, -0.5000001f, 0.5000001f),
            new Vector3(0f, -0.5000001f, 0.5000001f),
            new Vector3(0.1875f, -0.5f, 0.0625f),
            new Vector3(-0.1875f, -0.5f, 0.0625f),
            new Vector3(0.1875f, 0.0625f, 0.0625f),
            new Vector3(-0.1875f, 0.0625f, 0.0625f),
            new Vector3(0.1875f, 0.0625f, -0.125f),
            new Vector3(-0.1875f, 0.0625f, -0.125f),
            new Vector3(0.1875f, -0.5f, -0.125f),
            new Vector3(-0.1875f, -0.5f, -0.125f),
            new Vector3(0.1875f, 0.0625f, 0.0625f),
            new Vector3(-0.1875f, 0.0625f, 0.0625f),
            new Vector3(0.1875f, 0.0625f, -0.125f),
            new Vector3(-0.1875f, 0.0625f, -0.125f),
            new Vector3(0.1875f, -0.5f, -0.125f),
            new Vector3(0.1875f, -0.5f, 0.0625f),
            new Vector3(-0.1875f, -0.5f, 0.0625f),
            new Vector3(-0.1875f, -0.5f, -0.125f),
            new Vector3(-0.1875f, -0.5f, 0.0625f),
            new Vector3(-0.1875f, 0.0625f, 0.0625f),
            new Vector3(-0.1875f, 0.0625f, -0.125f),
            new Vector3(-0.1875f, -0.5f, -0.125f),
            new Vector3(0.1875f, -0.5f, -0.125f),
            new Vector3(0.1875f, 0.0625f, -0.125f),
            new Vector3(0.1875f, 0.0625f, 0.0625f),
            new Vector3(0.1875f, -0.5f, 0.0625f),
            new Vector3(-0.06249997f, 0.3269259f, -0.1374747f),
            new Vector3(-0.06249997f, 0.2780998f, -0.2333013f),
            new Vector3(-0.06249999f, 0.2020513f, -0.3093497f),
            new Vector3(-0.0625f, 0.1062247f, -0.3581758f),
            new Vector3(-0.06250001f, 7.450581E-09f, -0.3750002f),
            new Vector3(-0.06250002f, -0.1062246f, -0.3581758f),
            new Vector3(-0.06250003f, -0.2020513f, -0.3093497f),
            new Vector3(-0.06250004f, -0.2780997f, -0.2333012f),
            new Vector3(-0.06250004f, -0.3269258f, -0.1374746f),
            new Vector3(-0.06250005f, -0.3437501f, -0.03125f),
            new Vector3(-0.06250004f, -0.3269258f, 0.07497461f),
            new Vector3(-0.06250004f, -0.2780997f, 0.1708012f),
            new Vector3(-0.06250003f, -0.2020512f, 0.2468496f),
            new Vector3(-0.06250002f, -0.1062246f, 0.2956757f),
            new Vector3(-0.06250001f, -2.793969E-09f, 0.3125f),
            new Vector3(-0.0625f, 0.1062246f, 0.2956757f),
            new Vector3(-0.06249999f, 0.2020512f, 0.2468496f),
            new Vector3(-0.06249997f, 0.2780996f, 0.1708012f),
            new Vector3(-0.06249997f, 0.3269257f, 0.0749746f),
            new Vector3(-0.06249997f, 0.34375f, -0.03125f),
            new Vector3(0.06250004f, 0.3269259f, -0.1374747f),
            new Vector3(0.06250004f, 0.2780998f, -0.2333013f),
            new Vector3(0.06250003f, 0.2020513f, -0.3093497f),
            new Vector3(0.06250002f, 0.1062246f, -0.3581758f),
            new Vector3(0.06250001f, -7.450581E-09f, -0.3750002f),
            new Vector3(0.0625f, -0.1062247f, -0.3581758f),
            new Vector3(0.06249999f, -0.2020513f, -0.3093497f),
            new Vector3(0.06249997f, -0.2780997f, -0.2333012f),
            new Vector3(0.06249997f, -0.3269258f, -0.1374746f),
            new Vector3(0.06249997f, -0.3437501f, -0.03125f),
            new Vector3(0.06249997f, -0.3269258f, 0.07497461f),
            new Vector3(0.06249997f, -0.2780997f, 0.1708012f),
            new Vector3(0.06249999f, -0.2020513f, 0.2468496f),
            new Vector3(0.0625f, -0.1062246f, 0.2956757f),
            new Vector3(0.06250001f, -1.769513E-08f, 0.3125f),
            new Vector3(0.06250002f, 0.1062246f, 0.2956757f),
            new Vector3(0.06250003f, 0.2020512f, 0.2468496f),
            new Vector3(0.06250004f, 0.2780996f, 0.1708012f),
            new Vector3(0.06250004f, 0.3269257f, 0.0749746f),
            new Vector3(0.06250004f, 0.34375f, -0.03125f),
            new Vector3(-0.06250001f, 7.450581E-09f, -0.03125f),
            new Vector3(0.06250001f, -7.450581E-09f, -0.03125f),
            new Vector3(-0.06250005f, -0.3437501f, -0.03125f),
            new Vector3(0.06249997f, -0.3437501f, -0.03125f),
            new Vector3(-0.06249997f, 0.34375f, -0.03125f),
            new Vector3(0.06250004f, 0.34375f, -0.03125f),
            new Vector3(0.06250004f, 0.3269259f, -0.1374747f),
            new Vector3(-0.06249997f, 0.3269259f, -0.1374747f),
            new Vector3(-0.06249997f, 0.2780998f, -0.2333013f),
            new Vector3(-0.06249997f, 0.3269259f, -0.1374747f),
            new Vector3(-0.06249999f, 0.2020513f, -0.3093497f),
            new Vector3(-0.0625f, 0.1062247f, -0.3581758f),
            new Vector3(-0.06250001f, 7.450581E-09f, -0.3750002f),
            new Vector3(-0.06250002f, -0.1062246f, -0.3581758f),
            new Vector3(-0.06250003f, -0.2020513f, -0.3093497f),
            new Vector3(-0.06250004f, -0.2780997f, -0.2333012f),
            new Vector3(-0.06250004f, -0.3269258f, -0.1374746f),
            new Vector3(-0.06250005f, -0.3437501f, -0.03125f),
            new Vector3(-0.06250004f, -0.3269258f, 0.07497461f),
            new Vector3(-0.06250004f, -0.2780997f, 0.1708012f),
            new Vector3(-0.06250003f, -0.2020512f, 0.2468496f),
            new Vector3(-0.06250002f, -0.1062246f, 0.2956757f),
            new Vector3(-0.06250001f, -2.793969E-09f, 0.3125f),
            new Vector3(-0.0625f, 0.1062246f, 0.2956757f),
            new Vector3(-0.06249999f, 0.2020512f, 0.2468496f),
            new Vector3(-0.06249997f, 0.2780996f, 0.1708012f),
            new Vector3(-0.06249997f, 0.3269257f, 0.0749746f),
            new Vector3(-0.06249997f, 0.34375f, -0.03125f),
            new Vector3(0.06250004f, 0.3269259f, -0.1374747f),
            new Vector3(0.06250004f, 0.2780998f, -0.2333013f),
            new Vector3(0.06250003f, 0.2020513f, -0.3093497f),
            new Vector3(0.06250002f, 0.1062246f, -0.3581758f),
            new Vector3(0.06250001f, -7.450581E-09f, -0.3750002f),
            new Vector3(0.0625f, -0.1062247f, -0.3581758f),
            new Vector3(0.06249999f, -0.2020513f, -0.3093497f),
            new Vector3(0.06249997f, -0.2780997f, -0.2333012f),
            new Vector3(0.06249997f, -0.3269258f, -0.1374746f),
            new Vector3(0.06249997f, -0.3437501f, -0.03125f),
            new Vector3(0.06249997f, -0.3269258f, 0.07497461f),
            new Vector3(0.06249997f, -0.2780997f, 0.1708012f),
            new Vector3(0.06249999f, -0.2020513f, 0.2468496f),
            new Vector3(0.0625f, -0.1062246f, 0.2956757f),
            new Vector3(0.06250001f, -1.769513E-08f, 0.3125f),
            new Vector3(0.06250002f, 0.1062246f, 0.2956757f),
            new Vector3(0.06250003f, 0.2020512f, 0.2468496f),
            new Vector3(0.06250004f, 0.2780996f, 0.1708012f),
            new Vector3(0.06250004f, 0.3269257f, 0.0749746f),
            new Vector3(0.06250004f, 0.34375f, -0.03125f),
            new Vector3(0f, -0.4999999f, -0.5000001f),
            new Vector3(0.5f, -0.4999999f, -0.5000001f),
            new Vector3(0f, -0.5000001f, 0.5000001f),
            new Vector3(0.5f, -0.5000001f, 0.5000001f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 6, 7,
            4, 7, 5,
            12, 8, 9,
            12, 9, 13,
            14, 10, 11,
            14, 11, 15,
            16, 17, 18,
            16, 18, 19,
            20, 21, 22,
            20, 22, 23,
            24, 25, 26,
            24, 26, 27,
            28, 48, 49,
            28, 49, 29,
            29, 49, 50,
            29, 50, 30,
            30, 50, 51,
            30, 51, 31,
            31, 51, 52,
            31, 52, 32,
            32, 52, 53,
            32, 53, 33,
            33, 53, 54,
            33, 54, 34,
            34, 54, 55,
            34, 55, 35,
            35, 55, 56,
            35, 56, 36,
            36, 56, 57,
            36, 57, 37,
            70, 71, 58,
            70, 58, 38,
            38, 58, 59,
            38, 59, 39,
            39, 59, 60,
            39, 60, 40,
            40, 60, 61,
            40, 61, 41,
            41, 61, 62,
            41, 62, 42,
            42, 62, 63,
            42, 63, 43,
            43, 63, 64,
            43, 64, 44,
            44, 64, 65,
            44, 65, 45,
            45, 65, 66,
            45, 66, 46,
            46, 66, 67,
            46, 67, 47,
            72, 73, 74,
            72, 74, 75,
            76, 68, 77,
            78, 68, 76,
            77, 68, 95,
            79, 68, 78,
            95, 68, 94,
            80, 68, 79,
            94, 68, 93,
            81, 68, 80,
            93, 68, 92,
            82, 68, 81,
            92, 68, 91,
            83, 68, 82,
            91, 68, 90,
            84, 68, 83,
            90, 68, 89,
            85, 68, 84,
            89, 68, 88,
            86, 68, 85,
            88, 68, 87,
            87, 68, 86,
            96, 69, 97,
            115, 69, 96,
            97, 69, 98,
            114, 69, 115,
            98, 69, 99,
            113, 69, 114,
            99, 69, 100,
            112, 69, 113,
            100, 69, 101,
            111, 69, 112,
            101, 69, 102,
            110, 69, 111,
            102, 69, 103,
            109, 69, 110,
            103, 69, 104,
            108, 69, 109,
            104, 69, 105,
            107, 69, 108,
            105, 69, 106,
            106, 69, 107,
            116, 119, 117,
            119, 116, 118,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(-0.25f, 0f),
            new Vector2(0.25f, 0f),
            new Vector2(-0.25f, 1f),
            new Vector2(0.25f, 1f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.3125f, 0.375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.375f),
            new Vector2(0.4437875f, 0.1250284f),
            new Vector2(0.4500316f, 0.1250284f),
            new Vector2(0.4562757f, 0.1250284f),
            new Vector2(0.4625198f, 0.1250284f),
            new Vector2(0.4687639f, 0.1250284f),
            new Vector2(0.475008f, 0.1250284f),
            new Vector2(0.481252f, 0.1250284f),
            new Vector2(0.4874961f, 0.1250284f),
            new Vector2(0.4937402f, 0.1250284f),
            new Vector2(0.4999843f, 0.1250284f),
            new Vector2(0.4437875f, 0.1250284f),
            new Vector2(0.4500316f, 0.1250284f),
            new Vector2(0.4562757f, 0.1250284f),
            new Vector2(0.4625198f, 0.1250284f),
            new Vector2(0.4687638f, 0.1250284f),
            new Vector2(0.4750079f, 0.1250284f),
            new Vector2(0.481252f, 0.1250284f),
            new Vector2(0.4874961f, 0.1250284f),
            new Vector2(0.4937402f, 0.1250284f),
            new Vector2(0.4999843f, 0.1250284f),
            new Vector2(0.4437875f, 0.1874644f),
            new Vector2(0.4500316f, 0.1874644f),
            new Vector2(0.4562757f, 0.1874644f),
            new Vector2(0.4625198f, 0.1874644f),
            new Vector2(0.4687639f, 0.1874644f),
            new Vector2(0.475008f, 0.1874644f),
            new Vector2(0.481252f, 0.1874644f),
            new Vector2(0.4874961f, 0.1874644f),
            new Vector2(0.4937402f, 0.1874644f),
            new Vector2(0.4999843f, 0.1874644f),
            new Vector2(0.4437875f, 0.1874644f),
            new Vector2(0.4500316f, 0.1874644f),
            new Vector2(0.4562757f, 0.1874644f),
            new Vector2(0.4625198f, 0.1874644f),
            new Vector2(0.4687638f, 0.1874644f),
            new Vector2(0.4750079f, 0.1874644f),
            new Vector2(0.481252f, 0.1874644f),
            new Vector2(0.4874961f, 0.1874644f),
            new Vector2(0.4937401f, 0.1874644f),
            new Vector2(0.4999842f, 0.1874644f),
            new Vector2(0.468764f, 0.1562465f),
            new Vector2(0.468764f, 0.1562465f),
            new Vector2(0.4375434f, 0.1250284f),
            new Vector2(0.4375434f, 0.1874644f),
            new Vector2(0.4375434f, 0.1250284f),
            new Vector2(0.4375434f, 0.1874644f),
            new Vector2(0.4437875f, 0.1874644f),
            new Vector2(0.4437875f, 0.1250284f),
            new Vector2(0.4436786f, 0.1744721f),
            new Vector2(0.4392743f, 0.1658283f),
            new Vector2(0.4505383f, 0.1813319f),
            new Vector2(0.4591822f, 0.1857362f),
            new Vector2(0.468764f, 0.1872538f),
            new Vector2(0.4783458f, 0.1857362f),
            new Vector2(0.4869896f, 0.1813319f),
            new Vector2(0.4938494f, 0.1744721f),
            new Vector2(0.4982537f, 0.1658283f),
            new Vector2(0.4997713f, 0.1562465f),
            new Vector2(0.4982537f, 0.1466647f),
            new Vector2(0.4938494f, 0.1380209f),
            new Vector2(0.4869896f, 0.1311611f),
            new Vector2(0.4783458f, 0.1267568f),
            new Vector2(0.4687639f, 0.1252392f),
            new Vector2(0.4591822f, 0.1267569f),
            new Vector2(0.4505383f, 0.1311611f),
            new Vector2(0.4436786f, 0.1380209f),
            new Vector2(0.4392743f, 0.1466648f),
            new Vector2(0.4377567f, 0.1562465f),
            new Vector2(0.4982537f, 0.1658283f),
            new Vector2(0.4938495f, 0.1744721f),
            new Vector2(0.4869897f, 0.1813319f),
            new Vector2(0.4783458f, 0.1857362f),
            new Vector2(0.468764f, 0.1872538f),
            new Vector2(0.4591822f, 0.1857362f),
            new Vector2(0.4505384f, 0.1813319f),
            new Vector2(0.4436786f, 0.1744721f),
            new Vector2(0.4392743f, 0.1658283f),
            new Vector2(0.4377567f, 0.1562465f),
            new Vector2(0.4392744f, 0.1466647f),
            new Vector2(0.4436786f, 0.1380209f),
            new Vector2(0.4505384f, 0.1311611f),
            new Vector2(0.4591822f, 0.1267568f),
            new Vector2(0.468764f, 0.1252392f),
            new Vector2(0.4783458f, 0.1267568f),
            new Vector2(0.4869896f, 0.1311611f),
            new Vector2(0.4938494f, 0.1380209f),
            new Vector2(0.4982537f, 0.1466647f),
            new Vector2(0.4997713f, 0.1562465f),
            new Vector2(-0.25f, 0f),
            new Vector2(0.25f, 0f),
            new Vector2(-0.25f, 1f),
            new Vector2(0.25f, 1f),
        };

        return wheel;
    }

    Vector3[] BridgeMeshVertices = null;
    int[] BridgeMeshTriangles = null;
    Vector2[] BridgeMeshUV = null;

    GameObject CreateBridge()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject bridge = new GameObject("Bridge");
        bridge.name = "Bridge";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = bridge.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = bridge.AddComponent<MeshRenderer>();

        if (BridgeMeshVertices == null)
        {
            meshFilter.mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.1875f, 0.5f),
                new Vector3(0.5f, -0.1875f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.1875f, -0.5f),
                new Vector3(0.5f, -0.1875f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(0.5f, -0.5f, 0.0f),

                // double sided sides
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.1875f, 0.5f),
                new Vector3(-0.5f, -0.1875f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.1875f, -0.5f),
                new Vector3(-0.5f, -0.1875f, -0.5f)
            };

            meshFilter.mesh.triangles = new int[]
            {
                0, 3, 1,
                3, 0, 2,
                4, 7, 5,
                7, 4, 6,
                8, 11, 9,
                11, 8, 10,
                12, 15, 13,
                15, 12, 14,

                // double sided sides
                16, 19, 17,
                19, 16, 18,
                20, 23, 21,
                23, 20, 22,
            };

            meshFilter.mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0.3125f), // 6
                new Vector2(1f, 0.3125f), // 6
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
                new Vector2(0f, 0.3125f), // 6
                new Vector2(1f, 0.3125f), // 6
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
            
                // double sided sides
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(0f, 0.6875f), // 11
                new Vector2(1f, 0.6875f), // 11
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            };

            BridgeMeshVertices = meshFilter.mesh.vertices;
            BridgeMeshTriangles = meshFilter.mesh.triangles; 
            BridgeMeshUV = meshFilter.mesh.uv;
        }
        else
        {
            meshFilter.mesh.vertices = BridgeMeshVertices;
            meshFilter.mesh.triangles = BridgeMeshTriangles;
            meshFilter.mesh.uv = BridgeMeshUV;
        }

        return bridge;
    }

    GameObject CreateDoor()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject door = new GameObject("Door");
        door.name = "Door";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = door.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = door.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(0.0625f, -0.5f, 0.5f),
            new Vector3(-0.0625f, -0.5f, 0.5f),
            new Vector3(0.0625f, 0.5f, 0.5f),
            new Vector3(-0.0625f, 0.5f, 0.5f),
            new Vector3(0.0625f, 0.5f, -0.5f),
            new Vector3(-0.0625f, 0.5f, -0.5f),
            new Vector3(0.0625f, -0.5f, -0.5f),
            new Vector3(-0.0625f, -0.5f, -0.5f),
            new Vector3(0.0625f, 0.5f, 0.5f),
            new Vector3(-0.0625f, 0.5f, 0.5f),
            new Vector3(0.0625f, 0.5f, -0.5f),
            new Vector3(-0.0625f, 0.5f, -0.5f),
            new Vector3(0.0625f, -0.5f, -0.5f),
            new Vector3(0.0625f, -0.5f, 0.5f),
            new Vector3(-0.0625f, -0.5f, 0.5f),
            new Vector3(-0.0625f, -0.5f, -0.5f),
            new Vector3(-0.0625f, -0.5f, 0.5f),
            new Vector3(-0.0625f, 0.5f, 0.5f),
            new Vector3(-0.0625f, 0.5f, -0.5f),
            new Vector3(-0.0625f, -0.5f, -0.5f),
            new Vector3(0.0625f, -0.5f, -0.5f),
            new Vector3(0.0625f, 0.5f, -0.5f),
            new Vector3(0.0625f, 0.5f, 0.5f),
            new Vector3(0.0625f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.25f, -0.0625f, -0.375f),
            new Vector3(0.125f, -0.0625f, -0.375f),
            new Vector3(0.25f, 0.0625f, -0.375f),
            new Vector3(0.125f, 0.0625f, -0.375f),
            new Vector3(0.25f, 0.0625f, -0.0625f),
            new Vector3(0.125f, 0.0625f, -0.0625f),
            new Vector3(0.25f, -0.0625f, -0.0625f),
            new Vector3(0.125f, -0.0625f, -0.0625f),
            new Vector3(0.25f, 0.0625f, -0.375f),
            new Vector3(0.125f, 0.0625f, -0.375f),
            new Vector3(0.25f, 0.0625f, -0.0625f),
            new Vector3(0.125f, 0.0625f, -0.0625f),
            new Vector3(0.25f, -0.0625f, -0.0625f),
            new Vector3(0.25f, -0.0625f, -0.375f),
            new Vector3(0.125f, -0.0625f, -0.375f),
            new Vector3(0.125f, -0.0625f, -0.0625f),
            new Vector3(0.125f, -0.0625f, -0.375f),
            new Vector3(0.125f, 0.0625f, -0.375f),
            new Vector3(0.125f, 0.0625f, -0.0625f),
            new Vector3(0.125f, -0.0625f, -0.0625f),
            new Vector3(0.25f, -0.0625f, -0.0625f),
            new Vector3(0.25f, 0.0625f, -0.0625f),
            new Vector3(0.25f, 0.0625f, -0.375f),
            new Vector3(0.25f, -0.0625f, -0.375f),
            new Vector3(-0.125f, -0.03125f, -0.3125f),
            new Vector3(0.125f, -0.03125f, -0.3125f),
            new Vector3(-0.125f, 0.03125f, -0.3125f),
            new Vector3(0.125f, 0.03125f, -0.3125f),
            new Vector3(-0.125f, 0.03125f, -0.25f),
            new Vector3(0.125f, 0.03125f, -0.25f),
            new Vector3(-0.125f, -0.03125f, -0.25f),
            new Vector3(0.125f, -0.03125f, -0.25f),
            new Vector3(-0.125f, 0.03125f, -0.3125f),
            new Vector3(0.125f, 0.03125f, -0.3125f),
            new Vector3(-0.125f, 0.03125f, -0.25f),
            new Vector3(0.125f, 0.03125f, -0.25f),
            new Vector3(-0.125f, -0.03125f, -0.25f),
            new Vector3(-0.125f, -0.03125f, -0.3125f),
            new Vector3(0.125f, -0.03125f, -0.3125f),
            new Vector3(0.125f, -0.03125f, -0.25f),
            new Vector3(0.125f, -0.03125f, -0.3125f),
            new Vector3(0.125f, 0.03125f, -0.3125f),
            new Vector3(0.125f, 0.03125f, -0.25f),
            new Vector3(0.125f, -0.03125f, -0.25f),
            new Vector3(-0.125f, -0.03125f, -0.25f),
            new Vector3(-0.125f, 0.03125f, -0.25f),
            new Vector3(-0.125f, 0.03125f, -0.3125f),
            new Vector3(-0.125f, -0.03125f, -0.3125f),
            new Vector3(-0.25f, -0.0625f, -0.3750027f),
            new Vector3(-0.125f, -0.0625f, -0.3750027f),
            new Vector3(-0.25f, 0.0625f, -0.3750027f),
            new Vector3(-0.125f, 0.0625f, -0.3750027f),
            new Vector3(-0.25f, 0.0625f, -0.0625027f),
            new Vector3(-0.125f, 0.0625f, -0.0625027f),
            new Vector3(-0.25f, -0.0625f, -0.0625027f),
            new Vector3(-0.125f, -0.0625f, -0.0625027f),
            new Vector3(-0.25f, 0.0625f, -0.3750027f),
            new Vector3(-0.125f, 0.0625f, -0.3750027f),
            new Vector3(-0.25f, 0.0625f, -0.0625027f),
            new Vector3(-0.125f, 0.0625f, -0.0625027f),
            new Vector3(-0.25f, -0.0625f, -0.0625027f),
            new Vector3(-0.25f, -0.0625f, -0.3750027f),
            new Vector3(-0.125f, -0.0625f, -0.3750027f),
            new Vector3(-0.125f, -0.0625f, -0.0625027f),
            new Vector3(-0.125f, -0.0625f, -0.3750027f),
            new Vector3(-0.125f, 0.0625f, -0.3750027f),
            new Vector3(-0.125f, 0.0625f, -0.0625027f),
            new Vector3(-0.125f, -0.0625f, -0.0625027f),
            new Vector3(-0.25f, -0.0625f, -0.0625027f),
            new Vector3(-0.25f, 0.0625f, -0.0625027f),
            new Vector3(-0.25f, 0.0625f, -0.3750027f),
            new Vector3(-0.25f, -0.0625f, -0.3750027f),
            new Vector3(0.1875f, 0.06300001f, -0.3125f),
            new Vector3(0.25f, 0.06300001f, -0.3125f),
            new Vector3(0.1875f, 0.06299999f, -0.25f),
            new Vector3(0.25f, 0.06299999f, -0.25f),
            new Vector3(-0.1875f, 0.06300001f, -0.25f),
            new Vector3(-0.25f, 0.06300001f, -0.25f),
            new Vector3(-0.1875f, 0.06299999f, -0.3125f),
            new Vector3(-0.25f, 0.06299999f, -0.3125f)
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 2, 3,
            0, 3, 1,
            8, 4, 5,
            8, 5, 9,
            10, 6, 7,
            10, 7, 11,
            12, 13, 14,
            12, 14, 15,
            16, 17, 18,
            16, 18, 19,
            20, 21, 22,
            20, 22, 23,
            24, 27, 25,
            27, 24, 26,
            28, 31, 29,
            31, 28, 30,
            55, 54, 52,
            54, 53, 52,
            51, 50, 48,
            50, 49, 48,
            47, 46, 44,
            46, 45, 44,
            43, 39, 42,
            39, 38, 42,
            41, 37, 40,
            37, 36, 40,
            33, 35, 32,
            35, 34, 32,
            56, 58, 59,
            56, 59, 57,
            64, 60, 61,
            64, 61, 65,
            66, 62, 63,
            66, 63, 67,
            68, 69, 70,
            68, 70, 71,
            72, 73, 74,
            72, 74, 75,
            76, 77, 78,
            76, 78, 79,
            80, 82, 83,
            80, 83, 81,
            88, 84, 85,
            88, 85, 89,
            90, 86, 87,
            90, 87, 91,
            92, 93, 94,
            92, 94, 95,
            96, 97, 98,
            96, 98, 99,
            100, 101, 102,
            100, 102, 103,
            104, 107, 105,
            107, 104, 106,
            108, 111, 109,
            111, 108, 110
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0.4375f, 0f),
            new Vector2(0.5625f, 0f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.4375f, 0f),
            new Vector2(0.5625f, 0f),
            new Vector2(0.4375f, 0f),
            new Vector2(0.5625f, 0f),
            new Vector2(0.4375f, 0f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.5625f, 0f),
            new Vector2(0.4375f, 0f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.5625f, 0f),
            new Vector2(0.4375f, 0f),
            new Vector2(0.4375f, 1f),
            new Vector2(0.5625f, 1f),
            new Vector2(0.5625f, 0f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.5625f, 0.1875f),
            new Vector2(0.5625f, 0.25f),
            new Vector2(0.625f, 0.25f),
            new Vector2(0.625f, 0.1875f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.3125f, 0.125f),
            new Vector2(0.3125f, 0.4375f),
            new Vector2(0.375f, 0.4375f),
            new Vector2(0.375f, 0.125f),
            new Vector2(0.25f, 0.1875f),
            new Vector2(0.3125f, 0.1875f),
            new Vector2(0.25f, 0.25f),
            new Vector2(0.3125f, 0.25f),
            new Vector2(0.25f, 0.1875f),
            new Vector2(0.3125f, 0.1875f),
            new Vector2(0.25f, 0.25f),
            new Vector2(0.3125f, 0.25f)
        };

        return door;
    }

    GameObject CreateCastleLeft()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject castleLeft = new GameObject("Castle Left");
        castleLeft.name = "Castle Left";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = castleLeft.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = castleLeft.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(-0.375f, -0.5f, 0.375f),
            new Vector3(-0.375f, -0.5f, 0.0f),
            new Vector3(-0.375f, 0.5f, 0.375f),
            new Vector3(-0.375f, 0.5f, 0.0f),
            new Vector3(-0.375f, -0.5f, 0.0f),
            new Vector3(-0.375f, -0.5f, -0.375f),
            new Vector3(-0.375f, 0.5f, 0.0f),
            new Vector3(-0.375f, 0.5f, -0.375f),
            new Vector3(-0.375f, -0.5f, 0.0f),
            new Vector3(-0.375f, -0.5f, 0.375f),
            new Vector3(-0.375f, 0.5f, 0.0f),
            new Vector3(-0.375f, 0.5f, 0.375f),
            new Vector3(-0.375f, -0.5f, -0.375f),
            new Vector3(-0.375f, -0.5f, 0.0f),
            new Vector3(-0.375f, 0.5f, -0.375f),
            new Vector3(-0.375f, 0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(-0.375f, 0.0f, -0.375f),
            new Vector3(0.5f, 0.0f, -0.375f),
            new Vector3(-0.375f, -0.0f, 0.0f),
            new Vector3(0.5f, -0.0f, 0.0f),
            new Vector3(-0.375f, 0.0f, 0.0f),
            new Vector3(0.5f, 0.0f, 0.0f),
            new Vector3(-0.375f, -0.0f, 0.375f),
            new Vector3(0.5f, -0.0f, 0.375f)
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            6, 4, 7,
            5, 7, 4,
            8, 11, 9,
            11, 8, 10,
            12, 15, 13,
            15, 12, 14,
            16, 19, 17,
            19, 16, 18,
            20, 23, 21,
            23, 20, 22,
            26, 24, 27,
            25, 27, 24,
            28, 31, 29,
            31, 28, 30,
            32, 35, 33,
            35, 32, 34,
            36, 39, 37,
            39, 36, 38
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.625f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.625f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.625f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.625f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.625f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.625f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.625f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.625f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.125f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.125f, 0.375f),
            new Vector2(1f, 0.375f),
            new Vector2(0.125f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0.125f, 0.375f),
            new Vector2(1f, 0.375f)
        };

        return castleLeft;
    }

    GameObject CreateCastleRight()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject CastleRight = new GameObject("Castle Right");
        CastleRight.name = "Castle Right";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = CastleRight.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = CastleRight.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(0.375f, -0.5f, 0.0f),
            new Vector3(0.375f, -0.5f, 0.375f),
            new Vector3(0.375f, 0.5f, 0.0f),
            new Vector3(0.375f, 0.5f, 0.375f),
            new Vector3(0.375f, -0.5f, -0.375f),
            new Vector3(0.375f, -0.5f, 0.0f),
            new Vector3(0.375f, 0.5f, -0.375f),
            new Vector3(0.375f, 0.5f, 0.0f),
            new Vector3(0.375f, -0.5f, 0.375f),
            new Vector3(0.375f, -0.5f, 0.0f),
            new Vector3(0.375f, 0.5f, 0.375f),
            new Vector3(0.375f, 0.5f, 0.0f),
            new Vector3(0.375f, -0.5f, 0.0f),
            new Vector3(0.375f, -0.5f, -0.375f),
            new Vector3(0.375f, 0.5f, 0.0f),
            new Vector3(0.375f, 0.5f, -0.375f),
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(-0.5f, 0.0f, -0.375f),
            new Vector3(0.375f, 0.0f, -0.375f),
            new Vector3(-0.5f, -0.0f, 0.0f),
            new Vector3(0.375f, -0.0f, 0.0f),
            new Vector3(-0.5f, 0.0f, 0.0f),
            new Vector3(0.375f, 0.0f, 0.0f),
            new Vector3(-0.5f, -0.0f, 0.375f),
            new Vector3(0.375f, -0.0f, 0.375f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            6, 4, 7,
            5, 7, 4,
            8, 11, 9,
            11, 8, 10,
            12, 15, 13,
            15, 12, 14,
            16, 19, 17,
            19, 16, 18,
            20, 23, 21,
            23, 20, 22,
            26, 24, 27,
            25, 27, 24,
            28, 31, 29,
            31, 28, 30,
            32, 35, 33,
            35, 32, 34,
            36, 39, 37,
            39, 36, 38,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0.375f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0.375f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0.375f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0.375f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(0.875f, 0f),
            new Vector2(0f, 0.375f),
            new Vector2(0.875f, 0.375f),
            new Vector2(0f, 0f),
            new Vector2(0.875f, 0f),
            new Vector2(0f, 0.375f),
            new Vector2(0.875f, 0.375f),
        };

        return CastleRight;
    }

    GameObject CreateCastleCenter()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject CastleCenter = new GameObject("Castle Center");
        CastleCenter.name = "Castle Center";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = CastleCenter.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = CastleCenter.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(-0.5f, 0f, 0.375f),
            new Vector3(0f, 0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(-0.5f, -0.5f, -0.375f),
            new Vector3(0.5f, -0.5f, -0.375f),
            new Vector3(-0.5f, 0.5f, -0.375f),
            new Vector3(0.5f, 0.5f, -0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(0.5f, 0f, 0.375f),
            new Vector3(0f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(0.5f, 0f, 0.375f),
            new Vector3(0f, 0.5f, 0.375f),
            new Vector3(0.5f, 0.5f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(-0.5f, 0f, 0.375f),
            new Vector3(0f, 0.5f, 0.375f),
            new Vector3(-0.5f, 0.5f, 0.375f),
            new Vector3(0.25f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(0.25f, 0f, 0.375f),
            new Vector3(0.5f, 0f, 0.375f),
            new Vector3(0f, -0.5f, 0.375f),
            new Vector3(0.25f, -0.5f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(0.25f, 0f, 0.375f),
            new Vector3(-0.25f, -0.5f, 0.375f),
            new Vector3(0f, -0.5f, 0.375f),
            new Vector3(-0.25f, 0f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(-0.25f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0f, 0.375f),
            new Vector3(-0.25f, 0f, 0.375f),
            new Vector3(0.25f, -0.5f, 0.375f),
            new Vector3(0.5f, -0.5f, 0.375f),
            new Vector3(0.25f, 0f, 0.375f),
            new Vector3(0.5f, 0f, 0.375f),
            new Vector3(0f, -0.5f, 0.375f),
            new Vector3(0.25f, -0.5f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(0.25f, 0f, 0.375f),
            new Vector3(-0.25f, -0.5f, 0.375f),
            new Vector3(0f, -0.5f, 0.375f),
            new Vector3(-0.25f, 0f, 0.375f),
            new Vector3(0f, 0f, 0.375f),
            new Vector3(-0.5f, -0.5f, 0.375f),
            new Vector3(-0.25f, -0.5f, 0.375f),
            new Vector3(-0.5f, 0f, 0.375f),
            new Vector3(-0.25f, 0f, 0.375f),
            new Vector3(0.5f, 0.0f, 0.375f),
            new Vector3(0.25f, 0.0f, 0.375f),
            new Vector3(0.5f, 0.0f, -0.125f),
            new Vector3(0.25f, 0.0f, -0.125f),
            new Vector3(0.25f, 0.0f, 0.375f),
            new Vector3(0.0f, 0.0f, 0.375f),
            new Vector3(0.25f, 0.0f, -0.125f),
            new Vector3(0.0f, 0.0f, -0.125f),
            new Vector3(0.0f, 0.0f, 0.375f),
            new Vector3(-0.25f, 0.0f, 0.375f),
            new Vector3(0.0f, 0.0f, -0.125f),
            new Vector3(-0.25f, 0.0f, -0.125f),
            new Vector3(-0.25f, 0.0f, 0.375f),
            new Vector3(-0.5f, 0.0f, 0.375f),
            new Vector3(-0.25f, 0.0f, -0.125f),
            new Vector3(-0.5f, 0.0f, -0.125f),
            new Vector3(0f, 0.0f, -0.375f),
            new Vector3(0f, 0.0f, -0.125f),
            new Vector3(0.5f, 0.0f, -0.375f),
            new Vector3(0.5f, 0.0f, -0.125f),
            new Vector3(-0.5f, 0.0f, -0.375f),
            new Vector3(-0.5f, 0.0f, -0.125f),
            new Vector3(0f, 0.0f, -0.375f),
            new Vector3(0f, 0.0f, -0.125f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 7, 5,
            7, 4, 6,
            10, 8, 11,
            9, 11, 8,
            14, 12, 15,
            13, 15, 12,
            16, 19, 17,
            19, 16, 18,
            22, 20, 23,
            21, 23, 20,
            24, 27, 25,
            27, 24, 26,
            28, 31, 29,
            31, 28, 30,
            32, 35, 33,
            35, 32, 34,
            36, 39, 37,
            39, 36, 38,
            42, 40, 43,
            41, 43, 40,
            46, 44, 47,
            45, 47, 44,
            50, 48, 51,
            49, 51, 48,
            54, 52, 55,
            53, 55, 52,
            56, 59, 57,
            59, 56, 58,
            60, 63, 61,
            63, 60, 62,
            64, 67, 65,
            67, 64, 66,
            68, 71, 69,
            71, 68, 70,
            74, 72, 75,
            73, 75, 72,
            78, 76, 79,
            77, 79, 76,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.5f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
            new Vector2(0.125f, 0f),
            new Vector2(0.375f, 0f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.5f),
        };

        return CastleCenter;
    }

    GameObject CreatePyramid(float height = 1.0f)
    {
        GameObject pyramid = new GameObject("Pyramid");
        pyramid.name = "Pyramid";

        MeshFilter meshFilter = pyramid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pyramid.AddComponent<MeshRenderer>();

        Vector3 p4;
        if (height == 0.0f)
        {
            p4 = new Vector3(0f, 0f, 1.0f / Mathf.Sqrt(2));
        }
        else
        {
            p4 = new Vector3(0f, 0f, height);
        }

        meshFilter.mesh.vertices = new Vector3[] 
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0), 
            p4 
        };

        meshFilter.mesh.triangles = new int[] 
        {
            4,0,1,
            4,1,2,
            4,2,3,
            4,3,0
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector3(0f, 0f),
            new Vector3(0f, 1f),
            new Vector3(1f, 1f),
            new Vector3(1f, 0f),
            new Vector3(0.5f, 0.5f)
        };

        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.Optimize();

        // TODO rotate the verts instead of the created object;
        //pyramid.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        return pyramid;
    }

    //bool once = true;

    Vector3[] QuadMeshVerticies = null;
    int[] QuadMeshTriangles = null;
    Vector2[] QuadMeshUV = null;

    GameObject CreateQuad()
    {
        GameObject quad = new GameObject("Quad");

        quad.name = "Quad";

        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();

        if (QuadMeshVerticies == null)
        {
            meshFilter.mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f),
                new Vector3(0.5f, 0.5f, 0.0f)
            };

            meshFilter.mesh.triangles = new int[]
            {
                0, 3, 1,
                3, 0, 2
            };

            meshFilter.mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.Optimize();

            QuadMeshVerticies = meshFilter.mesh.vertices;
            QuadMeshTriangles = meshFilter.mesh.triangles;
            QuadMeshUV = meshFilter.mesh.uv;
        }
        else
        {
            meshFilter.mesh.vertices = QuadMeshVerticies;
            meshFilter.mesh.triangles = QuadMeshTriangles;
            meshFilter.mesh.uv = QuadMeshUV;
        }

        return quad;
    }
    GameObject CreateChest()
    {
        GameObject chest = new GameObject("Chest");

        chest.name = "Chest";

        MeshFilter meshFilter = chest.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = chest.AddComponent<MeshRenderer>();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.28125f, -0.5f, -3.061617E-17f),
            new Vector3(0.28125f, -0.5f, -3.061617E-17f),
            new Vector3(-0.28125f, -0.3125f, 3.061617E-17f),
            new Vector3(0.28125f, -0.3125f, 3.061617E-17f),
            new Vector3(0.28125f, -0.125f, -3.061617E-17f),
            new Vector3(-0.28125f, -0.125f, -3.061617E-17f),
            new Vector3(0.28125f, -0.3125f, 3.061617E-17f),
            new Vector3(-0.28125f, -0.3125f, 3.061617E-17f),
            new Vector3(0.28125f, -0.5f, 0.375f),
            new Vector3(-0.28125f, -0.5f, 0.375f),
            new Vector3(0.28125f, -0.3125f, 0.375f),
            new Vector3(-0.28125f, -0.3125f, 0.375f),
            new Vector3(-0.28125f, -0.125f, 0.375f),
            new Vector3(0.28125f, -0.125f, 0.375f),
            new Vector3(-0.28125f, -0.3125f, 0.375f),
            new Vector3(0.28125f, -0.3125f, 0.375f),
            new Vector3(0.2812501f, -0.06249999f, 0.375f),
            new Vector3(-0.2812501f, -0.06249999f, 0.375f),
            new Vector3(0.2812501f, -0.06250001f, 0.1875f),
            new Vector3(-0.2812501f, -0.06250001f, 0.1875f),
            new Vector3(-0.28125f, -0.06249999f, -1.490116E-08f),
            new Vector3(0.28125f, -0.06249999f, -1.490116E-08f),
            new Vector3(-0.28125f, -0.06250001f, 0.1875f),
            new Vector3(0.28125f, -0.06250001f, 0.1875f),
            new Vector3(-0.09375f, -0.3125f, -0.001f),
            new Vector3(0.09375f, -0.3125f, -0.001f),
            new Vector3(-0.09375f, -0.125f, -0.001f),
            new Vector3(0.09375f, -0.125f, -0.001f),
            new Vector3(0.28125f, -0.1248f, 0.375f),
            new Vector3(-0.28125f, -0.1248f, 0.375f),
            new Vector3(0.28125f, -0.0627f, 0.375f),
            new Vector3(-0.28125f, -0.0627f, 0.375f),
            new Vector3(0.28125f, -0.0627f, 0f),
            new Vector3(-0.28125f, -0.0627f, 0f),
            new Vector3(0.28125f, -0.1248f, 0f),
            new Vector3(-0.28125f, -0.1248f, 0f),
            new Vector3(0.28125f, -0.0627f, 0.375f),
            new Vector3(-0.28125f, -0.0627f, 0.375f),
            new Vector3(0.28125f, -0.0627f, 0f),
            new Vector3(-0.28125f, -0.0627f, 0f),
            new Vector3(0.28125f, -0.1248f, 0f),
            new Vector3(0.28125f, -0.1248f, 0.375f),
            new Vector3(-0.28125f, -0.1248f, 0.375f),
            new Vector3(-0.28125f, -0.1248f, 0f),
            new Vector3(-0.28125f, -0.1248f, 0.375f),
            new Vector3(-0.28125f, -0.0627f, 0.375f),
            new Vector3(-0.28125f, -0.0627f, 0f),
            new Vector3(-0.28125f, -0.1248f, 0f),
            new Vector3(0.28125f, -0.1248f, 0f),
            new Vector3(0.28125f, -0.0627f, 0f),
            new Vector3(0.28125f, -0.0627f, 0.375f),
            new Vector3(0.28125f, -0.1248f, 0.375f),
            new Vector3(0.28125f, -0.5f, 0.1875f),
            new Vector3(0.28125f, -0.5f, 0.375f),
            new Vector3(0.28125f, -0.125f, 0.1875f),
            new Vector3(0.28125f, -0.125f, 0.375f),
            new Vector3(0.28125f, -0.125f, 0.1875f),
            new Vector3(0.28125f, -0.125f, -1.490116E-08f),
            new Vector3(0.28125f, -0.5000001f, 0.1875f),
            new Vector3(0.28125f, -0.5000001f, -1.490116E-08f),
            new Vector3(-0.28125f, -0.5f, 0.1875f),
            new Vector3(-0.28125f, -0.5f, -1.490116E-08f),
            new Vector3(-0.28125f, -0.125f, 0.1875f),
            new Vector3(-0.28125f, -0.125f, -1.490116E-08f),
            new Vector3(-0.28125f, -0.125f, 0.1875f),
            new Vector3(-0.28125f, -0.125f, 0.375f),
            new Vector3(-0.28125f, -0.5000001f, 0.1875f),
            new Vector3(-0.28125f, -0.5000001f, 0.375f),
            new Vector3(-0.09375f, -0.1252f, -0.001f),
            new Vector3(0.09375f, -0.1252f, -0.001f),
            new Vector3(-0.09375f, -0.0623f, -0.001f),
            new Vector3(0.09375f, -0.0623f, -0.001f),
            new Vector3(-0.09375f, -0.0623f, -0.0002100039f),
            new Vector3(0.09375f, -0.0623f, -0.0002100039f),
            new Vector3(-0.09375f, -0.0623f, 0.06269f),
            new Vector3(0.09375f, -0.0623f, 0.06269f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 3, 1,
            3, 0, 2,
            4, 7, 5,
            7, 4, 6,
            8, 11, 9,
            11, 8, 10,
            12, 15, 13,
            15, 12, 14,
            16, 19, 17,
            19, 16, 18,
            20, 23, 21,
            23, 20, 22,
            24, 27, 25,
            27, 24, 26,
            28, 30, 31,
            28, 31, 29,
            36, 32, 33,
            36, 33, 37,
            38, 34, 35,
            38, 35, 39,
            40, 41, 42,
            40, 42, 43,
            44, 45, 46,
            44, 46, 47,
            48, 49, 50,
            48, 50, 51,
            52, 55, 53,
            55, 52, 54,
            56, 59, 57,
            59, 56, 58,
            60, 63, 61,
            63, 60, 62,
            64, 67, 65,
            67, 64, 66,
            68, 71, 69,
            71, 68, 70,
            72, 75, 73,
            75, 72, 74,
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.125f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.125f, 0.3125f),
            new Vector2(0.625f, 0.3125f),
            new Vector2(0.25f, 0.375f),
            new Vector2(0.4375f, 0.375f),
            new Vector2(0.25f, 0.5625f),
            new Vector2(0.4375f, 0.5625f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.125f, 0.5625f),
            new Vector2(0.1875f, 0.5625f),
            new Vector2(0.1875f, 0.5f),
            new Vector2(0.4375f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.4375f, 0.5f),
            new Vector2(0.625f, 0.5f),
            new Vector2(0.4375f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.4375f, 0.5f),
            new Vector2(0.625f, 0.5f),
            new Vector2(0.4375f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.4375f, 0.5f),
            new Vector2(0.625f, 0.5f),
            new Vector2(0.4375f, 0.125f),
            new Vector2(0.625f, 0.125f),
            new Vector2(0.4375f, 0.5f),
            new Vector2(0.625f, 0.5f),
            new Vector2(0.3125f, 0.5625f),
            new Vector2(0.5f, 0.5625f),
            new Vector2(0.3125f, 0.625f),
            new Vector2(0.5f, 0.625f),
            new Vector2(0.3125f, 0.5625f),
            new Vector2(0.5f, 0.5625f),
            new Vector2(0.3125f, 0.625f),
            new Vector2(0.5f, 0.625f),
        };

        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.Optimize();

        return chest;
    }

    GameObject CreateWedge()
    {
        GameObject wedge = new GameObject("Wedge");

        wedge.name = "Wedge";

        MeshFilter meshFilter = wedge.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = wedge.AddComponent<MeshRenderer>();

        if (meshFilter.mesh == null)
        {
            meshFilter.mesh = new Mesh();
        }

        meshFilter.mesh.Clear();

        meshFilter.mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),

            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),

            //new Vector3(-0.5f, -0.5f, -0.5f),
        };

        meshFilter.mesh.triangles = new int[]
        {
            0, 2, 1,
            5, 3, 4,
            1, 2, 4,
            4, 3, 1
        };

        meshFilter.mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),

            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.Optimize();

        return wedge;
    }

    // unfortuantly the game engine never saves this information after loading the combat terrain in function C_7C65()
    // the code is not re-entrant so I cannot just expose and call the function directly so I need to re-implement the 
    // logic here so I can on the fly determine the combat terrain to display by exposing the interal variables used in the
    // original function.

    // TODO: this does not handle the INN or shop or camp case  (e.g. In the middle of the night, while out for a stroll...)
    public U4_Decompiled.COMBAT_TERRAIN Convert_Tile_to_Combat_Terrian(U4_Decompiled.TILE tile)
    {
        U4_Decompiled.COMBAT_TERRAIN combat_terrain;


        if (u4.Party._tile <= U4_Decompiled.TILE.SHIP_SOUTH || (U4_Decompiled.TILE)((byte)u4.current_tile & ~3) == U4_Decompiled.TILE.SHIP_WEST)
        {
            if (u4.D_96F8 == U4_Decompiled.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled.COMBAT_TERRAIN.SHIPSHIP;
            }
            else if (u4.D_946C <= U4_Decompiled.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled.COMBAT_TERRAIN.SHIPSEA;
            }
            else
            {
                combat_terrain = U4_Decompiled.COMBAT_TERRAIN.SHIPSHOR;
            }
        }
        else
        {
            if (u4.D_96F8 == U4_Decompiled.TILE.PIRATE_WEST)
            {
                combat_terrain = U4_Decompiled.COMBAT_TERRAIN.SHORSHIP;
            }
            else if (u4.D_946C <= U4_Decompiled.TILE.SHALLOW_WATER)
            {
                combat_terrain = U4_Decompiled.COMBAT_TERRAIN.SHORE;
            }
            else
            {
                switch (tile)
                {
                    case U4_Decompiled.TILE.SWAMP:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.MARSH;
                            break;
                        }
                    case U4_Decompiled.TILE.BRUSH:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.BRUSH;
                            break;
                        }
                    case U4_Decompiled.TILE.FOREST:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.FOREST;
                            break;
                        }
                    case U4_Decompiled.TILE.HILLS:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.HILL;
                            break;
                        }
                    case U4_Decompiled.TILE.DUNGEON:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.DUNGEON;
                            break;
                        }
                    case U4_Decompiled.TILE.BRICK_FLOOR:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.BRICK;
                            break;
                        }
                    case U4_Decompiled.TILE.BRIDGE:
                    case U4_Decompiled.TILE.BRIDGE_TOP:
                    case U4_Decompiled.TILE.BRIDGE_BOTTOM:
                    case U4_Decompiled.TILE.WOOD_FLOOR:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.BRIDGE;
                            break;
                        }
                    default:
                        {
                            combat_terrain = U4_Decompiled.COMBAT_TERRAIN.GRASS;
                            break;
                        }
                }
            }
        }

        return combat_terrain;
    }

    enum EGA_COLOR
    {
        BLACK = 0,
        BLUE = 1,
        GREEN = 2,
        CYAN = 3,
        RED = 4,
        MEGENTA = 5,
        BROWN = 6,
        LIGHT_GRAY = 7,
        DARK_GRAY = 8,
        BRIGHT_BLUE = 9,
        BRIGHT_GREEN = 10,
        BRIGHT_CYAN = 11,
        BRIGHT_RED = 12,
        BRIGHT_MEGENTA = 13,
        BRIGHT_YELLOW = 14,
        WHITE = 15,
        MAX = 16
    };

    void InitializeEGAPalette()
    {
        // create a EGA color palette
        EGAColorPalette = new Color[(int)EGA_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out EGAColorPalette[(int)EGA_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#0000AA", out EGAColorPalette[(int)EGA_COLOR.BLUE]);
        ColorUtility.TryParseHtmlString("#00AA00", out EGAColorPalette[(int)EGA_COLOR.GREEN]);
        ColorUtility.TryParseHtmlString("#00AAAA", out EGAColorPalette[(int)EGA_COLOR.CYAN]);
        ColorUtility.TryParseHtmlString("#AA0000", out EGAColorPalette[(int)EGA_COLOR.RED]);
        ColorUtility.TryParseHtmlString("#AA00AA", out EGAColorPalette[(int)EGA_COLOR.MEGENTA]);
        ColorUtility.TryParseHtmlString("#AA5500", out EGAColorPalette[(int)EGA_COLOR.BROWN]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out EGAColorPalette[(int)EGA_COLOR.LIGHT_GRAY]);
        ColorUtility.TryParseHtmlString("#555555", out EGAColorPalette[(int)EGA_COLOR.DARK_GRAY]);
        ColorUtility.TryParseHtmlString("#5555FF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_BLUE]);
        ColorUtility.TryParseHtmlString("#55FF55", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_GREEN]);
        ColorUtility.TryParseHtmlString("#55FFFF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_CYAN]);
        ColorUtility.TryParseHtmlString("#FF5555", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_RED]);
        ColorUtility.TryParseHtmlString("#FF55FF", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_MEGENTA]);
        ColorUtility.TryParseHtmlString("#FFFF55", out EGAColorPalette[(int)EGA_COLOR.BRIGHT_YELLOW]);
        ColorUtility.TryParseHtmlString("#FFFFFF", out EGAColorPalette[(int)EGA_COLOR.WHITE]);
    }

    enum CGA_COLOR
    {
        BLACK = 0,
        CYAN = 1,
        MEGENTA = 2,
        WHITE = 3,
        MAX = 4
    };

    void InitializeCGAPalette()
    {
        // create CGA color palette
        CGAColorPalette = new Color[(int)CGA_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out CGAColorPalette[(int)CGA_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#00AAAA", out CGAColorPalette[(int)CGA_COLOR.CYAN]);
        ColorUtility.TryParseHtmlString("#AA00AA", out CGAColorPalette[(int)CGA_COLOR.MEGENTA]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out CGAColorPalette[(int)CGA_COLOR.WHITE]);
    }

    enum APPLE2_COLOR
    {
        BLACK = 0,
        GREEN = 1,
        PURPLE = 2,
        BLUE = 3,
        ORANGE = 4,
        GREENWHITE = 5,
        PURPLEWHITE = 6,
        BLUEWHITE = 7,
        ORANGEWHITE = 8,
        GREENBLACK = 9,
        PURPLEBLACK = 10,
        BLUEBLACK = 11,
        ORANGEBLACK = 12,
        WHITE = 13,
        MAX = 14
    };

    public Color[] Apple2ColorPalette;

    void InitializeApple2Palette()
    {
        // create CGA color palette
        Apple2ColorPalette = new Color[(int)APPLE2_COLOR.MAX];
        ColorUtility.TryParseHtmlString("#000000", out Apple2ColorPalette[(int)APPLE2_COLOR.BLACK]);
        ColorUtility.TryParseHtmlString("#38CB00", out Apple2ColorPalette[(int)APPLE2_COLOR.GREEN]);
        ColorUtility.TryParseHtmlString("#C734FF", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLE]);
        ColorUtility.TryParseHtmlString("#0DA1FF", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUE]);
        ColorUtility.TryParseHtmlString("#F25E00", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGE]);

        ColorUtility.TryParseHtmlString("#9ACB88", out Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE]);
        ColorUtility.TryParseHtmlString("#E8ABFF", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE]);
        ColorUtility.TryParseHtmlString("#ABDFFF", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE]);
        ColorUtility.TryParseHtmlString("#F2C1A2", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE]);

        ColorUtility.TryParseHtmlString("#124000", out Apple2ColorPalette[(int)APPLE2_COLOR.GREENBLACK]);
        ColorUtility.TryParseHtmlString("#320D40", out Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEBLACK]);
        ColorUtility.TryParseHtmlString("#042940", out Apple2ColorPalette[(int)APPLE2_COLOR.BLUEBLACK]);
        ColorUtility.TryParseHtmlString("#401900", out Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEBLACK]);

        ColorUtility.TryParseHtmlString("#FFFFFF", out Apple2ColorPalette[(int)APPLE2_COLOR.WHITE]);
    }

    Color Apple2ColorOdd(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!previousPixel && !pixel && !nextPixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && !nextPixel) //100
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (!previousPixel && pixel && !nextPixel) //010
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLE];
            }
        }
        else if (previousPixel && pixel && !nextPixel) //110
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE];
            }
        }
        else if (!previousPixel && !pixel && nextPixel) //001
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && nextPixel) //101
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEBLACK];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENBLACK];
            }
        }
        else if (!previousPixel && pixel && nextPixel) //011
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEWHITE];
            }
        }
        else if (previousPixel && pixel && nextPixel) //111
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }
        else
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }

        return color;
    }

    Color Apple2ColorEven(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!previousPixel && !pixel && !nextPixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && !nextPixel) //100
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (!previousPixel && pixel && !nextPixel) //010
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREEN];
            }
        }
        else if (previousPixel && pixel && !nextPixel) //110
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE];
            }
        }
        else if (!previousPixel && !pixel && nextPixel) //001
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else if (previousPixel && !pixel && nextPixel) //101
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.BLUEBLACK];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.PURPLEBLACK];
            }
        }
        else if (!previousPixel && pixel && nextPixel) //011
        {
            if (highBitSet)
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.ORANGEWHITE];
            }
            else
            {
                color = Apple2ColorPalette[(int)APPLE2_COLOR.GREENWHITE];
            }
        }
        else if (previousPixel && pixel && nextPixel)
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }
        else
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }

        return color;
    }

    Color Apple2ColorBW(bool highBitSet, bool previousPixel, bool pixel, bool nextPixel)
    {
        Color color;

        if (!pixel) //000
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.BLACK];
        }
        else //111
        {
            color = Apple2ColorPalette[(int)APPLE2_COLOR.WHITE];
        }

        return color;
    }

    public Texture2D PNGAtlas;
    public string PNGFilepath;

    void LoadTilesPNG()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + PNGFilepath))
        {
            Debug.Log("Could not find PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + PNGFilepath);

        // allocate something to start with so the loadImage can resize it to file the actual file
        PNGAtlas = new Texture2D(32, 32*256, TextureFormat.RGBA32, false);

        if (!PNGAtlas.LoadImage(fileData))
        {
            Debug.Log("Could not load PNG tiles atlas file " + Application.persistentDataPath + PNGFilepath);
            return;
        }

        originalTileWidth = PNGAtlas.width;
        originalTileHeight = PNGAtlas.height / 256;
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(originalTileWidth, originalTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            int y = (255-tile) * originalTileHeight;

            currentTile.SetPixels(0, 0, originalTileWidth, originalTileHeight, PNGAtlas.GetPixels(0, y, originalTileWidth, originalTileHeight));
            currentTile.Apply();
        }
    }

    void LoadTilesApple2()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath1))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath1);
            return;
        }

        // read the file
        byte[] fileData1 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath1);

        if (fileData1.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData1.Length);
            return;
        }

        if (!System.IO.File.Exists(Application.persistentDataPath + tileApple2Filepath2))
        {
            Debug.Log("Could not find Apple2 tiles file " + Application.persistentDataPath + tileApple2Filepath2);
            return;
        }

        // read the file
        byte[] fileData2 = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileApple2Filepath2);

        if (fileData2.Length != 4 * 1024)
        {
            Debug.Log("Apple2 Tiles file incorrect length " + fileData2.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(14, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the Apple2 color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width - 2; /* width incremented below */ )
                {
                    bool highBitSet; 
                    bool previousPixel;
                    bool pixel;
                    bool nextPixel;
                    Color color;

                    int y = currentTile.height - height - 1;

                    // go to the next byte in the file
                    int index = height * 256 + tile;

                    // get the first file pixel block
                    int pixelBlock = fileData1[index];

                    // go through the first half of the tile from the first file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    // assume no tiling on the side of the tile
                    previousPixel = false;
                    pixel = (pixelBlock & 0x01) != 0;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // next pixel is in the other file
                    pixelBlock = fileData2[index];
                    nextPixel = (pixelBlock & 0x01) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);

                    // do the second half of the tile from the other file
                    highBitSet = (pixelBlock & 0x80) != 0;

                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x02) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x04) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x08) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x10) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x20) != 0;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    nextPixel = (pixelBlock & 0x40) != 0;
                    color = Apple2ColorEven(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                    previousPixel = pixel;
                    pixel = nextPixel;
                    // assume no tiling on the side of the tile
                    nextPixel = false;
                    color = Apple2ColorOdd(highBitSet, previousPixel, pixel, nextPixel);
                    currentTile.SetPixel(width++, y, color);
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    void LoadTilesEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + tileEGAFilepath))
        {
            Debug.Log("Could not find EGA tiles file " + Application.persistentDataPath + tileEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileEGAFilepath);

        if (fileData.Length != 32 * 1024)
        {
            Debug.Log("EGA tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[index] >> 4;
                    Color color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures/ladders/anhk and use black as alpha channel 61
                    if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.ANKH ||
                        tile == (int)U4_Decompiled.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled.TILE.FOREST ||
                        tile == (int)U4_Decompiled.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled.TILE.SHRINE ||
                        tile == (int)U4_Decompiled.TILE.ALTAR ||
                        tile == (int)U4_Decompiled.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled.TILE.CASTLE ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled.TILE.BRUSH ||
                        tile == (int)U4_Decompiled.TILE.NIXIE ||
                        tile == (int)U4_Decompiled.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled.TILE.PARTY && tile <= (int)U4_Decompiled.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled.TILE.GUARD && tile <= (int)U4_Decompiled.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled.TILE.SERPENT && tile <= (int)U4_Decompiled.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled.TILE.BAT && tile <= (int)U4_Decompiled.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled.TILE.INSECTS && tile <= (int)U4_Decompiled.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled.TILE.PHANTOM && tile <= (int)U4_Decompiled.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled.TILE.HYDRA && tile <= (int)U4_Decompiled.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)EGA_COLOR.BROWN) && tile == (int)U4_Decompiled.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the grass speckels don't show when we use the tile as a billboard
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled.TILE.TOWN ||
                        tile == (int)U4_Decompiled.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled.TILE.MOONGATE1 && tile <= (int)U4_Decompiled.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent
                    else if (((colorIndex == (int)EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled.TILE.MOONGATE1 && tile <= (int)U4_Decompiled.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these squid tiles and make black and blue into alpha
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled.TILE.SQUID || tile == (int)U4_Decompiled.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // ships
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled.TILE.SHIP_WEST && tile <= (int)U4_Decompiled.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make energy fields are transparent
                    else if (tile >= (int)U4_Decompiled.TILE.POISON_FIELD && tile <= (int)U4_Decompiled.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else just copy all the pixels with no modifications for now
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[index] & 0xf;
                    color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    // check if these are people/creatures/ladders/anhk and use black as alpha channel
                    if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.ANKH ||
                        tile == (int)U4_Decompiled.TILE.LADDER_UP ||
                        tile == (int)U4_Decompiled.TILE.LADDER_DOWN ||
                        tile == (int)U4_Decompiled.TILE.FOREST ||
                        tile == (int)U4_Decompiled.TILE.COOKING_FIRE ||
                        tile == (int)U4_Decompiled.TILE.SHRINE ||
                        tile == (int)U4_Decompiled.TILE.ALTAR ||
                        tile == (int)U4_Decompiled.TILE.BALOON ||
                        //tile == (int)U4_Decompiled.TILE.CHEST ||
                        tile == (int)U4_Decompiled.TILE.CASTLE ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_LEFT ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_ENTRANCE ||
                        tile == (int)U4_Decompiled.TILE.CASTLE_RIGHT ||
                        tile == (int)U4_Decompiled.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE_BOTTOM ||
                        tile == (int)U4_Decompiled.TILE.BRIDGE_TOP ||
                        tile == (int)U4_Decompiled.TILE.BRUSH ||
                        tile == (int)U4_Decompiled.TILE.NIXIE ||
                        tile == (int)U4_Decompiled.TILE.NIXIE2 ||
                        (tile >= (int)U4_Decompiled.TILE.MISSLE_ATTACK_SMALL && tile <= (int)U4_Decompiled.TILE.MISSLE_ATTACK_RED) ||
                        (tile >= (int)U4_Decompiled.TILE.PARTY && tile <= (int)U4_Decompiled.TILE.SHEPHERD2) ||
                        (tile >= (int)U4_Decompiled.TILE.GUARD && tile <= (int)U4_Decompiled.TILE.LORD_BRITISH2) ||
                        (tile >= (int)U4_Decompiled.TILE.SERPENT && tile <= (int)U4_Decompiled.TILE.WATER_SPOUT2) ||
                        (tile >= (int)U4_Decompiled.TILE.BAT && tile <= (int)U4_Decompiled.TILE.TROLL4) ||
                        (tile >= (int)U4_Decompiled.TILE.INSECTS && tile <= (int)U4_Decompiled.TILE.INSECTS4) ||
                        (tile >= (int)U4_Decompiled.TILE.PHANTOM && tile <= (int)U4_Decompiled.TILE.MAGE_NPC4) ||
                        (tile >= (int)U4_Decompiled.TILE.LAVA_LIZARD && tile <= (int)U4_Decompiled.TILE.ZORN4) ||
                        (tile >= (int)U4_Decompiled.TILE.HYDRA && tile <= (int)U4_Decompiled.TILE.BALRON4)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove the brown line that overlaps the bridge support
                    else if ((colorIndex == (int)EGA_COLOR.BROWN) && tile == (int)U4_Decompiled.TILE.BRIDGE_BOTTOM && height == 9)
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // others where we need to make green an alpha channel also like towns/ruins/villages
                    // so the green grass speckels don't show when we use the tile standing upright
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.GREEN)) &&
                        (tile == (int)U4_Decompiled.TILE.VILLAGE ||
                        tile == (int)U4_Decompiled.TILE.TOWN ||
                        tile == (int)U4_Decompiled.TILE.RUINS))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // remove green grass specals from moongates 
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.GREEN)) &&
                        (tile >= (int)U4_Decompiled.TILE.MOONGATE1 && tile <= (int)U4_Decompiled.TILE.MOONGATE4))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // make the moongates blue and yellow transparent TODO make black inside portal transparent also
                    else if (((colorIndex == (int)EGA_COLOR.BRIGHT_YELLOW) || (colorIndex == (int)EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile >= (int)U4_Decompiled.TILE.MOONGATE1 && tile <= (int)U4_Decompiled.TILE.MOONGATE4))
                    {
                        color.a = 0.75f;
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }
                    // remove blue water from these tiles and make black and blue into alpha
                    else if (((colorIndex == (int)EGA_COLOR.BLACK) || (colorIndex == (int)EGA_COLOR.BRIGHT_BLUE)) &&
                        (tile == (int)U4_Decompiled.TILE.SQUID || tile == (int)U4_Decompiled.TILE.SQUID2))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.HORSE_EAST))
                    {
                        if ((width == 13) && (height == 4))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // special case the horse black eyes
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        (tile == (int)U4_Decompiled.TILE.HORSE_WEST))
                    {
                        if ((width == 3) && (height == 5))
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                        else
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                    }
                    // ships
                    else if ((colorIndex == (int)EGA_COLOR.BLACK) &&
                        ((tile >= (int)U4_Decompiled.TILE.SHIP_WEST && tile <= (int)U4_Decompiled.TILE.SHIP_SOUTH) ||
                        (tile >= (int)U4_Decompiled.TILE.PIRATE_WEST && tile <= (int)U4_Decompiled.TILE.PIRATE_SOUTH)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= (int)U4_Decompiled.TILE.POISON_FIELD && tile <= (int)U4_Decompiled.TILE.SLEEP_FIELD)
                    {
                        if (colorIndex == (int)EGA_COLOR.BLACK)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else has no alpha channel defined yet
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();
        }
    }

    void LoadTilesCGA()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + tileCGAFilepath))
        {
            Debug.Log("Could not find CGA tiles file " + Application.persistentDataPath + tileCGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileCGAFilepath);

        if (fileData.Length != 16 * 1024)
        {
            Debug.Log("CGA Tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        originalTiles = new Texture2D[256];

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {
            // create a texture for this tile
            Texture2D currentTile = new Texture2D(16, 16, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            currentTile.filterMode = FilterMode.Point;

            // assign this texture to the tile array index
            originalTiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height += 2)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    int colorIndex;

                    colorIndex = (fileData[index + 0x20] & 0xC0) >> 6;
                    Color color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0xC0) >> 6;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x30) >> 4;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x0C) >> 2;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index + 0x00] & 0x03) >> 0;
                    color = CGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    // go to the next byte in the file
                    index++;
                }
            }

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();

            // skip ahead
            index += 0x20;
        }
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
    void LoadCombatMap(string combatMapFilepath, 
        ref U4_Decompiled.TILE[,] combatMap, 
        ref CombatMonsterStartPositions [] monsterStartPositions, 
        ref CombatPartyStartPositions [] partyStartPositions)
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
                combatMap[x, y] = (U4_Decompiled.TILE)combatMapFileData[fileIndex++];
            }
        }
    }

    void CreateCombatTerrains()
    { 
        // create a game object to store the combat terrain game objects, this should be at the top with no parent same as the world
        GameObject combatTerrainsObject = new GameObject();
        combatTerrainsObject.name = "Combat Terrains";

        CombatTerrains = new GameObject[(int)U4_Decompiled.COMBAT_TERRAIN.MAX];

        // go through all the combat terrains and load their maps and create a game object to hold them
        // as a child of the above combat terrains game object
        for (int i = 0; i<(int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
        {
            // allocate space for the individual map
            combatMaps[i] = new U4_Decompiled.TILE[11, 11];
            // allocate space for the monster and party starting positions
            combatMonsterStartPositions[i] = new CombatMonsterStartPositions[16];
            combatPartyStartPositions[i] = new CombatPartyStartPositions[8];

            // load the combat map from the original files
            LoadCombatMap("/u4/" + ((U4_Decompiled.COMBAT_TERRAIN) i).ToString() + ".CON",
                ref combatMaps[i],
                ref combatMonsterStartPositions[i],
                ref combatPartyStartPositions[i]);

            // create a game object to hold it and set it as a child of the combat terrains game object
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(combatTerrainsObject.transform);

            // set it's name to match the combat terrain being created
            gameObject.name = ((U4_Decompiled.COMBAT_TERRAIN) i).ToString();

            // create the combat terrain based on the loaded map
            CreateMap(gameObject, combatMaps[i]);

            // Disable it initially
            gameObject.SetActive(false);

            // Position the combat map in place
            gameObject.transform.position = new Vector3(0, 0, entireMapTILEs.GetLength(1) - combatMaps[i].GetLength(1)); ;

            // rotate map into place
            gameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            // save the game object in the array
            CombatTerrains[i] = gameObject;
        }
    }

    [SerializeField]
    U4_Decompiled.TILE[,] entireMapTILEs = new U4_Decompiled.TILE[32 * 8, 32 * 8];

    [SerializeField]
    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];
    GameObject[][,] settlementsMapGameObjects = new GameObject[(int)SETTLEMENT.MAX][,];

    void LoadWorldMap()
    {
        /*
        This is the map of Britannia. It is 256x256 tiles in total and broken up into 64 32x32 chunks; 
        the total file is 65,536 bytes long. The first chunk is in the top left corner; 
        the next is just to the right of it, and so on. The last chunk is in the bottom right corner. 
        Each tile is stored as a byte that maps to a tile in SHAPES.EGA.The chunks are stored in the same way as the overall map: 
        left to right and top to bottom.

        The "chunked" layout is an artifact of the limited memory on the original machines that ran Ultima IV. 
        The whole map would take 64kb, too much for a C64 or an Apple II, so the game would keep a limited number of 1k chunks in memory 
        at a time.As the player moved around, old chunks were thrown out as new ones were swapped in.
        Offset  Length(in bytes)   Notes
        0x0     1024    32x32 map matrix for chunk 0
        0x400   1024    32x32 map matrix for chunk 1... 	... 	...
        0xFC00  1024    32x32 map matrix for chunk 63
        */

        if (!System.IO.File.Exists(Application.persistentDataPath + worldMapFilepath))
        {
            Debug.Log("Could not find world map file " + Application.persistentDataPath + worldMapFilepath);
            return;
        }

        // read the file
        byte[] worldMapFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + worldMapFilepath);

        if (worldMapFileData.Length != 32 * 32 * 64)
        {
            Debug.Log("World map file incorrect length " + worldMapFileData.Length);
            return;
        }

        int fileIndex = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int height = 0; height < 32; height++)
                {
                    for (int width = 0; width < 32; width++)
                    {
                        entireMapTILEs[x * 32 + width, y * 32 + height] = (U4_Decompiled.TILE)worldMapFileData[fileIndex++];
                    }
                }
            }
        }
    }

    // These are different than the map tiles
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

    [SerializeField]
    public struct FLOOR_TRIGGER
    {
        public U4_Decompiled.TILE changeTile;
        public int trigger_x, trigger_y;
        public int changeTile_x1, changeTile_y1;
        public int changeTile_x2, changeTile_y2;
    }

    [SerializeField]
    public struct DUNGEON_MONSTER
    {
        public U4_Decompiled.TILE monster;
        public int x, y;
    }

    [SerializeField]
    public struct DUNGEON_PARTY_START_LOCATION
    {
        public int x, y;
    }

    [SerializeField]
    public struct DUNGEON_ROOM
    {
        public FLOOR_TRIGGER[] triggers; // (4 bytes each X 4 triggers possible)
        public DUNGEON_MONSTER[] monsters; // 16 of them, (0 means no monster and 0's come FIRST)
        public DUNGEON_PARTY_START_LOCATION[] partyNorthEntry; // 0-7 (north entry)
        public DUNGEON_PARTY_START_LOCATION[] partyEastEntry; // 0-7 (east entry)
        public DUNGEON_PARTY_START_LOCATION[] partySouthEntry; // 0-7 (south entry)
        public DUNGEON_PARTY_START_LOCATION[] partyWestEntry; // 0-7 (west entry)
        public U4_Decompiled.TILE[,] dungeonRoomMap; // 11x11 map matrix for room
    }

    [SerializeField]
    public struct DUNGEON
    {
        public string name;
        public DUNGEON_TILE[][,] dungeonTILEs; // 8x8x8 map
        public DUNGEON_ROOM[] dungeonRooms; // 16 or 64 rooms
    }

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
    // only the upper nibble defines the dungeon tile, the lower nibble is used for active dungeon monsters
    public U4_Decompiled.COMBAT_TERRAIN[] convertDungeonTileToCombat  =
    { 
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // HALLWAY -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG1, // LADDER_UP -> ladder up
        U4_Decompiled.COMBAT_TERRAIN.DNG2, // LADDER_DOWN -> ladder down
        U4_Decompiled.COMBAT_TERRAIN.DNG3, // LADDER_UP_AND_DOWN -> ladder up and down
        U4_Decompiled.COMBAT_TERRAIN.DNG4, // TREASURE_CHEST -> chest
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // CEILING_HOLE -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // FLOOR_HOLE -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // MAGIC_ORB -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // TRAP -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // FOUNTAIN -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // FIELD -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // ALTAR -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG5, // DOOR -> doorway 
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // DUNGEON_ROOM -> hallway
        U4_Decompiled.COMBAT_TERRAIN.DNG0, // DOOR_SECRECT -> secret doorway  /* DNG6 make secret door just regular hallway as we do the secret door/wall on the ajacent room/hallway */
        U4_Decompiled.COMBAT_TERRAIN.DNG0  // WALL -> hallway, just in case
    };

    public enum NPC_MOVEMENT_MODE
    {
        FIXED = 0x00,
        WANDER = 0x01,
        FOLLOW = 0x80,
        ATTACK = 0xff
    };

    public enum NPC_STRING_INDEX
    {
        NAME = 0,
        PRONOUN = 1, //  (He, She or It)
        LOOK_DESCRIPTION = 2,
        JOB_RESPONSE = 3,
        HEALTH_RESPONSE = 4,
        KEYWORD1_RESPONSE = 5,
        KEYWORD2_RESPONSE = 6,
        QUESTION = 7,
        QUESTION_YES_RESPONSE = 8,
        QUESTION_NO_RESPONSE = 9,
        KEYWORD1 = 10,
        KEYWORD2 = 11,
        MAX = 12
    };

    // these need to line up with U4_Decompiled.LOCATIONS so we can convert from Party._loc to this enum and the LOCATIONS enum, LBC_1 is a special case
    public enum SETTLEMENT
    {
        // Castles
        LCB_1 = 0,
        LCB_2 = 1,
        LYCAEUM = 2,
        EMPATH = 3,
        SERPENT = 4,

        // Townes
        MOONGLOW = 5,
        BRITAIN = 6,
        JHELOM = 7,
        YEW = 8,
        MINOC = 9,
        TRINSIC = 10,
        SKARA = 11,
        MAGINCIA = 12,

        // Villages
        PAWS = 13,
        DEN = 14,
        VESPER = 15,
        COVE = 16,
        MAX = 17
    }

    public struct npc
    {
        public U4_Decompiled.TILE tile;
        public byte pos_x;
        public byte pos_y;
        public NPC_MOVEMENT_MODE movement;
        public int conversationIndex;
        public List<string> strings;
        public int probabilityOfTurningAway;
        public bool questionAffectHumility;
        public int questionTriggerIndex;
    };

    public npc[][] settlementNPCs = new npc[(int)SETTLEMENT.MAX][]; //32
    public int[][] npcQuestionTriggerIndex = new int[(int)SETTLEMENT.MAX][]; //16
    public bool[][] npcQuestionAffectHumility = new bool[(int)SETTLEMENT.MAX][]; //16
    public int[][] npcProbabilityOfTurningAway = new int[(int)SETTLEMENT.MAX][]; //16
    public List<string>[][] npcStrings = new List<string>[(int)SETTLEMENT.MAX][]; //16
    public U4_Decompiled.TILE[][,] settlementMap = new U4_Decompiled.TILE[(int)SETTLEMENT.MAX][,]; //32,32

    void LoadSettlements()
    {
        /* 
           Offset 	Length (in bytes) 	Purpose
           0x0 	    1024 	32x32 town map matrix
           0x400 	32 	    Tile for NPCs 0-31
           0x420 	32 	    Start_x for NPCs 0-31
           0x440 	32 	    Start_y for NPCs 0-31
           0x460 	32 	    Repetition of 0x400-0x41F
           0x480 	32 	    Repetition of 0x420-0x43F
           0x4A0 	32 	    Repetition of 0x440-0x45F
           0x4C0 	32 	    Movement_behavior for NPCs 0-31 (0x0-fixed, 0x1-wander, 0x80-follow, 0xFF-attack)
           0x4E0 	32 	    Conversion index (tlk file) for NPCs 0-31 
        */

        for (int settlement = 0; settlement < (int)SETTLEMENT.MAX; settlement++)
        {
            settlementNPCs[settlement] = new npc[32];
            npcQuestionTriggerIndex[settlement] = new int[16];
            npcQuestionAffectHumility[settlement] = new bool[16];
            npcProbabilityOfTurningAway[settlement] = new int[16];
            npcStrings[settlement] = new List<string>[16];
            settlementMap[settlement] = new U4_Decompiled.TILE[32,32];

            if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT"))
            {
                Debug.Log("Could not find settlement file " + Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");
                continue;
            }

            // read the file
            byte[] settlementFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + ((SETTLEMENT)settlement).ToString() + ".ULT");

            if (settlementFileData.Length != 1280)
            {
                Debug.Log("Settlement file incorrect length " + settlementFileData.Length);
                continue;
            }

            /*
                Offset 	Length (in bytes) 	Purpose
                0x0 	1 	Question Flag (3=JOB, 4=HEALTH, 5=KEYWORD1, 6=KEYWORD2)
                0x1 	1 	Does Response Affect Humility? (0=No, 1=Yes)
                0x2 	1 	Probability of Turning Away (out of 256)
                0x3 	Varies 	Name
                Varies 	Varies 	Pronoun (He, She or It)
                Varies 	Varies 	LOOK Description
                Varies 	Varies 	JOB Response
                Varies 	Varies 	HEALTH Response
                Varies 	Varies 	KEYWORD 1 Response
                Varies 	Varies 	KEYWORD 2 Response
                Varies 	Varies 	Yes/No Question
                Varies 	Varies 	YES Response
                Varies 	Varies 	NO Response
                Varies 	Varies 	KEYWORD 1
                Varies 	Varies 	KEYWORD 2
                Varies-0x119 	Varies 	00000....  
            */

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
                    continue;
                }
            }
            else
            {
                if (!System.IO.File.Exists(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK"))
                {
                    Debug.Log("Could not find settlement talk file " + Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
                    continue;
                }
            }

            byte[] talkFileData;

            if (settlement == (int)SETTLEMENT.LCB_1 || settlement == (int)SETTLEMENT.LCB_2)
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + "LCB" + ".TLK");
            }
            else
            {
                // read the file
                talkFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/u4/" + (SETTLEMENT)settlement + ".TLK");
            }

            if (talkFileData.Length != 4608)
            {
                Debug.Log("Settlement talk file incorrect length " + talkFileData.Length);
                continue;
            }

            for (int talkIndex = 0; talkIndex < 16; talkIndex++)
            {
                npcStrings[settlement][talkIndex] = new List<string>();

                npcQuestionTriggerIndex[settlement][talkIndex] = talkFileData[talkIndex * 288];
                if (talkFileData[(talkIndex * 288) + 1] != 0)
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = true;
                }
                else
                {
                    npcQuestionAffectHumility[settlement][talkIndex] = false;
                }
                npcProbabilityOfTurningAway[settlement][talkIndex] = talkFileData[talkIndex * 288 + 2];

                string s;
                int stringBufferIndex = 3;

                // search for strings in the .TLK file
                for (int stringIndex = 0; stringIndex < (int)NPC_STRING_INDEX.MAX; stringIndex++)
                {
                    // reset string
                    s = "";

                    // manually construct the string because C# doesn't work with null terminated C strings well
                    for (int i = 0; (i < 100) && (talkFileData[talkIndex * 288 + stringBufferIndex] != 0); i++)
                    {
                        s += (char)talkFileData[talkIndex * 288 + stringBufferIndex++];
                    }

                    // add it to the list even if it is empty
                    npcStrings[settlement][talkIndex].Add(s);

                    // skip over null terminator to go to the next string
                    stringBufferIndex++;
                }
            }

            // load settlement map data
            int bufferIndex = 0;

            for (int height = 0; height < 32; height++)
            {
                for (int width = 0; width < 32; width++)
                {
                    U4_Decompiled.TILE tileIndex = (U4_Decompiled.TILE)settlementFileData[bufferIndex++];
                    settlementMap[settlement][width, height] = tileIndex;
                }
            }

            // load npc data from the map data
            for (int npcIndex = 0; npcIndex < 32; npcIndex++)
            {
                U4_Decompiled.TILE npcTile = (U4_Decompiled.TILE)settlementFileData[0x400 + npcIndex];
                settlementNPCs[settlement][npcIndex].tile = npcTile;

                // zero indicates unused
                if (npcTile != 0)
                {
                    settlementNPCs[settlement][npcIndex].pos_x = settlementFileData[0x420 + npcIndex];
                    settlementNPCs[settlement][npcIndex].pos_y = settlementFileData[0x440 + npcIndex];
                    settlementNPCs[settlement][npcIndex].movement = (NPC_MOVEMENT_MODE)settlementFileData[0x4C0 + npcIndex];
                    int conversationIndex = settlementFileData[0x4E0 + npcIndex];
                    settlementNPCs[settlement][npcIndex].conversationIndex = conversationIndex;
                    // grab the talk data and add it to this structure
                    // zero indicates unused
                    if (conversationIndex != 0)
                    {
                        // this can be 128 for one vendor in Vincent, not sure why? TODO
                        if ((conversationIndex - 1) < npcStrings.Length)
                        {
                            settlementNPCs[settlement][npcIndex].strings = npcStrings[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionAffectHumility = npcQuestionAffectHumility[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].probabilityOfTurningAway = npcProbabilityOfTurningAway[settlement][conversationIndex - 1];
                            settlementNPCs[settlement][npcIndex].questionTriggerIndex = npcQuestionTriggerIndex[settlement][conversationIndex - 1];
                        }
                    }
                }
            }
        }
    }

    [SerializeField]
    public DUNGEON[] dungeons = new DUNGEON[(int)DUNGEONS.MAX];
    void LoadDungeons()
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

            if (dungeonFileData.Length != 0x200 + 0x100*rooms)
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
                dungeons[index].dungeonRooms[room].monsters = new DUNGEON_MONSTER[16];
                dungeons[index].dungeonRooms[room].partyNorthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyEastEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partySouthEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].partyWestEntry = new DUNGEON_PARTY_START_LOCATION[8];
                dungeons[index].dungeonRooms[room].dungeonRoomMap = new U4_Decompiled.TILE[11, 11];

                // get the triggers
                for (int i = 0; i < 4; i++)
                {
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile = (U4_Decompiled.TILE)dungeonFileData[fileIndex++];
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_x = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].trigger_y = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x1 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y1 = dungeonFileData[fileIndex++] & 0xf;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_x2 = dungeonFileData[fileIndex] >> 4;
                    dungeons[index].dungeonRooms[room].triggers[i].changeTile_y2 = dungeonFileData[fileIndex++] & 0xf;
                }

                // get the monsters
                for (int i = 0; i < 16; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].monster = (U4_Decompiled.TILE)dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 16; i++)
                {
                    dungeons[index].dungeonRooms[room].monsters[i].x = dungeonFileData[fileIndex++];
                }
                for (int i = 0; i < 16; i++)
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
                        dungeons[index].dungeonRooms[room].dungeonRoomMap[x, y] = (U4_Decompiled.TILE)dungeonFileData[fileIndex++];
                    }
                }

                fileIndex += 7; // skip over unused buffer
            }
        }
    }
    public void AddMonsters(GameObject dungeonRoomGameObject, ref DUNGEON_ROOM dungeonRoom)
    {
        GameObject monstersGameObject = new GameObject("Monsters");
        monstersGameObject.transform.SetParent(dungeonRoomGameObject.transform);

        // add all the monsters
        for (int i = 0; i < 16; i++)
        {
            U4_Decompiled.TILE  monsterTile = dungeonRoom.monsters[i].monster;

            if (monsterTile != 0)
            {
                GameObject monsterGameObject = CreateQuad();
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
                if ((monsterTile >= U4_Decompiled.TILE.POISON_FIELD) && (monsterTile <= U4_Decompiled.TILE.SLEEP_FIELD))
                {
                    renderer.material.mainTexture = combinedLinearTexture;
                    renderer.material.mainTextureOffset = new Vector2((float)((int)monsterTile * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                    renderer.material.mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);

                    Animate1 animate = monsterGameObject.AddComponent<Animate1>();
                }
                else
                {
                    // add our little animator script and set the tile
                    Animate3 animate = monsterGameObject.AddComponent<Animate3>();
                    animate.npcTile = 0;
                    animate.world = this;
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

                // set this as a parent of the monsters game object
                monsterGameObject.transform.SetParent(monstersGameObject.transform);
            }
        }
    }
    public void AddDungeonMapMonsters()
    {
        // have we finished creating the world
        if (dungeonMonsters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (dungeonMonsters.transform.childCount != 16)
        {
            for (int i = 0; i < 16; i++) // TODO figure out how many can be in the dungeon level
            {
                // a child object for each dungeon monster entry in the table
                GameObject dungeonMonsterGameObject = CreateQuad();

                // get the renderer
                MeshRenderer renderer = dungeonMonsterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile to zero
                Animate3 animate = dungeonMonsterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the fighters game object into position after creating
                Vector3 fightersLocation = new Vector3(0, 255, 0);
                dungeonMonsterGameObject.transform.localPosition = fightersLocation;
                dungeonMonsterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                dungeonMonsterGameObject.transform.SetParent(dungeonMonsters.transform);

                // set as intially disabled
                dungeonMonsterGameObject.SetActive(false);
            }

            // rotate characters into place
            dungeonMonsters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        int monsterIndex = 0;

        // add all the monsters found in the dungeon map
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                // get a dungeonTile in the dungeon map
                DUNGEON_TILE dungeonTile = (DUNGEON_TILE)u4.tMap8x8x8[u4.Party._z, y, x];

                // check upper nibble to see if there is anything to render
                int checkDungeonTile = (int)dungeonTile & 0xf0;

                if (checkDungeonTile == (int)DUNGEON_TILE.TRAP ||
                    checkDungeonTile == (int)DUNGEON_TILE.FOUNTAIN ||
                    checkDungeonTile == (int)DUNGEON_TILE.FIELD ||
                    checkDungeonTile == (int)DUNGEON_TILE.ALTAR || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile == (int)DUNGEON_TILE.MAGIC_ORB || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile >= (int)DUNGEON_TILE.DUNGEON_ROOM)
                {
                    continue;
                }

                // dungeon monster is stored in the low nibble of the dungeon tile
                // TODO DUNGEONTILE does not account for this.
                checkDungeonTile = (int)dungeonTile & 0x0f;

                // zero means no monster at that dungeon location
                if (checkDungeonTile == 0)
                {
                    continue;
                }

                // convert the monster nibble into a map tile
                U4_Decompiled.TILE monsterTile = (U4_Decompiled.TILE)((checkDungeonTile << 2) - 4 + U4_Decompiled.TILE.RAT);

                // did we create enough monster child game objects
                if (monsterIndex < dungeonMonsters.transform.childCount)
                {
                    // get the corresponding monster game object
                    Transform childofmonsters = dungeonMonsters.transform.GetChild(monsterIndex++);

                    // set it active
                    childofmonsters.gameObject.SetActive(true);

                    // update the tile of the game object
                    childofmonsters.GetComponent<Animate3>().SetNPCTile(monsterTile);

                    // update the position
                    childofmonsters.localPosition = new Vector3(x * 11 + 5, (7 - y) * 11 + 5, 0);
                    childofmonsters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                    // make it billboard
                    Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                    look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                    childofmonsters.transform.LookAt(look.transform);
                    Vector3 rot = childofmonsters.transform.eulerAngles;
                    childofmonsters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
                } 
            }
        }

        // set any remaining monsters to not active
        for (; monsterIndex < dungeonMonsters.transform.childCount; monsterIndex++)
        {
            // get the corresponding monster game object
            Transform childofmonsters = dungeonMonsters.transform.GetChild(monsterIndex);
            childofmonsters.gameObject.SetActive(false);
        }
    }
    public GameObject CreateDungeonRoom(ref DUNGEON_ROOM dungeonRoom)
    {
        GameObject mapGameObject = new GameObject();
        CreateMap(mapGameObject, dungeonRoom.dungeonRoomMap, false);
        mapGameObject.transform.position = Vector3.zero;
        mapGameObject.transform.localEulerAngles = Vector3.zero;
        AddMonsters(mapGameObject, ref dungeonRoom);
        return mapGameObject;
    }
    public void CreateDungeonRooms(GameObject dungeonsRoomsObject)
    {
        for (int i = 0; i < (int)DUNGEONS.MAX; i++)
        {
            for (int room = 0; room < dungeons[i].dungeonRooms.Length; room++)
            {
                GameObject dungeonRoomObject = CreateDungeonRoom(ref dungeons[i].dungeonRooms[room]);
                dungeonRoomObject.transform.SetParent(dungeonsRoomsObject.transform);
                dungeonRoomObject.name = ((DUNGEONS)i).ToString() + " room #" + room;
                dungeonRoomObject.transform.localPosition = new Vector3((room % 16) * 11, 0, (i + (room/16)) * 11);
                dungeonRoomObject.transform.eulerAngles = new Vector3(90.0f, 0f, 0f);
            }
        }
    }
    public void CreateDungeons(GameObject dungeonsGameObject)
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
                        U4_Decompiled.TILE tileIndex;
                        DUNGEON_TILE dungeonTile = dungeons[index].dungeonTILEs[z][x, y];

                        if (dungeonTile == DUNGEON_TILE.WALL)
                        {
                            mapTile = CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.HALLWAY)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_UP;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_DOWN)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_DOWN;
                        }
                        else if (dungeonTile == DUNGEON_TILE.LADDER_UP_AND_DOWN)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_DOWN; // TODO: need to overlap the up and down tiles, but this will do for now
                        }
                        else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.CHEST;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.FOUNTAIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_CURE) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_HEALING) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_POISIN) ||
                                (dungeonTile == DUNGEON_TILE.FOUNTAIN_ACID))
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.SHALLOW_WATER;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.ENERGY_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.FIRE_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.POISON_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.SLEEP_FIELD;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.DOOR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.DOOR_SECRECT)
                        {
                            mapTile = CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                        }
                        else if (dungeonTile == DUNGEON_TILE.ALTAR)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.ALTAR;
                        }
                        else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.MISSLE_ATTACK_BLUE;
                        }
                        else if ((dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS) ||
                            (dungeonTile == DUNGEON_TILE.TRAP_PIT))
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                        }
                        else
                        {
                            mapTile = CreateQuad(); 
                            tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                        }

                        mapTile.transform.SetParent(dungeonLevelGameObject.transform);
                        mapTile.transform.localPosition = new Vector3(x, y, 7-z);
                        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout"); 
                        renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                    }
                }
            }

            // rotate dungeon into place
            dungeonGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }
    }

    U4_Decompiled.TILE[,] CreateDungeonHallway(ref DUNGEON_TILE[,] dungeonTileMap, ref DUNGEON_ROOM[] dungeonRooms, int posx, int posy, int posz,
        U4_Decompiled.TILE centerTile = U4_Decompiled.TILE.TILED_FLOOR)
    {
        DUNGEON_TILE left = dungeonTileMap[posx, (posy + 1) % 8]; //  0,-1
        DUNGEON_TILE above = dungeonTileMap[(posx + 8 - 1) % 8, posy]; // -1, 0
        DUNGEON_TILE right = dungeonTileMap[(posx + 1) % 8, posy];     //  1, 0
        DUNGEON_TILE below = dungeonTileMap[posx, (posy + 8 - 1) % 8];     //  0, 1

        DUNGEON_TILE diagonalLeftBelow = dungeonTileMap[(posx + 8 - 1) % 8, (posy + 8 - 1) % 8 ];  //  0,-1 ->  -1, 1
        DUNGEON_TILE diagonalRightBelow = dungeonTileMap[(posx + 1) % 8, (posy + 8 - 1) % 8];  // -1, 0 ->   1, 1
        DUNGEON_TILE diagonalRightAbove = dungeonTileMap[(posx + 1) % 8, (posy + 1) % 8];      //  1, 0 ->   1,-1
        DUNGEON_TILE diagonalLeftAbove = dungeonTileMap[(posx + 8 - 1) % 8, (posy + 1) % 8];   //  0, 1 ->  -1,-1

        U4_Decompiled.TILE tileIndex;

        U4_Decompiled.TILE[,] map = new U4_Decompiled.TILE[11, 11];

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                tileIndex = U4_Decompiled.TILE.TILED_FLOOR;

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
                        tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                }
                if (((above == DUNGEON_TILE.WALL) || (above == DUNGEON_TILE.DOOR_SECRECT)) && (x == 1))
                {
                    if ((y == 5) && (above == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                }
                if (((right == DUNGEON_TILE.WALL) || (right == DUNGEON_TILE.DOOR_SECRECT)) && (x == 9))
                {
                    if ((y == 5) && (right == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                }
                if (((below == DUNGEON_TILE.WALL) || (below == DUNGEON_TILE.DOOR_SECRECT)) && (y == 9))
                {
                    if ((x == 5) && (below == DUNGEON_TILE.DOOR_SECRECT))
                    {
                        tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                }

                // rooms
                if (((left >= DUNGEON_TILE.DUNGEON_ROOM_0) && (left <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (y == 0))
                {

                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)left - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 10];
                    if ((roomTileIndex == U4_Decompiled.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                }
                if (((above >= DUNGEON_TILE.DUNGEON_ROOM_0) && (above <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (x == 0))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)above - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[10, y];
                    if ((roomTileIndex == U4_Decompiled.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }

                    //tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if (((right >= DUNGEON_TILE.DUNGEON_ROOM_0) && (right <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (x == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)right - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[0, y];
                    if ((roomTileIndex == U4_Decompiled.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }

                    // tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if (((below >= DUNGEON_TILE.DUNGEON_ROOM_0) && (below <= DUNGEON_TILE.DUNGEON_ROOM_15)) && (y == 10))
                {
                    // create brick walls on the other side of the room if it is not a tiled floor
                    int room = (int)below - (int)DUNGEON_TILE.DUNGEON_ROOM_0;
                    if (dungeonRooms.Length == 64)
                    {
                        room += (posz >> 1) * 16;
                    }

                    U4_Decompiled.TILE roomTileIndex = dungeonRooms[room].dungeonRoomMap[x, 0];
                    if ((roomTileIndex == U4_Decompiled.TILE.TILED_FLOOR) || (roomTileIndex == U4_Decompiled.TILE.BRICK_FLOOR))
                    {
                        tileIndex = U4_Decompiled.TILE.TILED_FLOOR;
                    }
                    else
                    {
                        tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                    }
                    // tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }

                //corners
                if ((x <= 1) && (y <= 1) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y <= 1) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if ((x >= 9) && (y >= 9) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if ((x <= 1) && (y >= 9) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                }
                if ((x == 0) && (y == 0) && ((diagonalLeftAbove == DUNGEON_TILE.WALL) || (diagonalLeftAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((x == 10) && (y == 0) && ((diagonalRightAbove == DUNGEON_TILE.WALL) || (diagonalRightAbove == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightAbove >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightAbove <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((x == 10) && (y == 10) && ((diagonalRightBelow == DUNGEON_TILE.WALL) || (diagonalRightBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalRightBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalRightBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((x == 0) && (y == 10) && ((diagonalLeftBelow == DUNGEON_TILE.WALL) || (diagonalLeftBelow == DUNGEON_TILE.DOOR_SECRECT) || ((diagonalLeftBelow >= DUNGEON_TILE.DUNGEON_ROOM_0) && (diagonalLeftBelow <= DUNGEON_TILE.DUNGEON_ROOM_15))))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }

                // override
                if ((left == DUNGEON_TILE.WALL) && (y == 0))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((above == DUNGEON_TILE.WALL) && (x == 0))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((right == DUNGEON_TILE.WALL) && (x == 10))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }
                if ((below == DUNGEON_TILE.WALL) && (y == 10))
                {
                    tileIndex = U4_Decompiled.TILE.BLANK;
                }

                map[x, y] = tileIndex;
            }
        }

        return map;
    }
    GameObject CreateDungeonBlock(U4_Decompiled.TILE tileIndex)
    {
        GameObject dungeonBlockGameObject = new GameObject();
        dungeonBlockGameObject.name = tileIndex.ToString();

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                GameObject mapTile;

                if (tileIndex == U4_Decompiled.TILE.BRICK_WALL)
                {
                    mapTile = CreatePartialCube(true, true, true, true);
                }
                else if (tileIndex == U4_Decompiled.TILE.TILED_FLOOR)
                {
                    mapTile = CreateQuad();
                }
                else
                {
                    mapTile = CreateQuad();
                }

                mapTile.transform.SetParent(dungeonBlockGameObject.transform);
                mapTile.transform.localPosition = new Vector3(x, y, 0);
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
            }
        }

        // rotate dungeon into place
        // do this after creating all the blocks
        //dungeonBlockGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonBlockGameObject;
    }
    GameObject CreateDungeonExpandedLevel(DUNGEONS dungeon, int level)
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

                if (dungeonTile == DUNGEON_TILE.WALL)
                {
                    //dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.BRICK_WALL);
                    //dungeonBlockGameObject.name = dungeonTile.ToString();
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
                }
                else if ((dungeonTile == DUNGEON_TILE.HALLWAY) 
                        || (dungeonTile == DUNGEON_TILE.TRAP_FALLING_ROCKS)
                        || (dungeonTile == DUNGEON_TILE.TRAP_PIT)
                        || (dungeonTile == DUNGEON_TILE.TRAP_WIND_DARKNESS)
                        )
                {
                    // TODO figure out what to do with these traps
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled.TILE.TILED_FLOOR);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

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
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled.TILE.SHALLOW_WATER);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_ENERGY)
                {
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled.TILE.ENERGY_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_FIRE)
                {
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled.TILE.FIRE_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_POISON)
                {
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled.TILE.POISON_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.FIELD_SLEEP)
                {
                    // TODO make a pretty fountain
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level],
                        ref dungeons[(int)dungeon].dungeonRooms,
                        x, y, level, U4_Decompiled.TILE.SLEEP_FIELD);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.TREASURE_CHEST)
                {
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled.TILE.CHEST);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else if (dungeonTile == DUNGEON_TILE.MAGIC_ORB)
                {
                    // TODO make orb into a billboard
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(
                        ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled.TILE.MISSLE_ATTACK_BLUE);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                    
                }
                else if (dungeonTile == DUNGEON_TILE.ALTAR)
                {
                    // TODO make altar into a billboard
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(ref dungeons[(int)dungeon].dungeonTILEs[level], 
                        ref dungeons[(int)dungeon].dungeonRooms, 
                        x, y, level, U4_Decompiled.TILE.ALTAR);

                    dungeonBlockGameObject = new GameObject();
                    CreateMap(dungeonBlockGameObject, map);

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);

                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                else
                {
                    // use a combat map as the dungeon room base on the dungeon tile
                    int combat = (int)convertDungeonTileToCombat[(int)dungeons[(int)dungeon].dungeonTILEs[level][x, y] >> 4];

                    // get a halway that fits
                    U4_Decompiled.TILE[,] map = CreateDungeonHallway(ref dungeons[(int)dungeon].dungeonTILEs[level], ref dungeons[(int)dungeon].dungeonRooms, x, y, level);

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
                    CreateMap(dungeonBlockGameObject, map);
                    dungeonBlockGameObject.name = "Combat map hallway " + ((U4_Decompiled.COMBAT_TERRAIN)combat).ToString();

                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);
                }
                //else
                //{
                //    dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.TILED_FLOOR);
                //}

                dungeonBlockGameObject.transform.SetParent(dungeonLevel.transform);
                dungeonBlockGameObject.transform.localPosition = new Vector3(x * 11, y * 11, 0);
            }
        }

        // TODO cannot combine yet as the  areas are already combined, need to rebuild them into the dungeon
        //Combine(dungeonLevel);

        dungeonLevel.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonLevel;
    }

    public GameObject GameText;

    void CreateParty()
    {
        // create player/party object to display texture
        //partyGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        partyGameObject = CreateQuad();
        partyGameObject.transform.SetParent(party.transform);
        
        // rotate the npc game object after creating and addition of child
        partyGameObject.transform.localPosition = new Vector3(0, 0, 0); 
        //partyGameObject.transform.localEulerAngles = new Vector3(90.0f, 180.0f, 0);
        partyGameObject.transform.localEulerAngles = new Vector3(270.0f, 180.0f, 180.0f);

        // create child object for texture
        MeshRenderer renderer = partyGameObject.GetComponent<MeshRenderer>();

        // set the tile
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.PARTY];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        // set the shader
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

        // add so speech works
        partyGameObject.AddComponent<UnityEngine.UI.Text>();

        /*
        // create the bubble text
        GameObject BubbleText = Instantiate(bubblePrefab);
        BubbleText.transform.SetParent(party.transform);
        bubblePrefab.GetComponent<Canvas>().worldCamera = Camera.main;
        bubblePrefab.transform.localPosition = Vector3.zero;
        bubblePrefab.GetComponent<RectTransform>().localPosition = new Vector3(-2.0f, 0.5f, -2.0f);
        bubblePrefab.transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        */
    }

    bool CheckTileForOpacity(U4_Decompiled.TILE tileIndex)
    {
        return (tileIndex == U4_Decompiled.TILE.BRICK_WALL
                    || tileIndex == U4_Decompiled.TILE.LARGE_ROCKS
                    || tileIndex == U4_Decompiled.TILE.SECRET_BRICK_WALL);
    }

    bool CheckShortTileForOpacity(U4_Decompiled.TILE tileIndex)
    {
        return (CheckTileForOpacity(tileIndex) ||
                    ((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.BRACKET_SQUARE)));
    }

    // NOTE certain shaders used for things like sprites and unlit textures do not
    // do well with edges and leave ghosts of the nearby textures from the texture atlas
    // to solve this issue I need to create at least a one pixel mirror border around the
    // tile, this function creates a larger tile texture and adds this border around the tile placed in the center.
    // Special care must be made when combining meshes with textures like this and the Combine()
    // function has been updated to handle this situation and update the uv's. Given that some
    // platforms may require textures be certain integer multiples of 2 this function will allow
    // a larger than one pixel border around the tile.
    const int TILE_BORDER_SIZE = 1;
    int expandedTileWidth;
    int expandedTileHeight;
    int originalTileWidth;
    int originalTileHeight;

    public void ExpandTiles()
    {
        // allocate some textures pointers
        expandedTiles = new Texture2D[256];
        expandedTileWidth = originalTiles[0].width + 2 * TILE_BORDER_SIZE;
        expandedTileHeight = originalTiles[0].height + 2 * TILE_BORDER_SIZE;
        originalTileWidth = originalTiles[0].width;
        originalTileHeight = originalTiles[0].height;

        // go through all the original tiles
        for (int i = 0; i < 256; i++)
        {
            // create a new slightly larger texture with boarder for this tile
            Texture2D currentTile = originalTiles[i];
            Texture2D newTile = new Texture2D(expandedTileWidth, expandedTileHeight, TextureFormat.RGBA32, false);

            // we want pixles not fuzzy images
            newTile.filterMode = FilterMode.Point;

            // go through all the pixels in the source texture
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; width++)
                {
                    // copy the pixles into the middle
                    newTile.SetPixel(width + TILE_BORDER_SIZE, height + TILE_BORDER_SIZE, currentTile.GetPixel(width, height));
                }
            }

            // mirror the pixles on either side
            for (int height = 0; height < currentTile.height; height++)
            {
                // left side
                newTile.SetPixel(TILE_BORDER_SIZE - 1, height + TILE_BORDER_SIZE, currentTile.GetPixel(0, height));
                // right side
                newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, height + TILE_BORDER_SIZE, currentTile.GetPixel(currentTile.width - 1, height));
            }

            // mirror the pixles on top and bottom
            for (int width = 0; width < currentTile.width; width++)
            {
                // left side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE - 1, currentTile.GetPixel(width, 0));
                // right side
                newTile.SetPixel(width + TILE_BORDER_SIZE, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(width, currentTile.height - 1));
            }

            // now the four corners
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE - 1, currentTile.GetPixel(0, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE - 1, currentTile.GetPixel(currentTile.width - 1, 0));
            newTile.SetPixel(TILE_BORDER_SIZE + currentTile.width, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(currentTile.width - 1, currentTile.height - 1));
            newTile.SetPixel(TILE_BORDER_SIZE - 1, TILE_BORDER_SIZE + currentTile.height, currentTile.GetPixel(0, currentTile.height - 1));

            // apply all the previous SetPixel() calls to the texture
            newTile.Apply();

            // save the new expanded texture
            expandedTiles[i] = newTile;
        }
    }

    // TODO fix horse tile also
    public void FixMageTile3()
    {
        // adjust the pixels on mage tile #3
        Texture2D currentTile = originalTiles[(int)U4_Decompiled.TILE.MAGE_NPC3];

        // go through all the pixels in the source texture and shift them one pixel
        for (int height = 0; height < currentTile.height; height++)
        {
            for (int width = currentTile.width - 1; width > 0 ; width--)
            {
                currentTile.SetPixel(width, height, currentTile.GetPixel((width - 1 + currentTile.width) % currentTile.width, height));
            }
        }

        // apply all the previous SetPixel() calls to the texture
        currentTile.Apply();
    }

    public void CreateMap(GameObject mapGameObject, U4_Decompiled.TILE[,] map, bool lookAtCamera = true)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;
        bool useExpandedTile;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // this takes about 1/2 second for the 64x64 outside grid.
        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location;
                Vector3 rotation;
                U4_Decompiled.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled.TILE.ARCHITECTURE))
                {
                    U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled.TILE.MOUNTAINS)
                {
                    mapTile = CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
                {
                    mapTile = CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
                {
                    mapTile = CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
                {
                    mapTile = CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // trees we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled.TILE.VILLAGE) ||
                    (tileIndex == U4_Decompiled.TILE.RUINS) ||
                    (tileIndex == U4_Decompiled.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled.TILE.ALTAR) ||
                   // (tileIndex == U4_Decompiled.TILE.CHEST) ||
                    (tileIndex == U4_Decompiled.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                    mapTile = CreateQuad();
                    mapTile.name = tileIndex.ToString();
                    mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y + 0.001f, 0.0f); // move it just a bit into the back
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    //mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    rotation = new Vector3(-90.0f, 180.0f, 180.0f);

                    if (lookAtCamera)
                    {
                        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                        mapTile.transform.LookAt(look.transform);
                        //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                        rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                        rotation.x = rotation.x - 180.0f;
                    }

                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE)
                {
                    mapTile = CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_TOP)
                {
                    mapTile = CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if ((tileIndex == U4_Decompiled.TILE.DOOR) || (tileIndex == U4_Decompiled.TILE.LOCKED_DOOR))
                {
                    mapTile = CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_MAST)
                {
                    mapTile = CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_WHEEL)
                {
                    mapTile = CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.CHEST)
                {
                    mapTile = CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_LEFT)
                {
                    mapTile = CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_RIGHT)
                {
                    mapTile = CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = CreateQuad();
                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled.TILE.LAVA))
                    {
                        mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                        useExpandedTile = false;
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        useExpandedTile = true;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                if (useExpandedTile)
                {
                    renderer.material.mainTexture = expandedTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                }
                else
                {
                    renderer.material.mainTexture = originalTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                    renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
                }
            }
        }

        // this takes about 150ms for the 64x64 outside grid.
        Combine(terrainGameObject);
        Combine2(animatedTerrrainGameObject);
        Combine(billboardTerrrainGameObject); // combine separately from terrain above as we need to point these towards the player

        // add our little water animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present
        if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
        {
            animatedTerrrainGameObject.AddComponent<Animate1>();
        }

        // Position the settlement in place
        mapGameObject.transform.position = new Vector3(0, 0, 224);

        // rotate settlement into place
        mapGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    public void CreateMapSubset(GameObject mapGameObject, U4_Decompiled.TILE[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                U4_Decompiled.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled.TILE.ARCHITECTURE))
                {
                    U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                    U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled.TILE.MOUNTAINS)
                {
                    mapTile = CreatePyramid(1.0f);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make dungeon entrance into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
                {
                    mapTile = CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
                {
                    mapTile = CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
                {
                    mapTile = CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // tress we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    mapTile = CreateQuad();

                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    // need to move it here first and rotate it into place before we can get the results of LookAt()
                    mapTile.transform.localPosition = location;
                    mapTile.transform.localEulerAngles = new Vector3(-180.0f, -90.0f, 90.0f);
                    Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                    look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                    mapTile.transform.LookAt(look.transform);
                    //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                    rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                    rotation.x = rotation.x - 180.0f;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = CreateQuad();

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled.TILE.LAVA))
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        // since we animate the texture using uv we cannot use the expanded tiles and need to use the original ones
                    }
                    else
                    {
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                Renderer renderer = mapTile.GetComponent<MeshRenderer>();

                renderer.material.mainTexture = originalTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // stash the object mesh, transform & texture information
                entireMapGameObjects[x, y] = mapTile;
            }
        }
    }

    public GameObject CreateMapTileObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled.TILE tileIndex, ref U4_Decompiled.TILE[,] map, int x, int y, bool allWalls)
    {
        GameObject mapTile;
        Vector3 location = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        bool useExpandedTile;
        bool useLinearTile;

        // solid object, brick, rocks etc. make into cubes
        if (CheckTileForOpacity(tileIndex))
        {
            if (allWalls == false)
            {
                U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
            }
            else
            {
                mapTile = CreatePartialCube();
            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // Letters, make into short cubes
        else if (((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.BRACKET_SQUARE))
            || (tileIndex == U4_Decompiled.TILE.ARCHITECTURE))
        {
            if (allWalls == false)
            {
                U4_Decompiled.TILE aboveTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE belowTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE leftTile = U4_Decompiled.TILE.BLANK;
                U4_Decompiled.TILE rightTile = U4_Decompiled.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
            }
            else
            {
                mapTile = CreatePartialCube();

            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make mountains into pyramids
        else if (tileIndex == U4_Decompiled.TILE.MOUNTAINS)
        {
            mapTile = CreatePyramid(1.0f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountatins to show their best side
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make dungeon entrace into pyramid, rotate so it faces the right direction
        else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
        {
            mapTile = CreatePyramid(0.2f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make brush and hills into short pyramids
        else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
        {
            mapTile = CreatePyramid(0.15f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make rocks into little bigger short pyramids since you cannot walk over them
        else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
        {
            mapTile = CreatePyramid(0.25f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // tress we need to stand upright and face the camera
        else if ((tileIndex == U4_Decompiled.TILE.FOREST) ||
            (tileIndex == U4_Decompiled.TILE.TOWN) ||
            (tileIndex == U4_Decompiled.TILE.VILLAGE) ||
            (tileIndex == U4_Decompiled.TILE.RUINS) ||
            (tileIndex == U4_Decompiled.TILE.SHRINE) ||
            (tileIndex == U4_Decompiled.TILE.ANKH) ||
            (tileIndex == U4_Decompiled.TILE.LADDER_UP) ||
            (tileIndex == U4_Decompiled.TILE.LADDER_DOWN) ||
            (tileIndex == U4_Decompiled.TILE.COOKING_FIRE) ||
            (tileIndex == U4_Decompiled.TILE.CASTLE))
        {
            mapTile = CreateQuad();
            mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            // put this in a resonable rotation, combine3() will do the actual lookat rotaion just before displaying
            rotation = new Vector3(-90.0f, -90.0f, 90.0f);

            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.BRIDGE)
        {
            mapTile = CreateBridge();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.BRIDGE_TOP)
        {
            mapTile = CreateBridgeUpper();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.BRIDGE_BOTTOM)
        {
            mapTile = CreateBridgeLower();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if ((tileIndex == U4_Decompiled.TILE.DOOR) || (tileIndex == U4_Decompiled.TILE.LOCKED_DOOR))
        {
            mapTile = CreateDoor();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.BRICK_FLOOR_COLUMN)
        {
            mapTile = CreatePillar();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.SHIP_MAST)
        {
            mapTile = CreateMast();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.SHIP_WHEEL)
        {
            mapTile = CreateWheel();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.CHEST)
        {
            mapTile = CreateChest();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.CASTLE_LEFT)
        {
            mapTile = CreateCastleLeft();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.CASTLE_RIGHT)
        {
            mapTile = CreateCastleRight();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled.TILE.CASTLE_ENTRANCE)
        {
            mapTile = CreateCastleCenter();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // all other terrain tiles are flat
        else
        {
            mapTile = CreateQuad();

            // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
            // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
            if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) ||
                (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)
                || (tileIndex == U4_Decompiled.TILE.LAVA))
            {
                mapTile.transform.SetParent(animatedTerrrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                // since we animate the texture using uv we cannot use the expanded tiles and need to use the linear ones
                useExpandedTile = false;
                useLinearTile = true;
            }
            else
            {
                mapTile.transform.SetParent(terrainGameObject.transform);
                location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                rotation = Vector3.zero;
                useExpandedTile = true;
                useLinearTile = false;
            }
        }

        mapTile.transform.localEulerAngles = rotation;
        mapTile.transform.localPosition = location;

        // all terrain is static, used by combine below to merge meshes
        mapTile.isStatic = true;

        // set the shader
        Renderer renderer = mapTile.GetComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

        // set the tile and texture offset and scale

        if (useExpandedTile)
        {
            renderer.material = combinedExpandedMaterial;
            renderer.material.mainTexture = combinedExpandedTexture;
            renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
            renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

        }
        else if (useLinearTile)
        {
            renderer.material = combinedLinearMaterial;
            renderer.material.mainTexture = combinedLinearTexture;
            renderer.material.mainTextureOffset = new Vector2((float)((int)tileIndex * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
            renderer.material.mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
        }
        else
        {
            renderer.material.mainTexture = originalTiles[(int)tileIndex];
            renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
            renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
        }

        Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
        Vector2[] uv = new Vector2[mesh.uv.Length];
        Vector2 textureAtlasOffset;

        textureAtlasOffset = new Vector2((int)tileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)tileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
        for (int u = 0; u < mesh.uv.Length; u++)
        {
            Vector2 mainTextureOffset;
            Vector2 mainTextureScale;

            if (useExpandedTile)
            {
                mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);
            }
            else if (useLinearTile)
            {
                mainTextureOffset = new Vector2((float)((int)tileIndex * originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                mainTextureScale = new Vector2((float)originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
            }
            else
            {
                mainTextureOffset = new Vector2(0.0f, 0.0f);
                mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
            uv[u] += (textureAtlasOffset + mainTextureOffset);
        }
        mesh.uv = uv;

        renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
        renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

        // disable these as we don't need them in the actual game only for mesh combine
        mapTile.SetActive(false);

        return mapTile;
    }

    GameObject[] allMapTilesGameObjects = null;

    GameObject GetCachedTileGameObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled.TILE tileIndex, ref U4_Decompiled.TILE[,] map, int x, int y, bool allWalls)
    {
        if (allMapTilesGameObjects == null)
        {
            allMapTilesGameObjects = new GameObject[(int)U4_Decompiled.TILE.MAX];
            for (int i = 0; i < (int)U4_Decompiled.TILE.MAX; i++)
            {
                allMapTilesGameObjects[i] = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, (U4_Decompiled.TILE)i, ref map, 0, 0, true);
            }
        }

        return allMapTilesGameObjects[(int)tileIndex];
    }

    public struct Labels
    {
        public List<Label> labels;// = new List<Words>();
    }

    public struct Label
    {
        public List<Letter> letters;// = new List<Letter>();
    }

    public struct Letter
    {
        public U4_Decompiled.TILE tileIndex;
        public int x;
        public int y;
    }

    public void CreateMapLabels(GameObject mapGameObject, ref U4_Decompiled.TILE[,] map)
    {
        Labels mapLabels = new Labels();
        mapLabels.labels = new List<Label>();

        GameObject mapLabelsGameObject;
        GameObject mapLabelsReverseGameObject;

        // create the labels child object if it does not exist
        Transform mapLabelsTransform = mapGameObject.transform.Find("labels");
        if (mapLabelsTransform == null)
        {
            mapLabelsGameObject = new GameObject("labels");
            mapLabelsGameObject.transform.SetParent(mapGameObject.transform);
            mapLabelsGameObject.transform.localPosition = Vector3.zero;
            mapLabelsGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            mapLabelsGameObject = mapLabelsTransform.gameObject;
        }

        // create the reverse labels child object if it does not exist
        Transform mapLabelsReverseTransform = mapGameObject.transform.Find("labels reverse");
        if (mapLabelsReverseTransform == null)
        {
            mapLabelsReverseGameObject = new GameObject("labels reverse");
            mapLabelsReverseGameObject.transform.SetParent(mapGameObject.transform);
            mapLabelsReverseGameObject.transform.localPosition = Vector3.zero;
            mapLabelsReverseGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            mapLabelsReverseGameObject = mapLabelsReverseTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in mapLabelsGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in mapLabelsReverseGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // go through the map tiles and find words
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                U4_Decompiled.TILE tileIndex;

                tileIndex = map[x, y];

                // check if a tile is a letter
                if ((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.Z) || (tileIndex == U4_Decompiled.TILE.SPACE))
                {
                    // create a new label
                    Label label = new Label();
                    label.letters = new List<Letter>();
                    Letter letter;

                    // find all the letters in this label
                    for (; x < map.GetLength(0); x++)
                    {
                        tileIndex = map[x, y];

                        // check if a tile is a letter
                        if ((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.Z) || (tileIndex == U4_Decompiled.TILE.SPACE))
                        {
                            letter.tileIndex = tileIndex;
                            letter.x = x;
                            letter.y = y;

                            // add these letters to the label
                            label.letters.Add(letter);
                        }
                        else
                        {
                            // we are done with this label, drop out
                            break;
                        }
                    }

                    // add this label to the label list and look for more
                    mapLabels.labels.Add(label);
                }
            }
        }

        // go through labels we found above and create a game object label and reverse label for each
        foreach (Label label in mapLabels.labels)
        {
            GameObject labelGameObject = new GameObject("");
            GameObject reverseLabelGameObject = new GameObject("");

            labelGameObject.transform.SetParent(mapLabelsGameObject.transform);
            reverseLabelGameObject.transform.SetParent(mapLabelsReverseGameObject.transform);

            for (int i = 0; i < label.letters.Count; i++)
            {
                Letter letter = label.letters[i];
                U4_Decompiled.TILE reverseTileIndex = label.letters[label.letters.Count - 1 - i].tileIndex;

                if (letter.tileIndex != U4_Decompiled.TILE.SPACE)
                {
                    labelGameObject.name += (char)(letter.tileIndex - U4_Decompiled.TILE.A + 'A');
                }
                else
                {
                    labelGameObject.name += ' ';
                }
                if (reverseTileIndex != U4_Decompiled.TILE.SPACE)
                {
                    //reverseLabelGameObject.name += (char)(reverseTileIndex - U4_Decompiled.TILE.A + 'A');
                    reverseLabelGameObject.name += (char)(letter.tileIndex - U4_Decompiled.TILE.A + 'A');
                }
                else
                {
                    reverseLabelGameObject.name += ' ';
                }

                // create the gameObject tile
                GameObject mapTile = CreatePartialCube();
                mapTile.transform.SetParent(labelGameObject.transform);
                mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                mapTile.transform.localPosition = new Vector3(letter.x, map.GetLength(1) - 1 - letter.y, 0.25f);
                mapTile.transform.localEulerAngles = Vector3.zero;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                renderer.material = combinedExpandedMaterial;
                renderer.material.mainTexture = combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 textureAtlasOffset;

                textureAtlasOffset = new Vector2((int)letter.tileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)letter.tileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // create another game object to match the first but we will reverse the letters
                GameObject reverseMapTile = CreatePartialCube();
                reverseMapTile.transform.SetParent(reverseLabelGameObject.transform);
                reverseMapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                reverseMapTile.transform.localPosition = new Vector3(letter.x, map.GetLength(1) - 1 - letter.y, 0.25f);
                reverseMapTile.transform.localEulerAngles = new Vector3(0, 0, 180); // flip the letter around

                // all terrain is static, used by combine below to merge meshes
                reverseMapTile.isStatic = true;

                // set the shader
                renderer = reverseMapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // set the tile and texture offset and scale
                renderer.material = combinedExpandedMaterial;
                renderer.material.mainTexture = combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                mesh = reverseMapTile.GetComponent<MeshFilter>().mesh;
                uv = new Vector2[mesh.uv.Length];

                textureAtlasOffset = new Vector2((int)reverseTileIndex % textureExpandedAtlasPowerOf2 * expandedTileWidth, (int)reverseTileIndex / textureExpandedAtlasPowerOf2 * expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            Combine(labelGameObject);
            Combine(reverseLabelGameObject);
        }
    }


    public void CreateMapSubsetPass2(GameObject mapGameObject, ref U4_Decompiled.TILE[,] map, ref GameObject[,] mapGameObjects, bool allWalls = false)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // remove any children if present
        foreach (Transform child in terrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in animatedTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }

        // go through the map tiles and create game objects for each
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                U4_Decompiled.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled.TILE.BLANK)
                {
                    // skip it
                    continue;
                }

                // create the gameObject tile
                //GameObject mapTile = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);
                GameObject mapTile = GetCachedTileGameObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, tileIndex, ref map, x, y, allWalls);

                // stash the game object in the array
                mapGameObjects[x, y] = mapTile;
            }
        }
    }

    public void followWorld(GameObject follow)
    {
        // hook the player game object into the camera and the game engine
        MySmoothFollow myScript = FindObjectsOfType<MySmoothFollow>()[0];

        if (myScript.target != follow.transform)
        {
            myScript.target = follow.transform;
        }
    }

    // this one will go two layers deep to avoid an implementation that relies on recursion
    GameObject[] GetAllChildrenWithMeshRenderers(GameObject gameObject)
    {
        int count = 0;

        foreach (Transform child in gameObject.transform)
        {
            if (child.transform.GetComponent<MeshRenderer>())
            {
                count++;
            }

            foreach (Transform childofchild in child.transform)
            {
                if (childofchild.transform.GetComponent<MeshRenderer>())
                {
                    count++;
                }
            }
        }

        GameObject[] objectsToCombine = new GameObject[count];

        count = 0;
        foreach (Transform child in gameObject.transform)
        {
            if (child.transform.GetComponent<MeshRenderer>())
            {
                objectsToCombine[count++] = child.gameObject;
            }
            foreach (Transform childofchild in child.transform)
            {
                if (childofchild.transform.GetComponent<MeshRenderer>())
                {
                    objectsToCombine[count++] = childofchild.gameObject;
                }
            }
        }

        return objectsToCombine;
    }

    GameObject[] GetAllChildrenWithMeshRenderers2(GameObject gameObject)
    {
        int count = 0;

        GameObject[] objectsToCheck = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>())
            {
                count++;
            }
        }

        GameObject[] objectsToCombine = new GameObject[count];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>())
            {
                objectsToCombine[i] = gameObject.transform.GetChild(i).gameObject;
            }
        }

        return objectsToCombine;
    }

    //TODO get the max texture size here also while I'm looking through all the objects
    private int GetTextureSquareSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            if (!textures.Contains(o[i].GetComponent<MeshRenderer>().material.mainTexture))
            {
                textures.Add(o[i].GetComponent<MeshRenderer>().material.mainTexture);
            }
        }

        if (textures.Count == 1) return 1;
        if (textures.Count <= 4) return 2;
        if (textures.Count <= 16) return 4;
        if (textures.Count <= 64) return 8;
        if (textures.Count <= 256) return 16;

        // Doesn't handle more than 256 different textures but I think you can see how to extend
        return 0;
    }

    private int GetTextureSquareSize(GameObject[,] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.GetLength(0); i++)
        {
            for (int j = 0; j < o.GetLength(1); j++)
            {
                if (!textures.Contains(o[i, j].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    textures.Add(o[i, j].GetComponent<MeshRenderer>().material.mainTexture);
                }
            }
        }

        if (textures.Count == 1) return 1;
        if (textures.Count <= 4) return 2;
        if (textures.Count <= 16) return 4;
        if (textures.Count <= 64) return 8;
        if (textures.Count <= 256) return 16;

        // Doesn't handle more than 256 different textures but I think you can see how to extend
        return 0;
    }

    private int GetTextureSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            MeshRenderer meshRenderer = o[i].GetComponent<MeshRenderer>();

            if (meshRenderer)
            {
                if (!textures.Contains(meshRenderer.material.mainTexture))
                {
                    textures.Add(meshRenderer.material.mainTexture);
                }
            }
        }

        return textures.Count;
    }

    private int GetTextureSize(Texture[,] o)
    {
        ArrayList textures = new ArrayList();

        // Find unique textures
        for (int i = 0; i < o.GetLength(0); i++)
        {
            for (int j = 0; i < o.GetLength(1); i++)
            {
                if (!textures.Contains(o[i, j]))
                {
                    textures.Add(o[i, j]);
                }
            }
        }

        return textures.Count;
    }

    /*
     * Combines all object textures into a single texture then creates a material used by all objects.
     * The materials properties are based on those of the material of the top level object.
     * 
     * Also combines any meshes marked as static into a single mesh.
     * 
     * https://forum.unity.com/threads/combine-textures-and-meshes-reduce-draw-calls.117155/
     * 
     * License is MIT ... you can do whatever you want with this, If you don't want to have to include the MIT license send me a PM and I will waive that requirement. 
     * Links or credits to www.jnamobile.com are appreciated but not required.
     * 
     */
    private void Combine(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        int pow2;
        Texture2D combinedTexture;
        //Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        GameObject[] objectsToCombine = GetAllChildrenWithMeshRenderers(gameObject);

        // save current position so we can set it to zero so the localToWorldMatrix works correctly below
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        // Create a mesh filter and renderer if needed
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }

        // clear the mesh just in case there is anything in there
        filter.mesh = null;

        if (objectsToCombine.Length > 0)
        {
            // try to get the texture size used from the first gameobject
            Texture checkTexture = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture;
            if (checkTexture && checkTexture.width > 0)
            {
                originalSizeW = checkTexture.width;
                originalSizeH = checkTexture.height;
            }
            else
            {
                originalSizeW = expandedTileWidth;
                originalSizeH = expandedTileHeight;
            }
            pow2 = GetTextureSquareSize(objectsToCombine);
            sizeW = pow2 * originalSizeW;
            sizeH = pow2 * originalSizeH;
            combinedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined square texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (texture && !textureAtlas.ContainsKey(texture))
                {
                    int x = (index % pow2) * originalSizeW;
                    int y = (index / pow2) * originalSizeH;

                    combinedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());

                    x = index % pow2;
                    y = index / pow2;
                    textureAtlas.Add(texture, new Vector2(x, y));
                    index++;
                }
            }
            combinedTexture.Apply();
            combinedMaterial = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
            combinedMaterial.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 textureAtlasOffset;
                Material objectMaterial = objectsToCombine[i].GetComponent<MeshRenderer>().material;
                if (objectMaterial.mainTexture && textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    textureAtlasOffset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = Vector2.Scale(mesh.uv[u], objectMaterial.mainTextureScale) / (float)pow2;
                        uv[u] += (textureAtlasOffset + objectMaterial.mainTextureOffset) / (float)pow2;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<MeshRenderer>().material = combinedMaterial;
            }

            // Measure starting point memory use
            long GC_MemoryStart = System.GC.GetTotalMemory(true);

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];

            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                //if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    // need to convert to world coords before combining
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Obtain measurements after creating the new byte[]
            long GC_MemoryEnd = System.GC.GetTotalMemory(true);

            // Ensure that the Array stays in memory and doesn't get optimized away
            System.GC.KeepAlive(combine);

            //Debug.Log("memory start " + GC_MemoryStart + " end " + GC_MemoryEnd + " diff " + (GC_MemoryEnd - GC_MemoryStart));

            // Create a mesh filter and renderer
            if (staticCount > 0)
            {
                filter.mesh = new Mesh();
                filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                //Debug.Log("Before : " + Time.realtimeSinceStartup);
                filter.mesh.CombineMeshes(combine);
                //Debug.Log("After : " + Time.realtimeSinceStartup);
                renderer.material = combinedMaterial;
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    //if (objectsToCombine[i].isStatic)
                    {
                        if (destroy)
                        {
                            Destroy(objectsToCombine[i]);
                        }
                        else
                        {
                            objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().material = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }
            }

            //Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    // This version creates a horizontal texture atlas so we can do UV animation for things like water and lava.
    private void Combine2(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int sizeW;
        int originalSizeW;
        int originalSizeH;
        int textureCount;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        GameObject[] objectsToCombine = GetAllChildrenWithMeshRenderers(gameObject);

        // save current position so we can set it to zero so the localToWorldMatrix works correctly below
        Vector3 position = gameObject.transform.position;
        Quaternion rotation = gameObject.transform.rotation;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        // Create a mesh filter and renderer if needed
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }

        // clear the mesh just in case there is anything in there
        filter.mesh = null;

        if (objectsToCombine.Length > 0)
        {
            originalSizeW = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            originalSizeH = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.height;
            textureCount = GetTextureSize(objectsToCombine);
            sizeW = textureCount * originalSizeW;
            combinedTexture = new Texture2D(sizeW, originalSizeH, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {
                    int x = index * originalSizeW;
                    int y = 0;

                    combinedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());

                    x = index;
                    y = 0;
                    textureAtlas.Add(texture, new Vector2(x, y));
                    index++;
                }
            }
            combinedTexture.Apply();
            material = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
            material.mainTexture = combinedTexture;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u];
                        uv[u].x = uv[u].x / (float)textureCount;
                        uv[u].x += ((float)offset.x) / (float)textureCount;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<MeshRenderer>().material = material;
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // create a new mesh and combine them if there is anything to combine
            if (staticCount > 0)
            {
                filter.mesh = new Mesh();
                // the world map has more the 64K elements, need to use 32 bit ints
                filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    if (objectsToCombine[i].isStatic)
                    {
                        if (destroy)
                        {
                            Destroy(objectsToCombine[i]);
                        }
                        else
                        {
                            objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().material = null;
                            objectsToCombine[i].GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                }
            }

            //Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    public Texture2D combinedTexture;
    public Material combinedMaterial;
    public Hashtable textureAtlasHashTable = new Hashtable();
    public int textureAtlasPowerOf2;
    void CreateSquareTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0 )
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = textureAtlasPowerOf2 * originalSizeW;
        sizeH = textureAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture && !textureAtlasHashTable.ContainsKey(texture))
            {
                int x = (index % textureAtlasPowerOf2) * originalSizeW;
                int y = (index / textureAtlasPowerOf2) * originalSizeH;

                combinedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());

                x = index % textureAtlasPowerOf2;
                y = index / textureAtlasPowerOf2;
                textureAtlasHashTable.Add(texture, new Vector2(x, y));
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedTexture.Apply();

        // create a material based on this texture atlas
        combinedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedMaterial.mainTexture = combinedTexture;
    }

    public Texture2D combinedLinearTexture;
    public Material combinedLinearMaterial;
    void CreateLinearTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW;
        int originalSizeW;
        int originalSizeH;
        Texture2D texture;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        // extend as needed
        else if (tilesTextures.Length > 256)
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width; 
        originalSizeH = tilesTextures[0].height;

        // calc full size of texture atlas
        sizeW = tilesTextures.Length * originalSizeW;

        // creare a new combined texture big enough to hold all the textures
        combinedLinearTexture = new Texture2D(sizeW, originalSizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedLinearTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        int index = 0;
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            texture = (Texture2D)tilesTextures[i];
            if (texture)
            {
                int x = index * originalSizeW;
                int y = 0;

                combinedLinearTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
                index++;
            }
        }
        // apply all the pixles we copied in the loop above
        combinedLinearTexture.Apply();

        // create a material based on this texture atlas
        combinedLinearMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedLinearMaterial.mainTexture = combinedLinearTexture;
    }

    public Texture2D combinedExpandedTexture;
    public Material combinedExpandedMaterial;
    public int textureExpandedAtlasPowerOf2;

    void CreateExpandedTextureAtlas(ref Texture2D[] tilesTextures, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32)
    {
        int sizeW; 
        int sizeH;
        int originalSizeW; 
        int originalSizeH;

        // figure out the square size power of 2 factor for the number of textures we have
        if (tilesTextures.Length == 0)
        {
            Debug.Log("empty tilesTextures");
            return;
        }
        else if (tilesTextures.Length == 1)
        {
            textureExpandedAtlasPowerOf2 = 1;
        }
        else if (tilesTextures.Length <= 4)
        {
            textureExpandedAtlasPowerOf2 = 2;
        }
        else if (tilesTextures.Length <= 16)
        {
            textureExpandedAtlasPowerOf2 = 4;
        }
        else if (tilesTextures.Length <= 64)
        {
            textureExpandedAtlasPowerOf2 = 8;
        }
        else if (tilesTextures.Length <= 256)
        {
            textureExpandedAtlasPowerOf2 = 16;
        }
        // extend as needed
        else
        {
            Debug.Log("too many tilesTextures");
            return;
        }

        // check for null texture in array
        if (tilesTextures[0] == null)
        {
            Debug.Log("empty tilesTextures[0]");
            return;
        }

        // get the texture size used from the first one
        originalSizeW = tilesTextures[0].width;
        originalSizeH = tilesTextures[0].height;

        // calc full square size of texture atlas
        sizeW = textureExpandedAtlasPowerOf2 * originalSizeW;
        sizeH = textureExpandedAtlasPowerOf2 * originalSizeH;

        // creare a new combined texture big enough to hold all the textures
        combinedExpandedTexture = new Texture2D(sizeW, sizeH, textureFormat, useMipMaps);
        // we want our pixels
        combinedExpandedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            Texture2D texture = tilesTextures[i];
            if (texture)
            {
                int x = (i % textureExpandedAtlasPowerOf2) * originalSizeW;
                int y = (i / textureExpandedAtlasPowerOf2) * originalSizeH;

                combinedExpandedTexture.SetPixels(x, y, originalSizeW, originalSizeH, texture.GetPixels());
            }
        }
        // apply all the pixles we copied in the loop above
        combinedExpandedTexture.Apply();

        // create a material based on this texture atlas
        combinedExpandedMaterial = new Material(Shader.Find("Unlit/Transparent Cutout"));
        combinedExpandedMaterial.mainTexture = combinedExpandedTexture;
    }

    private void Combine3(GameObject mapGameObject,
        ref U4_Decompiled.TILE [,] map, 
        int offset_x,
        int offset_y,
        ref GameObject [,] mapGameObjects,
        bool useMipMaps, 
        TextureFormat textureFormat,
        bool wrap)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;

        CombineInstance[] terrainCombine; 
        CombineInstance[] animatedTerrrainCombine;
        CombineInstance[] billboardTerrrainCombine;

        // counts how many meshes we need to combine
        int countTerrain = 0;
        int countAnimatedTerrrain = 0;
        int countBillboardTerrrain = 0;

        float startTime = Time.realtimeSinceStartup;

        // create the terrain child object if it does not exist
        Transform terrainTransform = mapGameObject.transform.Find("terrain");
        if (terrainTransform == null)
        {
            terrainGameObject = new GameObject("terrain");
            terrainGameObject.transform.SetParent(mapGameObject.transform);
            terrainGameObject.transform.localPosition = Vector3.zero;
            terrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            terrainGameObject = terrainTransform.gameObject;
        }

        // create the water child object if it does not exist
        Transform waterTransform = mapGameObject.transform.Find("water");
        if (waterTransform == null)
        {
            animatedTerrrainGameObject = new GameObject("water");
            animatedTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            animatedTerrrainGameObject.transform.localPosition = Vector3.zero;
            animatedTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            animatedTerrrainGameObject = waterTransform.gameObject;
        }

        // create the billboard child object if it does not exist
        Transform billboardTransform = mapGameObject.transform.Find("billboard");
        if (billboardTransform == null)
        {
            billboardTerrrainGameObject = new GameObject("billboard");
            billboardTerrrainGameObject.transform.SetParent(mapGameObject.transform);
            billboardTerrrainGameObject.transform.localPosition = Vector3.zero;
            billboardTerrrainGameObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            billboardTerrrainGameObject = billboardTransform.gameObject;
        }

        // save current position on this gameObject so we can set it to zero so the localToWorldMatrix works correctly below
        //Vector3 position = mapGameObject.transform.position;
        //Quaternion rotation = mapGameObject.transform.rotation;

        // set the game objects position to the zero location and rotation
        //mapGameObject.transform.position = Vector3.zero;
        //mapGameObject.transform.rotation = Quaternion.identity;

        // Create a mesh filter and renderer on this gameObject if needed
        MeshFilter terrainFilter = terrainGameObject.GetComponent<MeshFilter>();
        if (terrainFilter == null)
        {
            terrainFilter = terrainGameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer terrainRenderer = terrainGameObject.GetComponent<MeshRenderer>();
        if (terrainRenderer == null)
        {
            terrainRenderer = terrainGameObject.AddComponent<MeshRenderer>();
        }

        // Create a mesh filter and renderer on this gameObject if needed
        MeshFilter animatedTerrrainFilter = animatedTerrrainGameObject.GetComponent<MeshFilter>();
        if (animatedTerrrainFilter == null)
        {
            animatedTerrrainFilter = animatedTerrrainGameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer animatedTerrrainRenderer = animatedTerrrainGameObject.GetComponent<MeshRenderer>();
        if (animatedTerrrainRenderer == null)
        {
            animatedTerrrainRenderer = animatedTerrrainGameObject.AddComponent<MeshRenderer>();
        }

        // Create a mesh filter and renderer on this gameObject if needed
        MeshFilter billboardFilter = billboardTerrrainGameObject.GetComponent<MeshFilter>();
        if (billboardFilter == null)
        {
            billboardFilter = billboardTerrrainGameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer billboardRenderer = billboardTerrrainGameObject.GetComponent<MeshRenderer>();
        if (billboardRenderer == null)
        {
            billboardRenderer = billboardTerrrainGameObject.AddComponent<MeshRenderer>();
        }

        // clear the mesh and material just in case there is anything in there
        terrainFilter.mesh = null;
        terrainRenderer.material = null;
        animatedTerrrainFilter.mesh = null;
        animatedTerrrainRenderer.material = null;
        billboardFilter.mesh = null;
        billboardRenderer.material = null;

        // check if we have some textures
        if (mapGameObjects.GetLength(0) > 1 && mapGameObjects.GetLength(1) > 1)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    U4_Decompiled.TILE tileIndex = map[i, j];

                    // only count them if the raycaster has copied the tile
                    if (tileIndex != U4_Decompiled.TILE.BLANK)
                    {
                        int x;
                        int y;

                        // only count them if wrapping or inside bounds
                        if (wrap == false)
                        {
                            x = i + offset_x;
                            y = j + offset_y;

                            if ((x > mapGameObjects.GetLength(0) - 1) || (y > mapGameObjects.GetLength(1) - 1) || x < 0 || y < 0)
                            {
                                continue;
                            }
                        }

                        if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) || 
                            ((tileIndex >= U4_Decompiled.TILE.POISON_FIELD) && (tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)) || 
                            (tileIndex == U4_Decompiled.TILE.LAVA))
                        {
                            // increment the count
                            countAnimatedTerrrain++;
                        }
                        else if ((tileIndex == U4_Decompiled.TILE.FOREST) ||
                            (tileIndex == U4_Decompiled.TILE.TOWN) ||
                            (tileIndex == U4_Decompiled.TILE.VILLAGE) ||
                            (tileIndex == U4_Decompiled.TILE.ANKH) ||
                            (tileIndex == U4_Decompiled.TILE.LADDER_UP) ||
                            (tileIndex == U4_Decompiled.TILE.LADDER_DOWN) ||
                            (tileIndex == U4_Decompiled.TILE.COOKING_FIRE) ||
                            (tileIndex == U4_Decompiled.TILE.CASTLE))
                        {
                            countBillboardTerrrain++;
                        }
                        else
                        {
                            countTerrain++;
                        }
                    }
                }
            }

            // allocate the combine array to fit the number of meshes we need to combine
            terrainCombine = new CombineInstance[countTerrain];
            animatedTerrrainCombine = new CombineInstance[countAnimatedTerrrain];
            billboardTerrrainCombine = new CombineInstance[countBillboardTerrrain];
            
            // reset the counters
            countTerrain = 0;
            countAnimatedTerrrain = 0;
            countBillboardTerrrain = 0;

            // add all the meshes into the combine arrays
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    U4_Decompiled.TILE tileIndex = map[i, j];

                    // only add them if the raycaster has copied the tile
                    if (tileIndex != U4_Decompiled.TILE.BLANK)
                    {
                        int x;
                        int y;

                        if (wrap)
                        {
                            x = (i + offset_x + mapGameObjects.GetLength(0)) % mapGameObjects.GetLength(0);
                            y = (j + offset_y + mapGameObjects.GetLength(1)) % mapGameObjects.GetLength(1);

                            // need to adjust the position for wrapping
                            mapGameObjects[x, y].transform.localPosition = new Vector3(i + offset_x, mapGameObjects.GetLength(1) - 1 - (j + offset_y), mapGameObjects[x, y].transform.localPosition.z);
                        }
                        else
                        {
                            x = i + offset_x;
                            y = j + offset_y;

                            if ((x > mapGameObjects.GetLength(0) - 1) || (y > mapGameObjects.GetLength(1) - 1) || x < 0 || y < 0)
                            {
                                continue;
                            }
                            mapGameObjects[x, y].transform.localPosition = new Vector3(x, mapGameObjects.GetLength(1) - 1 - y, mapGameObjects[x, y].transform.localPosition.z);
                        }

                        if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) ||
                            ((tileIndex >= U4_Decompiled.TILE.POISON_FIELD) && (tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD)) ||
                            (tileIndex == U4_Decompiled.TILE.LAVA))
                        {
                            // add the game object mesh to the list we want to combine
                            animatedTerrrainCombine[countAnimatedTerrrain].mesh = mapGameObjects[x, y].GetComponent<MeshFilter>().mesh;

                            // need to convert the transform to world coords before combining
                            animatedTerrrainCombine[countAnimatedTerrrain].transform = mapGameObjects[x, y].transform.localToWorldMatrix;

                            // increment the count
                            countAnimatedTerrrain++;
                        }
                        else if ((tileIndex == U4_Decompiled.TILE.FOREST) ||
                            (tileIndex == U4_Decompiled.TILE.TOWN) ||
                            (tileIndex == U4_Decompiled.TILE.ANKH) ||
                            (tileIndex == U4_Decompiled.TILE.VILLAGE) ||
                            (tileIndex == U4_Decompiled.TILE.LADDER_UP) ||
                            (tileIndex == U4_Decompiled.TILE.LADDER_DOWN) ||
                            (tileIndex == U4_Decompiled.TILE.COOKING_FIRE) ||
                            (tileIndex == U4_Decompiled.TILE.CASTLE))
                        {
                            GameObject mapTile = mapGameObjects[x, y];
                            Vector3 saveLocalPosition = mapTile.transform.localPosition;

                            mapTile.transform.localPosition = Vector3.zero;
                            mapTile.transform.localRotation = Quaternion.identity;
                            // temp rotate it like it will be eventually
                            mapTile.transform.localEulerAngles = new Vector3(-90.0f, 90.0f, -90.0f); 
                            // get the location of where we will look from so we can create a LookAt() rotation
                            Vector3 look = new Vector3(u4.Party._x - saveLocalPosition.x, 0.0f, (255 - (u4.Party._y + 5)) - saveLocalPosition.y);
                            Quaternion rotation1 = Quaternion.LookRotation(look);
                            mapTile.transform.rotation = rotation1;
                            mapTile.transform.localEulerAngles = new Vector3( rotation1.eulerAngles.y + 90f, 90f, - 90f);

                            // restore position
                            mapTile.transform.localPosition = saveLocalPosition;

                            // add the game object mesh to the list we want to combine
                            billboardTerrrainCombine[countBillboardTerrrain].mesh = mapTile.GetComponent<MeshFilter>().mesh;
                            // need to convert the transform to world coords before combining
                            billboardTerrrainCombine[countBillboardTerrrain].transform = mapTile.transform.localToWorldMatrix;

                            // increment the index
                            countBillboardTerrrain++;
                        }
                        else
                        {
                            // add the game object mesh to the list we want to combine
                            terrainCombine[countTerrain].mesh = mapGameObjects[x, y].GetComponent<MeshFilter>().mesh;
                            // need to convert the transform to world coords before combining
                            terrainCombine[countTerrain].transform = mapGameObjects[x, y].transform.localToWorldMatrix;
                            
                            // increment the index
                            countTerrain++;
                        }
                    }
                }
            }

            // combine the meshes and set the game object material
            if (countTerrain > 0)
            {
                //terrainFilter.mesh = new Mesh();
                terrainFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                terrainFilter.mesh.CombineMeshes(terrainCombine);
                terrainRenderer.material = combinedExpandedMaterial;
                terrainRenderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                terrainRenderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            if (countAnimatedTerrrain > 0)
            {
                //animatedTerrrainFilter.mesh = new Mesh();
                animatedTerrrainFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                animatedTerrrainFilter.mesh.CombineMeshes(animatedTerrrainCombine);
                animatedTerrrainRenderer.material = combinedLinearMaterial;
                animatedTerrrainRenderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                animatedTerrrainRenderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // add our little water animator script
                // adding a script component in the editor is a significant performance hit, avoid adding if already present
                if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
                {
                    animatedTerrrainGameObject.AddComponent<Animate1>();
                }
            }

            if (countBillboardTerrrain > 0)
            {
                //billboardFilter.mesh = new Mesh();
                billboardFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                billboardFilter.mesh.CombineMeshes(billboardTerrrainCombine);
                billboardRenderer.material = combinedExpandedMaterial;
                billboardRenderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                billboardRenderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            // cleanup any unused resources now
            Resources.UnloadUnusedAssets(); // this can take a really long time if you have a lot of objects, keep the object count down and use only when nessesary
        }

        // Restore the game object position
        //terrainGameObject.transform.position = position;
        //terrainGameObject.transform.rotation = rotation;

        //Debug.Log("Combine3 processing time : " + (Time.realtimeSinceStartup - startTime));
    }

    public void AddFighters(U4_Decompiled.t_68[] currentFighters, U4_Decompiled.tCombat1[] currentCombat, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (fighters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (fighters.transform.childCount != 16)
        {
            for (int i = 0; i < 16; i++)
            {
                // a child object for each fighters entry in the table
                //GameObject fighterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject fighterGameObject = CreateQuad();

                // get the renderer
                MeshRenderer renderer = fighterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = fighterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the fighters game object into position after creating
                Vector3 fightersLocation = new Vector3(0, 255, 0);
                fighterGameObject.transform.localPosition = fightersLocation;
                fighterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                fighterGameObject.transform.SetParent(fighters.transform);

                // set as intially disabled
                fighterGameObject.SetActive(false);
            }

            // rotate characters into place
            fighters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all fighters in the table
        for (int fighterIndex = 0; fighterIndex < 16; fighterIndex++)
        {
            // get the tile
            U4_Decompiled.TILE npcTile = currentFighters[fighterIndex]._tile;
            U4_Decompiled.TILE npcCurrentTile = currentFighters[fighterIndex]._gtile;

            // get the corresponding fighters game object
            Transform childoffighters = fighters.transform.GetChild(fighterIndex);

            if (npcTile == U4_Decompiled.TILE.DEEP_WATER)
            {
                childoffighters.gameObject.SetActive(false);
            }
            else
            {
                childoffighters.gameObject.SetActive(true);
            }

            // update the tile of the game object
            if (currentFighters[fighterIndex]._sleeping == 0)
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(npcCurrentTile);
            }
            else
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(U4_Decompiled.TILE.SLEEP);
            }

            // update the position
            childoffighters.localPosition = new Vector3(currentCombat[fighterIndex]._npcX + offsetx, 255 - currentCombat[fighterIndex]._npcY + offsety, 0);
            childoffighters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            childoffighters.transform.LookAt(look.transform);
            Vector3 rot = childoffighters.transform.eulerAngles;
            childoffighters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }
    }

    public void AddCharacters(U4_Decompiled.tCombat2[] currentCombat2, U4_Decompiled.tParty currentParty, U4_Decompiled.t_68[] currentFighters, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (characters == null)
        {
            return;
        }

        // need to create character game objects if none are present
        if (characters.transform.childCount != 8)
        {
            for (int i = 0; i < 8; i++)
            {
                // a child object for each character entry in the table
                //GameObject characterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject characterGameObject = CreateQuad();

                // get the renderer
                MeshRenderer renderer = characterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = characterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the character game object into position after creating
                Vector3 characterLocation = new Vector3(0, 255, 0);
                characterGameObject.transform.localPosition = characterLocation;
                characterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                characterGameObject.transform.SetParent(characters.transform);

                // set as intially disabled
                characterGameObject.SetActive(false);
            }

            // rotate characters into place
            characters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all characters in the party table
        for (int characterIndex = 0; characterIndex < 8; characterIndex++)
        {
            U4_Decompiled.TILE npcTile;

            if (characterIndex < currentParty.f_1d8)
            {
                // get the tile ???, use class or something?
                npcTile = currentFighters[characterIndex]._chtile;
            }
            else
            {
                // set unused characters to 0
                npcTile = 0;
            }

            // get the corresponding character game object
            Transform childofcharacters = characters.transform.GetChild(characterIndex);

            if (npcTile == U4_Decompiled.TILE.DEEP_WATER)
            {
                childofcharacters.gameObject.SetActive(false);
            }
            else
            {
                childofcharacters.gameObject.SetActive(true);
            }

            // update the tile of the game object
            childofcharacters.GetComponent<Animate3>().SetNPCTile(npcTile);
  
            // update the position
            childofcharacters.localPosition = new Vector3(currentCombat2[characterIndex]._charaX + offsetx, 255 - currentCombat2[characterIndex]._charaY + offsety, 0); // appears to be one off in the Y from the fighters
            childofcharacters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            childofcharacters.transform.LookAt(look.transform);
            Vector3 rot = childofcharacters.transform.eulerAngles;
            childofcharacters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }

        FindObjectsOfType<MySmoothFollow>()[0].target = characters.transform.GetChild(0);
    }

    public void AddMoongate()
    {
        MeshRenderer renderer;
        
        // have we finished creating the world
        if (moongate == null)
        {
            return;
        }

        // need to create moongate child game objects if none is present
        if (moongate.transform.childCount != 1)
        {
            // create the moongate object
            GameObject moongateGameObject = CreateQuad();

            // get the renderer
            renderer = moongateGameObject.GetComponent<MeshRenderer>();

            // intially the texture is null
            renderer.material.mainTexture = null;

            // set the shader
            renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

            // rotate the moongate game object into position after creating
            Vector3 moongateLocation = new Vector3(0, 255, 0);
            moongateGameObject.transform.localPosition = moongateLocation;
            moongateGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // set this as a parent of the moongate game object
            moongateGameObject.transform.SetParent(moongate.transform);

            // rotate moongate into place
            moongate.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // get the corresponding moongate game object
        Transform childofmoongate = moongate.transform.GetChild(0);

        renderer = childofmoongate.GetComponent<MeshRenderer>();

        renderer.material.mainTexture = originalTiles[(int)u4.moongate_tile];

        // get adjusted position based on the offset of the raycastOutdoorMap due to the player position
        int posx = u4.moongate_x - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
        int posy = u4.moongate_y - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
        // can we see the npc
        if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == U4_Decompiled.TILE.BLANK)
        {
            childofmoongate.gameObject.SetActive(false);
        }
        else
        {
            childofmoongate.gameObject.SetActive(true);
        }

        // update the position
        childofmoongate.localPosition = new Vector3(u4.moongate_x, entireMapTILEs.GetLength(1) - 1 - u4.moongate_y, 0); 

        //childofmoongate.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);
        // make it billboard
        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
        childofmoongate.transform.LookAt(look.transform);
        Vector3 rot = childofmoongate.transform.eulerAngles;
        childofmoongate.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
    }

    public void AddNPCs(U4_Decompiled.tNPC[] currentNpcs)
    {
        // have we finished creating the world
        if (npcs == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (npcs.transform.childCount != 32)
        {
            for (int i = 0; i < 32; i++)
            {
                // a child object for each npc entry in the table
                //GameObject npcGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject npcGameObject = CreateQuad();

                // get the renderer
                MeshRenderer renderer = npcGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // add our little animator script and set the tile
                Animate3 animate = npcGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.ObjectRenderer = renderer;

                // rotate the npc game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                npcGameObject.transform.localPosition = npcLocation;
                npcGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the npcs game object
                npcGameObject.transform.SetParent(npcs.transform);

                // set as intially disabled
                npcGameObject.SetActive(false);
            }

            // rotate npcs into place
            npcs.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all npcs in the table
        for (int npcIndex = 0; npcIndex < 32; npcIndex++)
        {
            // get the corresponding npc game object
            Transform childofnpcs = npcs.transform.GetChild(npcIndex);

            // get the npc tile
            U4_Decompiled.TILE npcTile = currentNpcs[npcIndex]._tile;
            U4_Decompiled.TILE npcCurrentTile = currentNpcs[npcIndex]._gtile;

            // check if npc is active
            if (npcTile == U4_Decompiled.TILE.DEEP_WATER)
            {
                // disable object if not active
                childofnpcs.gameObject.SetActive(false);
            }
            else
            {
                // get the npc position
                int posx = currentNpcs[npcIndex]._x;
                int posy = currentNpcs[npcIndex]._y;

                // inside buildings we need to check extra stuff
                if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
                {
                    SETTLEMENT settlement;

                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((u4.Party._loc == U4_Decompiled.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled.TILE.LADDER_UP))
                    {
                        settlement = SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (SETTLEMENT)u4.Party._loc;
                    }

                    // set the name of the game object to match the npc
                    if ((currentNpcs[npcIndex]._tlkidx == 0) || (currentNpcs[npcIndex]._tlkidx > 16 /* sometimes this is 127 */))
                    {
                        childofnpcs.name = npcTile.ToString();
                    }
                    else
                    {
                        childofnpcs.name = npcStrings[(int)settlement][currentNpcs[npcIndex]._tlkidx - 1][(int)NPC_STRING_INDEX.NAME];
                    }

                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastSettlementMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastSettlementMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastSettlementMap.GetLength(0) || posy >= raycastSettlementMap.GetLength(1) || raycastSettlementMap[posx, posy] == U4_Decompiled.TILE.BLANK)
                    {
                        childofnpcs.gameObject.SetActive(false);
                    }
                    else
                    {
                        childofnpcs.gameObject.SetActive(true);
                    }
                }
                else if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
                {
                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == U4_Decompiled.TILE.BLANK)
                    {
                        childofnpcs.gameObject.SetActive(false);
                    }
                    else
                    {
                        childofnpcs.gameObject.SetActive(true);
                    }

                    // set the name of the game object to match the npc
                    childofnpcs.name = npcTile.ToString();
                }

                // update the tile of the game object
                //childofnpcs.GetComponent<Animate3>().SetNPCTile(npcTile);
                childofnpcs.GetComponent<Animate3>().SetNPCTile(npcCurrentTile);
                
                // update the position
                childofnpcs.localPosition = new Vector3(currentNpcs[npcIndex]._x, entireMapTILEs.GetLength(1) - 1 - currentNpcs[npcIndex]._y, 0);

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                childofnpcs.transform.LookAt(look.transform);
                Vector3 rot = childofnpcs.transform.eulerAngles;
                childofnpcs.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y , rot.z + 180.0f);
            }
        }
    }
    public void AddHits(List<U4_Decompiled.hit> currentHitList, int offsetx = 0, int offsety = 0)
    {
        // have we finished creating the world
        if (hits == null)
        {
            return;
        }

        // need to create hit game objects if none are present, will will use a pool of 10
        if (hits.transform.childCount != 10)
        {
            for (int i = 0; i < 10; i++)
            {
                // a child object for each npc entry in the table
                //GameObject hitGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GameObject hitGameObject = CreateQuad();

                // get the renderer
                MeshRenderer renderer = hitGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout");

                // rotate the hit game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                hitGameObject.transform.localPosition = npcLocation;
                hitGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the hits game object
                hitGameObject.transform.SetParent(hits.transform);

                // set as intially disabled
                hitGameObject.SetActive(false);
            }

            // rotate npcs into place
            hits.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        // update all hit games with data from the table
        for (int hitIndex = 0; hitIndex < 10; hitIndex++)
        {
            // get the corresponding hit game object
            Transform childofhits = hits.transform.GetChild(hitIndex);
            
            // do we need to use the pool game object
            if (hitIndex < currentHitList.Count)
            {
                // get the tile
                U4_Decompiled.TILE hitTile = currentHitList[hitIndex].tile;

                // update the tile of the game object
                childofhits.GetComponent<Renderer>().material.mainTexture = originalTiles[(int)hitTile];

                // update the position
                childofhits.localPosition = new Vector3(currentHitList[hitIndex].x + offsetx, 255 - currentHitList[hitIndex].y - 0.01f + offsety, 0); // move it slightly in from of the characters and fighters so we can see it.

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                childofhits.transform.LookAt(look.transform);
                Vector3 rot = childofhits.transform.eulerAngles;
                childofhits.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

                // set as enabled
                childofhits.gameObject.SetActive(true);
            }
            else
            {
                // set as disabled
                childofhits.gameObject.SetActive(false);
            }
        }
    }

    public void AddActiveCharacter(U4_Decompiled.activeCharacter currentActiveCharacter, int offsetx = 0, int offsety = 0)
    {
        if (activeCharacter == null)
        {
            activeCharacter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            activeCharacter.transform.SetParent(transform);
            activeCharacter.transform.localPosition = Vector3.zero;
            activeCharacter.transform.localRotation = Quaternion.identity;
            activeCharacter.name = "Active Character";
            MeshRenderer renderer = activeCharacter.GetComponent<MeshRenderer>();
            // set the shader
            renderer.material.shader = Shader.Find("Custom/Geometry/Wireframe");
            //Shader.Find("Custom/Geometry/Wireframe").EnableKeyword("_REMOVEDIAG_ON")
            renderer.material.SetFloat("_WireframeVal", 0.03f);
            renderer.material.SetFloat("_RemoveDiag", 1);
            renderer.material.SetColor("_FrontColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);

            // rotate active character box into place
            characters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        if (currentActiveCharacter.active)
        {
            Vector3 location = new Vector3(currentActiveCharacter.x + offsetx, 0.01f, entireMapTILEs.GetLength(1) - 1 - currentActiveCharacter.y + offsety);
            activeCharacter.transform.localPosition = location;
            activeCharacter.SetActive(true);
        }
        else
        {
            activeCharacter.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
 
    }

    // cast one ray
    int Cast_Ray(ref U4_Decompiled.TILE[,] map, 
        int diff_x,
        int diff_y,
        int pos_x,
        int pos_y, 
        ref U4_Decompiled.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled.TILE wrapTile)
    {
        int checksum = 0;

        U4_Decompiled.TILE temp_tile;

        // are we outside the destination raycast map area, stop here
        if (pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1) || pos_x - offset_x < 0 || pos_y - offset_y < 0)
        {
            return checksum;
        }

        // has the tile already been copied, if so stop here
        if (raycastMap[pos_x - offset_x, pos_y - offset_y] != U4_Decompiled.TILE.BLANK)
        {
            return checksum;
        }

        // check if we should wrap the source map or if we should fill
        // any tile outside of the map area with a specific tile such as GRASS
        // are we outside the source map?
        if ((wrapTile != U4_Decompiled.TILE.BLANK) && (pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1) || pos_x < 0 || pos_y < 0))
        {
            temp_tile = wrapTile;
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
            checksum += (int)temp_tile;
        }
        else
        {
            // get the tile and copy it to the raycast map
            temp_tile = map[(pos_x + map.GetLength(0)) % map.GetLength(0), (pos_y + map.GetLength(1)) % map.GetLength(1)];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
            checksum += (int)temp_tile;
        }

        // check the tile for opaque tiles
        if ((temp_tile == U4_Decompiled.TILE.FOREST) ||
            (temp_tile == U4_Decompiled.TILE.MOUNTAINS) ||
            (temp_tile == U4_Decompiled.TILE.BLANK) ||
            (temp_tile == U4_Decompiled.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == U4_Decompiled.TILE.BRICK_WALL))
        {
            return checksum;
        }

        // continue the ray cast recursively
        pos_x = (pos_x + diff_x);
        pos_y = (pos_y + diff_y);
        checksum += Cast_Ray(ref map, diff_x, diff_y, pos_x, pos_y, ref raycastMap, offset_x, offset_y, wrapTile);
        
        if ((diff_x & diff_y) != 0)
        {
            checksum += Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                pos_x, 
                (pos_y - diff_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            checksum += Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                (pos_x - diff_x), 
                pos_y, 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
        else
        {
            checksum += Cast_Ray(ref map, 
                (((diff_x == 0) ? 1 : 0) * diff_y + diff_x), 
                (diff_y - ((diff_y == 0) ? 1 : 0) * diff_x), 
                (diff_y + pos_x), 
                (pos_y - diff_x), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            checksum += Cast_Ray(ref map, 
                (diff_x - ((diff_x == 0) ? 1 : 0) * diff_y), 
                (((diff_y == 0) ? 1 : 0) * diff_x + diff_y), 
                (pos_x - diff_y), 
                (diff_x + pos_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }

        return checksum;
    }

    // visible area (raycast)
    int raycast(ref U4_Decompiled.TILE[,] map, int pos_x, int pos_y, ref U4_Decompiled.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled.TILE wrapTile)
    {
        float startTime = Time.realtimeSinceStartup;

        // initialize the checksum
        int checksum = 0;

        if (pos_x < 0 || pos_y < 0 || pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1))
        {
            Debug.Log("start position is outside of source map ( " + pos_x + ", " + pos_y + ")");
            return checksum;
        }

        if (pos_x - offset_x < 0 || pos_y - offset_y < 0 || pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1))
        {
            Debug.Log("offset does not contain the starting position given the dimensions of the destination raycast map " 
                + "position ( " + pos_x + ", " + pos_y + ")" 
                + " offset (" + offset_x + ", " + offset_y + ")" 
                + " dimensions (" + raycastMap.GetLength(0) + ", " + raycastMap.GetLength(1) + ")");
            return checksum;
        }

        // disable raycast outdoors when in the ballon and it is airborne, just copy all the tiles instead
        if ((u4.Party._tile == U4_Decompiled.TILE.BALOON) && (u4.Party.f_1dc == 1))
        {
            for (int y = 0; y < raycastMap.GetLength(1); y++)
            {
                for (int x = 0; x < raycastMap.GetLength(0); x++)
                {
                     raycastMap[x, y] = map[(x + offset_x + map.GetLength(0)) % map.GetLength(0), (y + offset_y + map.GetLength(1)) % map.GetLength(1)];
                }
            }
        }
        else
        {
            // set all visible tiles in the destination raycast map to blank to start
            for (int y = 0; y < raycastMap.GetLength(1); y++)
            {
                for (int x = 0; x < raycastMap.GetLength(0); x++)
                {
                    raycastMap[x, y] = U4_Decompiled.TILE.BLANK;
                }
            }

            // copy the starting position as it is alway visible given the map offset
            U4_Decompiled.TILE currentTile = map[pos_x, pos_y];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = currentTile;
            checksum += (int)currentTile; // add current tile to the checksum

            // cast out recusively from the starting position
            checksum += Cast_Ray(ref map, 0, -1, pos_x, (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP
            checksum += Cast_Ray(ref map, 0, 1, pos_x, (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN
            checksum += Cast_Ray(ref map, -1, 0, (pos_x - 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray LEFT
            checksum += Cast_Ray(ref map, 1, 0, (pos_x + 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray RIGHT
            checksum += Cast_Ray(ref map, 1, 1, (pos_x + 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the RIGHT
            checksum += Cast_Ray(ref map, 1, -1, (pos_x + 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the RIGHT
            checksum += Cast_Ray(ref map, -1, 1, (pos_x - 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the LEFT
            checksum += Cast_Ray(ref map, -1, -1, (pos_x - 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the LEFT

            //Debug.Log("Raycast processing time : " + (Time.realtimeSinceStartup - startTime));
        }

        // return the file tile checksum so we can determine if there were any changes from a previous raycase
        return checksum; 
    }

    // changes in these require redrawing the map
    int lastRaycastPlayer_posx = -1;
    int lastRaycastPlayer_posy = -1;
    int lastRaycastPlayer_f_1dc = -1;
    bool last_door_timer = false;

    // create a temp TILE map array to hold the combat terrains as we load them
    U4_Decompiled.TILE[][,] combatMaps = new U4_Decompiled.TILE[(int)U4_Decompiled.COMBAT_TERRAIN.MAX][,];
    CombatMonsterStartPositions[][] combatMonsterStartPositions = new CombatMonsterStartPositions[(int)U4_Decompiled.COMBAT_TERRAIN.MAX][];
    CombatPartyStartPositions[][] combatPartyStartPositions = new CombatPartyStartPositions[(int)U4_Decompiled.COMBAT_TERRAIN.MAX][];
    private void Start()
    {
        // this object needs to move around so it needs to be above the other which are based on the whole world map
        mainTerrain = new GameObject("Main Terrain");

        // create game object under us to hold these sub categories of things
        terrain = new GameObject("terrain");
        terrain.transform.SetParent(mainTerrain.transform);
        terrain.transform.localPosition = Vector3.zero;
        terrain.transform.localRotation = Quaternion.identity;
        animatedTerrrain = new GameObject("water");
        animatedTerrrain.transform.SetParent(mainTerrain.transform);
        animatedTerrrain.transform.localPosition = Vector3.zero;
        animatedTerrrain.transform.localRotation = Quaternion.identity;
        billboardTerrrain = new GameObject("billboard");
        billboardTerrrain.transform.SetParent(mainTerrain.transform);
        billboardTerrrain.transform.localPosition = Vector3.zero;
        billboardTerrrain.transform.localRotation = Quaternion.identity;

        npcs = new GameObject("npc");
        npcs.transform.SetParent(transform);
        npcs.transform.localPosition = Vector3.zero;
        npcs.transform.localRotation = Quaternion.identity;
        party = new GameObject("party");
        party.transform.localPosition = new Vector3(0.0f, 0.0f, -0.02f);// move it out a bit so it overlaps horses, chests etc.
        party.transform.localRotation = Quaternion.identity;
        party.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        fighters = new GameObject("fighters");
        fighters.transform.SetParent(transform);
        fighters.transform.localPosition = Vector3.zero;
        fighters.transform.localRotation = Quaternion.identity;
        characters = new GameObject("characters");
        characters.transform.SetParent(transform);
        characters.transform.localPosition = Vector3.zero;
        characters.transform.localRotation = Quaternion.identity;
        hits = new GameObject("hits");
        hits.transform.SetParent(transform);
        hits.transform.localPosition = Vector3.zero;
        hits.transform.localRotation = Quaternion.identity;
        moongate = new GameObject("moongate");
        moongate.transform.SetParent(transform);
        moongate.transform.localPosition = Vector3.zero;
        moongate.transform.localRotation = Quaternion.identity;
        dungeon = new GameObject("dungeon");
        dungeon.transform.SetParent(transform);
        dungeon.transform.localPosition = Vector3.zero;
        dungeon.transform.localRotation = Quaternion.identity;
        dungeonMonsters = new GameObject("dungeon monsters");
        dungeonMonsters.transform.SetParent(transform);
        dungeonMonsters.transform.localPosition = Vector3.zero;
        dungeonMonsters.transform.localRotation = Quaternion.identity;

        // initialize the palette and load the tiles
        InitializeEGAPalette();
        InitializeCGAPalette();
        InitializeApple2Palette();
        LoadTilesEGA();
        //LoadTilesCGA();
        //LoadTilesApple2();
        //LoadTilesPNG();

        // fix a tile
        FixMageTile3();

        // expand the tiles
        ExpandTiles();

        // create texture atlas
        CreateLinearTextureAtlas(ref originalTiles);
        CreateSquareTextureAtlas(ref originalTiles);
        CreateExpandedTextureAtlas(ref expandedTiles);

        // get the font
        LoadCharSetEGA();
        //LoadCharSetCGA();
        ImportFontFromTexture(fontAtlas);

        // load the entire world map
        LoadWorldMap();

        //load all settlements
        LoadSettlements();

        // load all dungeons
        LoadDungeons();

        // create the part game object
        CreateParty();

        // Create the combat terrains
        CreateCombatTerrains();

        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled>();

        // initialize hidden map
        hiddenWorldMapGameObject = new GameObject("Hidden World Map");
        CreateMapSubsetPass2(hiddenWorldMapGameObject, ref entireMapTILEs, ref entireMapGameObjects);

        GameObject hiddenSettlementsMaps = new GameObject("Hidden Settlements Maps");
        for (int i = 0; i < (int)SETTLEMENT.MAX; i++)
        {
            GameObject settlementGameObject = new GameObject(((SETTLEMENT)i).ToString());
            settlementGameObject.transform.SetParent(hiddenSettlementsMaps.transform);
            settlementsMapGameObjects[i] = new GameObject[32, 32];
            CreateMapSubsetPass2(settlementGameObject, ref settlementMap[i], ref settlementsMapGameObjects[i], true);
            //CreateMapLabels(settlementGameObject, ref settlementMap[i]);
        }

        u4.StartThread();

        //GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.HYTHLOTH, 4);

        // Some test stuff, commented out for the moment

        //GameObject dungeonsRoomsGameObject = new GameObject("Dungeon Rooms");
        //CreateDungeonRooms(dungeonsRoomsGameObject);

        /*
        //GameObject dungeonsGameObject = new GameObject("Dungeons");
        //CreateDungeons(dungeonsGameObject);
        */

        // create all the dungeons and all the levels, this will take a while so be prepared to wait, but it is cool and worth the wait
        /*
        for (int dungeon = 0; dungeon < (int)DUNGEONS.MAX; dungeon++)
        {
            for (int level = 0; level < 8; level++)
            {
                GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel((DUNGEONS)dungeon, level);
                dungeonExpandedLevelGameObject.transform.position = new Vector3(dungeon * 100, -level * 10, 0);
            }
        }
        */

        //GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.DESPISE, 0);

        //GameObject dr = CreateDungeonRoom(ref dungeons[(int)DUNGEONS.WRONG].dungeonRooms[5]);

        /*
        GameObject wedge = CreateWedge();
        Renderer renderer = wedge.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE4];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        GameObject wedge2 = CreateWedge();
        renderer = wedge2.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE1];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
        
        GameObject wedge3 = CreateWedge();
        renderer = wedge3.GetComponent<Renderer>();
        renderer.material.mainTexture = expandedTiles[(int)U4_Decompiled.TILE.WOOD_FLOOR];
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
    */
    }


    void AnimateFlags()
    {
        //animate flags
        // TODO there is probably a better way to do this than mucking around
        // with the 2d texture pixels like the original game did but it will work for now.
        const int flagSpeed = 25;

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.CASTLE_ENTRANCE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 4;
            const int swap_y2 = 5;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.TOWN;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 10;
            const int swap_x2 = 12;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.CASTLE;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_WEST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_EAST;

            int x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth;
            int y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight;

            Color color1 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2));
            Color color2 = combinedExpandedTexture.GetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x1, expandedTileHeight - 1 - (y + swap_y2), color2);
            color1 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2));
            color2 = combinedExpandedTexture.GetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1));
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y1), color1);
            combinedExpandedTexture.SetPixel(x + swap_x2, expandedTileHeight - 1 - (y + swap_y2), color2);
            combinedExpandedTexture.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6;
            const int swap_x2 = 8;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_WEST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7;
            const int swap_x2 = 9;
            const int swap_y1 = 2;
            const int swap_y2 = 3;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_EAST;
            Texture2D tex = originalTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, originalTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, originalTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, originalTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 6 + TILE_BORDER_SIZE;
            const int swap_x2 = 8 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_WEST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }
        if (Random.Range(0, 100) <= flagSpeed)
        {
            const int swap_x1 = 7 + TILE_BORDER_SIZE;
            const int swap_x2 = 9 + TILE_BORDER_SIZE;
            const int swap_y1 = 2 + TILE_BORDER_SIZE;
            const int swap_y2 = 3 + TILE_BORDER_SIZE;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.SHIP_EAST;
            Texture2D tex = expandedTiles[(int)tileIndex];
            Color color1 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y2);
            Color color2 = tex.GetPixel(swap_x1, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x1, expandedTileHeight - 1 - swap_y2, color2);
            color1 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y2);
            color2 = tex.GetPixel(swap_x2, expandedTileHeight - 1 - swap_y1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y1, color1);
            tex.SetPixel(swap_x2, expandedTileHeight - 1 - swap_y2, color2);
            tex.Apply();
        }

        //if (Random.Range(0, 100) <= flagSpeed)
        {
            const int x1 = 3;
            const int x2 = 9;
            const int y1 = 9;
            const int y2 = 16;
            U4_Decompiled.TILE tileIndex = U4_Decompiled.TILE.COOKING_FIRE;

            int offset_x = ((int)tileIndex % textureExpandedAtlasPowerOf2) * expandedTileWidth + TILE_BORDER_SIZE;
            int offset_y = ((int)tileIndex / textureExpandedAtlasPowerOf2) * expandedTileHeight + TILE_BORDER_SIZE;


            Color alpha = new Color(0, 0, 0, 0);
            //EGAColorPalette[(int)EGA_COLOR.BLACK];

            for (int y = y1; y <= y2; y++)
            {
                for (int x = x1; x <= x2; x++)
                {
                    Color color = combinedExpandedTexture.GetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y);
                    if ((color == EGAColorPalette[(int)EGA_COLOR.RED]) || (color == alpha))
                    {
                        if (Random.Range(0, 100) <= 50)
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, EGAColorPalette[(int)EGA_COLOR.RED]);
                        }
                        else
                        {
                            combinedExpandedTexture.SetPixel(offset_x + x, offset_y + expandedTileHeight - 1 - y, alpha);
                        }
                    }
                }
            }
            combinedExpandedTexture.Apply();
        }
    }

    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;

    // used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f;

    int lastChecksum = -1;
    int currentChecksum = 0;
    GameObject hiddenWorldMapGameObject;

    bool justChangedModes = true;

    // Update is called once per frame
    void Update()
    {
        // update the timer
        flagTimer += Time.deltaTime;

        // only update periodically
        if (flagTimer > flagTimerExpired)
        {
            // reset the expired timer
            flagTimer -= flagTimerExpired;
            flagTimerExpired = flagTimerPeriod;
            if (textureExpandedAtlasPowerOf2 != 0)
            {
                AnimateFlags();
            }
        }

        // update the timer
        timer += Time.deltaTime;

        // only update periodically
        if (timer > timerExpired)
        {
            // reset the expired timer
            timer -= timerExpired;
            timerExpired = timerPeriod;

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.CITIZEN_WORD)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkCitizen.SetActive(true);

                bool keyword1found = false;
                bool keyword2found = false;

                SETTLEMENT settlement;
                if ((u4.Party._loc == U4_Decompiled.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled.TILE.LADDER_UP))
                {
                    settlement = SETTLEMENT.LCB_1;
                }
                else
                {
                    settlement = (SETTLEMENT)u4.Party._loc;
                }

                foreach (string word in u4.wordList)
                {
                    // only add the special keywords if we already know them
                    // TODO don't need to do this so often, only when we get new text
                    // TODO need to clear npcTalkIndex when switching levels or settlements as the index might not be valid for the other location
                    if (word.Length >= 4)
                    {
                        string lower = word.ToLower();
                        //Debug.Log(lower);
                        string sub = lower.Substring(0, 4);
                        //Debug.Log(sub);
                        if (sub ==
                            settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)NPC_STRING_INDEX.KEYWORD1].ToLower().Substring(0, 4))
                        {
                            u4.keyword1 = lower;
                            lower = char.ToUpper(lower[0]) + lower.Substring(1, lower.Length - 1);
                            keyword1ButtonText.text = lower;
                            keyword1found = true;
                            keyword1Button.SetActive(true);
                        }
                        if (sub ==
                            settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)NPC_STRING_INDEX.KEYWORD2].ToLower().Substring(0, 4))
                        {
                            u4.keyword2 = lower;
                            lower = char.ToUpper(lower[0]) + lower.Substring(1, lower.Length - 1);
                            keyword2ButtonText.text = lower;
                            keyword2found = true;
                            keyword2Button.SetActive(true);
                        }
                    }

                    if (keyword1found == false)
                    {
                        u4.keyword1 = "";
                        keyword1ButtonText.text = "";
                        keyword1Button.SetActive(false);
                    }

                    if (keyword2found == false)
                    {
                        u4.keyword2 = "";
                        keyword2ButtonText.text = "";
                        keyword2Button.SetActive(false);
                    }
                }
            }
            else
            {
                TalkCitizen.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_YES_NO)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkYN.SetActive(true);
            }
            else
            {
                TalkYN.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_YES_NO_WORD)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkYesNo.SetActive(true);
            }
            else
            {
                TalkYesNo.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_HEALER)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkHealer.SetActive(true);
            }
            else
            {
                TalkHealer.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_CONTINUE)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkContinue.SetActive(true);
            }
            else
            {
                TalkContinue.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.MAIN_LOOP)
            {
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionMainLoop.SetActive(true);
            }
            else
            {
                ActionMainLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.DUNGEON_LOOP)
            {
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionDungeonLoop.SetActive(true);
            }
            else
            {
                ActionDungeonLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.HAWKWIND_WORD)
            {
                // TODO: need to filter buttons like citizen talk with word list
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkHawWind.SetActive(true);
            }
            else
            {
                TalkHawWind.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_FOOD_OR_ALE)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkFoodAle.SetActive(true);
            }
            else
            {
                TalkFoodAle.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_BUY_SELL)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkBuySell.SetActive(true);
            }
            else
            {
                TalkBuySell.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_ASK_CHARACTER_NUMBER)
            {
                Talk.SetActive(false);
                Action.SetActive(false);
                TalkPartyCharacter.SetActive(true);
            }
            else
            {
                TalkPartyCharacter.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_DIRECTION)
            {
                Talk.SetActive(false);
                Action.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.DELAY)
            {
                Talk.SetActive(false);
                Action.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.PUB_WORD)
            {
                /* TODO search for these words
                "black stone",
	            "sextant",
	            "white stone",
	            "mandrake",
	            "skull",
	            "nightshade",
	            "mandrake root"
                "nothing"
                */
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkPubWord.SetActive(true);
            }
            else
            {
                TalkPubWord.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.MANTRA_WORD)
            {
                /*
                "ahm", Honesty
                "mu", Compassion
                "ra", Valor
                "beh", Justice
                "cah", Sacrifice
                "summ", Honor
                "om", Spirituality
                "lum" Humility
                */

                Talk.SetActive(true);
                Action.SetActive(false);
                TalkMantras.SetActive(true);
            }
            else
            {
                TalkMantras.SetActive(false);
            }
            
            if (u4.inputMode == U4_Decompiled.INPUT_MODE.LOAD_BRITISH_WORD)
            {
                /*
                char* D_6FF0[28] = {
                "bye",
                "help",
                "health",
                "name",
                "look",
                "job",
                "truth",
                "love",
                "courage",
                "honesty",
                "compassion",
                "valor",
                "justice",
                "sacrifice",
                "honor",
                "spirituality",
                "humility",
                "pride",
                "avatar",
                "quest",
                "britannia",
                "ankh",
                "abyss",
                "mondain",
                "minax",
                "exodus",
                "virtue",
                ""
                };
                */

                Talk.SetActive(true);
                Action.SetActive(false);
                TalkLordBritish.SetActive(true);
            }
            else
            {
                TalkLordBritish.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.VIRTUE_WORD)
            {
                /*
                Honesty
                Compassion
                Valor
                Justice
                Sacrifice
                Honor
                Spirituality
                Humility
                */

                Talk.SetActive(true);
                Action.SetActive(false);
                TalkVirtue.SetActive(true);
            }
            else
            {
                TalkVirtue.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.USE_ITEM_WORD)
            {
                /*
                "stone"
                "stones"
                "bell"
                "book"
                "candle",
                "key"
                "keys"
                "horn"
                "wheel"
                "skull"
                */

                Talk.SetActive(true);
                Action.SetActive(false);
                TalkUseItem.SetActive(true);
            }
            else
            {
                TalkUseItem.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.USE_STONE_COLOR_WORD)
            {
                /*
                "Blue",
                "Yellow",
                "Red",
                "Green",
                "Orange",
                "Purple",
                "White",
                "Black"
                */

                Talk.SetActive(true);
                Action.SetActive(false);
                TalkColors.SetActive(true);
            }
            else
            {
                TalkColors.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_NUMBER_INPUT_2_DIGITS)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk2DigitInput.SetActive(true);
            }
            else
            {
                Talk2DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_NUMBER_INPUT_3_DIGITS)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk3DigitInput.SetActive(true);
            }
            else
            {
                Talk3DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.COMBAT_LOOP)
            {
                Action.SetActive(true);
                Talk.SetActive(false);
                ActionCombatLoop.SetActive(true);
            }
            else
            {
                ActionCombatLoop.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_WEAPON)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkWeapon.SetActive(true);
            }
            else
            {
                TalkWeapon.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_ARMOR)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkArmor.SetActive(true);
            }
            else
            {
                TalkArmor.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_GUILD)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkGuild.SetActive(true);
            }
            else
            {
                TalkGuild.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_REAGENT)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkReagents.SetActive(true);
            }
            else
            {
                TalkReagents.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_SPELL)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkSpells.SetActive(true);
            }
            else
            {
                TalkSpells.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_NUMBER_INPUT_1_DIGITS)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                Talk1DigitInput.SetActive(true);
            }
            else
            {
                Talk1DigitInput.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkEnergy.SetActive(true);
            }
            else
            {
                TalkEnergy.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_TELESCOPE)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkTelescope.SetActive(true);
            }
            else
            {
                TalkTelescope.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.ASK_LETTER_PHASE)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkPhase.SetActive(true);
            }
            else
            {
                TalkPhase.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_NUMBER_INPUT_0_1_2_3)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkDigit0123.SetActive(true);
            }
            else
            {
                TalkDigit0123.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.END_GAME_INFINITY_WORD)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkInfinity.SetActive(true);
            }
            else
            {
                TalkInfinity.SetActive(false);
            }

            if (u4.inputMode == U4_Decompiled.INPUT_MODE.END_GAME_VERAMOCOR_WORD)
            {
                Talk.SetActive(true);
                Action.SetActive(false);
                TalkVeramocor.SetActive(true);
            }
            else
            {
                TalkVeramocor.SetActive(false);
            }

            // disable this for now
            // TODO if this is going to be an actual input panel it needs to support
            // rotation and direction like the controller do
            if (u4.inputMode == U4_Decompiled.INPUT_MODE.GENERAL_DIRECTION)
            {
                Talk.SetActive(false);
                Action.SetActive(false);
                TalkDirection.SetActive(false);
            }
            else
            {
                TalkDirection.SetActive(false);
            }

            if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                followWorld(partyGameObject);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(true);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatically enter things when you are on an enterable tile unless just leaving or flying in the balloon
                if ((justChangedModes == false) && (u4.Party.f_1dc == 0) &&  ((u4.current_tile == U4_Decompiled.TILE.CASTLE_ENTRANCE) ||
                        (u4.current_tile == U4_Decompiled.TILE.CASTLE) ||
                        (u4.current_tile == U4_Decompiled.TILE.TOWN) ||
                        (u4.current_tile == U4_Decompiled.TILE.VILLAGE) ||
                        (u4.current_tile == U4_Decompiled.TILE.DUNGEON) ||
                        (u4.current_tile == U4_Decompiled.TILE.RUINS) ||
                        (u4.current_tile == U4_Decompiled.TILE.SHRINE)))
                {
                    u4.CommandEnter();
                    justChangedModes = true;
                }

                // automatically board horse, ship and balloon
                if ((u4.current_tile == U4_Decompiled.TILE.HORSE_EAST) ||
                    (u4.current_tile == U4_Decompiled.TILE.HORSE_EAST) ||
                    (u4.current_tile == U4_Decompiled.TILE.SHIP_EAST) ||
                    (u4.current_tile == U4_Decompiled.TILE.SHIP_NORTH) ||
                    (u4.current_tile == U4_Decompiled.TILE.SHIP_WEST) ||
                    (u4.current_tile == U4_Decompiled.TILE.SHIP_SOUTH) ||
                    (u4.current_tile == U4_Decompiled.TILE.BALOON))
                {
                    u4.CommandBoard();
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                followWorld(partyGameObject);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == U4_Decompiled.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled.TILE.LADDER_DOWN))
                {
                    u4.CommandKlimb();
                }

                // automatic Klimb and Descend ladders
                if ((u4.current_tile == U4_Decompiled.TILE.LADDER_DOWN) &&
                    (lastCurrentTile != U4_Decompiled.TILE.LADDER_UP) &&
                    (lastCurrentTile != U4_Decompiled.TILE.LADDER_DOWN))
                {
                    u4.CommandDecsend();
                }

                // update last tile so we don't get stuck in a loop
                lastCurrentTile = u4.current_tile;
            }
            else if (u4.current_mode == U4_Decompiled.MODE.COMBAT)
            {
                if ((u4.Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                {
                    AddFighters(u4.Fighters, u4.Combat1, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddCharacters(u4.Combat2, u4.Party, u4.Fighters, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddHits(u4.currentHits, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    AddActiveCharacter(u4.currentActiveCharacter, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                    followWorld(activeCharacter);
                    terrain.SetActive(false);
                    animatedTerrrain.SetActive(false);
                    billboardTerrrain.SetActive(false);
                    fighters.SetActive(true);
                    characters.SetActive(true);
                    npcs.SetActive(false);
                    party.SetActive(false);
                    moongate.SetActive(false);
                    dungeonMonsters.SetActive(false);
                    // check if we have the dungeon already created, create it if not
                    DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled.LOCATIONS.DUNGEONS);
                    if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                    {
                        Destroy(dungeon);
                        dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                    }
                    dungeon.SetActive(true);

                    for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    AddFighters(u4.Fighters, u4.Combat1);
                    AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                    AddHits(u4.currentHits);
                    AddActiveCharacter(u4.currentActiveCharacter);
                    followWorld(activeCharacter);
                    terrain.SetActive(false);
                    animatedTerrrain.SetActive(false);
                    billboardTerrrain.SetActive(false);
                    fighters.SetActive(true);
                    characters.SetActive(true);
                    npcs.SetActive(false);
                    party.SetActive(false);
                    moongate.SetActive(false);
                    dungeon.SetActive(false);
                    dungeonMonsters.SetActive(false);
                    
                    int currentCombatTerrain = (int)Convert_Tile_to_Combat_Terrian(u4.current_tile);

                    for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                    {
                        if (i == currentCombatTerrain)
                        {
                            CombatTerrains[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            CombatTerrains[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.COMBAT_ROOM) /* this is a dungeon room */
            {
                AddFighters(u4.Fighters, u4.Combat1, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddHits(u4.currentHits, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                AddActiveCharacter(u4.currentActiveCharacter, u4.Party._x * 11, -255 + (7 - u4.Party._y + 1) * 11 - 1);
                followWorld(activeCharacter);
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(true);
                characters.SetActive(true);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeonMonsters.SetActive(false);
                // check if we have the dungeon already created, create it if not
                DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled.LOCATIONS.DUNGEONS);
                if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    Destroy(dungeon);
                    dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                }
                dungeon.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.DUNGEON)
            {
                AddNPCs(u4._npc);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                AddDungeonMapMonsters();
                followWorld(partyGameObject);
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(false);
                party.SetActive(true);
                moongate.SetActive(false);

                // check if we have the dungeon already created, create it if not
                DUNGEONS dun = (DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled.LOCATIONS.DUNGEONS);
                if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
                {
                    // not the right dungeon, create a new dungeon
                    Destroy(dungeon);
                    dungeon = CreateDungeonExpandedLevel(dun, u4.Party._z);
                }

                if (u4.Party.f_1dc > 0) // torch active
                {
                    dungeon.SetActive(true);
                    dungeonMonsters.SetActive(true);
                }
                else
                {
                    dungeon.SetActive(false);
                    dungeonMonsters.SetActive(false);
                }

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.SHRINE)
            {
                AddFighters(u4.Fighters, u4.Combat1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                
                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    if (i == (int)U4_Decompiled.COMBAT_TERRAIN.SHRINE)
                    {
                        CombatTerrains[i].gameObject.SetActive(true); 
                        followWorld(CombatTerrains[i].gameObject);
                    }
                    else
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
            } 
            else if (u4.current_mode == U4_Decompiled.MODE.COMBAT_CAMP /* TODO: this could be the inn or shop or camp need to figure out which */ )
            {
                AddFighters(u4.Fighters, u4.Combat1);
                AddCharacters(u4.Combat2, u4.Party, u4.Fighters);
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);

                terrain.SetActive(false);
                animatedTerrrain.SetActive(false);
                billboardTerrrain.SetActive(false);
                fighters.SetActive(true);
                characters.SetActive(true);
                npcs.SetActive(false);
                party.SetActive(false);
                moongate.SetActive(false);
                dungeon.SetActive(false);
                dungeonMonsters.SetActive(false);

                int currentCombatTerrain;

                // need to special case the combat when in the inn and in combat camp mode outside or in dungeon
                if (u4.current_tile == U4_Decompiled.TILE.BRICK_FLOOR)
                {
                    currentCombatTerrain = (int)U4_Decompiled.COMBAT_TERRAIN.INN;
                }
                else
                {
                    currentCombatTerrain = (int)U4_Decompiled.COMBAT_TERRAIN.CAMP;
                }

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    if (i == currentCombatTerrain)
                    {
                        CombatTerrains[i].gameObject.SetActive(true);
                        followWorld(CombatTerrains[i].gameObject);
                    }
                    else
                    {
                        CombatTerrains[i].gameObject.SetActive(false);
                    }
                }
            }

            if ((party != null) && (originalTiles != null))
            {
                // set the party tile, person, horse, ballon, ship, etc.
                Renderer renderer = party.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    party.GetComponentInChildren<Renderer>().material.mainTexture = expandedTiles[(int)u4.Party._tile];
                    party.name = u4.Party._tile.ToString();

                    if ((u4.Party._tile == U4_Decompiled.TILE.BALOON) && (u4.Party.f_1dc == 1))
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 1, party.transform.position.z);
                    }
                    else
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 0, party.transform.position.z);
                    }
                }
            }
        }

        // make party a billboard
        Transform look = Camera.main.transform;
        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
        partyGameObject.transform.LookAt(look.transform);
        Vector3 rot = partyGameObject.transform.eulerAngles;
        partyGameObject.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

        U4_Decompiled.MODE currentMode = u4.current_mode;

        // we've moved, regenerate the raycast, TODO NPCs can also affect the raycast when moving, need to check them also or redo raycast more often
        if ((u4.Party._x != lastRaycastPlayer_posx) || (u4.Party._y != lastRaycastPlayer_posy) || // player moved
            (u4.Party.f_1dc != lastRaycastPlayer_f_1dc) || // balloon flying or grounded or dungeon torch active
            ((u4.open_door_timer > 0) != last_door_timer)) // door has opened or closed
        {
            Vector3 location = Vector3.zero;

            justChangedModes = false;

            // update the last raycast position
            lastRaycastPlayer_posx = u4.Party._x;
            lastRaycastPlayer_posy = u4.Party._y;
            lastRaycastPlayer_f_1dc = u4.Party.f_1dc; // flying in the balloon or not or dungeon torch active
            last_door_timer = (u4.open_door_timer > 0);

            if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
            {
                // generate a new raycast and get a checksum
                currentChecksum = raycast(ref entireMapTILEs,
                    u4.Party._x, 
                    u4.Party._y,
                    ref raycastOutdoorMap, 
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1) ) ,
                    ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) , 
                    U4_Decompiled.TILE.BLANK);
                location = new Vector3(
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2) - 1) , 0, 
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) ) - raycastOutdoorMap.GetLength(1));
            }
            else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
            {
                // generate a new raycast based on game engine map and get a checksum
                currentChecksum = raycast(ref u4.tMap32x32, 
                    u4.Party._x, 
                    u4.Party._y, 
                    ref raycastSettlementMap, 
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ), 
                    ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) ), 
                    U4_Decompiled.TILE.GRASS);
                location = new Vector3(
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ) , 0,
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) )  - raycastSettlementMap.GetLength(1));
            }

            // if last checksum does not match we need to regenerate the scene because the raycast is different
            // TODO also if we are sitting on a blank tile we are probably surrounded by something like trees so we need to regenerate
            // check if any of the surrounding tiles are emtpy also
            //if (false) //(lastChecksum != currentChecksum)
            {
                // save the checksum
                lastChecksum = currentChecksum;

                // create the game object children with meshes and textures
                if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
                {
                    Combine3(mainTerrain, 
                        ref raycastOutdoorMap, 
                        u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1, 
                        u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1, 
                        ref entireMapGameObjects,
                        false, 
                        TextureFormat.RGBA32, 
                        true);
                    
                    location = Vector3.zero;
                    //terrain.transform.eulerAngles = Vector3.zero; // reset back to zero, TODO: figure out who is modifiying this
                }
                else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
                {
                    
                    SETTLEMENT settlement;
                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((u4.Party._loc == U4_Decompiled.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled.TILE.LADDER_UP))
                    {
                        settlement = SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (SETTLEMENT)u4.Party._loc;
                    }

                    Combine3(mainTerrain, 
                        ref raycastSettlementMap, 
                        u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1, 
                        u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1, 
                        ref settlementsMapGameObjects[(int)settlement],
                        false,
                        TextureFormat.RGBA32,
                        false);

                    //CreateMapLabels(mainTerrain, ref raycastSettlementMap);

                    //location = Vector3.zero;
                    //terrain.transform.eulerAngles = Vector3.zero; // reset back to zero, TODO: figure out who is modifiying this

                    location = new Vector3(0, 0, 224);
                    
                    /*
                    CreateMap(mainTerrain, raycastSettlementMap);
                    location = new Vector3(
                        ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ) , 0,
                        entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) )  - raycastSettlementMap.GetLength(1));
                    */
                }

                // Position the map in place
                mainTerrain.transform.position = location;

                // rotate map into place
                mainTerrain.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

                /* used to manually create meshes
                if (once)
                {
                    if (convertMe)
                    {
                        Combine(convertMe, false, TextureFormat.RGBA32, false);

                        MeshFilter meshFilter = convertMe.GetComponent<MeshFilter>();
                        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                        {
                            Debug.Log("new Vector3(" + meshFilter.mesh.vertices[i].x + "f, " + meshFilter.mesh.vertices[i].y + "f, " + meshFilter.mesh.vertices[i].z + "f),");
                        }
                        for (int i = 0; i < meshFilter.mesh.triangles.Length; i += 3)
                        {
                            Debug.Log(meshFilter.mesh.triangles[i] + ", " + meshFilter.mesh.triangles[i + 1] + ", " + meshFilter.mesh.triangles[i + 2] + ",");
                        }
                        for (int i = 0; i < meshFilter.mesh.uv.Length; i++)
                        {
                            Debug.Log("new Vector2(" + meshFilter.mesh.uv[i].x + "f, " + meshFilter.mesh.uv[i].y + "f),");
                        }

                        once = false;
                    }
                }
                */
            }
        }

        // keep the party game object in sync with the game
        if (partyGameObject)
        {
            if ((u4.current_mode == U4_Decompiled.MODE.OUTDOORS) || (u4.current_mode == U4_Decompiled.MODE.BUILDING) || (u4.current_mode == U4_Decompiled.MODE.COMBAT_CAMP))
            {
                partyGameObject.transform.localPosition = new Vector3(u4.Party._x, 255 - u4.Party._y, 0);
                if (Camera.main.transform.eulerAngles.y != 0)
                {
                    if (rotateTransform)
                    {
                        if (u4.surface_party_direction == U4_Decompiled.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.surface_party_direction == U4_Decompiled.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.surface_party_direction == U4_Decompiled.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.surface_party_direction == U4_Decompiled.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                        }
                    }
                    else
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                    }
                }
                if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                {
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.COMBAT)
            {
                if ((u4.Party._loc >= U4_Decompiled.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                {
                    //partyGameObject.transform.localPosition = new Vector3(Party._x * 11 + 5, (7 - Party._y) * 11 + 5, 0);
                    if (Camera.main.transform.eulerAngles.y != 0)
                    {
                        //rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);

                        // if we are going to do rotation then we need to adjust the directional controls when in combat in the dungeon also
                        if (rotateTransform)
                        {
                            if (u4.Party._dir == U4_Decompiled.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                            }
                        }
                    }
                    if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                    {
                        Camera.main.clearFlags = CameraClearFlags.SolidColor;
                        Camera.main.backgroundColor = Color.black;
                    }
                }
                else
                {
                    //partyGameObject.transform.localPosition = new Vector3(Party._x, 255 - Party._y, 0);
                    if (Camera.main.transform.eulerAngles.y != 0)
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                    }
                    if (Camera.main.clearFlags != CameraClearFlags.Skybox)
                    {
                        Camera.main.clearFlags = CameraClearFlags.Skybox;
                    }
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.COMBAT_ROOM)
            {
                //partyGameObject.transform.localPosition = new Vector3(Party._x * 11 + 5, (7 - Party._y) * 11 + 5, 0);
                if (Camera.main.transform.eulerAngles.y != 0)
                {
                    //rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);

                    if (rotateTransform)
                    {
                        if (u4.Party._dir == U4_Decompiled.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                        {
                            rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                        }
                    }
                }
                if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = Color.black;
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.DUNGEON)
            {
                if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
                {
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    Camera.main.backgroundColor = Color.black;
                }
                partyGameObject.transform.localPosition = new Vector3(u4.Party._x * 11 + 5, (7 - u4.Party._y) * 11 + 5, 0);
                if (rotateTransform)
                {
                    if (u4.Party._dir == U4_Decompiled.DIRECTION.WEST && Camera.main.transform.eulerAngles.y != 270)
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 270, Camera.main.transform.eulerAngles.z);
                    }
                    else if (u4.Party._dir == U4_Decompiled.DIRECTION.NORTH && Camera.main.transform.eulerAngles.y != 0)
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 0, Camera.main.transform.eulerAngles.z);
                    }
                    else if (u4.Party._dir == U4_Decompiled.DIRECTION.EAST && Camera.main.transform.eulerAngles.y != 90)
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 90, Camera.main.transform.eulerAngles.z);
                    }
                    else if (u4.Party._dir == U4_Decompiled.DIRECTION.SOUTH && Camera.main.transform.eulerAngles.y != 180)
                    {
                        rotateTransform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, 180, Camera.main.transform.eulerAngles.z);
                    }
                }
            }

            if (u4.gameText != null && GameText != null)
            {
                GameText.GetComponent<UnityEngine.UI.Text>().text = u4.gameText;
            }

            if (u4.current_mode == U4_Decompiled.MODE.DUNGEON)
            {
                windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Level" + (char)(0x12) + (char)(u4.Party._z + '1') + (char)(0x11);
            }
            else
            {
                windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Wind" + (char)(0x12);

                switch (u4.WindDir)
                {
                    case U4_Decompiled.DIRECTION.NORTH:
                        windDirection.GetComponent<UnityEngine.UI.Text>().text += "North" + (char)(0x11);
                        break;
                    case U4_Decompiled.DIRECTION.SOUTH:
                        windDirection.GetComponent<UnityEngine.UI.Text>().text += "South" + (char)(0x11);
                        break;
                    case U4_Decompiled.DIRECTION.EAST:
                        windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "East" + (char)(0x11);
                        break;
                    case U4_Decompiled.DIRECTION.WEST:
                        windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "West" + (char)(0x11);
                        break;
                }
            }
            moons.GetComponent<UnityEngine.UI.Text>().text = ""+ (char)(0x10)+ (char)(((u4.Party._trammel - 1) & 7) + 0x14) + (char)(0x12) + (char)(((u4.Party._felucca - 1) & 7) + 0x14) + (char)(0x11);

            //trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.Party._trammel * (360f / 8f), 0f);
            //feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.Party._felucca * (360f / 8f), 0f);
            //trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.D_1665 * (360f / 256f), 0f);
            //feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, 180f - (float)u4.D_1666 * (360f / 256f), 0f);

            trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(trammelLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1665 * (360f / 256f), Time.deltaTime), 0f);
            feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(feluccaLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1666 * (360f / 256f), Time.deltaTime), 0f);
            //sunLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(sunLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1666 * (360f / 256f), Time.deltaTime), 0f);

            statsMagicStatus.GetComponent<UnityEngine.UI.Text>().text = "" + (char)(u4.spell_sta);
            statsFood.GetComponent<UnityEngine.UI.Text>().text = "F:" + (int)(u4.Party._food / 100);
            if ((u4.Party._tile == U4_Decompiled.TILE.SHIP_EAST)  || 
                (u4.Party._tile == U4_Decompiled.TILE.SHIP_WEST) ||
                (u4.Party._tile == U4_Decompiled.TILE.SHIP_NORTH) ||
                (u4.Party._tile == U4_Decompiled.TILE.SHIP_SOUTH))
            {
                statsGold.GetComponent<UnityEngine.UI.Text>().text = "SHIP:" + u4.Party._ship;
            }
            else
            {
                statsGold.GetComponent<UnityEngine.UI.Text>().text = "G:" + u4.Party._gold;
            }
            statsHP.GetComponent<UnityEngine.UI.Text>().text = "";
            statsNames.GetComponent<UnityEngine.UI.Text>().text = "";
            for (int i = 0; i < u4.Party.f_1d8; i++)
            {
                if (u4.Party.chara[i].highlight)
                {
                    statsHP.GetComponent<UnityEngine.UI.Text>().text += highlight("        " + u4.Party.chara[i].hitPoint + (char)(u4.Party.chara[i].state) + "\n");
                    statsNames.GetComponent<UnityEngine.UI.Text>().text += highlight((i + 1) + "-" + u4.Party.chara[i].name + "        \n");
                }
                else
                {
                    statsHP.GetComponent<UnityEngine.UI.Text>().text += u4.Party.chara[i].hitPoint;
                    statsHP.GetComponent<UnityEngine.UI.Text>().text += (char)(u4.Party.chara[i].state);
                    statsHP.GetComponent<UnityEngine.UI.Text>().text += "\n";
                    statsNames.GetComponent<UnityEngine.UI.Text>().text += (i + 1) + "-" + u4.Party.chara[i].name + "\n";
                }
            }

            if (currentMode != lastMode)
            {
                justChangedModes = true;
                lastMode = currentMode;
            }
        }
    }

    // The font is setup so if the high bit is set it will use the inverse highlighted text
    // this function will set the high bit on all the characters in a string so when displayed with the font
    // it will be highlighted
    public string highlight(string s)
    {
        string temp = "";
        for (int j = 0; j < s.Length; j++)
        {
            char c = s[j];

            if (c == '\n')
            {
                temp += '\n';
            }
            else if (c == ' ')
            {
                temp += (char)(0x12 + 0x80);
            }
            else
            {
                temp += (char)(s[j] + 0x80);
            }
        }

        return temp;
    }

    public GameObject statsNames;
    public GameObject statsHP;
    public GameObject statsGold;
    public GameObject statsFood;
    public GameObject statsMagicStatus;
    public GameObject windDirection;
    public GameObject moons;
    public GameObject trammelLight;
    public GameObject feluccaLight;
    public GameObject sunLight;

    public U4_Decompiled.MODE lastMode = (U4_Decompiled.MODE )(-1);
    public Transform rotateTransform;
    //public GameObject convertMe;


    public Texture2D fontAtlas;
    public Font myFont;
    public string charsetEGAFilepath = "/u4/CHARSET.EGA";
    public string charsetCGAFilepath = "/u4/CHARSET.CGA";

    void LoadCharSetEGA()
    {
        Color alpha = new Color(0, 0, 0, 0);

        if (!System.IO.File.Exists(Application.persistentDataPath + charsetEGAFilepath))
        {
            Debug.Log("Could not find EGA charset file " + Application.persistentDataPath + charsetEGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + charsetEGAFilepath);

        if (fileData.Length != 8 * 1024)
        {
            Debug.Log("EGA charset file incorrect length " + fileData.Length);
            return;
        }

        // create a texture for this tile
        fontAtlas = new Texture2D(8 * 16, 8 * 8 * 2, TextureFormat.RGBA32, false);

        // we want pixles not fuzzy images
        fontAtlas.filterMode = FilterMode.Point;

        // use and index to walk through the file
        int fileIndex = 0;

        // there are 128 characters in the file,
        // we will lay them out on a 16x8 texture atlas
        // so we can convert them into a font below
        for (int character = 0; character < 128; character++)
        {
            // manually go through the data and set the (x,y) pixels to the character based on the input file using the EGA color palette
            for (int height = 0; height < 8; height++)
            {
                for (int width = 0; width < 8; /* width incremented below because we need to do nibbles */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[fileIndex] >> 4;
                    Color color = EGAColorPalette[colorIndex];

                    // use black as alpha channel
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height + 64, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height + 64, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = EGAColorPalette[colorIndex];
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height+ 64, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height + 64, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // need inverse characters also for highlighting
        // so we will add those to the end

        // reset the file index to generate inverse chars
        fileIndex = 0;

        for (int character = 0; character < 128; character++)
        {
            // manually go through the data and set the (x,y) pixels to the character based on the input file using the EGA color palette
            for (int height = 0; height < 8; height++)
            {
                for (int width = 0; width < 8; /* width incremented below because we need to do nibbles */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[fileIndex] >> 4;
                    Color color = EGAColorPalette[15 - colorIndex]; // flip the colors

                    // use black as alpha channel
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[fileIndex] & 0xf;
                    color = EGAColorPalette[15 - colorIndex];
                    //if (colorIndex == 0)
                    //{
                    //    fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, alpha);
                    //}
                    //else
                    {
                        fontAtlas.SetPixel((character % 16) * 8 + width++, (7 - (character / 16)) * 8 + 7 - height, color);
                    }

                    // go to the next byte in the file
                    fileIndex++;
                }
            }
        }

        // Actually apply all previous SetPixel and SetPixels changes from above
        fontAtlas.Apply();
    }

#if DISABLED
// TODO use the CGA tileset loading code as a reference to load the CGA font
    void LoadCharsetCGA()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + charsetCGAFilepath))
        {
            Debug.Log("Could not find CGA tiles file " + Application.persistentDataPath + charsetCGAFilepath);
            return;
        }

        // read the file
        byte[] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + charsetCGAFilepath);

        if (fileData.Length != 16 * 1024)
        {
            Debug.Log("CGA Tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        tiles = new Texture2D(16*8, 16, TextureFormat.RGBA32, false);

        // we want pixles not fuzzy images
        currentChacter.filterMode = FilterMode.Point;

        // use and index to walk through the file
        int index = 0;

        // there are 256 tiles in the file
        for (int tile = 0; tile < 256; tile++)
        {

            // assign this texture to the tile array index
            tiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the EGA color palette
            for (int height = 0; height < currentTile.height; height += 2)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    int colorIndex = (fileData[index + 0x20] & 0xC0) >> 6;
                    Color color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0xC0) >> 6;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x30) >> 4;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x30) >> 4;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x0C) >> 2;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x0C) >> 2;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    colorIndex = (fileData[index + 0x20] & 0x03) >> 0;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width, currentTile.height - height - 2, color);

                    colorIndex = (fileData[index] & 0x03) >> 0;
                    color = EGAColorPalette[colorIndex];
                    currentTile.SetPixel(width++, currentTile.height - height - 1, color);

                    // go to the next byte in the file
                    index++;
                }
            }

            // skip ahead
            index += 0x20;
        }

        // Actually apply all previous SetPixel and SetPixels changes from above
        currentTile.Apply();
    }
#endif

    public void ImportFontFromTexture(Texture2D texture)
    {
        float texW = texture.width;
        float texH = texture.height;

        CharacterInfo[] charInfos = new CharacterInfo[256];
        Rect r;

        for (int i = 0; i < charInfos.Length; i++)
        {
            CharacterInfo charInfo = new CharacterInfo();

            charInfo.index = i;
            charInfo.advance = 8;

            r = new Rect();
            r.x = (i % 16) * (8 / texW);
            r.y = (i / 16) * (8 / texH);
            r.width = 8 / texW;
            r.height = 8 / texH;
            r.y = 1f - r.y - r.height;
            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);

            r = new Rect();
            r.x = 0;
            r.y = 0;
            r.width = 8;
            r.height = 8;
            r.y = -r.y;
            r.height = -r.height;
            charInfo.minX = (int)r.xMin;
            charInfo.maxX = (int)r.xMax;
            charInfo.minY = (int)r.yMax;
            charInfo.maxY = (int)r.yMin;
            //charInfo.size = 1000;
            charInfo.glyphHeight = 8;
            charInfo.glyphWidth = 8;
            //charInfo.maxX = 8;
            //charInfo.maxY = 8;
            charInfos[i] = charInfo;
        }

#if CREATE_DUMMY_FONT
        // WARNING: painful hack below
        // Font creation can only be done in the Unity Editor because SerializedObject
        // is not availible in the final release build so we don't have the ability
        // to set any of the Font() class properties thus making the the Font class kind of
        // useless for runtime font creation applications like this.
        // Need to create a dummy empty/blank font that is setup correctly in the Editor and then
        // replace the font texture atlas at runtime with the real one from the original game instead. 
        // Arrgh!

        // Create material
        Material material = new Material(Shader.Find("UI/Default"));

        // create an empty/blank texture
        material.mainTexture = new Texture2D(16*8, 16, TextureFormat.RGBA32, false);

        // create a new font
        myFont = new Font();
        myFont.material = material;
        myFont.name = "font";
        myFont.characterInfo = charInfos;

        // Set the magic properties we cannot be set in the release build
        // by adjusting font info so it spacing is correct. Begin Magic stuff.
        UnityEditor.SerializedObject mFont = new UnityEditor.SerializedObject(myFont);
        mFont.FindProperty("m_FontSize").floatValue = 10.0f;
        mFont.FindProperty("m_LineSpacing").floatValue = 8.0f;
        mFont.ApplyModifiedProperties();
        // End magic stuff

        // save the font to the unity assets folder, 
        // need to reference this asset in the public Font myFont variable
        // in the Unity Editor so we can use it in the release build below
        UnityEditor.AssetDatabase.CreateAsset (myFont, "Assets/font.fontsettings");
        System.IO.File.WriteAllBytes("Assets/u4font.png", texture.EncodeToPNG());
#else
        // Create material
        Material material = new Material(Shader.Find("UI/Default"));
        material.mainTexture = texture;

        // just update font with original game texture,
        // everything else should already be set from when we created the asset file above
        myFont.material = material;

        // Set the font in the game text UI
        GameText.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsNames.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsHP.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsGold.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsFood.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsMagicStatus.GetComponent<UnityEngine.UI.Text>().font = myFont;
        windDirection.GetComponent<UnityEngine.UI.Text>().font = myFont;
        moons.GetComponent<UnityEngine.UI.Text>().font = myFont;
        statsNames.GetComponent<UnityEngine.UI.Text>().font = myFont;
#endif
    }
}
