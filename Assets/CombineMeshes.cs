using UnityEngine;
using System.Collections;

public class CombineMeshes : MonoBehaviour
{
    void UpdateMesh()
    {
        // save current position so we can set it to zero so the localToWorldMatrix works correctly
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.GetComponent<Renderer>.enabled = false;
            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        transform.gameObject.SetActive(true);

        //restore position
        transform.position = position;
        transform.rotation = rotation;
    }

    // Use this for initialization
    void Start ()
    {
        UpdateMesh();
    }
}
