using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate2 : MonoBehaviour
{
    // used to check if NPC matches
    public int tile = 0;

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
            // need at least two to animate
            if (transform.childCount > 1)
            {
                // disable the current one
                transform.GetChild(frame).gameObject.SetActive(false);

                frame++;
                if (frame >= transform.childCount)
                {
                    frame = 0;
                }

                // enable the new one
                transform.GetChild(frame).gameObject.SetActive(true);

                timer = timer - timerExpired;
                timerExpired = animationPeriod;
            }
        }
    }
}
