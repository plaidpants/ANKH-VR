using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityStandardAssets.Utility;

public class World : MonoBehaviour
{
    public Color[] CGAColorPalette;
    public Color[] EGAColorPalette;
    public Texture2D[] tiles;

    public GameObject npcs;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain; // new item not used yet for trees, anks, etc.
    public GameObject party;
    public GameObject fighters; 
    public GameObject characters;
    public GameObject activeCharacter; 
    public GameObject hits;

    public GameObject mapHudGameObject;
    public GameObject npcsHud;
    public GameObject terrainHud;
    public GameObject animatedTerrrainHud;
    public GameObject otherHud;
    public GameObject activeCharacterHud;

    public string tileEGAFilepath = "/u4/SHAPES.EGA";
    public string tileCGAFilepath = "/u4/SHAPES.CGA";
    public string worldMapFilepath = "/u4/WORLD.MAP";

    GameObject CreatePyramid ()
    {
        GameObject pyramid = new GameObject("Pyramid");
        //pyramid.transform.position = Vector3.zero;

        //GameObject pyramid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pyramid.name = "Pyramid";

        MeshFilter meshFilter = pyramid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pyramid.AddComponent<MeshRenderer>();

        //MeshFilter meshFilter = pyramid.GetComponent<MeshFilter>();

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(1, 1, 0);
        Vector3 p3 = new Vector3(0, 1, 0);
        Vector3 p4 = new Vector3(0.5f, 0.5f, 1.0f / Mathf.Sqrt(2));
        //Vector3 p4 = new Vector3(0.5f, 0.5f, Random.Range(1.0f / Mathf.Sqrt(2), 2.0f));

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { p0, p1, p2, p3, p4 };
        mesh.triangles = new int[] {
            4,0,1,
            4,1,2,
            4,2,3,
            4,3,0
            };

        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].y, mesh.vertices[i].x);
        }

        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshFilter.mesh = mesh;

        return pyramid;
    }

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
        byte [] fileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + tileEGAFilepath);

        if (fileData.Length != 32*1024)
        {
            Debug.Log("EGA tiles file incorrect length " + fileData.Length);
            return;
        }

        // allocate an array of textures
        tiles = new Texture2D[256];

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
            tiles[tile] = currentTile;

            // manually go through the data and set the (x,y) pixels to the tile based on the input file using the CGA color palette
            for (int height = 0; height < currentTile.height; height++)
            {
                for (int width = 0; width < currentTile.width; /* width incremented below */ )
                {
                    // set the color of the first half of the nibble
                    int colorIndex = fileData[index] >> 4; 
                    Color color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    if ((colorIndex == 0) && ((tile >= 32 && tile <= 47) || (tile >= 80 && tile <= 95)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= 68 && tile <= 71)
                    {
                        if (colorIndex == 0)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    else
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                    }

                    // set the color of the second half of the nibble
                    colorIndex = fileData[index] & 0xf;
                    color = EGAColorPalette[colorIndex];

                    // check if these are people/creatures and use black as alpha channel
                    if ((colorIndex == 0) && ((tile >= 32 && tile <= 47) || (tile >= 80 && tile <= 95)))
                    {
                        currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                    }
                    // energy fields are transparent
                    else if (tile >= 68 && tile <= 71)
                    {
                        if (colorIndex == 0)
                        {
                            currentTile.SetPixel(width++, currentTile.height - height - 1, alpha);
                        }
                        else
                        {
                            color.a = 0.75f;
                            currentTile.SetPixel(width++, currentTile.height - height - 1, color);
                        }
                    }
                    // everything else has not alpha channel
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
        string destination = Application.persistentDataPath + "/u4/test1.txt";
        System.IO.FileStream file;

        if (System.IO.File.Exists(destination)) 
            file = System.IO.File.OpenWrite(destination);
        else 
            file = System.IO.File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, "hello world");
        file.Close();

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
        tiles = new Texture2D[256];

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
                    currentTile.SetPixel(width, currentTile.height - height -2, color);

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

    // this can take a while so we need to yield during this or the editor or game will lock up.
    IEnumerator LoadWorldMap()
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
            yield break;
        }

        // read the file
        byte[] worldMapFileData = System.IO.File.ReadAllBytes(Application.persistentDataPath + worldMapFilepath);

        if (worldMapFileData.Length != 32 * 32 * 64)
        {
            Debug.Log("World map file incorrect length " + worldMapFileData.Length);
            yield break; 
        }

        // create three game object under us to hold these sub categories of things
        terrain = new GameObject("terrain");
        terrain.transform.SetParent(transform);
        terrain.transform.localPosition = Vector3.zero;
        terrain.transform.localRotation = Quaternion.identity;
        npcs = new GameObject("npc");
        npcs.transform.SetParent(transform);
        npcs.transform.localPosition = Vector3.zero;
        npcs.transform.localRotation = Quaternion.identity;
        animatedTerrrain = new GameObject("water");
        animatedTerrrain.transform.SetParent(transform);
        animatedTerrrain.transform.localPosition = Vector3.zero;
        animatedTerrrain.transform.localRotation = Quaternion.identity;
        party = new GameObject("party");
        party.transform.SetParent(transform);
        party.transform.localPosition = Vector3.zero;
        party.transform.localRotation = Quaternion.identity;
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

        int index = 0;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject childTerrain = new GameObject("terrain (" + x + ", " + y + ")");
                childTerrain.transform.SetParent(terrain.transform);
                childTerrain.transform.localPosition = Vector3.zero;
                childTerrain.transform.localRotation = Quaternion.identity;

                GameObject childAnimatedTerrrain = new GameObject("water (" + x + ", " + y + ")");
                childAnimatedTerrrain.transform.SetParent(animatedTerrrain.transform);
                childAnimatedTerrrain.transform.localPosition = Vector3.zero;
                childAnimatedTerrrain.transform.localRotation = Quaternion.identity;

                for (int height = 0; height < 32; height++)
                {
                    for (int width = 0; width < 32; width++)
                    {
                        GameObject mapTile;
                        int tileIndex = worldMapFileData[index++];

                        // solid object, brick, rocks etc.
                        if (tileIndex == 73 || tileIndex == 127 || tileIndex == 57)
                        {
                            mapTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            mapTile.transform.SetParent(childTerrain.transform);
                            Vector3 location = new Vector3(width + x * 32, 31 - height + (7 - y) * 32, 0.0f);
                            mapTile.transform.localPosition = location;
                        }
                        // Letters, make into short cubes
                        else if (tileIndex >= 96 && tileIndex <= 125)
                        {
                            mapTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            mapTile.transform.SetParent(childTerrain.transform);
                            mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                            mapTile.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                            Vector3 location = new Vector3(width + x * 32, 31 - height + (7 - y) * 32, 0.25f);
                            mapTile.transform.localPosition = location;
                        }
                        else if (tileIndex == 8 || tileIndex == 9)
                        {
                            mapTile = CreatePyramid();
                            mapTile.transform.SetParent(childTerrain.transform);
                            mapTile.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                            Vector3 location = new Vector3(width + x * 32 + 0.5f, 31 - height + (7 - y) * 32 - 0.5f, 0.5f);
                            mapTile.transform.localPosition = location;
                        }
                        // all other terrain tiles are flat
                        else
                        {
                            Vector3 location;

                            mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);

                            // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                            if ((tileIndex < 3) || (tileIndex >= 68 && tileIndex <= 71) || (tileIndex == 76))
                            {
                                mapTile.transform.SetParent(childAnimatedTerrrain.transform);
                                location = new Vector3(width + x * 32, 31 - height + (7 - y) * 32, 0.5f);
                            }
                            else
                            {
                                mapTile.transform.SetParent(childTerrain.transform);
                                location = new Vector3(width + x * 32, 31 - height + (7 - y) * 32, 0.5f);
                            }

                            mapTile.transform.localPosition = location;
                        }

                        // all terrain is static, used by combine below to merge meshes
                        mapTile.isStatic = true;

                        // set the shader
                        Shader unlit = Shader.Find("Mobile/Unlit (Supports Lightmap)");

                        MeshRenderer render = mapTile.GetComponent<MeshRenderer>();

                        render.material.mainTexture = tiles[tileIndex];
                        render.material.shader = unlit;
                    }
                }

                Combine(childTerrain.gameObject);
                Combine2(childAnimatedTerrrain.gameObject);

                // add our little animator script
                childAnimatedTerrrain.AddComponent<Animate1>();

                yield return null;
            }
        }

        {
            // create player/party object to display texture
            GameObject partyGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            partyGameObject.transform.SetParent(party.transform);

            // rotate the npc game object after creating and addition of child
            partyGameObject.transform.localPosition = new Vector3(0, 0, 0);
            partyGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 180.0f);

            // create child object for texture
            MeshRenderer renderer = partyGameObject.GetComponent<MeshRenderer>();

            // set the tile
            renderer.material.mainTexture = tiles[31]; // this tile number 31 is in the Party structure under tile.

            // set the shader
            Shader unlit = Shader.Find("Sprites/Default");

            renderer.material.shader = unlit;

            followWorld();
            // don't add the script as the world map player does not have any animated tiles
        }

        // rotate world into place
        transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
    }

    public void DrawMap(U4_Decompiled.TILE[,] map, List<U4_Decompiled.hit> currentHits, U4_Decompiled.activeCharacter currentActiveCharacter)
    {
        if (mapHudGameObject == null)
        {
            mapHudGameObject = new GameObject("hud");
            mapHudGameObject.transform.SetParent(transform);
            mapHudGameObject.transform.localPosition = Vector3.zero;
            mapHudGameObject.transform.localRotation = Quaternion.identity;

            terrainHud = new GameObject("terrain");
            terrainHud.transform.SetParent(mapHudGameObject.transform);
            terrainHud.transform.localPosition = Vector3.zero;
            terrainHud.transform.localRotation = Quaternion.identity;

            npcsHud = new GameObject("npc");
            npcsHud.transform.SetParent(mapHudGameObject.transform);
            npcsHud.transform.localPosition = Vector3.zero;
            npcsHud.transform.localRotation = Quaternion.identity;

            animatedTerrrainHud = new GameObject("water");
            animatedTerrrainHud.transform.SetParent(mapHudGameObject.transform);
            animatedTerrrainHud.transform.localPosition = Vector3.zero;
            animatedTerrrainHud.transform.localRotation = Quaternion.identity;

            otherHud = new GameObject("other");
            otherHud.transform.SetParent(mapHudGameObject.transform);
            otherHud.transform.localPosition = Vector3.zero;
            otherHud.transform.localRotation = Quaternion.identity;

            activeCharacterHud = GameObject.CreatePrimitive(PrimitiveType.Cube);
            activeCharacterHud.transform.SetParent(mapHudGameObject.transform);
            activeCharacterHud.transform.localPosition = Vector3.zero;
            activeCharacterHud.transform.localRotation = Quaternion.identity;
            // set the shader
            Shader wireframe = Shader.Find("Custom/Geometry/Wireframe");
            MeshRenderer renderer = activeCharacterHud.GetComponent<MeshRenderer>();
            renderer.material.shader = wireframe;
            renderer.material.SetFloat("_WireframeVal", 0.03f);
            renderer.material.SetFloat("_RemoveDiag", 1);
            renderer.material.SetColor("_FrontColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
        }
        else
        {
            // start over each update
            foreach (Transform child in terrainHud.transform)
            {
                Object.Destroy(child.gameObject);
            }
            foreach (Transform child in npcsHud.transform)
            {
                Object.Destroy(child.gameObject);
            }
            foreach (Transform child in animatedTerrrainHud.transform)
            {
                Object.Destroy(child.gameObject);
            }
            foreach (Transform child in otherHud.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        if (currentActiveCharacter.active)
        {
            Vector3 location = new Vector3(currentActiveCharacter.x, 11 - currentActiveCharacter.y, 0.0f);
            activeCharacterHud.transform.localPosition = location;
            activeCharacterHud.SetActive(true);
        }
        else 
        { 
            activeCharacterHud.SetActive(false); 
        }

        for (int height = 0; height < 11; height++)
        {
            for (int width = 0; width < 11; width++)
            {
                GameObject mapTile;

                U4_Decompiled.TILE tileIndex;
                
                tileIndex = map[height, width];

                foreach (U4_Decompiled.hit checkHit in currentHits)
                {
                    if (checkHit.x == width && checkHit.y == height && checkHit.tile != 0)
                    {
                        // display this hit tile instead
                        tileIndex = checkHit.tile;
                        break;
                    }
                }

                // solid object, brick, rocks etc.
                if ((tileIndex == U4_Decompiled.TILE.SECRET_BRICK_WALL)
                    || (tileIndex == U4_Decompiled.TILE.LARGE_ROCKS) 
                    || (tileIndex == U4_Decompiled.TILE.BRICK_WALL))
                {
                    mapTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTile.transform.SetParent(terrainHud.transform);
                    Vector3 location = new Vector3(width, 11 - height, 0.0f);
                    mapTile.transform.localPosition = location;
                }
                // Letters, make into short cubes
                else if ((tileIndex >= U4_Decompiled.TILE.A) && (tileIndex <= U4_Decompiled.TILE.Z))
                {
                    mapTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTile.transform.SetParent(terrainHud.transform);
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    mapTile.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                    Vector3 location = new Vector3(width, 11 - height, 0.25f);
                    mapTile.transform.localPosition = location;
                }
                else if ((tileIndex == U4_Decompiled.TILE.MOUNTAINS) || (tileIndex == U4_Decompiled.TILE.DUNGEON))
                {
                    mapTile = CreatePyramid();
                    mapTile.transform.SetParent(terrainHud.transform);
                    mapTile.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
                    Vector3 location = new Vector3(width + 0.5f, 11 - height - 0.5f, 0.5f);
                    mapTile.transform.localPosition = location;
                }
                // all other terrain tiles are flat
                else
                {
                    Vector3 location;

                    mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    if ((tileIndex <= U4_Decompiled.TILE.SHALLOW_WATER) || 
                        (tileIndex >= U4_Decompiled.TILE.POISON_FIELD && tileIndex <= U4_Decompiled.TILE.SLEEP_FIELD) 
                        || (tileIndex == U4_Decompiled.TILE.LAVA))
                    {
                        mapTile.transform.SetParent(animatedTerrrainHud.transform);
                        location = new Vector3(width, 11 - height, 0.5f);
                    }
                    else
                    {
                        mapTile.transform.SetParent(terrainHud.transform);
                        location = new Vector3(width, 11 - height, 0.5f);
                    }

                    mapTile.transform.localPosition = location;
                }

                // all terrain is static, used by combine below to merge meshes
                mapTile.isStatic = true;

                // set the shader
                Shader unlit = Shader.Find("Mobile/Unlit (Supports Lightmap)");

                MeshRenderer render = mapTile.GetComponent<MeshRenderer>();

                render.material.mainTexture = tiles[(int)tileIndex];
                render.material.shader = unlit;
            }
        }

        Combine(terrainHud); 
        Combine2(animatedTerrrainHud);

        // add our little animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present
        if (animatedTerrrainHud.GetComponent<Animate1>() == null)
        {
            animatedTerrrainHud.AddComponent<Animate1>();
        }

        // rotate world into place
        //transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
    }

    public void followWorld()
    {
        // hook the player game object into the camera and the game engine
        FindObjectsOfType<SmoothFollow>()[0].target = party.transform;
        FindObjectsOfType<U4_Decompiled>()[0].partyGameObject = party;
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
        if (textures.Count <= 128) return 16;
        if (textures.Count <= 256) return 32;

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

        if (objectsToCombine.Length > 1)
        {
            originalSize = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
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
                if (!textureAtlas.ContainsKey(texture))
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
                        uv[u] = mesh.uv[u] / (float)pow2;
                        uv[u].x += ((float)offset.x) / (float)pow2;
                        uv[u].y += ((float)offset.y) / (float)pow2;
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

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
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

                filter.mesh = new Mesh();
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

            Resources.UnloadUnusedAssets();
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

        if (objectsToCombine.Length > 1)
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
                    int x = index  * originalSize;
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
            if (staticCount > 1)
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

            Resources.UnloadUnusedAssets();
        }

        // Restore position
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
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
                // a child object for each npc entry in the table
                GameObject fighterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                // get the renderer
                MeshRenderer renderer = fighterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                Shader unlit = Shader.Find("Sprites/Default");
                renderer.material.shader = unlit;

                // add our little animator script and set the tile
                Animate3 animate = fighterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.renderer = renderer;

                // rotate the npc game object into position after creating
                Vector3 fightersLocation = new Vector3(0, 255, 0);
                fighterGameObject.transform.localPosition = fightersLocation;
                fighterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                fighterGameObject.transform.SetParent(fighters.transform);

                // set as intially disabled
                fighterGameObject.SetActive(false);
            }
        }

        // update all fighters in the table
        for (int fighterIndex = 0; fighterIndex < 16; fighterIndex++)
        {
            // get the tile
            U4_Decompiled.TILE npcTile = currentFighters[fighterIndex]._tile;

            // get the corresponding npc game object
            Transform childoffighters = fighters.transform.GetChild(fighterIndex);

            // update the tile of the game object
            if (currentFighters[fighterIndex]._sleeping == 0)
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(npcTile);
            }
            else
            {
                childoffighters.GetComponent<Animate3>().SetNPCTile(U4_Decompiled.TILE.SLEEP);
            }

            // update the position
            childoffighters.localPosition = new Vector3(currentCombat[fighterIndex]._npcX, 255 - currentCombat[fighterIndex]._npcY, 0);
            childoffighters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);
        }
    }

 

    public void AddCharacters(U4_Decompiled.tCombat2[] currentCombat2, U4_Decompiled.tParty currentParty, U4_Decompiled.t_68[] currentFighters)
    {
        // have we finished creating the world
        if (characters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (characters.transform.childCount != 8)
        {
            for (int i = 0; i < 8; i++)
            {
                // a child object for each npc entry in the table
                GameObject characterGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                // get the renderer
                MeshRenderer renderer = characterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                Shader unlit = Shader.Find("Sprites/Default");
                renderer.material.shader = unlit;

                // add our little animator script and set the tile
                Animate3 animate = characterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.renderer = renderer;

                // rotate the npc game object into position after creating
                Vector3 characterLocation = new Vector3(0, 255, 0);
                characterGameObject.transform.localPosition = characterLocation;
                characterGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the fighters game object
                characterGameObject.transform.SetParent(characters.transform);

                // set as intially disabled
                characterGameObject.SetActive(false);
            }
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

            // get the corresponding npc game object
            Transform childofcharacters = characters.transform.GetChild(characterIndex);

            // update the tile of the game object
            childofcharacters.GetComponent<Animate3>().SetNPCTile(npcTile);
  
            // update the position
            childofcharacters.localPosition = new Vector3(currentCombat2[characterIndex]._charaX, 255 - currentCombat2[characterIndex]._charaY, 0); // appears to be one off in the Y from the fighters
            childofcharacters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);
        }

        FindObjectsOfType<SmoothFollow>()[0].target = characters.transform.GetChild(0);
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
                GameObject npcGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                // get the renderer
                MeshRenderer renderer = npcGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                Shader unlit = Shader.Find("Sprites/Default");
                renderer.material.shader = unlit;

                // add our little animator script and set the tile
                Animate3 animate = npcGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
                animate.world = this;
                animate.renderer = renderer;

                // rotate the npc game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                npcGameObject.transform.localPosition = npcLocation;
                npcGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the npcs game object
                npcGameObject.transform.SetParent(npcs.transform);

                // set as intially disabled
                npcGameObject.SetActive(false);
            }
        }

        // update all npcs in the table
        for (int npcIndex = 0; npcIndex < 32; npcIndex++)
        {
            // get the tile
            U4_Decompiled.TILE npcTile = currentNpcs[npcIndex]._tile;

            // get the corresponding npc game object
            Transform childofnpcs = npcs.transform.GetChild(npcIndex);
            if (currentNpcs[npcIndex]._tlkidx == 0)
            {
                childofnpcs.name = npcTile.ToString();
            }
            else
            {
                U4_Decompiled u4_Decompiled = FindObjectsOfType<U4_Decompiled>()[0];
                // TODO simplify this mess
                childofnpcs.name = u4_Decompiled.Settlements[(int)u4_Decompiled.Party._loc].GetComponent<Settlement>().npcStrings[currentNpcs[npcIndex]._tlkidx - 1][0];
            }

            // update the tile of the game object
            childofnpcs.GetComponent<Animate3>().SetNPCTile(npcTile);



            // update the position
            childofnpcs.localPosition = new Vector3(currentNpcs[npcIndex]._x, 255 - currentNpcs[npcIndex]._y, 0);
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
                GameObject npcGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

                // get the renderer
                MeshRenderer renderer = npcGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                Shader unlit = Shader.Find("Sprites/Default");
                renderer.material.shader = unlit;

                // rotate the hit game object into position after creating
                Vector3 npcLocation = new Vector3(0, 255, 0);
                npcGameObject.transform.localPosition = npcLocation;
                npcGameObject.transform.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                // set this as a parent of the hits game object
                npcGameObject.transform.SetParent(hits.transform);

                // set as intially disabled
                npcGameObject.SetActive(false);
            }
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
                childofhits.GetComponent<Renderer>().material.mainTexture = tiles[(int)hitTile];

                // update the position
                childofhits.localPosition = new Vector3(currentHitList[hitIndex].x, 255 - currentHitList[hitIndex].y - 0.01f, 0); // move it slightly in from of the characters and fighters so we can see it.

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
            MeshRenderer renderer = activeCharacter.GetComponent<MeshRenderer>();
            renderer.material.shader = wireframe;
            renderer.material.SetFloat("_WireframeVal", 0.03f);
            renderer.material.SetFloat("_RemoveDiag", 1);
            renderer.material.SetColor("_FrontColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
        }

        if (currentActiveCharacter.active)
        {
            Vector3 location = new Vector3(currentActiveCharacter.x, 255 - currentActiveCharacter.y, 0.0f);
            activeCharacter.transform.localPosition = location;
            activeCharacter.SetActive(true);
        }
        else
        {
            activeCharacter.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeEGAPalette();
        LoadTilesEGA();

        //InitializeCGAPalette();
        //LoadTilesCGA();

        StartCoroutine(LoadWorldMap());

        /*
        if (npcs)
        {
            Combine(npcs.gameObject);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }
}