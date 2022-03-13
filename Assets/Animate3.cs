using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate3 : MonoBehaviour
{
    public World world;
    public U4_Decompiled.TILE npcTile;
    public U4_Decompiled.TILE[] animationFrameIndexes;
    int frame = 0;
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float animationPeriod = 0.2f;
    public MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetNPCTile(U4_Decompiled.TILE tile)
    {
        // check if this npc tile is already set correctly
        if (npcTile == tile)
        {
            // already set
            return;
        }

        // update the tile index
        npcTile = tile;

        // set to the first animation frame
        frame = 0;

        // the npc has two animation tiles in these tile ranges
        if (((int)npcTile >= 32 && (int)npcTile <= 47) || ((int)npcTile >= 80 && (int)npcTile <= 95) || ((int)npcTile >= 132 && (int)npcTile <= 143))
        {
            // update to the initial animation texture frame
            renderer.material.mainTexture = world.tiles[(int)npcTile];

            // set the other frames based on the intial frame
            if (((int)npcTile % 2) == 0)
            {
                animationFrameIndexes = new U4_Decompiled.TILE[2] { npcTile, npcTile + 1 }; 
            }
            else
            {
                animationFrameIndexes = new U4_Decompiled.TILE[2] { npcTile, npcTile - 1 };
            }
        }
        // the npc has four animation tiles in this tile range
        else if ((int)npcTile >= 144 && (int)npcTile <= 255)
        {
            // update to the initial animation texture frame
            renderer.material.mainTexture = world.tiles[(int)npcTile];
            transform.gameObject.name = npcTile.ToString();

            // set the other frames based on the intial frame
            if      (((int)npcTile % 4) == 0)
            {
                animationFrameIndexes = new U4_Decompiled.TILE[4] { npcTile, npcTile + 1, npcTile + 2, npcTile + 3 };
            }
            else if (((int)npcTile % 4) == 1)
            {
                animationFrameIndexes = new U4_Decompiled.TILE[4] { npcTile, npcTile + 1, npcTile + 2, npcTile - 1 };
            }
            else if (((int)npcTile % 4) == 2)
            {
                animationFrameIndexes = new U4_Decompiled.TILE[4] { npcTile, npcTile + 1, npcTile - 2, npcTile - 1 };
            }
            else if (((int)npcTile % 4) == 3)
            {
                animationFrameIndexes = new U4_Decompiled.TILE[4] { npcTile, npcTile -3, npcTile - 2, npcTile - 1 };
            }
            else
            {
                // should never get here
                animationFrameIndexes = new U4_Decompiled.TILE[4] { npcTile, npcTile + 1, npcTile + 2, npcTile + 3 };
            }
        }
        // npc does not have any animation tiles
        else
        {
            // update to the animation texture frame
            renderer.material.mainTexture = world.tiles[(int)npcTile];

            // add only one frame to the animation
            animationFrameIndexes = new U4_Decompiled.TILE[1] { npcTile };
        }

        // check if this one is disabled
        if (npcTile != 0)
        {
            // enable if not enabled
            if (transform.gameObject.activeSelf == false)
            {
                transform.gameObject.SetActive(true);
            }
        }
        else if (transform.gameObject.activeSelf == true)
        {
            // disable if not disabled
            transform.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check a bit before using these
        if (animationFrameIndexes == null)
            return;

        if (world == null)
            return;

        if (world.tiles == null)
            return;

        if (world.tiles.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer > timerExpired)
        {
            frame++;
            if (frame >= animationFrameIndexes.Length)
            {
                frame = 0;
            }

            renderer.material.mainTexture = world.tiles[(int)animationFrameIndexes[frame]];

            timer -= timerExpired;
            timerExpired = animationPeriod;
        }
    }
}