using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenInvertVR : MonoBehaviour
{
    public Shader negativeShader;
    private Material negativeMaterial;

    void OnEnable()
    {
        if (!negativeShader)
        {
            Debug.LogError("Shader not found.");
            this.enabled = false;
            return;
        }

        negativeMaterial = new Material(negativeShader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (negativeMaterial != null)
        {
            Graphics.Blit(source, destination, negativeMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void OnDisable()
    {
        if (negativeMaterial)
        {
            DestroyImmediate(negativeMaterial);
            negativeMaterial = null;
        }
    }
}
