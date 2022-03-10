using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate3 : MonoBehaviour
{
    public World world;
    public int npcTile;
    public int[] animationFrameIndexes;
    int frame = 0;
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float animationPeriod = 0.2f;
    public MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetNPCTile(int tile)
    {
        // check if this npc tile is already set correctly
        if (npcTile == tile)
        {
            // already set
            return;
        }

        if (tile > 255)
        {
            Debug.Log("tile ?? " + tile);
        }

        // update the tile index
        npcTile = tile;

        // set to the first animation frame
        frame = 0;

        // the npc has two animation tiles in these tile ranges
        if ((npcTile >= 32 && npcTile <= 47) || (npcTile >= 80 && npcTile <= 95) || (npcTile >= 132 && npcTile <= 143))
        {
            // update to the initial animation texture frame
            renderer.material.mainTexture = world.tiles[npcTile];

            // set the other frames based on the intial frame
            if ((npcTile % 2) == 0)
            {
                animationFrameIndexes = new int[2] { npcTile, npcTile + 1 }; 
            }
            else
            {
                animationFrameIndexes = new int[2] { npcTile, npcTile - 1 };
            }
        }
        // the npc has four animation tiles in this tile range
        else if (npcTile >= 144 && npcTile <= 255)
        {
            // update to the initial animation texture frame
            renderer.material.mainTexture = world.tiles[npcTile];

            // set the other frames based on the intial frame
            if      ((npcTile % 4) == 0)
            {
                animationFrameIndexes = new int[4] { npcTile, npcTile + 1, npcTile + 2, npcTile + 3 };
            }
            else if ((npcTile % 4) == 1)
            {
                animationFrameIndexes = new int[4] { npcTile, npcTile + 1, npcTile + 2, npcTile - 1 };
            }
            else if ((npcTile % 4) == 2)
            {
                animationFrameIndexes = new int[4] { npcTile, npcTile + 1, npcTile - 2, npcTile - 1 };
            }
            else if ((npcTile % 4) == 3)
            {
                animationFrameIndexes = new int[4] { npcTile, npcTile -3, npcTile - 2, npcTile - 1 };
            }
            else
            {
                // should never get here
                animationFrameIndexes = new int[4] { npcTile, npcTile + 1, npcTile + 2, npcTile + 3 };
            }
        }
        // npc does not have any animation tiles
        else
        {
            // update to the animation texture frame
            renderer.material.mainTexture = world.tiles[npcTile];

            // add only one frame to the animation
            animationFrameIndexes = new int[1] { npcTile };
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

            renderer.material.mainTexture = world.tiles[animationFrameIndexes[frame]];

            timer -= timerExpired;
            timerExpired = animationPeriod;
        }
    }
}
