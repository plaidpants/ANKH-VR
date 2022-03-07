using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pyramid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent< MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(1, 1, 0);
        Vector3 p3 = new Vector3(0, 1, 0);
        Vector3 p4 = new Vector3(0.5f, 0.5f, 1.0f / Mathf.Sqrt(2));

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[] { p0, p1, p2, p3, p4 };
        mesh.triangles = new int[]{
            4,0,1,
            4,1,2,
            4,2,3,
            4,3,0
        };

        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].y, mesh.vertices[i].x);
        }

        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
