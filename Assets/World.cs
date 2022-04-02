using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class World : MonoBehaviour
{
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
    public GameObject bubblePrefab;
    public GameObject party;
    public GameObject fighters;
    public GameObject characters;
    public GameObject activeCharacter;
    public GameObject hits;
    public GameObject moongate;

    public GameObject partyGameObject;

    public GameObject[] Settlements;
    public GameObject[] CombatTerrains;

    public string tileEGAFilepath = "/u4/SHAPES.EGA";
    public string tileCGAFilepath = "/u4/SHAPES.CGA";
    public string worldMapFilepath = "/u4/WORLD.MAP";

    public U4_Decompiled u4;

    U4_Decompiled.TILE[,] raycastSettlementMap = new U4_Decompiled.TILE[32, 32];
    U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[64, 64];

    const int MAP_CHUNK = 1;

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
    GameObject CreateBridge()
    {
        // create a game object to hold the cube and give it a proper name
        GameObject bridge = new GameObject("Bridge");
        bridge.name = "Bridge";

        // add a mesh filter and mesh renderer so we can see this new cube game object 
        MeshFilter meshFilter = bridge.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = bridge.AddComponent<MeshRenderer>();

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
            p4 = new Vector3(0.5f, 0.5f, 1.0f / Mathf.Sqrt(2));
        }
        else
        {
            p4 = new Vector3(0.5f, 0.5f, height);
        }

        //meshFilter.mesh = new Mesh();
        //meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = new Vector3[] 
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0), 
            p4 
        };

        meshFilter.mesh.triangles = new int[] 
        {
            4,0,1,
            4,1,2,
            4,2,3,
            4,3,0
        };

        Vector2[] uvs = new Vector2[meshFilter.mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(meshFilter.mesh.vertices[i].y, meshFilter.mesh.vertices[i].x);
        }

        meshFilter.mesh.uv = uvs;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.Optimize();

        return pyramid;
    }

    bool once = true;
    GameObject CreateQuad()
    {
        GameObject quad = new GameObject("Quad");

        quad.name = "Quad";

        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();

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
            4, 3, 1,

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
        WHITE = 15
    };

    void InitializeEGAPalette()
    {
        // create a EGA color palette
        EGAColorPalette = new Color[16];
        ColorUtility.TryParseHtmlString("#000000", out EGAColorPalette[0]);
        ColorUtility.TryParseHtmlString("#0000AA", out EGAColorPalette[1]);
        ColorUtility.TryParseHtmlString("#00AA00", out EGAColorPalette[2]);
        ColorUtility.TryParseHtmlString("#00AAAA", out EGAColorPalette[3]);
        ColorUtility.TryParseHtmlString("#AA0000", out EGAColorPalette[4]);
        ColorUtility.TryParseHtmlString("#AA00AA", out EGAColorPalette[5]);
        ColorUtility.TryParseHtmlString("#AA5500", out EGAColorPalette[6]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out EGAColorPalette[7]);
        ColorUtility.TryParseHtmlString("#555555", out EGAColorPalette[8]);
        ColorUtility.TryParseHtmlString("#5555FF", out EGAColorPalette[9]);
        ColorUtility.TryParseHtmlString("#55FF55", out EGAColorPalette[10]);
        ColorUtility.TryParseHtmlString("#55FFFF", out EGAColorPalette[11]);
        ColorUtility.TryParseHtmlString("#FF5555", out EGAColorPalette[12]);
        ColorUtility.TryParseHtmlString("#FF55FF", out EGAColorPalette[13]);
        ColorUtility.TryParseHtmlString("#FFFF55", out EGAColorPalette[14]);
        ColorUtility.TryParseHtmlString("#FFFFFF", out EGAColorPalette[15]);
    }

    enum CGA_COLOR
    {
        BLACK = 0,
        BLUE = 1,
        GREEN = 2,
        CYAN = 3,
        RED = 4,
        MEGENTA = 5,
        BROWN = 6,
        LIGHT_GRAY = 7,
    };

    void InitializeCGAPalette()
    {
        // create CGA color palette
        CGAColorPalette = new Color[8];
        ColorUtility.TryParseHtmlString("#000000", out CGAColorPalette[0]);
        ColorUtility.TryParseHtmlString("#0000AA", out CGAColorPalette[1]);
        ColorUtility.TryParseHtmlString("#00AA00", out CGAColorPalette[2]);
        ColorUtility.TryParseHtmlString("#00AAAA", out CGAColorPalette[3]);
        ColorUtility.TryParseHtmlString("#AA0000", out CGAColorPalette[4]);
        ColorUtility.TryParseHtmlString("#AA00AA", out CGAColorPalette[5]);
        ColorUtility.TryParseHtmlString("#AA5500", out CGAColorPalette[6]);
        ColorUtility.TryParseHtmlString("#AAAAAA", out CGAColorPalette[7]);
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

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the CGA color palette
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
                    // so the grass speckels don't show when we use the tile standing upright
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

            // Actually apply all previous SetPixel and SetPixels changes from above
            currentTile.Apply();

            // skip ahead
            index += 0x20;
        }
    }

    void LoadCombatMap(string combatMapFilepath, ref U4_Decompiled.TILE[,] combatMap)
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

        int fileIndex;

        if (combatMapFilepath == "/u4/SHRINE.CON")
        {
            fileIndex = 0x40 - 64;
        }
        else
        {
            fileIndex = 0x40;
        }

        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                combatMap[x, y] = (U4_Decompiled.TILE)combatMapFileData[fileIndex++];
            }
        }
    }

    [SerializeField]
    U4_Decompiled.TILE[,] entireMapTILEs = new U4_Decompiled.TILE[32 * 8, 32 * 8];

    [SerializeField]
    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];
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
        NOTHING = 0x00,
        LADDER_UP = 0x10, // 	Ladder Up
        LADDER_DOWN = 0x20, // 	Ladder Down
        LADDER_UP_AND_DOWN = 0x30, // 	Laddr Up & Down
        TREASURE_CHEST = 0x40, // 	Treasure Chest
        CEILING_HOLE = 0x50, //	Ceiling Hole
        FLOOR_HOLE = 0x60, // 	Floor Hole (unused)
        MAGIC_ORB = 0x70, // 	Magic Orb
        TRAP_WIND_DARKNESS = 0x80, // 	Winds/Darknes Trap
        TRAP_FALLING_ROCKS = 0x81, // 	Falling Rock Trap
        TRAP_PIT = 0x8E, //	Pit Trap
        FOUNTAIN = 0x90, // 	Plain Fountain
        FOUNTAIN_HEALING = 0x91, // 	Healing Fountain
        FOUNTAIN_ACID = 0x92, // 	Acid Fountain
        FOUNTAIN_CURE = 0x93, // 	Cure Fountain
        FOUNTAIN_POISIN = 0x94, // 	Poison Fountain
        FIELD_POISON = 0xA0, // Poison Field
        FIELD_ENERGY = 0xA1, //  Energy Field
        FIELD_FIRE = 0xA2, // Fire Field
        FIELD_SLEEP = 0xA3, //  Sleep Field
        ALTAR = 0xB0, // Altar
        DOOR = 0xC0, // Door
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
        public DUNGEON_TILE[,,] dungeonTILEs; // 8x8x8 map
        public DUNGEON_ROOM[] dungeonRooms; // 16 or 64 rooms
    }

    // this is needed to determine which combat map to use for which dungeon tile
    public int[] convertDungeonTiletoCombat  =
        { -13, 1, 2, 3, 4, -13, -13, -13, -13, -13, -13, -13, 5, -13, 6, -13 };

    [SerializeField]
    public DUNGEON[] dungeons = new DUNGEON[(int)DUNGEONS.MAX];
    void LoadDungeons()
    {
        for (int index = 0; index < (int)DUNGEONS.MAX; index++)
        {
            int rooms = 0;
            dungeons[index].name = ((DUNGEONS)index).ToString();
            dungeons[index].dungeonTILEs = new DUNGEON_TILE[8, 8, 8];
            if (index == (int)DUNGEONS.ABYSS)
            {
                //  Levels 1 & 2 each have 8 rooms
                //  Levels 3 & 4 each have 8 rooms
                //  Level 5 has 5 rooms
                //  Level 6 has 11 rooms
                //  Level 7 has 4 rooms
                //  Level 8 has 12 rooms
                // The D# values still define the order the rooms appear; they simply reset every two dungeon levels. 
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

            for (int z = 0; z < 8; z++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        dungeons[index].dungeonTILEs[x, 7 - y, z] = (DUNGEON_TILE)dungeonFileData[fileIndex++];
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

                // get party start positions
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
                    renderer.material.mainTextureOffset = new Vector2((float)((int)monsterTile * 16) / (float)renderer.material.mainTexture.width, 0.0f);
                    renderer.material.mainTextureScale = new Vector2((float)16.0f / (float)renderer.material.mainTexture.width, 1.0f);

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

                        if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.WALL)
                        {
                            mapTile = CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled.TILE.BRICK_WALL;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.NOTHING)
                        {
                            mapTile = CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled.TILE.LARGE_ROCKS;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.LADDER_UP)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_UP;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.LADDER_DOWN)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_DOWN;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.LADDER_UP_AND_DOWN)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.LADDER_DOWN; // TODO: need to overlap the up and down tiles, but this will do for now
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.TREASURE_CHEST)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.CHEST;
                        }
                        else if ((dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FOUNTAIN) ||
                                (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FOUNTAIN_CURE) ||
                                (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FOUNTAIN_HEALING) ||
                                (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FOUNTAIN_POISIN) ||
                                (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FOUNTAIN_ACID))
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.SHALLOW_WATER;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FIELD_ENERGY)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.ENERGY_FIELD;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FIELD_FIRE)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.FIRE_FIELD;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FIELD_POISON)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.POISON_FIELD;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.FIELD_SLEEP)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.SLEEP_FIELD;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.DOOR)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.DOOR;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.DOOR_SECRECT)
                        {
                            mapTile = CreatePartialCube(true, true, true, true);
                            tileIndex = U4_Decompiled.TILE.SECRET_BRICK_WALL;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.ALTAR)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.ALTAR;
                        }
                        else if (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.MAGIC_ORB)
                        {
                            mapTile = CreateQuad();
                            tileIndex = U4_Decompiled.TILE.MISSLE_ATTACK_BLUE; // TODO: need to find out what this is??? and find an tile for it.
                        }
                        else if ((dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.TRAP_FALLING_ROCKS) ||
                            (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.TRAP_WIND_DARKNESS) ||
                            (dungeons[index].dungeonTILEs[x, y, z] == DUNGEON_TILE.TRAP_PIT))
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
                DUNGEON_TILE dungeonTile;

                dungeonTile = dungeons[(int)dungeon].dungeonTILEs[x, y, level];
                if (dungeonTile == DUNGEON_TILE.WALL)
                {
                    dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.BRICK_WALL);
                    dungeonBlockGameObject.name = dungeonTile.ToString();
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
                else
                {
                    // use a combat map as the dungeon room base on the dungeon tile
                    int combat = convertDungeonTiletoCombat[(int)dungeons[(int)dungeon].dungeonTILEs[x, y, level] >> 4] + (int)U4_Decompiled.COMBAT_TERRAIN.DNG0;
                    dungeonBlockGameObject = GameObject.Instantiate(CombatTerrains[combat]);
                    dungeonBlockGameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    dungeonBlockGameObject.SetActive(true);
                    dungeonBlockGameObject.name = dungeonTile.ToString();
                }
                //else
                //{
                //    dungeonBlockGameObject = CreateDungeonBlock(U4_Decompiled.TILE.TILED_FLOOR);
                //}

                dungeonBlockGameObject.transform.SetParent(dungeonLevel.transform);
                dungeonBlockGameObject.transform.localPosition = new Vector3(x * 11, y * 11, 0);
            }
        }

        dungeonLevel.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        return dungeonLevel;
    }
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
        renderer.material.mainTexture = expandedTiles[31]; // this tile number 31 is in the Party structure under tile.
        renderer.material.mainTextureOffset = new Vector2((float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        // set the shader
        Shader unlit = Shader.Find("Unlit/Transparent Cutout");
        renderer.material.shader = unlit;

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
    public void ExpandTiles()
    {
        // allocate some textures pointers
        expandedTiles = new Texture2D[256];

        // go through all the original tiles
        for (int i = 0; i < 256; i++)
        {
            // create a new slightly larger texture with boarder for this tile
            Texture2D currentTile = originalTiles[i];
            Texture2D newTile = new Texture2D(currentTile.width + 2 * TILE_BORDER_SIZE, currentTile.height + 2 * TILE_BORDER_SIZE, TextureFormat.RGBA32, false);

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
        bool useUIShader;

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
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                    useUIShader = false;
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
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                    useUIShader = false;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled.TILE.MOUNTAINS)
                {
                    mapTile = CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f);
                    location = new Vector3(x + 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
                {
                    mapTile = CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
                {
                    mapTile = CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
                {
                    mapTile = CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
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
                    useUIShader = true;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE)
                {
                    mapTile = CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_TOP)
                {
                    mapTile = CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if ((tileIndex == U4_Decompiled.TILE.DOOR) || (tileIndex == U4_Decompiled.TILE.LOCKED_DOOR))
                {
                    mapTile = CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_MAST)
                {
                    mapTile = CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_WHEEL)
                {
                    mapTile = CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CHEST)
                {
                    mapTile = CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_LEFT)
                {
                    mapTile = CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_RIGHT)
                {
                    mapTile = CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                }
                // all other terrain tiles are flat
                else
                {
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
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
                        useUIShader = false;
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        useExpandedTile = true;
                        useUIShader = false;
                    }

                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Shader unlit;
                if (useUIShader)
                {
                    unlit = Shader.Find("Unlit/Transparent Cutout");
                }
                else
                {
                    unlit = Shader.Find("Unlit/Transparent Cutout");
                }
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = unlit;

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
        Combine(billboardTerrrainGameObject); // combine separately from terrain above as we may need a different shader for these

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
                    rotation = new Vector3(0.0f, 180.0f, 0.0f);
                    location = new Vector3(x + 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
                {
                    mapTile = CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
                {
                    mapTile = CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
                {
                    mapTile = CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
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
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
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

    public void CreateMapSubsetPass2(GameObject mapGameObject, ref U4_Decompiled.TILE[,] map)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;
        bool useExpandedTile;
        bool useUIShader;
        bool useLinearTile;

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
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
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
                    useUIShader = false;
                    useLinearTile = false;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled.TILE.MOUNTAINS)
                {
                    mapTile = CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f);
                    location = new Vector3(x + 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled.TILE.DUNGEON)
                {
                    mapTile = CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled.TILE.BRUSH) || (tileIndex == U4_Decompiled.TILE.HILLS))
                {
                    mapTile = CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled.TILE.SMALL_ROCKS)
                {
                    mapTile = CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x - 0.5f, map.GetLength(1) - 1 - y - 0.5f, 0.5f); // TODO center pyramid mesh so we don't need to move it or rotate it
                    useExpandedTile = true;
                    useUIShader = false;
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
                    useUIShader = true;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE)
                {
                    mapTile = CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_TOP)
                {
                    mapTile = CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if ((tileIndex == U4_Decompiled.TILE.DOOR) || (tileIndex == U4_Decompiled.TILE.LOCKED_DOOR))
                {
                    mapTile = CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_MAST)
                {
                    mapTile = CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.SHIP_WHEEL)
                {
                    mapTile = CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CHEST)
                {
                    mapTile = CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_LEFT)
                {
                    mapTile = CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_RIGHT)
                {
                    mapTile = CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                else if (tileIndex == U4_Decompiled.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                    useUIShader = false;
                    useLinearTile = false;
                }
                // all other terrain tiles are flat
                else
                {
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
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
                        useLinearTile = true;
                        useUIShader = false;
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainGameObject.transform);
                        location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                        rotation = Vector3.zero;
                        useExpandedTile = true;
                        useUIShader = false;
                        useLinearTile = false;
                    }
                }

                mapTile.transform.localEulerAngles = rotation;
                mapTile.transform.localPosition = location;

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Shader unlit;
                if (useUIShader)
                {
                    unlit = Shader.Find("Unlit/Transparent Cutout");
                }
                else
                {
                    unlit = Shader.Find("Unlit/Transparent Cutout");
                }
                Renderer renderer = mapTile.GetComponent<MeshRenderer>();
                renderer.material.shader = unlit;

                // set the tile and texture offset and scale
                
                if (useExpandedTile)
                {
                    renderer.material = combinedExpandedMaterial;
                    renderer.material.mainTexture = combinedExpandedTexture;
                    renderer.material.mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * 18)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * 18)) / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(18 - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(18 - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                
                }
                else if (useLinearTile)
                {
                    renderer.material = combinedLinearMaterial;
                    renderer.material.mainTexture = combinedLinearTexture;
                    renderer.material.mainTextureOffset = new Vector2((float)((int)tileIndex * 16) / (float)renderer.material.mainTexture.width, 0.0f);
                    renderer.material.mainTextureScale = new Vector2((float)16.0f / (float)renderer.material.mainTexture.width, 1.0f);
                }
                else
                {
                    renderer.material.mainTexture = originalTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                    renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
                }
                
                //renderer.material = combinedExpandedMaterial;
                //renderer.material.mainTexture = combinedExpandedTexture;
                Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 textureAtlasOffset;

                textureAtlasOffset = new Vector2((int)tileIndex % textureExpandedAtlasPowerOf2 * 18, (int)tileIndex / textureExpandedAtlasPowerOf2 * 18);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    if (useExpandedTile)
                    {
                        mainTextureOffset = new Vector2((float)(TILE_BORDER_SIZE + (((int)tileIndex % 16) * 18)) / (float)renderer.material.mainTexture.width, (float)(TILE_BORDER_SIZE + (((int)tileIndex / 16) * 18)) / (float)renderer.material.mainTexture.height);
                        mainTextureScale = new Vector2((float)(18 - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(18 - (2 * TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                    }
                    else if (useLinearTile)
                    {
                        mainTextureOffset = new Vector2((float)((int)tileIndex * 16) / (float)renderer.material.mainTexture.width, 0.0f);
                        mainTextureScale = new Vector2((float)16 / (float)renderer.material.mainTexture.width, 1.0f);
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

                // stash the game object in the array
                entireMapGameObjects[x, y] = mapTile;
            }
        }
    }

    public void followWorld()
    {
        // hook the player game object into the camera and the game engine
        FindObjectsOfType<MySmoothFollow>()[0].target = partyGameObject.transform;
        u4.partyGameObject = partyGameObject;
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
        int size;
        int originalSize;
        int pow2;
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
            // try to get the texture size used from the first gameobject
            Texture checkTexture = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture;
            if (checkTexture && checkTexture.width > 0)
            {
                originalSize = checkTexture.width;
            }
            else
            {
                originalSize = 2 * TILE_BORDER_SIZE + 16;
            }
            pow2 = GetTextureSquareSize(objectsToCombine);
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined square texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (texture && !textureAtlas.ContainsKey(texture))
                {
                    int x = (index % pow2) * originalSize;
                    int y = (index / pow2) * originalSize;

                    combinedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());

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
        int size;
        int originalSize;
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
            originalSize = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            textureCount = GetTextureSize(objectsToCombine);
            size = textureCount * originalSize;
            combinedTexture = new Texture2D(size, originalSize, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (!textureAtlas.ContainsKey(texture))
                {
                    int x = index * originalSize;
                    int y = 0;

                    combinedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());

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
        int size;
        int originalSize;
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
        originalSize = tilesTextures[0].width;

        // calc full size of texture atlas
        size = textureAtlasPowerOf2 * originalSize;

        // creare a new combined texture big enough to hold all the textures
        combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);
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
                int x = (index % textureAtlasPowerOf2) * originalSize;
                int y = (index / textureAtlasPowerOf2) * originalSize;

                combinedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());

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
        int size;
        int originalSize;
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
        originalSize = tilesTextures[0].width;

        // calc full size of texture atlas
        size = tilesTextures.Length * originalSize;

        // creare a new combined texture big enough to hold all the textures
        combinedLinearTexture = new Texture2D(size, originalSize, textureFormat, useMipMaps);
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
                int x = index * originalSize;
                int y = 0;

                combinedLinearTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());
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
        int size;
        int originalSize;

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
        originalSize = tilesTextures[0].width;

        // calc full square size of texture atlas
        size = textureExpandedAtlasPowerOf2 * originalSize;

        // creare a new combined texture big enough to hold all the textures
        combinedExpandedTexture = new Texture2D(size, size, textureFormat, useMipMaps);
        // we want our pixels
        combinedExpandedTexture.filterMode = FilterMode.Point;

        // Create the combined square texture using the existing textures (remember to ensure the total size of the texture isn't
        // larger than the platform supports)
        for (int i = 0; i < tilesTextures.Length; i++)
        {
            Texture2D texture = tilesTextures[i];
            if (texture)
            {
                int x = (i % textureExpandedAtlasPowerOf2) * originalSize;
                int y = (i / textureExpandedAtlasPowerOf2) * originalSize;

                combinedExpandedTexture.SetPixels(x, y, originalSize, originalSize, texture.GetPixels());
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
        bool useMipMaps = false, 
        TextureFormat textureFormat = TextureFormat.RGBA32)
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
        Vector3 position = mapGameObject.transform.position;
        Quaternion rotation = mapGameObject.transform.rotation;

        // set the game objects position to the zero location and rotation
        mapGameObject.transform.position = Vector3.zero;
        mapGameObject.transform.rotation = Quaternion.identity;

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
                    int x = (i + offset_x + mapGameObjects.GetLength(0)) % mapGameObjects.GetLength(0);
                    int y = (j + offset_y + mapGameObjects.GetLength(1)) % mapGameObjects.GetLength(1);
                    U4_Decompiled.TILE tileIndex = entireMapTILEs[x, y];

                    // only count them if the raycaster has copied the tile
                    if (map[i, j] != U4_Decompiled.TILE.BLANK)
                    {
                        if (tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER)
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

            // add all the meshes to the combine arrays
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    // only add them if the raycaster has copied the tile
                    if (map[i, j] != U4_Decompiled.TILE.BLANK)
                    {
                        int x = (i + offset_x + mapGameObjects.GetLength(0)) % mapGameObjects.GetLength(0);
                        int y = (j + offset_y + mapGameObjects.GetLength(1)) % mapGameObjects.GetLength(1);
                        U4_Decompiled.TILE tileIndex = entireMapTILEs[x, y];
                        
                        if (tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER)
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
                terrainFilter.mesh = new Mesh();
                terrainFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                terrainFilter.mesh.CombineMeshes(terrainCombine);
                terrainRenderer.material = combinedExpandedMaterial;
                terrainRenderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                terrainRenderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            if (countAnimatedTerrrain > 0)
            {
                animatedTerrrainFilter.mesh = new Mesh();
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
                billboardFilter.mesh = new Mesh();
                billboardFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                billboardFilter.mesh.CombineMeshes(billboardTerrrainCombine);
                billboardRenderer.material = combinedExpandedMaterial;
                billboardRenderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                billboardRenderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            // cleanup any unused resources now
            //Resources.UnloadUnusedAssets(); // this takes a really long time and checks everything for collection
        }

        // Restore the game object position
        terrainGameObject.transform.position = position;
        terrainGameObject.transform.rotation = rotation;

        //Debug.Log("Combine3 processing time : " + (Time.realtimeSinceStartup - startTime));
    }

    public void AddFighters(U4_Decompiled.t_68[] currentFighters, U4_Decompiled.tCombat1[] currentCombat)
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
                Shader unlit = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.shader = unlit;

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
            childoffighters.localPosition = new Vector3(currentCombat[fighterIndex]._npcX, 255 - currentCombat[fighterIndex]._npcY, 0);
            childoffighters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            childoffighters.transform.LookAt(look.transform);
            Vector3 rot = childoffighters.transform.eulerAngles;
            childoffighters.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);

        }
    }

    public void AddCharacters(U4_Decompiled.tCombat2[] currentCombat2, U4_Decompiled.tParty currentParty, U4_Decompiled.t_68[] currentFighters)
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
                Shader unlit = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.shader = unlit;

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
            childofcharacters.localPosition = new Vector3(currentCombat2[characterIndex]._charaX, 255 - currentCombat2[characterIndex]._charaY, 0); // appears to be one off in the Y from the fighters
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
            Shader unlit;

            // create the moongate object
            GameObject moongateGameObject = CreateQuad();

            // get the renderer
            renderer = moongateGameObject.GetComponent<MeshRenderer>();

            // intially the texture is null
            renderer.material.mainTexture = null;

            // set the shader
            unlit = Shader.Find("Unlit/Transparent Cutout");
            renderer.material.shader = unlit;

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

        // get adjusted position based on the offset of the raycastSettlementMap due to the player position
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
                Shader unlit = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.shader = unlit;

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
                    Settlement settlement;

                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((u4.Party._loc == U4_Decompiled.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == U4_Decompiled.TILE.LADDER_DOWN))
                    {
                        settlement = Settlements[0].GetComponent<Settlement>();
                    }
                    else
                    {
                        settlement = Settlements[(int)u4.Party._loc].GetComponent<Settlement>();
                    }

                    // set the name of the game object to match the npc
                    if ((currentNpcs[npcIndex]._tlkidx == 0) && (currentNpcs[npcIndex]._tlkidx <= 16 /* sometimes this is 127 */))
                    {
                        childofnpcs.name = npcTile.ToString();
                    }
                    else
                    {
                        childofnpcs.name = settlement.npcStrings[currentNpcs[npcIndex]._tlkidx - 1][0];
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
    public void AddHits(List<U4_Decompiled.hit> currentHitList)
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
                Shader unlit = Shader.Find("Unlit/Transparent Cutout");
                renderer.material.shader = unlit;

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
                childofhits.localPosition = new Vector3(currentHitList[hitIndex].x, 255 - currentHitList[hitIndex].y - 0.01f, 0); // move it slightly in from of the characters and fighters so we can see it.

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

    public void AddActiveCharacter(U4_Decompiled.activeCharacter currentActiveCharacter)
    {
        if (activeCharacter == null)
        {
            activeCharacter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            activeCharacter.transform.SetParent(transform);
            activeCharacter.transform.localPosition = Vector3.zero;
            activeCharacter.transform.localRotation = Quaternion.identity;
            activeCharacter.name = "Active Character";
            // set the shader
            Shader wireframe = Shader.Find("Custom/Geometry/Wireframe");
            //Shader.Find("Custom/Geometry/Wireframe").EnableKeyword("_REMOVEDIAG_ON")
            MeshRenderer renderer = activeCharacter.GetComponent<MeshRenderer>();
            renderer.material.shader = wireframe;
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
            Vector3 location = new Vector3(currentActiveCharacter.x, 0.01f, entireMapTILEs.GetLength(1) - 1 - currentActiveCharacter.y);
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
        checksum += Cast_Ray(ref map,  0, -1, pos_x, (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP
        checksum += Cast_Ray(ref map,  0,  1, pos_x, (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN
        checksum += Cast_Ray(ref map, -1,  0, (pos_x - 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray LEFT
        checksum += Cast_Ray(ref map,  1,  0, (pos_x + 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray RIGHT
        checksum += Cast_Ray(ref map,  1,  1, (pos_x + 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the RIGHT
        checksum += Cast_Ray(ref map,  1, -1, (pos_x + 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the RIGHT
        checksum += Cast_Ray(ref map, -1,  1, (pos_x - 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the LEFT
        checksum += Cast_Ray(ref map, -1, -1, (pos_x - 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the LEFT
        
        //Debug.Log("Raycast processing time : " + (Time.realtimeSinceStartup - startTime));

        // return the file tile checksum so we can determine if there were any changes from a previous raycase
        return checksum; 
    }

    int lastRaycastPlayer_posx = 0;
    int lastRaycastPlayer_posy = 0;

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

        InitializeEGAPalette();
        LoadTilesEGA();
        FixMageTile3();
        ExpandTiles();

        //InitializeCGAPalette();
        //LoadTilesCGA();

        // load the entire world map
        LoadWorldMap();

        // create the part game object
        CreateParty();

        // create a game object to store the combat terrain game objects, this should be at the top with no parent same as the world
        GameObject combatTerrainsObject = new GameObject();
        combatTerrainsObject.name = "Combat Terrains";

        // create a temp TILE map array to hold the combat terrains as we load them
        U4_Decompiled.TILE[,] combatMap = new U4_Decompiled.TILE[11, 11];

        CombatTerrains = new GameObject[(int)U4_Decompiled.COMBAT_TERRAIN.MAX];

        // go through all the combat terrains and load their maps and create a game object to hold them
        // as a child of the above combat terrains game object
        for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
        {
            // load the combat map from the original files
            LoadCombatMap("/u4/" + ((U4_Decompiled.COMBAT_TERRAIN)i).ToString() + ".CON", ref combatMap);

            // create a game object to hold it and set it as a child of the combat terrains game object
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(combatTerrainsObject.transform);

            // set it's name to match the combat terrain being created
            gameObject.name = ((U4_Decompiled.COMBAT_TERRAIN)i).ToString();

            // create the combat terrain based on the loaded map
            CreateMap(gameObject, combatMap);

            // Disable it initially
            gameObject.SetActive(false);

            // Position the combat map in place
            gameObject.transform.position = new Vector3(0, 0, entireMapTILEs.GetLength(1) - combatMap.GetLength(1)); ;

            // rotate map into place
            gameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

            // save the game object in the array
            CombatTerrains[i] = gameObject;
        }

        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled>();

        // assign settlement game objects
        Settlements = new GameObject[17];
        Settlements[(int)U4_Decompiled.LOCATIONS.BRITANNIA] = GameObject.Find("LBC_1");
        Settlements[(int)0] = GameObject.Find("LBC_2"); // actual location of the upper and lower levels is determined by the location of the ladders
        Settlements[(int)U4_Decompiled.LOCATIONS.THE_LYCAEUM] = GameObject.Find("LYCAEUM");
        Settlements[(int)U4_Decompiled.LOCATIONS.EMPATH_ABBY] = GameObject.Find("EMPATH");
        Settlements[(int)U4_Decompiled.LOCATIONS.SERPENT_HOLD] = GameObject.Find("SERPENT");
        Settlements[(int)U4_Decompiled.LOCATIONS.MOONGLOW] = GameObject.Find("MOONGLOW");
        Settlements[(int)U4_Decompiled.LOCATIONS.BRITAIN] = GameObject.Find("BRITAIN");
        Settlements[(int)U4_Decompiled.LOCATIONS.JHELOM] = GameObject.Find("JHELOM");
        Settlements[(int)U4_Decompiled.LOCATIONS.YEW] = GameObject.Find("YEW");
        Settlements[(int)U4_Decompiled.LOCATIONS.MINOC] = GameObject.Find("MINOC");
        Settlements[(int)U4_Decompiled.LOCATIONS.TRINSIC] = GameObject.Find("TRINSIC");
        Settlements[(int)U4_Decompiled.LOCATIONS.SKARA_BRAE] = GameObject.Find("SKARA");
        Settlements[(int)U4_Decompiled.LOCATIONS.MAGINCIA] = GameObject.Find("MAGINCIA");
        Settlements[(int)U4_Decompiled.LOCATIONS.PAWS] = GameObject.Find("PAWS");
        Settlements[(int)U4_Decompiled.LOCATIONS.BUCCANEERS_DEN] = GameObject.Find("DEN");
        Settlements[(int)U4_Decompiled.LOCATIONS.VESPER] = GameObject.Find("VESPER");
        Settlements[(int)U4_Decompiled.LOCATIONS.COVE] = GameObject.Find("COVE");

        CreateLinearTextureAtlas(ref originalTiles);



        // load all dungeons
        LoadDungeons();

        /*
        GameObject dungeonsRoomsGameObject = new GameObject("Dungeon Rooms");
        CreateDungeonRooms(dungeonsRoomsGameObject);
        //GameObject dungeonsGameObject = new GameObject("Dungeons");
        //CreateDungeons(dungeonsGameObject);

        for (int i = 0; i < 8; i++)
        {
            GameObject dungeonExpandedLevelGameObject = CreateDungeonExpandedLevel(DUNGEONS.DESPISE, i);
        }
        */

        GameObject dr = CreateDungeonRoom(ref dungeons[(int)DUNGEONS.WRONG].dungeonRooms[5]);

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
    }

    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;
    int lastChecksum = -1;
    int currentChecksum = 0;
    GameObject test; 

    // Update is called once per frame
    void Update()
    {
        // update the timer
        timer += Time.deltaTime;

        // only update periodically
        if (timer > timerExpired)
        {
            // reset the expired timer
            timer -= timerExpired;
            timerExpired = timerPeriod;

            if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                followWorld();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(true);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }
            }
            else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
            {
                AddNPCs(u4._npc);
                AddMoongate();
                followWorld();
                AddHits(u4.currentHits);
                AddActiveCharacter(u4.currentActiveCharacter);
                terrain.SetActive(true);
                animatedTerrrain.SetActive(true);
                billboardTerrrain.SetActive(true);
                fighters.SetActive(false);
                characters.SetActive(false);
                npcs.SetActive(true);
                party.SetActive(true);
                moongate.SetActive(false);

                for (int i = 0; i < (int)U4_Decompiled.COMBAT_TERRAIN.MAX; i++)
                {
                    CombatTerrains[i].gameObject.SetActive(false);
                }
            }
            else if ((u4.current_mode == U4_Decompiled.MODE.COMBAT)  || (u4.current_mode == U4_Decompiled.MODE.COMBAT_CAMP /* TODO: this could be the inn or shop or camp need to figure out which */ ) || (u4.current_mode == U4_Decompiled.MODE.COMBAT_ROOM))
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

                int currentCombatTerrain = (int)u4.Convert_Tile_to_Combat_Terrian(u4.current_tile);

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

            if ((party != null) && (originalTiles != null))
            {
                // set the party tile, person, horse, ballon, ship, etc.
                Renderer renderer = party.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    party.GetComponentInChildren<Renderer>().material.mainTexture = expandedTiles[(int)u4.Party._tile];
                    party.name = u4.Party._tile.ToString();
                }
            }
        }

        // we've moved, regenerate the raycast, TODO NPCs can also affect the raycast when moving, need to check them also or redo raycast more often
        if ((u4.Party._x != lastRaycastPlayer_posx) || (u4.Party._y != lastRaycastPlayer_posy))
        {
            Vector3 location = Vector3.zero;

            // update the last raycast position
            lastRaycastPlayer_posx = u4.Party._x;
            lastRaycastPlayer_posy = u4.Party._y;

            if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
            {
                // generate a new raycast and get a checksum
                currentChecksum = raycast(ref entireMapTILEs,
                    u4.Party._x, 
                    u4.Party._y,
                    ref raycastOutdoorMap, 
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK,
                    ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK, 
                    U4_Decompiled.TILE.BLANK);
                location = new Vector3(
                    ((u4.Party._x - raycastOutdoorMap.GetLength(0) / 2) - 1) / MAP_CHUNK * MAP_CHUNK, 0, 
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK - raycastOutdoorMap.GetLength(1));
            }
            else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
            {
                // generate a new raycast and get a checksum
                currentChecksum = raycast(ref u4.tMap32x32, 
                    u4.Party._x, 
                    u4.Party._y, 
                    ref raycastSettlementMap, 
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK, 
                    ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK, 
                    U4_Decompiled.TILE.GRASS);
                location = new Vector3(
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK, 0,
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK - raycastSettlementMap.GetLength(1));
            }

            // if last checksum does not match we need to regenerate the scene because the raycast is different
            // TODO also if we are sitting on a blank tile we are probably surrounded by something like trees so we need to regenerate
            // check if any of the surrounding tiles are emtpy also
            //if (lastChecksum != currentChecksum)
            {
                // save the checksum
                lastChecksum = currentChecksum;


                // create the game object children with meshes and textures
                if (u4.current_mode == U4_Decompiled.MODE.OUTDOORS)
                {
                    if (test == null)
                    {
                        test = new GameObject();
                        CreateSquareTextureAtlas(ref originalTiles);
                        ExpandTiles();
                        CreateExpandedTextureAtlas(ref expandedTiles);
                        CreateLinearTextureAtlas(ref originalTiles);
                        CreateMapSubsetPass2(test, ref entireMapTILEs);
                    }

                    Combine3(mainTerrain, ref raycastOutdoorMap, u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1, u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1, ref entireMapGameObjects);
                    
                    location = Vector3.zero;
                    terrain.transform.eulerAngles = Vector3.zero; // reset back to zero, TODO: figure out who is modifiying this
                }
                else if (u4.current_mode == U4_Decompiled.MODE.BUILDING)
                {
                    CreateMap(mainTerrain, raycastSettlementMap);

                    location = new Vector3(
                        ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK, 0,
                        entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) / MAP_CHUNK) * MAP_CHUNK - raycastSettlementMap.GetLength(1));
                }

                // Position the map in place
                mainTerrain.transform.position = location;

                // rotate map into place
                mainTerrain.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

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
            }
        }
    }

    public GameObject convertMe;
}
