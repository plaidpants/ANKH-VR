using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Primitive 
{
    //bool once = true;

    static Vector3[] QuadMeshVerticies = null;
    static int[] QuadMeshTriangles = null;
    static Vector2[] QuadMeshUV = null;

    static Vector3[] BridgeMeshVertices = null;
    static int[] BridgeMeshTriangles = null;
    static Vector2[] BridgeMeshUV = null;

    public static GameObject CreatePartialCube(bool leftside = true, bool rightside = true, bool back = true, bool front = true)
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

    public static GameObject CreateBridgeUpper()
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

    public static GameObject CreateBridgeLower()
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

    public static GameObject CreatePillar()
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

    public static GameObject CreateMast()
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

    public static GameObject CreateWheel()
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

    public static GameObject CreateBridge()
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

    public static GameObject CreateDoor()
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

    public static GameObject CreateCastleLeft()
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

    public static GameObject CreateCastleRight()
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

    public static GameObject CreateCastleCenter()
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

    public static GameObject CreatePyramid(float height = 1.0f)
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

    public static GameObject CreateQuad()
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

    public static GameObject CreateChest()
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

    public static GameObject CreateWedge()
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
}