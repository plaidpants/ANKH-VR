using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public Tile.TILE_TYPE currentTileType;

    // used for automatic klimb and decsend ladders
    public Tile.TILE lastCurrentTile = (Tile.TILE)(-1);

    public Font myFont;
    public Font myTransparentFont;

    // mainTerrain holds the terrain, animatedTerrrain, billboardTerrrain
    public GameObject mainTerrain;
    public GameObject terrain;
    public GameObject animatedTerrrain;
    public GameObject billboardTerrrain;

    // these are fixed in space 
    public GameObject npcs;
    public GameObject party;
    public GameObject fighters;
    public GameObject characters;
    public GameObject activeCharacter;
    public GameObject hits;
    public GameObject moongate;
    public GameObject partyGameObject;
    public GameObject skyGameObject;
    public GameObject dungeon;
    public GameObject dungeonMonsters;

    public List<string> talkWordList = new List<string>();

    public Text keyword1ButtonText;
    public Text keyword2ButtonText;
    public Text DungeonMapText;
    public GameObject keyword1Button;
    public GameObject keyword2Button;
    public GameObject InputPanel;
    public GameObject StatsPanel;
    public GameObject TextPanel;
    public GameObject TalkLabel;
    public GameObject ActionLabel;
    public GameObject ActionMainLoopInputPanel;
    public GameObject ActionDungeonLoopInputPanel;
    public GameObject ActionCombatLoopInputPanel;
    public GameObject TalkCitizen;
    public GameObject TalkHealer;
    public GameObject TalkContinue;
    public Button TalkContinueButton;
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
    public GameObject TalkVirtue;
    public GameObject TalkDirection;
    public GameObject TalkEndGame;

    public string lastVisionFilename;

    public GameObject statsOverview;
    public GameObject windDirection;
    public GameObject moons;
    public GameObject trammelLight;
    public GameObject feluccaLight;
    public GameObject sunLight;

    public GameObject[] characterStatus = new GameObject[8];
    public GameObject weaponsStatus;
    public GameObject armourStatus;
    public GameObject equipmentStatus;
    public GameObject itemsStatus;
    public GameObject itemsStatusHeading;
    public GameObject reagentsStatus;
    public GameObject mixturesStatus;

    public U4_Decompiled_AVATAR.MODE lastMode = (U4_Decompiled_AVATAR.MODE)(-1);
    public U4_Decompiled_AVATAR.MODE lastModeCheck = (U4_Decompiled_AVATAR.MODE)(-1);
    
    public bool readyToAutomaticallyEnter = false;

    public Transform rotateTransform;
    //public GameObject convertMe;

    // reference to game engine
    public U4_Decompiled_AVATAR u4;

    // this array size can be adjusted to display more or less of the map at runtime
    Tile.TILE[,] raycastSettlementMap = new Tile.TILE[64, 64];

    // this array size can be adjusted to display more or less of the map at runtime
    Tile.TILE[,] raycastOutdoorMap = new Tile.TILE[128, 128];
    //   U4_Decompiled.TILE[,] raycastOutdoorMap = new U4_Decompiled.TILE[256, 256];

    GameObject[,] entireMapGameObjects = new GameObject[32 * 8, 32 * 8];

    public GameObject GameText;

    // changes in these require redrawing the map
    int lastRaycastPlayer_posx = -1;
    int lastRaycastPlayer_posy = -1;
    int lastRaycastPlayer_f_1dc = -1;
    U4_Decompiled_AVATAR.DIRECTION lastRaycastP_surface_party_direction = (U4_Decompiled_AVATAR.DIRECTION)(-1);
    U4_Decompiled_AVATAR.DIRECTION lastPartyDirection = (U4_Decompiled_AVATAR.DIRECTION)(-1);
    bool last_door_timer = false;
    Settlement.SETTLEMENT lastSettlement;

    public Image vision;
    Texture2D visionTexture;

    // Update is called once per frame
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float timerPeriod = 0.02f;

    // used for a flag animation timer
    float flagTimer = 0.0f;
    float flagTimerExpired = 0.0f;
    public float flagTimerPeriod = 0.10f;

    GameObject hiddenWorldMapGameObject;

    void CreateParty()
    {
        // create player/party object to display texture
        partyGameObject = Primitive.CreateQuad();
        partyGameObject.transform.SetParent(party.transform);
        
        // rotate the npc game object after creating and addition of child
        partyGameObject.transform.localPosition = new Vector3(0, 0, 0); 
        partyGameObject.transform.localEulerAngles = new Vector3(270.0f, 180.0f, 180.0f);

        // create child object for texture
        MeshRenderer renderer = partyGameObject.GetComponent<MeshRenderer>();

        // set the tile
        renderer.material.mainTexture = Tile.expandedTiles[(int)Tile.TILE.PARTY];
        renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        // set the shader
        renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

        // add so speech works
        partyGameObject.AddComponent<UnityEngine.UI.Text>();
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

    public void AddFighters(U4_Decompiled_AVATAR.t_68[] currentFighters, U4_Decompiled_AVATAR.tCombat1[] currentCombat, int offsetx = 0, int offsety = 0)
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
                GameObject fighterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = fighterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

                // add our little animator script and set the tile
                Animate3 animate = fighterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
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
            Tile.TILE npcTile = currentFighters[fighterIndex]._tile;
            Tile.TILE npcCurrentTile = currentFighters[fighterIndex]._gtile;

            // get the corresponding fighters game object
            Transform childoffighters = fighters.transform.GetChild(fighterIndex);

            if (npcTile == Tile.TILE.DEEP_WATER)
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
                childoffighters.GetComponent<Animate3>().SetNPCTile(Tile.TILE.SLEEP);
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

    public void AddCharacters(U4_Decompiled_AVATAR.tCombat2[] currentCombat2, U4_Decompiled_AVATAR.tParty currentParty, U4_Decompiled_AVATAR.t_68[] currentFighters, int offsetx = 0, int offsety = 0)
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
                GameObject characterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = characterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

                // add our little animator script and set the tile
                Animate3 animate = characterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
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
            Tile.TILE npcTile;

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

            if (npcTile == Tile.TILE.DEEP_WATER)
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
            GameObject moongateGameObject = Primitive.CreateQuad();

            // get the renderer
            renderer = moongateGameObject.GetComponent<MeshRenderer>();

            // intially the texture is null
            renderer.material.mainTexture = null;

            // set the shader
            renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

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

        //renderer.material.mainTexture = Tile.originalTiles[(int)u4.moongate_tile];
        renderer.material.mainTexture = Tile.expandedTiles[(int)u4.moongate_tile];
        renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
        renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

        // get adjusted position based on the offset of the raycastOutdoorMap due to the player position
        int posx = u4.moongate_x - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
        int posy = u4.moongate_y - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
        // can we see the npc
        if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == Tile.TILE.BLANK)
        {
            childofmoongate.gameObject.SetActive(false);
        }
        else
        {
            childofmoongate.gameObject.SetActive(true);
        }

        // update the position
        childofmoongate.localPosition = new Vector3(u4.moongate_x, Outdoor.outdoorMap.GetLength(1) - 1 - u4.moongate_y, 0); 

        //childofmoongate.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);
        // make it billboard
        Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
        look.position = new Vector3(look.position.x, 0.0f, look.position.z);
        childofmoongate.transform.LookAt(look.transform);
        Vector3 rot = childofmoongate.transform.eulerAngles;
        childofmoongate.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y, rot.z + 180.0f);
    }

    public void AddNPCs(U4_Decompiled_AVATAR.tNPC[] currentNpcs)
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
                GameObject npcGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = npcGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

                // add our little animator script and set the tile
                Animate3 animate = npcGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
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
            Tile.TILE npcTile = currentNpcs[npcIndex]._tile;
            Tile.TILE npcCurrentTile = currentNpcs[npcIndex]._gtile;

            // check if npc is active
            if (npcTile == Tile.TILE.DEEP_WATER)
            {
                // disable object if not active
                childofnpcs.gameObject.SetActive(false);
            }
            else
            {
                // get the npc position
                int posx = currentNpcs[npcIndex]._x;
                int posy = currentNpcs[npcIndex]._y;

                // inside settlements we need to check extra stuff
                if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
                {
                    Settlement.SETTLEMENT settlement;

                    // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
                    if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
                    {
                        settlement = Settlement.SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (Settlement.SETTLEMENT)u4.Party._loc;
                    }

                    // set the name of the game object to match the npc
                    if ((currentNpcs[npcIndex]._tlkidx == 0) || (currentNpcs[npcIndex]._tlkidx > 16 /* sometimes this is 127 */))
                    {
                        childofnpcs.name = npcTile.ToString();
                    }
                    else
                    {
                        childofnpcs.name = Settlement.npcStrings[(int)settlement][currentNpcs[npcIndex]._tlkidx - 1][(int)Settlement.NPC_STRING_INDEX.NAME];
                    }

                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastSettlementMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastSettlementMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastSettlementMap.GetLength(0) || posy >= raycastSettlementMap.GetLength(1) || raycastSettlementMap[posx, posy] == Tile.TILE.BLANK)
                    {
                        childofnpcs.gameObject.SetActive(false);
                    }
                    else
                    {
                        childofnpcs.gameObject.SetActive(true);
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
                {
                    // adjust position based on the offset of the raycastSettlementMap due to the player position
                    posx = posx - (lastRaycastPlayer_posx - raycastOutdoorMap.GetLength(0) / 2 - 1);
                    posy = posy - (lastRaycastPlayer_posy - raycastOutdoorMap.GetLength(1) / 2 - 1);
                    // can we see the npc
                    if (posx < 0 || posy < 0 || posx >= raycastOutdoorMap.GetLength(0) || posy >= raycastOutdoorMap.GetLength(1) || raycastOutdoorMap[posx, posy] == Tile.TILE.BLANK)
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
                childofnpcs.localPosition = new Vector3(currentNpcs[npcIndex]._x, Outdoor.outdoorMap.GetLength(1) - 1 - currentNpcs[npcIndex]._y, 0);

                // make it billboard
                Transform look = Camera.main.transform; // TODO we need to find out where the camera will be not where it is currently before pointing these billboards
                look.position = new Vector3(look.position.x, 0.0f, look.position.z);
                childofnpcs.transform.LookAt(look.transform);
                Vector3 rot = childofnpcs.transform.eulerAngles;
                childofnpcs.transform.eulerAngles = new Vector3(rot.x + 180.0f, rot.y , rot.z + 180.0f);
            }
        }
    }

    public void AddHits(List<U4_Decompiled_AVATAR.hit> currentHitList, int offsetx = 0, int offsety = 0)
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
                GameObject hitGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = hitGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

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
                Tile.TILE hitTile = currentHitList[hitIndex].tile;

                // update the tile of the game object
                Renderer renderer = childofhits.GetComponent<Renderer>();
                //renderer.material.mainTexture = Tile.originalTiles[(int)hitTile];
                renderer.material.mainTexture = Tile.expandedTiles[(int)hitTile];
                renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

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

    public void AddActiveCharacter(U4_Decompiled_AVATAR.activeCharacter currentActiveCharacter, int offsetx = 0, int offsety = 0)
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
            renderer.material.SetColor("_FrontColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);
            renderer.material.SetColor("_BackColor", Color.yellow);

            // rotate active character box into place
            characters.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }

        if (currentActiveCharacter.active)
        {
            Vector3 location = new Vector3(currentActiveCharacter.x + offsetx, 0.01f, Outdoor.outdoorMap.GetLength(1) - 1 - currentActiveCharacter.y + offsety);
            activeCharacter.transform.localPosition = location;
            activeCharacter.SetActive(true);
        }
        else
        {
            activeCharacter.SetActive(false);
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
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

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

                // set this as a parent of the monsters game object
                monsterGameObject.transform.SetParent(monstersGameObject.transform);
            }
        }
    }

    public void AddDungeonMapMonsters(U4_Decompiled_AVATAR u4)
    {
        // have we finished creating the world
        if (dungeonMonsters == null)
        {
            return;
        }

        // need to create npc game objects if none are present
        if (dungeonMonsters.transform.childCount != Dungeon.MAX_DUNGEON_MONSTERS)
        {
            for (int i = 0; i < Dungeon.MAX_DUNGEON_MONSTERS; i++)
            {
                // a child object for each dungeon monster entry in the table
                GameObject dungeonMonsterGameObject = Primitive.CreateQuad();

                // get the renderer
                MeshRenderer renderer = dungeonMonsterGameObject.GetComponent<MeshRenderer>();

                // intially the texture is null
                renderer.material.mainTexture = null;

                // set the shader
                renderer.material.shader = Shader.Find("Unlit/Transparent Cutout 2");

                // add our little animator script and set the tile to zero
                Animate3 animate = dungeonMonsterGameObject.AddComponent<Animate3>();
                animate.npcTile = 0;
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
                Dungeon.DUNGEON_TILE dungeonTile = (Dungeon.DUNGEON_TILE)u4.tMap8x8x8[u4.Party._z][x, 7 - y];

                // check upper nibble to see if there is anything to render
                int checkDungeonTile = (int)dungeonTile & 0xf0;

                if (checkDungeonTile == (int)Dungeon.DUNGEON_TILE.TRAP ||
                    checkDungeonTile == (int)Dungeon.DUNGEON_TILE.FOUNTAIN ||
                    checkDungeonTile == (int)Dungeon.DUNGEON_TILE.FIELD ||
                    checkDungeonTile == (int)Dungeon.DUNGEON_TILE.ALTAR || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile == (int)Dungeon.DUNGEON_TILE.MAGIC_ORB || // extra because I don't render these as dungeon monster sprites like the original game
                    checkDungeonTile >= (int)Dungeon.DUNGEON_TILE.DUNGEON_ROOM)
                {
                    continue;
                }

                // dungeon monster is stored in the low nibble of the dungeon tile
                checkDungeonTile = (int)dungeonTile & 0x0f;

                // zero means no monster at that dungeon location
                if (checkDungeonTile == 0)
                {
                    continue;
                }

                // convert the monster nibble into a map tile
                Tile.TILE monsterTile = (Tile.TILE)((checkDungeonTile << 2) - 4 + Tile.TILE.RAT);

                // did we create enough monster child game objects
                if (monsterIndex < dungeonMonsters.transform.childCount)
                {
                    // get the corresponding monster game object
                    Transform childofmonsters = dungeonMonsters.transform.GetChild(monsterIndex++);

                    // set it active
                    childofmonsters.gameObject.SetActive(true);

                    // update the tile of the game object
                    childofmonsters.GetComponent<Animate3>().SetNPCTile(monsterTile);

                    // wrap the position
                    int xx = x;
                    int yy = y;

                    if (yy - u4.Party._y > 3)
                    {
                        yy -= 8;
                    } 
                    else if (yy - u4.Party._y < -4)
                    {
                        yy += 8;
                    }

                    if (xx - u4.Party._x > 3)
                    {
                        xx -= 8;
                    }
                    else if (xx - u4.Party._x < -4)
                    {
                        xx += 8;
                    }

                    // update the position
                    childofmonsters.localPosition = new Vector3(xx * 11 + 5, (7 - yy) * 11 + 5, 0);
                    childofmonsters.localEulerAngles = new Vector3(-90.0f, 180.0f, 180.0f);

                    // simple lighting
                    Renderer renderer = childofmonsters.GetComponent<Renderer>();
                    renderer.material.color = Color.white;
                    float value = ((5f * 11f) - Vector3.Distance(childofmonsters.transform.position, new Vector3(u4.Party._x * 11, 0, (7 - u4.Party._y) * 11))) / (5f * 11f);
                    renderer.material.color = new Color(value, value, value, 1);

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

    // Start is called before the first frame update
    void Awake()
    {
 
    }

    private void SetupTiles()
    {
        // fix a tile
        Tile.FixMageTile3();

        // expand the tiles
        Tile.ExpandTiles();

        // create texture atlas
        Tile.CreateLinearTextureAtlas(ref Tile.originalTiles);
        Tile.CreateSquareTextureAtlas(ref Tile.originalTiles);
        Tile.CreateExpandedTextureAtlas(ref Tile.expandedTiles);

    }

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
        Palette.InitializeEGAPalette();
        Palette.InitializeCGAPalette();
        Palette.InitializeApple2Palette();

        Tile.LoadTiles(Tile.TILE_TYPE.EGA);
        currentTileType = Tile.TILE_TYPE.EGA;
        SetupTiles();

        // get the font
        GameFont.LoadCharSetEGA();
        //GameFont.LoadCharSetCGA();
        GameFont.ImportFontFromTexture(myFont, myTransparentFont, GameFont.fontAtlas, GameFont.fontTransparentAtlas);

        // set all the text objects to myFont in the input panel
        Text[] text = InputPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set all the text objects to myFont in the stats panel
        text = StatsPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set all the text objects to myFont in the game text panel
        text = TextPanel.GetComponentsInChildren<Text>(true);
        foreach (Text t in text)
        {
            t.font = myFont;
        }

        // set again all the button text objects in the input panel to myTransparentFont
        Button[] buttons = InputPanel.GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            Text[] texts = b.GetComponentsInChildren<Text>(true);
            foreach (Text t in texts)
            {
                t.font = myTransparentFont;
            }
        }

        // load the entire world map
        Outdoor.LoadOutdoorMap();

        //load all settlements
        Settlement.LoadSettlements();

        // load all dungeons
        Dungeon.LoadDungeons();

        // create the part game object
        CreateParty();

        // Create the combat terrains
        Combat.CreateCombatTerrains(Outdoor.outdoorMap.GetLength(1));

        // get a reference to the game engine
        u4 = FindObjectOfType<U4_Decompiled_AVATAR>();

        // initialize hidden map
        hiddenWorldMapGameObject = new GameObject("Hidden World Map");
        Map.CreateMapSubsetPass2(hiddenWorldMapGameObject, ref Outdoor.outdoorMap, ref entireMapGameObjects);

        GameObject hiddenSettlementsMaps = new GameObject("Hidden Settlements Maps");
        for (int i = 0; i < (int)Settlement.SETTLEMENT.MAX; i++)
        {
            GameObject settlementGameObject = new GameObject(((Settlement.SETTLEMENT)i).ToString());
            settlementGameObject.transform.SetParent(hiddenSettlementsMaps.transform);
            Settlement.settlementsMapGameObjects[i] = new GameObject[32, 32];
            Map.CreateMapSubsetPass2(settlementGameObject, ref Settlement.settlementMap[i], ref Settlement.settlementsMapGameObjects[i], true);
            //CreateMapLabels(settlementGameObject, ref settlementMap[i]);
        }

        // set the vision to blank
        vision.sprite = null;
        vision.color = new Color(0f, 0f, 0f, 0f);

        // allocate vision texture that we can overlap pictures onto
        visionTexture = new Texture2D(320, 200);
        visionTexture.filterMode = FilterMode.Point;
        Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        // everything I need it now loaded, start the game engine thread
        u4.StartThread();
    }

    void UpdateInputPanelState()
    {
        if ((u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.CITIZEN_WORD) && (u4.npcTalkIndex <= U4_Decompiled_AVATAR.TALK_INDEX.CITIZEN_31))
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkCitizen.SetActive(true);

            bool keyword1found = false;
            bool keyword2found = false;

            Settlement.SETTLEMENT settlement;
            if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
            {
                settlement = Settlement.SETTLEMENT.LCB_1;
            }
            else
            {
                settlement = (Settlement.SETTLEMENT)u4.Party._loc;
            }

            foreach (string word in u4.wordList)
            {
                // only add the special keywords if we already know them
                // TODO don't need to do this so often, only when we get new text
                // TODO need to clear npcTalkIndex when switching levels or settlements as the index might not be valid for the other location
                // Some keywords are just 1 or 2 characters so I need to remove this check
                //if (word.Length >= 2)
                {
                    // the talk file may have trailing spaces in the keyword if it is less than 4 characters, we need to remove them for the compare
                    // convert to lower
                    string lower = word.ToLower();
                    // trim any whitespace off the end
                    string trimmed = lower.TrimEnd();
                    //clip the string to a max of 4 characters
                    string sub;
                    if (trimmed.Length > 4)
                    { 
                        sub = trimmed.Substring(0, 4); 
                    }
                    else
                    {
                        sub = trimmed;
                    }

                    // convert to lower
                    string lowerKeyword1 = Settlement.settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)Settlement.NPC_STRING_INDEX.KEYWORD1].ToLower();
                    // trim any whitespace off the end
                    string trimmedKeyword1 = lowerKeyword1.TrimEnd();
                    //clip the string to a max of 4 characters
                    string subKeyword1;
                    if (trimmedKeyword1.Length > 4)
                    {
                        subKeyword1 = trimmedKeyword1.Substring(0, 4);
                    }
                    else
                    {
                        subKeyword1 = trimmedKeyword1;
                    }

                    //Debug.Log(sub);
                    //Debug.Log(settlement);
                    //Debug.Log("u4.npcTalkIndex " + u4.npcTalkIndex);

                    if ((sub == subKeyword1) && (sub != "a"))
                    {
                        u4.keyword1 = lower;
                        lower = char.ToUpper(lower[0]) + lower.Substring(1, lower.Length - 1);
                        keyword1ButtonText.text = lower;
                        keyword1found = true;
                        keyword1Button.SetActive(true);
                    }

                    // convert to lower
                    string lowerKeyword2 = Settlement.settlementNPCs[(int)settlement][(int)u4.npcTalkIndex].strings[(int)Settlement.NPC_STRING_INDEX.KEYWORD2].ToLower();
                    // trim any whitespace off the end
                    string trimmedKeyword2 = lowerKeyword2.TrimEnd();
                    //clip the string to a max of 4 characters
                    string subKeyword2;
                    if (trimmedKeyword2.Length > 4)
                    {
                        subKeyword2 = trimmedKeyword2.Substring(0, 4);
                    }
                    else
                    {
                        subKeyword2 = trimmedKeyword2;
                    }

                    if ((sub == subKeyword2) && (sub != "a"))
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

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_YES_NO)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkYN.SetActive(true);
        }
        else
        {
            TalkYN.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_YES_NO_WORD)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkYesNo.SetActive(true);
        }
        else
        {
            TalkYesNo.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_HEALER)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkHealer.SetActive(true);
        }
        else
        {
            TalkHealer.SetActive(false);
        }

        if ((u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_CONTINUE) ||
            (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_CONTINUE) ||
            (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_NO_CONTINUE))
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkContinue.SetActive(true);
            if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_CONTINUE)
            {
                TalkContinueButton.gameObject.SetActive(true);
            }
            else if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DELAY_NO_CONTINUE)
            {
                TalkContinueButton.gameObject.SetActive(false);

                if (vision.sprite == null)
                {
                    // no continue button and nothing to display so just disable the panel entirely
                    InputPanel.SetActive(false);
                }
            }
            else
            {
                TalkContinueButton.gameObject.SetActive(true);
            }
        }
        else
        {
            TalkContinue.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.MAIN_LOOP)
        {
            InputPanel.SetActive(true);
            ActionLabel.SetActive(true);
            TalkLabel.SetActive(false);
            ActionMainLoopInputPanel.SetActive(true);
        }
        else
        {
            ActionMainLoopInputPanel.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.DUNGEON_LOOP)
        {
            InputPanel.SetActive(true);
            ActionLabel.SetActive(true);
            TalkLabel.SetActive(false);
            ActionDungeonLoopInputPanel.SetActive(true);
        }
        else
        {
            ActionDungeonLoopInputPanel.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.HAWKWIND_WORD)
        {
            InputPanel.SetActive(true);
            // TODO: need to filter buttons like citizen talk with word list
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkHawWind.SetActive(true);
        }
        else
        {
            TalkHawWind.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_FOOD_OR_ALE)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkFoodAle.SetActive(true);
        }
        else
        {
            TalkFoodAle.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_BUY_SELL)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkBuySell.SetActive(true);
        }
        else
        {
            TalkBuySell.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_ASK_CHARACTER_NUMBER)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(false);
            ActionLabel.SetActive(false);
            TalkPartyCharacter.SetActive(true);
        }
        else
        {
            TalkPartyCharacter.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.PUB_WORD)
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
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkPubWord.SetActive(true);
        }
        else
        {
            TalkPubWord.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.MANTRA_WORD)
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

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkMantras.SetActive(true);
        }
        else
        {
            TalkMantras.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.LOAD_BRITISH_WORD)
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

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkLordBritish.SetActive(true);
        }
        else
        {
            TalkLordBritish.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.VIRTUE_WORD)
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

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkVirtue.SetActive(true);
        }
        else
        {
            TalkVirtue.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.END_GAME_WORD)
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
            Love
            Truth
            Courage
            */

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkEndGame.SetActive(true);
        }
        else
        {
            TalkEndGame.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.USE_ITEM_WORD)
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

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkUseItem.SetActive(true);
        }
        else
        {
            TalkUseItem.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.USE_STONE_COLOR_WORD)
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

            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkColors.SetActive(true);
        }
        else
        {
            TalkColors.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_2_DIGITS)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            Talk2DigitInput.SetActive(true);
        }
        else
        {
            Talk2DigitInput.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_3_DIGITS)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            Talk3DigitInput.SetActive(true);
        }
        else
        {
            Talk3DigitInput.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.COMBAT_LOOP)
        {
            InputPanel.SetActive(true);
            ActionLabel.SetActive(true);
            TalkLabel.SetActive(false);
            ActionCombatLoopInputPanel.SetActive(true);
        }
        else
        {
            ActionCombatLoopInputPanel.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_WEAPON)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkWeapon.SetActive(true);
        }
        else
        {
            TalkWeapon.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_ARMOR)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkArmor.SetActive(true);
        }
        else
        {
            TalkArmor.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_GUILD)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkGuild.SetActive(true);
        }
        else
        {
            TalkGuild.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_REAGENT)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkReagents.SetActive(true);
        }
        else
        {
            TalkReagents.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_SPELL)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkSpells.SetActive(true);
        }
        else
        {
            TalkSpells.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_1_DIGITS)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            Talk1DigitInput.SetActive(true);
        }
        else
        {
            Talk1DigitInput.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ENERGY_TYPE_POISON_FIRE_LIGHTNING_SLEEP)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkEnergy.SetActive(true);
        }
        else
        {
            TalkEnergy.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_TELESCOPE)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkTelescope.SetActive(true);
        }
        else
        {
            TalkTelescope.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.ASK_LETTER_PHASE)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkPhase.SetActive(true);
        }
        else
        {
            TalkPhase.SetActive(false);
        }

        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_NUMBER_INPUT_0_1_2_3)
        {
            InputPanel.SetActive(true);
            TalkLabel.SetActive(true);
            ActionLabel.SetActive(false);
            TalkDigit0123.SetActive(true);
        }
        else
        {
            TalkDigit0123.SetActive(false);
        }

        // disable this for now
        // TODO if this is going to be an actual input panel it needs to support
        // rotation and direction like the controller do
        if (u4.inputMode == U4_Decompiled_AVATAR.INPUT_MODE.GENERAL_DIRECTION)
        {
            InputPanel.SetActive(false);
            TalkLabel.SetActive(false);
            ActionLabel.SetActive(false);
            TalkDirection.SetActive(false);
        }
        else
        {
            TalkDirection.SetActive(false);
        }
    }

    void UpdateOutdoors()
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
        skyGameObject.SetActive(true);

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            Combat.CombatTerrains[i].gameObject.SetActive(false);
        }

        // automatically enter things when you are on an enterable tile unless just left somewhere or you are flying in the balloon or just attempted entering and failed
        if ((readyToAutomaticallyEnter == true) && (u4.Party.f_1dc == 0) && (u4.lastKeyboardHit != 'E') &&
            ((u4.current_tile == Tile.TILE.CASTLE_ENTRANCE) ||
            (u4.current_tile == Tile.TILE.CASTLE) ||
            (u4.current_tile == Tile.TILE.TOWN) ||
            (u4.current_tile == Tile.TILE.VILLAGE) ||
            (u4.current_tile == Tile.TILE.DUNGEON) ||
            (u4.current_tile == Tile.TILE.RUINS) ||
            (u4.current_tile == Tile.TILE.SHRINE)))
        {
            u4.CommandEnter();
            readyToAutomaticallyEnter = false; 
        }

        // wait until we move off of an entrance tile after leaving somewhere
        if ((readyToAutomaticallyEnter == false) &&
            (u4.current_tile != Tile.TILE.CASTLE_ENTRANCE) &&
            (u4.current_tile != Tile.TILE.CASTLE) &&
            (u4.current_tile != Tile.TILE.TOWN) &&
            (u4.current_tile != Tile.TILE.VILLAGE) &&
            (u4.current_tile != Tile.TILE.DUNGEON) &&
            (u4.current_tile != Tile.TILE.RUINS) &&
            (u4.current_tile != Tile.TILE.SHRINE))
        {
            readyToAutomaticallyEnter = true;
        }

        // automatically board horse, ship and balloon
        if (((u4.current_tile == Tile.TILE.HORSE_EAST) ||
            (u4.current_tile == Tile.TILE.HORSE_WEST) ||
            (u4.current_tile == Tile.TILE.SHIP_EAST) ||
            (u4.current_tile == Tile.TILE.SHIP_NORTH) ||
            (u4.current_tile == Tile.TILE.SHIP_WEST) ||
            (u4.current_tile == Tile.TILE.SHIP_SOUTH) ||
            (u4.current_tile == Tile.TILE.BALOON)) &&
            (lastCurrentTile != Tile.TILE.HORSE_EAST) &&
            (lastCurrentTile != Tile.TILE.HORSE_WEST) &&
            (lastCurrentTile != Tile.TILE.SHIP_EAST) &&
            (lastCurrentTile != Tile.TILE.SHIP_NORTH) &&
            (lastCurrentTile != Tile.TILE.SHIP_WEST) &&
            (lastCurrentTile != Tile.TILE.SHIP_SOUTH) &&
            (lastCurrentTile != Tile.TILE.BALOON) &&
            (u4.lastKeyboardHit != 'X') && (u4.lastKeyboardHit != 'B'))
        {
            u4.CommandBoard();
        }

        // update last tile so we don't get stuck in a loop
        lastCurrentTile = u4.current_tile;

        if (Camera.main.clearFlags != CameraClearFlags.Skybox)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
    }

    void UpdateSettlement()
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
        skyGameObject.SetActive(true);

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            Combat.CombatTerrains[i].gameObject.SetActive(false);
        }

        // automatic Klimb and Descend ladders
        if ((u4.current_tile == Tile.TILE.LADDER_UP) &&
            (lastCurrentTile != Tile.TILE.LADDER_UP) &&
            (lastCurrentTile != Tile.TILE.LADDER_DOWN))
        {
            u4.CommandKlimb();
        }

        // automatic Klimb and Descend ladders
        if ((u4.current_tile == Tile.TILE.LADDER_DOWN) &&
            (lastCurrentTile != Tile.TILE.LADDER_UP) &&
            (lastCurrentTile != Tile.TILE.LADDER_DOWN))
        {
            u4.CommandDecsend();
        }

        // automatically board horse, ship and balloon
        if (((u4.current_tile == Tile.TILE.HORSE_EAST) ||
            (u4.current_tile == Tile.TILE.HORSE_WEST)) &&
            (lastCurrentTile != Tile.TILE.HORSE_EAST) &&
            (lastCurrentTile != Tile.TILE.HORSE_WEST) &&
            (u4.lastKeyboardHit != 'X') && (u4.lastKeyboardHit != 'B'))
        {
            u4.CommandBoard();
        }

        // update last tile so we don't get stuck in a loop
        lastCurrentTile = u4.current_tile;

        if (Camera.main.clearFlags != CameraClearFlags.Skybox)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }

        // TODO auto open doors here
    }

    void UpdateCombat()
    {
        if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
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
            skyGameObject.SetActive(false);

            // check if we have the dungeon already created, create it if not
            Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
            if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
            {
                Destroy(dungeon);
                dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, Combat.combatMaps);
            }
            dungeon.SetActive(true);

            for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
            {
                Combat.CombatTerrains[i].gameObject.SetActive(false);
            }

            if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = Color.black;
            }

            // need to check and see if the map dungeon block we created for the level
            // matches what the game engine is using, we adjust halways and other dungeon
            // block features so the dungeon fit together nicely, if it does not match we nned
            // to to dynamically alter the game engine dungeon block map to match so monsters/fighters
            // do not try to walk through walls
            bool map_updated = false;
            Dungeon.DungeonBlockLevel level = Dungeon.currentDungeonBlockLevel[u4.Party._x, 7 - u4.Party._y];
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    // is the map different because we modified it?
                    if (u4.Combat_map[x, y] != level.dungeonBlockMap[y, x])
                    {
                        // update the map
                        u4.Combat_map[x, y] = level.dungeonBlockMap[y, x];

                        // flag that we made an update
                        map_updated = true;
                    }
                }
            }

            // check if dungeon block map needed to be updated
            if (map_updated)
            {
                // need to see if all the monsters/fighters are inside playable
                // space in the dungeon block and move or kill them if they are not
                // check the fighters
                for (int i = 0; i < 16; i++)
                {
                    // is the fighter real
                    if (u4.Fighters[i]._tile != Tile.TILE.DEEP_WATER)
                    {
                        // get the tile under the fighter
                        Tile.TILE fighterTile = u4.Combat_map[u4.Combat1[i]._npcY, u4.Combat1[i]._npcX];

                        // is it blank or a brick wall
                        if ((fighterTile == Tile.TILE.BRICK_WALL) || (fighterTile == Tile.TILE.BLANK))
                        {
                            // delete/erase/kill this fighter/monster because it is outside of playable space
                            //u4.Fighters[i]._tile = Tile.TILE.DEEP_WATER;
                            //u4.Fighters[i]._gtile = Tile.TILE.DEEP_WATER;

                            // Move this fighter/monster because it is outside of playable space
                            u4.Combat1[i]._npcX = 5;
                            u4.Combat1[i]._npcY = 5;

                            // TODO: find a better spot to put them so they aren't overlapping, random?
                            // Note: some dungeon rooms will put all the players in one spot so the game engine is designed to handle this case
                        }
                    }
                }

                // need to make sure all 8 players characters will also fit and move them if needed
                for (int i = 0; i < u4.Party.f_1d8; i++)
                {
                    // is the character real
                    if (u4.Fighters[i]._chtile != Tile.TILE.DEEP_WATER)
                    {
                        // get the tile under the character
                        Tile.TILE fighterTile = u4.Combat_map[u4.Combat2[i]._charaY, u4.Combat2[i]._charaX];

                        // is it blank or a brick wall
                        if ((fighterTile == Tile.TILE.BRICK_WALL) || (fighterTile == Tile.TILE.BLANK))
                        {
                            // Move this character because it is outside of playable space
                            u4.Combat2[i]._charaX = 5;
                            u4.Combat2[i]._charaY = 5;

                            // TODO: find a better spot to put them so they aren't overlapping, random?
                            // Note: some dungeon rooms will put all the players in one spot so the game engine is designed to handle this case
                        }
                    }
                }

                // update the game engine map with our modifications
                u4.SetCombat();

                // reset the flag
                map_updated = false;
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
            skyGameObject.SetActive(true);

            Combat.COMBAT_TERRAIN currentCombatTerrain = Combat.Convert_Tile_to_Combat_Terrian(u4.current_tile, u4.Party._tile, u4.D_96F8, u4.D_946C);

            for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
            {
                if (i == (int)currentCombatTerrain)
                {
                    Combat.CombatTerrains[i].SetActive(true);
                    Combat.UpdateBillboardCombatTerrains(Camera.main.transform.gameObject, Outdoor.outdoorMap.GetLength(1), currentCombatTerrain);
                }
                else
                {
                    Combat.CombatTerrains[i].SetActive(false);
                }
            }

            if (Camera.main.clearFlags != CameraClearFlags.Skybox)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
        }
    }

    void UpdateDungeonRoom()
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
        skyGameObject.SetActive(false);

        // check if we have the dungeon already created, create it if not
        Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
        if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
        {
            Destroy(dungeon);
            dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, Combat.combatMaps);
        }
        dungeon.SetActive(true);

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            Combat.CombatTerrains[i].gameObject.SetActive(false);
        }

        if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
        }

        foreach (Transform child in dungeon.transform)
        {
            Map.UpdateExistingBillboardsMap(child.gameObject);
            child.Find("Monsters").gameObject.SetActive(false);
        }

        // the game engine will dynamically alter dungeon rooms during combat using tigger tiles or dispell magic
        // we need to update the unity engine game dungeon tiles maps to match as these tiles are changed
        Dungeon.DungeonBlockLevel level = Dungeon.currentDungeonBlockLevel[u4.Party._x, 7 - u4.Party._y];
        bool map_updated = false;
        for (int y = 0; y < 11; y++)
        {
            for (int x = 0; x < 11; x++)
            {
                // is the map different because the game engine modified it?
                if (level.dungeonBlockMap[y, x] != u4.Combat_map[x, y])
                {
                    // update the unity based map
                    level.dungeonBlockMap[y, x] = u4.Combat_map[x, y];

                    // flag that we made an update
                    map_updated = true;
                }
            }
        }

        // check if the map needed updating
        if (map_updated)
        {
            // re-create the game object using the new map
            Map.CreateMap(level.dungeonGameObject, level.dungeonBlockMap, Vector3.zero, Vector3.zero);

            // put the new map in place
            level.dungeonGameObject.transform.localPosition = new Vector3(u4.Party._x * 11, (7 - u4.Party._y) * 11, 0);
            level.dungeonGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

            // update adjacent hallways as an opening could now exist
            Dungeon.DUNGEON_TILE dungeonTile;
            dungeonTile = (Dungeon.DUNGEON_TILE)((int)(u4.tMap8x8x8[u4.Party._z][(u4.Party._x + 1) % 8, 7 - u4.Party._y]) & 0xf0);
            if (dungeonTile == (Dungeon.DUNGEON_TILE.HALLWAY))
            {
                UpdateDungeonHallway(dungeonTile, (u4.Party._x + 1) % 8, u4.Party._y, dun);
            }
            dungeonTile = (Dungeon.DUNGEON_TILE)((int)(u4.tMap8x8x8[u4.Party._z][(u4.Party._x + 7) % 8, 7 - u4.Party._y]) & 0xf0);
            if (dungeonTile == (Dungeon.DUNGEON_TILE.HALLWAY))
            {
                UpdateDungeonHallway(dungeonTile, (u4.Party._x + 7) % 8, u4.Party._y, dun);
            }
            dungeonTile = (Dungeon.DUNGEON_TILE)((int)(u4.tMap8x8x8[u4.Party._z][u4.Party._x, 7 - (u4.Party._y + 1) % 8]) & 0xf0);
            if (dungeonTile == (Dungeon.DUNGEON_TILE.HALLWAY))
            {
                UpdateDungeonHallway(dungeonTile, u4.Party._x, (u4.Party._y + 1) % 8, dun);
            }
            dungeonTile = (Dungeon.DUNGEON_TILE)((int)(u4.tMap8x8x8[u4.Party._z][u4.Party._x, 7 - (u4.Party._y + 7) % 8]) & 0xf0);
            if (dungeonTile == (Dungeon.DUNGEON_TILE.HALLWAY))
            {
                UpdateDungeonHallway(dungeonTile, u4.Party._x, (u4.Party._y + 7) % 8, dun);
            }
        }

        // the transition to the dungeon room happens before we know the player has moved,
        // if this move to the dungeon room happens on a wrap point the room will be over blank space
        // need to update the dungeon wrap here so this doesn't happen
        WrapDungeon();
    }

    void WrapDungeon()
    {
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 8; i++)
            {
                int offset_x = u4.Party._x - Dungeon.currentDungeonBlockLevel.GetLength(0) / 2;
                int offset_y = Dungeon.currentDungeonBlockLevel.GetLength(1) - (u4.Party._y - Dungeon.currentDungeonBlockLevel.GetLength(1) / 2);

                int x = (i + offset_x + Dungeon.currentDungeonBlockLevel.GetLength(0)) % Dungeon.currentDungeonBlockLevel.GetLength(0);
                int y = (j + offset_y + Dungeon.currentDungeonBlockLevel.GetLength(1)) % Dungeon.currentDungeonBlockLevel.GetLength(1);

                Dungeon.DungeonBlockLevel dungeonBlock = Dungeon.currentDungeonBlockLevel[x, y];

                // check if the dungeon block exist for this coord, (walls do not get generated into dungeon blocks)
                if (dungeonBlock.dungeonGameObject)
                {
                    // adjust the position for wrapping
                    dungeonBlock.dungeonGameObject.transform.localPosition = new Vector3((i + offset_x) * 11, (j + offset_y - Dungeon.currentDungeonBlockLevel.GetLength(1)) * 11, 0);
                    
                    // get a value for distance
                    float value = ((5f * 11f) - Vector3.Distance(dungeonBlock.dungeonGameObject.transform.position, new Vector3(u4.Party._x * 11, 0, (7 - u4.Party._y) * 11))) / (5f * 11f);

                    // apply this as a color tint to all children of the dungeon block
                    // TODO: we could also apply this lighting color tint based on a raytrace or some other algorithm
                    foreach (Transform child in dungeonBlock.dungeonGameObject.transform)
                    {
                        Renderer renderer = child.GetComponent<Renderer>();
                        if (renderer)
                        {
                            renderer.material.color = new Color(value, value, value, 1);
                        }

                        foreach (Transform childofchild in child)
                        {
                            renderer = childofchild.GetComponent<Renderer>();
                            if (renderer)
                            {
                                renderer.material.color = new Color(value, value, value, 1);
                            }
                        }
                    }
                }
            }
        }
    }

    void UpdateDungeon()
    {
        AddNPCs(u4._npc);
        AddHits(u4.currentHits);
        AddActiveCharacter(u4.currentActiveCharacter);
        AddDungeonMapMonsters(u4);
        followWorld(partyGameObject);
        terrain.SetActive(false);
        animatedTerrrain.SetActive(false);
        billboardTerrrain.SetActive(false);
        fighters.SetActive(false);
        characters.SetActive(false);
        npcs.SetActive(false);
        party.SetActive(true);
        moongate.SetActive(false);
        skyGameObject.SetActive(false);

        // check if we have the dungeon already created, create it if not
        Dungeon.DUNGEONS dun = (Dungeon.DUNGEONS)((int)u4.Party._loc - (int)U4_Decompiled_AVATAR.LOCATIONS.DUNGEONS);
        if (dungeon.name != dun.ToString() + " Level #" + u4.Party._z)
        {
            // not the right dungeon, create a new dungeon
            Destroy(dungeon);
            dungeon = Dungeon.CreateDungeonExpandedLevel(dun, u4.Party._z, Combat.combatMaps);
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

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            Combat.CombatTerrains[i].gameObject.SetActive(false);
        }

        if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
        }

        foreach (Transform child in dungeon.transform)
        {
            Map.UpdateExistingBillboardsMap(child.gameObject);
            Dungeon.UpdateExistingBillboardsDungeonRoomMonster(child.gameObject);
            child.Find("Monsters").gameObject.SetActive(true);
        }

        // The dungeon can be changed due to monster dropping chests or dispell magic
        // need to update dungeon map hallway blocks as needed
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Dungeon.DUNGEON_TILE dungeonTile = (Dungeon.DUNGEON_TILE)((int)(u4.tMap8x8x8[u4.Party._z][x, 7-y]) & 0xf0);
                if (dungeonTile != (Dungeon.DUNGEON_TILE)((int)(Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonTile) & 0xf0))
                {
                    UpdateDungeonHallway(dungeonTile, x, y, dun);
                }
            }
        }

        // wrap dungeon so we are always mostly in the center of the 8x8 map
        WrapDungeon();
    }

    void UpdateDungeonHallway(Dungeon.DUNGEON_TILE dungeonTile, int x, int y, Dungeon.DUNGEONS dun)
    {
        Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonTile = dungeonTile;

        Tile.TILE tileIndex;

        if (dungeonTile == Dungeon.DUNGEON_TILE.WALL)
        {
            tileIndex = Tile.TILE.BRICK_WALL;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.HALLWAY)
        {
            tileIndex = Tile.TILE.TILED_FLOOR;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.LADDER_UP)
        {
            tileIndex = Tile.TILE.LADDER_UP;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.LADDER_DOWN)
        {
            tileIndex = Tile.TILE.LADDER_DOWN;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.LADDER_UP_AND_DOWN)
        {
            tileIndex = Tile.TILE.LADDER_DOWN;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.TREASURE_CHEST)
        {
            tileIndex = Tile.TILE.CHEST;
        }
        else if ((dungeonTile == Dungeon.DUNGEON_TILE.FOUNTAIN) ||
                (dungeonTile == Dungeon.DUNGEON_TILE.FOUNTAIN_CURE) ||
                (dungeonTile == Dungeon.DUNGEON_TILE.FOUNTAIN_HEALING) ||
                (dungeonTile == Dungeon.DUNGEON_TILE.FOUNTAIN_POISIN) ||
                (dungeonTile == Dungeon.DUNGEON_TILE.FOUNTAIN_ACID))
        {
            tileIndex = Tile.TILE.SHALLOW_WATER;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.FIELD_ENERGY)
        {
            tileIndex = Tile.TILE.ENERGY_FIELD;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.FIELD_FIRE)
        {
            tileIndex = Tile.TILE.FIRE_FIELD;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.FIELD_POISON)
        {
            tileIndex = Tile.TILE.POISON_FIELD;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.FIELD_SLEEP)
        {
            tileIndex = Tile.TILE.SLEEP_FIELD;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.DOOR)
        {
            tileIndex = Tile.TILE.DOOR;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.DOOR_SECRECT)
        {
            tileIndex = Tile.TILE.SECRET_BRICK_WALL;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.ALTAR)
        {
            tileIndex = Tile.TILE.ALTAR;
        }
        else if (dungeonTile == Dungeon.DUNGEON_TILE.MAGIC_ORB)
        {
            tileIndex = Tile.TILE.MISSLE_ATTACK_BLUE;
        }
        else if ((dungeonTile == Dungeon.DUNGEON_TILE.TRAP_FALLING_ROCKS) ||
            (dungeonTile == Dungeon.DUNGEON_TILE.TRAP_WIND_DARKNESS) ||
            (dungeonTile == Dungeon.DUNGEON_TILE.TRAP_PIT))
        {
            tileIndex = Tile.TILE.TILED_FLOOR;
        }
        else
        {
            tileIndex = Tile.TILE.TILED_FLOOR;
        }

        Tile.TILE[,] map = Dungeon.CreateDungeonHallway(
            ref u4.tMap8x8x8[u4.Party._z],
            ref Dungeon.dungeons[(int)dun].dungeonRooms,
            x, 7 - y, u4.Party._z,
            tileIndex);
        foreach (Transform child in Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonGameObject.transform)
        {
            Object.DestroyImmediate(child.gameObject);
        }
        Map.CreateMap(Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonGameObject, map, new Vector3(x * 11, y * 11, 0), new Vector3(90.0f, 0.0f, 0.0f));

        // put the new map in place
        Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonGameObject.transform.localPosition = new Vector3(x * 11, (7 - y) * 11, 0);
        Dungeon.currentDungeonBlockLevel[x, 7 - y].dungeonGameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    void UpdateShrine()
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
        skyGameObject.SetActive(true);

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            if (i == (int)Combat.COMBAT_TERRAIN.SHRINE)
            {
                Combat.CombatTerrains[i].gameObject.SetActive(true);
                followWorld(Combat.CenterOfCombatTerrain);
            }
            else
            {
                Combat.CombatTerrains[i].gameObject.SetActive(false);
            }
        }

        if (Camera.main.clearFlags != CameraClearFlags.Skybox)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
    }

    void UpdateVision()
    {
        // since you are looking at a gem or through a telescope you should not be able to see anything else so temp turn everything off
        terrain.SetActive(false);
        animatedTerrrain.SetActive(false);
        billboardTerrrain.SetActive(false);
        fighters.SetActive(false);
        characters.SetActive(false);
        npcs.SetActive(false);
        party.SetActive(false);
        moongate.SetActive(false);
        skyGameObject.SetActive(false); 
        dungeon.SetActive(false);
        dungeonMonsters.SetActive(false);

        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
    }

    void UpdateCombatCamp()
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
        followWorld(activeCharacter);

        int currentCombatTerrain;
        // need to special case the combat when in the inn and in combat camp mode outside or in dungeon
        if (u4.current_tile == Tile.TILE.BRICK_FLOOR)
        {
            currentCombatTerrain = (int)Combat.COMBAT_TERRAIN.INN;
            skyGameObject.SetActive(true);
            if (Camera.main.clearFlags != CameraClearFlags.Skybox)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
        }
        else if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
        {
            currentCombatTerrain = (int)Combat.COMBAT_TERRAIN.CAMP_DNG;
            skyGameObject.SetActive(false);
            if (Camera.main.clearFlags != CameraClearFlags.SolidColor)
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = Color.black;
            }
        }
        else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
        {
            currentCombatTerrain = (int)Combat.COMBAT_TERRAIN.CAMP;
            skyGameObject.SetActive(true);
            if (Camera.main.clearFlags != CameraClearFlags.Skybox)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
        }
        else
        {
            currentCombatTerrain = (int)Combat.COMBAT_TERRAIN.CAMP;
            skyGameObject.SetActive(true);
            if (Camera.main.clearFlags != CameraClearFlags.Skybox)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
        }

        for (int i = 0; i < (int)Combat.COMBAT_TERRAIN.MAX; i++)
        {
            if (i == currentCombatTerrain)
            {
                Combat.CombatTerrains[i].gameObject.SetActive(true);
                if (u4.currentActiveCharacter.active)
                {
                    followWorld(activeCharacter);
                }
                else
                {
                    followWorld(Combat.CenterOfCombatTerrain);
                }
            }
            else
            {
                Combat.CombatTerrains[i].gameObject.SetActive(false);
            }
        }
    }

    public enum VISION_TILE
    {
        EMPTY = 0,
        GRASS = 1,
        FOREST = 2,
        LAVA = 3,
        CASTLE_BRIDGE = 4,
        DUNEGON_TOWN = 5,
        SHRINE = 6,
        MOUNTAIN = 7,
        HILL = 8,
        BUSH = 9,
        SHALLOW_WATER = 10,
        WATER = 11,
        DEEP_WATER = 12
    }

    public VISION_TILE[] ConvertMapToVision = 
    {
        VISION_TILE.DEEP_WATER,
        VISION_TILE.WATER,
        VISION_TILE.SHALLOW_WATER,
        VISION_TILE.GRASS,
        VISION_TILE.GRASS,
        VISION_TILE.BUSH,
        VISION_TILE.FOREST,
        VISION_TILE.HILL,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.SHRINE,
        VISION_TILE.SHRINE,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.EMPTY,
        VISION_TILE.LAVA,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.MOUNTAIN,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.LAVA,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.DUNEGON_TOWN,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.CASTLE_BRIDGE,
        VISION_TILE.SHRINE,
        VISION_TILE.EMPTY,
        VISION_TILE.MOUNTAIN
    };

    void PaintGreen(int x, int y, int xx, int yy)
    {
        visionTexture.SetPixel((visionTexture.width / 2) - (32 * 4 / 2) + y + 4 * xx, (visionTexture.height / 2) - (32 * 4 / 2) + 4 - x + 4 * yy, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.GREEN]);
    }

    void PaintBlue(int x, int y, int xx, int yy)
    {
        visionTexture.SetPixel((visionTexture.width / 2) - (32 * 4 / 2) + y + 4 * xx, (visionTexture.height / 2) - (32 * 4 / 2) + 4 - x + 4 * yy, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLUE]);
    }

    void PaintWhite(int x, int y, int xx, int yy)
    {
        visionTexture.SetPixel((visionTexture.width / 2) - (32 * 4 / 2) + y + 4 * xx, (visionTexture.height / 2) - (32 * 4 / 2) + 4 - x + 4 * yy, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.WHITE]);
    }

    void DisplayMapVision()
    {
        Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // the table only converts the first 128 tiles
                if ((int)u4.tMap32x32[x, 31 - y] < 0x80)
                {
                    switch (ConvertMapToVision[(int)u4.tMap32x32[x, 31 - y]])
                    {
                        case VISION_TILE.GRASS:
                        {
                            PaintGreen(0, 1, x, y);
                            PaintGreen(1, 3, x, y);
                            PaintGreen(2, 1, x, y);
                            PaintGreen(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.FOREST:
                        {
                            PaintGreen(0, 3, x, y);
                            PaintGreen(1, 1, x, y);
                            PaintGreen(2, 3, x, y);
                            PaintGreen(3, 1, x, y);
                            PaintGreen(0, 1, x, y);
                            PaintGreen(1, 3, x, y);
                            PaintGreen(2, 1, x, y);
                            PaintGreen(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.LAVA:
                        {
                            PaintBlue(0, 0, x, y);
                            PaintBlue(1, 0, x, y);
                            PaintBlue(2, 0, x, y);
                            PaintBlue(3, 0, x, y);
                            PaintBlue(0, 2, x, y);
                            PaintBlue(1, 2, x, y);
                            PaintBlue(2, 2, x, y);
                            PaintBlue(3, 2, x, y);
                            break;
                        }
                        case VISION_TILE.CASTLE_BRIDGE:
                        {
                            PaintWhite(0, 0, x, y);
                            PaintWhite(0, 1, x, y);
                            PaintWhite(0, 2, x, y);
                            PaintWhite(0, 3, x, y);
                            PaintWhite(3, 0, x, y);
                            PaintWhite(3, 1, x, y);
                            PaintWhite(3, 2, x, y);
                            PaintWhite(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.DUNEGON_TOWN:
                        {
                            PaintWhite(1, 1, x, y);
                            PaintWhite(2, 1, x, y);
                            PaintWhite(1, 2, x, y);
                            PaintWhite(2, 2, x, y);
                            break;
                        }
                        case VISION_TILE.SHRINE:
                        {
                            PaintWhite(1, 0, x, y);
                            PaintWhite(2, 0, x, y);
                            PaintWhite(1, 3, x, y);
                            PaintWhite(2, 3, x, y);
                            PaintWhite(0, 0, x, y);
                            PaintWhite(0, 1, x, y);
                            PaintWhite(0, 2, x, y);
                            PaintWhite(0, 3, x, y);
                            PaintWhite(3, 0, x, y);
                            PaintWhite(3, 1, x, y);
                            PaintWhite(3, 2, x, y);
                            PaintWhite(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.MOUNTAIN:
                        {
                            PaintWhite(1, 1, x, y);
                            PaintWhite(2, 1, x, y);
                            PaintWhite(1, 2, x, y);
                            PaintWhite(2, 2, x, y);
                            PaintWhite(1, 0, x, y);
                            PaintWhite(2, 0, x, y);
                            PaintWhite(1, 3, x, y);
                            PaintWhite(2, 3, x, y);
                            PaintWhite(0, 0, x, y);
                            PaintWhite(0, 1, x, y);
                            PaintWhite(0, 2, x, y);
                            PaintWhite(0, 3, x, y);
                            PaintWhite(3, 0, x, y);
                            PaintWhite(3, 1, x, y);
                            PaintWhite(3, 2, x, y);
                            PaintWhite(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.HILL:
                        {
                            PaintWhite(0, 0, x, y);
                            PaintWhite(1, 0, x, y);
                            PaintWhite(0, 1, x, y);
                            PaintWhite(1, 1, x, y);
                            PaintWhite(2, 2, x, y);
                            PaintWhite(3, 2, x, y);
                            PaintWhite(2, 3, x, y);
                            PaintWhite(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.BUSH:
                        {
                            PaintGreen(0, 1, x, y);
                            PaintGreen(1, 1, x, y);
                            PaintGreen(2, 1, x, y);
                            PaintGreen(0, 3, x, y);
                            PaintGreen(2, 3, x, y);
                            PaintGreen(3, 3, x, y);
                            break;
                        }
                        case VISION_TILE.SHALLOW_WATER:
                        {
                            PaintBlue(0, 0, x, y);
                            PaintBlue(2, 0, x, y);
                            PaintBlue(1, 2, x, y);
                            PaintBlue(3, 2, x, y);
                            break;
                        }
                        case VISION_TILE.WATER:
                        {
                            PaintBlue(0, 0, x, y);
                            PaintBlue(2, 2, x, y);
                            break;
                        }
                        case VISION_TILE.DEEP_WATER:
                        {
                            PaintWhite(2 * (x & 1), 2, x, y);
                            break;
                        }
                        default:
                        {
                            // do nothing
                            break;
                        }
                    }
                }
                else
                {
                    PaintWhite(1, 1, x, y);
                    PaintWhite(2, 1, x, y);
                    PaintWhite(1, 2, x, y);
                    PaintWhite(2, 2, x, y);
                }
            }
        }
        visionTexture.Apply();

        vision.sprite = Sprite.Create(visionTexture, new Rect(0.0f, 0.0f, visionTexture.width, visionTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        vision.color = new Color(255f, 255f, 255f, 255f);
    }

    // used to convert the dungeon tiles into utlima font characters for display as text that will be able to render the dungeon map
    public int [] ConvertDungeonMapToFont = { 0x20, 0x06, 0x05, 0x04, 0x24, 0x20, 0x20, 0x0F, 0x54, 0x46, 0x5E, 0x00, 0x0E, 0x0E, 0x02, 0x03 };

    string DisplayDungeonVisionSimple()
    {
        string dungString = "";
        byte[] dun = new byte[8];
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (x == u4.Party._x && y == u4.Party._y)
                {
                    dun[x] = 1;
                }
                else
                {
                    dun[x] = (byte)(ConvertDungeonMapToFont[((int)u4.tMap8x8x8[u4.Party._z][x, 7 - y]) >> 4]);
                }
            }
            dungString += u4.enc.GetString(dun, 0, 8) + "\n";
        }

        return dungString;
    }

    public int[,] DungeonVisionMap = new int[22, 22];
    public bool[,] DungeonVisionMapMarker = new bool[22, 22];
    public int[] DungeonVisionMapStack = new int[600];

    // TODO: the game engine uses a much more sophisticated map display to account for wrapping,
    // might want to consider this for the 3D dungeon also but this simple map display is sufficent for now
    string DisplayDungeonVision()
    {
        int last_check_X;
        int check_adjust_y;
        uint StackPointer;
        int check_adjust_x;
        int last_check_Y;
        int map_x;
        int map_y;

        // initialize dungeon vision array
        for (map_y = 0; map_y < 22; map_y++)
        {
            for (map_x = 0; map_x < 22; map_x++)
            {
                DungeonVisionMap[map_y, map_x] = 18; // blank character in font
                DungeonVisionMapMarker[map_y, map_x] = false; // all unset to start
            }
        }

        StackPointer = 0;
        map_y = 11;
        map_x = 11;
        DungeonVisionMapStack[StackPointer++] = map_y;
        DungeonVisionMapStack[StackPointer++] = map_x;
        DungeonVisionMapStack[StackPointer++] = u4.Party._x;
        DungeonVisionMapStack[StackPointer++] = 7 - u4.Party._y;
        DungeonVisionMapMarker[map_y - 1, map_x - 1] = true; // mark off starting position as checked
        DungeonVisionMap[map_y - 1, map_x - 1] = 1; // red dot for player

        do
        {
            last_check_Y = DungeonVisionMapStack[--StackPointer];
            last_check_X = DungeonVisionMapStack[--StackPointer];
            map_x = DungeonVisionMapStack[--StackPointer] - 1;
            map_y = DungeonVisionMapStack[--StackPointer] - 1;

            for (check_adjust_y = -1; check_adjust_y < 2; map_x++, map_y -= 3, check_adjust_y++)
            {
                for (check_adjust_x = -1; check_adjust_x < 2; map_y++, check_adjust_x++)
                {
                    if (((check_adjust_y != 0) | (check_adjust_x != 0)))
                    {
                        int check_x;
                        int check_y;

                        check_x = (check_adjust_y + last_check_Y) & 7;
                        check_y = (check_adjust_x + last_check_X) & 7;

                        if ((map_y < 1) ||
                            (map_y > 22) ||
                            (map_x < 1) ||
                            (map_x > 22) ||
                            DungeonVisionMapMarker[map_y - 1, map_x - 1])
                        {
                            continue;
                        }

                        // mark it on the map
                        DungeonVisionMap[map_y - 1, map_x - 1] = ConvertDungeonMapToFont[(int)u4.tMap8x8x8[u4.Party._z][check_y, check_x] >> 4];

                        if (((int)u4.tMap8x8x8[u4.Party._z][check_y, check_x] & 0xf0) != 0xf0)
                        {
                            DungeonVisionMapStack[StackPointer++] = map_y;
                            DungeonVisionMapStack[StackPointer++] = map_x;
                            DungeonVisionMapStack[StackPointer++] = check_y;
                            DungeonVisionMapStack[StackPointer++] = check_x;
                            if (StackPointer >= DungeonVisionMapStack.Length)
                            {
                                StackPointer = 0;
                            }

                            DungeonVisionMapMarker[map_y - 1, map_x - 1] = true;
                        }
                    }
                }
            }
        } while (StackPointer != 0);

        // construct strings to use the as text displayed in the ultima font which has dungeon map characters
        string dungString = "";
        byte[] dun = new byte[22];
        for (int y = 0; y < 22; y++)
        {
            for (int x = 0; x < 22; x++)
            {
                dun[x] = (byte)DungeonVisionMap[x, 21 - y];
            }

            dungString += u4.enc.GetString(dun, 0, 22) + "\n";
        }

        return dungString;
    }

    void UpdatePanelsText()
    {
        if (u4.gameText != null && GameText != null)
        {
            GameText.GetComponent<UnityEngine.UI.Text>().text = u4.gameText;
        }

        if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON) ||
            ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS)))
        {
            windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Level" + (char)(0x12) + (char)(u4.Party._z + '1') + (char)(0x11);
        }
        else
        {
            windDirection.GetComponent<UnityEngine.UI.Text>().text = (char)(0x10) + "Wind" + (char)(0x12);

            switch (u4.WindDir)
            {
                case U4_Decompiled_AVATAR.DIRECTION.NORTH:
                    windDirection.GetComponent<UnityEngine.UI.Text>().text += "North" + (char)(0x11);
                    break;
                case U4_Decompiled_AVATAR.DIRECTION.SOUTH:
                    windDirection.GetComponent<UnityEngine.UI.Text>().text += "South" + (char)(0x11);
                    break;
                case U4_Decompiled_AVATAR.DIRECTION.EAST:
                    windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "East" + (char)(0x11);
                    break;
                case U4_Decompiled_AVATAR.DIRECTION.WEST:
                    windDirection.GetComponent<UnityEngine.UI.Text>().text += (char)(0x12) + "West" + (char)(0x11);
                    break;
            }
        }
        moons.GetComponent<UnityEngine.UI.Text>().text = "" + (char)(0x10) + (char)(((u4.Party._trammel - 1) & 7) + 0x14) + (char)(0x12) + (char)(((u4.Party._felucca - 1) & 7) + 0x14) + (char)(0x11);

        System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

        statsOverview.GetComponent<UnityEngine.UI.Text>().text = "" + '\n';

        for (int i = 0; i < 8; i++)
        {
            if (i < u4.Party.f_1d8)
            {
                if (u4.Party.chara[i].highlight)
                {
                    statsOverview.GetComponent<UnityEngine.UI.Text>().text += GameFont.highlight((i + 1) + "-" + u4.Party.chara[i].name.PadRight(18, ' ') + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, ' ') + (char)(u4.Party.chara[i].state) + '\n');
                }
                else
                {
                    statsOverview.GetComponent<UnityEngine.UI.Text>().text += (i + 1) + "-" + u4.Party.chara[i].name.PadRight(18, ' ') + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, ' ') + (char)(u4.Party.chara[i].state) + '\n';
                }
            }
            else
            {
                statsOverview.GetComponent<UnityEngine.UI.Text>().text += '\n';
            }
        }

        string bottomStatus = "" + '\n' + ("Food:" + (int)(u4.Party._food / 100)).ToString().PadRight(12, ' ') + (char)(u4.spell_sta);

        if ((u4.Party._tile == Tile.TILE.SHIP_EAST) ||
            (u4.Party._tile == Tile.TILE.SHIP_WEST) ||
            (u4.Party._tile == Tile.TILE.SHIP_NORTH) ||
            (u4.Party._tile == Tile.TILE.SHIP_SOUTH))
        {
            bottomStatus += ("Ship:" + u4.Party._ship).PadLeft(12, ' ');
        }
        else
        {
            bottomStatus += ("Gold:" + u4.Party._gold).PadLeft(12, ' '); ;
        }

        statsOverview.GetComponent<UnityEngine.UI.Text>().text += bottomStatus;

        for (int i = 0; i < characterStatus.Length; i++)
        {
            if (i < u4.Party.f_1d8)
            {
                int classLength = u4.Party.chara[i].Class.ToString().Length;

                characterStatus[i].GetComponent<UnityEngine.UI.Text>().text = "" +
                    (char)(0x10) + u4.Party.chara[i].name + (char)(0x11) + '\n' +
                    (char)(u4.Party.chara[i].sex) + myTI.ToTitleCase(u4.Party.chara[i].Class.ToString().ToLower()).PadLeft(12 + classLength / 2, ' ').PadRight(23, ' ') + (char)u4.Party.chara[i].state + '\n' +
                    '\n' +
                    " MP:" + u4.Party.chara[i].magicPoints.ToString().PadLeft(2, '0').PadRight(14, ' ') + "LV:" + ((int)(u4.Party.chara[i].hitPointsMaximum / 100)).ToString().PadRight(4, ' ') + '\n' +
                    "STR:" + u4.Party.chara[i].strength.ToString().PadLeft(2, '0').PadRight(14, ' ') + "HP:" + u4.Party.chara[i].hitPoint.ToString().PadLeft(4, '0') + '\n' +
                    "DEX:" + u4.Party.chara[i].dexterity.ToString().PadLeft(2, '0').PadRight(14, ' ') + "HM:" + u4.Party.chara[i].hitPointsMaximum.ToString().PadLeft(4, '0') + '\n' +
                    "INT:" + u4.Party.chara[i].intelligence.ToString().PadLeft(2, '0').PadRight(14, ' ') + "EX:" + u4.Party.chara[i].experiencePoints.ToString().PadLeft(4, '0') + '\n' +
                    "W:" + myTI.ToTitleCase(u4.Party.chara[i].currentWeapon.ToString().Replace('_', ' ').ToLower()).PadRight(23, ' ') + '\n' +
                    "A:" + myTI.ToTitleCase(u4.Party.chara[i].currentArmor.ToString().Replace('_', ' ').ToLower()).PadRight(23, ' ') + '\n' +
                    bottomStatus;
            }
            else
            {
                characterStatus[i].GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\n" + bottomStatus;
            }
        }

        weaponsStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Weapons" + (char)(0x11) + '\n' +
            "A  -Hands   Cross Bow-I" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.CROSSBOW].ToString().PadLeft(2, '0') + '\n' +
            'B' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.STAFF].ToString().PadLeft(2, '0') + "-Staff Flaming Oil-J" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.FLAMING_OIL].ToString().PadLeft(2, '0') + '\n' +
            'C' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.DAGGER].ToString().PadLeft(2, '0') + "-Dagger    Halbert-K" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.HALBERD].ToString().PadLeft(2, '0') + '\n' +
            'D' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.SLING].ToString().PadLeft(2, '0') + "-Sling   Magic Axe-L" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_AXE].ToString().PadLeft(2, '0') + '\n' +
            'E' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MACE].ToString().PadLeft(2, '0') + "-Mace  Magic Sword-M" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_SWORD].ToString().PadLeft(2, '0') + '\n' +
            'F' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.AXE].ToString().PadLeft(2, '0') + "-Axe     Magic Bow-N" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_BOW].ToString().PadLeft(2, '0') + '\n' +
            'G' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.SWORD].ToString().PadLeft(2, '0') + "-Sword  Magic Wand-O" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MAGIC_WAND].ToString().PadLeft(2, '0') + '\n' +
            'H' + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.BOW].ToString().PadLeft(2, '0') + "-Bow  Mystic Sword-P" + u4.Party._weapons[(int)U4_Decompiled_AVATAR.WEAPON.MYSTIC_SWORD].ToString().PadLeft(2, '0') + '\n' +
            bottomStatus;

        armourStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Armour" + (char)(0x11) + '\n' +
            "A  " + "-No Armour".PadRight(22, ' ') + '\n' +
            'B' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.CLOTH].ToString().PadLeft(2, '0') + "-Clothing".PadRight(22, ' ') + '\n' +
            'C' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.LEATHER].ToString().PadLeft(2, '0') + "-Leather".PadRight(22, ' ') + '\n' +
            'D' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.CHAIN_MAIL].ToString().PadLeft(2, '0') + "-Chain Mail".PadRight(22, ' ') + '\n' +
            'E' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.PLATE_MAIL].ToString().PadLeft(2, '0') + "-Plate Mail".PadRight(22, ' ') + '\n' +
            'F' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MAGIC_CHAIN].ToString().PadLeft(2, '0') + "-Magic Chain Mail".PadRight(22, ' ') + '\n' +
            'G' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MAGIC_PLATE].ToString().PadLeft(2, '0') + "-Magic Plate Mail".PadRight(22, ' ') + '\n' +
            'H' + u4.Party._armors[(int)U4_Decompiled_AVATAR.ARMOR.MYSTIC_ROBE].ToString().PadLeft(2, '0') + "-Mystic Robe".PadRight(22, ' ') + '\n' +
            bottomStatus;

        reagentsStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Reagents" + (char)(0x11) + '\n' +
            'A' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.SULFER_ASH].ToString().PadLeft(2, '0') + "-Sulfer Ash".PadRight(22, ' ') + '\n' +
            'B' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.GINSENG].ToString().PadLeft(2, '0') + "-Ginseng".PadRight(22, ' ') + '\n' +
            'C' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.GARLIC].ToString().PadLeft(2, '0') + "-Galic".PadRight(22, ' ') + '\n' +
            'D' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.SPIDER_SILK].ToString().PadLeft(2, '0') + "-Spider Silk".PadRight(22, ' ') + '\n' +
            'E' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.BLOOD_MOSS].ToString().PadLeft(2, '0') + "-Blood Moss".PadRight(22, ' ') + '\n' +
            'F' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.BLACK_PEARL].ToString().PadLeft(2, '0') + "-Black Pearl".PadRight(22, ' ') + '\n' +
            'G' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.NIGHTSHADE].ToString().PadLeft(2, '0') + "-Nightshade".PadRight(22, ' ') + '\n' +
            'H' + u4.Party._reagents[(int)U4_Decompiled_AVATAR.REAGENT.MANDRAKE].ToString().PadLeft(2, '0') + "-Mandrake Root".PadRight(22, ' ') + '\n' +
            bottomStatus;

        mixturesStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Mixtures" + (char)(0x11) + '\n' +
            "Awak-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.AWAKEN].ToString().PadLeft(2, '0') + " IceBa-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.ICEBALLS].ToString().PadLeft(2, '0') + " Quick-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.QUICKNESS].ToString().PadLeft(2, '0') + '\n' +
            "Blin-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.BLINK].ToString().PadLeft(2, '0') + "  Jinx-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.JINX].ToString().PadLeft(2, '0') + "  Resu-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.RESURECTION].ToString().PadLeft(2, '0') + '\n' +
            "Cure-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.CURE].ToString().PadLeft(2, '0') + "  Kill-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.KILL].ToString().PadLeft(2, '0') + " Sleep-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.SLEEP].ToString().PadLeft(2, '0') + '\n' +
            "Disp-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.DISPELL].ToString().PadLeft(2, '0') + " Light-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.LIGHT].ToString().PadLeft(2, '0') + " Tremo-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.TREMOR].ToString().PadLeft(2, '0') + '\n' +
            "Eneg-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.ENERGY].ToString().PadLeft(2, '0') + " Missl-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.MAGIC_MISSLE].ToString().PadLeft(2, '0') + " Undea-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.UNDEAD].ToString().PadLeft(2, '0') + '\n' +
            "Fire-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.FIREBALL].ToString().PadLeft(2, '0') + " Negat-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.NEGATE].ToString().PadLeft(2, '0') + "  View-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.VIEW].ToString().PadLeft(2, '0') + '\n' +
            "Gate-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.GATE].ToString().PadLeft(2, '0') + "  Open-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.OPEN].ToString().PadLeft(2, '0') + " Winds-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.WINDS].ToString().PadLeft(2, '0') + '\n' +
            "Heal-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.HEAL].ToString().PadLeft(2, '0') + " Prote-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.PROTECT].ToString().PadLeft(2, '0') + "  X-It-" + u4.Party._mixtures[(int)U4_Decompiled_AVATAR.MIXTURES.X_IT].ToString().PadLeft(2, '0') + '\n' +
            bottomStatus;

        equipmentStatus.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Equipment" + (char)(0x11) + '\n' +
            'A' + u4.Party._torches.ToString().PadLeft(2, '0') + "-Torches".PadRight(22, ' ') + '\n' +
            'B' + u4.Party._gems.ToString().PadLeft(2, '0') + "-Gems".PadRight(22, ' ') + '\n' +
            'C' + u4.Party._keys.ToString().PadLeft(2, '0') + "-Keys".PadRight(22, ' ') + '\n' +
            'D' + u4.Party._sextants.ToString().PadLeft(2, '0') + "-Sextants".PadRight(22, ' ') + "\n\n\n\n\n" +
            bottomStatus;

        itemsStatus.GetComponent<UnityEngine.UI.Text>().text = "" + '\n' + "Stones: ";

        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.BLUE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=blue>Bl</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.YELLOW))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=yellow>Ye</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.RED))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=red>Re</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.GREEN))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=green>Gr</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.ORANGE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=orange>Or</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.PURPLE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=purple>Pu</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.WHITE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=white>Wh</color>";
        }
        if (u4.Party.mStones.HasFlag(U4_Decompiled_AVATAR.STONES.BLACK))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "<color=grey>Bl</color>";
        }

        itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' + "Runes: ";

        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HONOR))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Honor ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.COMPASSION))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Compassion ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.VALOR))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Valor ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.JUSTICE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Justice ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HUMILITY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Humility ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.HONESTY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Honesty ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.SPIRITUALITY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Spirituality ";
        }
        if (u4.Party.mRunes.HasFlag(U4_Decompiled_AVATAR.RUNES.SACRIFICE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Sacrifice";
        }

        itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' +  "Items: ";

        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.BELL))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Bell ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.BOOK))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Book ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.WHEEL))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Wheel ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.HORN))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Horn ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.CANDLE))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Candle ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.SKULL))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Skull ";
        }

        itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "" + '\n' + "Key:";

        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.LOVE_KEY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Love ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.TRUTH_KEY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Truth ";
        }
        if (u4.Party.mItems.HasFlag(U4_Decompiled_AVATAR.ITEMS.COMPASSION_KEY))
        {
            itemsStatus.GetComponent<UnityEngine.UI.Text>().text += "Compassion";
        }

        itemsStatusHeading.GetComponent<UnityEngine.UI.Text>().text = "" +
            (char)(0x10) + "Items" + (char)(0x11) + "\n\n\n\n\n\n\n\n\n" +
            bottomStatus;

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.CHARACTER_OVERVIEW)
        {
            statsOverview.SetActive(true);
        }
        else
        {
            statsOverview.SetActive(false);
        }
        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.CHARACTER_DETAIL)
        {
            for (int i = 0; i < 8; i++)
            {
                if (u4.zstats_character == i)
                {
                    characterStatus[i].SetActive(true);
                }
                else
                {
                    characterStatus[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                characterStatus[i].SetActive(false);
            }
        }

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.WEAPONS)
        {
            weaponsStatus.SetActive(true);
        }
        else
        {
            weaponsStatus.SetActive(false);
        }
        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.ARMOUR)
        {
            armourStatus.SetActive(true);
        }
        else
        {
            armourStatus.SetActive(false);
        }

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.EQUIPMENT)
        {
            equipmentStatus.SetActive(true);
        }
        else
        {
            equipmentStatus.SetActive(false);
        }

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.ITEMS)
        {
            itemsStatus.SetActive(true);
        }
        else
        {
            itemsStatus.SetActive(false);
        }

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.MIXTURES)
        {
            mixturesStatus.SetActive(true);
        }
        else
        {
            mixturesStatus.SetActive(false);
        }

        if (u4.zstats_mode == U4_Decompiled_AVATAR.ZSTATS_MODE.REAGENTS)
        {
            reagentsStatus.SetActive(true);
        }
        else
        {
            reagentsStatus.SetActive(false);
        }

        if (lastVisionFilename != u4.visionFilename)
        {
            if (u4.visionFilename.Length > 0)
            {
                // the game engine uses the lower quality .pic files, we will load the higher quality .EGA ones instead
                Picture.LoadAVATAREGAFile(u4.visionFilename.Replace(".pic", ".EGA"), visionTexture);
                vision.sprite = Sprite.Create(visionTexture, new Rect(0.0f, 0.0f, visionTexture.width, visionTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                vision.color = new Color(255f, 255f, 255f, 255f);

                lastVisionFilename = u4.visionFilename;
            }
            else
            {
                vision.sprite = null;
                vision.color = new Color(0f, 0f, 0f, 0f);
                Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            }
        }
    }

    public bool changeTiles = false;

    // Update is called once per frame
    void Update()
    {
        if (changeTiles == true)
        {
            currentTileType = (Tile.TILE_TYPE)(((int)currentTileType + 1) % ((int)Tile.TILE_TYPE.MAX));

            Tile.LoadTiles(currentTileType);
            currentTileType = Tile.currentTileType;
            SetupTiles();

            Combat.CreateCombatTerrains(Outdoor.outdoorMap.GetLength(1));

            // force an update
            lastModeCheck = (U4_Decompiled_AVATAR.MODE)(-1);

            // reset flag
            changeTiles = false;
        }

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

            // handle all input panel state updates
            UpdateInputPanelState();
            UpdatePanelsText();

            // did we just change modes, used by UpdateOutdoors() for automatic enter
            if (lastMode != u4.current_mode)
            {
                // did we just come out of somewhere to the outdoors
                if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
                {
                    // set flag because we were just inside so we don't try to enter something right away again
                    readyToAutomaticallyEnter = false;
                }

                // are we currently indoors
                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.SHRINE))
                {
                    // set flag because we are inside so we don't try to enter something
                    readyToAutomaticallyEnter = false;
                }

                // update last mode
                lastMode = u4.current_mode;
            }

            if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
            {
                UpdateOutdoors();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
            {
                UpdateSettlement();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
            {
                UpdateCombat();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM) /* this is a dungeon room */
            {
                UpdateDungeonRoom();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
            {
                UpdateDungeon();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SHRINE)
            {
                UpdateShrine();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP)
            {
                UpdateCombatCamp();
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.VISION)
            {
                UpdateVision();
            }

            // keep the sky game objects in sync with the game
            if (skyGameObject)
            {
                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS) || (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT))
                {
                    skyGameObject.transform.localPosition = new Vector3(u4.Party._x, 0, 255 - u4.Party._y);
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
                {
                    skyGameObject.transform.localPosition = new Vector3(u4.currentActiveCharacter.x, 0, 255 - u4.currentActiveCharacter.y);
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP)
                {
                    if (u4.currentActiveCharacter.active)
                    {
                        skyGameObject.transform.localPosition = new Vector3(u4.currentActiveCharacter.x, 0, 255 - u4.currentActiveCharacter.y);
                    }
                    else
                    {
                        skyGameObject.transform.localPosition = Combat.CenterOfCombatTerrain.transform.localPosition;
                    }
                }
            }

            if ((party != null) && (Tile.originalTiles != null))
            {
                // set the party tile, person, horse, ballon, ship, etc.
                Renderer renderer = party.GetComponentInChildren<Renderer>();
                if (renderer)
                {
                    renderer.material.mainTexture = Tile.expandedTiles[(int)u4.Party._tile];
                    renderer.material.mainTextureOffset = new Vector2((float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.width, (float)Tile.TILE_BORDER_SIZE / (float)renderer.material.mainTexture.height);
                    renderer.material.mainTextureScale = new Vector2((float)(renderer.material.mainTexture.width - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.width, (float)(renderer.material.mainTexture.height - (2 * Tile.TILE_BORDER_SIZE)) / (float)renderer.material.mainTexture.height);

                    party.name = u4.Party._tile.ToString();

                    if ((u4.Party._tile == Tile.TILE.BALOON) && (u4.Party.f_1dc == 1))
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 1, party.transform.position.z);
                    }
                    else
                    {
                        party.transform.position = new Vector3(party.transform.position.x, 0, party.transform.position.z);
                    }
                }
            }

            // keep the party game object in sync with the game
            if (partyGameObject)
            {
                if ((u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT) ||
                    (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP))
                {
                    partyGameObject.transform.localPosition = new Vector3(u4.Party._x, 255 - u4.Party._y, 0);
                    if (Camera.main.transform.eulerAngles.y != 0)
                    {
                        if (rotateTransform)
                        {
                            if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.WEST && rotateTransform.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 270, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.NORTH && rotateTransform.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.eulerAngles.x, 0, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.EAST && rotateTransform.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 90, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.SOUTH && rotateTransform.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 180, rotateTransform.transform.eulerAngles.z);
                            }
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT)
                {
                    if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                    {
                        // if we are going to do rotation then we need to adjust the directional controls when in combat in the dungeon also
                        if (rotateTransform)
                        {
                            if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && rotateTransform.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 270, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && rotateTransform.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 0, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && rotateTransform.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 90, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && rotateTransform.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 180, rotateTransform.transform.eulerAngles.z);
                            }
                        }
                    }
                    else
                    {
                        if (rotateTransform)
                        {
                            if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.WEST && rotateTransform.transform.eulerAngles.y != 270)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 270, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.NORTH && rotateTransform.transform.eulerAngles.y != 0)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 0, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.EAST && rotateTransform.transform.eulerAngles.y != 90)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 90, rotateTransform.transform.eulerAngles.z);
                            }
                            else if (u4.surface_party_direction == U4_Decompiled_AVATAR.DIRECTION.SOUTH && rotateTransform.transform.eulerAngles.y != 180)
                            {
                                rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 180, rotateTransform.transform.eulerAngles.z);
                            }
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM)
                {
                    if (rotateTransform)
                    {
                        if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && rotateTransform.transform.eulerAngles.y != 270)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 270, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && rotateTransform.transform.eulerAngles.y != 0)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 0, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && rotateTransform.transform.eulerAngles.y != 90)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 90, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && rotateTransform.transform.eulerAngles.y != 180)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 180, rotateTransform.transform.eulerAngles.z);
                        }
                    }
                }
                else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.DUNGEON)
                {
                    partyGameObject.transform.localPosition = new Vector3(u4.Party._x * 11 + 5, (7 - u4.Party._y) * 11 + 5, 0);
                    if (rotateTransform)
                    {
                        if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.WEST && rotateTransform.transform.eulerAngles.y != 270)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 270, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.NORTH && rotateTransform.transform.eulerAngles.y != 0)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 0, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.EAST && rotateTransform.transform.eulerAngles.y != 90)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 90, rotateTransform.transform.eulerAngles.z);
                        }
                        else if (u4.Party._dir == U4_Decompiled_AVATAR.DIRECTION.SOUTH && rotateTransform.transform.eulerAngles.y != 180)
                        {
                            rotateTransform.eulerAngles = new Vector3(rotateTransform.transform.eulerAngles.x, 180, rotateTransform.transform.eulerAngles.z);
                        }
                    }
                }
            }

            // update moons
            trammelLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(trammelLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1665 * (360f / 256f), Time.deltaTime), 0f);
            feluccaLight.GetComponent<Light>().transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(feluccaLight.GetComponent<Light>().transform.eulerAngles.y, 180f - (float)u4.D_1666 * (360f / 256f), Time.deltaTime), 0f);
        }

        Settlement.SETTLEMENT settlement;

        // get the current settlement, need to special case BRITANNIA as the castle has two levels, use the ladder to determine which level
        if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
        {
            settlement = Settlement.SETTLEMENT.LCB_1;
        }
        else
        {
            settlement = (Settlement.SETTLEMENT)u4.Party._loc;
        }

        // assume no settlement map changes
        bool settlementMapChanged = false;

        // check if we are in a settlement
        if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
        {
            // check for any changes in the entire settlement map, this could be because the player has jimmy'd a lock, opened a door or dispelled a field in a settlement
            // dispelling a field will copy current tile under the player to the map, so if you dispell from a tree tile it will replace the field with a tree
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    // get the current tile from the game engine
                    Tile.TILE currentTileIndex = u4.tMap32x32[x, y];

                    // check if we have already seen this change
                    if (Settlement.settlementsMapGameObjects[(int)settlement][x, y] != Map.allMapTilesGameObjects[(int)currentTileIndex])
                    {
                        // update the tile game object to match the current map tile
                        Settlement.settlementsMapGameObjects[(int)settlement][x, y] = Map.allMapTilesGameObjects[(int)currentTileIndex];

                        // indicate below that we need to redo the raycast
                        settlementMapChanged = true;
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

        // we've moved, regenerate the raycast, TODO NPCs can also affect the raycast when moving, need to check them also or redo raycast more often
        if ((u4.current_mode != lastModeCheck) ||
            (u4.Party._x != lastRaycastPlayer_posx) || // player moved
            (u4.Party._y != lastRaycastPlayer_posy) || // player moved
            (u4.Party.f_1dc != lastRaycastPlayer_f_1dc) || // balloon flying or grounded or dungeon torch active
            ((u4.open_door_timer > 0) != last_door_timer) || // door has opened or closed
            (u4.surface_party_direction != lastRaycastP_surface_party_direction) || // have we rotated the camera
            (u4.Party._dir != lastPartyDirection) ||  // have we rotated in the dungeon
            (settlement != lastSettlement) || // did we change settlements
            settlementMapChanged) // did the settlement map change (going from LCB1 to LCB2)
        {
            Vector3 location = Vector3.zero;

            // if we werejust in a vision, clear it out
            if (lastModeCheck == U4_Decompiled_AVATAR.MODE.VISION)
            {
                Picture.ClearTexture(visionTexture, Palette.EGAColorPalette[(int)Palette.EGA_COLOR.BLACK]);
            }

            lastModeCheck = u4.current_mode;
            lastSettlement = settlement; // update the settlement if it changed
            lastRaycastPlayer_posx = u4.Party._x; // update the last raycast position
            lastRaycastPlayer_posy = u4.Party._y;
            lastRaycastP_surface_party_direction = u4.surface_party_direction; // update the direction
            lastRaycastPlayer_f_1dc = u4.Party.f_1dc; // flying in the balloon or not or dungeon torch active
            last_door_timer = u4.open_door_timer > 0; // update the door timer
            lastPartyDirection = u4.Party._dir; // update the direction

            // check if we are outdoors
            if (u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS)
            {
                int offset_x = u4.Party._x - raycastOutdoorMap.GetLength(0) / 2 - 1;
                int offset_y = u4.Party._y - raycastOutdoorMap.GetLength(1) / 2 - 1;

                if (u4.Party.f_1dc == 0)
                {
                    // generate a new raycast
                    Map.raycast(ref Outdoor.outdoorMap,
                        u4.Party._x,
                        u4.Party._y,
                        ref raycastOutdoorMap,
                        offset_x,
                        offset_y,
                        Tile.TILE.BLANK);
                }
                else
                {
                    // just copy the entire map (no raycast) so it is visible when you are flying in the balloon
                    for (int y = 0; y < raycastOutdoorMap.GetLength(1); y++)
                    {
                        for (int x = 0; x < raycastOutdoorMap.GetLength(0); x++)
                        {
                            raycastOutdoorMap[x, y] = Outdoor.outdoorMap[(x + offset_x + Outdoor.outdoorMap.GetLength(0)) % Outdoor.outdoorMap.GetLength(0), (y + offset_y + Outdoor.outdoorMap.GetLength(1)) % Outdoor.outdoorMap.GetLength(1)];
                        }
                    }
                }

                // recreate the gameobjects and meshes
                Combine.Combine3(mainTerrain,
                    ref raycastOutdoorMap,
                    offset_x,
                    offset_y,
                    ref entireMapGameObjects,
                    false,
                    TextureFormat.RGBA32,
                    true,
                    Tile.combinedExpandedMaterial,
                    Tile.combinedLinearMaterial,
                    u4.Party._x,
                    255 - u4.Party._y,
                    u4.surface_party_direction);

                location = Vector3.zero;
            }
            // check if we are in a settlement
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
            {
                int offset_x = u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1;
                int offset_y = u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1;

                // generate a new raycast based on game engine map
                Map.raycast(ref u4.tMap32x32, 
                    u4.Party._x, 
                    u4.Party._y, 
                    ref raycastSettlementMap,
                    offset_x,
                    offset_y, 
                    Tile.TILE.GRASS);

                // recreate the gameobjects and meshes
                Combine.Combine3(mainTerrain,
                    ref raycastSettlementMap,
                    offset_x,
                    offset_y,
                    ref Settlement.settlementsMapGameObjects[(int)settlement],
                    false,
                    TextureFormat.RGBA32,
                    false,
                    Tile.combinedExpandedMaterial,
                    Tile.combinedLinearMaterial,
                    u4.Party._x,
                    31 - u4.Party._y,
                    u4.surface_party_direction);

                location = new Vector3(0, 0, 224);

                // TODO: implement label, so they appear upright and in letter/word order when you move around the settlement map
                /*
                CreateMapLabels(mainTerrain, ref raycastSettlementMap);

                CreateMap(mainTerrain, raycastSettlementMap);

                location = new Vector3(
                    ((u4.Party._x - raycastSettlementMap.GetLength(0) / 2 - 1) ) , 0,
                    entireMapTILEs.GetLength(1) - ((u4.Party._y - raycastSettlementMap.GetLength(1) / 2 - 1) )  - raycastSettlementMap.GetLength(1));
                */
            }
            else if (u4.current_mode == U4_Decompiled_AVATAR.MODE.VISION)
            {
                if (u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.OUTDOORS)
                {
                    // we are in a outdoors, show an outdoor vision
                    vision.transform.gameObject.SetActive(true);
                    DungeonMapText.transform.gameObject.SetActive(false);
                    DisplayMapVision();
                }
                else if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
                {
                    // we are in a dungeon, show a dungeon vision
                    vision.transform.gameObject.SetActive(false);
                    DungeonMapText.transform.gameObject.SetActive(true);
                    DungeonMapText.text = DisplayDungeonVision();
                }
                else if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.COVE))
                {
                    // check which castle level we are on first
                    if ((u4.Party._loc == U4_Decompiled_AVATAR.LOCATIONS.BRITANNIA) && (u4.tMap32x32[3, 3] == Tile.TILE.LADDER_UP))
                    {
                        settlement = Settlement.SETTLEMENT.LCB_1;
                    }
                    else
                    {
                        settlement = (Settlement.SETTLEMENT)u4.Party._loc;
                    }

                    // we are in a settlement, show a settlement vision
                    vision.transform.gameObject.SetActive(true);
                    DungeonMapText.transform.gameObject.SetActive(false);
                    DisplayMapVision();
                }
                else if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.HONESTY) && (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.HUMILITY))
                {
                    // we are in a shrine
                    // TODO: should move the shrine stuff into same place as here so we handle all the vision stuff in one place
                    vision.transform.gameObject.SetActive(true);
                    DungeonMapText.transform.gameObject.SetActive(false);
                }
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
}
