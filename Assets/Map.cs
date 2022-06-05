using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Map
{
    //public static GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];

    public static GameObject[] allMapTilesGameObjects = null;

    public static bool CheckTileForOpacity(Tile.TILE tileIndex)
    {
        return (tileIndex == Tile.TILE.BRICK_WALL
                    || tileIndex == Tile.TILE.LARGE_ROCKS
                    || tileIndex == Tile.TILE.SECRET_BRICK_WALL);
    }

    public static bool CheckShortTileForOpacity(Tile.TILE tileIndex)
    {
        return (CheckTileForOpacity(tileIndex) ||
                    ((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.BRACKET_SQUARE)));
    }


    public static void CreateMap(GameObject mapGameObject, Tile.TILE[,] map, Vector3 finalPosition, Vector3 finalRotation, GameObject lookAtObject = null)
    {
        GameObject terrainGameObject;
        GameObject animatedTerrrainGameObject;
        GameObject billboardTerrrainGameObject;
        bool useExpandedTile;

        // Position the map in place
        mapGameObject.transform.position = finalPosition; //  new Vector3(-5, 0, 7);

        // rotate map into place
        mapGameObject.transform.eulerAngles = finalRotation; // new Vector3(90.0f, 0.0f, 0.0f);

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
                Tile.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == Tile.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    Tile.TILE aboveTile = Tile.TILE.BLANK;
                    Tile.TILE belowTile = Tile.TILE.BLANK;
                    Tile.TILE leftTile = Tile.TILE.BLANK;
                    Tile.TILE rightTile = Tile.TILE.BLANK;

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
                else if (((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.BRACKET_SQUARE))
                    || (tileIndex == Tile.TILE.ARCHITECTURE))
                {
                    Tile.TILE aboveTile = Tile.TILE.BLANK;
                    Tile.TILE belowTile = Tile.TILE.BLANK;
                    Tile.TILE leftTile = Tile.TILE.BLANK;
                    Tile.TILE rightTile = Tile.TILE.BLANK;

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
                else if (tileIndex == Tile.TILE.DIAGONAL_WATER_ARCHITECTURE1)
                {
                    mapTile = Primitive.CreateWedge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.25f);
                    rotation = Vector3.zero;
                    useExpandedTile = true;
                }
                // make mountains into pyramids
                else if (tileIndex == Tile.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make dungeon entrace into pyramid, rotate so it faces the right direction
                else if (tileIndex == Tile.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == Tile.TILE.BRUSH) || (tileIndex == Tile.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == Tile.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                    useExpandedTile = true;
                }
                // trees we need to stand upright and face the camera
                else if ((tileIndex == Tile.TILE.FOREST) ||
                    (tileIndex == Tile.TILE.TOWN) ||
                    (tileIndex == Tile.TILE.VILLAGE) ||
                    (tileIndex == Tile.TILE.RUINS) ||
                    (tileIndex == Tile.TILE.ANKH) ||
                    (tileIndex == Tile.TILE.ALTAR) ||
                    // (tileIndex == U4_Decompiled.TILE.CHEST) ||
                    (tileIndex == Tile.TILE.LADDER_UP) ||
                    (tileIndex == Tile.TILE.LADDER_DOWN) ||
                    (tileIndex == Tile.TILE.COOKING_FIRE) ||
                    (tileIndex == Tile.TILE.PARTY) || // the shrine map uses a fixed party tile instead of putting the party characters into the map
                    (tileIndex == Tile.TILE.CASTLE))
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

                    if (lookAtObject)
                    {
                        Transform look = lookAtObject.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these bulboards
                        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                        mapTile.transform.LookAt(look.transform);
                        //mapTile.transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
                        rotation = mapTile.transform.localEulerAngles; // new Vector3(rotx, -90f, 90.0f);
                        rotation.x = rotation.x - 180.0f;
                    }

                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.BRIDGE)
                {
                    mapTile = Primitive.CreateBridge();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.BRIDGE_TOP)
                {
                    mapTile = Primitive.CreateBridgeUpper();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.BRIDGE_BOTTOM)
                {
                    mapTile = Primitive.CreateBridgeLower();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if ((tileIndex == Tile.TILE.DOOR) || (tileIndex == Tile.TILE.LOCKED_DOOR))
                {
                    mapTile = Primitive.CreateDoor();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.BRICK_FLOOR_COLUMN)
                {
                    mapTile = Primitive.CreatePillar();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.SHIP_MAST)
                {
                    mapTile = Primitive.CreateMast();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.SHIP_WHEEL)
                {
                    mapTile = Primitive.CreateWheel();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.CHEST)
                {
                    mapTile = Primitive.CreateChest();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.CASTLE_LEFT)
                {
                    mapTile = Primitive.CreateCastleLeft();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.CASTLE_RIGHT)
                {
                    mapTile = Primitive.CreateCastleRight();
                    mapTile.transform.SetParent(terrainGameObject.transform);
                    rotation = new Vector3(-90.0f, 0.0f, 0.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
                    useExpandedTile = true;
                }
                else if (tileIndex == Tile.TILE.CASTLE_ENTRANCE)
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
                    if ((tileIndex <= Tile.TILE.SHALLOW_WATER) ||
                        (tileIndex >= Tile.TILE.POISON_FIELD && tileIndex <= Tile.TILE.SLEEP_FIELD)
                        || (tileIndex == Tile.TILE.LAVA))
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
        // don't combine these as we need to rotate them to keep the bill board effect, cost much more time to recreate with a combine mesh than the leave them separate
        //Combine.Combine1(billboardTerrrainGameObject); // combine separately from terrain above as we need to point these towards the player

        // add our little water animator script
        // adding a script component in the editor is a significant performance hit, avoid adding if already present

        if (animatedTerrrainGameObject.GetComponent<Animate1>() == null)
        {
            animatedTerrrainGameObject.AddComponent<Animate1>();
        }
    }


    public static void UpdateExistingBillboardsMap(GameObject mapGameObject, Tile.TILE[,] map, Vector3 finalPosition, Vector3 finalRotation, GameObject lookAtObject = null)
    {
        GameObject billboardTerrrainGameObject;
        //bool useExpandedTile;

        // Position the map in place
        //mapGameObject.transform.position = finalPosition; //  new Vector3(-5, 0, 7);

        // rotate map into place
        //mapGameObject.transform.eulerAngles = finalRotation; // new Vector3(90.0f, 0.0f, 0.0f);

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

        // update any children
        foreach (Transform child in billboardTerrrainGameObject.transform)
        {
            // make it billboard
            Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
            look.position = new Vector3(look.position.x, 0.0f, look.position.z);
            child.transform.LookAt(look.transform);
            Vector3 rot = child.transform.eulerAngles;
            child.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
        }
    }

    public static void CreateMapSubset(GameObject mapGameObject, Tile.TILE[,] map, ref GameObject[,] mapGameObjects)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                GameObject mapTile;
                Vector3 location = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                Tile.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == Tile.TILE.BLANK)
                {
                    // skip it
                    continue;
                }
                // solid object, brick, rocks etc. make into cubes
                else if (CheckTileForOpacity(tileIndex))
                {
                    Tile.TILE aboveTile = Tile.TILE.BLANK;
                    Tile.TILE belowTile = Tile.TILE.BLANK;
                    Tile.TILE leftTile = Tile.TILE.BLANK;
                    Tile.TILE rightTile = Tile.TILE.BLANK;

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
                else if (((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.BRACKET_SQUARE))
                    || (tileIndex == Tile.TILE.ARCHITECTURE))
                {
                    Tile.TILE aboveTile = Tile.TILE.BLANK;
                    Tile.TILE belowTile = Tile.TILE.BLANK;
                    Tile.TILE leftTile = Tile.TILE.BLANK;
                    Tile.TILE rightTile = Tile.TILE.BLANK;

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
                else if (tileIndex == Tile.TILE.MOUNTAINS)
                {
                    mapTile = Primitive.CreatePyramid(1.0f);
                    rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountains to show their best side
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make dungeon entrance into pyramid, rotate so it faces the right direction
                else if (tileIndex == Tile.TILE.DUNGEON)
                {
                    mapTile = Primitive.CreatePyramid(0.2f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make brush and hills into short pyramids
                else if ((tileIndex == Tile.TILE.BRUSH) || (tileIndex == Tile.TILE.HILLS))
                {
                    mapTile = Primitive.CreatePyramid(0.15f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // make rocks into little bigger short pyramids since you cannot walk over them
                else if (tileIndex == Tile.TILE.SMALL_ROCKS)
                {
                    mapTile = Primitive.CreatePyramid(0.25f);
                    rotation = new Vector3(0.0f, 180.0f, 90.0f);
                    location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
                }
                // tress we need to stand upright and face the camera
                else if ((tileIndex == Tile.TILE.FOREST) ||
                    (tileIndex == Tile.TILE.TOWN) ||
                    (tileIndex == Tile.TILE.ANKH) ||
                    (tileIndex == Tile.TILE.LADDER_UP) ||
                    (tileIndex == Tile.TILE.LADDER_DOWN) ||
                    (tileIndex == Tile.TILE.COOKING_FIRE) ||
                    (tileIndex == Tile.TILE.CASTLE))
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
                    if ((tileIndex <= Tile.TILE.SHALLOW_WATER) ||
                        (tileIndex >= Tile.TILE.POISON_FIELD && tileIndex <= Tile.TILE.SLEEP_FIELD)
                        || (tileIndex == Tile.TILE.LAVA))
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
                mapGameObjects[x, y] = mapTile;
            }
        }
    }

    public static GameObject CreateMapTileObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, Tile.TILE tileIndex, ref Tile.TILE[,] map, int x, int y, bool allWalls)
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
                Tile.TILE aboveTile = Tile.TILE.BLANK;
                Tile.TILE belowTile = Tile.TILE.BLANK;
                Tile.TILE leftTile = Tile.TILE.BLANK;
                Tile.TILE rightTile = Tile.TILE.BLANK;

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
        else if (((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.BRACKET_SQUARE))
            || (tileIndex == Tile.TILE.ARCHITECTURE))
        {
            if (allWalls == false)
            {
                Tile.TILE aboveTile = Tile.TILE.BLANK;
                Tile.TILE belowTile = Tile.TILE.BLANK;
                Tile.TILE leftTile = Tile.TILE.BLANK;
                Tile.TILE rightTile = Tile.TILE.BLANK;

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
        else if (tileIndex == Tile.TILE.MOUNTAINS)
        {
            mapTile = Primitive.CreatePyramid(1.0f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 0.0f); // rotate mountatins to show their best side
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make dungeon entrace into pyramid, rotate so it faces the right direction
        else if (tileIndex == Tile.TILE.DUNGEON)
        {
            mapTile = Primitive.CreatePyramid(0.2f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make brush and hills into short pyramids
        else if ((tileIndex == Tile.TILE.BRUSH) || (tileIndex == Tile.TILE.HILLS))
        {
            mapTile = Primitive.CreatePyramid(0.15f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // make rocks into little bigger short pyramids since you cannot walk over them
        else if (tileIndex == Tile.TILE.SMALL_ROCKS)
        {
            mapTile = Primitive.CreatePyramid(0.25f);
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(0.0f, 180.0f, 90.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.5f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        // tress we need to stand upright and face the camera
        else if ((tileIndex == Tile.TILE.FOREST) ||
            (tileIndex == Tile.TILE.TOWN) ||
            (tileIndex == Tile.TILE.VILLAGE) ||
            (tileIndex == Tile.TILE.RUINS) ||
            (tileIndex == Tile.TILE.SHRINE) ||
            (tileIndex == Tile.TILE.ANKH) ||
            (tileIndex == Tile.TILE.LADDER_UP) ||
            (tileIndex == Tile.TILE.LADDER_DOWN) ||
            (tileIndex == Tile.TILE.COOKING_FIRE) ||
            (tileIndex == Tile.TILE.CASTLE))
        {
            mapTile = Primitive.CreateQuad();
            mapTile.transform.SetParent(billboardTerrrainGameObject.transform);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            // put this in a resonable rotation, combine3() will do the actual lookat rotaion just before displaying
            rotation = new Vector3(-90.0f, -90.0f, 90.0f);

            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.BRIDGE)
        {
            mapTile = Primitive.CreateBridge();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.BRIDGE_TOP)
        {
            mapTile = Primitive.CreateBridgeUpper();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.BRIDGE_BOTTOM)
        {
            mapTile = Primitive.CreateBridgeLower();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if ((tileIndex == Tile.TILE.DOOR) || (tileIndex == Tile.TILE.LOCKED_DOOR))
        {
            mapTile = Primitive.CreateDoor();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.BRICK_FLOOR_COLUMN)
        {
            mapTile = Primitive.CreatePillar();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.SHIP_MAST)
        {
            mapTile = Primitive.CreateMast();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.SHIP_WHEEL)
        {
            mapTile = Primitive.CreateWheel();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.CHEST)
        {
            mapTile = Primitive.CreateChest();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.CASTLE_LEFT)
        {
            mapTile = Primitive.CreateCastleLeft();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.CASTLE_RIGHT)
        {
            mapTile = Primitive.CreateCastleRight();
            mapTile.transform.SetParent(terrainGameObject.transform);
            rotation = new Vector3(-90.0f, 0.0f, 0.0f);
            location = new Vector3(x, map.GetLength(1) - 1 - y, 0.0f);
            useExpandedTile = true;
            useLinearTile = false;
        }
        else if (tileIndex == Tile.TILE.CASTLE_ENTRANCE)
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
            if ((tileIndex <= Tile.TILE.SHALLOW_WATER) ||
                (tileIndex >= Tile.TILE.POISON_FIELD && tileIndex <= Tile.TILE.SLEEP_FIELD)
                || (tileIndex == Tile.TILE.LAVA))
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

    public static GameObject GetCachedTileGameObject(GameObject terrainGameObject, GameObject billboardTerrrainGameObject, GameObject animatedTerrrainGameObject, Tile.TILE tileIndex, ref Tile.TILE[,] map, int x, int y, bool allWalls)
    {
        if (allMapTilesGameObjects == null)
        {
            allMapTilesGameObjects = new GameObject[(int)Tile.TILE.MAX];
            for (int i = 0; i < (int)Tile.TILE.MAX; i++)
            {
                allMapTilesGameObjects[i] = CreateMapTileObject(terrainGameObject, billboardTerrrainGameObject, animatedTerrrainGameObject, (Tile.TILE)i, ref map, 0, 0, true);
            }
        }

        return allMapTilesGameObjects[(int)tileIndex];
    }


    public static  void CreateMapSubsetPass2(GameObject mapGameObject, ref Tile.TILE[,] map, ref GameObject[,] mapGameObjects, bool allWalls = false)
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
                Tile.TILE tileIndex;

                tileIndex = map[x, y];

                // check if it tile is blank
                if (tileIndex == Tile.TILE.BLANK)
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

    // cast one ray
    public static void Cast_Ray(ref Tile.TILE[,] map,
        int diff_x,
        int diff_y,
        int pos_x,
        int pos_y,
        ref Tile.TILE[,] raycastMap, int offset_x, int offset_y, Tile.TILE wrapTile)
    {
        Tile.TILE temp_tile;

        // are we outside the destination raycast map area, stop here
        if (pos_x - offset_x >= raycastMap.GetLength(0) || pos_y - offset_y >= raycastMap.GetLength(1) || pos_x - offset_x < 0 || pos_y - offset_y < 0)
        {
            return;
        }

        // has the tile already been copied, if so stop here
        if (raycastMap[pos_x - offset_x, pos_y - offset_y] != Tile.TILE.BLANK)
        {
            return;
        }

        // check if we should wrap the source map or if we should fill
        // any tile outside of the map area with a specific tile such as GRASS
        // are we outside the source map?
        if ((wrapTile != Tile.TILE.BLANK) && (pos_x >= map.GetLength(0) || pos_y >= map.GetLength(1) || pos_x < 0 || pos_y < 0))
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
        if ((temp_tile == Tile.TILE.FOREST) ||
            (temp_tile == Tile.TILE.MOUNTAINS) ||
            (temp_tile == Tile.TILE.BLANK) ||
            (temp_tile == Tile.TILE.SECRET_BRICK_WALL) ||
            (temp_tile == Tile.TILE.BRICK_WALL))
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
    public static void raycast(ref Tile.TILE[,] map, int pos_x, int pos_y, ref Tile.TILE[,] raycastMap, int offset_x, int offset_y, Tile.TILE wrapTile)
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
                raycastMap[x, y] = Tile.TILE.BLANK;
            }
        }

        // copy the starting position as it is alway visible given the map offset
        Tile.TILE currentTile = map[pos_x, pos_y];
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
        public Tile.TILE tileIndex;
        public int x;
        public int y;
    }

    public static void CreateMapLabels(GameObject mapGameObject, ref Tile.TILE[,] map)
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
                Tile.TILE tileIndex;

                tileIndex = map[x, y];

                // check if a tile is a letter
                if ((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.Z) || (tileIndex == Tile.TILE.SPACE))
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
                        if ((tileIndex >= Tile.TILE.A) && (tileIndex <= Tile.TILE.Z) || (tileIndex == Tile.TILE.SPACE))
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
                Tile.TILE reverseTileIndex = label.letters[label.letters.Count - 1 - i].tileIndex;

                if (letter.tileIndex != Tile.TILE.SPACE)
                {
                    labelGameObject.name += (char)(letter.tileIndex - Tile.TILE.A + 'A');
                }
                else
                {
                    labelGameObject.name += ' ';
                }
                if (reverseTileIndex != Tile.TILE.SPACE)
                {
                    //reverseLabelGameObject.name += (char)(reverseTileIndex - U4_Decompiled.TILE.A + 'A');
                    reverseLabelGameObject.name += (char)(letter.tileIndex - Tile.TILE.A + 'A');
                }
                else
                {
                    reverseLabelGameObject.name += ' ';
                }

                // create the gameObject tile
                GameObject mapTile = Primitive.CreatePartialCube();
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
                renderer.material = Tile.combinedExpandedMaterial;
                renderer.material.mainTexture = Tile.combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

                Mesh mesh = mapTile.GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 textureAtlasOffset;

                textureAtlasOffset = new Vector2((int)letter.tileIndex % Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileWidth, (int)letter.tileIndex / Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)letter.tileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)letter.tileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);

                // create another game object to match the first but we will reverse the letters
                GameObject reverseMapTile = Primitive.CreatePartialCube();
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
                renderer.material = Tile.combinedExpandedMaterial;
                renderer.material.mainTexture = Tile.combinedExpandedTexture;
                renderer.material.mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

                mesh = reverseMapTile.GetComponent<MeshFilter>().mesh;
                uv = new Vector2[mesh.uv.Length];

                textureAtlasOffset = new Vector2((int)reverseTileIndex % Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileWidth, (int)reverseTileIndex / Tile.textureExpandedAtlasPowerOf2 * Tile.expandedTileHeight);
                for (int u = 0; u < mesh.uv.Length; u++)
                {
                    Vector2 mainTextureOffset;
                    Vector2 mainTextureScale;

                    mainTextureOffset = new Vector2((float)(Tile.TILE_BORDER_SIZE + (((int)reverseTileIndex % 16) * Tile.expandedTileWidth)) / (float)renderer.material.mainTexture.width, (float)(Tile.TILE_BORDER_SIZE + (((int)reverseTileIndex / 16) * Tile.expandedTileHeight)) / (float)renderer.material.mainTexture.height);
                    mainTextureScale = new Vector2((float)(Tile.originalTileWidth) / (float)renderer.material.mainTexture.width, (float)(Tile.originalTileHeight) / (float)renderer.material.mainTexture.height);

                    uv[u] = Vector2.Scale(mesh.uv[u], mainTextureScale);
                    uv[u] += (textureAtlasOffset + mainTextureOffset);
                }
                mesh.uv = uv;

                renderer.material.mainTextureOffset = new Vector2(0.0f, 0.0f);
                renderer.material.mainTextureScale = new Vector2(1.0f, 1.0f);
            }

            Combine.Combine1(labelGameObject);
            Combine.Combine1(reverseLabelGameObject);
        }
    }
}
