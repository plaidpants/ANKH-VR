using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingTerrains : MonoBehaviour
{
    public Terrain terrainObject;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x + terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z + terrainObject.terrainData.size.z), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x + terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z - terrainObject.terrainData.size.z), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x - terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z + terrainObject.terrainData.size.z), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x - terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z - terrainObject.terrainData.size.z), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x + terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z                                   ), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x                                   , terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z + terrainObject.terrainData.size.z), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x - terrainObject.terrainData.size.x, terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z                                   ), Quaternion.identity);
        Instantiate(terrainObject, new Vector3(terrainObject.transform.localPosition.x                                   , terrainObject.transform.localPosition.y, terrainObject.transform.localPosition.z - terrainObject.terrainData.size.z), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
