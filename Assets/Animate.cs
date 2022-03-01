using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TilemapSystem3D;

public class Animate : MonoBehaviour
{
    public TilePalette tiles;
    public int[] animationFrameIndexes;
    int frame = 0;
    float timer = 0.0f;
    float timerExpired = 0.0f;
    public float animationPeriod = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timerExpired)
        {
            frame++;
            if (frame >= animationFrameIndexes.Length)
            {
                frame = 0;
            }
            //GetComponent<MeshRenderer>().material.mainTexture = animationFrames[frame];
            GetComponent<MeshRenderer>().material.mainTexture = tiles.temporaryTileTextureArray[animationFrameIndexes[frame]];
            timer = timer - timerExpired;
            timerExpired = animationPeriod;
        }
    }
}
