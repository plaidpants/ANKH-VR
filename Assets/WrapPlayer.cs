using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapPlayer : MonoBehaviour
{
    public Terrain wrappingTerrain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position;

        // If the player exceeds the right side of the screen.
        if (transform.position.x > wrappingTerrain.transform.position.x + wrappingTerrain.terrainData.size.x)
        {
            newPos.x = transform.position.x - wrappingTerrain.terrainData.size.x;
        }
        // If the player exceeds the left side of the screen.
        else if (transform.position.x < wrappingTerrain.transform.position.x)
        {
            newPos.x = wrappingTerrain.transform.position.x + wrappingTerrain.terrainData.size.x;
        }

        // If the player exceeds the right side of the screen.
        if (transform.position.z > wrappingTerrain.transform.position.z + wrappingTerrain.terrainData.size.z)
        {
            newPos.z = transform.position.z - wrappingTerrain.terrainData.size.z;
        }
        // If the player exceeds the left side of the screen.
        else if (transform.position.z < wrappingTerrain.transform.position.z)
        {
            newPos.z = wrappingTerrain.transform.position.z + wrappingTerrain.terrainData.size.z;
        }

        transform.position = newPos;
    }
}
