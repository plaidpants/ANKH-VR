using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public Font myFont;
    public Font myTransparentFont;

    // mainTerrain holds the terrain, animatedTerrrain, billboardTerrrain
    public GameObject mainTerrain;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain;

    public GameObject InputPanel;
    public GameObject MainMainLoop;
    public GameObject Keyboard;
    public GameObject KeyboardUpper;
    public GameObject KeyboardLower;
    public GameObject GameText;
    public GameObject PictureGameObject;
    public GameObject TalkChoice;
    public GameObject TalkContinue;
    public GameObject TalkMF;

    // reference to game engine
    public U4_Decompiled_TITLE u4_TITLE;

    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];

    bool CheckTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_WALL
                    || tileIndex == U4_Decompiled_AVATAR.TILE.LARGE_ROCKS
                    || tileIndex == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL);
    }

    bool CheckShortTileForOpacity(U4_Decompiled_AVATAR.TILE tileIndex)
    {
        return (CheckTileForOpacity(tileIndex) ||
                    ((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE)));
    }


    public void CreateMap(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map, bool lookAtCamera = true)
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
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = Primitive.CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // trees we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ALTAR) ||
                   // (tileIndex == U4_Decompiled.TILE.CHEST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.PARTY) || // the shrine map uses a fixed party tile instead of putting the party characters into the map
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                    mapTile = Primitive.CreateQuad();
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
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
                {
                    mapTile = Primitive.CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
                {
                    mapTile = Primitive.CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = Primitive.CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
                {
                    mapTile = Primitive.CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = Primitive.CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
                {
                    mapTile = Primitive.CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
                {
                    mapTile = Primitive.CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
                {
                    mapTile = Primitive.CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
                {
                    mapTile = Primitive.CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
                {
                    mapTile = Primitive.CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
                {
                    mapTile = Primitive.CreateCastleCenter();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                // all other terrain tiles are flat
                else
                {
                    mapTile = Primitive.CreateQuad();
                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
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
                    renderer.material.mainTexture = Tile.expandedTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);
                }
                else
                {
                    renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
                    renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                    renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
                }
            }
        }

        // this takes about 150ms for the 64x64 outside grid.
        Combine.Combine1(terrainGameObject);
        Combine.Combine2(animatedTerrrainGameObject);
        Combine.Combine1(billboardTerrrainGameObject); // combine separately from terrain above as we need to point these towards the player

        // add our little water animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present

        if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
        {
            animatedTerrrainGameObject.AddComponent<Animate1>();
        }

        // Position the settlement in place
        mapGameObject.transform.position = new Vector3(-5, 0, 7);

        // rotate settlement into place
        mapGameObject.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
    }

    public void CreateMapSubset(GameObject mapGameObject, U4_Decompiled_AVATAR.TILE[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    rotation = Vector3.zero;
                }
                // Letters, make into short cubes
                else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
                    || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
                {
                    U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                    U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                    if (y > 0)
                        aboveTile = map[x, y - 1];
                    if (y < map.GetLength(1) - 1)
                        belowTile = map[x, y + 1];
                    if (x > 0)
                        leftTile = map[x - 1, y];
                    if (x < map.GetLength(0) - 1)
                        rightTile = map[x + 1, y];

                    mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
                    mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                }
                // make mountains into pyramids
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make dungeon entrance into pyramid, rotate so it faces the right direction
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // tress we need to stand upright and face the camera
                else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
                    (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
                {
                    // create a billboard gameobject
                    //mapTile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    mapTile = Primitive.CreateQuad();

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
                    mapTile = Primitive.CreateQuad();

                    // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
                    // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
                    if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                        (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                        || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
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

                renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // stash the object mesh, transform & texture information
                entireMapGameObjects[x, y] = mapTile;
            }
        }
    }

    public GameObject CreateMapTileObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
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
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckTileForOpacity(leftTile), !CheckTileForOpacity(rightTile), !CheckTileForOpacity(aboveTile), !CheckTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();
            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // Letters, make into short cubes
        else if (((tileIndex >= U4_Decompiled_AVATAR.TILE.A) && (tileIndex <= U4_Decompiled_AVATAR.TILE.BRACKET_SQUARE))
            || (tileIndex == U4_Decompiled_AVATAR.TILE.ARCHITECTURE))
        {
            if (allWalls == false)
            {
                U4_Decompiled_AVATAR.TILE aboveTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE belowTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE leftTile = U4_Decompiled_AVATAR.TILE.BLANK;
                U4_Decompiled_AVATAR.TILE rightTile = U4_Decompiled_AVATAR.TILE.BLANK;

                if (y > 0)
                    aboveTile = map[x, y - 1];
                if (y < map.GetLength(1) - 1)
                    belowTile = map[x, y + 1];
                if (x > 0)
                    leftTile = map[x - 1, y];
                if (x < map.GetLength(0) - 1)
                    rightTile = map[x + 1, y];

                mapTile = Primitive.CreatePartialCube(!CheckShortTileForOpacity(leftTile), !CheckShortTileForOpacity(rightTile), !CheckShortTileForOpacity(aboveTile), !CheckShortTileForOpacity(belowTile));
            }
            else
            {
                mapTile = Primitive.CreatePartialCube();

            }
            mapTile.transform.SetParent(terrainGameObject.transform);
            mapTile.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
            rotation = Vector3.zero;
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make mountains into pyramids
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.MOUNTAINS)
        {
            mapTile = Primitive.CreatePyramid(1.0f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountatins to show their best side
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make dungeon entrace into pyramid, rotate so it faces the right direction
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.DUNGEON)
        {
            mapTile = Primitive.CreatePyramid(0.2f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make brush and hills into short pyramids
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.BRUSH) || (tileIndex == U4_Decompiled_AVATAR.TILE.HILLS))
        {
            mapTile = Primitive.CreatePyramid(0.15f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make rocks into little bigger short pyramids since you cannot walk over them
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SMALL_ROCKS)
        {
            mapTile = Primitive.CreatePyramid(0.25f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // tress we need to stand upright and face the camera
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.TOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.VILLAGE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.RUINS) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.SHRINE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.ANKH) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_UP) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.LADDER_DOWN) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.COOKING_FIRE) ||
            (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE))
        {
            mapTile = Primitive.CreateQuad();
            mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            // put this in a resonable rotation, combine3() will do the actual lookat rotaion just before displaying
            rotation = new Vector3(-90.0f, -90.0f, 90.0f);

            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE)
        {
            mapTile = Primitive.CreateBridge();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_TOP)
        {
            mapTile = Primitive.CreateBridgeUpper();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRIDGE_BOTTOM)
        {
            mapTile = Primitive.CreateBridgeLower();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if ((tileIndex == U4_Decompiled_AVATAR.TILE.DOOR) || (tileIndex == U4_Decompiled_AVATAR.TILE.LOCKED_DOOR))
        {
            mapTile = Primitive.CreateDoor();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.BRICK_FLOOR_COLUMN)
        {
            mapTile = Primitive.CreatePillar();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_MAST)
        {
            mapTile = Primitive.CreateMast();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.SHIP_WHEEL)
        {
            mapTile = Primitive.CreateWheel();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CHEST)
        {
            mapTile = Primitive.CreateChest();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_LEFT)
        {
            mapTile = Primitive.CreateCastleLeft();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_RIGHT)
        {
            mapTile = Primitive.CreateCastleRight();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == U4_Decompiled_AVATAR.TILE.CASTLE_ENTRANCE)
        {
            mapTile = Primitive.CreateCastleCenter();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // all other terrain tiles are flat
        else
        {
            mapTile = Primitive.CreateQuad();

            // water, lava and entergy fields need to be handled separately so we can animate the texture using UV
            // TODO may need to have single textures for the three water tiles if we want to use UV animation to show wind direction
            if ((tileIndex <= U4_Decompiled_AVATAR.TILE.SHALLOW_WATER) ||
                (tileIndex >= U4_Decompiled_AVATAR.TILE.POISON_FIELD && tileIndex <= U4_Decompiled_AVATAR.TILE.SLEEP_FIELD)
                || (tileIndex == U4_Decompiled_AVATAR.TILE.LAVA))
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
            renderer.material = Tile.combinedExpandedMaterial;
            renderer.material.mainTexture = Tile.combinedExpandedTexture;
            renderer.material.mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
            renderer.material.mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

        }
        else if (useLinearTile)
        {
            renderer.material = Tile.combinedLinearMaterial;
            renderer.material.mainTexture = Tile.combinedLinearTexture;
            renderer.material.mainTextureOffset = new Vector2((float)((int)tileIndex * Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
            renderer.material.mainTextureScale = new Vector2((float)Tile.originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
        }
        else
        {
            renderer.material.mainTexture = Tile.originalTiles[(int)tileIndex];
            renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
            renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
        }

        Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
        Vector2[] uv = new Vector2[mesh.uv.Length];
        Vector2 textureAtlasOffset;

        textureAtlasOffset = new Vector2((int)tileIndex % Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileWidth, (int)tileIndex / Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileHeight);
        for (int u = 0; u < mesh.uv.Length; u++)
        {
            Vector2 mainTextureOffset;
            Vector2 mainTextureScale;

            if (useExpandedTile)
            {
                mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);
            }
            else if (useLinearTile)
            {
                mainTextureOffset = new Vector2((float)((int)tileIndex * Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, 0.0f);
                mainTextureScale = new Vector2((float)Tile.originalTileWidth / (float)renderer.material.mainTexture.width, 1.0f);
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

    GameObject GetCachedTileGameObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, U4_Decompiled_AVATAR.TILE tileIndex, ref U4_Decompiled_AVATAR.TILE[,] map, int x, int y, bool allWalls)
    {
        if (allMapTilesGameObjects == null)
        {
            allMapTilesGameObjects = new GameObject[(int)U4_Decompiled_AVATAR.TILE.MAX];
            for (int i = 0; i < (int)U4_Decompiled_AVATAR.TILE.MAX; i++)
            {
                allMapTilesGameObjects[i] = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, (U4_Decompiled_AVATAR.TILE)i, ref map, 0, 0, true);
            }
        }

        return allMapTilesGameObjects[(int)tileIndex];
    }


    public void CreateMapSubsetPass2(GameObject mapGameObject, ref U4_Decompiled_AVATAR.TILE[,] map, ref GameObject[,] mapGameObjects, bool allWalls = false)
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
                U4_Decompiled_AVATAR.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == U4_Decompiled_AVATAR.TILE.BLANK)
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


    // Start is called before the first frame update
    void Awake()
    {
 
    }

    // cast one ray
    void Cast_Ray(ref U4_Decompiled_AVATAR.TILE[,] map, 
        int diff_x,
        int diff_y,
        int pos_x,
        int pos_y, 
        ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        U4_Decompiled_AVATAR.TILE temp_tile;

        // are we outside the destination raycast map area, stop here
        if (pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1) || pos_x - offset_x < 0 || pos_y - offset_y < 0)
        {
            return;
        }

        // has the tile already been copied, if so stop here
        if (raycastMap[pos_x - offset_x, pos_y - offset_y] != U4_Decompiled_AVATAR.TILE.BLANK)
        {
            return;
        }

        // check if we should wrap the source map or if we should fill
        // any tile outside of the map area with a specific tile such as GRASS
        // are we outside the source map?
        if ((wrapTile != U4_Decompiled_AVATAR.TILE.BLANK) && (pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1) || pos_x < 0 || pos_y < 0))
        {
            temp_tile = wrapTile;
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }
        else
        {
            // get the tile and copy it to the raycast map
            temp_tile = map[(pos_x + map.GetLength(0)) % map.GetLength(0), (pos_y + map.GetLength(1)) % map.GetLength(1)];
            raycastMap[pos_x - offset_x, pos_y - offset_y] = temp_tile;
        }

        // check the tile for opaque tiles
        if ((temp_tile == U4_Decompiled_AVATAR.TILE.FOREST) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.MOUNTAINS) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BLANK) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == U4_Decompiled_AVATAR.TILE.BRICK_WALL))
        {
            return;
        }

        // continue the ray cast recursively
        pos_x = (pos_x + diff_x);
        pos_y = (pos_y + diff_y);
        Cast_Ray(ref map, diff_x, diff_y, pos_x, pos_y, ref raycastMap, offset_x, offset_y, wrapTile);
        
        if ((diff_x & diff_y) != 0)
        {
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                pos_x, 
                (pos_y - diff_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                diff_x, 
                diff_y, 
                (pos_x - diff_x), 
                pos_y, 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
        else
        {
            Cast_Ray(ref map, 
                (((diff_x == 0) ? 1 : 0) * diff_y + diff_x), 
                (diff_y - ((diff_y == 0) ? 1 : 0) * diff_x), 
                (diff_y + pos_x), 
                (pos_y - diff_x), 
                ref raycastMap, offset_x, offset_y, wrapTile);
            Cast_Ray(ref map, 
                (diff_x - ((diff_x == 0) ? 1 : 0) * diff_y), 
                (((diff_y == 0) ? 1 : 0) * diff_x + diff_y), 
                (pos_x - diff_y), 
                (diff_x + pos_y), 
                ref raycastMap, offset_x, offset_y, wrapTile);
        }
    }

    // visible area (raycast)
    void raycast(ref U4_Decompiled_AVATAR.TILE[,] map, int pos_x, int pos_y, ref U4_Decompiled_AVATAR.TILE[,] raycastMap, int offset_x, int offset_y, U4_Decompiled_AVATAR.TILE wrapTile)
    {
        if (pos_x < 0 || pos_y < 0 || pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1))
        {
            Debug.Log("start position is outside of source map ( " + pos_x + ", " + pos_y + ")");
            return;
        }

        if (pos_x - offset_x < 0 || pos_y - offset_y < 0 || pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1))
        {
            Debug.Log("offset does not contain the starting position given the dimensions of the destination raycast map " 
                + "position ( " + pos_x + ", " + pos_y + ")" 
                + " offset (" + offset_x + ", " + offset_y + ")" 
                + " dimensions (" + raycastMap.GetLength(0) + ", " + raycastMap.GetLength(1) + ")");
            return;
        }

        // set all visible tiles in the destination raycast map to blank to start
        for (int y = 0; y < raycastMap.GetLength(1); y++)
        {
            for (int x = 0; x < raycastMap.GetLength(0); x++)
            {
                raycastMap[x, y] = U4_Decompiled_AVATAR.TILE.BLANK;
            }
        }

        // copy the starting position as it is alway visible given the map offset
        U4_Decompiled_AVATAR.TILE currentTile = map[pos_x, pos_y];
        raycastMap[pos_x - offset_x, pos_y - offset_y] = currentTile;

        // cast out recusively from the starting position
        Cast_Ray(ref map, 0, -1, pos_x, (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP
        Cast_Ray(ref map, 0, 1, pos_x, (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN
        Cast_Ray(ref map, -1, 0, (pos_x - 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray LEFT
        Cast_Ray(ref map, 1, 0, (pos_x + 1), pos_y, ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray RIGHT
        Cast_Ray(ref map, 1, 1, (pos_x + 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the RIGHT
        Cast_Ray(ref map, 1, -1, (pos_x + 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the RIGHT
        Cast_Ray(ref map, -1, 1, (pos_x - 1), (pos_y + 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray DOWN and to the LEFT
        Cast_Ray(ref map, -1, -1, (pos_x - 1), (pos_y - 1), ref raycastMap, offset_x, offset_y, wrapTile); // Cast a ray UP and to the LEFT
    }

    public Image picture;
    public Texture2D pictureTexture;
    public Hashtable pictureTextureAtlas = new Hashtable();
    public Hashtable pictureRawAtlas = new Hashtable();
    public string lastPictureFilename;
    public int lastPictureDest;


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

        // initialize the palette and load the tiles
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();
        Tile.LoadTilesEGA();
        //Tile.LoadTilesCGA();
        //Tile.LoadTilesApple2();
        //Tile.LoadTilesPNG();

        // fix a tile
        Tile.FixMageTile3();

        // expand the tiles
        Tile.ExpandTiles();

        // create texture atlas
        Tile.CreateLinearTextureAtlas(ref Tile.originalTiles);
        Tile.CreateSquareTextureAtlas(ref Tile.originalTiles);
        Tile.CreateExpandedTextureAtlas(ref Tile.expandedTiles);

        // get the font
        GameFont.LoadCharSetEGA();
        //LoadCharSetCGA();
        GameFont.ImportFontFromTexture(myFont, myTransparentFont, GameFont.fontAtlas, GameFont.fontTransparentAtlas);

        // set all the text objects to myFont in the input panel
        Text[] text = InputPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set again just the button text objects in the input panel to myTransparentFont
        Button[] buttons = InputPanel.GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            Text[] texts = b.GetComponentsInChildren<Text>(true);
            foreach (Text t in texts)
            {
                t.font = myTransparentFont;
            }
        }


        // get a reference to the game engine
        u4_TITLE = FindObjectOfType<U4_Decompiled_TITLE>();


        // allocate the onscreen texture
        pictureTexture = new Texture2D(320, 200);
        pictureTexture.filterMode = FilterMode.Point;
        Picture.ClearTexture(pictureTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // set the onscreen texture to the sprite
        picture.sprite = Sprite.Create(pictureTexture, new Rect(0, 0, pictureTexture.width, pictureTexture.height), new Vector2(0.5f, 0.5f));
        picture.color = Color.white;

        // everything I need it now loaded, start the game engine thread
        u4_TITLE.StartThread();
    }



    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;

    // used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f;

    GameObject hiddenWorldMapGameObject;
    U4_Decompiled_TITLE.INPUT_MODE lastInputMode = 0;

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
            if (Tile.textureExpandedAtlasPowerOf2 != 0)
            {
                Tile.AnimateFlags();
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

            if (u4_TITLE.gameText != null && GameText != null)
            {
                GameText.GetComponentInChildren<UnityEngine.UI.Text>().text = u4_TITLE.gameText;
            }

            if (lastInputMode != u4_TITLE.inputMode)
            {
                if (lastInputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
                {
                    // clear the text after a name is entered
                    u4_TITLE.gameText = "";
                }

                lastInputMode = u4_TITLE.inputMode;

                if (u4_TITLE.inputMode != U4_Decompiled_TITLE.INPUT_MODE.LAUNCH_GAME)
                {
                    InputPanel.SetActive(true);
                }
                else
                {
                    InputPanel.SetActive(false);
                    Application.LoadLevel(1);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU)
                {
                    u4_TITLE.gameText = "In another world, in a time to come.\n \nOptions:\n";
                    GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerCenter;

                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30);

                    u4_TITLE.specialEffectAudioSource.Stop();

                    MainMainLoop.SetActive(true);
                }
                else
                {
                    MainMainLoop.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME)
                {
                    u4_TITLE.gameText = "By what name shalt thou be known in this world and time? \n \n              ";
                    GameText.GetComponentInChildren<Text>().alignment = TextAnchor.LowerLeft;

                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 40);

                    Keyboard.SetActive(true);
                    KeyboardUpper.SetActive(true);
                    KeyboardLower.SetActive(false);
                }
                else
                {
                    Keyboard.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE) ||
                       (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU))
                {
                    GameText.SetActive(true);
                }
                else
                {
                    GameText.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE)||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE))
                {
                    RectTransform rt = GameText.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 94);

                    rt = PictureGameObject.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 155);
                }
                else
                {
                    RectTransform rt = PictureGameObject.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 99);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MAIN_MENU) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_NO_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.NAME))
                {
                    PictureGameObject.SetActive(true);
                }
                else
                {
                    PictureGameObject.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.A_OR_B_CHOICE)
                {
                    TalkChoice.SetActive(true);
                }
                else
                {
                    TalkChoice.SetActive(false);
                }

                if (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.MALE_OR_FEMALE)
                {
                    TalkMF.SetActive(true);
                }
                else
                {
                    TalkMF.SetActive(false);
                }

                if ((u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_CONTINUE) ||
                        (u4_TITLE.inputMode == U4_Decompiled_TITLE.INPUT_MODE.DELAY_TEXT_CONTINUE))
                {
                    TalkContinue.SetActive(true);
                }
                else
                {
                    TalkContinue.SetActive(false);
                }
            }

            while (u4_TITLE.screenCopyFrameQueue.Count != 0)
            {
                U4_Decompiled_TITLE.ScreenCopyFrame screenCopyFrame = u4_TITLE.screenCopyFrameQueue.Dequeue();

                Texture2D sourceTexture = (Texture2D)pictureTextureAtlas[screenCopyFrame.p];
                byte[] raw = (byte[])pictureRawAtlas[screenCopyFrame.p];

                if (sourceTexture != null && raw != null)
                {
                    if (screenCopyFrame.random_stuff == -1)
                    {
                        Graphics.CopyTexture(
                            sourceTexture,
                            0,
                            0,
                            screenCopyFrame.src_x_in_char * 8,
                            sourceTexture.height - screenCopyFrame.src_y - screenCopyFrame.height,
                            screenCopyFrame.width_in_char * 8,
                            screenCopyFrame.height,
                            pictureTexture,
                            0,
                            0,
                            screenCopyFrame.dst_x_in_char * 8,
                            pictureTexture.height - screenCopyFrame.dst_y - screenCopyFrame.height);
                    }
                    else
                    {
                        Picture.CopyTexture2D(
                            raw,
                            screenCopyFrame.src_x_in_char * 8,
                            screenCopyFrame.src_y,
                            screenCopyFrame.dst_x_in_char * 8,
                            screenCopyFrame.dst_y,
                            screenCopyFrame.width_in_char * 8,
                            screenCopyFrame.height,
                            screenCopyFrame.random_stuff,
                            pictureTexture);
                    }
                }
            }

            while (u4_TITLE.loadPictureQueue.Count != 0)
            {
                U4_Decompiled_TITLE.LoadPicture loadPicture = u4_TITLE.loadPictureQueue.Dequeue();
                if (loadPicture.filename.Length > 0)
                {
                    lastPictureFilename = loadPicture.filename;
                    lastPictureDest = loadPicture.dest;

                    if (!pictureTextureAtlas.ContainsKey(lastPictureDest))
                    {
                        // create new texture
                        Texture2D addPictureTexture = new Texture2D(320, 200);

                        byte[] destRaw = Picture.LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));
                        Picture.EGA_To_Texture2D(destRaw, addPictureTexture);

                        pictureTextureAtlas.Add(lastPictureDest, addPictureTexture);
                        pictureRawAtlas.Add(lastPictureDest, destRaw);
                    }
                    else
                    {
                        // update texture with new picture from file
                        byte[] destRaw = Picture.LoadTITLEEGAPictureFile(loadPicture.filename.ToLower().Replace(".pic", ".ega"));
                        Picture.EGA_To_Texture2D(destRaw, (Texture2D)pictureTextureAtlas[lastPictureDest]);
                        pictureRawAtlas[loadPicture.dest] = destRaw;
                    }
                }
            }

            if (u4_TITLE.screenDotQueue.Count > 0)
            {
                while (u4_TITLE.screenDotQueue.Count != 0)
                {
                    U4_Decompiled_TITLE.ScreenDot screenDot = u4_TITLE.screenDotQueue.Dequeue();

                    // convert back to EGA colors because the game engine is running with different palette
                    if (screenDot.color == 3)
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BRIGHT_CYAN]);
                    }
                    else if (screenDot.color == 2)
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.RED]);
                    }
                    else
                    {
                        pictureTexture.SetPixel(screenDot.x, 199 - screenDot.y, Palette.CGAColorPalette[screenDot.color]);
                    }
                }
            }
            pictureTexture.Apply(); // TODO: try to do this only once per frame at the end to speed things up

            // TODO this is slower than other methods need to switch
            if (u4_TITLE.mapChanged)
            {
                u4_TITLE.mapChanged = false;
                CreateMap(gameObject, u4_TITLE.map);
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
}
