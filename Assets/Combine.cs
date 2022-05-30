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
 * NOTE: this code has been heavily modified from the original to support this use case.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Combine
{
    // this one will go two layers deep to avoid an implementation that relies on recursion
    static GameObject[] GetAllChildrenWithMeshRenderers(GameObject gameObject)
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

    static GameObject[] GetAllChildrenWithMeshRenderers2(GameObject gameObject)
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
    private static int GetTextureSquareSize(GameObject[] o)
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

    private static int GetTextureSquareSize(GameObject[,] o)
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

    private static int GetTextureSize(GameObject[] o)
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

    private static int GetTextureSize(Texture[,] o)
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

    public static void Combine1(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int sizeW;
        int sizeH;
        int originalSizeW;
        int originalSizeH;
        int pow2;
        Texture2D combinedTexture;
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
                Debug.Log("original texture missing");
                return;
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
                Texture2D texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
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
            Material combinedMaterial = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
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
                            Object.Destroy(objectsToCombine[i]);
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

            Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    // This version creates a horizontal texture atlas instead of a square one so we can do UV animation for things like water and lava.
    public static void Combine2(GameObject gameObject, bool useMipMaps = false, TextureFormat textureFormat = TextureFormat.RGBA32, bool destroy = true)
    {
        int sizeW;
        int originalSizeW;
        int originalSizeH;
        int textureCount;
        Texture2D combinedTexture;
        Material material;
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
            Texture checkTexture = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture;
            if (checkTexture && checkTexture.width > 0)
            {
                originalSizeW = checkTexture.width;
                originalSizeH = checkTexture.height;
            }
            else
            {
                Debug.Log("original texture missing");
                return;
            }
            textureCount = GetTextureSize(objectsToCombine);
            sizeW = textureCount * originalSizeW;
            combinedTexture = new Texture2D(sizeW, originalSizeH, textureFormat, useMipMaps);
            combinedTexture.filterMode = FilterMode.Point;

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            int index = 0;
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                Texture2D texture = (Texture2D)objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture;
                if (texture && !textureAtlas.ContainsKey(texture))
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
                Material mat = objectsToCombine[i].GetComponent<MeshRenderer>().material;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(mat.mainTexture))
                {
                    offset = (Vector2)textureAtlas[mat.mainTexture];
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
                //if (objectsToCombine[i].isStatic)
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
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    //if (objectsToCombine[i].isStatic)
                    {
                        if (destroy)
                        {
                            Object.Destroy(objectsToCombine[i]);
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

            Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    public static void Combine3(GameObject mapGameObject,
        ref U4_Decompiled.TILE[,] map,
        int offset_x,
        int offset_y,
        ref GameObject[,] mapGameObjects,
        bool useMipMaps,
        TextureFormat textureFormat,
        bool wrap,
        Material combinedExpandedMaterial,
        Material combinedLinearMaterial,
        int billboardx,
        int billboardy,
        U4_Decompiled.DIRECTION direction)
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

        //float startTime = Time.realtimeSinceStartup;

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
                            Vector3 look = Vector3.zero;
                            // TODO hook this up to the actual camera position
                            //look = new Vector3(Camera.main.transform.position.x - saveLocalPosition.x, 0.0f, 255 - Camera.main.transform.position.z - saveLocalPosition.y);

                            if (direction == U4_Decompiled.DIRECTION.NORTH)
                            {
                                look = new Vector3(billboardx - saveLocalPosition.x, 0.0f, 255 - (billboardy + 10) - saveLocalPosition.y);
                            }
                            else if(direction == U4_Decompiled.DIRECTION.SOUTH)
                            {
                                look = new Vector3(billboardx - saveLocalPosition.x, 0.0f, 255 - (billboardy - 10) - saveLocalPosition.y);
                            }
                            else if (direction == U4_Decompiled.DIRECTION.EAST)
                            {
                                look = new Vector3((billboardx - 10) - saveLocalPosition.x, 0.0f, 255 - billboardy - saveLocalPosition.y);
                            }
                            else if (direction == U4_Decompiled.DIRECTION.WEST)
                            {
                                look = new Vector3((billboardx + 10) - saveLocalPosition.x, 0.0f, 255 - billboardy - saveLocalPosition.y);
                            }
                            Quaternion rotation1 = Quaternion.LookRotation(look);
                            mapTile.transform.rotation = rotation1;
                            mapTile.transform.localEulerAngles = new Vector3(rotation1.eulerAngles.y + 90f, 90f, -90f);

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
    }
}