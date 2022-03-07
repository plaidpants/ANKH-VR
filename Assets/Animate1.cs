using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate1 : MonoBehaviour
{
    public float animationPeriod = 0.3f;

    // Update is called once per frame
    void Update()
    {
        // Get the mesh renderer
        MeshRenderer waterRenderer = transform.GetComponent<MeshRenderer>();

        // if it exists then animate the UV
        if (waterRenderer)
        {
            waterRenderer.material.mainTextureOffset = new Vector2(0.0f, Time.time * animationPeriod % 1.0f);
        }
    }
}
