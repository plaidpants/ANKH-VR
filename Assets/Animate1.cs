using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate1 : MonoBehaviour
{
    public float animationPeriod = 0.3f;
    MeshRenderer waterRenderer;

    // Update is called once per frame
    void Update()
    {
        // Get the mesh renderer
        if (waterRenderer == null)
        {
            waterRenderer = transform.GetComponent<MeshRenderer>();
        }
        // if it exists then animate the UV
        else
        {
            waterRenderer.material.mainTextureOffset = new Vector2(waterRenderer.material.mainTextureOffset.x, Time.time * animationPeriod % 1.0f);
        }
    }
}
